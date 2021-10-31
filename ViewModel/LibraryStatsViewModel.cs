using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyLibrary.ViewModel
{
    public class LibraryStatsViewModel
    {
        [Key]
        public Guid LibraryId { get; set; }
        public string Description { get; set; }
        public int NumberOfBookshelves { get; set; }
        public int NumberOfBooks { get; set; }
    }
}