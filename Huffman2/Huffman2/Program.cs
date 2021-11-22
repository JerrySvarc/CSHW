using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace Huffman2
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Argument Error");
                return;
            }
            else
            {
                try
                {
                    string outputFile = args[0] + ".huff";
                    Outputer output = new Outputer(outputFile);
                    Tree tree = new Tree();
                    PriorityQueue queue = new PriorityQueue();
                    Input input = new Input(args[0]);
                    Counter counter = new Counter();
                    FileEncoder encoder = new FileEncoder();
                    using (output.writer)
                    {
                        //TODO: Build it all together - finish main()
                        tree.BuildTree(input, queue, counter);
                        input.file.Close();
                        input = new Input(args[0]);
                        //write header
                        output.WriteHeader();
                        //write coded tree
                        tree.OutputTree(output);
                        output.WriteTreeEnd();
                        //write encoded file contents
                        List<bool> list = new List<bool>();
                        encoder.FindCharCoding(tree.Root, list);
                        output.WriteEncodedFile(encoder.GetCodingTemplates(), input);

                    }

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
        public BinaryWriter writer;
        byte[] result = new byte[1];
        List<bool> buffer = new List<bool>(1024);
        BitArray bits = new BitArray(8);
        public Outputer(string fileName)
        {
            writer = new BinaryWriter(new FileStream(fileName, FileMode.OpenOrCreate));
        }
        //TODO: Implement writing output
        public void WriteEncodedFile(Dictionary<byte, List<bool>> templates, Input input)
        {
            
        }
        public void WriteEncodedByte(BitArray bits)
        {
            bits.CopyTo(result, 0);
            writer.Write(result);
        }

        public void WriteTreeEnd()
        {
            byte[] footer = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            writer.Write(footer);
        }
        public void WriteHeader()
        {
            byte[] header = new byte[] { 0x7B, 0x68, 0x75, 0x7C, 0x6D, 0x7D, 0x66, 0x66 };
            writer.Write(header);
        }

        public void WriteInner(Node node)
        {
            long result = 0;
            long weight = node.Weight;
            weight = weight & 0x00007FFFFFFFFFFF;
            result = weight << 1;
            writer.Write(result);
        }

        public void WriteLeaf(Node node)
        {
            long result = 0;
            long weight = node.Weight;
            byte symbol = (byte)node.Symbol;
            weight = ((weight & 0x00007FFFFFFFFFFF) << 1) + 1;
            result = (result | symbol) << 56;
            result |= weight;
            writer.Write(result);
        }
        public void WriteTree(Node node)
        {
            if (node == null)
            {
                return;
            }

            Stack<Node> stack = new Stack<Node>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                Node currNode = stack.Pop();
                if (currNode.Symbol != -1)
                {
                    WriteLeaf(currNode);
                }
                else
                {
                    WriteInner(currNode);
                    stack.Push(currNode.RightChild);
                    stack.Push(currNode.LeftChild);
                }

            }

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


        public Node Root;

        /// <summary>
        /// Builds a Huffman tree from Input file. Inner nodes have a symbol value of "-1". 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="queue"></param>
        /// <param name="counter"></param>
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
        /// <summary>
        /// A priority queue implementation using a SortedSet with a custom comparer. 
        /// </summary>
        private SortedSet<Tuple<Node, int>>
            heap = new SortedSet<Tuple<Node, int>>(
                Comparer<Tuple<Node, int>>.Create(
                    (x, y) =>
                    {
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

        /// <summary>
        /// Removes item from the top of the priotity queue.
        /// </summary>
        /// <returns></returns>
        public Node Dequeue()
        {
            using IEnumerator<Tuple<Node, int>> enumerator = heap.GetEnumerator();
            enumerator.MoveNext();
            heap.Remove(enumerator.Current);
            return enumerator.Current.Item1;


        }
        /// <summary>
        /// Adds item to the priority queue.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="weight"></param>
        public void Queue(Node node, int weight)
        {
            heap.Add(new Tuple<Node, int>(node, weight));
        }

        public int GetHeapCount()
        {
            return heap.Count;
        }
    }

    class FileEncoder
    {
        //TODO: IMPLEMENT ME!

        /// <summary>
        /// Recursively find encoding template for all leaves.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="bits"></param>
        public void FindCharCoding(Node node, List<bool> bits)
        {

        }
        public Dictionary<byte, List<bool>> GetCodingTemplates()
        {
            //return codingTemplates;
        }
    }
}
