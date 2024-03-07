using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projekat.Controllers
{
    public class PrijavaController : Controller
    {
        // GET: Prijava
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Prijava(string korisnickoIme, string lozinka)
        {
            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];

            Korisnik korisnik = new Korisnik();

            foreach (Korisnik k in korisnici)
            {
                if (k.KorisnickoIme.Equals(korisnickoIme) && k.Lozinka.Equals(lozinka) && !k.Obrisan)
                {
                    korisnik = k;
                    Session["korisnik"] = korisnik;

                    HttpContext.Application["korisnici"] = korisnici;
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["greska"] = "Korisnik ne postoji";
            return RedirectToAction("Index");

        }
        public ActionResult Odjava()
        {
            Session["korisnik"] = null;
            ViewBag.Korisnik = null;
            return RedirectToAction("Index", "Home");
        }
    }
}