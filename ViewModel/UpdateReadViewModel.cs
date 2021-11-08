using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyLibrary.ViewModel
{
    public class UpdateReadViewModel
    {
        [Key]
        public Guid OwnershipId { get; set; }
        public Guid BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsRead { get; set; }
        public int? BookmarkedPage { get; set; }
    }
}