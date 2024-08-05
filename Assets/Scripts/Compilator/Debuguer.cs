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
            Power: 10,
            Faction: ""Source"",
            Range: [""Melee"", ""Siege""]
        }";

        Lexer lexer = new Lexer(input);
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

        Debug.Log($"Nombre: {cardNode.name} \n Tipo: {cardNode.type} \n Faccion: {cardNode.faction} \n Power: {cardNode.power} \n Range: [{cardNode.range[0]}, {cardNode.range[1]}]");
    }

}
