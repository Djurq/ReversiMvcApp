using System.ComponentModel.DataAnnotations;

namespace ReversiMvcApp.Models
{
    public class Speler
    {
        [Key]
        public int GUID { get; set; }
        public int Naam { get; set; }
        public int AantalGewonnen { get; set; }
        public int AantalVerloren { get; set; }
        public int AantalGelijk { get; set; }

    }
}
