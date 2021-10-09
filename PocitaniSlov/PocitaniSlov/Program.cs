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
                reader.CheckFile(args[0]);
                counter.CountWords(args[0]);
                Console.WriteLine(counter.GetOutput());
            }
        }
    }

    class FileReader
    {

        public void CheckFile(string name)
        {
            try
            {
                File.Exists(name);
                File.OpenRead(name);
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                System.Environment.Exit(0);
            }
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
        
        public void CountWords(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);

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
