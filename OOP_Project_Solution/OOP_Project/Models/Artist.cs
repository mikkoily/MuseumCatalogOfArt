using System.Collections.Generic;

namespace OOP_Project.Models {
    public class Artist {
        public string Name { get; set; }
        public int BirthYear { get; set; }
        public string Nationality { get; set; }
        public int? DeathYear { get; set; }
        public List<Artwork> Artworks { get; set; } = new List<Artwork>();

        public Artist(string name, int birthYear, string nationality, int? deathYear) {
            Name = name;
            BirthYear = birthYear;
            Nationality = nationality;
            DeathYear = deathYear;
        }
    }

}