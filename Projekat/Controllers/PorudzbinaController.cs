using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Projekat.Controllers
{
    public class PorudzbinaController : Controller
    {
        // GET: Porudzbina
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult ListaPorucenihProizvoda(string korisnickoIme = "") //kupac
        {
           
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Porudzbina> porudzbine = (List<Porudzbina>)HttpContext.Application["porudzbine"];
            List<Porudzbina> ret = new List<Porudzbina>();

            foreach (Korisnik k in korisnici)
            {
                for(int i = 0; i< k.ListaPorudzbina.Count; i++)
                {
                    string[] idPorudzbine = k.ListaPorudzbina[i].Id.Split(':');
                    string idKorisnika = idPorudzbine[0];
                    int idProizvoda = Int32.Parse(idPorudzbine[1]);

                    k.ListaPorudzbina[i].Kupac = korisnici.Find(x => x.KorisnickoIme == idKorisnika);
                    k.ListaPorudzbina[i].Proizvod = proizvodi.Find(x => x.Id == idProizvoda);
                }
            }

            ret = (korisnici.Find(x => x.KorisnickoIme == korisnickoIme).ListaPorudzbina) == null ? new List<Porudzbina>() : korisnici.Find(x => x.KorisnickoIme == korisnickoIme).ListaPorudzbina;

            HttpContext.Application["porudzbine"] = porudzbine;

            if(ret.Count == 0)
            {
                TempData["nemaPorudzbina"] = "Nema porudzbina za prikaz";
            }

            return View("Porudzbine", ret);
        }
        public ActionResult ListaPorudzbina(string korisnickoIme = "") //admin
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Porudzbina> porudzbine = (List<Porudzbina>)HttpContext.Application["porudzbine"];

            if (porudzbine.Count == 0)
            {
                TempData["nemaPorudzbina"] = "Nema porudzbina za prikaz";
            }

            return View("Porudzbine", porudzbine);
        }
        public ActionResult Izvrsi(int id, string korisnickoIme = "")
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;
            
            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Porudzbina> porudzbine = (List<Porudzbina>)HttpContext.Application["porudzbine"];
            
            foreach(Porudzbina p in porudzbine)
            {
                if(p.Kupac.KorisnickoIme == korisnickoIme && p.Proizvod.Id == id || p.Id == $"{korisnickoIme}:{id}")
                {
                    if(p.StatusPorudzbine == StatusPorudzbine.Aktivna)
                    {
                        p.StatusPorudzbine = StatusPorudzbine.Izvrsena;
                    }
                }
            }
            foreach (Korisnik korisnik in korisnici)
            {
                for (int i = 0; i < korisnik.ListaPorudzbina.Count; i++)
                {
                    if(korisnik.ListaPorudzbina[i].Id == $"{korisnickoIme}:{id}" && 
                        korisnik.ListaPorudzbina[i].StatusPorudzbine == StatusPorudzbine.Aktivna)
                    {
                        korisnik.ListaPorudzbina[i].StatusPorudzbine = StatusPorudzbine.Izvrsena;
                    }
                }
            }

            HttpContext.Application["korisnici"] = korisnici;
            HttpContext.Application["porudzbine"] = porudzbine;

            Podaci.UpisiKorisnike(korisnici);
            Podaci.UpisiPorudzbine(porudzbine);

            List<Porudzbina> ret = new List<Porudzbina>();

            if (prijavljeniKorisnik.Uloga == Uloga.Kupac)
            {
                korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
                foreach(Korisnik k in korisnici)
                {
                    if(k.KorisnickoIme == ViewBag.Korisnik.KorisnickoIme)
                    {
                        ret = k.ListaPorudzbina;
                    }
                }
                return View("Porudzbine", ret);

            }
            else /*if(prijavljeniKorisnik.Uloga == Uloga.Administrator)*/
            {
                ret = porudzbine;
                return View("Porudzbine", ret);
            }
        }
        public ActionResult Otkazi(int id, string korisnickoIme = "") //ADMIN
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Porudzbina> porudzbine = (List<Porudzbina>)HttpContext.Application["porudzbine"];

            foreach (Porudzbina p in porudzbine)
            {
                if (p.Kupac.KorisnickoIme == korisnickoIme && p.Proizvod.Id == id || p.Id == $"{korisnickoIme}:{id}")
                {
                    if (p.StatusPorudzbine == StatusPorudzbine.Aktivna)
                    {
                        p.StatusPorudzbine = StatusPorudzbine.Otkazana;
                    }
                }
            }

            foreach (Korisnik korisnik in korisnici)
            {
                for (int i = 0; i < korisnik.ListaPorudzbina.Count; i++)
                {
                    if (korisnik.ListaPorudzbina[i].Id == $"{korisnickoIme}:{id}" &&
                        korisnik.ListaPorudzbina[i].StatusPorudzbine == StatusPorudzbine.Aktivna)
                    {
                        korisnik.ListaPorudzbina[i].StatusPorudzbine = StatusPorudzbine.Otkazana;
                    }
                }
            }

            foreach(Proizvod proizvod in proizvodi)
            {
                if(proizvod.Id == id)
                {
                    proizvod.Kolicina += 1;

                    if (proizvod.Kolicina >= 1)
                    {
                        proizvod.Dostupan = true;
                    }
                }
            }
            HttpContext.Application["korisnici"] = korisnici;
            HttpContext.Application["proizvodi"] = proizvodi;
            HttpContext.Application["porudzbine"] = porudzbine;

            Podaci.UpisiKorisnike(korisnici);
            Podaci.UpisiProizvode(proizvodi);
            Podaci.UpisiPorudzbine(porudzbine);
            

            List<Porudzbina> ret = new List<Porudzbina>();
            if (prijavljeniKorisnik.Uloga == Uloga.Kupac)
            {
                ret = prijavljeniKorisnik.ListaPorudzbina;
                return View("Porudzbine", ret);

            }
            else /*if(prijavljeniKorisnik.Uloga == Uloga.Administrator)*/
            {
                ret = porudzbine;
                return View("Porudzbine", ret);
            }
        }

    }
}