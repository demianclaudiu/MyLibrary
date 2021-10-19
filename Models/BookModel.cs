using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.Models
{
    public class BookModel
    {
        public Guid BookId { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public Guid CoverImageId { get; set; }
        public int? YearPublished { get; set; }
        public string Publisher { get; set; }
        public string Genre { get; set; }
        public string CoverImageLocation { get; set; }
        public bool? IsCoverImageLocal { get; set; }

    }
}