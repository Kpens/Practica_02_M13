﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{
    public class Metode_enviamentMDB
    {
        public ObjectId Id { get; set; }
        public int MinTemps_en_dies { get; set; }
        public int MaxTemps_en_dies { get; set; }
        public string Nom { get; set; }
        public double Preu_base { get; set; }
        public double Preu_min_compra { get;set; }
        public ObjectId Id_IVA { get; set; }
    }
}
