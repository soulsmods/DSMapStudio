using System.Collections.Generic;
using System;

namespace StudioCore.ParamEditor
{
    public class ExpressionEngine
    {
        //Written just to annoy matt

        public static Expression ParseExpression(string expression)
        {
            int head = 0;
            return ParseExpression(expression, ref head, null);
        }
        private static Expression ParseExpression(string expression, ref int head, Expression firstArg)
        {
            List<Expression> currentArgs = new List<Expression>();
            Operator currentOp;

            if (firstArg == null)
            {
                SkipWhitespace(expression, ref head);
                currentArgs.Add(ParseSingleExpression(expression, ref head));
            }
            else
            {
                currentArgs.Add(firstArg);
            }
            SkipWhitespace(expression, ref head);
            currentOp = ParseOperator(expression, ref head);
            if (currentOp == Operator.NON)
                return currentArgs[0];
            while (head < expression.Length)
            {
                SkipWhitespace(expression, ref head);
                currentArgs.Add(ParseSingleExpression(expression, ref head));
                SkipWhitespace(expression, ref head);
                if (head > expression.Length || expression[head] == ')')
                    return new OpExpression(currentOp, currentArgs);
                Operator newOp = ParseOperator(expression, ref head);
                if (newOp == Operator.NON)
                   throw new Exception();
                
                if (newOp.GetHashCode()/10 > currentOp.GetHashCode()/10)
                {
                    return ParseExpression(expression, ref head, new OpExpression(currentOp, currentArgs));
                }
                else if (newOp.GetHashCode()/10 < currentOp.GetHashCode()/10)
                {
                    currentArgs.Add(ParseExpression(expression, ref head, null));
                }
                else
                {
                    currentArgs.Add(ParseExpression(expression, ref head, null));
                }
            }
            throw new Exception();
        }
        private static Expression ParseSingleExpression(string expression, ref int head)
        {
            char firstChar = expression[head];
            if (firstChar == '(')
            {
                head++;
                Expression subExp = ParseExpression(expression, ref head, null);
                char endChar = expression[head];
                AssertChar(expression, ref head, ')');
                return subExp;
            }
            else if (char.IsDigit(firstChar))
            {
                string number = "";
                head++;
                while (char.IsDigit(firstChar) || firstChar == '.')
                {
                    number += firstChar;
                    firstChar = expression[head];
                    head++;
                }
                return new DoubleExpression(number);
            }
            else if (char.IsLetter(firstChar) || firstChar == '_')
            {
                string reference = "";
                head++;
                while (char.IsLetter(firstChar) || char.IsNumber(firstChar) || firstChar == '_')
                {
                    reference += firstChar;
                    firstChar = expression[head];
                    head++;
                }
                return new LookupExpression(reference);
            }
            throw new Exception();
        }
        private static Operator ParseOperator(string expression, ref int head)
        {
            char opChar = expression[head];
            head++;
            switch (opChar)
            {
                case '|':
                    AssertChar(expression, ref head, '|');
                    return Operator.OR;
                case '&':
                    AssertChar(expression, ref head, '&');
                    return Operator.AND;
                case '=':
                    AssertChar(expression, ref head, '=');
                    return Operator.EQU;
                case '!':
                    AssertChar(expression, ref head, '=');
                    return Operator.NEQ;
                case '<':
                    if (expression[head] == '=')
                    {
                        head++;
                        return Operator.LEQ;
                    }
                    else
                    {
                        return Operator.LES;
                    }
                case '>':
                    if (expression[head] == '=')
                    {
                        head++;
                        return Operator.GEQ;
                    }
                    else
                    {
                        return Operator.GRT;
                    }
                case '+':
                    return Operator.ADD;
                case '-':
                    return Operator.SUB;
                case '*':
                    return Operator.MUL;
                case '/':
                    return Operator.DIV;
                case '%':
                    return Operator.MOD;
            }
            return Operator.NON;
        }
        private static void SkipWhitespace(string expression, ref int head)
        {
            while (head < expression.Length)
            {
                char h = expression[head];
                if (h == ' ' || h == '\t' || h == '\n' || h == '\r')
                    head++;
                else
                    return;
            }
        }
        private static void AssertChar(string expression, ref int head, char expected)
        {
            char h = expression[head];
            head++;
            if (h != expected)
                throw new Exception();
        }

    }
    public abstract class Expression
    {
        public abstract ExpType GetExpType();
    }
    public class OpExpression : Expression
    {
        Operator op;
        List<Expression> args = new List<Expression>();

        public OpExpression(Operator op, List<Expression> args)
        {
            this.op = op;
            this.args = args;
        }
        public override ExpType GetExpType()
        {
            switch (op)
            {
                case Operator.OR:
                case Operator.AND:
                    foreach (Expression e in args)
                    {
                        if (e.GetExpType() != ExpType.boolExp)
                            return ExpType.invalidExp;
                    }
                    return ExpType.boolExp;
                case Operator.EQU:
                case Operator.NEQ:
                case Operator.LEQ:
                case Operator.LES:
                case Operator.GEQ:
                case Operator.GRT:
                    foreach (Expression e in args)
                    {
                        if (e.GetExpType() != ExpType.doubleExp)
                            return ExpType.invalidExp;
                    }
                    return ExpType.boolExp;
                case Operator.ADD:
                case Operator.SUB:
                case Operator.MUL:
                case Operator.DIV:
                case Operator.MOD:
                    foreach (Expression e in args)
                    {
                        if (e.GetExpType() != ExpType.doubleExp)
                            return ExpType.invalidExp;
                    }
                    return ExpType.doubleExp;
            }
            return ExpType.invalidExp;
        }
    }
    public class DoubleExpression : Expression
    {
        double value;
        public DoubleExpression(string toParse)
        {
            value = double.Parse(toParse);
        }
        public override ExpType GetExpType()
        {
            return ExpType.doubleExp;
        }
    }
    public class LookupExpression : Expression
    {
        string name;
        public LookupExpression(string reference)
        {
            name = reference;
        }
        public override ExpType GetExpType()
        {
            return ExpType.doubleExp;
        }
    }
    public enum ExpType
    {
        boolExp,
        doubleExp,
        invalidExp
    }
    public enum Operator
    {
        OR = 50,
        AND = 40,

        EQU = 30,
        NEQ = 31,
        LEQ = 32,
        LES = 33,
        GEQ = 34,
        GRT = 35,

        ADD = 20,
        SUB = 21,

        MUL = 10,
        DIV = 11,
        MOD = 12,

        NON = 0

    }
}