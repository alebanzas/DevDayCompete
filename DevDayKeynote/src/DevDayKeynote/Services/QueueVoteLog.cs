using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevDayKeynote.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DevDayKeynote.Services
{
    public class QueueVoteLog : IVoteLog
    {
        private readonly CloudQueue _messageQueue;

        public QueueVoteLog()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(Startup.Configuration["AppSettings:StorageConnectionString"]);
            var createCloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            _messageQueue = createCloudQueueClient.GetQueueReference(typeof(Voto).Name.ToLowerInvariant());
        }
        public async Task SendVoteAsync(string community, string user)
        {
            var voto = new Voto
            {
                Comunidad = community,
                Usuario = user,
            };

            await _messageQueue.AddMessageAsync(voto.AsQueueMessage());
        }
    }
}
