using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Projekat.Models
{
    [Serializable]
    public enum Uloga { Kupac, Prodavac, Administrator}
    [Serializable]
    public enum Pol { Muski, Zenski}
    [Serializable]
    public class Korisnik
    {
        public string KorisnickoIme { get; set; }
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public Pol Pol { get; set; }
        public string Email { get; set; }
        [XmlElement(DataType ="date")]
        public DateTime DatumRodjenja { get; set; }
        public Uloga Uloga { get; set; }
        public List<Porudzbina> ListaPorudzbina { get; set; }
        public List<Proizvod> ListaOmiljenihProizvoda { get; set; }
        public List<Proizvod> ListaObjavljenihProizvoda { get; set; }
        public bool Obrisan { get; set; }

        public Korisnik()
        {
            KorisnickoIme = "";
            Lozinka = "";
            Ime = "";
            Prezime = "";
            Pol = Pol.Muski;
            Email = "";
            DatumRodjenja = DateTime.Now.Date;
            Uloga = Uloga.Kupac;
            ListaPorudzbina = new List<Porudzbina>();
            ListaOmiljenihProizvoda = new List<Proizvod>();
            ListaObjavljenihProizvoda = new List<Proizvod>();
            Obrisan = false;
        }

        public Korisnik(string korisnickoIme, string lozinka, string ime, string prezime, Pol pol, string email, DateTime datumRodjenja, Uloga uloga)
        {
            KorisnickoIme = korisnickoIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Email = email;
            DatumRodjenja = datumRodjenja.Date;
            Uloga = uloga;
            ListaPorudzbina = new List<Porudzbina>();
            ListaOmiljenihProizvoda = new List<Proizvod>();
            ListaObjavljenihProizvoda = new List<Proizvod>();
            Obrisan = false;
        }
    }
}