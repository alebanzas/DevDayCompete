using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;

namespace DevDayKeynote.Job
{
    public class Functions
    {
        private string _databaseConn = ConfigurationManager.ConnectionStrings["AzureWebJobsDatabase"].ConnectionString;

        public static void ProcessQueueMessage([QueueTrigger("voto")] string logMessage, TextWriter logger)
        {
            var voto = (Voto)JsonConvert.DeserializeObject(logMessage);


        }
    }
}
