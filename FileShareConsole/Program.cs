using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using System;
using System.IO;

// Get a connection string to our Azure Storage account.  You can
// obtain your connection string from the Azure Portal (click
// Access Keys under Settings in the Portal Storage account blade)
// or using the Azure CLI with:
//
//     az storage account show-connection-string --name <account_name> --resource-group <resource_group>
//
// And you can provide the connection string to your application
// using an environment variable.

namespace FileShareConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            testFunc();
        }


        static void testFunc()
        {
            string connectionString = "<connection_string>";

            // Name of the share, directory, and file we'll download from
            string shareName = "sample-share";
            string dirName = "sample-dir";
            string fileName = "sample-file";

            // Path to the save the downloaded file
            string localFilePath = @"C:\Temp\response_20211115161803.xml";

            // Get a reference to a share and then create it
            ShareClient share = new ShareClient(connectionString, shareName);
            share.CreateIfNotExists();

            // Get a reference to a directory and create it
            ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
            directory.CreateIfNotExists();

            // Get a reference to a file and upload it
            ShareFileClient file = directory.GetFileClient(fileName);
            using (FileStream stream = File.OpenRead(localFilePath))
            {
                file.Create(stream.Length);
                file.UploadRange(
                    new HttpRange(0, stream.Length),
                    stream);
            }

            //// ----------------Download----------------------
            //// Get a reference to the file
            //ShareClient share = new ShareClient(connectionString, shareName);
            //ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
            //ShareFileClient file = directory.GetFileClient(fileName);

            //// Download the file
            //ShareFileDownloadInfo download = file.Download();
            //using (FileStream stream = File.OpenWrite(localFilePath))
            //{
            //    download.Content.CopyTo(stream);
            //}
        }
    }
}
