using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
                    Input input = new Input(inputFileName);
                    Output output = new Output();
                    TableMaker tableMaker = new TableMaker();
                    tableMaker.ProcessRows(input);
                    tableMaker.ResolveAllFormulas();
                    using (var outputFile = File.Open(outputFileName, FileMode.OpenOrCreate))
                    {
                        using (StreamWriter writer = new StreamWriter(outputFile))
                        {
                            output.WriteTable(writer);
                        }
                    }
                    
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
        public void WriteTable(StreamWriter writer)
        {
            foreach (var line in ExcelTable.Table)
            {
                foreach (var item in line)
                {
                    if (item.Error == "")
                    {
                        if (item.Value == 0)
                        {
                            writer.Write("[]");
                        }
                        else
                        {
                            writer.Write(item.Value);
                        }
                    }
                    else
                    {
                        writer.Write(item.Error);
                    }
                    writer.Write(" ");
                }
                writer.Write('\n');
            }
        }
    }

    class Cell 
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public int Value { get; set; } = 0;
        public string Error { get; set; } = "";
        public string Operation { get; set; } = "";
        public int[] LeftOperandIndex { get; set; }
        public int[] RightOperandIndex { get; set; }

        public Cell(int row, int column,int value)
        {
            Column = column;
            Row = row;
            Value = value;
        }
        public Cell(int row, int column, string error)
        {
            Column = column;
            Row = row;
            Error = error;
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
    static class ExcelTable
    {
        public static List<List<Cell>> Table = new List<List<Cell>>();

        public static Cell GetCell(int row, int column)
        {
            if (row - 1 > Table.Count || column-1 > Table[row-1].Count)
            {
                return null;
            }
            return Table[row-1][column-1];
        }
    }
    class TableMaker
    {
        /// <summary>
        /// Function which processes data cells from the input file and adds them to a local representation of the Excel table.
        /// </summary>
        /// <param name="input"></param>
        public void ProcessRows(Input input)
        {
            int rowIndex = 1;
            Regex regexFormula = new Regex(@"^=([A-Z]+)(\d+)([+\-*\/])([A-Z]+)(\d+)");
            foreach (string line in input.Lines)
            {
                string[] cells = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var row = new List<Cell>();
                int columnIndex = 1;
                foreach (string cell in cells)
                {
                    
                    if (regexFormula.IsMatch(cell))
                    {
                        string[] groups = regexFormula.Split(cell);
                        int[] leftOperand = new int[2] { int.Parse(groups[2]), ColumnConverter.ConvertCellIndex(groups[1]) };
                        int[] rightOperand = new int[2]{ int.Parse(groups[5]), ColumnConverter.ConvertCellIndex(groups[4]) };
                        var cellItem = new Cell(rowIndex, columnIndex, groups[3], leftOperand, rightOperand);
                        row.Add(cellItem);
                    }
                    else if(cell.Contains('+') || cell.Contains('-') || cell.Contains('*') || cell.Contains('/'))
                    {
                        var cellItem = new Cell(rowIndex, columnIndex, "#FORMULA");
                        row.Add(cellItem);
                    }
                    else if (cell == "[]")
                    { 
                        var cellItem = new Cell(rowIndex, columnIndex);
                        row.Add(cellItem);
                    }
                    else if(int.TryParse(cell, out int cellValue))
                    {
                        var cellItem = new Cell(rowIndex, columnIndex, cellValue);
                        row.Add(cellItem);
                    }

                    else
                    {
                        if (cell[0] == '=')
                        {
                            var cellItem = new Cell(rowIndex, columnIndex, "#MISSOP");
                            row.Add(cellItem);
                        }
                        else
                        {
                            var cellItem = new Cell(rowIndex, columnIndex, "#INVVAL");
                            row.Add(cellItem);
                        }
                    }

                    columnIndex++;
                }
                ExcelTable.Table.Add(row);
                rowIndex++;
            }
        }
        public void ResolveAllFormulas()
        {
            foreach (var line in ExcelTable.Table)
            {
                foreach (var cell in line)
                {
                    ResolveFormula(cell);
                }
            }
        }
        public void ResolveFormula(Cell cell)
        {
            Stack<Cell> stack = new Stack<Cell>();
            int leftOperandRow = 0;
            int leftOperandCol = 0;
            int rightOperandRow = 0;
            int rightOperandCol = 0;
            int leftOperand = int.MaxValue;
            int rightOperand = int.MaxValue;
            if (cell.Operation != "" && cell.Error == "")
            {
                Cell originalCell = cell;
                stack.Push(cell);
                while (stack.Count > 0)
                {
                    Cell currFormula = stack.Pop();
                    
                    leftOperandRow = currFormula.LeftOperandIndex[0];
                    leftOperandCol = currFormula.LeftOperandIndex[1];
                    rightOperandRow = currFormula.RightOperandIndex[0];
                    rightOperandCol = currFormula.RightOperandIndex[1];
                    
                    //left operand
                    if (ExcelTable.GetCell(leftOperandRow, leftOperandCol).Operation != "" && ExcelTable.GetCell(leftOperandRow, leftOperandCol).Error == "")
                    {
                        //cycle detection
                        if (leftOperandRow == originalCell.Row &&  leftOperandCol == originalCell.Column)
                        {
                            ExcelTable.GetCell(leftOperandRow, leftOperandCol).Error = "#CYCLE";
                            currFormula.Error = "#CYCLE";
                            break;
                        }
                        if (stack.Count == 0 || stack.Peek() != currFormula)
                        {
                            stack.Push(currFormula);
                        }
                       
                        stack.Push(ExcelTable.GetCell(leftOperandRow, leftOperandCol));
                    }
                    else if( ExcelTable.GetCell(leftOperandRow, leftOperandCol).Error != "")
                    {
                        currFormula.Error = "#ERROR";
                        stack.Clear();
                        break;
                    }
                    else
                    {
                        leftOperand = ExcelTable.GetCell(leftOperandRow, leftOperandCol).Value;
                    }
                    //right operand
                    if (ExcelTable.GetCell(rightOperandRow, rightOperandCol).Operation != "" && ExcelTable.GetCell(rightOperandRow, rightOperandCol).Error == "")
                    {
                        //cycle detection
                        if (rightOperandRow== originalCell.Row &&  rightOperandCol == originalCell.Column)
                        {
                            ExcelTable.GetCell(rightOperandRow, rightOperandCol).Error = "#CYCLE";
                            currFormula.Error = "#CYCLE";
                            break;
                        }

                        if (stack.Count == 0 || stack.Peek() != currFormula)
                        {
                            stack.Push(currFormula);
                        }
                        stack.Push(ExcelTable.GetCell(rightOperandRow, rightOperandCol));
                    }
                    else if (ExcelTable.GetCell(rightOperandRow, rightOperandCol).Error != "")
                    {
                        currFormula.Error = "#ERROR";
                        stack.Clear();
                        break;
                    }
                    else
                    {
                        rightOperand = ExcelTable.GetCell(rightOperandRow,rightOperandCol).Value;
                    }

                    //if both values are retrieved
                    if(leftOperand != int.MaxValue && rightOperand != int.MaxValue)
                    {
                        switch (currFormula.Operation)
                        {
                            case "+":
                                currFormula.Value = leftOperand + rightOperand;
                                break;
                            case "-":
                                currFormula.Value = leftOperand - rightOperand;
                                break;
                            case "*":
                                currFormula.Value = leftOperand * rightOperand;
                                break;
                            case "/":
                                if (rightOperand != 0)
                                {
                                    currFormula.Value = leftOperand / rightOperand;
                                }
                                else
                                {
                                    currFormula.Error = "#DIV0";
                                }
                                break;
                        }
                        currFormula.Operation = "";
                    }
                }
            }
        }


    }
    static class ColumnConverter
    {
        public static int ConvertCellIndex(string columnString)
        {
            int columnNumber = 0;
            int stringLength = columnString.Length-1;
            int exponent = 1;

            for (int i = stringLength; i >= 0; i--)
            {
                columnNumber = columnNumber + (columnString[i] - 64) * exponent;
                exponent = exponent * 26;
            }
            return columnNumber;
        }



    }
}
