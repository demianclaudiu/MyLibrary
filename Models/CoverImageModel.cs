using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.Models
{
    public class CoverImageModel
    {
        public Guid CoverImageId { get; set; }
        public string ImageLocation { get; set; }
        public bool IsLocal { get; set; }
    }
}