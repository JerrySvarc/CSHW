using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                counter.GetOutput();
            }
        }
    }

    class FileReader
    {
        private StreamReader reader;
        /// <summary>
        /// Function for checking if a file exists. Also tries to open the file using StreamReader.
        /// </summary>
        /// <param name="name"></param>
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
        Dictionary<string, int> result = new Dictionary<string, int>();
        
        /// <summary>
        /// Function designed to output a dictionary. Dictionary is ordered by the keys. 
        /// </summary>
        public void GetOutput()
        {
            foreach (var item in result.OrderBy(x => x.Key))
            {
                if(item.Key != "")
                    Console.WriteLine(item.Key + ": " + item.Value);
            }
        }
        /// <summary>
        /// Function for counting the frequency of words in a file. Results are stored in a dictionary. 
        /// </summary>
        /// <param name="reader"></param>
        public void CountWords(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] words = line.Split(null);

                foreach (var word in words)
                {
                    if (!result.ContainsKey(word))
                    {
                        result.Add(word, 0);
                    }
                }
                foreach (var word in words)
                {
                    
                    result[word] += 1;
                    

                }
            }
        }
    }
}
