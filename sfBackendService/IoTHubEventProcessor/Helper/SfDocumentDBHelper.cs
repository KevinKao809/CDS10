using Microsoft.Azure.Documents.Client;
using System;

namespace IoTHubEventProcessor
{
    public class SfDocumentDBHelper
    {
        public int CompanyId { get; set; }
        public DocumentClient DocumentClient { get; set; }
        public string DocumentDbDatabaseName { get; set; }
        public string DocumentDbCollectionName { get; set; }

        public SfDocumentDBHelper(DocumentClient documentClient, int companyId)
        {
            this.CompanyId = companyId;
            this.DocumentClient = documentClient;
            this.DocumentDbDatabaseName = getDocumentDbName(companyId);
            this.DocumentDbCollectionName = getDocumentDbCollectionName(companyId);
        }

        private string getDocumentDbName(int companyId)
        {
            return "db" + companyId;
        }

        private string getDocumentDbCollectionName(int companyId)
        {
            return companyId.ToString();
        }

        public static string CombineDocumentName(string deviceId, string messageCatalogId)
        {
            return deviceId + "_" + messageCatalogId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }
    }
}
