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
                    Regex regexFormula = new Regex(@"^=([A-Z]+)(\d+)([+\-*\/])([A-Z]+)(\d+)");
                    tableMaker.ProcessRows(input, regexFormula);
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
        StreamReader reader;

        public Input(string name)
        {
            reader = new StreamReader(name);
        }

        public string GetLine()
        {
            return reader.ReadLine();
        }
    }

    //TODO: rewrite writing output 
    class Output
    {
        public void WriteTable(StreamWriter writer)
        {
            foreach (var line in ExcelTable.Table)
            {
                foreach (var item in line)
                {
                    if(item is ErrorCell)
                    {
                        writer.Write(((ErrorCell)item).Error);
                    }
                    else if (item is ValueCell)
                    {
                        if(((ValueCell)item).Value != 0)
                        {
                            writer.Write(((ValueCell)item).Value);
                        }
                        else
                        {
                            writer.Write("[]");
                        }
                    }
                    writer.Write(" ");
                }
                writer.Write('\n');
            }
        }
    }
    public abstract class Cell
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public Cell()
        {

        }

    }
    class ValueCell : Cell
    {
        public int Value { get; set; } = 0;

        public ValueCell(int row, int column, int value)
        {
            Column = column;
            Row = row;
            Value = value;
        }
    }
    class FormulaCell : Cell
    {
        public string Operation { get; set; }
        public int[] LeftOperandIndex { get; set; }
        public int[] RightOperandIndex { get; set; }

        public FormulaCell(int row, int column, string operation, int[] leftOperandIndex, int[] rightOperandIndex)
        {
            Row = row;
            Column = column;
            Operation = operation;
            LeftOperandIndex = leftOperandIndex;
            RightOperandIndex = rightOperandIndex;
        }

    }
    class ErrorCell : Cell
    {
        public string Error { get; set; }
        public ErrorCell(int row, int column, string error)
        {
            Column = column;
            Row = row;
            Error = error;
        }
    }

    /// <summary>
    /// Table representation containing all cell values. Cell can be retrieved by providing row and column indices to the GetCell() method. 
    /// </summary>
    static class ExcelTable
    {
        public static List<List<Cell>> ProvisionalTableBasic = new List<List<Cell>>();
        public static List<Cell>[] Table;
        public static void ProvisionalToUsable() 
        {
            Table = ProvisionalTableBasic.ToArray();
            ProvisionalTableBasic.Clear();
        }
        public static Cell GetCell(int row, int column)
        {
            if (row - 1 > Table.Length || column - 1 > Table[row - 1].Count)
            {
                return null;
            }
            return Table[row - 1][column - 1];
        }
        public static void SetCell(int row, int column, Cell cell)
        {
            Table[row - 1][column - 1] = cell;
        }
    }

    class TableMaker
    {
        /// <summary>
        /// Function which processes data cells from the input file and adds them to a local representation of the Excel table.
        /// </summary>
        /// <param name="input"></param>
        public void ProcessRows(Input input, Regex regexFormula)
        {
            const string missOp = "#MISSOP";
            const string missFormula = "#FORMULA";
            const string missVal = "#INVVAL";
            int rowIndex = 1;
            string line = input.GetLine();
            while (line != null)
            {
                string[] cells = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var row = new List<Cell>();
                int columnIndex = 1;
                foreach (string cell in cells)
                {
                    if (cell == "[]")
                    {
                        var cellItem = new ValueCell(rowIndex, columnIndex, 0);
                        row.Add(cellItem);
                    }
                    else if (cell.Contains('+') || cell.Contains('-') || cell.Contains('*') || cell.Contains('/'))
                    {
                        if (regexFormula.IsMatch(cell))
                        {
                            string[] groups = regexFormula.Split(cell);
                            int[] leftOperand = new int[2] { int.Parse(groups[2]), ColumnConverter.ConvertCellIndex(groups[1]) };
                            int[] rightOperand = new int[2] { int.Parse(groups[5]), ColumnConverter.ConvertCellIndex(groups[4]) };
                            var cellItem = new FormulaCell(rowIndex, columnIndex, groups[3], leftOperand, rightOperand);
                            row.Add(cellItem);
                        }
                        else
                        {
                            var cellItem = new ErrorCell(rowIndex, columnIndex, missFormula);
                            row.Add(cellItem);
                        }
                    }
                    else if (int.TryParse(cell, out int cellValue))
                    {
                        var cellItem = new ValueCell(rowIndex, columnIndex, cellValue);
                        row.Add(cellItem);
                    }
                    else
                    {
                        if (cell[0] == '=')
                        {
                            var cellItem = new ErrorCell(rowIndex, columnIndex, missOp);
                            row.Add(cellItem);
                        }
                        else
                        {
                            var cellItem = new ErrorCell(rowIndex, columnIndex, missVal);
                            row.Add(cellItem);
                        }
                    }

                    columnIndex++;
                }

                ExcelTable.ProvisionalTableBasic.Add(row);
                rowIndex++;
                line = input.GetLine();
            }
            ExcelTable.ProvisionalToUsable();
        }

        /// <summary>
        /// Resolves all formulas contained within the input table.
        /// </summary>
        public void ResolveAllFormulas()
        {
            int lineCount = ExcelTable.Table.Length;
            for (int i = 0; i < lineCount; i++)
            {
                for (int j = 0; j < ExcelTable.Table[i].Count; j++)
                {
                    if (ExcelTable.Table[i][j] is FormulaCell)
                    {
                        ResolveFormula((FormulaCell)ExcelTable.Table[i][j]);
                    }
                }
            }
        }

        /// <summary>
        /// Resolves a certain formula using a stack. Detects errors/problems with the computation of a formula.
        /// </summary>
        /// <param name="cell"></param>
        public void ResolveFormula(FormulaCell cell)
        {
            //constants for all the different errors that we try to catch

            const string missCycle = "#CYCLE";
            const string missError = "#ERROR";
            const string missDiv = "#DIV0";
            FormulaCell currCell;
            FormulaCell originalCell = cell;
            //stack for operations embedded in an operator
            Stack<FormulaCell> stack = new Stack<FormulaCell>();
            int leftOperandRow = 0;
            int leftOperandCol = 0;
            int rightOperandRow = 0;
            int rightOperandCol = 0;
            int? leftOperand = null;
            int? rightOperand = null;

            stack.Push(cell);
            while (stack.Count > 0)
            {
                currCell = stack.Pop();
                
                leftOperandRow = currCell.LeftOperandIndex[0];
                leftOperandCol = currCell.LeftOperandIndex[1];
                rightOperandRow = currCell.RightOperandIndex[0];
                rightOperandCol = currCell.RightOperandIndex[1];


                //left operand
                Cell leftOperandCell = ExcelTable.GetCell(leftOperandRow, leftOperandCol);
                if (leftOperandCell != null)
                {
                    if (leftOperandCell is FormulaCell)
                    {

                        //cycle detection
                        if (leftOperandCell == originalCell)
                        {
                            ExcelTable.SetCell(currCell.Row, currCell.Column, new ErrorCell(currCell.Row, currCell.Column, missCycle));
                            foreach (var cycleCell in stack)
                            {
                                ExcelTable.SetCell(cycleCell.Row, cycleCell.Column, new ErrorCell(cycleCell.Row, cycleCell.Column, missCycle));
                            }
                            stack.Clear();
                            break;
                        }
                        if (leftOperand == null && (stack.Count == 0 || stack.Peek() != currCell))
                        {
                            stack.Push(currCell);
                        }
                        stack.Push((FormulaCell)leftOperandCell);
                        
                    }
                    else if (leftOperandCell is ErrorCell)
                    {
                        ExcelTable.SetCell(currCell.Row, currCell.Column, new ErrorCell(currCell.Row, currCell.Column, missError));
                        stack.Clear();
                        break;
                    }
                    else if (leftOperandCell is ValueCell)
                    {
                        leftOperand = ((ValueCell)leftOperandCell).Value;
                    }
                }
                else
                {
                    leftOperand = 0;
                }
                //right operand
                Cell rightOperandCell = ExcelTable.GetCell(rightOperandRow, rightOperandCol);
                if (rightOperandCell != null)
                {
                    if (rightOperandCell is FormulaCell)
                    {
                        //cycle detection
                        
                            if (rightOperandCell == originalCell)
                            {
                                ExcelTable.SetCell(currCell.Row, currCell.Column, new ErrorCell(currCell.Row, currCell.Column, missCycle));
                                foreach (var cycleCell in stack)
                                {
                                    ExcelTable.SetCell(cycleCell.Row, cycleCell.Column, new ErrorCell(cycleCell.Row, cycleCell.Column, missCycle));
                                }
                                stack.Clear();
                                break;
                            }
                            if (rightOperand == int.MaxValue && (stack.Count == 0 || stack.Peek() != currCell))
                            {
                                stack.Push(currCell);
                            }

                            stack.Push((FormulaCell)rightOperandCell);
                        
                    }
                    else if (rightOperandCell is ErrorCell)
                    {
                        ExcelTable.SetCell(currCell.Row, currCell.Column, new ErrorCell(currCell.Row, currCell.Column, missError));
                        stack.Clear();
                        break;
                    }
                    else if (rightOperandCell is ValueCell)
                    {
                        rightOperand = ((ValueCell)rightOperandCell).Value;
                    }
                }
                else
                {
                    rightOperand = 0;
                }

                //if both values are retrieved
                if (currCell is FormulaCell && leftOperand != null && rightOperand != null)
                {
                    int left = leftOperand.Value;
                    int right = rightOperand.Value;
                    switch (currCell.Operation)
                    {
                        case "+":
                            ExcelTable.SetCell(currCell.Row, currCell.Column, new ValueCell(currCell.Row, currCell.Column, left + right));
                            break;
                        case "-":
                            ExcelTable.SetCell(currCell.Row, currCell.Column, new ValueCell(currCell.Row, currCell.Column, left - right));
                            break;
                        case "*":
                            ExcelTable.SetCell(currCell.Row, currCell.Column, new ValueCell(currCell.Row, currCell.Column, left * right));
                            break;
                        case "/":
                            if (rightOperand != 0)
                            {
                                ExcelTable.SetCell(currCell.Row, currCell.Column, new ValueCell(currCell.Row, currCell.Column, left / right));
                            }
                            else
                            {
                                ExcelTable.SetCell(currCell.Row, currCell.Column, new ErrorCell(currCell.Row, currCell.Column, missDiv));
                                stack.Clear();
                            }
                            break;
                    }
                    currCell.Operation = "";
                }

            }
        }

    }
    static class ColumnConverter
    {
        /// <summary>
        /// Convert cell index from letter to a number representation.
        /// </summary>
        /// <param name="columnString"></param>
        /// <returns></returns>
        public static int ConvertCellIndex(string columnString)
        {
            int columnNumber = 0;
            int stringLength = columnString.Length - 1;
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

