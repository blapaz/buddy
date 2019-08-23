using System.Collections.Generic;

namespace Blapaz.Buddy.Compiler.Library
{
    class Compiler
    {
        public string Code { get; private set; }
        private int repeats = 0;

        public Compiler(List<Stmt> list)
        {
            Code = "";
            CompileStmtList(list);
        }
        
        private void CompileStmtList(List<Stmt> statements)
        {
            foreach (Stmt statement in statements)
            {
                if (statement is Func)
                {
                    CompileFunc((Func)statement);
                }
                else if (statement is Evnt)
                {
                    CompileEvnt((Evnt)statement);
                }
                else if (statement is IfBlock)
                {
                    CompileIf((IfBlock)statement);
                }
                else if (statement is ElseIfBlock)
                {
                    CompileElseIf((ElseIfBlock)statement);
                }
                else if (statement is ElseBlock)
                {
                    CompileElse((ElseBlock)statement);
                }
                else if (statement is EndIf)
                {
                    Write("endif");
                }
                else if (statement is RepeatBlock)
                {
                    CompileRepeat((RepeatBlock)statement);
                }
                else if (statement is WhileBlock)
                {
                    CompileWhile((WhileBlock)statement);
                }
                else if (statement is GlobalAssign)
                {
                    CompileGlobalAssign((GlobalAssign)statement);
                }
                else if (statement is Assign)
                {
                    CompileAssign((Assign)statement);
                }
                else if (statement is Call)
                {
                    CompileCall((Call)statement);
                }
                else if (statement is Goto)
                {
                    Write("goto " + ((Goto)statement).Name);
                }
                else if (statement is Return)
                {
                    if (((Return)statement).expr == null)
                    {
                        Write("ret");
                    }
                    else
                    {
                        CompileExpr(((Return)statement).expr);
                        Write("ret");
                    }
                }
            }
        }

        private void CompileFunc(Func data)
        {
            Write(":" + data.ident);

            foreach (string s in data.vars)
            {
                Write("setVar " + s);
            }

            CompileStmtList(data.statements);
        }

        private void CompileEvnt(Evnt data)
        {
            Write("e:" + data.ident);

            foreach (string s in data.vars)
            {
                Write("setVar " + s);
            }

            CompileStmtList(data.statements);
        }

        private void CompileIf(IfBlock data)
        {
            CompileExpr(data.leftExpr);
            CompileExpr(data.rightExpr);

            Write($"if{CompileSymbol(data.op)}");

            CompileStmtList(data.statements);
        }

        private void CompileElseIf(ElseIfBlock data)
        {
            CompileExpr(data.leftExpr);
            CompileExpr(data.rightExpr);

            Write($"elseif{CompileSymbol(data.op)}");

            CompileStmtList(data.statements);
        }

        private string CompileSymbol(Symbol op)
        {
            if (op == Symbol.doubleEqual)
            {
                return "e";
            }
            else if (op == Symbol.notEqual)
            {
                return "n";
            }

            return op.ToString();
        }

        private void CompileElse(ElseBlock data)
        {
            Write("else");
            CompileStmtList(data.statements);
        }

        private void CompileRepeat(RepeatBlock data)
        {
            string name = ".repeat" + repeats.ToString();
            repeats++;
            Write(name);
            CompileStmtList(data.statements);
            Write("goto " + name);
        }

        private void CompileWhile(WhileBlock data)
        {
            string name = ".repeat" + repeats.ToString();
            repeats++;

            Write(name);

            IfBlock ifBlock = new IfBlock(data.leftExpr, data.op, data.rightExpr);
            ifBlock.statements.AddRange(data.statements);
            ifBlock.statements.Add(new Goto(name));
            ifBlock.statements.Add(new EndIf());

            CompileStmtList(new List<Stmt>() { ifBlock });
        }

        private void CompileGlobalAssign(GlobalAssign globalAssign)
        {
            CompileExpr(globalAssign.Value);
            Write("setGlobalVar " + globalAssign.Ident);
        }

        private void CompileAssign(Assign data)
        {
            CompileExpr(data.value);
            Write("setVar " + data.ident);
        }

        private void CompileCall(Call data)
        {
            data.args.Reverse();

            foreach (Expr e in data.args)
            {
                CompileExpr(e);
            }

            Write("call " + data.ident);
        }

        private void CompileExpr(Expr data)
        {
            if (data is IntLiteral)
            {
                Write("pushInt " + ((IntLiteral)data).value);
            }
            else if (data is StringLiteral)
            {
                Write("pushString " + ((StringLiteral)data).value);
            }
            else if (data is Ident)
            {
                Write("pushVar " + ((Ident)data).value);
            }
            else if (data is CallExpr)
            {
                foreach (Expr e in ((CallExpr)data).args)
                {
                    CompileExpr(e);
                }

                Write("call " + ((CallExpr)data).ident);
            }
            else if (data is MathExpr)
            {
                CompileExpr(((MathExpr)data).leftExpr);
                CompileExpr(((MathExpr)data).rightExpr);

                if (((MathExpr)data).op == Symbol.add)
                {
                    Write("add");
                }
                else if (((MathExpr)data).op == Symbol.sub)
                {
                    Write("sub");
                }
                else if (((MathExpr)data).op == Symbol.mul)
                {
                    Write("mul");
                }
                else if (((MathExpr)data).op == Symbol.div)
                {
                    Write("div");
                }
            }
            else if (data is ParanExpr)
            {
                CompileExpr(((ParanExpr)data).value);
            }
        }

        private void Write(string data)
        {
            if (Code == "")
            {
                Code += data;
            }
            else
            {
                Code += "\n" + data;
            }
        }
    }
}
