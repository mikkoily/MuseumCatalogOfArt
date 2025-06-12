using System;
using System.Collections.Generic;

namespace OOP_Project.Models {
    public class Exhibition {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Artwork> Artworks { get; set; } = new List<Artwork>();

        public Exhibition(string title, DateTime startDate, DateTime endDate) {
            Title = title;
            StartDate = startDate;
            EndDate = endDate;
        }
    }

}