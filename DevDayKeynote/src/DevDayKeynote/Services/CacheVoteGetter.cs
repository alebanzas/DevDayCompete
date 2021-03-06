﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevDayKeynote.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
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

        private readonly ApplicationDbContext _dbContext;

        private readonly CloudQueue _messageQueue;

        public CacheVoteGetter(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

            var cloudStorageAccount = CloudStorageAccount.Parse(Startup.Configuration["MicrosoftAzureStorage:devdaydemo_AzureStorageConnectionString"]);
            var createCloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            _messageQueue = createCloudQueueClient.GetQueueReference(typeof(Voto).Name.ToLowerInvariant());
        }

        public Task<VotoResult> GetVoteAsync()
        {
            VotoResult result = new VotoResult
            {
                Php = (int) (MemoryCache.Get("php") ?? GetFromDB("php")),
                Net = (int) (MemoryCache.Get("net") ?? GetFromDB("net")),
                Java = (int) (MemoryCache.Get("java") ?? GetFromDB("java")),
                Javascript = (int) (MemoryCache.Get("javascript") ?? GetFromDB("javascript")),
            };

            return Task.FromResult(result);
        }

        private int GetFromDB(string community)
        {
            var votos = _dbContext.Votos.Count(x => x.Comunidad == community);

            MemoryCache.Set(community, votos);

            return votos;
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
