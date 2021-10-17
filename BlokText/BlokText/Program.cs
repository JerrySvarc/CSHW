using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Security;
using System.Text;

namespace BlokText
{
    class Program
    {
        static void Main(string[] args)
        {
            FileReader reader = new FileReader();
            TextWriter writer = new TextWriter();

            if (args.Length != 3 || !int.TryParse(args[2], out int maxLength) || maxLength < 1)
            {
                Console.WriteLine("Argument Error");
            }
            else
            {
                reader.GetInput(args[0]);
                writer.CreateOutputFile(args[1]);
                writer.FormatFile(reader, maxLength, args[1]);
            }
        }
    }

    interface IFileReader
    {
        public StreamReader GetInput(string name);
        public char ReadChar();
    }

    interface ITextWriter
    {
        public void CreateOutputFile(string name);
        public void FormatFile(FileReader reader, int maxLength, string name);
    }

    class FileReader : IFileReader
    {
        public StreamReader reader;
        private int characterVal;


        //Loads input file 
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
        public char ReadChar()
        {
            try
            {
                characterVal = reader.Read();
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                throw;
            }
            return (char)characterVal; ;
        }


    }

    class TextWriter : ITextWriter
    {
        
        public void CreateOutputFile(string name)
        {
            try
            {
                File.Create(name).Close();
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                throw;
            }
            
        }

        public void FormatFile(FileReader inputFile, int maxLength, string name)
        {
            StringBuilder builder = new StringBuilder();
            bool endOfLine = false;
            List<StringBuilder> buffer = new List<StringBuilder>();
            int currLenght = 0;
            char character = '1';
            
            while ((int)character > 0)
            {
                character = inputFile.ReadChar();
                if (character == ' ' || character == '\t' || endOfLine)
                {
                    if ((builder.Length + 1) + currLenght < maxLength)
                    {
                        currLenght += builder.Length+1;
                        buffer.Add(builder);
                        Console.WriteLine(builder);
                        builder = new StringBuilder();
                    }
                    else
                    {
                        WriteOutput(buffer, maxLength, name);
                        buffer.Clear();
                        buffer.Add(builder);
                        currLenght = builder.Length;
                        builder = new StringBuilder();
                    }
                }
                else if(character == '\n')
                {
                    endOfLine = true;
                }
                else
                {
                    builder.Append(character);
                }
            }
        }
        public void WriteOutput(List<StringBuilder> buffer, int maxLength, string name)
        {

            int gapCount = buffer.Count-1;
            int charCount = 0;
            int spacesLeft;
            // count the number of characters across all words + 1 for at least one space after each word
            foreach (var word in buffer)
            {
                charCount += word.Length+1;
            }

            charCount -= 1; // No space after the last word - number represents a normal line with 1 space between each word

            if (gapCount != 0)
            {
                 spacesLeft = (maxLength - charCount);
            }
            else
            {
                spacesLeft = maxLength - charCount;
            }

            StringBuilder line = new StringBuilder();
            if (gapCount != 0 && spacesLeft % gapCount == 0)
            {
                int spacesToAdd = spacesLeft / gapCount;
                
                foreach (var word in buffer)
                {
                    if (buffer.Count == 1)
                    {
                        line.Append(word);
                    }
                    else
                    {
                        line.Append(word).Append(' ', spacesToAdd + 1);
                    }
                    
                }
            }
            else if (buffer.Count == 1 && gapCount == 0)
            {
                line.Append(buffer[0]).Append(' ', spacesLeft);
            }
            else
            {
                int spacesToAdd = (spacesLeft / gapCount) + (spacesLeft%gapCount);

                foreach (var word in buffer)
                {
                    if (buffer.Count == 1)
                    {
                        line.Append(word);
                    }
                    else
                    {
                        line.Append(word).Append(' ', spacesToAdd + 1);
                        --spacesToAdd;
                    }

                }
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(name,append:true))
                {
                    writer.WriteLine(line);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("File Error");
                throw;
            }
        }
    }




}
