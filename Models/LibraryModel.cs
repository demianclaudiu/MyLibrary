﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyLibrary.Models
{
    public class LibraryModel
    {
        [Key]
        public Guid LibraryId { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
    }
}