using System;
using System.Collections.Generic;
using System.IO;

namespace Blapaz.Buddy.Compiler
{
    public class Program
    {
        public static List<string> Imports { get; private set; }

        public static string Compile(string scriptFile)
        {
            Imports = new List<string>();
            Lexer lexer = new Lexer(File.ReadAllText(scriptFile));
            List<Token> tokens = new List<Token>();

            Token token;
            while ((token = lexer.GetToken()).TokenName != Lexer.Tokens.EOF)
            {
                if (token.TokenName != Lexer.Tokens.Whitespace && token.TokenName != Lexer.Tokens.NewLine && token.TokenName != Lexer.Tokens.Undefined)
                {
                    tokens.Add(token);
                }
            }

            tokens.Add(new Token(Lexer.Tokens.EOF, "EOF"));

            Parser parser = new Parser(new TokenList(tokens));
            List<Stmt> tree = parser.GetTree();

            Compiler compiler = new Compiler(tree);
            string compiledCode = compiler.GetCode();

            foreach (string import in Imports)
            {
                using (StreamReader s = new StreamReader(Path.Combine(Path.GetDirectoryName(scriptFile), import + ".buddy")))
                {
                    compiledCode += Environment.NewLine + s.ReadToEnd();
                }
            }

            return compiledCode;
        }
    }
}
