using System;
using System.Collections.Generic;
using System.IO;

namespace Excel
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2 || args[0] == "" || args[1] == "")
            {
                Console.WriteLine("Argument Error");
            }
            else
            {
                string inputFileName = args[0];
                string outputFileName = args[1];

                try
                {
                    //TODO: implement main
                    
                    Input input = new Input(inputFileName);






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




    class Input
    {
        public string[] Lines { get; }

        public Input(string name)
        {
            Lines = File.ReadAllLines(name);
        }
    }

    //TODO: implement writing output 
    class Output
    {






    }

    class Cell 
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public int Value { get; set; } = 0;
        public string Operation { get; set; }
        public int[] LeftOperandIndex { get; set; }
        public int[] RightOperandIndex { get; set; }

        public Cell(int row, int column,int value)
        {
            Column = column;
            Row = row;
            Value = value;
        }
        public Cell(int row,int column )
        {
            Column = column;
            Row = row;
        }
        public Cell(int row, int column, string operation, int[] leftOperandIndex, int[] rightOperandIndex)
        {
            Row = row;
            Column = column;
            Operation = operation;
            LeftOperandIndex = leftOperandIndex;    
            RightOperandIndex = rightOperandIndex;
        }

    }
    class ExcelTable
    {
        public List<List<Cell>> Table = new List<List<Cell>>();
        public void InitializeTable()
        {
            Table[0] = null; 
        }

        public Cell GetCell(int row, int column)
        {
            if (row > Table.Count || column > Table[row].Count)
            {
                return null;
            }
            return Table[row][column];
        }
    }
    class TableMaker
    {
        /// <summary>
        /// Function which processes data cells from the input file and adds them to a local representation of the Excel table.
        /// </summary>
        /// <param name="input"></param>
        public void ProcessRows(Input input, ExcelTable excelTable)
        {
            int rowIndex = 1;
            foreach (string line in input.Lines)
            {
                string[] cells = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var row = new List<Cell>();
                row[0] = null;
                int columnIndex = 1;
                foreach (string cell in cells)
                {
                    if (cell[0] == '=')
                    {







                    }
                    else
                    {
                        if (cell == "[]")
                        {
                            var cellItem = new Cell(rowIndex, columnIndex);
                            row.Add(cellItem);
                        }
                        else if(int.TryParse(cell, out int cellValue))
                        {
                            var cellItem = new Cell(rowIndex, columnIndex, cellValue);
                            row.Add(cellItem);
                        }
                    }
                    columnIndex++;
                }
                excelTable.Table.Add(row);
                rowIndex++;
            }
        }




    }
    class ColumnConverter
    {




    }






}
