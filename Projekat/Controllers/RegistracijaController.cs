using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projekat.Controllers
{
    public class RegistracijaController : Controller
    {
        // GET: Registracija
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registracija(Korisnik korisnik)
        {
            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];

            foreach (Korisnik k in korisnici)
            {
                if (k.KorisnickoIme == korisnik.KorisnickoIme)
                {
                    //korisnik vec postoji 
                    TempData["greska"] = $"Korisnik {korisnik.KorisnickoIme} vec postoji";
                    return RedirectToAction("Index");
                }
                else
                {
                    
                }
            }

            korisnici.Add(korisnik);
            
            HttpContext.Application["korisnici"] = korisnici;
            Podaci.UpisiKorisnika(korisnik);

            Session["korisnik"] = korisnik;
            return RedirectToAction("Index", "Home");

        }
    }
}