using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

class ShareableSpreadSheet
{
    private  String[,] sheet;
    /***               TO DO LOCKS ! */
    //private static Mutex[] mutRowLock;
    private  Semaphore semaphorePool;
    private  Mutex mutex,counterLock;
    private volatile int rSize, cSize, Readers,maxReaders;
 
    public ShareableSpreadSheet(int nRows, int nCols)
    {
        sheet = new String[nRows, nCols];
        semaphorePool = new Semaphore(0, 10);
        mutex = new Mutex();
        counterLock = new Mutex();
        rSize = nRows;
        cSize = nCols;
        Readers = 0;
        semaphorePool.Release(10);
        maxReaders = 10;
    }
    public String getCell(int row, int col)
    {
        // return the string at [row,col]
        if (row < 1 || col < 1)
            return null;
        if (row > rSize || col > cSize)
            return null;
        row--;
        col--;
        String returnVal;
        readersLock();
        returnVal = sheet[row, col];
        readersFree();
        return returnVal;
    }
    public bool setCell(int row, int col, String str)
    {
        if (row < 1 || col < 1)
            return false;
        if (row > rSize || col > cSize)
            return false;
        row--;
        col--;
        mutex.WaitOne();
        // set the string at [row,col]
        sheet[row, col] = str;
        mutex.ReleaseMutex();
        return true;
    }
    public bool searchString(String str, ref int row, ref int col)
    {
        readersLock();
        semaphorePool.WaitOne();
        for (int i = 0; i < rSize; i++)
        {
            for (int j = 0; j < cSize; j++)
            {
                if (str == sheet[i, j])
                {
                    row = i++;
                    col = j++;
                    readersFree();
                    semaphorePool.Release();
                    return true;
                }               
            }
        }
        readersFree();
        semaphorePool.Release();
        return false;
    }
    public bool exchangeRows(int row1, int row2)
    {
        if (row1 < 1 || row2 < 1)
            return false;
        if (row1 > rSize || row2 > rSize)
            return false;
        String temp = null;
        row1--;
        row2--;
        mutex.WaitOne();
        for (int i = 0; i < cSize; i++)
        {    
            temp = sheet[row1, i];
            sheet[row1, i] = sheet[row2, i];
            sheet[row2, i] = temp;
        }
        mutex.ReleaseMutex();
        return true;
    }
    public bool exchangeCols(int col1, int col2)
    {
        if (col1 < 1 || col2 < 1)
            return false;
        if (col1 > cSize || col2 > cSize)
            return false;
        col1--;
        col2--;
        String temp = null;
        mutex.WaitOne();
        for (int i = 0; i < rSize; i++)
        {
            temp = sheet[i, col1];
            sheet[i, col1] = sheet[i, col2];
            sheet[i, col2] = temp;
        }
        mutex.ReleaseMutex();
        return true;
    }
    public bool searchInRow(int row, String str, ref int col)
    {
        // perform search in specific row
        if (row > rSize || row < 1)
            return false;
        row--;
        readersLock();
        semaphorePool.WaitOne();
        for (int i = 0; i < cSize; i++)
        {
            if (str == sheet[row, i])
            {
                col = i++;
                readersFree();
                semaphorePool.Release();
                return true;
            }
        }
        readersFree();
        semaphorePool.Release();
        return false;
    }
    public bool searchInCol(int col, String str, ref int row)
    {
        if (col < 1 || col > cSize)
            return false;
        col--;
        readersLock();
        semaphorePool.WaitOne();
        for (int i = 0; i < rSize; i++)
        {
            if (str == sheet[i, col])
            {
                row = i++;
                readersFree();
                semaphorePool.Release();
                return true;
            }
        }
        readersFree();
        semaphorePool.Release();
        return false;
    }
    public bool searchInRange(int col1, int col2, int row1, int row2, String str, ref int row, ref int col)
    {
        // perform search within spesific range: [row1:row2,col1:col2] 
        //includes col1,col2,row1,row2

        if (row1 < 1 || row2 < 1 || col1 < 1 || col2 < 1)
            return false;
        if (row1 > row2 || col1 > col2)
            return false;
        if (row1 > rSize || row2 > rSize || col1 > cSize || col2 > cSize)
            return false;
        row1--;
        row2--;
        col1--;
        col2--;
        readersLock();
        semaphorePool.WaitOne();
        for (int i = row1; i < row2; i++)
        {
            for (int j = col1; j < col2; j++)
            {
                if (sheet[i, j] == str)
                {
                    row = i++;
                    col = j++;
                    readersFree();
                    semaphorePool.Release();
                    return true;
                }
            }
        }
        readersFree();
        semaphorePool.Release();
        return false;
    }
    public bool addRow(int row1)
    {
        if (row1 < 1 || row1 > rSize)
            return false;
        row1--;
        String[,] temp = new string[rSize + 1, cSize];
        mutex.WaitOne();
        for (int i = 0; i < row1 + 1; i++)
        {
            for (int j = 0; j < cSize; j++)
            {
                temp[i, j] = sheet[i, j];
            }
        }
        for (int k = row1 + 2; k < rSize + 1; k++)
        {
            for (int j = 0; j < cSize; j++)
            {
                temp[k, j] = sheet[k - 1, j];
            }
        }
        rSize = rSize + 1;
        sheet = temp;
        mutex.ReleaseMutex();
        return true;
    }
    public bool addCol(int col1)
    {
        if (col1 < 1 || col1 > cSize)
            return false;
        col1--;
        String[,] temp = new string[rSize, cSize + 1];
        mutex.WaitOne();
        for (int i = 0; i < rSize; i++)
        {
            for (int j = 0; j < col1 + 1; j++)
            {
                temp[i, j] = sheet[i, j];
            }
        }
        for (int k = 0; k < rSize; k++)
        {
            for (int j = col1 + 2; j < cSize + 1; j++)
            {
                temp[k, j] = sheet[k, j - 1];
            }
        }
        cSize = cSize + 1;
        sheet = temp;
        mutex.ReleaseMutex();
        return true;
    }
    public void getSize(ref int nRows, ref int nCols)
    {
        nRows = rSize;
        nCols = cSize;
    }
    public bool setConcurrentSearchLimit(int nUsers)
    {
        // this function aims to limit the number of users that can perform the search operations concurrently.
        // The default is no limit. When the function is called, the max number of concurrent search operations is set to nUsers. 
        // In this case additional search operations will wait for existing search to finish.
        if (nUsers > maxReaders)
        {
            maxReaders = nUsers;
            mutex.WaitOne();
            semaphorePool = new Semaphore(0, nUsers);
            mutex.ReleaseMutex();
            return true;
            //TODO 
        }
        return false;
    }

    public bool save(String fileName)
    {
        // save the spreadsheet to a file fileName.
        // you can decide the format you save the data. There are several options.
        StreamWriter streamWriter = new StreamWriter(fileName + ".csv");
        StreamWriter file = streamWriter;
        readersLock();
        for (int i = 0; i < rSize; i++)
        {
            for (int j = 0; j < cSize; j++)
            {
                file.Write(sheet[i, j]);
                file.Write(",");
            }
            file.Write("\n"); // go to next line
        }
        readersFree();
        file.Close();
        return true;
    }
    public bool load(String fileName)
    {
        // load the spreadsheet from fileName
        // replace the data and size of the current spreadsheet with the loaded data
        try
        {
            String line;
            int rows, cols;
             StreamReader file = new StreamReader(fileName);
            line = file.ReadLine();
            rows = line.Count(x => (x == ',')) + 1;
            file.DiscardBufferedData();
            file.BaseStream.Seek(0, SeekOrigin.Begin);
            line = file.ReadToEnd();
            cols = line.Count(x => (x == '\n'));
            /*
            Console.WriteLine("this is Cols-" + cols);
            Console.WriteLine("this is Rows-" + rows);
            */
            String[,] outStr = new String[rows, cols];
            file.DiscardBufferedData();
            file.BaseStream.Seek(0, SeekOrigin.Begin);
            int k = 0;
            while ((line = file.ReadLine()) != null)
            {
                String[] temp = line.Split(',');
                for (int i = 0; i < cols; i++)
                    outStr[k, i] = temp[i];
                k++;
            }
            file.Close();
            /*
            for (int i = 0; i < rows; i++)
            {
                for (int ka = 0; ka < cols; ka++)
                    Console.Write(outStr[i, ka] + " ");
                Console.WriteLine();
            }
            */
            this.sheet = outStr;
            this.rSize = rows;
            this.cSize = cols;
            return true;
        }
        catch { return false; }
    }

    private void readersLock()
    {
        counterLock.WaitOne();
        Interlocked.Increment(ref Readers);
        //Readers++;
        if (Readers == 1) 
            mutex.WaitOne();
        counterLock.ReleaseMutex();
    }

    private void readersFree()
    {
        counterLock.WaitOne();

        Interlocked.Decrement(ref Readers);
        //Readers--
        if (Readers == 0)
        {
            mutex.ReleaseMutex();
        }
        counterLock.ReleaseMutex();
    }


}



