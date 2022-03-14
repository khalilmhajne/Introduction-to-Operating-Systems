using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApp1
{
	class Simulator
	{
		static void Main(string[] args)
		{
			int nRows = Int32.Parse(args[0]);
			int nCols = Int32.Parse(args[1]);
			int nThreads = Int32.Parse(args[2]);
			int nOperations = Int32.Parse(args[3]);			
			/*
			int nRows = 10;
			int nCols = 10;
			int nThreads = 3;
			int nOperations = 5;
			*/
			//create a rows*cols spreadsheet.
			ShareableSpreadSheet shareableSpreadSheet = new ShareableSpreadSheet(nRows, nCols);

			// Fill the spreadsheet with prepared strings (e.g., testcell11, testcell12, testcell13, etc).
			for (int i = 1; i < nRows; i++)
			{
				for (int j = 1; j < nCols; j++)
				{
					shareableSpreadSheet.setCell(i - 1, j - 1, "testcell" + i.ToString() + j.ToString());
				}
			}

			// Create nThreads threads
			for( int t = 0; t <nThreads; t++)
            {
				Thread thread1 = new Thread(ThreadProc);
				Object[] obj = { shareableSpreadSheet, nOperations };
				thread1.Start(obj);
				Thread.Sleep(50);
			}
			//saves the spread sheet to a file spreadsheet.dat and exit
			shareableSpreadSheet.save("spreadsheet.dat");
		}

		static void ThreadProc(Object stateInfo)
		{
			int UserId = Thread.CurrentThread.ManagedThreadId;
			Object[] obj = (object[]) stateInfo;
			ShareableSpreadSheet shareableSpreadSheet = (ShareableSpreadSheet)obj[0];
			int nOperations = (int)obj[1];
			Random r = new Random();
			for (int i = 0; i < nOperations ; i++)
			{
				int random = r.Next(0, 10);
				if (random.Equals(0))
				{
					int row = 0;
					int col = 0;
					shareableSpreadSheet.getSize(ref row, ref col);
					row = r.Next(0, row);
					col = r.Next(0,col);
					String found = shareableSpreadSheet.getCell(row, col);
					Console.WriteLine("User[" + UserId + "]: " + "String found in cell [" + row + "," + col + "]");
				}
				if (random.Equals(1))
				{
					int row = 0;
					int col = 0;
					shareableSpreadSheet.getSize(ref row, ref col);
					Random R1 = new Random();
					row = R1.Next(0, row);
					col = R1.Next(0, col);
					shareableSpreadSheet.setCell(row, col, "updated");
					Console.WriteLine("User[" + UserId + "]: " + "cell [" + row + "," + col + "] updated to string updated");
				}
                if (random.Equals(2))
                {
					int row = 0;
					int col = 0;
					bool b = shareableSpreadSheet.searchString("updated", ref row, ref col);
                    if (b)
                    {
						Console.WriteLine("User[" + UserId + "]: "+ "String found in cell [" + row + "," + col + "]");
					}
                    else
                    {
						Console.WriteLine("User[" + UserId + "]: " + "String not found ");
					}
				}
                if (random.Equals(3))
                {
					int row = 0;
					int col = 0;
					shareableSpreadSheet.getSize(ref row, ref col);
					Random R3 = new Random();
					int row1 = R3.Next(0, row);
					int row2 = R3.Next(0, row);
					bool b = shareableSpreadSheet.exchangeRows(row1, row2);
                    if (b)
                    {
						Console.WriteLine("User[" + UserId + "]: " + "rows [" + row1 + "]" + " and [" + row2 + "] exchanged successfully.");
					}
                    else
                    {
						Console.WriteLine("User[" + UserId + "]: " + "rows [" + row1 + "]" + " and [" + row2 + "] not exchanged ");
					}
				}
                if (random.Equals(4))
                {
					int row = 0;
					int col = 0;
					shareableSpreadSheet.getSize(ref row, ref col);
					Random R4 = new Random();
					int col1 = R4.Next(0, col);
					int col2 = R4.Next(0, col);
					bool b = shareableSpreadSheet.exchangeRows(col1, col2);
					if (b)
					{
						Console.WriteLine("User[" + UserId + "]: " + "cols [" + col1 + "]" + " and [" + col2 + "] exchanged successfully.");
					}
					else
					{
						Console.WriteLine("User[" + UserId + "]: " + "cols [" + col1 + "]" + " and [" + col2 + "] not exchanged ");
					}
				}
                if (random.Equals(5))
                {
					int row = 0;
					int col = 0;
					shareableSpreadSheet.getSize(ref row, ref col);
					row = r.Next(0, row);
					bool b = shareableSpreadSheet.searchInRow(row, "updated", ref col);
                    if (b)
                    {
						Console.WriteLine("User[" + UserId + "]: " + "String found in cell [" + row + "," + col + "]");
					}
                    else
                    {
						Console.WriteLine("User[" + UserId + "]: " + "String not found ");
					}
				}
                if (random.Equals(6))
                {
					int row = 0;
					int col = 0;
					shareableSpreadSheet.getSize(ref row, ref col);
					col = r.Next(0, col);
					bool b = shareableSpreadSheet.searchInCol(col, "updated", ref row);
					if (b)
					{
						Console.WriteLine("User[" + UserId + "]: " + "String found in cell [" + row + "," + col + "]");
					}
					else
					{
						Console.WriteLine("User[" + UserId + "]: " + "String not found ");
					}
				}
                if (random.Equals(7))
                {
					int row = 0;
					int col = 0;
					shareableSpreadSheet.getSize(ref row, ref col);
					int col1 = r.Next(0, col);
					int col2 = r.Next(col1, col);
					int row1 = r.Next(0, row);
					int row2 = r.Next(row1, row);

					bool b = shareableSpreadSheet.searchInRange(col1, col2, row1, row2, "updated", ref row, ref col);
                    if (b)
                    {
						Console.WriteLine("User[" + UserId + "]: " + "String found in cell [" + row + "," + col + "]");
					}
                    else
                    {
						Console.WriteLine("User[" + UserId + "]: " + "String not found ");
					}
				}

                if (random.Equals(8))
                {
					int row = 0;
					int col = 0;
					shareableSpreadSheet.getSize(ref row, ref col);
					Random R8 = new Random();
					row = R8.Next(1, row-1);
					bool b = shareableSpreadSheet.addRow(row);
                    if (b)
                    {
						Console.WriteLine("User[" + UserId + "]:" + " a new row added after row " + row);
					}
				}
				if (random.Equals(9))
				{
					int row = 0;
					int col = 0;
					shareableSpreadSheet.getSize(ref row, ref col);
					Random R9 = new Random();
					col = R9.Next(0, col);
					bool b = shareableSpreadSheet.addCol(col);
					if (b)
					{
						Console.WriteLine("User[" + UserId + "]:" + " a new col added after col " + col);
					}
                    else
                    {
						Console.WriteLine("User[" + UserId + "]:" + " the number of columns not updated " );
					}
				}
				if (random.Equals(8))
				{
					int row = 0;
					int col = 0;
					shareableSpreadSheet.getSize(ref row, ref col);
					Console.WriteLine("User[" + UserId + "]: the size of Sheet is ["+row+ "," + col +"]");
				}
                if (r.Equals(10)) {
					Random R10 = new Random();
					int nUsers = R10.Next(0, 5);
					bool b = shareableSpreadSheet.setConcurrentSearchLimit(nUsers);
                    if (b)
                    {
						Console.WriteLine("User[" + UserId + "]:" + nUsers + " can perform the search operations concurrently");
					}
                    else
                    {
						Console.WriteLine("User[" + UserId + "]:" + nUsers + " can't perform the search operations concurrently");
					}
				}
				Thread.Sleep(100);
			}
		}
	}

}
