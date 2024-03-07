using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Projekat.Models
{
    [Serializable]
    public enum StatusPorudzbine { Aktivna, Izvrsena, Otkazana}
    [Serializable]
    public class Porudzbina
    {
        public string Id { get; set; }
        public Proizvod Proizvod { get; set; }
        public int Kolicina { get; set; }
        [XmlIgnore]
        public Korisnik Kupac { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime DatumPorudzbine { get; set; }
        public StatusPorudzbine StatusPorudzbine { get; set; }
        public bool Obrisana { get; set; }
        public Porudzbina()
        {
            Id = "";
            Proizvod = new Proizvod();
            Kolicina = 0;
            Kupac = new Korisnik();
            DatumPorudzbine = DateTime.Now.Date;
            StatusPorudzbine = StatusPorudzbine.Aktivna;
            Obrisana = false;
        }
        public Porudzbina(Proizvod proizvod, int kolicina, Korisnik kupac, DateTime datumPorudzbine, StatusPorudzbine statusPorudzbine)
        {
            Id = $"{kupac.KorisnickoIme}:{proizvod.Id}";
            Proizvod = (proizvod == null) ? new Proizvod() : proizvod;
            Kolicina = kolicina;
            Kupac = (kupac == null) ? new Korisnik() : kupac;
            DatumPorudzbine = datumPorudzbine.Date;
            StatusPorudzbine = statusPorudzbine;
            Obrisana = false;
        }
    }
}