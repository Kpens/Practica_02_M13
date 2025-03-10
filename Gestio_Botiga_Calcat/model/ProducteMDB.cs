using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{
    class ProducteMDB
    {
        public ObjectId Id { get; set; }
        public string Nom{ get; set; }
        //public List fotos
        public string Desc { get; set; }
        public List<ObjectId> Categories { get; set; }
        public ObjectId Tipus_IVA { get; set; }
        public List<VariantMDB> Variants { get; set; } = new List<VariantMDB>();

    }

}
