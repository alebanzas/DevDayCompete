using System.Threading.Tasks;
using DevDayKeynote.Models;
using DevDayKeynote.Services;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DevDayKeynote.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVoteLog _voteLogger;

        public HomeController(IVoteLog voteLogger)
        {
            _voteLogger = voteLogger;
        }

        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> Vote(string c, string u)
        {
            _voteLogger.SendVoteAsync(c, u).RunSynchronously();
            

            if (!string.IsNullOrWhiteSpace(Request.Headers["X-Requested-With"]))
            {
                return new HttpOkResult();
            }

            return RedirectToAction("Index");
        }
        public IActionResult Dashboard()
        {
            if (!string.IsNullOrWhiteSpace(Request.Headers["X-Requested-With"]))
            {
                return new HttpOkResult();
            }

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
