using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyLibrary.Models
{
    public class BookshelfModel
    {
        [Key]
        public Guid BookshelfId { get; set; }
        public string Description { get; set; }
        public Guid LibraryId { get; set; }

    }
}