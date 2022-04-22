using AzureStorageLibrary.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace QueueConsoleApp
{
    internal class Program
    {
        private async static Task Main(string[] args)
        {
            AzureStorageLibrary.ConnectionStrings.AzureStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=projectbyasaman;AccountKey=/i3ZOXxgBMGCCE8uX7p1FH3H3Qmd+RgZuIeiWk1M3bRRkJ4E4JRzCR/YpWqzJ0xPI/RZIHcEiI3v+AStEtfg1A==;BlobEndpoint=https://projectbyasaman.blob.core.windows.net/;QueueEndpoint=https://projectbyasaman.queue.core.windows.net/;TableEndpoint=https://projectbyasaman.table.core.windows.net/;FileEndpoint=https://projectbyasaman.file.core.windows.net/;";
            AzQueueStorage queue = new AzQueueStorage("examplequeue");

            #region SendingMessage
            string base64Message = Convert.ToBase64String(Encoding.UTF8.GetBytes("salam Yasemen Azizova"));
            queue.SendMessageAsync(base64Message).Wait();
            Console.WriteLine("sending process was succesfully");
            Console.ReadLine();
            #endregion

            #region ReadingMessage
            var message = queue.RetrieveNextMessageAsync().Result;

            string text = Encoding.UTF8.GetString(Convert.FromBase64String(message.MessageText));

            Console.WriteLine("Message: " + text);
            #endregion

            #region DeletingMessage
            await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            Console.WriteLine("Message was deleted");
            #endregion

        }
    }
}
