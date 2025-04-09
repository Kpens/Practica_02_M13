using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ObservableCollection<Prod_select> Prod_select { get; set; }
    }

}
