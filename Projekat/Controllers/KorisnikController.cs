using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Projekat.Controllers
{
    public class KorisnikController : Controller
    {
        // GET: Korisnik
        public ActionResult Index()
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            return View();
        }

        #region Korisnik opcije

        
        [HttpPost]
        public ActionResult Izmena(Korisnik korisnik) //forma
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];

            foreach (Korisnik k in korisnici)
            {
                if (k.KorisnickoIme == korisnik.KorisnickoIme)
                {
                    //korisnik postoji 
                    k.Lozinka = korisnik.Lozinka;
                    k.Ime = korisnik.Ime;
                    k.Prezime = korisnik.Prezime;
                    k.Pol = korisnik.Pol;
                    k.DatumRodjenja = korisnik.DatumRodjenja;
                    k.Email = korisnik.Email;
                    k.Uloga = korisnik.Uloga;
                }
            }

            HttpContext.Application["korisnici"] = korisnici;
            Podaci.UpisiKorisnike(korisnici);

            TempData["izmenaKorisnika"] = "Profil je izmenjen";

            if (prijavljeniKorisnik.KorisnickoIme != korisnik.KorisnickoIme)
            {
                //admin je izmenio neki drugi profil
                return RedirectToAction("ListaKorisnika", "Korisnik");
            }

            return RedirectToAction("Pregled", "Korisnik");
        }


        public ActionResult IzmenaProfila(string korisnickoIme = "") //view sa formom
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            if (prijavljeniKorisnik != null && prijavljeniKorisnik.Uloga == Uloga.Administrator)
            {
                foreach (Korisnik k in korisnici)
                {
                    if (k.KorisnickoIme == korisnickoIme)
                    {
                        return View("IzmenaProfila", k);
                    }
                }
            }
            return View("IzmenaProfila", prijavljeniKorisnik);

        }
       
        public ActionResult Pregled(string korisnickoIme = "")
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];

            if (prijavljeniKorisnik.Uloga == Uloga.Administrator)
            {
                foreach (Korisnik k in korisnici)
                {
                    if (k.KorisnickoIme == korisnickoIme)
                    {
                        return View("Profil", k);
                    }
                }
            }

            return View("Profil", prijavljeniKorisnik);
        }

        #endregion

        #region Administrator opcije
        public ActionResult ObrisiKorisnika(string korisnickoIme = "")
        {
            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Porudzbina> porudzbine = (List<Porudzbina>)HttpContext.Application["porudzbine"];
            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];

            string idKorisnikaZaBrisanje = korisnickoIme;

            foreach (Korisnik k in korisnici)
            {
                if (k.KorisnickoIme == idKorisnikaZaBrisanje)
                {
                    k.Obrisan = true;

                    

                        for (int i = 0; i < k.ListaPorudzbina.Count; i++)
                        {
                            if (k.ListaPorudzbina[i].StatusPorudzbine == StatusPorudzbine.Aktivna)
                            {
                                foreach (Porudzbina p in porudzbine)
                                {
                                    if (p.Id == k.ListaPorudzbina[i].Id || p.Id.Split(':')[0] == idKorisnikaZaBrisanje)
                                    {
                                        p.Proizvod.Kolicina += 1;

                                        if (p.Proizvod.Kolicina > 0)
                                        {
                                            p.Proizvod.Dostupan = true;
                                        }
                                        
                                    }
                                    p.Obrisana = true;
                                }
                                foreach (Proizvod p in proizvodi)
                                {
                                    p.Kolicina += 1;
                                    if(p.Kolicina > 0)
                                    {
                                        p.Dostupan = true;
                                    }
                                }

                            }
                        }

                        foreach (Recenzija r in recenzije)
                        {
                            if (r.Recezent.KorisnickoIme == idKorisnikaZaBrisanje || r.Id.Split(':')[0] == idKorisnikaZaBrisanje)
                            {
                                r.Obrisana = true;
                            }
                        }
                    
                   
                        for (int i = 0; i < k.ListaObjavljenihProizvoda.Count; i++)
                        {
                            foreach (Proizvod p in proizvodi)
                            {
                                if (p.Id == k.ListaObjavljenihProizvoda[i].Id)
                                {
                                    p.Obrisan = true;
                                }
                            }
                            for(int j = 0; j < k.ListaOmiljenihProizvoda.Count; j++)
                            {
                                if(k.ListaOmiljenihProizvoda[j].Id == k.ListaObjavljenihProizvoda[i].Id)
                                {
                                    k.ListaOmiljenihProizvoda[j].Obrisan = true;
                                }
                            }
                            for(int j = 0; j < k.ListaPorudzbina.Count; j++)
                            {
                                if(k.ListaPorudzbina[j].Proizvod.Id == k.ListaObjavljenihProizvoda[i].Id)
                                {
                                    k.ListaPorudzbina[j].Obrisana = true;
                                }
                            }
                            foreach(Porudzbina pr in porudzbine)
                            {
                                if(pr.Proizvod.Id == k.ListaObjavljenihProizvoda[i].Id)
                                {
                                    pr.Obrisana = true;
                                }
                            }
                        }
                    
                }
            }

            HttpContext.Application["korisnici"] = korisnici;
            HttpContext.Application["proizvodi"] = proizvodi;
            HttpContext.Application["porudzbine"] = porudzbine;
            HttpContext.Application["recenzije"] = recenzije;

            Podaci.UpisiKorisnike(korisnici);
            Podaci.UpisiProizvode(proizvodi);
            Podaci.UpisiPorudzbine(porudzbine);
            Podaci.UpisiRecenzije(recenzije);

            Podaci.Osvezi();

            return RedirectToAction("ListaKorisnika", "Korisnik");
        }
        public ActionResult ListaKorisnika()
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Korisnik> ret = korisnici.Where(x => x.Uloga == Uloga.Prodavac || x.Uloga == Uloga.Kupac).ToList();

            if(ret.Count == 0)
            {
                TempData["nemaKorisnika"] = "Nema korisnika za prikaz";
            }

            return View("Korisnici", ret);
        }

        #region Dodavanje prodavca
        public ActionResult Dodaj()
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            return View("Index");
        }
        [HttpPost]
        public ActionResult DodajProdavca(Korisnik korisnik)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Korisnik> ret = new List<Korisnik>();

            foreach (Korisnik k in korisnici)
            {
                if (k.KorisnickoIme == korisnik.KorisnickoIme)
                {
                    //korisnik vec postoji 
                    TempData["korisnikVecPostoji"] = "vec postoji";
                    return View("Index");
                }
            }

            korisnik.Obrisan = false;
            korisnici.Add(korisnik);
            HttpContext.Application["korisnici"] = korisnici;

            Podaci.UpisiKorisnika(korisnik);
            ret = korisnici.Where(x => (x.Uloga == Uloga.Prodavac || x.Uloga == Uloga.Kupac)).ToList();

            return View("Korisnici", ret);
        }
        #endregion

        #endregion

        #region Pretraga
        public ActionResult Pretraga(string pretragaIme, string pretragaPrezime, Uloga pretragaUloga, DateTime pretragaOdDatuma, DateTime pretragaDoDatuma)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> ret = new List<Korisnik>();
            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            ret = korisnici;

            if (pretragaIme != "")
            {
                ret = ret.Where(x => x.Ime == pretragaIme).ToList();
            }
            if (pretragaPrezime != "")
            {
                ret = ret.Where(x => x.Prezime == pretragaPrezime).ToList();
            }
            if (pretragaUloga == Uloga.Administrator)
            {
                ret = ret.Where(x => x.Uloga == Uloga.Kupac || x.Uloga == Uloga.Prodavac).ToList();
            }
            if (pretragaUloga == Uloga.Kupac)
            {
                ret = ret.Where(x => x.Uloga == Uloga.Kupac).ToList();
            }
            if (pretragaUloga == Uloga.Prodavac)
            {
                ret = ret.Where(x => x.Uloga == Uloga.Prodavac).ToList();
            }
            if (pretragaOdDatuma.Date.ToString("yyyy-MM-dd") != "1900-01-01")
            {
                ret = ret.Where(x => x.DatumRodjenja >= pretragaOdDatuma).ToList();
            }
            if (pretragaDoDatuma.Date.ToString("yyyy-MM-dd") != DateTime.Now.ToString("yyyy-MM-dd"))
            {
                ret = ret.Where(x => x.DatumRodjenja <= pretragaDoDatuma).ToList();
            }

            return View("Korisnici", ret);
        }
        #endregion

        #region Kupac opcije
        public ActionResult DodajOmiljeni(string korisnickoIme, int id)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];

            bool vecDodat = false;
            foreach (Korisnik k in korisnici)
            {
                if (k.KorisnickoIme == korisnickoIme)
                {
                    for (int i = 0; i < k.ListaOmiljenihProizvoda.Count; i++)
                    {
                        if (k.ListaOmiljenihProizvoda[i].Id == id)
                        {
                            TempData["vecOmiljeni"] = $"{k.ListaOmiljenihProizvoda[i].Naziv} je vec dodat u omiljene";
                            vecDodat = true;
                            break;
                        }
                    }
                }
            }

            if (!vecDodat)
            {
                prijavljeniKorisnik.ListaOmiljenihProizvoda.Add(proizvodi.Find(x => x.Id == id));

                HttpContext.Application["korisnici"] = korisnici;
                HttpContext.Application["proizvodi"] = proizvodi;

                Podaci.UpisiKorisnike(korisnici);
                Podaci.UpisiProizvode(proizvodi);

                return View("Profil", prijavljeniKorisnik);
            }

            HttpContext.Application["korisnici"] = korisnici;
            HttpContext.Application["proizvodi"] = proizvodi;

            Podaci.UpisiKorisnike(korisnici);
            Podaci.UpisiProizvode(proizvodi);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Poruci(string korisnickoIme, int id)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Porudzbina> porudzbine = (List<Porudzbina>)HttpContext.Application["porudzbine"];

            bool vecDodat = false;

            foreach (Korisnik k in korisnici)
            {
                if (k.KorisnickoIme == prijavljeniKorisnik.KorisnickoIme)
                {
                    for (int i = 0; i < k.ListaPorudzbina.Count; i++)
                    {
                        if (k.ListaPorudzbina[i].Id == $"{prijavljeniKorisnik.KorisnickoIme}:{id}")
                        {
                            TempData["vecPorucen"] = $"{k.ListaPorudzbina[i].Proizvod.Naziv} je vec porucen";
                            vecDodat = true;
                            break;
                        }
                    }
                }
            }

            if (!vecDodat)
            {
                foreach (Korisnik k in korisnici)
                {
                    if (k.KorisnickoIme == prijavljeniKorisnik.KorisnickoIme)
                    {
                        foreach (Proizvod p in proizvodi)
                        {
                            if (p.Id == id && p.Dostupan && p.Kolicina >= 1)
                            {
                                //izmeni proizvod
                                p.Kolicina -= 1;

                                if (p.Kolicina == 0)
                                {
                                    p.Dostupan = false;
                                }

                                //napravi porudzbinu
                                Porudzbina porudzbina = new Porudzbina(p, 1, k, DateTime.Now.Date, StatusPorudzbine.Aktivna);
                                porudzbina.Id = $"{k.KorisnickoIme}:{p.Id}";

                                porudzbine.Add(porudzbina);

                                k.ListaPorudzbina.Add(porudzbina);

                            }
                        }
                    }
                }
            }


            if (!vecDodat)
            {
                HttpContext.Application["korisnici"] = korisnici;
                HttpContext.Application["proizvodi"] = proizvodi;
                HttpContext.Application["porudzbine"] = porudzbine;

                Podaci.UpisiKorisnike(korisnici);
                Podaci.UpisiPorudzbine(porudzbine);
                Podaci.UpisiProizvode(proizvodi);

                return View("Profil", prijavljeniKorisnik);
            }


            HttpContext.Application["korisnici"] = korisnici;
            HttpContext.Application["proizvodi"] = proizvodi;
            HttpContext.Application["porudzbine"] = porudzbine;

            Podaci.UpisiKorisnike(korisnici);
            Podaci.UpisiPorudzbine(porudzbine);
            Podaci.UpisiProizvode(proizvodi);

            return RedirectToAction("Index", "Home");

        }

        #endregion

        #region Sortiranje Liste Korisnika
        public ActionResult SortirajPoImenu()
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Korisnik> ret = new List<Korisnik>();

            ret = korisnici.Where(x => (x.Uloga == Uloga.Prodavac || x.Uloga == Uloga.Kupac)).ToList();
            var sortiranje = Request["sortiranjePoImenu"] != null ? Request["sortiranjePoImenu"] : string.Empty;

            if (sortiranje == "opadajuce")
            {
                ret = ret.OrderByDescending(x => x.Ime).ToList();
            }
            else if (sortiranje == "rastuce")
            {
                ret = ret.OrderBy(x => x.Ime).ToList();
            }

            return View("Korisnici", ret);
        }
        public ActionResult SortirajPoDatumuRodjenja()
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Korisnik> ret = new List<Korisnik>();

            ret = korisnici.Where(x => (x.Uloga == Uloga.Prodavac || x.Uloga == Uloga.Kupac)).ToList();
            var sortiranje = Request["sortiranjePoDatumu"] != null ? Request["sortiranjePoDatumu"] : string.Empty;

            if (sortiranje == "opadajuce")
            {
                ret = ret.OrderByDescending(x => x.DatumRodjenja).ToList();
            }
            else if (sortiranje == "rastuce")
            {
                ret = ret.OrderBy(x => x.DatumRodjenja).ToList();
            }

            return View("Korisnici", ret);
        }
        public ActionResult SortirajPoUlozi()
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Korisnik> ret = new List<Korisnik>();

            ret = korisnici.Where(x => (x.Uloga == Uloga.Prodavac || x.Uloga == Uloga.Kupac)).ToList();
            var sortiranje = Request["sortiranjePoUlozi"] != null ? Request["sortiranjePoUlozi"] : string.Empty;

            if (sortiranje == "opadajuce")
            {
                ret = ret.OrderByDescending(x => x.Uloga).ToList();
            }
            else if (sortiranje == "rastuce")
            {
                ret = ret.OrderBy(x => x.Uloga).ToList();
            }

            return View("Korisnici", ret);
        }
        #endregion

    }
}