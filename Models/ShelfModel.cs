using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyLibrary.Models
{
    public class ShelfModel
    {
        [Key]
        public Guid ShelfId { get; set; }
        public string Description { get; set; }
        public Guid BookshelfId { get; set; }

    }
}