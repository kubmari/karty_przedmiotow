using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace karty_przedmiotow
{
    public class KartContext : DbContext
    {
        public DbSet<KodGlobalny> kodGlobalny { get; set; }
        public DbSet<Typ> typ { get; set; }
    }
}
