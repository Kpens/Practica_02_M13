using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestio_Botiga_Calcat.model
{
    public class FacturaMDB
    {
        public ObjectId Id { get; set; }
        public string Codi{ get; set; }
        public ObjectId Emissor { get; set; }
        public ObjectId Destinatari { get; set; }
        public List<Linies> Linies_compra { get; set; }
        public DateTime Data_factura { get; set; }
        public DateTime Data_venciment { get; set; }
        public double Cost_enviament { get; set; }
        public string Metode_pagament { get; set; }
        public double Preu_base1 { get; set; }
        public double Preu_base2 { get; set; }
        public double Preu_base3 { get; set; }
        public double iva1 { get; set; }
        public double iva2 { get; set; }
        public double iva3 { get; set; }
        public double Total { get; set; }

    }

    public class Linies
    {
        public string Nom { get; set; }
        public int Descompte { get; set; }
        public double Preu_base { get; set; }
        public ObjectId tipus_IVA { get; set; }
        public int perc_IVA { get; set; }
        public ObjectId Estoc { get; set; }
        public int Quantitat { get; set; }
        public double Bases_imposables { get; set; }
        public double Total { get; set; }
    }
}
