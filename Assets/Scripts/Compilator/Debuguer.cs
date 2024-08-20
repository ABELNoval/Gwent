using UnityEngine;
using Console;
using System.Collections.Generic;

public class Debuguer : MonoBehaviour
{
    void Start()
    {
        string input = @"
        card{
        Name: ""Prueba1"",
            Type: ""Oro"",
            Power: 3 + 4 * (2 - 1) / (4 - (1 + 1))^0 + 5,
            Faction: ""Source"",
            Range: [""Melee"", ""Siege""],
            OnActivation:
            [
                {
            Effect:
                {
                Name: ""Draw"",
                        Amount: 5
                    },
                    Selector:
                {
                Source: ""board"",
                        Single: false,
                        Predicate: ""(unit) => unit.Faction == Source"",
                    },
                    PosAction:
                {
                Name: ""ReturnRoDeck"",
                        Selector:
                    {
                    Source: ""parent"",
                            Single: false,
                            Predicate: ""(unit) => unit.Power < 1""
                        }
                }
            },
                {
            Effect: ""Kill""
                }
            ]
        }
        ";

        string input3 = @"
        card{
            Name: ""Prueba1"",
            Type: ""Oro"",
            Power: 3 + 4 * (2 - 1) / (4 - (1 + 1))^0 + 5,
            Faction: ""Source"",
            Range: [""Melee"", ""Siege""],
            OnActivation:
            []
        }";

        string input2 = @"
            effect
            {
                Name: ""Damage"",
                Params:
                {
                    amount: Number
                },
                Action: (targets, context) =>
                {
                    for target in targets
                    {
                        i = 1;
                        while(i++ < amount)
                            target.Power -= 1;
                    };
                }
            }
        ";
        Lexer lexer = new Lexer(input3);
        List<Token> Tokens = lexer.Analyze();

        Debug.Log("------------Lexer-------------");

        foreach (Token token in Tokens)
        {
            Debug.Log(token.type);
            Debug.Log(token.value);
        }

        Parser parser = new Parser(Tokens);
        ProgramNode node = parser.Parse();
        CardNode cardNode = node as CardNode;

        Debug.Log("------------Parser-------------");

        Debug.Log($@"
             Nombre: {cardNode.Name.Evaluate(null, null)}
             Tipo: {cardNode.Type.Evaluate(null, null)}
             Faccion: {cardNode.Faction.Evaluate(null, null)}
             Power: {cardNode.Power.Evaluate(null, null)}
             Range: [{cardNode.Range[0].Evaluate(null, null)}, {cardNode.Range[1].Evaluate(null, null)}]
             OnActivation: []"
            );

    }

}
