using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace karty_przedmiotow
{
    class DataBlock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        string Kod_lokalny { get; set; }
        string Kod_globalny { get; set; }
        string Typ { get; set; }



    }
}
