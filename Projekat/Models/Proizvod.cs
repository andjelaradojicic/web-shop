using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Projekat.Models
{
    [Serializable]
    public class Proizvod
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public double Cena { get; set; }
        public int Kolicina { get; set; }
        public string Opis { get; set; }
        public string PutanjaDoSlike { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime DatumPostavljanja { get; set; }
        public string Grad { get; set; }
        public List<Recenzija> ListaRecenzija { get; set; }
        public bool Dostupan { get; set; }
        public bool Obrisan { get; set; }

        public Proizvod()
        {
            Id = -1;
            Naziv = "";
            Cena = 0;
            Kolicina = 0;
            Opis = "";
            PutanjaDoSlike = "";
            DatumPostavljanja = DateTime.Now.Date;
            Grad = "";
            ListaRecenzija = new List<Recenzija>();
            Dostupan = false;
            Obrisan = false;
        }

        public Proizvod(int id, string naziv, double cena, int kolicina, string opis, string putanjaDoSlike, DateTime datumPostavljanja, string grad, bool dostupan)
        {
            Id = id;
            Naziv = naziv;
            Cena = cena;
            Kolicina = kolicina;
            Opis = opis;
            PutanjaDoSlike = putanjaDoSlike;
            DatumPostavljanja = datumPostavljanja.Date;
            ListaRecenzija = new List<Recenzija>();
            Grad = grad;
            Dostupan = dostupan;
            Obrisan = false;
        }
    }
}