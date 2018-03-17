using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpsInfra
{
    public class DocumentDBMessageModel
    {
        public string TaskId { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionId { get; set; }
        public string ConnectionString { get; set; }
    }
    public class DocumentDBHelper
    {
        public string _ConnectionString;
        public string _DatabaseName;
        public string _CollectionId;
        public string _Action;
        public int _TaskId;
        private DocumentClient _Client;

        public DocumentDBHelper()
        {
        }
        public DocumentDBHelper(DocumentDBMessageModel docDBMsg, string action)
        {
            try
            {
                _ConnectionString = docDBMsg.ConnectionString;
                _DatabaseName = docDBMsg.DatabaseName;
                _CollectionId = docDBMsg.CollectionId;
                _Action = action;
                _TaskId = Int32.Parse(docDBMsg.TaskId);

                //init DocumentClient
                _ConnectionString = _ConnectionString.Replace("AccountEndpoint=", "");
                _ConnectionString = _ConnectionString.Replace(";", "");
                _ConnectionString = _ConnectionString.Replace("AccountKey=", ";");
                string endpointUri = _ConnectionString.Split(';')[0];
                string primaryKey = _ConnectionString.Split(';')[1];
                _Client = new DocumentClient(new Uri(endpointUri), primaryKey);

                if (action.StartsWith("create"))
                    _Action = "Create";
                else
                    _Action = "Purge";
            }
            catch(Exception)
            {
                throw new Exception("[DocumentDB] DocumentDBHelper initial error : ConnectionString's format is wrong");
            }            
        }

        public async void ThreadProc()
        {
            try
            {
                switch (_Action)
                {
                    case "Create":
                        await CreateDatabaseAndCollection();
                        break;
                    case "Purge":
                        await PurgeDatabase();
                        break;
                }

                Program.UpdateTaskBySuccess(_TaskId);
                Console.WriteLine("[DocumentDB] " + _Action + " success: Databae-" + _DatabaseName + ", CollectionId-" + _CollectionId);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("[DocumentDB] " + _Action + " Failed: Databae-" + _DatabaseName + ", CollectionId-" + _CollectionId);
                logMessage.AppendLine("\tMessage:" + JsonConvert.SerializeObject(this));
                logMessage.AppendLine("\tException:" + ex.Message);
                Program._sfAppLogger.Error(logMessage);
                Program.UpdateTaskByFail(_TaskId, ex.Message);

                Console.WriteLine(logMessage);
            }
        }
        
        private async Task PurgeDatabase()
        {
            try
            {
                await _Client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(_DatabaseName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }
            }
        }

        private async Task CreateDatabaseAndCollection()
        {
            try
            {
                await _Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_DatabaseName));
            }
            catch (DocumentClientException de)
            {

                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await _Client.CreateDatabaseAsync(new Database { Id = _DatabaseName });
                }
                else
                {
                    throw new Exception("[DocumentDB] Database-" + _DatabaseName + " exist!");
                }
            }

            //Create collection
            try
            {
                await _Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_DatabaseName, _CollectionId));
            }
            catch (DocumentClientException de)
            {
                // If the document collection does not exist, create a new collection
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = _CollectionId;
                    collectionInfo.PartitionKey.Paths.Add("/Message/equipmentId");

                    // Configure collections for maximum query flexibility including string range queries.
                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 }, new RangeIndex(DataType.Number) { Precision = -1 });
                    collectionInfo.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
                    collectionInfo.DefaultTimeToLive = 30*24*60*60; //30 days

                    // Here we create a collection with 400 RU/s.
                    DocumentCollection ttlEnabledCollection = await _Client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_DatabaseName),
                        collectionInfo,
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw new Exception("[DocumentDB] Databae-" + _DatabaseName + ", CollectionId-" + _CollectionId  + "exists!");
                }
            }
        }

        public void GetAllDatabase()
        {
            List<Database> databases = _Client.CreateDatabaseQuery().ToList();

            foreach (Database db in databases)
            {
                Console.WriteLine("Database id: " + db.Id + ", SelfLink: " + db.SelfLink);
            }
        }

        public void GetAllCollectionByDBId(string dbId)
        {
            Database database = _Client.CreateDatabaseQuery().Where(d => d.Id == dbId).AsEnumerable().First();
            List<DocumentCollection> collections = _Client.CreateDocumentCollectionQuery((string)database.SelfLink).ToList();

            foreach (DocumentCollection collection in collections)
            {
                Console.WriteLine("CollectionId: " + collection.Id + ", SelfLink: " + collection.SelfLink);
            }
        }

        public async Task GetCountOfMsgInCollection(string dbId, string collectionId)
        {
            DocumentCollection collection = await _Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(dbId, collectionId));
            try
            {
                var document = _Client.CreateDocumentQuery(collection.SelfLink, "SELECT c.id FROM c", new FeedOptions { EnableCrossPartitionQuery = true });
                Console.WriteLine("Count of Message :" + document.AsEnumerable().Count());
            }
            catch (Exception ex) {
            }
        }

        public void GetDatabaseAndCollectionInfo(string dbId, string collectionId)
        {
            var dbLink = UriFactory.CreateDatabaseUri(dbId);
            var task_db = _Client.ReadDatabaseAsync(dbLink);
            task_db.Wait();            
            Console.WriteLine("[Database] Quota: " + task_db.Result.DatabaseQuota + ", Usage: " + task_db.Result.DatabaseUsage);

            var collLink = UriFactory.CreateDocumentCollectionUri(dbId, collectionId);
            var task_collection = _Client.ReadDocumentCollectionAsync(collLink);
            task_collection.Wait();
            Console.WriteLine("[Collection] Quota: " + task_collection.Result.CollectionSizeQuota + "KB, Usage: " + task_collection.Result.CollectionSizeUsage + "KB");
        }
    }
}
