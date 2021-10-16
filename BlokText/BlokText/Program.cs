using System;
using System.IO;

namespace BlokText
{
    class Program
    {
        static void Main(string[] args)
        {
            FileReader reader = new FileReader();
            TextWriter writer = new TextWriter();

            if (args.Length != 3 || !int.TryParse(args[2], out int value) || value < 1)
            {
                Console.WriteLine("Argument Error");
            }
            else
            {

            }
        }
    }

    interface IFileReader
    {
        public StreamReader GetInput(string name);
        public char ReadChar(StreamReader reader);
    }

    public interface ITextWriter
    {
        public void CreateOutputFile(string name);
        public void WriteText(string text, StreamWriter writer);
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
        // Reads one character from the input file 
        public char ReadChar(StreamReader reader)
        {
            try
            {
                characterVal = reader.Read();
                character = (char)characterVal;
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                throw;
            }
            return character;
        }


    }

    class TextWriter : ITextWriter
    {
        private StreamWriter writer;


        public void CreateOutputFile(string name)
        {
            try
            {
                writer = new StreamWriter(name);
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                throw;
            }
            
        }


        public void WriteText(string text, StreamWriter writer)
        {
            writer.WriteLine(text);
        }

        public void FormatLine()
        {

        }
    }




}
