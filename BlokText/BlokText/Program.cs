using System;
using System.Collections.Generic;
using System.IO;
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
            int lastBufferLen = 0;
            int lastLineLen = 0;

            while (!inputFile.reader.EndOfStream)
            {
                char character = (char)inputFile.reader.Read(); 
                //skip all whitespace characters before first word
                while (character != '\uffff' && (character == '\n' || character == '\t' || character == ' '))
                {
                    character = (char)inputFile.reader.Read();
                }

                while (character != '\uffff')
                {
                    //read a word and add it to buffer
                    while (character != '\uffff' && character != '\n' && character != '\t' && character != ' ')
                    {
                        buffer.Append(character);
                        character = (char)inputFile.reader.Read();
                    }
                    //if a word is longer than the max length of line, output current line, add the buffer to a new line and output it
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
                    //if a character fits on a line, add it 
                    else if (buffer.Length + currLenght <= maxLength)
                    {
                        currLenght += buffer.Length + 1;
                        line.Add(buffer.ToString());
                        buffer.Clear();
                    }
                    //if the word fits on a line but can't be added to a current line, display current line, then add the word to the next line
                    else
                    {
                        WriteOutput(line, maxLength, name);
                        line.Clear();
                        line.Add(buffer.ToString());
                        currLenght = buffer.Length + 1;
                        buffer.Clear();
                    }

                    while (character != '\uffff' && (character == '\n' || character == '\t' || character == ' '))
                    {
                        //if there was a new line but no characters were read -> end of paragraph
                        if (character == '\n' && endOfLine && lastLineLen == line.Count && lastBufferLen == buffer.Length)
                        {
                            if (buffer.Length + currLenght <= maxLength)
                            {
                                line.Add(buffer.ToString());
                                WriteOutput(line, maxLength, name);
                                currLenght = 0;
                                line.Clear();
                                buffer.Clear();
                            }
                            else 
                            {
                                WriteOutput(line, maxLength, name);
                                line.Clear();
                                line.Add(buffer.ToString());
                                buffer.Clear();
                                WriteOutput(line, maxLength, name);
                                line.Clear();
                                currLenght = 0;
                            }
                            
                            line.Add(" ");
                            WriteOutput(line, maxLength, name);
                            line.Clear();
                            endOfLine = false;

                        }
                        // there was a line break but not an empty line after it
                        else if (character == '\n' && endOfLine && (lastBufferLen != buffer.Length || lastLineLen != line.Count))
                        {
                            endOfLine = false;
                        }

                        if (character == '\n' && endOfLine == false)
                        {
                            lastBufferLen = buffer.Length;
                            lastLineLen = line.Count;
                            endOfLine = true;
                        }
                        character = (char)inputFile.reader.Read();
                    }
                }
                //if there is something left for output
                WriteOutput(line, maxLength, name);

            }
        }
        
        public void WriteOutput(List<string> line, int maxLength, string name)
        {
            StringBuilder builder = new StringBuilder();

            

            using (StreamWriter writer = new StreamWriter(name, append: true))
            {


            }
           
        }
    }




}
