using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                try
                {
                    reader.GetInput(args[0]);
                    writer.CreateOutputFile(args[1]);
                    writer.FormatFile(reader, maxLength, args[1]);
                }
                catch (FileNotFoundException)
                {
                    ReportFileError();
                }
                catch (IOException)
                {
                    ReportFileError();
                }
                catch (UnauthorizedAccessException)
                {
                    ReportFileError();
                }
                catch (System.Security.SecurityException)
                {
                    ReportFileError();
                }
            }
        }
        static void ReportFileError()
        {
            Console.WriteLine("File Error");
        }
    }

    interface IFileReader
    {
        public void GetInput(string name);
    }

    interface ITextWriter
    {
        public void CreateOutputFile(string name);
        public void FormatFile(FileReader reader, int maxLength, string name);
    }

    class FileReader : IFileReader
    {
        public StreamReader reader = null;

        //Loads input file 
        public void GetInput(string name)
        {
            reader = new StreamReader(name);
        }
    }

    class TextWriter : ITextWriter
    {

        public void CreateOutputFile(string name)
        {
            File.Create(name).Close();
        }

        public void FormatFile(FileReader inputFile, int maxLength, string name)
        {
            StringBuilder buffer = new StringBuilder();
            List<string> line = new List<string>();
            bool endOfLine = false;
            bool endOfFile = false;
            int currLenght = 0;
            

            while (!inputFile.reader.EndOfStream)
            {
                char character  = (char)inputFile.reader.Read(); ;
                while (character != '\uffff' && char.IsWhiteSpace(character))
                {
                    character = (char) inputFile.reader.Read();
                }

                while (character != '\uffff')
                {
                    while (character != '\uffff' && !char.IsWhiteSpace(character))
                    {
                        buffer.Append(character);
                        character = (char)inputFile.reader.Read();
                    }

                    if (buffer.Length >= maxLength)
                    {
                        WriteOutput(line, maxLength, name);
                        line.Clear();
                        line.Add(buffer.ToString());
                        buffer.Clear();
                        WriteOutput(line, maxLength, name);
                        line.Clear();
                        currLenght = 0;
                    }
                    else if (buffer.Length + currLenght <= maxLength)
                    {
                        currLenght += buffer.Length + 1;
                        line.Add(buffer.ToString());
                        buffer.Clear();
                    }
                    else
                    {
                        WriteOutput(line, maxLength, name);
                        line.Clear();
                        line.Add(buffer.ToString());
                        currLenght = buffer.Length + 1;
                        buffer.Clear();
                    }
                    

                    while (character != '\uffff' && char.IsWhiteSpace(character))
                    {
                        character = (char)inputFile.reader.Read();
                    }
                }
            }
        }

        public int ManageLine(StringBuilder buffer, List<string> line, int maxLength, string name, int currLenght)
        {
           

            return currLenght;
        }
        public void WriteOutput(List<string> line, int maxLength, string name)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var word in line)
            {
                if (word == line[line.Count -1])
                {
                    builder.Append(word).Append('\n');
                }
                else
                {
                    builder.Append(word).Append(" ");
                }
               
            }
            Console.Write(builder);
            

            try
            {
                using (StreamWriter writer = new StreamWriter(name, append: true))
                {
                    
                    
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
