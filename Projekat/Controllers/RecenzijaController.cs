using Projekat.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Projekat.Controllers
{
    public class RecenzijaController : Controller
    {
        // GET: Recenzija
        public ActionResult Index()
        {
            return View();
        }

        #region Kupac
        public ActionResult Recenzija(string korisnickoIme, int id) //forma
        {
            //vraca porudzbinu u index
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];
            List<Porudzbina> porudzbine = (List<Porudzbina>)HttpContext.Application["porudzbine"];
            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            Porudzbina porudzbina = new Porudzbina();
            foreach (Korisnik k in korisnici)
            {
                for (int i = 0; i < k.ListaPorudzbina.Count; i++)
                {
                    string[] idPorudzbine = k.ListaPorudzbina[i].Id.Split(':');
                    string idKorisnika = idPorudzbine[0];
                    int idProizvoda = Int32.Parse(idPorudzbine[1]);

                    if(idKorisnika == korisnickoIme && idProizvoda == id)
                    {
                        porudzbina.Proizvod = proizvodi.Find(x => x.Id == idProizvoda);
                        porudzbina.Kupac = k;
                    }
                }
            }

            foreach (Recenzija r in recenzije)
            {
                if (r.Id == $"{prijavljeniKorisnik.KorisnickoIme}:{id}")
                {
                    TempData["recenzijaPostoji"] = "Recenzija za ovaj proizod je vec poslata";
                    return View("../Porudzbina/Porudzbine", prijavljeniKorisnik.ListaPorudzbina);


                }
            }
            
            HttpContext.Application["porudzbine"] = porudzbine;

            Podaci.UpisiPorudzbine(porudzbine);

            return View("Index", porudzbina);
        }
        [HttpPost]
        public ActionResult DodajRecenziju(Recenzija recenzija, HttpPostedFileBase slika, string korisnickoIme, int id) //view sa formom
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];

            Recenzija recenzija1 = new Recenzija();

            foreach (Proizvod p in proizvodi)
            {
                if(p.Id == id)
                {
                    foreach(Korisnik k in korisnici)
                    {
                        if(k.KorisnickoIme == korisnickoIme || k.KorisnickoIme == prijavljeniKorisnik.KorisnickoIme)
                        {
                            recenzija1.Id = $"{prijavljeniKorisnik.KorisnickoIme}:{id}";
                            recenzija1.Proizvod = p;
                            recenzija1.Recezent = k;
                            recenzija1.Naslov = recenzija.Naslov;
                            recenzija1.Sadrzaj = recenzija.Sadrzaj;
                            recenzija.Odobrena = false;

                            try
                            {
                                if (slika != null && slika.ContentLength > 0)
                                {
                                    string naziv = Path.GetFileName(slika.FileName);
                                    string putanjaSlika = Path.Combine(Server.MapPath("~/Images/"), naziv);
                                    slika.SaveAs(putanjaSlika);
                                    recenzija1.PutanjaDoSlike = naziv;
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }

            recenzije.Add(recenzija1);
            foreach(Proizvod p in proizvodi)
            {
                if(p.Id == id)
                {
                    p.ListaRecenzija.Add(recenzija1);
                }
            }

            
            HttpContext.Application["korisnici"] = korisnici;
            HttpContext.Application["proizvodi"] = proizvodi;
            HttpContext.Application["recenzije"] = recenzije;

            Podaci.UpisiKorisnike(korisnici);
            Podaci.UpisiProizvode(proizvodi);
            Podaci.UpisiRecenzije(recenzije);

            return View("../Porudzbina/Porudzbine", prijavljeniKorisnik.ListaPorudzbina);
        }
        
        #endregion

        #region Recezent
        public ActionResult Izmena(string korisnickoIme, int id)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];

            Recenzija recenzija = new Recenzija();

            recenzija = recenzije.Find(x => (x.Proizvod.Id == id && x.Recezent.KorisnickoIme == korisnickoIme) || x.Id == $"{korisnickoIme}:{id}");
            recenzija.Recezent = korisnici.Find(x => x.KorisnickoIme == korisnickoIme);
            recenzija.Proizvod = proizvodi.Find(x => x.Id == id);

            HttpContext.Application["recenzije"] = recenzije;

            return View("IzmenaRecenzije", recenzija);
        }
        [HttpPost]
        public ActionResult IzmenaRecenzije(Recenzija recenzija, HttpPostedFileBase slika, string korisnickoIme, int id)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;
            
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];

            Recenzija izmenjenaRecenzija = recenzije.Find(x => (x.Proizvod.Id == id && x.Recezent.KorisnickoIme == korisnickoIme) || x.Id == $"{korisnickoIme}:{id}");

            izmenjenaRecenzija.Naslov = recenzija.Naslov;
            izmenjenaRecenzija.Sadrzaj = recenzija.Sadrzaj;
            izmenjenaRecenzija.Odobrena = recenzija.Odobrena;

            try
            {
                if (slika != null && slika.ContentLength > 0)
                {
                    string naziv = Path.GetFileName(slika.FileName);
                    string putanjaSlika = Path.Combine(Server.MapPath("~/Images/"), naziv);
                    slika.SaveAs(putanjaSlika);
                    izmenjenaRecenzija.PutanjaDoSlike = naziv;
                }
            }

            catch
            {
                izmenjenaRecenzija.PutanjaDoSlike = recenzije.Find(x => (x.Proizvod.Id == id && x.Recezent.KorisnickoIme == korisnickoIme) || x.Id == $"{korisnickoIme}:{id}").PutanjaDoSlike;
            }

            foreach (Proizvod p in proizvodi)
            {
                for (int i = 0; i < p.ListaRecenzija.Count; i++)
                {
                    if (p.ListaRecenzija[i].Proizvod.Id == id && p.ListaRecenzija[i].Recezent.KorisnickoIme == korisnickoIme)
                    {
                        p.ListaRecenzija[i] = izmenjenaRecenzija;
                    }
                }
            }

            for(int i = 0; i < recenzije.Count; i++)
            {
                if(recenzije[i].Id == izmenjenaRecenzija.Id)
                {
                    recenzije[i] = izmenjenaRecenzija;
                }
            }


            HttpContext.Application["proizvodi"] = proizvodi;
            HttpContext.Application["recenzije"] = recenzije;

            Podaci.UpisiProizvode(proizvodi);
            Podaci.UpisiRecenzije(recenzije);

            return View("../Korisnik/Profil", prijavljeniKorisnik);
        }
        public ActionResult Obrisi(string korisnickoIme, int id)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];

            foreach (Recenzija r in recenzije)
            {
                if (r.Id == $"{korisnickoIme}:{id}")
                {
                    r.Obrisana = true;
                }
            }

            foreach (Proizvod p in proizvodi)
            {
                for (int i = 0; i < p.ListaRecenzija.Count; i++)
                {
                    if (p.ListaRecenzija[i].Id == $"{korisnickoIme}:{id}")
                    {
                        p.ListaRecenzija[i].Obrisana = true;
                    }
                }
            }


            HttpContext.Application["proizvodi"] = proizvodi;
            HttpContext.Application["recenzije"] = recenzije;

            Podaci.UpisiProizvode(proizvodi);
            Podaci.UpisiRecenzije(recenzije);

            return View("../Korisnik/Profil", prijavljeniKorisnik);
        }

        #endregion

        #region Admin
        public ActionResult OdobriRecenziju(string korisnickoIme, int id)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];

            foreach(Recenzija r in recenzije)
            {
                if(r.Id == $"{korisnickoIme}:{id}")
                {
                    r.Odobrena = true;
                }
            }

            foreach(Proizvod proizvod in proizvodi)
            {
                for(int i = 0; i< proizvod.ListaRecenzija.Count; i++)
                {
                    if(proizvod.ListaRecenzija[i].Id == $"{korisnickoIme}:{id}")
                    {
                        proizvod.ListaRecenzija[i].Odobrena = true;
                    }
                }
            }

            HttpContext.Application["korisnici"] = korisnici;
            HttpContext.Application["proizvodi"] = proizvodi;
            HttpContext.Application["recenzije"] = recenzije;

            Podaci.UpisiKorisnike(korisnici);
            Podaci.UpisiProizvode(proizvodi);
            Podaci.UpisiRecenzije(recenzije);


            return View("../Korisnik/Profil", prijavljeniKorisnik);

        }
        public ActionResult OdbijRecenziju(string korisnickoIme, int id)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];

            Recenzija r = recenzije.Find(x => x.Proizvod.Id == id && x.Recezent.KorisnickoIme == korisnickoIme);
            r.Odobrena = false;

            foreach (Proizvod p in proizvodi)
            {
                for (int i = 0; i < p.ListaRecenzija.Count; i++)
                {
                    if (p.ListaRecenzija[i].Proizvod.Id == id && p.ListaRecenzija[i].Recezent.KorisnickoIme == korisnickoIme)
                    {
                        p.ListaRecenzija[i].Odobrena = false;
                    }
                }
            }

            HttpContext.Application["proizvodi"] = proizvodi;
            HttpContext.Application["recenzije"] = recenzije;

            Podaci.UpisiProizvode(proizvodi);
            Podaci.UpisiRecenzije(recenzije);

            return View("../Korisnik/Profil", prijavljeniKorisnik);
        }
        #endregion

        public ActionResult ListaRecenzijaZaProizvod(string korisnickoIme, int id)
        {
            Korisnik prijavljeniKorisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = prijavljeniKorisnik;

            List<Korisnik> korisnici = (List<Korisnik>)HttpContext.Application["korisnici"];
            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Recenzija> recenzije = (List<Recenzija>)HttpContext.Application["recenzije"];

            List<Recenzija> ret = new List<Recenzija>();

            foreach (Recenzija r in recenzije)
            {
                string[] idRecenzije = r.Id.Split(':');
                string idKorisnika = idRecenzije[0];
                int idProizvoda = Int32.Parse(idRecenzije[1]);

                r.Proizvod = proizvodi.Find(x => x.Id == idProizvoda);
                r.Recezent = korisnici.Find(x => x.KorisnickoIme == idKorisnika);
            }

            foreach (Proizvod p in proizvodi)
            {
                if (p.Id == id)
                {
                    ret = p.ListaRecenzija;
                }
            }

            HttpContext.Application["recenzije"] = recenzije;

            return View("Recenzije", ret);
        }
    }
}