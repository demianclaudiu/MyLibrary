using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyLibrary.ViewModel
{
    public class BookshelfStatsViewModel
    {
        [Key]
        public Guid BookshelfId { get; set; }
        public Guid LibraryId { get; set; }
        public string Description { get; set; }
        public int NumberOfShelves { get; set; }
        public int NumberOfBooks { get; set; }

    }
}