using Gestio_Botiga_Calcat.model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gestio_Botiga_Calcat
{
    class Service
    {
        private IMongoDatabase _database;

        public Service(string databaseName)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase(databaseName);
        }

        public UsuariMDB GetUser(string login)
        {
            var collection = _database.GetCollection<BsonDocument>("Usuari");
            var result = collection.Find(Builders<BsonDocument>.Filter.Eq("login", login)).FirstOrDefault();
            UsuariMDB usuariMDB = new UsuariMDB();

            usuariMDB.Login = login;
            usuariMDB.Pwd = (string)result["pwd"];
            usuariMDB.Nom = (string)result["nom"];
            usuariMDB.Cognom = (string)result["cognom"];
            usuariMDB.Mail = (string)result["mail"];
            usuariMDB.Telf = (string)result["telf"];

            var adreca = result["adreca"].AsBsonDocument;
            usuariMDB.Carrer = adreca["carrer"].AsString;
            usuariMDB.CodiPostal = adreca["codi_postal"].AsString;
            usuariMDB.Municipi = adreca["municipi"].AsString;
            usuariMDB.Pais = adreca["pais"].AsString;

            return usuariMDB;
        }

        public List<CategoriaMDB> GetAllCates()
        {
            var collection = _database.GetCollection<BsonDocument>("Categoria");
            var cates = collection.Find(new BsonDocument()).ToList();

            List<CategoriaMDB> cats = new List<CategoriaMDB>();
            foreach (var cate in cates)
            {
                CategoriaMDB c = new CategoriaMDB();

                string nom = cate["nom"].ToString();
                ObjectId id = ((ObjectId)cate["_id"]);
                c.Id = id;
                c.Name = nom;
                if (cate.Contains("cate_pare"))
                {
                    ObjectId cate_pare = ((ObjectId)cate["cate_pare"]);
                    c.cate_pare = cate_pare;
                }
                else
                {
                    c.cate_pare = new ObjectId();
                }
                cats.Add(c);
            }
            return cats;
        }
        public CategoriaMDB GetCate(ObjectId oid)
        {
            var collection = _database.GetCollection<BsonDocument>("Categoria");
            var result = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", oid)).FirstOrDefault();
            return Bdoc_a_cate(result);
        }
        public List<CategoriaMDB> GetCatesPare()
        {
            var collection = _database.GetCollection<BsonDocument>("Categoria");
            var result = collection.Find(Builders<BsonDocument>.Filter.Exists("cate_pare", false)).ToList();


            return List_BDocs_a_cates(result);
        }


        public List<CategoriaMDB> GetCatesFill(ObjectId oid)
        {
            Console.WriteLine("AAA");
            var collection = _database.GetCollection<BsonDocument>("Categoria");
            var result = collection.Find(Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Ne("_id", oid),
                Builders<BsonDocument>.Filter.Exists("cate_pare", true),
                Builders<BsonDocument>.Filter.Eq("cate_pare", oid)
            )).ToList();
            return List_BDocs_a_cates(result);
        }

        List<CategoriaMDB> List_BDocs_a_cates(List<BsonDocument> bdoc)
        {
            List<CategoriaMDB> cats = new List<CategoriaMDB>();
            foreach (var cate in bdoc)
            {

                cats.Add(Bdoc_a_cate(cate));
            }
            return cats;
        }

        CategoriaMDB Bdoc_a_cate(BsonDocument cate)
        {
            CategoriaMDB c = new CategoriaMDB();

            string nom = cate["nom"].ToString();
            ObjectId id = ((ObjectId)cate["_id"]);
            c.Id = id;
            c.Name = nom;
            if (cate.Contains("cate_pare"))
            {
                ObjectId cate_pare = ((ObjectId)cate["cate_pare"]);
                c.cate_pare = cate_pare;
            }
            else
            {
                c.cate_pare = new ObjectId();
            }
            return c;

        }


        /*public List<BsonDocument> GetProdsDeCate(ObjectId oid)
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");
            List<BsonDocument> result = collection.Find(Builders<BsonDocument>.Filter.Eq("categories", oid)).ToList();
            if(result.Count == 0)
            {
                List<BsonDocument> cates = GetCatesFill(oid);
                foreach (BsonDocument c in cates)
                {
                    result = collection.Find(Builders<BsonDocument>.Filter.Eq("categories", ((ObjectId)c))).ToList();
                    if (result.Count == 0)
                    {
                        break;
                    }
                }
            }

            return result;
        }*/
        public List<ProducteMDB> GetProdsDeCate(ObjectId oid)
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");
            List<BsonDocument> result = collection.Find(Builders<BsonDocument>.Filter.Eq("categories", oid)).ToList();

            List<ProducteMDB> cats = List_Bson_a_prod(result);

            if (cats.Count == 0)
            {
                List<CategoriaMDB> cates = GetCatesFill(oid);
                foreach (CategoriaMDB c in cates)
                {
                    result = collection.Find(Builders<BsonDocument>.Filter.Eq("categories", c.Id)).ToList();
                    cats = List_Bson_a_prod(result);
                    if (cats.Count == 0)
                    {
                        break;
                    }
                }
            }

            return cats;
        }

        public List<IVA_MDB> GetAllIVA()
        {
            var collection = _database.GetCollection<BsonDocument>("IVA");
            var cates = collection.Find(new BsonDocument()).ToList();
            List<IVA_MDB> ives = new List<IVA_MDB>();
            foreach (var c in cates)
            {
                IVA_MDB iva = new IVA_MDB();
                iva.Id = (ObjectId)c["_id"];
                iva.Percentatge = (double)c["val"];

            }
            return ives;
        }

        public IVA_MDB GetIVA(ObjectId oid)
        {
            var collection = _database.GetCollection<BsonDocument>("IVA");
            var c = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", oid)).FirstOrDefault();
           
                IVA_MDB iva = new IVA_MDB();
                iva.Id = (ObjectId)c["_id"];
                iva.Percentatge = (double)c["val"];

            return iva;
        }

        public List<ProducteMDB> GetAllProds()
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");
            var prods = collection.Find(new BsonDocument()).ToList();
            return List_Bson_a_prod(prods);
        }

        List<ProducteMDB> List_Bson_a_prod(List<BsonDocument> prods) {
            List<ProducteMDB> productes = new List<ProducteMDB>();

            foreach (BsonDocument p in prods)
            {
                ProducteMDB prod = new ProducteMDB();

                ObjectId id = ((ObjectId)p["_id"]);
                prod.Id = id;
                prod.Nom = (string)p["nom"];
                prod.Desc = (string)p["desc"];

                var categories = p["categories"].AsBsonArray.Select(c => c.AsObjectId).ToList();
                prod.Categories = categories;

                prod.Tipus_IVA = p["tipus_IVA"].AsObjectId;

                var variants = p["variants"].AsBsonArray.Select(v => new VariantMDB
                {
                    Color = v["color"].AsString,
                    Preu = v["preu"].AsDouble,
                    DescomptePercent = v["descompte_percent"].AsInt32,
                    Stock = v["stock"].AsBsonArray.Select(s => new StockMDB
                    {
                        Quantitat = s["num"].AsInt32,
                        Talla = s["talla"].AsInt32
                    }).ToList()
                }).ToList();
                prod.Variants = variants;

                productes.Add(prod);
            }

            return productes;

        }
    }
}
