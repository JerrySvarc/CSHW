using System;
using System.Collections;
using System.Collections.Generic;

namespace Expressions2
{
    class Program
    {
        static void Main(string[] args)
        {
            Expression expr = null;
            int? iResult = null;
            double? dResult = null; 
            string input = Console.ReadLine();
            while (input != null && input != "end")
            {
                if(input == "")
                {
                    input = Console.ReadLine();
                    continue;
                }
                else if(input.Length >= 7 && input[0] == '=' && input[1] == ' ' )
                {
                    expr = Expression.ParsePrefixExpression(input[1..]);
                    iResult = null; 
                    dResult = null;
                    if (expr == null)
                    {
                        Console.WriteLine("Format Error");
                        input = Console.ReadLine();
                        continue;
                    }

                }
                else if(input == "i")
                {
                    if(expr == null)
                    {
                        Console.WriteLine("Expression Missing");
                    }
                    else
                    {
                        if(iResult == null)
                        {
                            try
                            {
                                Console.WriteLine(expr.RecursiveEval());
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
                else if(input == "d" )
                {
                    if(expr == null)
                    {
                        Console.WriteLine("Expression Missing");
                    }
                    else
                    {
                        if(dResult == null)
                        {
                            try
                            {
                                RealNumberVisitor visitor = new RealNumberVisitor();
                                Console.WriteLine(expr.Accept(visitor).ToString("f05"));
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
        double Visit(ConstantExpr consatantExpr);
        double Visit(PlusExpr plusExpr);
        double Visit(MinusExpr minusExpr);
        double Visit(MultiplyExpr multiplyExpr);
        double Visit(DivideExpr divideExpr);
        double Visit(UnaryMinusExpr unaryMinusExpr);
    }
    
    class RealNumberVisitor : IVisitor
    {
        public double Visit(PlusExpr plusExpr)
        {
          return checked(plusExpr.OperandLeft.Accept(this) + plusExpr.OperandRight.Accept(this)); 
        }

        public double Visit(MinusExpr minusExpr)
        {
            return checked(minusExpr.OperandLeft.Accept(this) - minusExpr.OperandRight.Accept(this));
        }

        public double Visit(MultiplyExpr multiplyExpr)
        {
            return checked(multiplyExpr.OperandLeft.Accept(this) * multiplyExpr.OperandRight.Accept(this));
        }

        public double Visit(DivideExpr divideExpr)
        {
            return checked(divideExpr.OperandLeft.Accept(this) / divideExpr.OperandRight.Accept(this));
        }

        public double Visit(UnaryMinusExpr unaryMinusExpr)
        {
            return checked(-unaryMinusExpr.Operand.Accept(this));
        }

        public double Visit(ConstantExpr constantExpr)
        {
            return (double)constantExpr.Value;
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
        public abstract int RecursiveEval();
        public abstract double Accept(IVisitor visitor);
    }
    abstract class ValueExpr: Expression
    {
        public abstract int Value { get;}
        public override int RecursiveEval() => Value;
    }

    sealed class ConstantExpr : ValueExpr
    {
        private int value;
        public override int Value
        {
            get { return value; }
        }

        public ConstantExpr(int value)
        {
            this.value = value;
        }

        public override double Accept(IVisitor visitor)
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

        public sealed override int RecursiveEval()
        {
            return Eval(operand.RecursiveEval());
        }

        protected abstract int Eval(int opValue);
        
    }
    abstract class BinaryOpEpr: OperatorExpr
    {
        protected Expression left;
        protected Expression right;

        public Expression OperandLeft { get => left; set { left = value; } }
        public Expression OperandRight { get => right; set { right = value; } }
        public override int RecursiveEval()
        {
            return Eval(left.RecursiveEval(), right.RecursiveEval());
        }
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
        protected abstract int Eval(int leftVal, int rightVal);
        
    }
    sealed class UnaryMinusExpr : UnaryOpExpr
    {
        public override double Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }

        protected override int Eval(int opValue)
        {
            return checked(-opValue);
        }
    }
    sealed class PlusExpr : BinaryOpEpr
    {
        public override double Accept(IVisitor visitor)
        {
            OperandLeft.Accept(visitor);
            OperandRight.Accept(visitor);
           return visitor.Visit(this);
        }

        protected override int Eval(int leftVal, int rightVal)
        {
            return checked(leftVal + rightVal);
        }
    }
    sealed class MinusExpr : BinaryOpEpr
    {
        public override double Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }

        protected override int Eval(int leftVal, int rightVal)
        {
            return checked(leftVal - rightVal);
        }
    }
    sealed class DivideExpr : BinaryOpEpr
    {
        public override double Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }

        protected override int Eval(int leftVal, int rightVal)
        {
            return checked(leftVal / rightVal);
        }
    }
    sealed class MultiplyExpr : BinaryOpEpr
    {
        public override double Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }

        protected override int Eval(int leftVal, int rightVal)
        {
            return checked(leftVal * rightVal);
        }
    }
}