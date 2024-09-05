using UnityEngine;
using Console;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;

public class Debuguer : MonoBehaviour
{
    //  void Start()
    //  {
    readonly string input = @"
         card{
         Name: ""Hamil"",
             Type: ""Oro"",
             Power: ((5+3)*2)-(4/2)^1,
             Faction: ""Source"",
             Range: [""Melee"", ""Siege"", ""Range""],
             OnActivation:
             [
                 {
                    Effect:
                       {
                         Name: ""DecrementPower"",
                         Amount: 3
                        },
                    Selector:
                        {
                        Source: ""field"",
                         Single: true,
                         Predicate: (unit) => unit.Power > 5
                        },
                 },
              ]
            }
         ";

    // Ok, parsea propiedades sin OnActivation
    readonly string input3 = @"
          card{
              Name: ""Prueba1"",
              Type: ""Oro"",
              Power: 3 + 4 * (2 - 1) / (4 - (1 + 1))^0 + 5,
              Faction: ""Source"",
              Range: [""Melee"", ""Siege""],
              OnActivation:
              []
          }";

    readonly string input2 = @"
        effect
             {
                 Name: ""DecrementPower"",
                 Params:
                 {
                     amount: Number
                 },
                 Action: (targets, context)
                 {
                    for target in targets =>
                    {
                        target.Power = target.Power - amount;
                        target.Power--;
                    };
                 }
             }
         ";
    //     Lexer lexer1 = new Lexer(input);
    //     List<Token> Tokens = lexer1.Analyze();
    //     Lexer lexer2 = new Lexer(input2);
    //     List<Token> Tokens2 = lexer2.Analyze();

    //     Debug.Log("------------Lexer-------------");

    //     foreach (Token token in Tokens)
    //     {
    //         Debug.Log(token.type);
    //         Debug.Log(token.value);
    //     }

    //     Parser parser = new Parser(Tokens);
    //     ProgramNode node = parser.Parse();
    //     SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer();
    //     semanticAnalyzer.CheckCardNode(node as CardNode, new GlobalContext());
    //     // Parser parser2 = new Parser(Tokens2);
    //     // ProgramNode node2 = parser2.Parse();
    //     // EffectNode effectNode = node2 as EffectNode;
    //     //CardNode cardNode = node as CardNode;

    //     Debug.Log("------------Parser-------------");

    //     //Input3
    //     // Debug.Log($@"
    //     //      Nombre: {cardNode.Name.Evaluate(null, null)}
    //     //      Tipo: {cardNode.Type.Evaluate(null, null)}
    //     //      Faccion: {cardNode.Faction.Evaluate(null, null)}
    //     //      Power: {cardNode.Power.Evaluate(null, null)}
    //     //      Range: [{cardNode.Range[0].Evaluate(null, null)}, {cardNode.Range[1].Evaluate(null, null)}]
    //     //      OnActivation: []"
    //     //     );

    //     //Input1
    //     // Debug.Log($@"
    //     //      Nombre: {cardNode.Name.Evaluate(null, null)}
    //     //      Tipo: {cardNode.Type.Evaluate(null, null)}
    //     //      Faccion: {cardNode.Faction.Evaluate(null, null)}
    //     //      Power: {cardNode.Power.Evaluate(null, null)}
    //     //      Range: [{cardNode.Range[0].Evaluate(null, null)}, {cardNode.Range[1].Evaluate(null, null)}]
    //     //      OnActivation: [Effect1 : Name = {cardNode.OnActivation.OnActValues[0].EffectData.Name.Evaluate(null, null)}, Effect1 : Amount = {cardNode.OnActivation.OnActValues[0].EffectData.Params[0].Item2.Evaluate(null, null)}, Effect2 : Name = {cardNode.OnActivation.OnActValues[1].EffectData.Name.Evaluate(null, null)}]"
    //     // );

    //     // Debug.Log($@"
    //     //      Nombre: {effectNode.Name.Evaluate(null, null)}
    //     //      Params: {effectNode.Parameters[0].Item2.Evaluate(null, null)}
    //     //      Action: {effectNode.Action.expressions[0].Evaluate(null, null)}
    //     // ");
    // }

}
