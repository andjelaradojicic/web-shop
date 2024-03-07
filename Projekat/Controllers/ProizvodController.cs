using Projekat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projekat.Controllers
{
    public class ProizvodController : Controller
    {
        // GET: Proizvod
        public ActionResult Index()
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = korisnik;

            return View();
        }

        #region Prikaz proizvoda
        public ActionResult DetaljniPrikaz(int id)
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = korisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];



            List<Recenzija> ret = new List<Recenzija>();


            foreach (Proizvod p in proizvodi)
            {
                for (int i = 0; i < p.ListaRecenzija.Count; i++)
                {
                    foreach (Recenzija r in recenzije)
                    {
                        string[] idRecenzije = r.Id.Split(':');
                        string idKorisnika = idRecenzije[0];
                        int idProizvoda = Int32.Parse(idRecenzije[1]);

                        r.Proizvod = proizvodi.Find(x => x.Id == idProizvoda);
                        r.Recezent = korisnici.Find(x => x.KorisnickoIme == idKorisnika);
                        if (p.ListaRecenzija[i].Id == r.Id)
                        {
                            p.ListaRecenzija[i].Proizvod = r.Proizvod;
                            p.ListaRecenzija[i].Recezent = r.Recezent;
                        }
                    }

                }
            }


            Proizvod proizvod = proizvodi.Find(x => x.Id == id);


            return View("PrikazProizvoda", proizvod);
        }

        #endregion

        #region Dodaj

        public ActionResult Dodaj(string korisnickoIme)//
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = korisnik;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DodajProizvod(Proizvod proizvod, HttpPostedFileBase slika) //
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;


            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];

            foreach (Proizvod p in proizvodi)
            {
                if (p.Id == proizvod.Id)
                {
                    TempData["proizvodPostoji"] = $"Proizvod sa Id:{proizvod.Id} vec postoji";
                    return RedirectToAction("Index");
                }
            }

            proizvod.DatumPostavljanja = DateTime.Now.Date;
            proizvod.Dostupan = proizvod.Kolicina != 0;
            proizvod.Obrisan = false;

            try
            {
                if (slika.ContentLength > 0)
                {
                    string naziv = Path.GetFileName(slika.FileName);
                    string putanjaSlika = Path.Combine(Server.MapPath("~/Images/"), naziv);
                    slika.SaveAs(putanjaSlika);
                    proizvod.PutanjaDoSlike = naziv;

                }

                proizvodi.Add(proizvod);
                for (int i = 0; i < korisnici.Count; i++)
                {
                    if (korisnici[i].KorisnickoIme == prijavljeniKorisnik.KorisnickoIme)
                    {
                        korisnici[i].ListaObjavljenihProizvoda.Add(proizvod);
                    }
                }
                //prijavljeniKorisnik.ListaObjavljenihProizvoda.Add(proizvod);

                HttpContext.Application["proizvodi"] = proizvodi;
                HttpContext.Application["korisnici"] = korisnici;

                Podaci.UpisiKorisnike(korisnici);
                Podaci.UpisiProizvode(proizvodi);

                return View("../Korisnik/Profil", prijavljeniKorisnik);
            }

            catch
            {
                return RedirectToAction("Index");
            }


        }

        #endregion

        #region Izmena
        public ActionResult Izmena(string korisnickoIme, int id)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            Proizvod proizvod = new Proizvod();
            proizvod = proizvodi.Find(x => x.Id == id);

            return View("IzmenaProizvoda", proizvod);
        }

        [HttpPost]
        public ActionResult IzmeniProizvod(Proizvod proizvod, HttpPostedFileBase slika)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Porudzbina> porudzbine = (List<Porudzbina>)HttpContext.Application["porudzbine"];

            foreach (Proizvod p in proizvodi)
            {
                if (p.Id == proizvod.Id)
                { //izmena
                    p.Id = proizvod.Id;
                    p.Naziv = proizvod.Naziv;
                    p.Cena = proizvod.Cena;
                    p.Kolicina = proizvod.Kolicina;
                    p.Opis = proizvod.Opis;
                    p.DatumPostavljanja = proizvod.DatumPostavljanja;
                    p.Grad = proizvod.Grad;
                    p.Dostupan = proizvod.Kolicina != 0;
                    p.Obrisan = false;

                    try
                    {
                        if (slika != null && slika.ContentLength > 0)
                        {
                            string naziv = Path.GetFileName(slika.FileName);
                            string putanjaSlika = Path.Combine(Server.MapPath("~/Images/"), naziv);
                            slika.SaveAs(putanjaSlika);
                            p.PutanjaDoSlike = naziv;
                        }
                    }
                    catch
                    {
                        p.PutanjaDoSlike = proizvod.PutanjaDoSlike;
                    }

                    foreach (Korisnik k in korisnici)
                    {
                        for (int i = 0; i < k.ListaObjavljenihProizvoda.Count; i++)
                        {
                            if (p.Id == k.ListaObjavljenihProizvoda[i].Id)
                            {
                                k.ListaObjavljenihProizvoda[i] = p;
                            }
                        }
                        for (int i = 0; i < k.ListaOmiljenihProizvoda.Count; i++)
                        {
                            if (p.Id == k.ListaOmiljenihProizvoda[i].Id)
                            {
                                k.ListaOmiljenihProizvoda[i] = p;
                            }
                        }
                        for (int i = 0; i < k.ListaPorudzbina.Count; i++)
                        {
                            if (p.Id == k.ListaPorudzbina[i].Proizvod.Id)
                            {
                                k.ListaPorudzbina[i].Proizvod = p;
                            }
                        }
                    }

                    foreach(Porudzbina pr in porudzbine)
                    {
                        if(pr.Proizvod.Id == p.Id || p.Id == Int32.Parse(pr.Id.Split(':')[1])){
                            pr.Proizvod = p;

                        }
                    }

                }
            }

            HttpContext.Application["proizvodi"] = proizvodi;
            HttpContext.Application["korisnici"] = korisnici;
            HttpContext.Application["porudzbine"] = porudzbine;

            Podaci.UpisiKorisnike(korisnici);
            Podaci.UpisiProizvode(proizvodi);
            Podaci.UpisiPorudzbine(porudzbine);

            return View("../Korisnik/Profil", prijavljeniKorisnik);
        }

        #endregion

        #region Brisanje
        public ActionResult Obrisi(string korisnickoIme, int id)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Porudzbina> porudzbine = (List<Porudzbina>)HttpContext.Application["porudzbine"];
            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];

            int idProizvodaZaBrisanje = id;

            foreach (Proizvod p in proizvodi)
            {
                if (p.Id == idProizvodaZaBrisanje)
                {
                    p.Obrisan = true;
                    break;
                }
            }
            foreach (Korisnik k in korisnici)
            {
                for (int i = 0; i < k.ListaObjavljenihProizvoda.Count; i++)
                {
                    if (k.ListaObjavljenihProizvoda[i].Id == idProizvodaZaBrisanje)
                    {
                        k.ListaObjavljenihProizvoda[i].Obrisan = true;
                        break;
                    }
                }
                for (int i = 0; i < k.ListaOmiljenihProizvoda.Count; i++)
                {
                    if (k.ListaOmiljenihProizvoda[i].Id == idProizvodaZaBrisanje)
                    {
                        k.ListaOmiljenihProizvoda[i].Obrisan = true;
                        break;
                    }
                }
                for (int i = 0; i < k.ListaPorudzbina.Count; i++)
                {
                    if (k.ListaPorudzbina[i].Proizvod.Id == idProizvodaZaBrisanje || (Int32.Parse(k.ListaPorudzbina[i].Id.Split(':')[1]) == idProizvodaZaBrisanje))
                    {
                        k.ListaPorudzbina[i].Obrisana = true;
                        break;
                    }
                }
            }
            foreach (Porudzbina p in porudzbine)
            {
                if (p.Proizvod.Id == idProizvodaZaBrisanje || (Int32.Parse(p.Id.Split(':')[1]) == idProizvodaZaBrisanje))
                {
                    p.Obrisana = true;
                    break;
                }
            }
            foreach (Recenzija r in recenzije)
            {
                if (r.Proizvod.Id == idProizvodaZaBrisanje || (Int32.Parse(r.Id.Split(':')[1]) == idProizvodaZaBrisanje))
                {
                    r.Obrisana = true;
                    break;
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

            return View("../Korisnik/Profil", prijavljeniKorisnik);
        }

        #endregion

        #region Liste Proizvoda
        public ActionResult ListaProizvoda(string korisnickoIme = "") //admin
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = korisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];

            if(proizvodi.Count == 0)
            {
                TempData["nemaProizvoda1"] = "Nema proizvoda za prikaz";
            }

            return View("Proizvodi", proizvodi);
        }

        public ActionResult ListaOmiljenihProizvoda(string korisnickoIme = "")
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> ret = new List<Proizvod>();
            Korisnik korisnik = new Korisnik();

            korisnik = korisnici.Find(x => x.KorisnickoIme == korisnickoIme);
            for (int i = 0; i < korisnik.ListaOmiljenihProizvoda.Count; i++)
            {
                ret.Add(korisnik.ListaOmiljenihProizvoda[i]);
            }

            if (ret.Count == 0)
            {
                TempData["praznaLista"] = "Nema omiljenih proizvoda za prikaz.";
            }

            return View("Proizvodi", ret);
        }

        public ActionResult ListaObjavljenihProizvoda(string korisnickoIme = "")
        {

            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> ret = new List<Proizvod>();
            Korisnik korisnik = korisnici.Find(x => x.KorisnickoIme == korisnickoIme);
            for (int i = 0; i < korisnik.ListaObjavljenihProizvoda.Count; i++)
            {
                ret.Add(korisnik.ListaObjavljenihProizvoda[i]);

            }

            if (ret.Count == 0)
            {
                TempData["praznaLista"] = "Nema objavljenih proizvoda za prikaz";
            }

            return View("Proizvodi", ret);
        }
        #endregion

        #region Filter i sort liste proizvoda
        public ActionResult PretragaPoStatusu()
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];

            List<Proizvod> ret = new List<Proizvod>();

            var sortiranje = Request["pretragaPoStatusu"] != null ? Request["pretragaPoStatusu"] : string.Empty;

            if (prijavljeniKorisnik.Uloga == Uloga.Prodavac)
            {
                ret = korisnici.Find(x => x.KorisnickoIme == prijavljeniKorisnik.KorisnickoIme).ListaObjavljenihProizvoda;
            }
            else //admin
            {
                ret = proizvodi;
            }

            if (sortiranje == "dostupni")
            {
                ret = ret.Where(x => x.Dostupan == true).ToList();
            }
            else if (sortiranje == "nedostupni")
            {

                ret = ret.Where(x => x.Dostupan == false).ToList();
            }

            if (ret.Count == 0)
            {
                TempData["nemaproizvoda"] = "Nema proizvoda za prikaz.";
            }

            return View("Proizvodi", ret);
        }

        public ActionResult SortirajPoNazivu()
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Proizvod> ret = new List<Proizvod>();

            var sortiranje = Request["sortiranjePoNazivu"] != null ? Request["sortiranjePoNazivu"] : string.Empty;

            if (prijavljeniKorisnik.Uloga == Uloga.Prodavac)
            {
                ret = korisnici.Find(x => x.KorisnickoIme == prijavljeniKorisnik.KorisnickoIme).ListaObjavljenihProizvoda;
            }
            else //admin
            {
                ret = proizvodi;
            }

            if (sortiranje == "opadajuce")
            {
                ret = ret.OrderByDescending(x => x.Naziv).ToList();
            }
            else if (sortiranje == "rastuce")
            {

                ret = ret.OrderBy(x => x.Naziv).ToList();
            }

            if (ret.Count == 0)
            {
                TempData["nemaproizvoda"] = "Nema proizvoda za prikaz.";
            }
            return View("Proizvodi", ret);

        }
        public ActionResult SortirajPoDatumu()
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Proizvod> ret = new List<Proizvod>();

            var sortiranje = Request["sortiranjePoDatumu"] != null ? Request["sortiranjePoDatumu"] : string.Empty;

            if (prijavljeniKorisnik.Uloga == Uloga.Prodavac)
            {
                ret = korisnici.Find(x => x.KorisnickoIme == prijavljeniKorisnik.KorisnickoIme).ListaObjavljenihProizvoda;
            }
            else //admin
            {
                ret = proizvodi;
            }

            if (sortiranje == "opadajuce")
            {
                ret = ret.OrderByDescending(x => x.DatumPostavljanja).ToList();
            }
            else if (sortiranje == "rastuce")
            {

                ret = ret.OrderBy(x => x.DatumPostavljanja).ToList();
            }
            if (ret.Count == 0)
            {
                TempData["nemaproizvoda"] = "Nema proizvoda za prikaz.";
            }

            if (ret.Count == 0)
            {
                TempData["nemaproizvoda"] = "Nema proizvoda za prikaz.";
            }

            return View("Proizvodi", ret);

        }
        public ActionResult SortirajPoCeni()
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Proizvod> ret = new List<Proizvod>();

            var sortiranje = Request["sortiranjePoCeni"] != null ? Request["sortiranjePoCeni"] : string.Empty;

            if (prijavljeniKorisnik.Uloga == Uloga.Prodavac)
            {
                ret = korisnici.Find(x => x.KorisnickoIme == prijavljeniKorisnik.KorisnickoIme).ListaObjavljenihProizvoda;
            }
            else //admin
            {
                ret = proizvodi;
            }

            if (sortiranje == "opadajuce")
            {
                ret = ret.OrderByDescending(x => x.Cena).ToList();
            }
            else if (sortiranje == "rastuce")
            {

                ret = ret.OrderBy(x => x.Cena).ToList();
            }

            if (ret.Count == 0)
            {
                TempData["nemaproizvoda"] = "Nema proizvoda za prikaz.";
            }

            return View("Proizvodi", ret);
        }

        #endregion

    }
}