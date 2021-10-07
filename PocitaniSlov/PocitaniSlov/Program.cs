using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PocitaniSlov
{
    class Program
    {
        static void Main(string[] args)
        {
            Reader reader = new Reader();
            Counter counter = new Counter();
            string[] lines;
            if (args.Length != 1)
            {
                Console.WriteLine("Argument Error");
            }
            else
            {
                lines = reader.GetInput(args[0]);
                counter.CountWords(lines, counter);
                Console.WriteLine(counter.GetOutput());
            }

        }

    }

    class Reader
    {
        public string[] GetInput(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            return lines;
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

        public void CountWords(string[] lines, Counter counter)
        {
            string[] words;
            foreach (string line in lines)
            {
                
                words = line.Split(" ");

                foreach (string word in words)
                {
                    counter.IncrementCounter();
                }

            }
        }
    }
}
