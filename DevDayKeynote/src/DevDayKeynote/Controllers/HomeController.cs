using System.Threading.Tasks;
using DevDayKeynote.Models;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.OptionsModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DevDayKeynote.Controllers
{
    public class HomeController : Controller
    {
        private readonly CloudQueue MessageQueue;

        public HomeController()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(Startup.Configuration["AppSettings:StorageConnectionString"]);
            var createCloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            MessageQueue = createCloudQueueClient.GetQueueReference(typeof(Voto).Name.ToLowerInvariant());
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c">comunidad</param>
        /// <param name="u">usuario</param>
        /// <returns></returns>
        public async Task<IActionResult> Vote(string c, string u)
        {
            var voto = new Voto
            {
                Comunidad = c,
                Usuario = u,
            };

            await MessageQueue.AddMessageAsync(voto.AsQueueMessage());

            if (Request.Headers["X-Requested-With"] != null &&
                !string.IsNullOrWhiteSpace(Request.Headers["X-Requested-With"]))
            {
                return new HttpOkResult();
            }

            return RedirectToAction("Index");
        }
        
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
