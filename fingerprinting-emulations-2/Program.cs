using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace fingerprinting_emulations_2
{
    class Program
    {
        static void Main(string[] args)
        {
            //Firslty get all the Logical Drives of the system using System.IO.DriveInfo.GetDrives method
            //This will return an array of objects of all the drives in the filesystem
            DriveInfo[] drives = DriveInfo.GetDrives();
            Console.WriteLine("==============================================Logical Drives=======================");
            foreach (DriveInfo disk in drives)
            {
                //Print the name of the disk, filesystem type, total space, and free space available.
                Console.WriteLine("============Disk " + disk.Name + " Information ==================");
                Console.WriteLine("  Drive type: {0}", disk.DriveType);

                //check whether the disk is ready to avoid IOExceptions
                if (disk.IsReady == true)
                {
                    Console.WriteLine(" File system Type: {0}", disk.DriveFormat);
                    Console.WriteLine(" Total Free space: {0}",convertBytes(disk.TotalFreeSpace));
                    Console.WriteLine(" Total  space: {0}", convertBytes(disk.TotalSize));

                    //Get the root directory of the Disk and all the files there.
                    System.IO.DirectoryInfo rootDir = disk.RootDirectory;
                    FileInfo[] files = rootDir.GetFiles("*.*");

                    //Print the name of the root file & its LastAccessTime
                    Console.WriteLine("===Root files==");
                    Console.WriteLine("================");
                    foreach (System.IO.FileInfo file in files)
                    {
                        Console.WriteLine("{0}: {1}: ", file.Name, file.LastAccessTime);
                    }

                    //Now get the subdirectories under the root
                    DirectoryInfo[] subDirectories = rootDir.GetDirectories("*.*");
                    Console.WriteLine("===Sub-directories of root==");
                    Console.WriteLine("=============================");
                    foreach (System.IO.DirectoryInfo directory in subDirectories)
                    {
                        Console.WriteLine(directory.Name);
                    }

                    
                }
            }

            /*
            //This code is for retrieving all the executables in the filesystem
            string startDir = @"C:\";
            try
            {
                //all files ending in .exe
                //SearchOption.AllDirectories => All subdirectories of the specified directory
                String[] executableFiles = Directory.GetFiles(startDir,".exe", SearchOption.AllDirectories).Select(fileName => Path.GetFileNameWithoutExtension(fileName)).AsEnumerable().ToArray();
                foreach (string file in executableFiles) {
                    Console.WriteLine(file);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }  */

            //Get the Computer-Name && MAC Address
            Console.WriteLine("===========Machine info================");
            Console.WriteLine("MachineName: {0}", Environment.MachineName);
            printNICinfo();


            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            
        }

        //Convert bytes to any other form
        private static string convertBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int iterator;
            double dblSByte = bytes;
            //While the number of byte is greater than 1Kb and we havent iterate through all types
            // divide the bytes by 1024 to convert it to the next form
            for (iterator = 0; iterator < Suffix.Length && bytes >= 1024; iterator++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[iterator]);
        }

        //Get the MAC address
        private static void printNICinfo(){
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in nics)
            {
                //Get only the NIC which are working and ignore Virtual or PSeudo Interfaces
                if (nic.OperationalStatus == OperationalStatus.Up && (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                {
                    if (nic.GetPhysicalAddress().ToString() != "")
                    {
                        //Print information related to the NIC
                        Console.WriteLine("NIC name: {0}", nic.Name);
                        Console.WriteLine("NIC MAC address: {0}", nic.GetPhysicalAddress().ToString());
                        Console.WriteLine("NIC Description: {0}", nic.Description);
                        //get the ip address
                        IPInterfaceProperties properties = nic.GetIPProperties();
                        foreach (IPAddressInformation unicast in properties.UnicastAddresses)
                        {      
                        Console.WriteLine(unicast.Address); 
                         }

                    }
                }
            }
       
        }
    }
}

