using System;
using System.Collections.Generic;

namespace Blapaz.Buddy.Compiler.Library
{
    class Parser
    {
        public List<Stmt> Tree { get; private set; }

        private TokenList _tokens;
        private Block _currentBlock;
        private Stack<Block> _blockstack;
        private bool _isRunning;

        public Parser(TokenList tokenList)
        {
            Tree = new List<Stmt>();

            _tokens = tokenList;
            _currentBlock = null;
            _blockstack = new Stack<Block>();        
            _isRunning = true;
            Token token = null;

            while (_isRunning)
            {
                try
                {
                    token = _tokens.GetToken();
                }
                catch { }

                if (token.Name == Lexer.TokenType.Import)
                {
                    Program.Imports.Add(ParseImport());
                }
                else if (token.Name == Lexer.TokenType.Global)
                {
                    if (_tokens.Peek().Name == Lexer.TokenType.Ident)
                    {
                        string ident = _tokens.GetToken().Value.ToString();

                        // Standard assign, could be string, int, or array
                        if (_tokens.Peek().Name == Lexer.TokenType.Equal)
                        {
                            _tokens.pos++;

                            // Assign an array
                            if (_tokens.Peek().Name == Lexer.TokenType.LeftBracket)
                            {
                                // Go back two tokens to get ident
                                _tokens.pos -= 2;

                                foreach (GlobalAssign assign in ParseGlobalAssignArray())
                                {
                                    Tree.Add(assign);
                                }
                            }
                            // Assign an expr
                            else
                            {
                                Tree.Add(new GlobalAssign(ident, ParseExpr()));
                            }
                        }
                    }
                }
                else if (token.Name == Lexer.TokenType.Function)
                {
                    Func func = ParseFunc();

                    if (_currentBlock == null)
                    {
                        _currentBlock = func;
                    }
                    else
                    {
                        _currentBlock.AddStmt(new Return(null));
                        Tree.Add(_currentBlock);
                        _currentBlock = func;
                    }
                }
                else if (token.Name == Lexer.TokenType.Event)
                {
                    Evnt evnt = ParseEvnt();

                    if (_currentBlock == null)
                    {
                        _currentBlock = evnt;
                    }
                    else
                    {
                        _currentBlock.AddStmt(new Return(null));
                        Tree.Add(_currentBlock);
                        _currentBlock = evnt;
                    }
                }
                else if (token.Name == Lexer.TokenType.If)
                {
                    IfBlock ifblock = ParseIf();

                    if (_currentBlock != null)
                    {
                        _blockstack.Push(_currentBlock);
                        _currentBlock = ifblock;
                    }
                }
                else if (token.Name == Lexer.TokenType.ElseIf)
                {
                    ElseIfBlock elseifblock = ParseElseIf();

                    if (_currentBlock != null)
                    {
                        _blockstack.Push(_currentBlock);
                        _currentBlock = elseifblock;
                    }
                }
                else if (token.Name == Lexer.TokenType.Else)
                {
                    if (_currentBlock != null)
                    {
                        _blockstack.Push(_currentBlock);
                        _currentBlock = new ElseBlock();
                    }
                }
                else if (token.Name == Lexer.TokenType.Repeat)
                {
                    if (_currentBlock != null)
                    {
                        _blockstack.Push(_currentBlock);
                        _currentBlock = new RepeatBlock();
                    }
                }
                else if (token.Name == Lexer.TokenType.While)
                {
                    WhileBlock whileblock = ParseWhile();

                    if (_currentBlock != null)
                    {
                        _blockstack.Push(_currentBlock);
                        _currentBlock = whileblock;
                    }
                }
                else if (token.Name == Lexer.TokenType.Ident)
                {
                    // Standard assign, could be string, int, or array
                    if (_tokens.Peek().Name == Lexer.TokenType.Equal)
                    {
                        _tokens.pos++;

                        // Assign an array
                        if (_tokens.Peek().Name == Lexer.TokenType.LeftBracket)
                        {
                            // Go back two tokens to get ident
                            _tokens.pos -= 2;

                            foreach (Assign assign in ParseAssignArray())
                            {
                                _currentBlock.AddStmt(assign);
                            }
                        }
                        // Assign an expr
                        else
                        {
                            // Go back two tokens to ident
                            _tokens.pos -= 2;
                            _currentBlock.AddStmt(ParseAssign());
                        }
                    }
                    // Reference to a function call
                    else if (_tokens.Peek().Name == Lexer.TokenType.LeftParan)
                    {
                        _tokens.pos--;
                        Call c = ParseCall();
                        _currentBlock.AddStmt(c);
                    }
                }
                else if (token.Name == Lexer.TokenType.Return)
                {
                    Return r = ParseReturn();
                    _currentBlock.AddStmt(r);
                }
                else if (token.Name == Lexer.TokenType.RightBrace)
                {
                    if (_currentBlock is Func)
                    {
                        _currentBlock.AddStmt(new Return(null));
                        Tree.Add(_currentBlock);
                        _currentBlock = null;
                    }
                    else if (_currentBlock is IfBlock || _currentBlock is ElseIfBlock || _currentBlock is ElseBlock)
                    {
                        _currentBlock.AddStmt(new EndIf());
                        Block block = _currentBlock;

                        if (_blockstack.Count > 0)
                        {
                            _currentBlock = _blockstack.Pop();
                            _currentBlock.AddStmt(block);
                        }
                    }
                    else if (_currentBlock is RepeatBlock || _currentBlock is WhileBlock)
                    {
                        Block block = _currentBlock;

                        if (_blockstack.Count > 0)
                        {
                            _currentBlock = _blockstack.Pop();
                            _currentBlock.AddStmt(block);
                        }
                    }
                }
                else if (token.Name == Lexer.TokenType.EOF)
                {
                    Tree.Add(_currentBlock);
                    _isRunning = false;
                }
            }
        }

        private string ParseImport()
        {
            string ret = "";
            Token token = _tokens.GetToken();

            if (token.Name == Lexer.TokenType.Ident)
            {
                ret = token.Value;
            }

            return ret;
        }

        private Func ParseFunc()
        {
            string ident = "";
            List<string> vars = new List<string>();

            if (_tokens.Peek().Name == Lexer.TokenType.Ident)
            {
                ident = _tokens.GetToken().Value.ToString();
            }
            
            if (_tokens.Peek().Name == Lexer.TokenType.LeftParan)
            {
                _tokens.pos++;
            }

            if (_tokens.Peek().Name == Lexer.TokenType.RightParan)
            {
                _tokens.pos++;
            }
            else
            {
                vars = ParseFuncArgs();
            }

            if (_tokens.Peek().Name == Lexer.TokenType.LeftBrace)
            {
                _tokens.pos++;
            }

            return new Func(ident, vars);
        }

        private Evnt ParseEvnt()
        {
            string ident = "";
            List<string> vars = new List<string>();

            if (_tokens.Peek().Name == Lexer.TokenType.Ident)
            {
                ident = _tokens.GetToken().Value.ToString();
            }

            if (_tokens.Peek().Name == Lexer.TokenType.LeftParan)
            {
                _tokens.pos++;
            }

            if (_tokens.Peek().Name == Lexer.TokenType.RightParan)
            {
                _tokens.pos++;
            }
            else
            {
                vars = ParseFuncArgs();
            }

            if (_tokens.Peek().Name == Lexer.TokenType.LeftBrace)
            {
                _tokens.pos++;
            }

            return new Evnt(ident, vars);
        }

        private WhileBlock ParseWhile()
        {
            WhileBlock ret = null;
            Symbol op = 0;

            if (_tokens.Peek().Name == Lexer.TokenType.LeftParan)
            {
                _tokens.pos++;
            }

            Expr lexpr = ParseExpr();

            if (_tokens.Peek().Name == Lexer.TokenType.DoubleEqual)
            {
                op = Symbol.doubleEqual;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.Greater)
            {
                op = Symbol.gt;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.GreaterEqual)
            {
                op = Symbol.gte;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.Lesser)
            {
                op = Symbol.lt;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.LesserEqual)
            {
                op = Symbol.lte;
                _tokens.pos++;
            }

            Expr rexpr = ParseExpr();

            if (_tokens.Peek().Name == Lexer.TokenType.RightParan)
            {
                _tokens.pos++;
            }

            ret = new WhileBlock(lexpr, op, rexpr);

            return ret;
        }

        private IfBlock ParseIf()
        {
            IfBlock ret = null;
            Symbol op = 0;

            if (_tokens.Peek().Name == Lexer.TokenType.LeftParan)
            {
                _tokens.pos++;
            }

            Expr lexpr = ParseExpr();

            if (_tokens.Peek().Name == Lexer.TokenType.DoubleEqual)
            {
                op = Symbol.doubleEqual;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.Greater)
            {
                op = Symbol.gt;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.GreaterEqual)
            {
                op = Symbol.gte;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.Lesser)
            {
                op = Symbol.lt;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.LesserEqual)
            {
                op = Symbol.lte;
                _tokens.pos++;
            }

            Expr rexpr = ParseExpr();

            if (_tokens.Peek().Name == Lexer.TokenType.RightParan)
            {
                _tokens.pos++;
            }

            ret = new IfBlock(lexpr, op, rexpr);

            return ret;
        }

        private ElseIfBlock ParseElseIf()
        {
            ElseIfBlock ret = null;
            Symbol op = 0;

            if (_tokens.Peek().Name == Lexer.TokenType.LeftParan)
            {
                _tokens.pos++;
            }

            Expr lexpr = ParseExpr();

            if (_tokens.Peek().Name == Lexer.TokenType.DoubleEqual)
            {
                op = Symbol.doubleEqual;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.NotEqual)
            {
                op = Symbol.notEqual;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.Greater)
            {
                op = Symbol.gt;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.GreaterEqual)
            {
                op = Symbol.gte;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.Lesser)
            {
                op = Symbol.lt;
                _tokens.pos++;
            }
            else if (_tokens.Peek().Name == Lexer.TokenType.LesserEqual)
            {
                op = Symbol.lte;
                _tokens.pos++;
            }

            Expr rexpr = ParseExpr();

            if (_tokens.Peek().Name == Lexer.TokenType.RightParan)
            {
                _tokens.pos++;
            }

            ret = new ElseIfBlock(lexpr, op, rexpr);

            return ret;
        }

        private GlobalAssign ParseGlobalAssign()
        {
            string ident = _tokens.GetToken().Value.ToString();

            _tokens.pos++;

            return new GlobalAssign(ident, ParseExpr());
        }

        private Assign ParseAssign()
        {
            Assign ret = null;
            string ident = "";

            Token token = _tokens.GetToken();
            ident = token.Value.ToString();

            _tokens.pos++;

            Expr value = ParseExpr();

            ret = new Assign(ident, value);

            return ret;
        }

        private List<GlobalAssign> ParseGlobalAssignArray()
        {
            List<GlobalAssign> ret = new List<GlobalAssign>();
            string ident = _tokens.GetToken().Value.ToString();

            // Skips '=' and '['
            _tokens.pos += 2;

            int index = 0;

            while (_tokens.Peek().Name != Lexer.TokenType.RightBracket)
            {
                Expr value = ParseExpr();
                ret.Add(new GlobalAssign($"{ident}.{index}", value));
                index++;

                if (_tokens.Peek().Name == Lexer.TokenType.Comma)
                    _tokens.pos++;
            }

            return ret;
        }

        private List<Assign> ParseAssignArray()
        {
            List<Assign> ret = new List<Assign>();
            string ident = _tokens.GetToken().Value.ToString();

            // Skips '=' and '['
            _tokens.pos += 2;

            int index = 0;

            while (_tokens.Peek().Name != Lexer.TokenType.RightBracket)
            {
                Expr value = ParseExpr();
                ret.Add(new Assign($"{ident}.{index}", value));
                index++;

                if (_tokens.Peek().Name == Lexer.TokenType.Comma)
                    _tokens.pos++;
            }

            return ret;
        }

        private Call ParseCall()
        {
            string ident = "";
            Token token = _tokens.GetToken();
            List<Expr> args = new List<Expr>();

            if (token.Name == Lexer.TokenType.Ident)
            {
                ident = token.Value.ToString();
            }

            if (_tokens.Peek().Name == Lexer.TokenType.LeftParan)
            {
                _tokens.pos++;
            }

            if (_tokens.Peek().Name == Lexer.TokenType.RightParan)
            {
                _tokens.pos++;
            }
            else
            {
                args = ParseCallArgs();
            }

            return new Call(ident, args);
        }

        private Return ParseReturn()
        {
            return new Return(ParseExpr());
        }

        private Expr ParseExpr()
        {
            Expr ret = null;
            Token token = _tokens.GetToken();

            if (_tokens.Peek().Name == Lexer.TokenType.LeftParan)
            {
                string ident = "";

                if (token.Name == Lexer.TokenType.Ident)
                {
                    ident = token.Value.ToString();
                }

                _tokens.pos++;

                if (_tokens.Peek().Name == Lexer.TokenType.RightParan)
                {
                    ret = new CallExpr(ident, new List<Expr>());
                }
                else
                {
                    ret = new CallExpr(ident, ParseCallArgs());
                }
            }
            else if (token.Name == Lexer.TokenType.IntLiteral)
            {
                IntLiteral i = new IntLiteral(Convert.ToInt32(token.Value.ToString()));
                ret = i;
            }
            else if (token.Name == Lexer.TokenType.StringLiteral)
            {
                StringLiteral s = new StringLiteral(token.Value.ToString());
                ret = s;
            }
            else if (token.Name == Lexer.TokenType.Ident)
            {
                string ident = token.Value.ToString();

                Ident i = new Ident(ident);
                ret = i;
            }
            else if (token.Name == Lexer.TokenType.LeftParan)
            {
                Expr e = ParseExpr();

                if (_tokens.Peek().Name == Lexer.TokenType.RightParan)
                {
                    _tokens.pos++;
                }

                ParanExpr p = new ParanExpr(e);

                if (_tokens.Peek().Name == Lexer.TokenType.Add)
                {
                    _tokens.pos++;
                    Expr expr = ParseExpr();
                    ret = new MathExpr(p, Symbol.add, expr);
                }
                else if (_tokens.Peek().Name == Lexer.TokenType.Sub)
                {
                    _tokens.pos++;
                    Expr expr = ParseExpr();
                    ret = new MathExpr(p, Symbol.sub, expr);
                }
                else if (_tokens.Peek().Name == Lexer.TokenType.Mul)
                {
                    _tokens.pos++;
                    Expr expr = ParseExpr();
                    ret = new MathExpr(p, Symbol.mul, expr);
                }
                else if (_tokens.Peek().Name == Lexer.TokenType.Div)
                {
                    _tokens.pos++;
                    Expr expr = ParseExpr();
                    ret = new MathExpr(p, Symbol.div, expr);
                }
                else
                {
                    ret = p;
                }
            }

            if (_tokens.Peek().Name == Lexer.TokenType.Add || _tokens.Peek().Name == Lexer.TokenType.Sub || _tokens.Peek().Name == Lexer.TokenType.Mul || _tokens.Peek().Name == Lexer.TokenType.Div)
            {
                Expr lexpr = ret;
                Symbol op = 0;

                if (_tokens.Peek().Name == Lexer.TokenType.Add)
                {
                    op = Symbol.add;
                }
                else if (_tokens.Peek().Name == Lexer.TokenType.Sub)
                {
                    op = Symbol.sub;
                }
                else if (_tokens.Peek().Name == Lexer.TokenType.Mul)
                {
                    op = Symbol.mul;
                }
                else if (_tokens.Peek().Name == Lexer.TokenType.Div)
                {
                    op = Symbol.div;
                }

                _tokens.pos++;

                Expr rexpr = ParseExpr();

                ret = new MathExpr(lexpr, op, rexpr);
            }

            return ret;
        }

        private List<string> ParseFuncArgs()
        {
            List<string> ret = new List<string>();

            while (true)
            {
                Token token = _tokens.GetToken();

                if (token.Name == Lexer.TokenType.Ident)
                {
                    ret.Add(token.Value.ToString());
                }

                if (_tokens.Peek().Name == Lexer.TokenType.Comma)
                {
                    _tokens.pos++;
                }
                else if (_tokens.Peek().Name == Lexer.TokenType.RightParan)
                {
                    _tokens.pos++;
                    break;
                }
            }

            return ret;
        }

        private List<Expr> ParseCallArgs()
        {
            List<Expr> ret = new List<Expr>();

            while (true)
            {
                ret.Add(ParseExpr());

                if (_tokens.Peek().Name == Lexer.TokenType.Comma)
                {
                    _tokens.pos++;
                }
                else if (_tokens.Peek().Name == Lexer.TokenType.RightParan)
                {
                    _tokens.pos++;
                    break;
                }
            }

            return ret;
        }
    }
}
