using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyLibrary.ViewModel
{
    public class MyBookMoveViewModel
    {
        [Key]
        public Guid BookId { get; set; }
        public string Title { get; set; }

        [Display(Name = "Library")]
        public Guid SelectedLibrary { get; set; }
        public IEnumerable<SelectListItem> Libraries { get; set; }

        [Display(Name = "Bookshelf")]
        public Guid SelectedBookshelf { get; set; }
        public IEnumerable<SelectListItem> Bookshelves { get; set; }

        [Display(Name = "Shelf")]
        public Guid SelectedShelf { get; set; }
        public IEnumerable<SelectListItem> Shelves { get; set; }

    }
}