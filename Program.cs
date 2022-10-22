using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum Type
{
    Punctuation,
    Number,
    Operation
}

public class Token
{
    public Type TokenType { get; set; }
    public string Content { get; set; }
    public string Value { get; set; }

    public Token(Type type, string content, string value)
    {
        TokenType = type;
        Content = content;
        Value = value;
    }
}



public static class LexicalAnalyzer
{
    private static string GetLexemaValue(string value)
        => value switch
        {
            "(" => "PAR_ESQ",
            ")" => "PAR_DIR",
            "[" => "CHV_ESQ",
            "]" => "CHV_DIR",
            "+" => "SOMA",
            "-" => "SUBT",
            "*" => "MULT",
            "/" => "DIVS",
            "**" => "EXP",
            _ => throw new ArgumentException("O lexema informado não possui valor definido.")
        };

    public static List<Token> Analyze(string txt)
    {
        var result = new List<Token>();
        
        // Remove comments
        txt = Regex.Replace(txt, @"#.*\n", string.Empty);

        int i = 0;
        
        // Loops until it finds the EOL char ';'
        while (txt[i] != ';')
        {            
            // Ignore whitespace
            while (char.IsWhiteSpace(txt[i]))
            {
                i++;
            }

            var numMatch = Regex.Match(txt.Substring(i), @"^-?\d+");             
            var opMatch = Regex.Match(txt.Substring(i), @"^\*\*|^[+\-\/*]");             
            var punctMatch = Regex.Match(txt.Substring(i), @"^[\(\)\[\]]");

            Token token;

            if (numMatch.Success)
            {
                token = new Token(Type.Number, numMatch.Value, numMatch.Value);
            }
            else if (opMatch.Success)
            {
                token = new Token(Type.Operation, opMatch.Value, GetLexemaValue(opMatch.Value));
            }
            else if (punctMatch.Success)
            {
                token = new Token(Type.Punctuation, punctMatch.Value, GetLexemaValue(punctMatch.Value));
            }
            else
            {
                throw new Exception($"Erro de leitura: caractere inválido: {txt[i]}");
            }                        

            i += token.Content.Length;

            result.Add(token);
        }

        return result;
    }
}

public class Program
{
    public static void Main()
    {
        string input = "(2 + 4)**2;";
        string input2 = "42 + (675 * 31) - 20925;";
        string input3 = "       2345 #teste \n + [123 / 2 * (-379 \n + -11) ** 3];";
        string input4 = "       2345 #teste \n + [12t3 / 2 * (-379 \n + -11) ** 3];";
        string input5 = "2-3";

        var result = LexicalAnalyzer.Analyze(input);

        Console.WriteLine("Lexema\t\t\tTipo\t\t\t\tValor");

        foreach (var token in result)
        {
            string tokenValue = token.TokenType != Type.Number ? token.Value : ("\t" + token.Value);

            Console.WriteLine($"{token.Content}\t\t\t{Enum.GetName(token.TokenType)}\t\t\t{tokenValue}");
        }
    }
}