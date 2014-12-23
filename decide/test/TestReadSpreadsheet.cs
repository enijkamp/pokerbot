using System;
using System.IO;
using System.Text;
using ExcelLibrary.SpreadSheet;

namespace PokerBot
{	
	class TestReadSpreadsheet
	{
		public static void Main(string[] args)
		{
            string tempFilePath = "/home/erik/Test.xls";
            {
                Workbook workbook = new Workbook();
                Worksheet worksheet = new Worksheet("Test1");
                worksheet.Cells[0, 1] = new Cell(100);
                worksheet.Cells[2, 0] = new Cell("Test String");
                workbook.Worksheets.Add(worksheet);
                workbook.Save(tempFilePath);
            }

            {
                Workbook workbook = Workbook.Load(tempFilePath);
                Console.WriteLine(workbook.Worksheets.Count);

                Worksheet worksheet = workbook.Worksheets[0];
                Console.WriteLine(worksheet.Name);
                Console.WriteLine(worksheet.Cells[0, 1].Value);
                Console.WriteLine(worksheet.Cells[2, 0].Value);
            }
		}
	}
}