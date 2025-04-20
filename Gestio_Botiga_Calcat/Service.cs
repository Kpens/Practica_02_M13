using Gestio_Botiga_Calcat.model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gestio_Botiga_Calcat
{
    public class Service
    {
        private IMongoDatabase _database;

        public Service(string databaseName)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase(databaseName);
        }

        public CistellMDB GetCistell(ObjectId id)
        {
            var collection = _database.GetCollection<BsonDocument>("Cistell");
            var result = collection.Find(Builders<BsonDocument>.Filter.Eq("id_usu", id)).FirstOrDefault();


            if (result != null)
            {
                return new CistellMDB
                {
                    Id = result["_id"].AsObjectId,
                    Id_usu = result["id_usu"].AsObjectId,
                    Cost_enviament = result["cost_enviament"].AsDouble,
                    Metode_enviament = result["metode_enviament"].AsObjectId,
                    Prod_select = new ExtendedObservableCollection<Prod_select>(
                    result["prods_select"].AsBsonArray.Select(prod => new Prod_select
                    {
                        Id = prod["_id"].AsObjectId,
                        Estoc_id = prod["estoc_id"].AsObjectId,
                        Quantitat = prod["qt"].AsInt32
                    }))
                };
            }

            return null;
        }

        public UsuariMDB GetUsuari(string login, string contra)
        {
            var collection = _database.GetCollection<BsonDocument>("Usuari");
            var result = collection.Find(Builders<BsonDocument>.Filter.Eq("login", login)).FirstOrDefault();

            if (result != null) { 
            
                UsuariMDB usuariMDB = new UsuariMDB();

                usuariMDB.Login = login;
                usuariMDB.Pwd = (string)result["pwd"];

                string pwd_b64 = usuariMDB.Pwd;
                string contra_bbdd = Encoding.UTF8.GetString(Convert.FromBase64String(pwd_b64));

                if (contra_bbdd == contra)
                {
                    usuariMDB.Id = ((ObjectId)result["_id"]);
                    usuariMDB.Nom = result["nom"].ToString().ElementAt(0).ToString().ToUpper() +result["nom"].ToString().Substring(1).ToLower();
                    usuariMDB.Cognom = result["cognom"].ToString().ElementAt(0).ToString().ToUpper() + result["cognom"].ToString().Substring(1).ToLower();
                    usuariMDB.Mail = (string)result["mail"];
                    usuariMDB.Telf = (string)result["telf"];

                    var adreca = result["adreca"].AsBsonDocument;
                    usuariMDB.Carrer = adreca["carrer"].AsString;
                    usuariMDB.CodiPostal = adreca["codi_postal"].AsString;
                    usuariMDB.Ciutat = adreca["Ciutat"].AsString;
                    usuariMDB.Municipi = adreca["Municipi"].AsString;
                    usuariMDB.Pais = adreca["pais"].AsString;
                    return usuariMDB;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
                
                
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
        private Metode_enviamentMDB doc_a_metode(BsonDocument c)
        {
            Metode_enviamentMDB metode = new Metode_enviamentMDB();
            metode.Id = (ObjectId)c["_id"];
            metode.MinTemps_en_dies = (int)c["min_temps"];
            metode.MaxTemps_en_dies = (int)c["max_temps"];
            metode.Nom = (string)c["metode"];
            metode.Preu_base = (double)c["preu_base"];
            metode.Preu_min_compra = (double)c["preu_min_compra"];
            metode.Id_IVA = (ObjectId)c["tipus_IVA"];
            return metode;
        }
        public List<Metode_enviamentMDB> GetMetodes_enviament()
        {
            var collection = _database.GetCollection<BsonDocument>("Metode_enviament");
            var cates = collection.Find(new BsonDocument()).ToList();
            List<Metode_enviamentMDB> metodes = new List<Metode_enviamentMDB>();
            foreach (var c in cates)
            {
                metodes.Add(doc_a_metode(c));
            }
            return metodes;
        }
        public Metode_enviamentMDB GetMetode_enviament(ObjectId oid)
        {
            var collection = _database.GetCollection<BsonDocument>("Metode_enviament");
            var metode = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", oid)).FirstOrDefault();
            
            
            
            return doc_a_metode(metode);
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
                DescomptePercent = v["descompte_percent"].AsDouble,
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

            var prod = collection.Find(
                    Builders<BsonDocument>.Filter.ElemMatch("variants",
                    Builders<BsonDocument>.Filter.ElemMatch("stock",
                    Builders<BsonDocument>.Filter.Eq("_id", stockId)))
                ).FirstOrDefault();

            if (prod != null)
            {
                var variants = prod["variants"].AsBsonArray;
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
                                DescomptePercent = variant["descompte_percent"].AsDouble,
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
            BsonDocument prod = collection.Find(
                    Builders<BsonDocument>.Filter.ElemMatch("variants",
                    Builders<BsonDocument>.Filter.ElemMatch("stock",
                    Builders<BsonDocument>.Filter.Eq("_id", stockId)))
                ).FirstOrDefault();

            if (prod != null)
            {
                var variants = prod["variants"].AsBsonArray;
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
            foreach (BsonDocument prod in prods)
            {
                var vars = prod["variants"].AsBsonArray;
                foreach (var v in vars)
                {
                    VariantMDB var = new VariantMDB
                    {
                        Color = v["color"].AsString,
                        Preu = v["preu"].AsDouble,
                        DescomptePercent = v["descompte_percent"].AsDouble,
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

            var fil = new List<FilterDefinition<BsonDocument>>();

            if (!string.IsNullOrEmpty(nom))
            {
                fil.Add(Builders<BsonDocument>.Filter.Regex("nom", new BsonRegularExpression(nom, "i")));
            }

            fil.Add(Builders<BsonDocument>.Filter.Lte("variants.preu", max_preu));
            fil.Add(Builders<BsonDocument>.Filter.Gte("variants.preu", min_preu)); 
            

            if (cate != null)
            {
                fil.Add(Builders<BsonDocument>.Filter.Eq("categories", cate.Id));
            }
            if (stock != null)
            {
                var stockFilter = Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>("variants.stock", Builders<BsonDocument>.Filter.Eq("talla", stock.Talla));
                fil.Add(stockFilter);
            }
            var combinedFilter = Builders<BsonDocument>.Filter.And(fil);

            var result = collection.Find(combinedFilter).ToList();

            return List_Bson_a_prod(result);
        }
        public void ActualizarCistell()
        {
            if(Global.Usuari != null) {

                var collection = _database.GetCollection<BsonDocument>("Cistell");

                var cistell = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", Global.cistellManager.Id_cistell)).FirstOrDefault();

                if (cistell == null)
                {
                    Global.cistellManager.Metode_enviament = new Metode_enviamentMDB();
                    var doc = new BsonDocument
                {
                    { "id_usu", Global.Usuari.Id},
                    { "cost_enviament", Global.cistellManager.Cost_enviament },
                    { "metode_enviament", Global.cistellManager.Metode_enviament.Id },
                    { "prods_select", new BsonArray(Global.cistellManager.GetLlistaProds().Select(p => new BsonDocument
                        {
                            { "_id", p.Id },
                            { "estoc_id", p.Estoc_id },
                            { "qt", p.Quantitat }
                        }))
                    }
                };

                    collection.InsertOne(doc);
                }
                else
                {
                    var prods_existents = new Dictionary<ObjectId, BsonValue>();

                    foreach (var prod in cistell["prods_select"].AsBsonArray)
                    {
                        var estocId = prod["estoc_id"].AsObjectId;

                        if (!prods_existents.ContainsKey(estocId))
                        {
                            prods_existents.Add(estocId, prod);
                        }
                    }


                    var arrModif_prods = new BsonArray();

                    foreach (var prod in Global.cistellManager.GetLlistaProds())
                    {
                        if (prods_existents.TryGetValue(prod.Estoc_id, out var prod_existent))
                        {
                            prod_existent["qt"] = prod.Quantitat;
                            arrModif_prods.Add(prod_existent);
                        }
                        else
                        {
                            var doc = new BsonDocument
                        {
                            { "_id", prod.Id },
                            { "estoc_id", prod.Estoc_id },
                            { "qt", prod.Quantitat }
                        };
                            arrModif_prods.Add(doc);
                        }
                    }

                    var fil = Builders<BsonDocument>.Filter.Eq("_id", Global.cistellManager.Id_cistell);
                    var modif = Builders<BsonDocument>.Update.Set("prods_select", arrModif_prods);
                    collection.UpdateOne(fil, modif);
                }

            }
                           
        }
        public int magicDev(StockMDB stock)
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");

            var prod = collection.Find(
                    Builders<BsonDocument>.Filter.ElemMatch("variants",
                    Builders<BsonDocument>.Filter.ElemMatch("stock",
                    Builders<BsonDocument>.Filter.Eq("_id", stock.Id)))
                ).FirstOrDefault();

            if (prod != null)
            {
                var variants = prod["variants"].AsBsonArray;
                foreach (BsonDocument variant in variants)
                {
                    var sts = variant["stock"].AsBsonArray;
                    foreach (BsonDocument st in sts)
                    {
                        if (st["_id"] == stock.Id)
                        {
                            st["num"] = st["num"].AsInt32 + 1;

                            var fil = Builders<BsonDocument>.Filter.Eq("_id", prod["_id"]);
                            var modif = Builders<BsonDocument>.Update.Set("variants", variants);
                            collection.UpdateOne(fil, modif);

                            return st["num"].AsInt32;
                        }
                    }
                }
            }
            return 0;
        }
        public void crear_factura(FacturaMDB factura)
        {
            var collection_factura = _database.GetCollection<FacturaMDB>("Factura");
            collection_factura.InsertOne(factura);

            var collection_compt = _database.GetCollection<BsonDocument>("Comptador");

            var filter = Builders<BsonDocument>.Filter.Exists("factura");
            var update = Builders<BsonDocument>.Update.Inc("factura", 1);

            collection_compt.UpdateOne(filter, update);

            var updatedDoc = collection_compt.Find(new BsonDocument()).First();
            Global.QtFactures = updatedDoc["factura"].AsInt32;
        }
        public int GetNumFactura()
        {
            var collection = _database.GetCollection<BsonDocument>("Comptador");
            var prods = collection.Find(new BsonDocument()).First();

            Global.QtFactures = prods["factura"].AsInt32;
            Global.QtReparacions = prods["reparacio"].AsInt32;
            return prods["num_decimals_admesos"].AsInt32;
        }
        public Dades_empresa GetEmpresa()
        {
            Dades_empresa empresa = new Dades_empresa();
            var collection = _database.GetCollection<BsonDocument>("Dades_empresa");
            var prods = collection.Find(new BsonDocument()).First();

            empresa.Id = prods["_id"].AsObjectId;
            empresa.Nom = prods["nom"].AsString;
            empresa.Mail = prods["mail"].AsString;
            empresa.Telf = prods["telf"].AsString;
            empresa.CIF = prods["CIF"].AsString;

            var adreca = prods["adreca"].AsBsonDocument;

            empresa.Carrer = adreca["Carrer"].AsString;
            empresa.CodiPostal = adreca["Codi_postal"].AsString;
            empresa.Ciutat = adreca["Ciutat"].AsString;
            empresa.Pais = adreca["Pais"].AsString;
            return empresa;
        }

        public void ActualitzarQtProdsFactura(List<Prod_select> prods)
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");
            foreach (var prod in prods)
            {
                var filter = Builders<BsonDocument>.Filter.ElemMatch("variants",
                    Builders<BsonDocument>.Filter.ElemMatch("stock",
                    Builders<BsonDocument>.Filter.Eq("_id", prod.Estoc_id)));
                var update = Builders<BsonDocument>.Update.Inc("variants.$[].stock.$[stock].num", -prod.Quantitat);
                var options = new UpdateOptions
                {
                    ArrayFilters = new List<ArrayFilterDefinition<BsonDocument>>
                    {
                        new BsonDocumentArrayFilterDefinition<BsonDocument>(
                            new BsonDocument("stock._id", prod.Estoc_id)
                        )
                    }
                };
                collection.UpdateOne(filter, update, options);
            }

        }
        public void RetornarQtProdsFactura(List<Prod_select> prods)
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");
            foreach (var prod in prods)
            {
                var filter = Builders<BsonDocument>.Filter.ElemMatch("variants",
                    Builders<BsonDocument>.Filter.ElemMatch("stock",
                    Builders<BsonDocument>.Filter.Eq("_id", prod.Estoc_id)));
                var update = Builders<BsonDocument>.Update.Inc("variants.$[].stock.$[stock].num", prod.Quantitat);
                var options = new UpdateOptions
                {
                    ArrayFilters = new List<ArrayFilterDefinition<BsonDocument>>
                    {
                        new BsonDocumentArrayFilterDefinition<BsonDocument>(
                            new BsonDocument("stock._id", prod.Estoc_id)
                        )
                    }
                };
                collection.UpdateOne(filter, update, options);
            }
        }

    }

}
