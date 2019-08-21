using System.Collections.Generic;

namespace Blapaz.Buddy.Compiler
{
    class Stmt { }

    class Expr { }

    class Block : Stmt
    {
        public List<Stmt> statements;

        public Block()
        {
            statements = new List<Stmt>();
        }

        public void AddStmt(Stmt stmt)
        {
            statements.Add(stmt);
        }
    }

    class Func : Block
    {
        public string ident;
        public List<string> vars;

        public Func(string i, List<string> v)
        {
            ident = i;
            vars = v;
        }
    }

    class Evnt : Block
    {
        public string ident;
        public List<string> vars;

        public Evnt(string i, List<string> v)
        {
            ident = i;
            vars = v;
        }
    }

    class IfBlock : Block
    {
        public Expr leftExpr;
        public Symbol op;
        public Expr rightExpr;

        public IfBlock(Expr lexpr, Symbol o, Expr rexpr)
        {
            leftExpr = lexpr;
            op = o;
            rightExpr = rexpr;
        }
    }

    class ElseIfBlock : Block
    {
        public Expr leftExpr;
        public Symbol op;
        public Expr rightExpr;

        public ElseIfBlock(Expr lexpr, Symbol o, Expr rexpr)
        {
            leftExpr = lexpr;
            op = o;
            rightExpr = rexpr;
        }
    }

    class ElseBlock : Block { }

    class EndIf : Block { }

    class RepeatBlock : Block { }

    class GlobalAssign : Stmt
    {
        public string Ident { get; private set;  }
        public Expr Value { get; private set; }

        public GlobalAssign(string ident, Expr value)
        {
            Ident = ident;
            Value = value;
        }
    }

    class Assign : Stmt
    {
        public string ident;
        public Expr value;

        public Assign(string i, Expr v)
        {
            ident = i;
            value = v;
        }
    }

    class Call : Stmt
    {
        public string ident;
        public List<Expr> args;

        public Call(string i, List<Expr> a)
        {
            ident = i;
            args = a;
        }
    }

    class Return : Stmt
    {
        public Expr expr;

        public Return(Expr e)
        {
            expr = e;
        }
    }

    class IntLiteral : Expr
    {
        public int value;

        public IntLiteral(int v)
        {
            value = v;
        }
    }

    class StringLiteral : Expr
    {
        public string value;

        public StringLiteral(string v)
        {
            value = v;
        }
    }

    class Ident : Expr
    {
        public string value;

        public Ident(string v)
        {
            value = v;
        }
    }

    class MathExpr : Expr
    {
        public Expr leftExpr;
        public Symbol op;
        public Expr rightExpr;

        public MathExpr(Expr lexpr, Symbol o, Expr rexpr)
        {
            leftExpr = lexpr;
            op = o;
            rightExpr = rexpr;
        }
    }

    class ParanExpr : Expr
    {
        public Expr value;

        public ParanExpr(Expr v)
        {
            value = v;
        }
    }

    class CallExpr : Expr
    {
        public string ident;
        public List<Expr> args;

        public CallExpr(string i, List<Expr> a)
        {
            ident = i;
            args = a;
        }
    }

    enum Symbol
    {
        add = 0,
        sub = 1,
        mul = 2,
        div = 3,
        equal = 4,
        doubleEqual = 5,
        notEqual = 6,
        gt = 7,
        gte = 8,
        lt = 9,
        lte = 10,
        leftParan = 7,
        rightParan = 8,
        leftBrace = 9,
        rightbrace = 10
    }
}
