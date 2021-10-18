using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.Models
{
    public class ShelfModel
    {
        public Guid ShelfId { get; set; }
        public string Description { get; set; }
        public Guid BookshelfId { get; set; }

    }
}