using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{

    public class Prod_select : INotifyPropertyChanged
    {
        public ObjectId Id { get; set; }
        public ObjectId Estoc_id { get; set; }
        public int Quantitat { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
