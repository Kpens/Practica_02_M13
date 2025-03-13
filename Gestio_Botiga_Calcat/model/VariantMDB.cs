using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{
    public class VariantMDB
    {
        public string Color { get; set; }

        public double Preu { get; set; }

        public List<string> Fotos { get; set; } = new List<string>();

        public int DescomptePercent { get; set; }

        public List<StockMDB> Stock { get; set; } = new List<StockMDB>();
    }
}
