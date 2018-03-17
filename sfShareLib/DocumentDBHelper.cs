using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using sfShareLib;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace sfShareLib
{
    public class DocumentDBHelper
    {
        public string _ConnectionString;
        public string _DatabaseName;
        public string _CollectionId;
        public string _Action;
        public int _TaskId;
        private Uri _CollectionLink;
        private DocumentClient _Client;

        public long _SizeQuotaInKB;
        public long _SizeUsageInKB;

        public DocumentDBHelper(int companyId, string connectionString)
        {
            SFDatabaseEntities dbEnty = new SFDatabaseEntities();
            _ConnectionString = connectionString;

            _DatabaseName = "db" + companyId;
            _CollectionId = companyId.ToString();
            _CollectionLink = UriFactory.CreateDocumentCollectionUri(_DatabaseName, _CollectionId);

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

        public void Init()
        {
            var task = _Client.ReadDocumentCollectionAsync(_CollectionLink, new RequestOptions { PopulateQuotaInfo = true });
            task.Wait();
            Microsoft.Azure.Documents.DocumentCollection response = task.Result;
            _SizeQuotaInKB = Convert.ToInt32(task.Result.CollectionSizeQuota.ToString());
            _SizeUsageInKB = Convert.ToInt32(task.Result.CollectionSizeUsage.ToString());
        }

        public long GetCompanyDeviceMsgQty()
        {
            string sql = "SELECT VALUE COUNT(1) FROM c WHERE c.Type = 'Message'";
            dynamic result = _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true });
            foreach (long count in result)
            {
                return count;
            }
            return 0;
        }
        public long GetCompanyAlarmMessageQty()
        {
            string sql = "SELECT VALUE COUNT(1) FROM c WHERE c.Type = 'Alarm' AND c.AlarmSent = true";
            dynamic result = _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true });
            foreach (long count in result)
            {
                return count;
            }
            return 0;
        }
        public dynamic GetAlarmMessageByCompanyId(int companyId, int top, int hours, string order)
        {
            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int queryUnixTimestamp = unixTimestamp - (hours * 60 * 60);
            string sql = "";

            if (order.ToLower().Equals("desc"))
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm' AND c.AlarmSent = true AND c._ts > " + queryUnixTimestamp + " ORDER BY c._ts DESC";
            else
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm' AND c.AlarmSent = true AND c._ts > " + queryUnixTimestamp + " ORDER BY c._ts";

            return _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true });
        }

        public dynamic GetAlarmMessageByFactoryId(int factoryId, int top, int hours, string order, int companyId = 0)
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
                return new object();
            else
            {
                int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                int queryUnixTimestamp = unixTimestamp - (hours * 60 * 60);
                string sql = "";
                string checkCompanyIdSql = companyId > 0 ? " AND c.Message.companyId = " + companyId.ToString() + " " : " ";

                if (order.ToLower().Equals("desc"))
                    sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm'" + checkCompanyIdSql + "AND c.AlarmSent = true AND c.Message.equipmentId IN (" + sql_equipmentId + ") AND c._ts > " + queryUnixTimestamp + " ORDER BY c._ts DESC";
                else
                    sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm'" + checkCompanyIdSql + "AND c.AlarmSent = true AND c.Message.equipmentId IN (" + sql_equipmentId + ") AND c._ts > " + queryUnixTimestamp;

                return _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true });
            }
        }

        public dynamic GetAlarmMessageByEquipmentId(string equipmentId, int top, int hours, string order, int companyId = 0)
        {
            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int queryUnixTimestamp = unixTimestamp - (hours * 60 * 60);
            string sql = "";
            string checkCompanyIdSql = companyId > 0 ? " AND c.Message.companyId = " + companyId.ToString() + " " : " ";
            
            if (order.ToLower().Equals("desc"))
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm'" + checkCompanyIdSql + "AND c.AlarmSent = true AND c._ts > " + queryUnixTimestamp + " AND c.Message.equipmentId='" + equipmentId + "' ORDER BY c._ts DESC";
            else
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Alarm'" + checkCompanyIdSql + "AND c.AlarmSent = true AND c._ts > " + queryUnixTimestamp + " AND c.Message.equipmentId='" + equipmentId + "'";

            return _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true });
        }

        public dynamic GetMessageByCompanyId(int companyId, int top, int hours, string order)
        {
            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int queryUnixTimestamp = unixTimestamp - (hours * 60 * 60);
            string sql = "";

            if (order.ToLower().Equals("desc"))
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Message' AND c._ts > " + queryUnixTimestamp + " ORDER BY c._ts DESC";
            else
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Message' AND c._ts > " + queryUnixTimestamp + " ORDER BY c._ts";

            return _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true });
        }

        public dynamic GetMessageByFactoryId(int factoryId, int top, int hours, string order, int companyId = 0)
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
                return new Object();
            else
            {
                int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                int queryUnixTimestamp = unixTimestamp - (hours * 60 * 60);
                string sql = "";
                string checkCompanyIdSql = companyId > 0 ? " AND c.Message.companyId = " + companyId.ToString() + " " : " " ;

                if (order.ToLower().Equals("desc"))
                    sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Message'" + checkCompanyIdSql + "AND c.Message.equipmentId IN (" + sql_equipmentId + ") AND c._ts > " + queryUnixTimestamp + " ORDER BY c._ts DESC";
                else
                    sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Message'" + checkCompanyIdSql + " AND c.Message.equipmentId IN (" + sql_equipmentId + ") AND c._ts > " + queryUnixTimestamp;

                return _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true });
            }
        }

        public dynamic GetMessageByEquipmentId(string equipmentId, int top, int hours, string order, int companyId = 0)
        {
            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int queryUnixTimestamp = unixTimestamp - (hours * 60 * 60);
            string sql = "";
            string checkCompanyIdSql = companyId > 0 ? " AND c.Message.companyId = " + companyId.ToString() + " " : " ";

            if (order.ToLower().Equals("desc"))
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Message'" + checkCompanyIdSql + " AND c._ts > " + queryUnixTimestamp + " AND c.Message.equipmentId='" + equipmentId + "' ORDER BY c._ts DESC";
            else
                sql = "SELECT TOP " + top + " * FROM c WHERE c.Type = 'Message'" + checkCompanyIdSql + " AND c._ts > " + queryUnixTimestamp + " AND c.Message.equipmentId='" + equipmentId + "'";

            return _Client.CreateDocumentQuery(_CollectionLink, sql, new FeedOptions { EnableCrossPartitionQuery = true });
        }
    }
}
