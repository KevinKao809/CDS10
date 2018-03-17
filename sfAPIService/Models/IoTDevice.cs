using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace sfAPIService.Models
{
    public class IoTDeviceModels
    {
        public class Detail
        {
            public string IoTHubDeviceId { get; set; }
            public string IoTHubAlias { get; set; }
            public string IoTHubProtocol { get; set; }
            public string FactoryId { get; set; }
            public string FactoryName { get; set; }
            public string AuthenticationType { get; set; }
            public string DeviceCertificateId { get; set; }
            public string DeviceCertificateName { get; set; }
            public string DeviceTypeId { get; set; }
            public string DeviceTypeName { get; set; }
            public string DeviceVendor { get; set; }
            public string DeviceModel { get; set; }
            public string DeviceConfigurationStatus { get; set; }
        }

        public class Detail_readonly
        {
            public string IoTHubDeviceId { get; set; }
            public string IoTHubProtocol { get; set; }
            public string FactoryId { get; set; }
            public string FactoryName { get; set; }
            public string DeviceTypeId { get; set; }
            public string DeviceTypeName { get; set; }
            public string DeviceVendor { get; set; }
            public string DeviceModel { get; set; }
        }

        public class Add
        {
            [Required]
            public string IoTHubDeviceId { get; set; }
            [Required]
            public string IoTHubDevicePW { get; set; }
            public string IoTHubDeviceKey { get; set; }
            [Required]
            public string IoTHubAlias { get; set; }
            [Required]
            public string IoTHubProtocol { get; set; }
            [Required]
            public int FactoryId { get; set; }
            [Required]
            public string AuthenticationType { get; set; }
            public int DeviceCertificateId { get; set; }
            [Required]
            public int DeviceTypeId { get; set; }
            public string DeviceVendor { get; set; }
            public string DeviceModel { get; set; }
        }
        public class Update
        {
            public string IoTHubDeviceKey { get; set; }
            [Required]
            public string IoTHubAlias { get; set; }
            [Required]
            public string IoTHubProtocol { get; set; }
            [Required]
            public int FactoryId { get; set; }
            [Required]
            public string AuthenticationType { get; set; }
            public int DeviceCertificateId { get; set; }
            [Required]
            public int DeviceTypeId { get; set; }
            public string DeviceVendor { get; set; }
            public string DeviceModel { get; set; }
        }

        public class Update_Desired
        {
            public string DeviceTwinsDesired { get; set; }
        }

        public List<Detail> GetAllIoTDeviceByCompanyId(int companyId)
        {
            DBHelper._IoTDevice dbhelp_iotDevice = new DBHelper._IoTDevice();
            DBHelper._Factory dbhelp_factory = new DBHelper._Factory();
            List<int> factoryIdList = dbhelp_factory.GetAllByCompanyId(companyId).Select(s => s.Id).ToList<int>();
            List<IoTDevice> iotDeviceList = new List<IoTDevice>();

            foreach (int factoryId in factoryIdList)
            {
                iotDeviceList.AddRange(dbhelp_iotDevice.GetAllByFactory(factoryId));
            }

            return iotDeviceList.Select(s => new Detail()
            {
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTHubAlias = s.IoTHubAlias,
                IoTHubProtocol = s.IoTHubProtocol,
                FactoryId = s.FactoryID.ToString(),
                FactoryName = (s.Factory == null ? "" : s.Factory.Name),
                AuthenticationType = s.AuthenticationType,
                DeviceCertificateId = s.DeviceCertificateID.ToString(),
                DeviceCertificateName = (s.DeviceCertificate == null ? "" : s.DeviceCertificate.Name),
                DeviceTypeId = s.DeviceTypeId.ToString(),
                DeviceTypeName = (s.DeviceType == null ? "" : s.DeviceType.Name),
                DeviceVendor = s.DeviceVendor,
                DeviceModel = s.DeviceModel,
                DeviceConfigurationStatus = s.DeviceConfigurationStatus.ToString()
            }).ToList<Detail>();

        }

        public List<Detail_readonly> GetAllIoTDeviceByCompanyIdReadonly(int companyId)
        {
            DBHelper._IoTDevice dbhelp_iotdevice = new DBHelper._IoTDevice();
            DBHelper._Factory dbhelp_factory = new DBHelper._Factory();
            List<IoTDevice> iotDeviceList = new List<IoTDevice>();
            List<int> factoryIdList = dbhelp_factory.GetAllByCompanyId(companyId).Select(s => s.Id).ToList<int>();

            foreach (int factoryId in factoryIdList)
            {
                iotDeviceList.AddRange(dbhelp_iotdevice.GetAllByFactory(factoryId));
            }
            return iotDeviceList.Select(s => new Detail_readonly()
            {
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTHubProtocol = s.IoTHubProtocol,
                FactoryId = s.FactoryID.ToString(),
                FactoryName = s.Factory.Name,
                DeviceTypeId = s.DeviceTypeId.ToString(),
                DeviceTypeName = s.DeviceType.Name,
                DeviceVendor = s.DeviceVendor,
                DeviceModel = s.DeviceModel
            }).ToList<Detail_readonly>();
        }

        public Detail_readonly getIoTDeviceByIdReadonly(string iotDeviceId)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            IoTDevice iotDevice = dbhelp.GetByid(iotDeviceId);

            return new Detail_readonly()
            {
                IoTHubDeviceId = iotDevice.IoTHubDeviceID,
                IoTHubProtocol = iotDevice.IoTHubProtocol,
                FactoryId = iotDevice.FactoryID.ToString(),
                FactoryName = iotDevice.Factory.Name,
                DeviceTypeId = iotDevice.DeviceTypeId.ToString(),
                DeviceTypeName = iotDevice.DeviceType.Name,
                DeviceVendor = iotDevice.DeviceVendor,
                DeviceModel = iotDevice.DeviceModel
            };
        }

        public List<Detail> GetAllIoTDeviceByFactoryId(int factoryId)
        {
            DBHelper._IoTDevice dbhelp_iotDevice = new DBHelper._IoTDevice();

            return dbhelp_iotDevice.GetAllByFactory(factoryId).Select(s => new Detail()
            {
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTHubAlias = s.IoTHubAlias,
                IoTHubProtocol = s.IoTHubProtocol,
                FactoryId = s.FactoryID.ToString(),
                FactoryName = (s.Factory == null ? "" : s.Factory.Name),
                AuthenticationType = s.AuthenticationType,
                DeviceCertificateId = s.DeviceCertificateID.ToString(),
                DeviceCertificateName = (s.DeviceCertificate == null ? "" : s.DeviceCertificate.Name),
                DeviceTypeId = s.DeviceTypeId.ToString(),
                DeviceTypeName = (s.DeviceType == null ? "" : s.DeviceType.Name),
                DeviceVendor = s.DeviceVendor,
                DeviceModel = s.DeviceModel,
                DeviceConfigurationStatus = s.DeviceConfigurationStatus.ToString()
            }).ToList<Detail>();
        }

        public List<Detail_readonly> GetAllIoTDeviceByFactoryIdReadonly(int factoryId)
        {
            DBHelper._IoTDevice dbhelp_iotDevice = new DBHelper._IoTDevice();

            return dbhelp_iotDevice.GetAllByFactory(factoryId).Select(s => new Detail_readonly()
            {
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTHubProtocol = s.IoTHubProtocol,
                FactoryId = s.FactoryID.ToString(),
                FactoryName = s.Factory.Name,
                DeviceTypeId = s.DeviceTypeId.ToString(),
                DeviceTypeName = s.DeviceType.Name,
                DeviceVendor = s.DeviceVendor,
                DeviceModel = s.DeviceModel
            }).ToList<Detail_readonly>();
        }

        public Detail getIoTDeviceById(string iotHubDeviceId)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            IoTDevice iotDevice = dbhelp.GetByid(iotHubDeviceId);

            return new Detail()
            {
                IoTHubDeviceId = iotDevice.IoTHubDeviceID,
                IoTHubAlias = iotDevice.IoTHubAlias,
                IoTHubProtocol = iotDevice.IoTHubProtocol,
                FactoryId = iotDevice.Factory.Id.ToString(),
                FactoryName = (iotDevice.Factory == null ? "" : iotDevice.Factory.Name),
                AuthenticationType = iotDevice.AuthenticationType,
                DeviceCertificateId = iotDevice.DeviceCertificateID.ToString(),
                DeviceCertificateName = (iotDevice.DeviceCertificate == null ? "" : iotDevice.DeviceCertificate.Name),
                DeviceTypeId = iotDevice.DeviceTypeId.ToString(),
                DeviceTypeName = (iotDevice.DeviceType == null ? "" : iotDevice.DeviceType.Name),
                DeviceVendor = iotDevice.DeviceVendor,
                DeviceModel = iotDevice.DeviceModel,
                DeviceConfigurationStatus = iotDevice.DeviceConfigurationStatus.ToString()
            };
        }

        public void addIoTDevice(Add iotDevice)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            var newIoTDevice = new IoTDevice()
            {
                IoTHubDeviceID = iotDevice.IoTHubDeviceId,
                IoTHubDevicePW = Crypto.HashPassword(iotDevice.IoTHubDevicePW),
                IoTHubDeviceKey = iotDevice.IoTHubDeviceKey,
                IoTHubAlias = iotDevice.IoTHubAlias,
                IoTHubProtocol = iotDevice.IoTHubProtocol,
                FactoryID = iotDevice.FactoryId,
                AuthenticationType = iotDevice.AuthenticationType,
                DeviceCertificateID = (iotDevice.DeviceCertificateId == 0) ? (int?)null : iotDevice.DeviceCertificateId,
                DeviceTypeId = iotDevice.DeviceTypeId,
                DeviceVendor = iotDevice.DeviceVendor,
                DeviceModel = iotDevice.DeviceModel,
                DeviceTwinsDesired = "{\"SF_CustomizedConfig\":{},\"SF_SystemConfig\":{}}",
                DeviceTwinsReported = "{\"SF_CustomizedConfig\":{},\"SF_SystemConfig\":{},\"SF_LastUpdatedTimestamp\":0}"
            };
            dbhelp.Add(newIoTDevice);
        }

        public void updateIoTDevice(string iotHubDeviceId, Update iotDevice)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            IoTDevice existingIoTDevice = dbhelp.GetByid(iotHubDeviceId);
            existingIoTDevice.IoTHubAlias = iotDevice.IoTHubAlias;
            existingIoTDevice.IoTHubProtocol = iotDevice.IoTHubProtocol;
            existingIoTDevice.FactoryID = iotDevice.FactoryId;
            existingIoTDevice.AuthenticationType = iotDevice.AuthenticationType;
            existingIoTDevice.DeviceCertificateID = (iotDevice.DeviceCertificateId == 0) ? (int?)null : iotDevice.DeviceCertificateId;
            existingIoTDevice.DeviceTypeId = iotDevice.DeviceTypeId;
            existingIoTDevice.DeviceVendor = iotDevice.DeviceVendor;
            existingIoTDevice.DeviceModel = iotDevice.DeviceModel;

            dbhelp.Update(existingIoTDevice);
        }

        public void deleteIoTDevice(string iotHubDeviceId)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            IoTDevice existingIoTDevice = dbhelp.GetByid(iotHubDeviceId);

            dbhelp.Delete(existingIoTDevice);
        }

        public int GetIoTDeviceCompanyId(string iotHubDeviceId)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            IoTDevice existingIoTDevice = dbhelp.GetByid(iotHubDeviceId);

            return existingIoTDevice.Factory.CompanyId;
        }

        public void updateIoTDeviceDesired(string iotHubDeviceId, JObject desiredProperty)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            dbhelp.UpdateDeviceConfigurationStatusAndProperty(iotHubDeviceId, 0, JsonConvert.SerializeObject(desiredProperty));
        }
    }
}