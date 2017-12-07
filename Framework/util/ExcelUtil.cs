using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using WebdriverFramework.Framework.WebDriver;

namespace WebdriverFramework.Framework.Util
{
    /// <summary>
    /// microsoft office excel must be installed on your PC before using this class!!
    /// It is desirable install a full office, or at least install the components VBA
    /// </summary>
    public class ExcelUtils
    {
        private Workbooks _excelappworkbooks;
        private Workbook _excelappworkbook;
        private Application _excelapp;
        private readonly XlFileFormat _fileFormat;
        private readonly string _filePath;
        private static readonly Logger Log = Logger.Instance;

        /// <summary>
        /// constructor: set filepath and set set fileFormat
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileFormat">defualt save format</param>
        public ExcelUtils(string filePath, XlFileFormat fileFormat)
        {
            _filePath = filePath;
            _fileFormat = fileFormat;
        }

        /// <summary>
        /// constructor: set filepath
        /// fileFormat = xlWorkbookDefault
        /// </summary>
        /// <param name="filePath"></param>
        public ExcelUtils(string filePath)
        {
            _filePath = filePath;
            _fileFormat = XlFileFormat.xlWorkbookDefault;
        }

        /// <summary>
        /// create new excel file
        /// </summary>
        public void CreateFile()
        {
            _excelapp = new Application { SheetsInNewWorkbook = 3 };
            _excelapp.Workbooks.Add(Type.Missing);
            _excelappworkbooks = _excelapp.Workbooks;
            _excelappworkbook = _excelappworkbooks[1];
            _excelappworkbook.Saved = true;
            _excelapp.DefaultSaveFormat = _fileFormat;
            SaveAsAndQuit();
        }

        /// <summary>
        /// openfile (visible = true)
        /// </summary>
        public void OpenFile()
        {
            if (!File.Exists(_filePath))
            {
                Log.Error(string.Format("File not found {0}", _filePath));
            }

            _excelapp = new Application
            {
                Visible = false,
                UserControl = false,
                //AutomationSecurity = MsoAutomationSecurity.msoAutomationSecurityForceDisable,
                DisplayAlerts = false
            };
            _excelapp.Workbooks.Open(_filePath, Editable: true, IgnoreReadOnlyRecommended: true, Notify: true, CorruptLoad: true);
        }

        /// <summary>
        /// write text to cell with open file and quit
        /// </summary>
        /// <param name="rowIndex">row index of cell</param>
        /// <param name="colIndex">column index of cell</param>
        /// <param name="text"></param>
        public void WriteToFileAndQuit(int rowIndex, int colIndex, string text)
        {
            OpenFile();
            WriteToFile(rowIndex, colIndex, text);
            SaveAsAndQuit();
        }

        /// <summary>
        /// method of write text to cel without open file 
        /// </summary>
        /// <param name="rowIndex">row index of cell</param>
        /// <param name="colIndex">column index of cell</param>
        /// <param name="text"></param>
        public void WriteToFile(int rowIndex, int colIndex, string text)
        {
            _excelapp.Cells[rowIndex, colIndex] = text;
        }

        /// <summary>
        /// save as file and quit
        /// </summary>
        public void SaveAsAndQuit()
        {
            _excelapp.Workbooks.Item[1].SaveAs(_filePath, _fileFormat, AccessMode: XlSaveAsAccessMode.xlNoChange, ConflictResolution: true);
            _excelapp.Workbooks.Close();
            _excelapp.Quit();
        }

        /// <summary>
        /// write column header to file(fill first row of file)
        /// </summary>
        public void WriteColumnHeaders(params string[] list)
        {
            OpenFile();
            for (var i = 0; i < list.Length; i++)
            {
                WriteToFile(1, i + 1, list[i]);
            }

            SaveAsAndQuit();
        }

        /// <summary>
        /// fill row with rownumber by dataList[i] 
        /// </summary>
        /// <param name="rowNumber"> row number</param>
        /// <param name="dataList"> array of data for filling row</param>
        public void FillRow(int rowNumber, params string[] dataList)
        {
            OpenFile();
            for (var i = 0; i < dataList.Length; i++)
            {
                WriteToFile(rowNumber, i + 1, dataList[i]);
            }

            SaveAsAndQuit();
        }

        /// <summary>
        /// read excel file
        /// </summary>
        /// <param name="sheetName">sheet name</param>
        /// <returns>list of rows</returns>
        public List<List<object>> GetRowList(string sheetName)
        {
            OpenFile();
            var xlWorkSheet = (Worksheet)_excelapp.Worksheets.Item[sheetName];
            var range = xlWorkSheet.UsedRange;
            var rowList = new List<List<object>>();
            for (var rowCnt = 1; rowCnt <= range.Rows.Count; rowCnt++)
            {
                var rowL = new List<object>();
                for (var columnCnt = 1; columnCnt <= range.Columns.Count; columnCnt++)
                {
                    var range1 = range.Cells[rowCnt, columnCnt] as Range;
                    if (range1 != null)
                    {
                        rowL.Add(range1.Value2);
                    }
                }

                rowList.Add(rowL);
            }

            SaveAsAndQuit();
            return rowList;
        }

        /// <summary>
        /// get columns names (first row)
        /// </summary>
        /// <param name="sheetName"></param>
        /// <returns>dictionary: key = column name , column index</returns>
        public Dictionary<string, int> GetColumnNames(string sheetName)
        {
            OpenFile();
            var xlWorkSheet = (Worksheet)_excelapp.Worksheets.Item[sheetName];
            var range = xlWorkSheet.UsedRange;
            var columnNames = new Dictionary<string, int>();
            for (var columnCnt = 1; columnCnt <= range.Columns.Count; columnCnt++)
            {
                columnNames.Add(range.Cells[1, columnCnt].Value2.ToString(), columnCnt);
            }

            SaveAsAndQuit();
            return columnNames;
        }

        /// <summary>
        /// delete cell
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <returns>list of names</returns>
        public void DeleteCell(string sheetName, int rowIndex, int colIndex)
        {
            OpenFile();
            var xlWorkSheet = (Worksheet)_excelapp.Worksheets.Item[sheetName];
            var range = xlWorkSheet.UsedRange;
            ((Range)range.Cells[rowIndex, colIndex]).Delete();
            SaveAsAndQuit();
        }

        /// <summary>
        /// delete row with shift up
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="rowIndex"></param>
        /// <returns>list of names</returns>
        public void DeleteRowWithShiftUp(string sheetName, int rowIndex)
        {
            OpenFile();
            var xlWorkSheet = (Worksheet)_excelapp.Worksheets.Item[sheetName];
            var range = xlWorkSheet.UsedRange;
            var cell1 = range.Cells[rowIndex, 1];
            var cell2 = range.Cells[rowIndex, range.Columns.Count];
            range.Range[cell1, cell2].Delete(XlDeleteShiftDirection.xlShiftUp);
            SaveAsAndQuit();
        }

        /// <summary>
        /// delete rows with rowIndexesToDelete
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="rowIndexesToDelete"></param>
        /// <returns>list of names</returns>
        public void DeleteRowWithShiftUp(string sheetName, List<int> rowIndexesToDelete)
        {
            OpenFile();
            var xlWorkSheet = (Worksheet)_excelapp.Worksheets.Item[sheetName];
            foreach (var rowIndex in Enumerable.Reverse(rowIndexesToDelete))
            {
                var range = xlWorkSheet.Rows[rowIndex] as Range;
                if (range != null)
                {
                    range.Delete(XlDeleteShiftDirection.xlShiftUp);
                }
            }

            SaveAsAndQuit();
        }

        /// <summary>
        /// delete columns with names {columnNames}
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="columnNamesToDelete"></param>
        /// <returns>list of names</returns>
        public void DeleteColumnWithShiftUp(string sheetName, List<string> columnNamesToDelete)
        {
            OpenFile();
            var allColumnNames = GetColumnNames(sheetName);
            var xlWorkSheet = (Worksheet)_excelapp.Worksheets.Item[sheetName];
            foreach (var colName in allColumnNames.Reverse())
            {
                if (!columnNamesToDelete.Contains(colName.Key))
                {
                    continue;
                }

                var range = xlWorkSheet.Columns[colName.Value] as Range;
                if (range != null)
                {
                    range.Delete(XlDeleteShiftDirection.xlShiftUp);
                }
            }

            SaveAsAndQuit();
        }

        /// <summary>
        /// delete columns with indexes {columnNames}
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="columnIndexToDelete"></param>
        /// <returns>list of names</returns>
        public void DeleteColumnWithShiftUp(string sheetName, List<int> columnIndexToDelete)
        {
            OpenFile();
            var xlWorkSheet = (Worksheet)_excelapp.Worksheets.Item[sheetName];
            foreach (var colIndex in Enumerable.Reverse(columnIndexToDelete))
            {
                var range = xlWorkSheet.Columns[colIndex] as Range;
                if (range != null)
                {
                    range.Delete(XlDeleteShiftDirection.xlShiftToLeft);
                }
            }

            SaveAsAndQuit();
        }

        /// <summary>
        /// get values of column with row index
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="columnName"></param>
        /// <returns>list of names</returns>
        public Dictionary<int, object> GetColumnContent(string sheetName, string columnName)
        {
            var columnNames = GetColumnNames(sheetName);
            var colIndex = columnNames[columnName];
            var columnContent = new Dictionary<int, object>();

            OpenFile();
            var xlWorkSheet = (Worksheet)_excelapp.Worksheets.Item[sheetName];
            var range = xlWorkSheet.UsedRange;
            //var rowCnt = 2 , because first row is header
            for (var rowCnt = 2; rowCnt <= range.Rows.Count; rowCnt++)
            {
                var colValue = range.Cells[rowCnt, colIndex].Value2 ?? string.Empty;
                columnContent.Add(rowCnt, colValue);
            }

            SaveAsAndQuit();
            return columnContent;
        }
    }
}
