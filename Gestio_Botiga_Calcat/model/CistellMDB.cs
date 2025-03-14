using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{
    public class CistellMDB
    {
        public ObjectId Id { get; set; }
        public ObjectId Id_usu { get; set; }
        public double Cost_enviament { get; set; }
        public ObjectId Metode_enviament { get; set; }
        public List<Prod_select> Prod_select { get; set; }
    }

    public class Prod_select
    {
        public ObjectId Id { get; set; }
        public ObjectId Estoc_id { get; set; }
        public int Quantitat { get; set; }
    }
}
