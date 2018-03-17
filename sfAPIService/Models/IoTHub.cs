using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class IoTHubModels
    {

        public class Detail
        {
            public string IoTHubAlias { get; set; }
            public string Description { get; set; }
            public string CompanyId { get; set; }
            public string CompanyName { get; set; }
            public string P_IoTHubEndPoint { get; set; }
            public string P_IoTHubConnectionString { get; set; }
            public string P_EventConsumerGroup { get; set; }
            public string P_EventHubStorageConnectionString { get; set; }
            public string P_UploadContainer { get; set; }
            public string S_IoTHubEndPoint { get; set; }
            public string S_IoTHubConnectionString { get; set; }
            public string S_EventConsumerGroup { get; set; }
            public string S_EventHubStorageConnectionString { get; set; }
            public string S_UploadContainer { get; set; }
        }
        public class Edit
        {
            [Required]
            public string IoTHubAlias { get; set; }
            [Required]
            public int CompanyId { get; set; }
            public string Description { get; set; }
            public string P_IoTHubEndPoint { get; set; }
            [Required]
            public string P_IoTHubConnectionString { get; set; }
            public string P_EventConsumerGroup { get; set; }
            public string P_EventHubStorageConnectionString { get; set; }
            public string P_UploadContainer { get; set; }
            public string S_IoTHubEndPoint { get; set; }
            [Required]
            public string S_IoTHubConnectionString { get; set; }
            public string S_EventConsumerGroup { get; set; }
            public string S_EventHubStorageConnectionString { get; set; }
            public string S_UploadContainer { get; set; }
        }

        public List<Detail> GetAllIoTHub()
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();

            return dbhelp.GetAll().Select(s => new Detail()
            {
                IoTHubAlias = s.IoTHubAlias,
                Description = s.Description,
                CompanyId = s.Company == null ? "" : s.Company.Id.ToString(),
                CompanyName = s.Company == null ? "" : s.Company.Name,
                P_IoTHubEndPoint = s.P_IoTHubEndPoint,
                P_IoTHubConnectionString = s.P_IoTHubConnectionString,
                P_EventConsumerGroup = s.P_EventConsumerGroup,
                P_EventHubStorageConnectionString = s.P_EventHubStorageConnectionString,
                P_UploadContainer = s.P_UploadContainer,
                S_IoTHubEndPoint = s.S_IoTHubEndPoint,
                S_IoTHubConnectionString = s.S_IoTHubConnectionString,
                S_EventConsumerGroup = s.S_EventConsumerGroup,
                S_EventHubStorageConnectionString = s.S_EventHubStorageConnectionString,
                S_UploadContainer = s.S_UploadContainer
            }).ToList<Detail>();

        }

        public List<Detail> GetAllIoTHubByCompanyId(int companyId)
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();

            return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
            {
                IoTHubAlias = s.IoTHubAlias,
                Description = s.Description,
                CompanyId = s.Company == null ? "" : s.Company.Id.ToString(),
                CompanyName = s.Company == null ? "" : s.Company.Name,
                P_IoTHubEndPoint = s.P_IoTHubEndPoint,
                P_IoTHubConnectionString = s.P_IoTHubConnectionString,
                P_EventConsumerGroup = s.P_EventConsumerGroup,
                P_EventHubStorageConnectionString = s.P_EventHubStorageConnectionString,
                P_UploadContainer = s.P_UploadContainer,
                S_IoTHubEndPoint = s.S_IoTHubEndPoint,
                S_IoTHubConnectionString = s.S_IoTHubConnectionString,
                S_EventConsumerGroup = s.S_EventConsumerGroup,
                S_EventHubStorageConnectionString = s.S_EventHubStorageConnectionString,
                S_UploadContainer = s.S_UploadContainer
            }).ToList<Detail>();

        }

        public Detail getIoTHubById(string IoTHubAlias)
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();
            IoTHub iotHub = dbhelp.GetByid(IoTHubAlias);

            return new Detail()
            {
                IoTHubAlias = iotHub.IoTHubAlias,
                Description = iotHub.Description,
                CompanyId = iotHub.Company == null ? "" : iotHub.Company.Id.ToString(),
                CompanyName = iotHub.Company == null ? "" : iotHub.Company.Name,
                P_IoTHubEndPoint = iotHub.P_IoTHubEndPoint,
                P_IoTHubConnectionString = iotHub.P_IoTHubConnectionString,
                P_EventConsumerGroup = iotHub.P_EventConsumerGroup,
                P_EventHubStorageConnectionString = iotHub.P_EventHubStorageConnectionString,
                P_UploadContainer = iotHub.P_UploadContainer,
                S_IoTHubEndPoint = iotHub.S_IoTHubEndPoint,
                S_IoTHubConnectionString = iotHub.S_IoTHubConnectionString,
                S_EventConsumerGroup = iotHub.S_EventConsumerGroup,
                S_EventHubStorageConnectionString = iotHub.S_EventHubStorageConnectionString,
                S_UploadContainer = iotHub.S_UploadContainer
            };
        }

        public void addIoTHub(Edit iotHub)
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();
            var newIoTHub = new IoTHub()
            {
                IoTHubAlias = iotHub.IoTHubAlias,
                Description = iotHub.Description,
                CompanyID = iotHub.CompanyId,
                P_IoTHubEndPoint = iotHub.P_IoTHubEndPoint,
                P_IoTHubConnectionString = iotHub.P_IoTHubConnectionString,
                P_EventConsumerGroup = iotHub.P_EventConsumerGroup,
                P_EventHubStorageConnectionString = iotHub.P_EventHubStorageConnectionString,
                P_UploadContainer = iotHub.P_UploadContainer,
                S_IoTHubEndPoint = iotHub.S_IoTHubEndPoint,
                S_IoTHubConnectionString = iotHub.S_IoTHubConnectionString,
                S_EventConsumerGroup = iotHub.S_EventConsumerGroup,
                S_EventHubStorageConnectionString = iotHub.S_EventHubStorageConnectionString,
                S_UploadContainer = iotHub.S_UploadContainer
            };
            dbhelp.Add(newIoTHub);
        }

        public void updateIoTHub(string IoTHubAlias, Edit iotHub)
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();
            IoTHub existingIoTHub = dbhelp.GetByid(IoTHubAlias);
            existingIoTHub.IoTHubAlias = iotHub.IoTHubAlias;
            existingIoTHub.Description = iotHub.Description;
            existingIoTHub.CompanyID = iotHub.CompanyId;
            existingIoTHub.P_IoTHubEndPoint = iotHub.P_IoTHubEndPoint;
            existingIoTHub.P_IoTHubConnectionString = iotHub.P_IoTHubConnectionString;
            existingIoTHub.P_EventConsumerGroup = iotHub.P_EventConsumerGroup;
            existingIoTHub.P_EventHubStorageConnectionString = iotHub.P_EventHubStorageConnectionString;
            existingIoTHub.P_UploadContainer = iotHub.P_UploadContainer;
            existingIoTHub.S_IoTHubEndPoint = iotHub.S_IoTHubEndPoint;
            existingIoTHub.S_IoTHubConnectionString = iotHub.S_IoTHubConnectionString;
            existingIoTHub.S_EventConsumerGroup = iotHub.S_EventConsumerGroup;
            existingIoTHub.S_EventHubStorageConnectionString = iotHub.S_EventHubStorageConnectionString;
            existingIoTHub.S_UploadContainer = iotHub.S_UploadContainer;

            dbhelp.Update(existingIoTHub);
        }

        public void deleteIoTHub(string IoTHubAlias)
        {
            DBHelper._IoTHub dbhelp = new DBHelper._IoTHub();
            IoTHub existingIoTHub = dbhelp.GetByid(IoTHubAlias);

            dbhelp.Delete(existingIoTHub);
        }
    }
}