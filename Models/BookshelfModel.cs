using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.Models
{
    public class BookshelfModel
    {
        public Guid BookshelfId { get; set; }
        public string Description { get; set; }
        public Guid LibraryId { get; set; }

    }
}