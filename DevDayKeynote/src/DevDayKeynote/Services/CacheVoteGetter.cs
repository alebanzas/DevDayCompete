using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevDayKeynote.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DevDayKeynote.Services
{
    public class CacheVoteGetter : IVoteGet, IVoteLog
    {
        private static readonly IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions
        {
            CompactOnMemoryPressure = false,
        });

        private CloudQueue _messageQueue;

        public CacheVoteGetter()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(Startup.Configuration["MicrosoftAzureStorage:devdaydemo_AzureStorageConnectionString"]);
            var createCloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            _messageQueue = createCloudQueueClient.GetQueueReference(typeof(Voto).Name.ToLowerInvariant());
        }

        public Task<VotoResult> GetVoteAsync()
        {
            VotoResult result = new VotoResult
            {
                Php = (int) (MemoryCache.Get("php") ?? 0),
                Net = (int) (MemoryCache.Get("net") ?? 0),
                Java = (int) (MemoryCache.Get("java") ?? 0),
                Javascript = (int) (MemoryCache.Get("javascript") ?? 0),
            };

            return Task.FromResult(result);
        }

        public async Task SendVoteAsync(string community, string user)
        {
            var c = community.ToLowerInvariant();
            var voto = new Voto
            {
                Comunidad = c,
                Usuario = user,
            };

            await _messageQueue.AddMessageAsync(voto.AsQueueMessage());

            await Task.Run(() =>
            {
                var v = (int)(MemoryCache.Get(c) ?? 0);

                MemoryCache.Set(c, ++v);
            });
        }
    }
}
