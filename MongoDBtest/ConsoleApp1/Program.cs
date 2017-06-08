using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
namespace ConsoleApp1
{
    class MongoDBManager
    {
        private string m_ipAddress = null;
        private int m_portNumber = -1;
        private string m_userID = null;
        private string m_password = null;
        private string m_databaseName = null;

        private MongoClient m_mongoClient = null;
        private IMongoDatabase m_mongoDatabase = null;
        private FilterDefinitionBuilder<BsonDocument> m_builder = null;
        private FilterDefinition<BsonDocument> m_filter = null;
        private string m_CollectionName = null;

        public MongoDBManager(string pIP, int pPortNum, string pID, string pPW, string pDBName)
        {
            m_ipAddress = pIP;
            m_portNumber = pPortNum;
            m_userID = pID;
            m_password = pPW;
            m_databaseName = pDBName;

            var credential = MongoCredential.CreateCredential(m_databaseName, m_userID, m_password);

            var setting = new MongoClientSettings
            {
                Credentials = new[] { credential },
                Server = new MongoServerAddress(m_ipAddress, m_portNumber)
            };

            m_mongoClient = new MongoClient(setting);
            m_mongoDatabase = m_mongoClient.GetDatabase(m_databaseName);
        }

        public void GetResult()
        {

        }

        public void SetCollection(string pCollectionName)
        {
            m_CollectionName = pCollectionName;
        }

        public void ClearFilter()
        {
            m_filter = null;
        }

        public void AddFilter(string pIndex, string pValue , int pCharaterNum)
        {
         
            if(pIndex.Length == pCharaterNum)
            {
                equalString(pIndex, pValue);
            }
            else
            {
                regexString(pIndex, pValue, pCharaterNum);
            }
        }

        public void AddFilterEqInt(string pIndex,int pValue)
        {
            m_builder = Builders<BsonDocument>.Filter;
            if (m_filter == null)
            {
                m_filter = m_builder.Eq(pIndex, pValue);
            }
            else
            {
                m_filter = m_filter & m_builder.Eq(pIndex, pValue);
            }
            m_builder = null;
        }

        private void equalString(string pIndex, string pValue)
        {
            m_builder = Builders<BsonDocument>.Filter;
            if (m_filter == null)
            {
                m_filter = m_builder.Eq(pIndex, pValue);
            }
            else
            {
                m_filter = m_filter & m_builder.Eq(pIndex, pValue);
            }

            m_builder = null;
        }

        private void regexString(string pIndex, string pValue , int pNum)
        {
            m_builder = Builders<BsonDocument>.Filter;
            var str = pValue.Substring(0, pNum);
            if (m_filter == null)
            {
                m_filter = m_builder.Regex(pIndex, str);   
            }
            else
            {
                m_filter = m_filter & m_builder.Regex(pIndex, str);
            }

            m_builder = null;
        }


    }

    class BabyName
    {
        public ObjectId Id { get; set; }
        public int NotUse { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public string Gender{ get; set; }
        public int Count{ get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var m_ipAddress = "211.249.60.69";
            var m_portNumber = 27017;
            var m_userID = "next";
            var m_password = "next!!@@##$$";
            var m_databaseName = "baybName";

            var credential = MongoCredential.CreateCredential(m_databaseName, m_userID, m_password);

            var setting = new MongoClientSettings
            {
                Credentials = new[] { credential },
                Server = new MongoServerAddress(m_ipAddress, m_portNumber)
            };

            var m_mongoClient = new MongoClient(setting);
            var m_mongoDatabase = m_mongoClient.GetDatabase(m_databaseName);


            /*
            var collection = m_mongoDatabase.GetCollection<BabyName>("nationalBabyName");
           // var filter = new BsonDocument("Name", "Mary");
            using (var cursor = collection.FindSync(BabyName => BabyName.Name == "Mary"))
            {

                while (cursor.MoveNext())
                {
                    IEnumerable<BabyName> batch = cursor.Current;
                    foreach (BabyName document in batch)
                    {
                        Console.WriteLine(document.Name + " " + document.Gender);
                    }
                }
            }
            //collection.Find(BabyName => BabyName.Name == "Mary").ForEachAsync(BabyName => Console.WriteLine(BabyName.Name + " " + BabyName.Gender));
            */

            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Regex("Name", "Ma") & builder.Eq("Gender", "F");
            var collection = m_mongoDatabase.GetCollection<BsonDocument>("nationalBabyName");
            //var qu = collection.AsQueryable();
            //var filter = new BsonDocument("Name", "Mary");
            using (IAsyncCursor<BsonDocument> cursor =  collection.FindSync(filter))
            {
                
                while ( cursor.MoveNext())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        string name = null;
                        
                        if (document["Name"].IsString == true)
                        {
                            name = document["Name"].AsString;
                        }

                        int count = 0;

                        if(document["Count"].IsNumeric == true )
                        {
                            count = document["Count"].AsInt32;
                        }

                        string gender = null;

                        if(document["Gender"].IsString == true)
                        {
                            gender = document["Gender"].AsString;
                        }

                        Console.Write(name);
                        Console.Write(", Gender = ");
                        Console.Write(gender);
                        Console.Write(", Count = ");
                        Console.WriteLine(count);
                        Console.WriteLine();
                    }
                }
            }
            
        }
    }
}
