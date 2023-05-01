using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReversiMvcApp.Data;
using ReversiMvcApp.Models;

namespace ReversiMvcApp.Controllers
{
    public class SpelController : Controller
    {
        private readonly ReversiRestApiService _reversiRestApiService;
        private readonly ReversiDbContext _reversiDbContext;

        public SpelController(ReversiDbContext reversiDbContext, ReversiRestApiService reversiRestApiService)
        {
            _reversiDbContext = reversiDbContext;
            _reversiRestApiService = reversiRestApiService;
        }


        // GET: Spel
        public ActionResult Index()
        {
            return View(_reversiRestApiService.GetAllOpen());
        }

        // GET: Spel/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Spel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("Omschrijving")] Spel spel)
        {
            if (ModelState.IsValid)
            {
                ClaimsPrincipal gebruiker = this.User;
                var gebruikerId = gebruiker.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                spel = _reversiRestApiService.NewSpel(gebruikerId, spel.omschrijving);
                return RedirectToAction(nameof(Speel), new {id = spel.token});
            }

            return View();
        }

        public IActionResult Speel(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Spel spel = _reversiRestApiService.Get(id);

            if (spel == null)
            {
                return NotFound();
            }

            return View(spel);
        }

        public IActionResult Join(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ClaimsPrincipal gebruiker = this.User;
            var gebruikerId = gebruiker.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Spel spel = _reversiRestApiService.JoinSpel(id, gebruikerId);

            return RedirectToAction(nameof(Speel), new {id = spel.token});
        }

        [HttpGet]
        public async Task<ActionResult> Afgelopen(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Spel spel = _reversiRestApiService.Get(id);

            if (spel == null)
            {
                return NotFound();
            }

            if (spel.status != 2)
            {
                return BadRequest();
            }
            
            ClaimsPrincipal gebruiker = this.User;
            var gebruikerId = gebruiker.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Speler speler1 = _reversiDbContext.Spelers.FirstOrDefault(s => s.GUID == spel.speler1Token);
            Speler speler2 = _reversiDbContext.Spelers.FirstOrDefault(s => s.GUID == spel.speler2Token);
            
            if (speler1 == null || speler2 == null)
            {
                return NotFound();
            }

            if (gebruikerId != spel.beurt)
            {
                return Unauthorized();
            }

            int puntenSpeler1 = spel.bord.Count(p => p.Value == 1);
            int puntenSpeler2 = spel.bord.Count(p => p.Value == 2);

            if (puntenSpeler1 > puntenSpeler2)
            {
                speler1.AantalGewonnen += 1;
                speler2.AantalVerloren += 1;
            }else if (puntenSpeler2 > puntenSpeler1)
            {
                speler2.AantalGewonnen += 1;
                speler1.AantalVerloren += 1;
            }
            else
            {
                speler1.AantalGelijk += 1;
                speler2.AantalGelijk += 1;
            }

            await _reversiDbContext.SaveChangesAsync();

            if (!_reversiRestApiService.Delete(id, gebruikerId))
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}