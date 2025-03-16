using Gestio_Botiga_Calcat.model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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
        public List<Metode_enviamentMDB> GetMetodes_enviament()
        {
            var collection = _database.GetCollection<BsonDocument>("Metode_enviament");
            var cates = collection.Find(new BsonDocument()).ToList();
            List<Metode_enviamentMDB> metodes = new List<Metode_enviamentMDB>();
            foreach (var c in cates)
            {
                Metode_enviamentMDB metode = new Metode_enviamentMDB();
                metode.Id = (ObjectId)c["_id"];
                metode.MinTemps_en_dies = (int)c["min_temps"];
                metode.MaxTemps_en_dies = (int)c["max_temps"];
                metode.Nom = (string)c["metode"];
                metode.Preu_base = (double)c["preu_base"];
                metode.Preu_min_compra = (double)c["preu_min_compra"];
                metode.Id_IVA = (ObjectId)c["tipus_IVA"];
                metodes.Add(metode);
            }
            return metodes;
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
        public ProducteMDB GetProd(ObjectId oid)
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");
            var result = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", oid)).FirstOrDefault();
            return Bson_a_prod(result);
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
                ProducteMDB prod = Bson_a_prod(p);

                productes.Add(prod);
            }

            return productes;

        }


        ProducteMDB Bson_a_prod(BsonDocument p)
        {
            ProducteMDB prod = new ProducteMDB();

            prod.Id = ((ObjectId)p["_id"]);
            prod.Nom = (string)p["nom"];
            prod.Marca = (string)p["marca"];
            prod.Desc = (string)p["desc"];

            var categories = p["categories"].AsBsonArray.Select(c => c.AsObjectId).ToList();
            prod.Categories = categories;

            prod.Tipus_IVA = p["tipus_IVA"].AsObjectId;

            var variants = p["variants"].AsBsonArray.Select(v => new VariantMDB
            {
                Color = v["color"].AsString,
                Preu = v["preu"].AsDouble,
                DescomptePercent = v["descompte_percent"].AsInt32,
                Fotos = v["fotos"].AsBsonArray.Select(f => f.AsString).ToList(),
                Stock = v["stock"].AsBsonArray.Select(s => new StockMDB
                {
                    Id = ((ObjectId)s["_id"]),
                    Quantitat = s["num"].AsInt32,
                    Talla = s["talla"].AsInt32
                }).ToList()

            }).ToList();
            prod.Variants = variants;
            return prod;
        }

        public VariantMDB GetVariantByStockId(ObjectId stockId)
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");

            var product = collection.Find(
                    Builders<BsonDocument>.Filter.ElemMatch("variants",
                    Builders<BsonDocument>.Filter.ElemMatch("stock",
                    Builders<BsonDocument>.Filter.Eq("_id", stockId)))
                ).FirstOrDefault();

            if (product != null)
            {
                var variants = product["variants"].AsBsonArray;
                foreach (BsonDocument variant in variants)
                {
                    var sts = variant["stock"].AsBsonArray;
                    foreach (BsonDocument st in sts)
                    {
                        if (st["_id"] == stockId)
                        {
                            return new VariantMDB
                            {
                                Color = variant["color"].AsString,
                                Preu = variant["preu"].AsDouble,
                                DescomptePercent = variant["descompte_percent"].AsInt32,
                                Fotos = variant["fotos"].AsBsonArray.Select(f => f.AsString).ToList(),
                                Stock = sts.Select(s => new StockMDB
                                {
                                    Id = s["_id"].AsObjectId,
                                    Quantitat = s["num"].AsInt32,
                                    Talla = s["talla"].AsInt32
                                }).ToList()
                            };
                        }
                    }
                }
            }

            return null;
        }

        public StockMDB GetStockById(ObjectId stockId)
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");
            BsonDocument product = collection.Find(
                    Builders<BsonDocument>.Filter.ElemMatch("variants",
                    Builders<BsonDocument>.Filter.ElemMatch("stock",
                    Builders<BsonDocument>.Filter.Eq("_id", stockId)))
                ).FirstOrDefault();

            if (product != null)
            {
                var variants = product["variants"].AsBsonArray;
                foreach (var variant in variants)
                {
                    var stocks = variant["stock"].AsBsonArray;
                    foreach (BsonDocument st in stocks)
                    {
                        if (st["_id"] == stockId)
                        {
                            StockMDB stock = new StockMDB
                            {
                                Id = st["_id"].AsObjectId,
                                Quantitat = st["num"].AsInt32,
                                Talla = st["talla"].AsInt32
                            };

                            return stock;
                        }
                    }
                }
            }

            return null;
        }

        public List<VariantMDB> GetAllVariants()
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");
            var prods = collection.Find(new BsonDocument()).ToList();
            List<VariantMDB> variants = new List<VariantMDB>();
            foreach (BsonDocument product in prods)
            {
                var vars = product["variants"].AsBsonArray;
                foreach (var v in vars)
                {
                    VariantMDB var = new VariantMDB
                    {
                        Color = v["color"].AsString,
                        Preu = v["preu"].AsDouble,
                        DescomptePercent = v["descompte_percent"].AsInt32,
                        Fotos = v["fotos"].AsBsonArray.Select(f => f.AsString).ToList(),
                        Stock = v["stock"].AsBsonArray.Select(s => new StockMDB
                        {
                            Id = ((ObjectId)v["_id"]),
                            Quantitat = s["num"].AsInt32,
                            Talla = s["talla"].AsInt32
                        }).ToList()

                    };
                    variants.Add(var);

                }
            }

            return variants;
        }
        public List<ProducteMDB> ProdsFiltrats(StockMDB stock, CategoriaMDB cate, int min_preu, int max_preu, string nom)
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");

            var filters = new List<FilterDefinition<BsonDocument>>();

            if (!string.IsNullOrEmpty(nom))
            {
                filters.Add(Builders<BsonDocument>.Filter.Regex("nom", new BsonRegularExpression(nom, "i")));
            }

            filters.Add(Builders<BsonDocument>.Filter.Lte("variants.preu", max_preu)); 
            filters.Add(Builders<BsonDocument>.Filter.Gte("variants.preu", min_preu)); 
            

            if (cate != null)
            {
                filters.Add(Builders<BsonDocument>.Filter.Eq("categories", cate.Id));
            }
            if (stock != null)
            {
                var stockFilter = Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>("variants.stock", Builders<BsonDocument>.Filter.Eq("talla", stock.Talla));
                filters.Add(stockFilter);
            }
            var combinedFilter = Builders<BsonDocument>.Filter.And(filters);

            var result = collection.Find(combinedFilter).ToList();

            return List_Bson_a_prod(result);
        }

    }


}
