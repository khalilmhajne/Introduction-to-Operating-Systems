using System;
using System.Threading ;
using System.IO;



namespace DistributedSearch
{
    class Program
    {         
        static void Main(string[] args)
        {
            int n ;
            //parameters : textfile.txt StringToSearch nThreads Delta
            String textfile = (String)args[0] ;
            String StringToSearch = (String)args[1];
            int nThread = int.Parse(args[2]) ;
            int Delta = int.Parse(args[3]) ;
          
            
            //open the file
            using (FileStream fsSource = File.OpenRead(textfile))
            {
                
            // Read the source file into a byte array.
            byte [] bytes = new byte[10000];
            int numBytesToRead = (int)fsSource.Length;
            int numBytesRead = 0;
            ManualResetEvent myWaitHandle = new ManualResetEvent(false);

             //set number of threads
            ThreadPool.SetMinThreads(nThread,nThread);
            ThreadPool.SetMaxThreads(nThread,nThread);
            while (numBytesToRead > 0 )
                {
            // Read may return anything from 0 to numBytesToRead.
                if(numBytesToRead < 10000){
                    Array.Clear(bytes,0,10000);//clear the buffer
                    n= fsSource.Read(bytes, 0, numBytesToRead);                      
                }else
                {
                    n = fsSource.Read(bytes, 0, 10000); 
                }
                numBytesRead += n;
                numBytesToRead -= n;
                String buffer = System.Text.Encoding.UTF8.GetString(bytes) ;   
            //multi-Threading
                ThreadPool.QueueUserWorkItem( search ,new object[] {buffer , StringToSearch , Delta , numBytesRead -n , myWaitHandle });
                myWaitHandle.WaitOne();    
                }
            }
            //if the application wasn't find the string - ( any thread )
            Console.WriteLine("output not found.");
        }

          static void search ( Object arg )
        {            
             object[] array = arg as object[];
            String arr1 = (string)array[0];
            String word = (string)array[1];
            int delta = (int)array[2];
            int file_indexer = (int)array[3];
            ManualResetEvent waitHandleFromParent = (ManualResetEvent)array[4];
 
            // TODO delta=0 need to k Done
            delta = delta + 1;
            int index=0; // holds the  Index of  Substring Start  (If Exist)! 
            string arr;
            arr = arr1.ToLower(); // All Lower Cases  
            word = word.ToLower();
            //  arr = arr.Replace(" ",""); // Remove Spaces 
            // Itereate Over Arr and search for subString
            while (arr.Length > 0 )
            {
                //Console.WriteLine(arr.Length);
                if (arr.IndexOf(word[0])==-1)
                break;
                index = index + arr.IndexOf(word[0]);
                arr = arr.Substring(arr.IndexOf(word[0]));
                string candidate="";
                candidate = candidate + arr[arr.IndexOf(word[0])];    
                for(int i=1;i<word.Length;i++)
                {
                    if (i * delta > arr.Length)
                        index = -1 ;
                    char temp = arr[i * delta];
                    candidate += temp;
                    if (temp != word[i])
                        break;                    
                }
                arr = arr.Substring(1);
                if (candidate.Equals(word)){
                    
                    Console.WriteLine("output: " + (file_indexer + index)) ;// the index of string in the textfile
                    Environment.Exit(Environment.ExitCode); //stop all the threads
                    waitHandleFromParent.Set();
                }  
            }
            waitHandleFromParent.Set();
           
        }
    }      
}


        
    
         
    

        
       
        
    


