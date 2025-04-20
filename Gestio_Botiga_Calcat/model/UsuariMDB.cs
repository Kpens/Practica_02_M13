using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{
    public class UsuariMDB
    {
        public ObjectId Id { get; set; }
        public string Login { get; set; }
        public string Nom { get; set; }
        public string Pwd { get; set; }
        public string Cognom { get; set; }
        public string Mail { get; set; }
        public string Telf { get; set; }
        public string CodiPostal { get; set; }
        public string Carrer { get; set; }
        public string Pais { get; set; }
        public string Ciutat { get; set; }
        public string Municipi { get; set; }


    }
}
