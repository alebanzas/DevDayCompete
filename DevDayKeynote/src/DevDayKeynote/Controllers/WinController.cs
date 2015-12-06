using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using DevDayKeynote.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity.Internal;

namespace DevDayKeynote.Controllers
{
    public class WinController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public WinController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            var grupos = _dbContext.Votos
                .GroupBy(x => new
                {
                    x.Usuario,
                    x.Comunidad
                })
                .OrderByDescending(x => x.Count())
                .Select(grupo => new
                {
                    grupo.Key.Comunidad,
                    grupo.Key.Usuario,
                    Votos = grupo.Count(),
                })
                .ToList()
                .GroupBy(x => x.Comunidad)
                .SelectMany(x => x.Take(1));


            var result = grupos.Select(x => $"{x.Comunidad} {x.Usuario} ({x.Votos})").ToList();


            return View(result);
        }
    }
}
