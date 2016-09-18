using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace karty_przedmiotow
{
    public class Typ
    {
        public int id { get; set; }
        public string kodLokalny { get; set; }
        public string typ { get; set; }
    }
}
