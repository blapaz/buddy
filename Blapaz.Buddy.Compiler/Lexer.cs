using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Blapaz.Buddy.Compiler
{
    public class Lexer
    {
        private readonly Dictionary<TokenType, string> _tokens;
        private readonly Dictionary<TokenType, MatchCollection> _matchCollection;
        private string _input;
        private int _index;

        public enum TokenType
        {
            Undefined,
            Import,
            Global,
            Function,
            If,
            ElseIf,
            Else,
            While,
            Repeat,
            Event,
            Return,
            StringLiteral,
            IntLiteral,
            ArrayLiteral,
            Ident,
            Whitespace,
            NewLine,
            Add,
            Sub,
            Mul,
            Div,
            Equal,
            DoubleEqual,
            NotEqual,
            LeftParan,
            RightParan,
            LeftBrace,
            RightBrace,
            LeftBracket,
            RightBracket,
            Comma,
            Period,
            Comment,
            EOF
        }

        public Lexer(string input)
        {
            _tokens = new Dictionary<TokenType, string>();
            _matchCollection = new Dictionary<TokenType, MatchCollection>();
            _input = input;
            _index = 0;

            _tokens.Add(TokenType.Import, "import");
            _tokens.Add(TokenType.Global, "global");
            _tokens.Add(TokenType.Function, "function");
            _tokens.Add(TokenType.If, "if");
            _tokens.Add(TokenType.ElseIf, "elseif");
            _tokens.Add(TokenType.Else, "else");
            _tokens.Add(TokenType.Repeat, "repeat");
            _tokens.Add(TokenType.Return, "return");
            _tokens.Add(TokenType.Event, "event");
            _tokens.Add(TokenType.StringLiteral, "\".*?\"");
            _tokens.Add(TokenType.IntLiteral, "[0-9][0-9]*");
            _tokens.Add(TokenType.ArrayLiteral, "\\[.*?\\]");
            _tokens.Add(TokenType.Ident, "[a-zA-Z_][a-zA-Z0-9_]*");
            _tokens.Add(TokenType.Whitespace, "[ \\t]+");
            _tokens.Add(TokenType.NewLine, "\\n");
            _tokens.Add(TokenType.Add, "\\+");
            _tokens.Add(TokenType.Sub, "\\-");
            _tokens.Add(TokenType.Mul, "\\*");
            _tokens.Add(TokenType.Div, "\\/");
            _tokens.Add(TokenType.DoubleEqual, "\\==");
            _tokens.Add(TokenType.NotEqual, "\\!=");
            _tokens.Add(TokenType.Equal, "\\=");
            _tokens.Add(TokenType.LeftParan, "\\(");
            _tokens.Add(TokenType.RightParan, "\\)");
            _tokens.Add(TokenType.LeftBrace, "\\{");
            _tokens.Add(TokenType.RightBrace, "\\}");
            _tokens.Add(TokenType.Comma, "\\,");
            _tokens.Add(TokenType.Period, "\\.");
            _tokens.Add(TokenType.Comment, "\\#.*?\n");

            foreach (KeyValuePair<TokenType, string> pair in _tokens)
            {
                _matchCollection.Add(pair.Key, Regex.Matches(_input, pair.Value));
            }
        }

        public Token GetToken()
        {
            if (_index >= _input.Length)
                return new Token(TokenType.EOF, string.Empty);

            foreach (KeyValuePair<TokenType, MatchCollection> pair in _matchCollection)
            {
                foreach (Match match in pair.Value)
                {
                    if (match.Index == _index)
                    {
                        _index += match.Length;
                        return new Token(pair.Key, match.Value);
                    }

                    if (match.Index > _index)
                    {
                        break;
                    }
                }
            }

            _index++;
            return new Token(TokenType.Undefined, string.Empty);
        }

        public PeekToken Peek()
        {
            return Peek(new PeekToken(_index, new Token(TokenType.Undefined, string.Empty)));
        }

        public PeekToken Peek(PeekToken peekToken)
        {
            int oldIndex = _index;

            _index = peekToken.Index;

            if (_index >= _input.Length)
            {
                _index = oldIndex;
                return null;
            }

            foreach (KeyValuePair<TokenType, string> pair in _tokens)
            {
                Regex r = new Regex(pair.Value);
                Match m = r.Match(_input, _index);

                if (m.Success && m.Index == _index)
                {
                    _index += m.Length;
                    PeekToken pt = new PeekToken(_index, new Token(pair.Key, m.Value));
                    _index = oldIndex;
                    return pt;
                }
            }

            PeekToken pt2 = new PeekToken(_index + 1, new Token(TokenType.Undefined, string.Empty));
            _index = oldIndex;
            return pt2;
        }
    }

    public class PeekToken
    {
        public int Index { get; set; }
        public Token Peek { get; set; }

        public PeekToken(int index, Token value)
        {
            Index = index;
            Peek = value;
        }
    }

    public class Token
    {
        public Lexer.TokenType Name { get; set; }
        public string Value { get; set; }

        public Token(Lexer.TokenType name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public class TokenList
    {
        public List<Token> Tokens;
        public int pos = 0;

        public TokenList(List<Token> tokens)
        {
            Tokens = tokens;
        }

        public Token GetToken()
        {
            Token ret = Tokens[pos];
            pos++;
            return ret;
        }

        public Token Peek()
        {
            return Tokens[pos];
        }
    }
}
