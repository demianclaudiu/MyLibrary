using MyLibrary.Models;
using MyLibrary.Models.DBObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace MyLibrary.ViewModel
{
    public class LibraryAddViewModel
    {
        [Key]
        public Guid LibraryId { get; set; }
        public string LibraryDescription { get; set; }
        public int NumberOfBookshelfs { get; set; }
    }
}