﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{
    public class StockMDB
    {
        public ObjectId Id { get; set; }
        public int Quantitat { get; set; }

        public int Talla { get; set; }
    }
}
