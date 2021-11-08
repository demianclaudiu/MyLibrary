using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyLibrary.ViewModel
{
    public class SelectBookViewModel
    {
        public Guid SelectedBook { get; set; }
        public List<BookRadioButtonViewModel> Books { get; set; }
    }
}