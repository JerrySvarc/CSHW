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