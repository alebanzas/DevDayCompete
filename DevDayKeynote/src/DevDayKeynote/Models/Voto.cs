using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace DevDayKeynote.Models
{
    public class Voto
    {
        public string Usuario { get; set; }

        public string Comunidad { get; set; }
    }

    public static class VotoExtensions
    {
        public static CloudQueueMessage AsQueueMessage(this Voto voto)
        {
            return new CloudQueueMessage(JsonConvert.SerializeObject(voto));
        }
    }
}