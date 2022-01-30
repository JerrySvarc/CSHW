using System;
using System.Collections;
using System.Collections.Generic;

namespace Expressions3
{
    class Program
    {
        static void Main(string[] args)
        {
            Expression expr = null;
            int? iResult = null;
            double? dResult = null;
            string input = "";
            while (input != null && input != "end")
            {
                if (input == String.Empty)
                {
                    //Do nothing
                }
                //Epression
                else if (input[0] == '=')
                {
                    expr = null;
                    if (input.Length >= 3 && input[1] == ' ' && input[2] != ' ')
                    {
                        expr = Expression.ParsePrefixExpression(input[1..]);
                        iResult = null;
                        dResult = null;
                    }
                    if (expr == null)
                    {
                        Console.WriteLine("Format Error");
                    }

                }
                //"i" is selected
                else if (input == "i")
                {
                    if (expr == null)
                    {
                        Console.WriteLine("Expression Missing");
                    }
                    else
                    {
                        if (iResult == null)
                        {
                            try
                            {
                                IntegerVisitor visitor = new IntegerVisitor();
                                iResult = expr.AcceptIntegerVisitor(visitor);
                                Console.WriteLine(iResult);
                            }
                            catch (DivideByZeroException)
                            {
                                Console.WriteLine("Divide Error");
                            }
                            catch (OverflowException)
                            {
                                Console.WriteLine("Overflow Error");
                            }
                        }
                        else
                        {
                            Console.WriteLine(iResult);
                        }

                    }

                }
                //"d" is selected
                else if (input == "d")
                {
                    if (expr == null)
                    {
                        Console.WriteLine("Expression Missing");

                    }
                    else
                    {
                        if (dResult == null)
                        {
                            RealNumberVisitor visitor = new RealNumberVisitor();
                            dResult = expr.AcceptRealVisitor(visitor);
                            Console.WriteLine(((double)dResult).ToString("f05"));
                        }
                        else
                        {
                            Console.WriteLine(((double)dResult).ToString("f05"));

                        }

                    }

                }
                else
                {
                    Console.WriteLine("Format Error");
                }
                input = Console.ReadLine();
            }
        }
    }
    interface IVisitor
    {
        //Implementation must represent a Visitor class
    }

    class RealNumberVisitor : IVisitor
    {
        public double Visit(PlusExpr plusExpr)
        {
            return plusExpr.OperandLeft.AcceptRealVisitor(this) + plusExpr.OperandRight.AcceptRealVisitor(this);
        }

        public double Visit(MinusExpr minusExpr)
        {
            return minusExpr.OperandLeft.AcceptRealVisitor(this) - minusExpr.OperandRight.AcceptRealVisitor(this);
        }

        public double Visit(MultiplyExpr multiplyExpr)
        {
            return multiplyExpr.OperandLeft.AcceptRealVisitor(this) * multiplyExpr.OperandRight.AcceptRealVisitor(this);
        }

        public double Visit(DivideExpr divideExpr)
        {
            return divideExpr.OperandLeft.AcceptRealVisitor(this) / divideExpr.OperandRight.AcceptRealVisitor(this);
        }

        public double Visit(UnaryMinusExpr unaryMinusExpr)
        {
            return -(unaryMinusExpr.Operand.AcceptRealVisitor(this));
        }

        public double Visit(ConstantExpr constantExpr)
        {
            return (double)constantExpr.Value;
        }
    }

    class IntegerVisitor : IVisitor
    {
        public int Visit(PlusExpr plusExpr)
        {
            return checked(plusExpr.OperandLeft.AcceptIntegerVisitor(this) + plusExpr.OperandRight.AcceptIntegerVisitor(this));
        }

        public int Visit(MinusExpr minusExpr)
        {
            return checked(minusExpr.OperandLeft.AcceptIntegerVisitor(this) - minusExpr.OperandRight.AcceptIntegerVisitor(this));
        }

        public int Visit(MultiplyExpr multiplyExpr)
        {
            return checked(multiplyExpr.OperandLeft.AcceptIntegerVisitor(this) * multiplyExpr.OperandRight.AcceptIntegerVisitor(this));
        }

        public int Visit(DivideExpr divideExpr)
        {
            return checked(divideExpr.OperandLeft.AcceptIntegerVisitor(this) / divideExpr.OperandRight.AcceptIntegerVisitor(this));
        }

        public int Visit(UnaryMinusExpr unaryMinusExpr)
        {
            return checked(-unaryMinusExpr.Operand.AcceptIntegerVisitor(this));
        }

        public int Visit(ConstantExpr constantExpr)
        {
            return checked(constantExpr.Value);
        }
    }
    abstract class Expression
    {
        public static Expression ParsePrefixExpression(string exprString)
        {
            string[] tokens = exprString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Expression result = null;
            Stack<OperatorExpr> unresolved = new Stack<OperatorExpr>();
            foreach (string token in tokens)
            {
                if (result != null)
                {
                    // We correctly parsed the whole tree, but there was at least one more unprocessed token left.
                    // This implies incorrect input, thus return null.

                    return null;
                }

                switch (token)
                {
                    case "+":
                        unresolved.Push(new PlusExpr());
                        break;

                    case "-":
                        unresolved.Push(new MinusExpr());
                        break;

                    case "*":
                        unresolved.Push(new MultiplyExpr());
                        break;

                    case "/":
                        unresolved.Push(new DivideExpr());
                        break;

                    case "~":
                        unresolved.Push(new UnaryMinusExpr());
                        break;

                    default:
                        int value;
                        if (!int.TryParse(token, out value))
                        {
                            return null;    // Invalid token format
                        }

                        Expression expr = new ConstantExpr(value);
                        while (unresolved.Count > 0)
                        {
                            OperatorExpr oper = unresolved.Peek();
                            if (oper.AddOperand(expr))
                            {
                                unresolved.Pop();
                                expr = oper;
                            }
                            else
                            {
                                expr = null;
                                break;
                            }
                        }

                        if (expr != null)
                        {
                            result = expr;
                        }

                        break;
                }
            }

            return result;
        }

        public abstract double AcceptRealVisitor(RealNumberVisitor visitor);
        public abstract int AcceptIntegerVisitor(IntegerVisitor visitor);
    }
    abstract class ValueExpr : Expression
    {
        public abstract int Value { get; }
    }

    sealed class ConstantExpr : ValueExpr
    {
        private int value;
        public override int Value
        {
            get { return this.value; }
        }

        public ConstantExpr(int value)
        {
            this.value = value;
        }

        public override double AcceptRealVisitor(RealNumberVisitor visitor)
        {
            return visitor.Visit(this);
        }
        public override int AcceptIntegerVisitor(IntegerVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }



    abstract class OperatorExpr : Expression
    {
        public abstract bool AddOperand(Expression op);
    }
    abstract class UnaryOpExpr : OperatorExpr
    {
        protected Expression operand;

        public Expression Operand
        {
            get { return operand; }
            set { operand = value; }
        }

        public sealed override bool AddOperand(Expression op)
        {
            if (this.operand == null)
            {
                this.operand = op;
            }
            return true;
        }

    }
    abstract class BinaryOpEpr : OperatorExpr
    {
        protected Expression left;
        protected Expression right;

        public Expression OperandLeft { get => left; set { left = value; } }
        public Expression OperandRight { get => right; set { right = value; } }

        public sealed override bool AddOperand(Expression operand)
        {
            if (left == null)
            {
                left = operand;
                return false;
            }
            else if (right == null)
            {
                right = operand;
            }
            return true;
        }


    }
    sealed class UnaryMinusExpr : UnaryOpExpr
    {
        public override double AcceptRealVisitor(RealNumberVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public override int AcceptIntegerVisitor(IntegerVisitor visitor)
        {
            return visitor.Visit(this);
        }



    }
    sealed class PlusExpr : BinaryOpEpr
    {
        public override double AcceptRealVisitor(RealNumberVisitor visitor)
        {

            return visitor.Visit(this);
        }
        public override int AcceptIntegerVisitor(IntegerVisitor visitor)
        {
            return visitor.Visit(this);
        }

    }
    sealed class MinusExpr : BinaryOpEpr
    {
        public override double AcceptRealVisitor(RealNumberVisitor visitor)
        {
            return visitor.Visit(this);
        }
        public override int AcceptIntegerVisitor(IntegerVisitor visitor)
        {
            return visitor.Visit(this);
        }

    }
    sealed class DivideExpr : BinaryOpEpr
    {
        public override double AcceptRealVisitor(RealNumberVisitor visitor)
        {
            return visitor.Visit(this);
        }
        public override int AcceptIntegerVisitor(IntegerVisitor visitor)
        {
            return visitor.Visit(this);
        }

    }
    sealed class MultiplyExpr : BinaryOpEpr
    {
        public override double AcceptRealVisitor(RealNumberVisitor visitor)
        {
            return visitor.Visit(this);
        }
        public override int AcceptIntegerVisitor(IntegerVisitor visitor)
        {
            return visitor.Visit(this);
        }

    }
}