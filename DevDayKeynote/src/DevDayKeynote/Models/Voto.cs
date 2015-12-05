using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace DevDayKeynote.Models
{
    public class Voto
    {
        public int Id { get; set; }

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

    public class VotoResult
    {
        public int Php { get; set; }
        public int Net { get; set; }
        public int Java { get; set; }
        public int Javascript { get; set; }
    }
}