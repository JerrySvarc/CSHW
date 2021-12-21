using System;
using System.Collections.Generic;

namespace Expressions
{

    //UNIT testy se nachazi na konci souboru

    interface INode
    {
        /// <contract>
        /// INode implementation must represent a Node. Specify the number of child nodes, data that it holds etc.
        /// </contract>
    }
    interface IInput
    {
        public string GetInput();
    }
    interface IOutput
    {
        public void OutputResult(Node resultNode);
    }
    interface ITree
    {
        public Node BuildTree(string input);
    }

    public class Node : INode
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
    }
    public class OperandNode : Node
    {
        public int OperandVal { get; set; }

        public OperandNode(int operandVal, Node left, Node right)
        {
            OperandVal = operandVal;
            Left = left;
            Right = right;
        }
    }
    public class OperatorNode : Node
    {
        public string OperatorType { get; set; }

        public OperatorNode(string operatorType, Node left, Node right)
        {
            OperatorType = operatorType;
            Left = left;
            Right = right;
        }
    }

    public class ErrorNode : Node
    {
        public ErrorNode(string error)
        {
            Error = error;
        }

        public string Error { get; set; }
    }

    public class Inputer : IInput
    {
        public string GetInput()
        {
            return Console.ReadLine();
        }

    }
    public class Outputer : IOutput
    {
        public void OutputResult(Node resultNode)
        {
            if (resultNode is ErrorNode)
            {
                Console.WriteLine(((ErrorNode)resultNode).Error);
            }
            else
            {
                Console.WriteLine(((OperandNode)resultNode).OperandVal);
            }

        }
    }


   public class ExpTree : ITree
    {
        /// <summary>
        /// Function for building the expression tree from a string. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Node BuildTree(string input)
        {
            Node root = null;
            
            string[] line = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            if (line.Length < 3)
            {
                return new ErrorNode("Format Error");
            }
            else
            {
                try
                {
                    return root = AddToTree(line);
                }
                catch (Exception)
                {

                    return new ErrorNode("Format Error");
                }

            }

        }
        /// <summary>
        /// Function for building the expression tree from a string[].
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public Node AddToTree(string[] line)
        {
            Stack<Node> stack = new Stack<Node>();

            for (int i = line.Length; i > 0; --i)
            {
                if (int.TryParse(line[i - 1], out int value))
                {
                    stack.Push(new OperandNode(value, null, null));
                }
                else if (line[i - 1] == "+" || line[i - 1] == "-"
                || line[i - 1] == "*" || line[i - 1] == "/" || line[i - 1] == "~")
                {
                    if (line[i - 1] == "~")
                    {
                        Node left = stack.Pop();
                        if (left is OperandNode)
                        {
                            OperandNode leftOperand = (OperandNode)left;
                            stack.Push(new OperandNode(-1 * leftOperand.OperandVal, null, null));
                            continue;
                        }

                    }
                    if (stack.Count >= 2)
                    {
                        Node left = stack.Pop();
                        Node right = stack.Pop();
                        stack.Push(new OperatorNode(line[i - 1], left, right));
                    }
                    else
                    {
                        Node left = stack.Pop();
                        stack.Push(new OperatorNode(line[i - 1], left, null));
                    }
                }
                else
                {
                    i = 0;
                    return new ErrorNode("Format Error");
                }
            }
            Node result =  stack.Pop();
            if(stack.Count > 0)
            {
                return new ErrorNode("Format Error");
            }
            else
            {
                return result;
            }
        }
        /// <summary>
        /// Function for recursively resolving the expression tree.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public Node ResolveTree(Node root)
        {
            if (root is ErrorNode)
            {
                return root;
            }
            if (root is OperandNode)
            {
                return root;
            }
            else if (root is OperatorNode)
            {
                Node leftOperand = ResolveTree(root.Left);
                Node rightOperand = ResolveTree(root.Right);
                try
                {
                    checked
                    {
                        switch (((OperatorNode)root).OperatorType)
                        {
                            case "+":
                                return new OperandNode(((OperandNode)leftOperand).OperandVal + ((OperandNode)rightOperand).OperandVal, null, null);
                            case "-":
                                return new OperandNode(((OperandNode)leftOperand).OperandVal - ((OperandNode)rightOperand).OperandVal, null, null);
                            case "*":
                                return new OperandNode(((OperandNode)leftOperand).OperandVal * ((OperandNode)rightOperand).OperandVal, null, null);
                            case "/":
                                try
                                {
                                    return new OperandNode(((OperandNode)leftOperand).OperandVal / ((OperandNode)rightOperand).OperandVal, null, null);
                                }
                                catch (DivideByZeroException)
                                {

                                    return new ErrorNode("Divide Error");
                                }

                            default:
                                return null;
                        }
                    }
                }
                catch (Exception)
                {
                    return new ErrorNode("Overflow Error");
                }
            }
            else
            {
                return new ErrorNode("Format Error");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Inputer inputer = new Inputer();
            Outputer outputer = new Outputer();
            ExpTree expTree = new ExpTree();
            var treeRoot = expTree.BuildTree(inputer.GetInput());

            Node result = expTree.ResolveTree(treeRoot);
            outputer.OutputResult(result);
        }
    }

}

/*
UNIT TESTY


using Expressions;
using NUnit.Framework;
namespace ExpTests
{
    public class Tests
    {

        [Test]
        public void SimpleExp()
        {
            string input = " + ~ 1 3";
            Inputer inputer = new Inputer();
            Outputer outputer = new Outputer();
            ExpTree expTree = new ExpTree();
            var treeRoot = expTree.BuildTree(input);
            Node result = expTree.ResolveTree(treeRoot);
            if(result is ErrorNode)
            {
                Assert.Fail();
            }
            else if(result is OperandNode)
            {
                Assert.AreEqual(2, ((OperandNode)result).OperandVal);
                Assert.Pass();
            }
            
        }

        [Test]
        public void ComplexExpression()
        {
            string input = "/ + - 5 2 * 2 + 3 3 ~ 2";
            Inputer inputer = new Inputer();
            Outputer outputer = new Outputer();
            ExpTree expTree = new ExpTree();
            var treeRoot = expTree.BuildTree(input);
            Node result = expTree.ResolveTree(treeRoot);
            if (result is ErrorNode)
            {
                Assert.Fail();
            }
            else if (result is OperandNode)
            {
                Assert.AreEqual(-7, ((OperandNode)result).OperandVal);
                Assert.Pass();
            }

        }

        [Test]
        public void Overflow()
        {
            string input = " - - 2000000000 2100000000 2100000000";
            Inputer inputer = new Inputer();
            Outputer outputer = new Outputer();
            ExpTree expTree = new ExpTree();
            var treeRoot = expTree.BuildTree(input);
            Node result = expTree.ResolveTree(treeRoot);
            if (result is ErrorNode)
            {
                Assert.AreEqual("Overflow Error", ((ErrorNode)result).Error);
                Assert.Pass();
            }
            else if (result is OperandNode)
            {
                Assert.Fail();   
            }
        }

        [Test]
        public void ZeroDivision()
        {
            string input = " / 100 - + 10 10 20";
            Inputer inputer = new Inputer();
            Outputer outputer = new Outputer();
            ExpTree expTree = new ExpTree();
            var treeRoot = expTree.BuildTree(input);
            Node result = expTree.ResolveTree(treeRoot);
            if (result is ErrorNode)
            {
                Assert.AreEqual("Divide Error", ((ErrorNode)result).Error);
                Assert.Pass();
            }
            else if (result is OperandNode)
            {
                Assert.Fail();
            }

        }

        [Test]
        public void TooManyOperands()
        {
            string input = "+- 1 2 3";
            Inputer inputer = new Inputer();
            Outputer outputer = new Outputer();
            ExpTree expTree = new ExpTree();
            var treeRoot = expTree.BuildTree(input);
            Node result = expTree.ResolveTree(treeRoot);
            if (result is ErrorNode)
            {
                Assert.AreEqual("Format Error", ((ErrorNode)result).Error);
                Assert.Pass();
            }
            else if (result is OperandNode)
            {
                Assert.Fail();
            }

        }

        [Test]
        public void OperandTooLarge()
        {
            string input = "- 2000000000 4000000000";
            Inputer inputer = new Inputer();
            Outputer outputer = new Outputer();
            ExpTree expTree = new ExpTree();
            var treeRoot = expTree.BuildTree(input);
            Node result = expTree.ResolveTree(treeRoot);
            if (result is ErrorNode)
            {
                Assert.AreEqual("Format Error", ((ErrorNode)result).Error);
                Assert.Pass();
            }
            else if (result is OperandNode)
            {
                Assert.Fail();
            }

        }
       
    }
}
*/