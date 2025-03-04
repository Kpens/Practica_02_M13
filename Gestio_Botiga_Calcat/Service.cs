using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<BsonDocument> GetAllUsers()
        {
            var collection = _database.GetCollection<BsonDocument>("Usuari");
            var users = collection.Find(new BsonDocument()).ToList();
            return users;
        }
        public List<BsonDocument> GetAllCates()
        {
            var collection = _database.GetCollection<BsonDocument>("Categoria");
            var cates = collection.Find(new BsonDocument()).ToList();
            return cates;
        }
        public List<BsonDocument> GetAllIVA()
        {
            var collection = _database.GetCollection<BsonDocument>("IVA");
            var cates = collection.Find(new BsonDocument()).ToList();
            return cates;
        }
        public List<BsonDocument> GetAllProds()
        {
            var collection = _database.GetCollection<BsonDocument>("Producte");
            var cates = collection.Find(new BsonDocument()).ToList();
            return cates;
        }
    }
}
