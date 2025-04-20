using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{
    public class Dades_empresa
    {
        public ObjectId Id { get; set; }
        public string Nom { get; set; }
        public string Telf { get; set; }
        public string Logo { get; set; }
        public string Mail { get; set; }
        public string CIF { get; set; }
        public string CodiPostal { get; set; }
        public string Carrer { get; set; }
        public string Pais { get; set; }
        public string Ciutat { get; set; }
        //public ObservableCollection<ProducteMDB> Productes { get; set; }
    }
}
