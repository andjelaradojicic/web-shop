using Projekat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Projekat.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = korisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Proizvod> ret = proizvodi.Where(x => x.Dostupan).ToList();

            if(ret.Count == 0)
            {
                TempData["praznaListaDostupnihProizvoda"] = "Nema dostupnih proizvoda za prikaz.";
            }

            return View(ret);
        }

        #region Pretraga
        public ActionResult Pretraga(string nazivPretraga, string gradPretraga, int minCenaPretraga = 0, int maxCenaPretraga = 0)
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = korisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Proizvod> ret = new List<Proizvod>();

            ret = proizvodi.Where(x => x.Dostupan).ToList();

            if (nazivPretraga != "")
            {
                ret = ret.Where(x => x.Naziv == nazivPretraga).ToList();
            }
            if (minCenaPretraga != 0)
            {
                ret = ret.Where(x => x.Cena >= minCenaPretraga).ToList();
            }
            if (maxCenaPretraga != 0)
            {
                ret = ret.Where(x => x.Cena <= maxCenaPretraga).ToList();
            }
            if (gradPretraga != "")
            {
                ret = ret.Where(x => x.Grad == gradPretraga).ToList();
            }

            if (ret.Count == 0)
            {
                TempData["praznaListaDostupnihProizvoda"] = "Nema dostupnih proizvoda za prikaz.";
            }


            return View("Index", ret);
        }
        #endregion

        #region Sortiranje
        public ActionResult SortirajPoCeni()
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = korisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Proizvod> ret = new List<Proizvod>();

            var sortiranje = Request["sortiranjePoCeni"] != null ? Request["sortiranjePoCeni"] : string.Empty;

            ret = proizvodi.Where(x => x.Dostupan).ToList();

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
                TempData["praznaListaDostupnihProizvoda"] = "Nema dostupnih proizvoda za prikaz.";
            }


            return View("Index", ret);
        }
        public ActionResult SortirajPoNazivu()
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = korisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Proizvod> ret = new List<Proizvod>();

            var sortiranje = Request["sortiranjePoNazivu"] != null ? Request["sortiranjePoNazivu"] : string.Empty;

            ret = proizvodi.Where(x => x.Dostupan).ToList();

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
                TempData["praznaListaDostupnihProizvoda"] = "Nema dostupnih proizvoda za prikaz.";
            }


            return View("Index", ret);

        }
        public ActionResult SortirajPoDatumu()
        {
            Korisnik korisnik = (Korisnik)Session["korisnik"];
            ViewBag.Korisnik = korisnik;

            List<Proizvod> proizvodi = (List<Proizvod>)HttpContext.Application["proizvodi"];
            List<Proizvod> ret = new List<Proizvod>();

            var sortiranje = Request["sortiranjePoDatumu"] != null ? Request["sortiranjePoDatumu"] : string.Empty;

            ret = proizvodi.Where(x => x.Dostupan).ToList();

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
                TempData["praznaListaDostupnihProizvoda"] = "Nema dostupnih proizvoda za prikaz.";
            }

            return View("Index", ret);
        }
        #endregion
    }
}