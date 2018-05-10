using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DotnetWorker
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Auth;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine();

            LoadDataAsync().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit the sample application.");
            Console.ReadLine();
        }

        private static async Task LoadDataAsync()
        {
            // Constants
            const int SIZE_OF_DOUBLE = 8;
            
            var accountName = Environment.GetEnvironmentVariable("AZURE_ACC_NAME");
            var accountKey = Environment.GetEnvironmentVariable("AZURE_ACC_KEY");

            // TODO: these should be parsed ...
            var containerName = "test";
            var blobName = "dataset_40k_3ref";
            var startRange = 0;
            var nSamples = 40000;
            var nPoints = 4200;
            var nRefs = 3;

            // Calculate byte range
            var nBytes = nSamples * ((nPoints + nRefs)*SIZE_OF_DOUBLE);
            var endRange = startRange + nBytes - 1;

            // Instantiate target array
            var target = new byte[nBytes];

            // Get storage client
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer blobContainer = null;

            StorageCredentials credentials = new StorageCredentials(accountName, accountKey);
            storageAccount = new CloudStorageAccount(credentials, useHttps: true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get container ref
            blobContainer = blobClient.GetContainerReference(containerName);

            // Download blob as byte array
            var blob = blobContainer.GetBlockBlobReference(blobName);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var data = await blob.DownloadRangeToByteArrayAsync(target, 0, 0, nBytes);
            stopWatch.Stop();

            Console.WriteLine(stopWatch.Elapsed);
        }
    }
}
