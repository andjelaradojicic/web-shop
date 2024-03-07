using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat.Models
{
    public class Podaci
    {
        public static void Osvezi()
        {
            List<Korisnik> korisnici = ProcitajKorisnike();
            List<Proizvod> proizvodi = ProcitajProizvode();
            List<Porudzbina> porudzbine = ProcitajPorudzbine();
            List<Recenzija> recenzije = ProcitajRecenzije();

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
                for (int i = 0; i < p.ListaRecenzija.Count; i++)
                {
                    string[] idRecenzije = p.ListaRecenzija[i].Id.Split(':');
                    string idKorisnika = idRecenzije[0];
                    int idProizvoda = Int32.Parse(idRecenzije[1]);

                    p.ListaRecenzija[i].Proizvod = proizvodi.Find(x => x.Id == idProizvoda);
                    p.ListaRecenzija[i].Recezent = korisnici.Find(x => x.KorisnickoIme == idKorisnika);
                }
            }
            foreach (Korisnik k in korisnici)
            {
                for (int i = 0; i < k.ListaPorudzbina.Count; i++)
                {
                    string[] idPorudzbine = k.ListaPorudzbina[i].Id.Split(':');
                    string idKorisnika = idPorudzbine[0];
                    int idProizvoda = Int32.Parse(idPorudzbine[1]);

                    k.ListaPorudzbina[i].Kupac = korisnici.Find(x => x.KorisnickoIme == idKorisnika);
                    k.ListaPorudzbina[i].Proizvod = proizvodi.Find(x => x.Id == idProizvoda);
                }
            }
            foreach(Proizvod p in proizvodi)
            {
                for (int i = 0; i < p.ListaRecenzija.Count; i++)
                {
                    string[] idRecenzije = p.ListaRecenzija[i].Id.Split(':');
                    string idKorisnika = idRecenzije[0];
                    int idProizvoda = Int32.Parse(idRecenzije[1]);

                    p.ListaRecenzija[i].Recezent = korisnici.Find(x => x.KorisnickoIme == idKorisnika);
                    p.ListaRecenzija[i].Proizvod = proizvodi.Find(x => x.Id == idProizvoda);
                }
            }

            HttpContext.Current.Application["korisnici"] = korisnici;
            HttpContext.Current.Application["proizvodi"] = proizvodi;
            HttpContext.Current.Application["porudzbine"] = porudzbine;
            HttpContext.Current.Application["recenzije"] = recenzije;

        }
        #region Serijalizuj
        public static void UpisiKorisnika(Korisnik korisnik)
        {
            List<Korisnik> korisnici = ProcitajKorisnike();
            korisnici.Add(korisnik);
            UpisiKorisnike(korisnici);
        }

        public static void UpisiProizvod(Proizvod proizvod)
        {
            List<Proizvod> proizvodi = ProcitajProizvode();
            proizvodi.Add(proizvod);
            UpisiProizvode(proizvodi);
        }
        public static void UpisiPorudzbinu(Porudzbina porudzbina)
        {
            List<Porudzbina> porudzbine = ProcitajPorudzbine();
            porudzbine.Add(porudzbina);
            UpisiPorudzbine(porudzbine);
        }
        public static void UpisiRecenziju(Recenzija recenzija)
        {
            List<Recenzija> recenzije = ProcitajRecenzije();
            recenzije.Add(recenzija);
            UpisiRecenzije(recenzije);
        }

        public static void UpisiKorisnike(List<Korisnik> korisnici)
        {
            Serijalizacija s = new Serijalizacija();
            string putanjaKorisnici = "~/App_Data/korisnici.xml";
            s.Serijalizuj<List<Korisnik>>(korisnici, putanjaKorisnici);
        }
        public static void UpisiProizvode(List<Proizvod> proizvodi)
        {
            Serijalizacija s = new Serijalizacija();
            string putanjaProizvodi = "~/App_Data/proizvodi.xml";
            s.Serijalizuj<List<Proizvod>>(proizvodi, putanjaProizvodi);
        }
        public static void UpisiPorudzbine(List<Porudzbina> porudzbine)
        {
            Serijalizacija s = new Serijalizacija();
            string putanjaPorudzbine = "~/App_Data/porudzbine.xml";
            s.Serijalizuj<List<Porudzbina>>(porudzbine, putanjaPorudzbine);
        }
        public static void UpisiRecenzije(List<Recenzija> recenzije)
        {
            Serijalizacija s = new Serijalizacija();
            string putanjaRecenzije = "~/App_Data/recenzije.xml";
            s.Serijalizuj<List<Recenzija>>(recenzije, putanjaRecenzije);
        }
        #endregion

        #region Deserijalizuj
        public static List<Korisnik> ProcitajKorisnike()
        {
            Serijalizacija s = new Serijalizacija();
            string putanjaKorisnici = "~/App_Data/korisnici.xml";
            List<Korisnik> korisnici = s.Deserijalizuj<List<Korisnik>>(putanjaKorisnici);

            List<Korisnik> ret = new List<Korisnik>();
            ret = korisnici.Where(x => !x.Obrisan).ToList();

            foreach (Korisnik korisnik in ret)
            {
                for (int i = 0; i < korisnik.ListaOmiljenihProizvoda.Count; i++)
                {
                    if (korisnik.ListaOmiljenihProizvoda[i].Obrisan)
                    {
                        korisnik.ListaOmiljenihProizvoda.Remove(korisnik.ListaOmiljenihProizvoda[i]);
                    }
                }
                for (int i = 0; i < korisnik.ListaPorudzbina.Count; i++)
                {
                    string[] idPorudzbine = korisnik.ListaPorudzbina[i].Id.Split(':');
                    string idKorisnika = idPorudzbine[0];
                    int idProizvoda = Int32.Parse(idPorudzbine[1]);

                    if (korisnik.ListaPorudzbina[i].Obrisana)
                    {
                        korisnik.ListaPorudzbina.Remove(korisnik.ListaPorudzbina[i]);
                    }
                }
                for (int i = 0; i < korisnik.ListaObjavljenihProizvoda.Count; i++)
                {
                    if (korisnik.ListaObjavljenihProizvoda[i].Obrisan)
                    {
                        korisnik.ListaObjavljenihProizvoda.Remove(korisnik.ListaObjavljenihProizvoda[i]);
                    }
                }
            }



            return ret;
        }
        public static List<Proizvod> ProcitajProizvode()
        {
            Serijalizacija s = new Serijalizacija();
            string putanjaProizvodi = "~/App_Data/proizvodi.xml";
            List<Proizvod> proizvodi = s.Deserijalizuj<List<Proizvod>>(putanjaProizvodi);

            List<Proizvod> ret = proizvodi.Where(x => !x.Obrisan).ToList();

            for (int i = 0; i < ret.Count; i++)
            {
                for (int j = 0; j < ret[i].ListaRecenzija.Count; j++)
                {
                    if (ret[i].ListaRecenzija[j].Obrisana)
                    {
                        ret[i].ListaRecenzija.Remove(ret[i].ListaRecenzija[j]);
                    }
                }
            }

            return ret;
        }
        public static List<Porudzbina> ProcitajPorudzbine()
        {
            Serijalizacija s = new Serijalizacija();
            string putanjaPorudzbine = "~/App_Data/porudzbine.xml";
            List<Porudzbina> porudzbine = s.Deserijalizuj<List<Porudzbina>>(putanjaPorudzbine);

            List<Porudzbina> ret = porudzbine.Where(x => !x.Obrisana).ToList();

            return ret;
        }
        public static List<Recenzija> ProcitajRecenzije()
        {
            Serijalizacija s = new Serijalizacija();
            string putanjaRecenzije = "~/App_Data/recenzije.xml";
            List<Recenzija> recenzije = s.Deserijalizuj<List<Recenzija>>(putanjaRecenzije);

            List<Recenzija> ret = recenzije.Where(x => !x.Obrisana).ToList();

            return ret;
        }
        #endregion
    }
}