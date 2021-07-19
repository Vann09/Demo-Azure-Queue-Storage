using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Text;
using System.Threading.Tasks;

public class Program 
{
    private const string storageConnectionString = "<Access-key>";
    private const string queueName = "messagequeue";
    public static async Task Main (string[] args)
    {
        QueueClient client = new QueueClient(storageConnectionString, queueName);
        await client.CreateAsync();
        
        Console.WriteLine($"---Metadata de la cuenta---");
        Console.WriteLine ($"Account Uri: \t{client.Uri}");
        Console.WriteLine($"---Mensajes existentes---");

        int batchSize = 10;
        TimeSpan visibilityTimeout = TimeSpan.FromSeconds(2.5d);
        
        Response<QueueMessage[]> messages = await client.ReceiveMessagesAsync(batchSize, visibilityTimeout);
        
        foreach(QueueMessage message in messages?.Value)
        {
            Console.WriteLine($"[{message.MessageId}]\t{message.MessageText}");
            await client.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        }
        Console.WriteLine($"---Nuevos Mensajes---");
        string greeting = "Hola desarrollador!";
        await client.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(greeting)));
    
        Console.WriteLine($"Mensaje Enviado:\t{greeting}");
    }
}