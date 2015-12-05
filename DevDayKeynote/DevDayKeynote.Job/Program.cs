using Microsoft.Azure.WebJobs;
using System;
using System.Configuration;
using System.IO;

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

            var _storageConn = ConfigurationManager
            .ConnectionStrings["AzureWebJobsStorage"].ConnectionString;

            var _dashboardConn = ConfigurationManager
                .ConnectionStrings["AzureWebJobsDashboard"].ConnectionString;
            
            var host = new JobHost(new JobHostConfiguration
            {
                StorageConnectionString = _storageConn,
                DashboardConnectionString = _dashboardConn,
            });
            host.RunAndBlock();
        }

        private static bool VerifyConfiguration()
        {
            var webJobsStorage = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;
            var webJobsDashboard = ConfigurationManager.ConnectionStrings["AzureWebJobsDashboard"].ConnectionString;
            var databaseConn = ConfigurationManager.ConnectionStrings["AzureWebJobsDatabase"].ConnectionString;

            var configOK = true;
            if (string.IsNullOrWhiteSpace(webJobsStorage) || string.IsNullOrWhiteSpace(webJobsDashboard))
            {
                configOK = false;
                Console.WriteLine("Please add the Azure Storage account credentials in App.config");
            }
            if (string.IsNullOrWhiteSpace(databaseConn))
            {
                configOK = false;
                Console.WriteLine("Please add the Database connstring in App.config");
            }
            return configOK;
        }
    }
}
