using System;
using System.Xml.Serialization;

namespace Projekat.Models
{
    [Serializable]
    public class Recenzija
    {
        public string Id { get; set; }
        [XmlIgnore]
        public Proizvod Proizvod { get; set; }
        [XmlIgnore]
        public Korisnik Recezent { get; set; }
        public string Naslov { get; set; }
        public string Sadrzaj { get; set; }
        public string PutanjaDoSlike { get; set; }
        public bool Odobrena { get; set; }
        public bool Obrisana { get; set; }

        public Recenzija()
        {
            Id = "";
            Naslov = "";
            Sadrzaj = "";
            PutanjaDoSlike = "";
            Odobrena = false;
            Obrisana = false;
        }
        public Recenzija(Proizvod proizvod, Korisnik recezent, string naslov, string sadrzaj, string putanjaDoSlike, bool odobrena)
        {
            Id = $"{recezent.KorisnickoIme}:{proizvod.Id}";
            Proizvod = (proizvod == null) ? new Proizvod() : proizvod;
            Recezent = (recezent == null) ? new Korisnik() : recezent;
            Naslov = naslov;
            Sadrzaj = sadrzaj;
            PutanjaDoSlike = putanjaDoSlike;
            Odobrena = odobrena;
            Obrisana = false;
        }
    }
}