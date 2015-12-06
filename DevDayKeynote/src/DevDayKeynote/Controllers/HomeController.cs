using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevDayKeynote.Models;
using DevDayKeynote.Services;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json.Converters;

namespace DevDayKeynote.Controllers
{
    public class HomeController : Controller
    {

        private readonly IVoteGet _voteGetter;

        private readonly IVoteLog _voteLogger;

        public HomeController(IVoteLog voteLogger, IVoteGet voteGetter)
        {
            _voteLogger = voteLogger;
            _voteGetter = voteGetter;
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
            var sendVoteAsync = _voteLogger.SendVoteAsync(c, u);
            
            await sendVoteAsync;

            if (sendVoteAsync.IsCanceled)
            {
                var a = "";
            }

            if (!string.IsNullOrWhiteSpace(Request.Headers["X-Requested-With"]))
            {
                return new HttpOkResult();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Dashboard()
        {
            var result = await _voteGetter.GetVoteAsync();

            if (!string.IsNullOrWhiteSpace(Request.Headers["X-Requested-With"]))
            {
                return Json(new
                {
                    Php = result.Php,
                    Java = result.Java,
                    Javascript = result.Javascript,
                    Net = result.Net,
                });
            }

            return View(result);
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
