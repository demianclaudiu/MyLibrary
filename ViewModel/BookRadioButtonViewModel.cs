using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.ViewModel
{
    public class BookRadioButtonViewModel
    {
        public Guid BookId { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string CoverImageLocation { get; set; }
        public string Author { get; set; }
        public int? YearPublished { get; set; }
        public bool isSelected { get; set; }
    }
}