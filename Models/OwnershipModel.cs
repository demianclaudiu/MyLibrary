using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.Models
{
    public class OwnershipModel
    {
        public Guid OwnershipId { get; set; }
        public Guid BookId { get; set; }
        public Guid ShelfId { get; set; }
        public bool IsRead { get; set; }
        public int? BookmarkedPage { get; set; }

    }
}