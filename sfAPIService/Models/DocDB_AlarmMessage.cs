using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using sfShareLib;
using System.Configuration;

namespace sfAPIService.Models
{
    public class DocDB_AlarmMessageModels
    {
        private string _DBName;
        private string _CollecitonName;
        private string _ConnectionString;
        private Uri _CollectionLink;
        private DocumentClient _Client;

        public DocDB_AlarmMessageModels(int companyId)
        {
            SFDatabaseEntities dbEnty = new SFDatabaseEntities();
            _ConnectionString = (from c in dbEnty.Company
                             where c.Id == companyId
                             select c.DocDBConnectionString).Single<string>();
            if (string.IsNullOrEmpty(_ConnectionString))
                _ConnectionString = ConfigurationManager.AppSettings["sfDocDBConnectionString"];

            _DBName = "db" + companyId;
            _CollecitonName = companyId.ToString();
            _CollectionLink = UriFactory.CreateDocumentCollectionUri(_DBName, _CollecitonName);

            try
            {
                //init DocumentClient
                _ConnectionString = _ConnectionString.Replace("AccountEndpoint=", "");
                _ConnectionString = _ConnectionString.Replace(";", "");
                _ConnectionString = _ConnectionString.Replace("AccountKey=", ";");
                string endpointUri = _ConnectionString.Split(';')[0];
                string primaryKey = _ConnectionString.Split(';')[1];
                _Client = new DocumentClient(new Uri(endpointUri), primaryKey);
            }
            catch (Exception ex)
            {
                throw new Exception("Initial DocumentClient failed: " + ex.Message);
            }
        }
        public dynamic GetByCompanyId(int companyId, int top, int hours, string order)
        {
            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int queryUnixTimestamp = unixTimestamp - (hours * 60 * 60);
            string sql = "";
            
            if(order.ToLower().Equals("desc"))
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm' AND c.AlarmSent = true AND c._ts > " + queryUnixTimestamp + " ORDER BY c._ts DESC";
            else
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm' AND c.AlarmSent = true AND c._ts > " + queryUnixTimestamp + " ORDER BY c._ts";

            return _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true});
        }

        public dynamic GetByFactoryId(int factoryId, int top, int hours, string order)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            List<string> equipmentIdList = dbhelp.GetAllByFactoryId(factoryId).Select(s => s.EquipmentId).ToList<string>();

            int maxIndex = equipmentIdList.Count - 1;
            string sql_equipmentId = "";
            for (int i = 0; i <= maxIndex; i++)
            {
                if (i == maxIndex)
                    sql_equipmentId += ("'" + equipmentIdList[i] + "'");
                else
                    sql_equipmentId += ("'" + equipmentIdList[i] + "',");
            }

            if (string.IsNullOrEmpty(sql_equipmentId))
                return null;
            else
            {
                int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                int queryUnixTimestamp = unixTimestamp - (hours * 60 * 60);
                string sql = "";

                if (order.ToLower().Equals("desc"))
                    sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm' AND c.AlarmSent = true AND c.Message.equipmentId IN (" + sql_equipmentId + ") AND c._ts > " + queryUnixTimestamp + " ORDER BY c._ts DESC";
                else
                    sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm' AND c.AlarmSent = true AND c.Message.equipmentId IN (" + sql_equipmentId + ") AND c._ts > " + queryUnixTimestamp;

                return _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true });
            }
        }

        public dynamic GetByEquipmentId(string equipmentId, int top, int hours, string order)
        {
            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int queryUnixTimestamp = unixTimestamp - (hours * 60 * 60);
            string sql = "";

            if (order.ToLower().Equals("desc"))
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm' AND c.AlarmSent = true AND c._ts > " + queryUnixTimestamp + " AND c.Message.equipmentId='" + equipmentId + "' ORDER BY c._ts DESC";
            else
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm' AND c.AlarmSent = true AND c._ts > " + queryUnixTimestamp + " AND c.Message.equipmentId='" + equipmentId + "'";
            
            return _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true });
        }
        
    }
}