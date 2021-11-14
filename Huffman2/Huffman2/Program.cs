using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Huffman2
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Argument Error");
            }
            else
            {
                try
                {
                    Outputer output = new Outputer(args[1]);
                    Tree tree = new Tree();
                    PriorityQueue queue = new PriorityQueue();
                    Input input = new Input(args[0]);
                    Counter counter = new Counter();

                    //TODO: Build it all together - finish main()
                    tree.BuildTree(input, queue, counter);


                    tree.OutputTree(output);
                }
                catch (FileNotFoundException)
                {
                    WriteFileError();
                }
                catch (IOException)
                {
                    WriteFileError();
                }
                catch (UnauthorizedAccessException)
                {
                    WriteFileError();
                }
                catch (System.Security.SecurityException)
                {
                    WriteFileError();
                }
            }
        }
        static void WriteFileError()
        {
            Console.WriteLine("File Error");
        }

    }

    class Outputer
    {
        BinaryWriter writer;
        public Outputer(string fileName)
        {
            writer = new BinaryWriter(new FileStream(fileName, FileMode.Create));
        }
        //TODO: Implement writing output
        public void WriteOutput(byte value, BinaryWriter writer)
        {

            


        }

        public void WriteTree(Node node)
        {
            if (node == null)
            {
                return;
            }

            if (node.Symbol != -1)
            {
                Console.Write("*" + node.Symbol + ":" + node.Weight + " ");
            }
            else
            {
                Console.Write(node.Weight + " ");
            }

            WriteTree(node.LeftChild);
            WriteTree(node.RightChild);
        }




    }

    class Counter
    {
        private Dictionary<int, int> SymbolCount = new Dictionary<int, int>();
        public Dictionary<int, int> GetSymbolCount(Input input)
        {
            int symbol = input.ReadSymbol();
            while (symbol != -1)
            {
                if (SymbolCount.ContainsKey(symbol))
                {
                    SymbolCount[symbol] += 1;
                }
                else
                {
                    SymbolCount.Add(symbol, 1);
                }
                symbol = input.ReadSymbol();

            }
            return SymbolCount;
        }

        public void LoadQueue(Dictionary<int, int> symbolCount, PriorityQueue queue)
        {
            foreach (var symbol in symbolCount)
            {
                Node node = new Node(null, null, symbol.Key, symbol.Value, 0);
                queue.Queue(node, node.Weight);
            }
        }
    }

    class Input
    {
        public FileStream file;

        public Input(string name)
        {
            file = File.Open(name, FileMode.Open);
        }

        public int ReadSymbol()
        {
            int value = file.ReadByte();
            return value;
        }
    }

    class Node
    {
        public Node LeftChild { get; }
        public Node RightChild { get; }
        public int Symbol { get; }
        public int Weight { get; }

        public int Turn { get; }
        public Node(Node leftNode, Node rightNode, int symbol, int weight, int turn)
        {
            LeftChild = leftNode;
            RightChild = rightNode;
            Symbol = symbol;
            Weight = weight;
            Turn = turn;
        }
    }

    class Tree
    {
        public HashSet<Node> tree = new HashSet<Node>();
        
        public Node Root;
        public void AddNode(Node node)
        {
            tree.Add(node);
        }

        public void BuildTree(Input input, PriorityQueue queue, Counter counter)
        {
            counter.LoadQueue(counter.GetSymbolCount(input), queue);
            int turn = 0;
            while (queue.GetHeapCount() > 1)
            {
                Node left = queue.Dequeue();
                Node right = queue.Dequeue();
                ++turn;
                Node newNode = new Node(left, right, -1, left.Weight + right.Weight, turn);
                tree.Add(left);
                tree.Add(right);
                tree.Add(newNode);
                queue.Queue(newNode, newNode.Weight);
            }

            Root = queue.Dequeue();
        }

        public void OutputTree(Outputer output)
        {
            output.WriteTree(Root);
        }
    }

    class PriorityQueue
    {
        private SortedSet<Tuple<Node, int>>
            heap = new SortedSet<Tuple<Node, int>>(
                Comparer<Tuple<Node, int>>.Create(
                    (x, y) => {
                        if (x.Item2 == y.Item2)
                        {
                            if (x.Item1.Turn == 0 && y.Item1.Turn != 0)
                            {
                                return -1;
                            }
                            if (x.Item1.Turn != 0 && y.Item1.Turn == 0)
                            {
                                return 1;
                            }

                            if (x.Item1.Turn == 0 && y.Item1.Turn == 0)
                            {
                                if (x.Item1.Symbol < y.Item1.Symbol)
                                {
                                    return -1;
                                }
                                if (x.Item1.Symbol > y.Item1.Symbol)
                                {
                                    return 1;
                                }
                            }

                            if (x.Item1.Turn != 0 && y.Item1.Turn != 0)
                            {
                                if (x.Item1.Turn < y.Item1.Turn)
                                {
                                    return -1;
                                }
                                if (x.Item1.Turn > y.Item1.Turn)
                                {
                                    return 1;
                                }
                            }
                        }
                        return x.Item2 - y.Item2;
                    }
                ));

        public Node Dequeue()
        {
            using IEnumerator<Tuple<Node, int>> enumerator = heap.GetEnumerator();
            enumerator.MoveNext();
            heap.Remove(enumerator.Current);
            return enumerator.Current.Item1;
        }

        public void Queue(Node node, int weight)
        {
            heap.Add(new Tuple<Node, int>(node, weight));
        }

        public int GetHeapCount()
        {
            return heap.Count;
        }
    }
    //TODO: implement building output bytes
    class ByteBuilder
    {





    }

}