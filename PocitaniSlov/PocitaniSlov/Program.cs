using System;
using System.IO;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace PocitaniSlov
{
    class Program
    {
        static void Main(string[] args)
        {
            Counter counter = new Counter();
            FileReader reader = new FileReader();
            if (args.Length != 1)
            {
                Console.WriteLine("Argument Error");
            }
            else
            {
                reader.GetFile(args[0]);
                counter.CountWords(reader.GetInput());
                Console.WriteLine(counter.GetOutput());
            }
        }
    }

    class FileReader
    {
        private StreamReader reader;
        public void GetFile(string name)
        {
            try
            {
                File.Exists(name);
                reader = new StreamReader(name);
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                System.Environment.Exit(0);
            }
        }

        public StreamReader GetInput()
        {
            return reader;
        }

    }

    class Counter
    {
        private int wordCount;

        public int GetOutput()
        {
            return wordCount;
        }

        public void IncrementCounter()
        {
            wordCount += 1;
        }
        
        public void CountWords(StreamReader reader)
        {
            

            while (!reader.EndOfStream)
            {
                int index = 0;
                string line = reader.ReadLine();

                while (index < line.Length && char.IsWhiteSpace(line[index]))
                {
                    ++index;
                }


                while (index < line.Length)
                {
                    
                    while (index < line.Length && !char.IsWhiteSpace(line[index]))
                    {
                        ++index;
                    }

                    IncrementCounter();

                    while (index < line.Length && char.IsWhiteSpace(line[index]))
                    {
                        ++index;
                    }

                    
                }
            }
        }
    }
}
