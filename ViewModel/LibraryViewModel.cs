using MyLibrary.Models;
using MyLibrary.Models.DBObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.ViewModel
{
    public class LibraryViewModel
    {
        public string LibraryDescription { get; set; }
        public Guid LibraryId { get; set; }
        public List<BookshelfModel> Bookshelfs { get; set; }
        public Dictionary<Guid, List<ShelfModel>> Shelfs { get; set; }
    }
}