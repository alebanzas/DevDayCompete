using Microsoft.Azure.WebJobs;
using System;
using System.Configuration;

namespace DevDayKeynote.Job
{
    class Program
    {
        static void Main()
        {
            if (!VerifyConfiguration())
            {
                Console.ReadLine();
                return;
            }
            


            JobHost host = new JobHost();
            host.RunAndBlock();
        }

        private static bool VerifyConfiguration()
        {
            var webJobsStorage = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;

            var configOK = true;
            if (string.IsNullOrWhiteSpace(webJobsStorage))
            {
                configOK = false;
                Console.WriteLine("Please add the Azure Storage account credentials in App.config");

            }
            return configOK;
        }
        
    }
}
