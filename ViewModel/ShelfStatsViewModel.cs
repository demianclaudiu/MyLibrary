using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyLibrary.ViewModel
{
    public class ShelfStatsViewModel
    {
        [Key]
        public Guid ShelfId { get; set; }
        public string Description { get; set; }
        public Guid BookshelfId { get; set; }
        public int NumberOfBooks { get; set; }

    }
}