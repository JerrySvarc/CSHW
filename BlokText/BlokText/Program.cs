using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

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
            int emptyLines = 0;
            int currLenght = 0;


            while (!inputFile.reader.EndOfStream)
            {
                char character = (char)inputFile.reader.Read();
                while (character != '\uffff' && (character == '\n' || character == '\t' || character == ' '))
                {
                    character = (char)inputFile.reader.Read();
                }

                while (character != '\uffff')
                {
                    while (character != '\uffff' && character != '\n' && character != '\t' && character != ' ')
                    {
                        buffer.Append(character);
                        character = (char)inputFile.reader.Read();
                    }

                    if (buffer.Length >= maxLength)
                    {
                        WriteOutput(line, maxLength, name, false, false);
                        line.Clear();
                        line.Add(buffer.ToString());
                        buffer.Clear();
                        WriteOutput(line, maxLength, name, false, false);
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
                        WriteOutput(line, maxLength, name, false, false);
                        line.Clear();
                        line.Add(buffer.ToString());
                        currLenght = buffer.Length + 1;
                        buffer.Clear();
                    }

                    while (character != '\uffff' && (character == '\n' || character == '\t' || character == ' '))
                    {
                        if (character == '\n' && endOfLine)
                        {
                            ++emptyLines;
                        }

                        if (character == '\n' && !endOfLine)
                        {
                            endOfLine = true;
                        }
                        character = (char)inputFile.reader.Read();
                    }

                    endOfLine = false;

                    if (emptyLines > 0)
                    {
                        if (buffer.Length + currLenght <= maxLength)
                        {
                            if (buffer.Length != 0)
                            {
                                line.Add(buffer.ToString());
                            }
                            WriteOutput(line, maxLength, name, true, false);
                            currLenght = 0;
                        }
                        else if (buffer.Length >= maxLength)
                        {
                            WriteOutput(line, maxLength, name, false, false);
                            line.Clear();
                            if (buffer.Length != 0)
                            {
                                line.Add(buffer.ToString());
                            }
                            WriteOutput(line, maxLength, name, true, false);
                            currLenght = 0;
                        }
                        else
                        {
                            WriteOutput(line, maxLength, name, false, false);
                        }
                        buffer.Clear();
                        line.Clear();
                        if (character != '\uffff')
                        {
                            line.Add("");
                            WriteOutput(line, maxLength, name, false, false);
                            line.Clear();
                            emptyLines = 0;
                        }
                    }
                }
            }

            if (line.Count != 0)
            {
                WriteOutput(line, maxLength, name, true, true);
            }

        }

        public void WriteOutput(List<string> line, int maxLength, string name, bool paraEnd, bool endOfFile)
        {
            StringBuilder builder = new StringBuilder();

            int gapCount = line.Count - 1;
            int charCount = 0;
            foreach (var word in line)
            {
                charCount += word.Length;
            }

            int spacesLeft = maxLength - (charCount + gapCount);

            if (line.Count != 0 && line[0] != "" && line[0] != "\n")
            {
                if (paraEnd)
                {
                    foreach (var word in line)
                    {
                        if (line.IndexOf(word) == line.Count - 1)
                        {
                            builder.Append(word).Append(Environment.NewLine);
                        }
                        else
                        {
                            builder.Append(word).Append(" ");
                        }
                    }
                }
                else if (!paraEnd && gapCount != 0)
                {
                    if (spacesLeft % gapCount == 0)
                    {
                        foreach (var word in line)
                        {
                            int spacesToAdd = spacesLeft / gapCount;
                            if (line.IndexOf(word) == line.Count - 1)
                            {
                                builder.Append(word).Append(Environment.NewLine);
                            }
                            else
                            {
                                builder.Append(word).Append(' ', spacesToAdd + 1);
                            }
                        }
                    }
                    else
                    {
                        List<char> differentSpaces = new List<char>();
                        foreach (var word in line)
                        {
                            foreach (var character in word)
                            {
                                differentSpaces.Add(character);
                            }

                            if (line.IndexOf(word) != line.Count - 1)
                            {
                                differentSpaces.Add(' ');
                            }
                        }

                        while (differentSpaces.Count < maxLength)
                        {
                            for (int i = 0; i < differentSpaces.Count; i++)
                            {
                                if (differentSpaces[i] == ' ' && differentSpaces.Count < maxLength)
                                {
                                    differentSpaces.Insert(i, ' ');
                                    ++i;
                                }
                            }
                        }

                        foreach (var character in differentSpaces)
                        {
                            builder.Append(character);
                        }

                        builder.Append(Environment.NewLine);
                    }

                }
                else if (gapCount == 0)
                {
                    builder.Append(line[0]).Append(Environment.NewLine);
                }
            }
            else if(line.Count != 0)
            {
                builder.Append(Environment.NewLine);
            }

            //Console.Write(builder);
            using (StreamWriter writer = new StreamWriter(name, append:true))
            {
                if (endOfFile)
                {
                    writer.Write(builder.ToString().TrimEnd() + "\n");
                }
                else
                { 
                    writer.Write(builder);
                }
                
            }
        }
    }
}
