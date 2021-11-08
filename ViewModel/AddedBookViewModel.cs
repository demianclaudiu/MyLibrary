using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyLibrary.ViewModel
{
    public class AddedBookViewModel
    {
        [Key]
        public Guid OwnershipId { get; set; }
        public Guid BookId { get; set; }
        public string CoverImageLocation { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string IsRead { get; set; }
        public int BookMark { get; set; }
        public string LibraryDescription { get; set; }
        public string BookshelfDescription { get; set; }
        public Guid ShelfId { get; set; }
        public string ShelfDescription { get; set; }
    }
}