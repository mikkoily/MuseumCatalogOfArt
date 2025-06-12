using System;
using Newtonsoft.Json;

namespace OOP_Project.Models { public abstract class Artwork {
        public string Title { get; set; }
        public int Year { get; set; }
        public string Medium { get; set; }
        public string Weight { get; set; }
        public string Location { get; set; }

        [JsonIgnore]
        public Artist Artist { get; set; }
        public string ArtistName { get; set; } // For serialization

        protected Artwork(string title, int year, string medium, string weight, string location, Artist artist) {
            Title = title;
            Year = year;
            Medium = medium;
            Weight = weight;
            Location = location;
            Artist = artist;
            ArtistName = artist?.Name;
        }
    }

    public class Painting : Artwork {
        public string FrameType { get; set; }

        public Painting(string title, int year, string medium, string weight, string location, Artist artist, string frameType)
            : base(title, year, medium, weight, location, artist) {
            FrameType = frameType;
        }
    }

    public class Sculpture : Artwork {
        public string Material { get; set; }

        public Sculpture(string title, int year, string medium, string weight, string location, Artist artist, string material)
            : base(title, year, medium, weight, location, artist) {
            Material = material;
        }
    }

}