using UnityEngine;
using Console;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Reflection;

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

        //Ok, parsea propiedades sin OnActivation
        // string input3 = @"
        // card{
        //     Name: ""Prueba1"",
        //     Type: ""Oro"",
        //     Power: 3 + 4 * (2 - 1) / (4 - (1 + 1))^0 + 5,
        //     Faction: ""Source"",
        //     Range: [""Melee"", ""Siege""],
        //     OnActivation:
        //     []
        // }";

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
                            target.Power = 1;
                    };
                }
            }
        ";
        Lexer lexer1 = new Lexer(input);
        List<Token> Tokens = lexer1.Analyze();
        Lexer lexer2 = new Lexer(input2);
        List<Token> Tokens2 = lexer2.Analyze();

        Debug.Log("------------Lexer-------------");

        foreach (Token token in Tokens)
        {
            Debug.Log(token.type);
            Debug.Log(token.value);
        }

        //Parser parser = new Parser(Tokens);
        //ProgramNode node = parser.Parse();
        Parser parser2 = new Parser(Tokens2);
        ProgramNode node2 = parser2.Parse();
        EffectNode effectNode = node2 as EffectNode;
        //CardNode cardNode = node as CardNode;

        Debug.Log("------------Parser-------------");

        //Input3
        // Debug.Log($@"
        //      Nombre: {cardNode.Name.Evaluate(null, null)}
        //      Tipo: {cardNode.Type.Evaluate(null, null)}
        //      Faccion: {cardNode.Faction.Evaluate(null, null)}
        //      Power: {cardNode.Power.Evaluate(null, null)}
        //      Range: [{cardNode.Range[0].Evaluate(null, null)}, {cardNode.Range[1].Evaluate(null, null)}]
        //      OnActivation: []"
        //     );

        //Input1
        // Debug.Log($@"
        //      Nombre: {cardNode.Name.Evaluate(null, null)}
        //      Tipo: {cardNode.Type.Evaluate(null, null)}
        //      Faccion: {cardNode.Faction.Evaluate(null, null)}
        //      Power: {cardNode.Power.Evaluate(null, null)}
        //      Range: [{cardNode.Range[0].Evaluate(null, null)}, {cardNode.Range[1].Evaluate(null, null)}]
        //      OnActivation: [Effect1 : Name = {cardNode.OnActivation.OnActValues[0].EffectData.Name.Evaluate(null, null)}, Effect1 : Amount = {cardNode.OnActivation.OnActValues[0].EffectData.Params[0].Item2.Evaluate(null, null)}, Effect2 : Name = {cardNode.OnActivation.OnActValues[1].EffectData.Name.Evaluate(null, null)}]"
        // );

        // Debug.Log($@"
        //      Nombre: {effectNode.Name.Evaluate(null, null)}
        //      Params: {effectNode.Parameters[0].Item2.Evaluate(null, null)}
        //      Action: {effectNode.Action.expressions[0].Evaluate(null, null)}
        // ");
    }

    // public static void PrintObjectProperties(object obj, int indent = 0)
    // {
    //     if (obj == null)
    //     {
    //         Debug.Log($"{new string(' ', indent)}null");
    //         return;
    //     }

    //     Type type = obj.GetType();
    //     PropertyInfo[] properties = type.GetProperties();

    //     foreach (var property in properties)
    //     {
    //         object value = property.GetValue(obj);
    //         string indentString = new string(' ', indent);

    //         if (value is IList list) // Check if the property is a list
    //         {
    //             Debug.Log($"{indentString}{property.Name} (List):");
    //             for (int i = 0; i < list.Count; i++)
    //             {
    //                 Debug.Log($"{indentString}  [{i}]:");
    //                 PrintObjectProperties(list[i], indent + 4);
    //             }
    //         }
    //         else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
    //         {
    //             // If it's a complex object, print its properties recursively
    //             Debug.Log($"{indentString}{property.Name}:");
    //             PrintObjectProperties(value, indent + 2);
    //         }
    //         else
    //         {
    //             // Otherwise, just print the value
    //             Debug.Log($"{indentString}{property.Name}: {value}");
    //         }
    //     }
    // }

}
