using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{
    class CategoriaMDB
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public ObjectId cate_pare { get; set; }

    }
}
