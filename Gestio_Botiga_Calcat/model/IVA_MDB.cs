using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{
    class IVA_MDB
    {
        public ObjectId Id { get; set; }
        public double Percentatge { get; set; }
    }
}
