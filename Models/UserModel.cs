using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.Models
{
    public class UserModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}