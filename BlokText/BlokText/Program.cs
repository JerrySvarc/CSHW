using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlokText
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3 || !int.TryParse(args[2], out int value) || value < 1)
            {
                Console.WriteLine("Argument Error");
            }
        }
    }

    public interface IFileReader
    {
        public StreamReader GetInput(string name);
        public char ReadChar(StreamReader reader);
    }

    public interface ITextWriter
    {
        public void WriteSpace();
    }

    class FileReader : IFileReader
    {
        private StreamReader reader;
        private char character;
        private int characterVal;

        public StreamReader GetInput(string name)
        {
            try
            {
                reader = new StreamReader(name);
                
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                throw;
            }
            return reader;
        }

        public char ReadChar(StreamReader reader)
        {
            try
            {
                characterVal = reader.Read();
                character = (char) characterVal;
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                throw;
            }
            return character;
        }
    }

    
}
