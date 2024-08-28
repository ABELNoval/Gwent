using System;
using System.Collections.Generic;
using System.Diagnostics;
using Gwent_Proyect.Assets.Scripts.Compilator;
using Unity.VisualScripting;

namespace Console
{

    public abstract class ExpressionNode
    {
        public abstract object Evaluate(Context context, List<Cards> target);
    }

    //BinaryExpression
    public class BinaryExpressionNode : ExpressionNode
    {
        public Token Operator { get; }
        public ExpressionNode left { get; set; }
        public ExpressionNode right { get; set; }
        public override void SetProperty(ExpressionNode property) { }

        public BinaryExpressionNode(ExpressionNode left, Token Operator, ExpressionNode right)
        {
            this.Operator = Operator;
            this.left = left;
            this.right = right;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            return Operator.type switch
            {
                TokenType.Plus => (int)left.Evaluate(context, target) + (int)right.Evaluate(context, target),
                TokenType.Minus => (int)left.Evaluate(context, target) - (int)right.Evaluate(context, target),
                TokenType.Multiply => (int)left.Evaluate(context, target) * (int)right.Evaluate(context, target),
                TokenType.Divide => (int)left.Evaluate(context, target) / (int)right.Evaluate(context, target),
                TokenType.Exponent => (int)Math.Pow((int)left.Evaluate(context, target), (int)right.Evaluate(context, target)),
                TokenType.Equals => left.Evaluate(context, target) == right.Evaluate(context, target),
                TokenType.NotEquals => left.Evaluate(context, target) != right.Evaluate(context, target),
                TokenType.LessThan => (int)left.Evaluate(context, target) < (int)right.Evaluate(context, target),
                TokenType.GreaterThan => (int)left.Evaluate(context, target) > (int)right.Evaluate(context, target),
                TokenType.LessThanOrEqual => (int)left.Evaluate(context, target) <= (int)right.Evaluate(context, target),
                TokenType.GreaterThanOrEqual => (int)left.Evaluate(context, target) >= (int)right.Evaluate(context, target),
                TokenType.LogicalAnd => (bool)left.Evaluate(context, target) && (bool)right.Evaluate(context, target),
                TokenType.LogicalOr => (bool)left.Evaluate(context, target) || (bool)right.Evaluate(context, target),
                TokenType.Dot => (IdentifierNode)left.Evaluate(context, target),
                _ => throw new Exception("Operador no valido"),
            };

        }
    }



    public abstract class List : ExpressionNode
    {
        public Token accessToken;
        public GameComponent gameComponent;

        public List(Token accesToken)
        {
            this.accessToken = accesToken;
        }
    }

    // List of cards on the board
    public class BoardList : List
    {
        public BoardList(ExpressionNode context, Token accessToken) : base(accessToken)
        {
            this.context = context;
        }
        public ExpressionNode context;
        public override object Evaluate(Context context, List<Cards> targets)
        {
            return context.board.cards;
        }
    }

    // Abstract class for lists specific to a player
    public abstract class IndividualList : List
    {
        //This field isn't used in the evaluation method, it is only for the semnatic check
        //This is why in cases where a semantic check isn't needed it will have null value
        public ExpressionNode context;
        public Token playertoken;
        public ExpressionNode player;
        public IndividualList(ExpressionNode context, ExpressionNode player, Token accessToken, Token playertoken) : base(accessToken)
        {
            this.context = context;
            this.player = player;
            this.playertoken = playertoken;
        }
    }

    // List of cards in a player's hand
    public class HandList : IndividualList
    {
        public HandList(ExpressionNode context, ExpressionNode player, Token accessToken, Token playertoken) : base(context, player, accessToken, playertoken) { }

        public override object Evaluate(Context context, List<Card> targets)
        {
            Player targetPlayer = (Player)player.Evaluate(context, targets);
            gameComponent = GlobalContext.Hand(targetPlayer);
            return gameComponent.cards;
        }
    }

    // List of cards in a player's deck
    public class DeckList : IndividualList
    {
        public DeckList(ExpressionNode context, ExpressionNode player, Token accessToken, Token playertoken) : base(context, player, accessToken, playertoken) { }

        public override object Evaluate(Context context, List<Card> targets)
        {
            Player targetPlayer = (Player)player.Evaluate(context, targets);
            gameComponent = GlobalContext.Deck(targetPlayer);
            return gameComponent.cards;
        }
    }

    // List of cards in a player's graveyard
    public class GraveyardList : IndividualList
    {
        public GraveyardList(ExpressionNode context, ExpressionNode player, Token accessToken, Token playertoken) : base(context, player, accessToken, playertoken) { }

        public override object Evaluate(Context context, List<Card> targets)
        {
            Player targetPlayer = (Player)player.Evaluate(context, targets);
            gameComponent = GlobalContext.Graveyard(targetPlayer);
            return gameComponent.cards;
        }
    }

    // List of cards in a player's field
    public class FieldList : IndividualList
    {
        public FieldList(ExpressionNode context, ExpressionNode player, Token accessToken, Token playertoken) : base(context, player, accessToken, playertoken) { }

        public override object Evaluate(Context context, List<Card> targets)
        {
            Player targetPlayer = (Player)player.Evaluate(context, targets);
            gameComponent = GlobalContext.Field(targetPlayer);
            return gameComponent.cards;
        }
    }

    // List of cards filtered by a predicate
    public class ListFind : List
    {
        public ListFind() : base(null) { }

        public ListFind(ExpressionNode list, ExpressionNode predicate, Token parameter, Token accessToken, Token argumentToken) : base(accessToken)
        {
            this.list = list;
            this.predicate = predicate;
            this.parameter = parameter;
            this.argumentToken = argumentToken;
        }

        public IExpression list;
        public IExpression predicate;
        public Token parameter;
        public Token argumentToken;

        public override object Evaluate(Context context, List<Card> targets)
        {
            // Save the variable value if it exists in the context
            object card = 0;
            List<Card> result = new List<Card>();
            bool usedvariable = false;
            if (context.variables.ContainsKey(parameter.lexeme))
            {
                card = context.variables[parameter.lexeme];
                usedvariable = true;
            }

            // Evaluate the predicate for each card in the list
            foreach (Card listcard in (List<Card>)list.Evaluate(context, targets))
            {
                context.Set(parameter, listcard);
                if ((bool)predicate.Evaluate(context, targets)) result.Add(listcard);
            }

            // Restore the original variable value if it was used
            if (usedvariable) context.Set(parameter, card);
            else context.variables.Remove(parameter.lexeme);

            return result;
        }
    }



    public class LiteralNode : ExpressionNode
    {
        public object value { get; }
        public override void SetProperty(ExpressionNode property) { }

        public LiteralNode(object value)
        {
            this.value = value;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            return value;
        }
    }

    public class ForNode : ExpressionNode
    {
        public List<ExpressionNode> body { get; }

        public ForNode(List<ExpressionNode> body)
        {
            this.body = body;
        }
        public override void SetProperty(ExpressionNode property) { }

        public override object Evaluate(Context context, List<Cards> target)
        {
            foreach (var expression in body)
            {
                return expression.Evaluate(context, target);
            }
            return null;
        }
    }

    public class WhileNode : ExpressionNode
    {
        public ExpressionNode condition { get; }
        public List<ExpressionNode> body { get; }
        public override void SetProperty(ExpressionNode property) { }

        public WhileNode(ExpressionNode condition, List<ExpressionNode> body)
        {
            this.condition = condition;
            this.body = body;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            foreach (var expression in body)
            {
                return expression.Evaluate(context, target);
            }
            return null;
        }
    }

    public class IdentifierNode : ExpressionNode
    {
        public string value { get; }
        public string type { get; }
        public ExpressionNode property { get; private set; }

        public IdentifierNode(string value, string type)
        {
            this.value = value;
            this.type = type;
        }

        public override void SetProperty(ExpressionNode property)
        {
            if (this.property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        // public override void SetValue(GlobalContext context, List<Cards> targets, object value)
        // {

        // }

        public override object Evaluate(Context context, List<Cards> target)
        {
            if (property == null)
                return property.Evaluate(context, target);
            return value;
        }
    }

    public class AssignamentNode : ExpressionNode
    {
        public ExpressionNode identifier { get; }
        public ExpressionNode value { get; private set; }
        public override void SetProperty(ExpressionNode property) { }

        public AssignamentNode(ExpressionNode identifier, ExpressionNode value)
        {
            this.identifier = identifier;
            this.value = value;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            identifier.SetValue(context, target, value.Evaluate(context, target));
            return val;
        }

    }

    public class MethodAccessNode : ExpressionNode
    {
        public string name { get; }
        public ExpressionNode parameter { get; private set; }
        public ExpressionNode property { get; private set; }

        public MethodAccessNode(string name)
        {
            this.name = name;
        }

        public override void SetProperty(ExpressionNode property)
        {
            if (this.property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        public void AddParameter(ExpressionNode parameter)
        {
            this.parameter = parameter;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            if (parameter == null)
            {
                return name switch
                {
                    "Field" => context.Field,
                    "Deck" => context.Deck,
                    "Hand" => context.Hand,
                    "Graveyard" => context.Graveyard,
                    "Board" => context.board,
                    _ => throw new Exception("Unknown method"),
                };
            }
            else
            {
                return name switch
                {
                    "FieldOfPlayer" => context.FieldOfPlayer((Guid)parameter.Evaluate(context, target)),
                    "DeckOfPlayer" => context.DeckOfPlayer((Guid)parameter.Evaluate(context, target)),
                    "HandOfPlayer" => context.HandOfPlayer((Guid)parameter.Evaluate(context, target)),
                    "GraveyardOfPlayer" => context.GraveyardOfPlayer((Guid)parameter.Evaluate(context, target)),
                    _ => throw new Exception("Unknown method"),
                };
            }
            throw new NotImplementedException();
        }
    }

    public class ListNode : ExpressionNode
    {
        public ExpressionNode left { get; }
        public ExpressionNode arg { get; }
        public ExpressionNode property { get; private set; }

        public ListNode(ExpressionNode left, ExpressionNode arg)
        {
            this.arg = arg;
            this.left = left;
        }

        public override void SetProperty(ExpressionNode property)
        {
            if (this.property != null)
            {
                this.property.SetProperty(property);
                return;
            }
            this.property = property;
        }

        public override object Evaluate(Context context, List<Cards> target)
        {
            return ((List<Cards>)property.Evaluate(context, target))[(int)arg.Evaluate(context, target)];
        }
    }

}