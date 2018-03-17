using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfShareLib
{
    public class DBHelper
    {
        public class _Company
        {
            public List<Company> GetAll()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Company.AsNoTracking()
                             where c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<Company>();
            }

            public List<Company> GetAllBySuperAdmin()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Company.AsNoTracking()
                             select c;
                return L2Enty.ToList<Company>();
            }

            public Company GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Company.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Company company)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                company.DeletedFlag = false;
                company.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.Company.Add(company);
                dbEntity.SaveChanges();
                return company.Id;
            }

            public void Update(Company company)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                company.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.Company.Attach(company);
                dbEntity.Entry(company).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Company company)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                company.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                company.DeletedFlag = true;
                dbEntity.Company.Attach(company);
                dbEntity.Entry(company).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _Employee
        {
            public List<Employee> GetAll()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Employee.AsNoTracking()
                             where c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<Employee>();
            }

            public List<Employee> GetAllBySuperAdmin()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Employee.AsNoTracking()
                             select c;
                return L2Enty.ToList<Employee>();
            }

            public List<Employee> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Employee.AsNoTracking()
                             where c.CompanyId == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<Employee>();
            }

            public Employee GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Employee.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Employee employee)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                employee.DeletedFlag = false;
                employee.CreatedAt = DateTime.Parse(DateTime.Now.ToString());
                dbEntity.Employee.Add(employee);
                dbEntity.SaveChanges();
                return employee.Id;
            }

            public void Update(Employee employee)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                employee.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.Employee.Attach(employee);
                dbEntity.Entry(employee).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Employee employee)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                employee.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                employee.DeletedFlag = true;
                dbEntity.Employee.Attach(employee);
                dbEntity.Entry(employee).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _Factory
        {
            public List<Factory> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Factory.AsNoTracking()
                             where c.CompanyId == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<Factory>();
            }

            public Factory GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Factory.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public Factory GetByid(int id, int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Factory.AsNoTracking()
                             where c.Id == id && c.CompanyId == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Factory factory)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                factory.DeletedFlag = false;
                factory.CreatedAt = DateTime.Parse(DateTime.Now.ToString());
                dbEntity.Factory.Add(factory);
                dbEntity.SaveChanges();
                return factory.Id;
            }

            public void Update(Factory factory)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                factory.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.Factory.Attach(factory);
                dbEntity.Entry(factory).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Factory factory)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                factory.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                factory.DeletedFlag = true;
                dbEntity.Factory.Attach(factory);
                dbEntity.Entry(factory).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _EmployeeInRole
        {
            public List<EmployeeInRole> GetAllByUserRoleId(int userRoleId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.EmployeeInRole.AsNoTracking()
                             where c.UserRoleID == userRoleId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<EmployeeInRole>();
            }

            public List<EmployeeInRole> GetAllByEmployeeId(int employeeId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.EmployeeInRole.AsNoTracking()
                             where c.EmployeeID == employeeId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<EmployeeInRole>();
            }

            public EmployeeInRole GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.EmployeeInRole.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(EmployeeInRole employeeInRole)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                employeeInRole.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.EmployeeInRole.Add(employeeInRole);
                dbEntity.SaveChanges();
                return employeeInRole.Id;
            }

            public void AddManyRows(List<EmployeeInRole> employeeInRoleList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (EmployeeInRole employeeInRole in employeeInRoleList)
                {
                    employeeInRole.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                    dbEntity.EmployeeInRole.Add(employeeInRole);
                }
                dbEntity.SaveChanges();
            }

            public void Update(EmployeeInRole employeeInRole)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                employeeInRole.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.EmployeeInRole.Attach(employeeInRole);
                dbEntity.Entry(employeeInRole).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }
            public void UpdateManyRows(List<EmployeeInRole> employeeInRoleList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (EmployeeInRole employeeInRole in employeeInRoleList)
                {
                    employeeInRole.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                    dbEntity.EmployeeInRole.Attach(employeeInRole);
                    dbEntity.Entry(employeeInRole).State = System.Data.Entity.EntityState.Modified;
                }
                dbEntity.SaveChanges();
            }

            public void Delete(EmployeeInRole employeeInRole)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                employeeInRole.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                employeeInRole.DeletedFlag = true;
                dbEntity.EmployeeInRole.Attach(employeeInRole);
                dbEntity.Entry(employeeInRole).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(List<EmployeeInRole> employeeInRoleList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (EmployeeInRole employeeInRole in employeeInRoleList)
                {
                    employeeInRole.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                    employeeInRole.DeletedFlag = true;
                    dbEntity.EmployeeInRole.Attach(employeeInRole);
                    dbEntity.Entry(employeeInRole).State = System.Data.Entity.EntityState.Modified;
                }

                dbEntity.SaveChanges();
            }

        }

        public class _IoTDevice
        {
            public List<IoTDevice> GetAllByIoTHubAlias(string iotHubAlias)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.IoTHubAlias == iotHubAlias && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<IoTDevice>();
            }

            public List<IoTDevice> GetAllByFactory(int factoryId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.FactoryID == factoryId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<IoTDevice>();
            }

            public IoTDevice GetByid(string iotHubDeviceId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTDevice.AsNoTracking()
                             where c.IoTHubDeviceID == iotHubDeviceId && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int GetCompanyId(string iotHubDeviceId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                int companyId = (from c in dbEntity.IoTDevice.AsNoTracking()
                                 where c.IoTHubDeviceID == iotHubDeviceId
                                 select c.Factory.CompanyId).FirstOrDefault();

                return companyId;
            }
            public void Add(IoTDevice iotDevice)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                iotDevice.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.IoTDevice.Add(iotDevice);
                dbEntity.SaveChanges();
            }

            public void Update(IoTDevice iotDevice)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                iotDevice.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.IoTDevice.Attach(iotDevice);
                dbEntity.Entry(iotDevice).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void UpdateDeviceConfigurationStatusAndProperty(string iotDeviceId, int status, string desiredProperty = null, string reportedProeperty = null)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                IoTDevice iotDevice = dbEntity.IoTDevice.Find(iotDeviceId);
                iotDevice.DeviceConfigurationStatus = status;

                if (!string.IsNullOrEmpty(desiredProperty))
                    iotDevice.DeviceTwinsDesired = desiredProperty;

                if (!string.IsNullOrEmpty(reportedProeperty))
                    iotDevice.DeviceTwinsReported = reportedProeperty;

                iotDevice.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.Entry(iotDevice).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(IoTDevice iotDevice)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                iotDevice.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                iotDevice.DeletedFlag = true;
                dbEntity.IoTDevice.Attach(iotDevice);
                dbEntity.Entry(iotDevice).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _IoTDeviceConfiguration
        {
            public List<IoTDeviceConfiguration> GetAll()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTDeviceConfiguration.AsNoTracking()
                             select c;
                return L2Enty.ToList<IoTDeviceConfiguration>();
            }

            public IoTDeviceConfiguration GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTDeviceConfiguration.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(IoTDeviceConfiguration ioTDeviceConfiguration)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.IoTDeviceConfiguration.Add(ioTDeviceConfiguration);
                dbEntity.SaveChanges();
                return ioTDeviceConfiguration.Id;
            }

            public void Update(IoTDeviceConfiguration ioTDeviceConfiguration)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.IoTDeviceConfiguration.Attach(ioTDeviceConfiguration);
                dbEntity.Entry(ioTDeviceConfiguration).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(IoTDeviceConfiguration ioTDeviceConfiguration)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.IoTDeviceConfiguration.Attach(ioTDeviceConfiguration);
                dbEntity.Entry(ioTDeviceConfiguration).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

        }

        public class _IoTDeviceCustomizedConfiguration
        {
            public List<IoTDeviceCustomizedConfiguration> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTDeviceCustomizedConfiguration.AsNoTracking()
                             where c.CompanyId == companyId & c.DeleteFlag == false
                             select c;
                return L2Enty.ToList<IoTDeviceCustomizedConfiguration>();
            }

            public IoTDeviceCustomizedConfiguration GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTDeviceCustomizedConfiguration.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(IoTDeviceCustomizedConfiguration customizedConfig)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                customizedConfig.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                customizedConfig.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.IoTDeviceCustomizedConfiguration.Add(customizedConfig);
                dbEntity.SaveChanges();
                return customizedConfig.Id;
            }

            public void Update(IoTDeviceCustomizedConfiguration customizedConfig)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                customizedConfig.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.IoTDeviceCustomizedConfiguration.Attach(customizedConfig);
                dbEntity.Entry(customizedConfig).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(IoTDeviceCustomizedConfiguration customizedConfig)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                customizedConfig.DeleteFlag = true;
                customizedConfig.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.IoTDeviceCustomizedConfiguration.Attach(customizedConfig);
                dbEntity.Entry(customizedConfig).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }
        }

        public class _IoTDeviceMessageCatalog
        {
            public List<IoTDeviceMessageCatalog> GetAllByIoTDeviceId(string iotDeviceId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTDeviceMessageCatalog.AsNoTracking()
                             where c.IoTHubDeviceID == iotDeviceId
                             select c;
                return L2Enty.ToList<IoTDeviceMessageCatalog>();
            }

            public IoTDeviceMessageCatalog GetById(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTDeviceMessageCatalog.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault<IoTDeviceMessageCatalog>();
            }

            public void Add(List<IoTDeviceMessageCatalog> iotDMCList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (IoTDeviceMessageCatalog iotDMC in iotDMCList)
                {
                    iotDMC.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                    dbEntity.IoTDeviceMessageCatalog.Add(iotDMC);
                }
                dbEntity.SaveChanges();
            }

            public void Delete(List<IoTDeviceMessageCatalog> iotDMCList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (IoTDeviceMessageCatalog iotDMC in iotDMCList)
                {
                    dbEntity.IoTDeviceMessageCatalog.Attach(iotDMC);
                    dbEntity.Entry(iotDMC).State = System.Data.Entity.EntityState.Deleted;
                }

                dbEntity.SaveChanges();
            }

        }

        public class _MetaDataDefination
        {
            public List<MetaDataDefination> GetAllByCompanyIdAndEntityType(int companyId, string entityType)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.MetaDataDefination.AsNoTracking()
                             where c.CompanyId == companyId && c.EntityType == entityType
                             select c;
                return L2Enty.ToList<MetaDataDefination>();
            }

            public MetaDataDefination GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.MetaDataDefination.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(MetaDataDefination metaDataDefination)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();                
                dbEntity.MetaDataDefination.Add(metaDataDefination);
                dbEntity.SaveChanges();
                return metaDataDefination.Id;
            }

            public void Update(MetaDataDefination metaDataDefination)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.MetaDataDefination.Attach(metaDataDefination);
                dbEntity.Entry(metaDataDefination).State = System.Data.Entity.EntityState.Modified;
                dbEntity.SaveChanges();
            }

            public void Delete(MetaDataDefination metaDataDefination)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.Entry(metaDataDefination).State = System.Data.Entity.EntityState.Deleted;
                dbEntity.SaveChanges();
            }
        }
        
        public class _DeviceCertificate
        {
            public List<DeviceCertificate> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.DeviceCertificate.AsNoTracking()
                             where c.CompanyID == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<DeviceCertificate>();
            }

            public DeviceCertificate GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.DeviceCertificate.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(DeviceCertificate deviceCertificate)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                deviceCertificate.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.DeviceCertificate.Add(deviceCertificate);
                dbEntity.SaveChanges();
                return deviceCertificate.Id;
            }

            public void Update(DeviceCertificate deviceCertificate)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                deviceCertificate.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.DeviceCertificate.Attach(deviceCertificate);
                dbEntity.Entry(deviceCertificate).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(DeviceCertificate deviceCertificate)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                deviceCertificate.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                deviceCertificate.DeletedFlag = true;
                dbEntity.DeviceCertificate.Attach(deviceCertificate);
                dbEntity.Entry(deviceCertificate).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _DeviceType
        {
            public List<DeviceType> GetAll()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.DeviceType.AsNoTracking()
                             where c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<DeviceType>();
            }

            public List<DeviceType> GetAllBySuperAdmin()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.DeviceType.AsNoTracking()
                             select c;
                return L2Enty.ToList<DeviceType>();
            }

            public DeviceType GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.DeviceType.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(DeviceType deviceType)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                deviceType.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.DeviceType.Add(deviceType);
                dbEntity.SaveChanges();
                return deviceType.Id;
            }

            public void Update(DeviceType deviceType)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                deviceType.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.DeviceType.Attach(deviceType);
                dbEntity.Entry(deviceType).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(DeviceType deviceType)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                deviceType.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                deviceType.DeletedFlag = true;
                dbEntity.DeviceType.Attach(deviceType);
                dbEntity.Entry(deviceType).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _IoTHub
        {
            public List<IoTHub> GetAll()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTHub.AsNoTracking()
                             where c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<IoTHub>();
            }
            public List<IoTHub> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTHub.AsNoTracking()
                             where c.CompanyID == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<IoTHub>();
            }

            public IoTHub GetByid(string id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.IoTHub.AsNoTracking()
                             where c.IoTHubAlias == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public string Add(IoTHub iotHub)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                iotHub.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.IoTHub.Add(iotHub);
                dbEntity.SaveChanges();
                return iotHub.IoTHubAlias;
            }

            public void Update(IoTHub iotHub)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                iotHub.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.IoTHub.Attach(iotHub);
                dbEntity.Entry(iotHub).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(IoTHub iotHub)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                iotHub.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                iotHub.DeletedFlag = true;
                dbEntity.IoTHub.Attach(iotHub);
                dbEntity.Entry(iotHub).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _Equipment
        {
            public List<Equipment> GetAll()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<Equipment>();
            }

            public List<Equipment> GetAllByFactoryId(int factoryId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.FactoryId == factoryId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<Equipment>();
            }

            public List<Equipment> GetAllByIoTHubDeviceId(string iotHubDeviceId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.IoTHubDeviceID == iotHubDeviceId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<Equipment>();
            }

            public List<MetaDataValue> GetMetaDataById(int id, int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var dataDefEntry = from dataDef in dbEntity.MetaDataDefination
                                   where dataDef.CompanyId == companyId
                                   select dataDef;
                var dataVEntry = from dataV in dbEntity.MetaDataValue
                                 where dataV.ReferenceId == id
                                 select dataV;

                List < MetaDataValue > metaDatas = new List<MetaDataValue>();
                foreach (var dataD in dataDefEntry)
                {
                    MetaDataValue metaData = new MetaDataValue();
                    metaData.MetaDataDefinationId = dataD.Id;
                    metaData.MetaDataDefination = new MetaDataDefination();
                    metaData.MetaDataDefination.ObjectName = dataD.ObjectName;

                    foreach(var dataV in dataVEntry)
                    {
                        if (dataV.MetaDataDefinationId == dataD.Id)
                        {
                            metaData.Id = dataV.Id;
                            metaData.ObjectValue = dataV.ObjectValue;
                            break;
                        }
                    }
                    metaDatas.Add(metaData);
                }


                //var L2Enty = from dataDef in dbEntity.MetaDataDefination
                //             where dataDef.CompanyId == companyId
                //             join dataV in dbEntity.MetaDataValue.Where(o => o.ReferenceId == id)
                //                 on dataDef equals dataV.MetaDataDefination into groupJoin                             
                //             from subDataV in groupJoin.DefaultIfEmpty()
                //             select new { dataDef.ObjectName, subDataV.Id, subDataV.MetaDataDefinationId, ObjectValue = subDataV.ObjectValue ?? String.Empty };

                //var L2Enty = from dataV in dbEntity.MetaDataValue
                //             join dataDef in dbEntity.MetaDataDefination on dataV.MetaDataDefinationId equals dataDef.Id
                //             where dataDef.EntityType == "equipment" && dataV.ReferenceId == id
                //             select dataV;                

                //List<MetaDataValue> metaDatas = new List<MetaDataValue>();
                //foreach (var v in L2Enty)
                //{
                //    MetaDataValue metaData = new MetaDataValue();
                //    metaData.Id = v.Id;
                //    metaData.MetaDataDefinationId = v.MetaDataDefinationId;
                //    metaData.MetaDataDefination = new MetaDataDefination();
                //    metaData.MetaDataDefination.ObjectName = v.ObjectName;
                //    metaData.ObjectValue = v.ObjectValue;
                //    metaDatas.Add(metaData);
                //}
                return metaDatas;
            }

            public void UpdateMetaDataById(int id, List<MetaDataValue> metaDataList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (var metaData in metaDataList)
                {
                    if (metaData.Id == 0)
                    {
                        dbEntity.MetaDataValue.Add(metaData);                        
                    }
                    else
                    {
                        dbEntity.MetaDataValue.Attach(metaData);
                        dbEntity.Entry(metaData).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                dbEntity.SaveChanges();
            }

            public Equipment GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public Equipment GetByid(string equipmentId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Equipment.AsNoTracking()
                             where c.EquipmentId == equipmentId && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Equipment equipment)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                equipment.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.Equipment.Add(equipment);
                dbEntity.SaveChanges();
                return equipment.Id;
            }

            public void Update(Equipment equipment)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                equipment.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.Equipment.Attach(equipment);
                dbEntity.Entry(equipment).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Equipment equipment)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                equipment.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                equipment.DeletedFlag = true;
                dbEntity.Equipment.Attach(equipment);
                dbEntity.Entry(equipment).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public int GetCompanyId(string equipmentId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                int companyId = (from c in dbEntity.Equipment
                                 where c.EquipmentId == equipmentId
                                 select c.Factory.CompanyId).Single<int>();
                return companyId;
            }
        }

        public class _EquipmentClass
        {
            public List<EquipmentClass> GetAll()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.EquipmentClass.AsNoTracking()
                             where c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<EquipmentClass>();
            }

            public List<EquipmentClass> GetAllBySuperAdmin()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.EquipmentClass.AsNoTracking()
                             select c;
                return L2Enty.ToList<EquipmentClass>();
            }

            public EquipmentClass GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.EquipmentClass.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(EquipmentClass equipmentClass)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                equipmentClass.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.EquipmentClass.Add(equipmentClass);
                dbEntity.SaveChanges();
                return equipmentClass.Id;
            }

            public void Update(EquipmentClass equipmentClass)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                equipmentClass.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.EquipmentClass.Attach(equipmentClass);
                dbEntity.Entry(equipmentClass).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(EquipmentClass equipmentClass)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                equipmentClass.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                equipmentClass.DeletedFlag = true;
                dbEntity.EquipmentClass.Attach(equipmentClass);
                dbEntity.Entry(equipmentClass).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _UserRole
        {
            public List<UserRole> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.UserRole.AsNoTracking()
                             where c.CompanyId == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<UserRole>();
            }

            public UserRole GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.UserRole.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(UserRole userRole)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                userRole.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.UserRole.Add(userRole);
                dbEntity.SaveChanges();
                return userRole.Id;
            }

            public void Update(UserRole userRole)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                userRole.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.UserRole.Attach(userRole);
                dbEntity.Entry(userRole).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(UserRole userRole)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                userRole.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                userRole.DeletedFlag = true;
                dbEntity.UserRole.Attach(userRole);
                dbEntity.Entry(userRole).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _UserRolePermission
        {
            public List<UserRolePermission> GetAllByPermissionCatalogId(int permissionCatalogId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.UserRolePermission.AsNoTracking()
                             where c.PermissionCatalogID == permissionCatalogId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<UserRolePermission>();
            }

            public List<UserRolePermission> GetAllByUserRoleId(int userRoleId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.UserRolePermission.AsNoTracking()
                             where c.UserRoleID == userRoleId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<UserRolePermission>();
            }

            public List<UserRolePermission> GetAllByUserRoleIdIncludeDelete(int userRoleId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.UserRolePermission.AsNoTracking()
                             where c.UserRoleID == userRoleId
                             select c;
                return L2Enty.ToList<UserRolePermission>();
            }

            public UserRolePermission GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.UserRolePermission.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(UserRolePermission userRolePermission)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                userRolePermission.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.UserRolePermission.Add(userRolePermission);
                dbEntity.SaveChanges();
                return userRolePermission.Id;
            }

            public void AddManyRows(List<UserRolePermission> userRolePermissionList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (UserRolePermission userRolePermission in userRolePermissionList)
                {
                    userRolePermission.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                    dbEntity.UserRolePermission.Add(userRolePermission);
                }
                dbEntity.SaveChanges();
            }

            public void Update(UserRolePermission userRolePermission)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                userRolePermission.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.UserRolePermission.Attach(userRolePermission);
                dbEntity.Entry(userRolePermission).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }
            public void UpdateManyRows(List<UserRolePermission> userRolePermissionList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (UserRolePermission userRolePermission in userRolePermissionList)
                {
                    userRolePermission.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                    dbEntity.UserRolePermission.Attach(userRolePermission);
                    dbEntity.Entry(userRolePermission).State = System.Data.Entity.EntityState.Modified;
                }
                dbEntity.SaveChanges();
            }

            public void Delete(UserRolePermission userRolePermission)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                userRolePermission.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                userRolePermission.DeletedFlag = true;
                dbEntity.UserRolePermission.Attach(userRolePermission);
                dbEntity.Entry(userRolePermission).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _PermissionCatalog
        {
            public List<PermissionCatalog> GetAll()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.PermissionCatalog.AsNoTracking()
                             where c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<PermissionCatalog>();
            }

            public List<PermissionCatalog> GetAllBySuperAdmin()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.PermissionCatalog.AsNoTracking()
                             select c;
                return L2Enty.ToList<PermissionCatalog>();
            }

            public PermissionCatalog GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.PermissionCatalog.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(PermissionCatalog permissionCatalog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                permissionCatalog.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                permissionCatalog.DeletedFlag = false;
                dbEntity.PermissionCatalog.Add(permissionCatalog);
                dbEntity.SaveChanges();
                return permissionCatalog.Id;
            }

            public void Update(PermissionCatalog permissionCatalog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                permissionCatalog.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.PermissionCatalog.Attach(permissionCatalog);
                dbEntity.Entry(permissionCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(PermissionCatalog permissionCatalog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                permissionCatalog.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                permissionCatalog.DeletedFlag = true;
                dbEntity.PermissionCatalog.Attach(permissionCatalog);
                dbEntity.Entry(permissionCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _MessageCatalog
        {
            public List<MessageCatalog> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.MessageCatalog.AsNoTracking()
                             where c.CompanyID == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<MessageCatalog>();
            }

            public List<MessageCatalog> GetAllNonChildByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.MessageCatalog.AsNoTracking()
                             where c.CompanyID == companyId && c.ChildMessageFlag == false && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<MessageCatalog>();
            }

            public MessageCatalog GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.MessageCatalog.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(MessageCatalog messageCatalog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                messageCatalog.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.MessageCatalog.Add(messageCatalog);
                dbEntity.SaveChanges();
                return messageCatalog.Id;
            }

            public void Update(MessageCatalog messageCatalog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                messageCatalog.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.MessageCatalog.Attach(messageCatalog);
                dbEntity.Entry(messageCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(MessageCatalog messageCatalog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                messageCatalog.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                messageCatalog.DeletedFlag = true;
                dbEntity.MessageCatalog.Attach(messageCatalog);
                dbEntity.Entry(messageCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _MessageElement
        {
            public List<MessageElement> GetAllByMessageCatalog(int messageCatalogId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from element in dbEntity.MessageElement.AsNoTracking()
                             join catalog in dbEntity.MessageCatalog.AsNoTracking() on element.MessageCatalogID equals catalog.Id
                             where catalog.Id == messageCatalogId && catalog.DeletedFlag == false && element.MessageCatalogID == messageCatalogId
                             select element;
                return L2Enty.ToList<MessageElement>();
            }

            public MessageElement GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.MessageElement.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public MessageElement GetByid(int[] idList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.MessageElement.AsNoTracking()
                             where idList.Contains(c.Id)
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(MessageElement messageElement)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.MessageElement.Add(messageElement);
                dbEntity.SaveChanges();
                return messageElement.Id;
            }

            public void Update(MessageElement messageElement)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.MessageElement.Attach(messageElement);
                dbEntity.Entry(messageElement).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(MessageElement messageElement)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.MessageElement.Attach(messageElement);
                dbEntity.Entry(messageElement).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

        }

        public class _MessageMandatoryElementDef
        {
            public List<MessageMandatoryElementDef> GetAll()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.MessageMandatoryElementDef.AsNoTracking()
                             where c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<MessageMandatoryElementDef>();
            }

            public List<MessageMandatoryElementDef> GetAllBySuperAdmin()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.MessageMandatoryElementDef.AsNoTracking()
                             select c;
                return L2Enty.ToList<MessageMandatoryElementDef>();
            }

            public MessageMandatoryElementDef GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.MessageMandatoryElementDef.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(MessageMandatoryElementDef messageMandatoryElementDef)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                messageMandatoryElementDef.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.MessageMandatoryElementDef.Add(messageMandatoryElementDef);
                dbEntity.SaveChanges();
                return messageMandatoryElementDef.Id;
            }

            public void Update(MessageMandatoryElementDef messageMandatoryElementDef)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                messageMandatoryElementDef.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.MessageMandatoryElementDef.Attach(messageMandatoryElementDef);
                dbEntity.Entry(messageMandatoryElementDef).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(MessageMandatoryElementDef messageMandatoryElementDef)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                messageMandatoryElementDef.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.MessageMandatoryElementDef.Attach(messageMandatoryElementDef);
                dbEntity.Entry(messageMandatoryElementDef).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }
        }

        public class _OperationTask
        {
            public List<OperationTask> Search(string taskStatus, int hours)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                DateTime searchDatetime = DateTime.UtcNow.AddHours(hours);

                if (taskStatus != null)
                {
                    var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                                 where c.CreatedAt > searchDatetime && c.TaskStatus.ToString() == taskStatus.ToString() && c.DeletedFlag == false
                                 orderby c.CreatedAt descending
                                 select c;
                    return L2Enty.ToList<OperationTask>();
                }
                else
                {
                    var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                                 where c.CreatedAt > searchDatetime && c.DeletedFlag == false
                                 orderby c.CreatedAt descending
                                 select c;
                    return L2Enty.ToList<OperationTask>();
                }

            }
            public List<OperationTask> Search(string taskStatus, int hours, int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                DateTime searchDatetime = DateTime.UtcNow.AddHours(hours);

                if (taskStatus != null)
                {
                    var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                                 where c.CompanyId == companyId && c.CreatedAt > searchDatetime && c.TaskStatus.ToString() == taskStatus.ToString() && c.DeletedFlag == false
                                 orderby c.CreatedAt descending
                                 select c;
                    return L2Enty.ToList<OperationTask>();
                }
                else
                {
                    var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                                 where c.CompanyId == companyId && c.CreatedAt > searchDatetime && c.DeletedFlag == false
                                 orderby c.CreatedAt descending
                                 select c;
                    return L2Enty.ToList<OperationTask>();
                }

            }
            public List<OperationTask> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                             where c.CompanyId == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<OperationTask>();
            }

            public OperationTask GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.OperationTask.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(OperationTask operationTask)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                operationTask.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.OperationTask.Add(operationTask);
                dbEntity.SaveChanges();
                return operationTask.Id;
            }

            public void Update(OperationTask operationTask)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                operationTask.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.OperationTask.Attach(operationTask);
                dbEntity.Entry(operationTask).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(OperationTask operationTask)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                operationTask.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                operationTask.DeletedFlag = true;
                dbEntity.OperationTask.Attach(operationTask);
                dbEntity.Entry(operationTask).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _AlarmRuleCatalog
        {
            public List<AlarmRuleCatalog> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.AlarmRuleCatalog.AsNoTracking()
                             where c.CompanyId == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<AlarmRuleCatalog>();
            }

            public AlarmRuleCatalog GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.AlarmRuleCatalog.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(AlarmRuleCatalog alarmRuleCatalog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                alarmRuleCatalog.CreatedAt = DateTime.Parse(DateTime.Now.ToString());
                dbEntity.AlarmRuleCatalog.Add(alarmRuleCatalog);
                dbEntity.SaveChanges();
                return alarmRuleCatalog.Id;
            }

            public void Update(AlarmRuleCatalog alarmRuleCatalog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                alarmRuleCatalog.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.AlarmRuleCatalog.Attach(alarmRuleCatalog);
                dbEntity.Entry(alarmRuleCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(AlarmRuleCatalog alarmRuleCatalog)
            {
                int id = alarmRuleCatalog.Id;
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                alarmRuleCatalog.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                alarmRuleCatalog.DeletedFlag = true;
                dbEntity.AlarmRuleCatalog.Attach(alarmRuleCatalog);
                dbEntity.Entry(alarmRuleCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _ExternalApplication
        {
            public List<ExternalApplication> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.ExternalApplication.AsNoTracking()
                             where c.CompanyId == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<ExternalApplication>();
            }

            public ExternalApplication GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.ExternalApplication.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(ExternalApplication externalApplication)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                externalApplication.CreatedAt = DateTime.Parse(DateTime.Now.ToString());
                dbEntity.ExternalApplication.Add(externalApplication);
                dbEntity.SaveChanges();
                return externalApplication.Id;
            }

            public void Update(ExternalApplication externalApplication)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                externalApplication.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.ExternalApplication.Attach(externalApplication);
                dbEntity.Entry(externalApplication).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(ExternalApplication externalApplication)
            {
                int id = externalApplication.Id;
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                externalApplication.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                externalApplication.DeletedFlag = true;
                dbEntity.ExternalApplication.Attach(externalApplication);
                dbEntity.Entry(externalApplication).State = System.Data.Entity.EntityState.Modified;

                // Delete AlaraNotification 
                dbEntity.AlarmNotification.RemoveRange(dbEntity.AlarmNotification.Where(s => s.ExternalApplicationId == id));

                dbEntity.SaveChanges();
            }

        }

        public class _ExternalDashboard
        {
            public List<ExternalDashboard> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.ExternalDashboard.AsNoTracking()
                             where c.CompanyId == companyId && c.DeletedFlag == false
                             orderby c.Order ascending
                             select c;
                return L2Enty.ToList<ExternalDashboard>();
            }

            public ExternalDashboard GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.ExternalDashboard.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(ExternalDashboard externalDashboard)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                externalDashboard.CreatedAt = DateTime.Parse(DateTime.Now.ToString());
                dbEntity.ExternalDashboard.Add(externalDashboard);
                dbEntity.SaveChanges();
                return externalDashboard.Id;
            }

            public void Update(ExternalDashboard externalDashboard)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                externalDashboard.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.ExternalDashboard.Attach(externalDashboard);
                dbEntity.Entry(externalDashboard).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(ExternalDashboard externalDashboard)
            {
                int id = externalDashboard.Id;
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                externalDashboard.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                externalDashboard.DeletedFlag = true;
                dbEntity.ExternalDashboard.Attach(externalDashboard);
                dbEntity.Entry(externalDashboard).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

        }

        public class _AlarmNotification
        {
            public List<AlarmNotification> GetAllByAlarmRuleCatalogId(int alarmRuleCatalogId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.AlarmNotification.AsNoTracking()
                             where c.AlarmRuleCatalogId == alarmRuleCatalogId
                             select c;
                return L2Enty.ToList<AlarmNotification>();
            }

            public AlarmNotification GetById(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.AlarmNotification.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault<AlarmNotification>();
            }

            public void Add(List<AlarmNotification> alarmNotificationList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (AlarmNotification alarmNotification in alarmNotificationList)
                {
                    alarmNotification.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                    dbEntity.AlarmNotification.Add(alarmNotification);
                }
                dbEntity.SaveChanges();
            }

            public void Update(List<AlarmNotification> alarmNotificationList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (AlarmNotification alarmNotification in alarmNotificationList)
                {
                    dbEntity.AlarmNotification.Attach(alarmNotification);
                    dbEntity.Entry(alarmNotification).State = System.Data.Entity.EntityState.Modified;
                }

                dbEntity.SaveChanges();
            }

            public void Delete(List<AlarmNotification> alarmNotificationList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (AlarmNotification alarmNotification in alarmNotificationList)
                {
                    dbEntity.AlarmNotification.Attach(alarmNotification);
                    dbEntity.Entry(alarmNotification).State = System.Data.Entity.EntityState.Deleted;
                }

                dbEntity.SaveChanges();
            }

        }

        public class _AlarmRuleItem
        {
            public class DetailForRuleEngineModel
            {
                public int Id { get; set; }
                public int AlarmRuleCatalogId { get; set; }
                public int Ordering { get; set; }
                public int MessageElementId { get; set; }
                public string MessageElementFullName { get; set; }
                public string MessageElementDataType { get; set; }
                public string EqualOperation { get; set; }
                public string Value { get; set; }
                public string BitWiseOperation { get; set; }
            }

            public List<DetailForRuleEngineModel> GetAllByAlarmRuleCatalogIdForRuleEngine(int alarmRuleCatalogId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                List<AlarmRuleItem> itemList = (from c in dbEntity.AlarmRuleItem.AsNoTracking()
                                                where c.AlarmRuleCatalogId == alarmRuleCatalogId
                                                orderby c.Ordering ascending
                                                select c).ToList<AlarmRuleItem>();

                List<DetailForRuleEngineModel> returnDataList = new List<DetailForRuleEngineModel>();

                foreach (var item in itemList)
                {
                    DetailForRuleEngineModel returnData = new DetailForRuleEngineModel();
                    returnData.Id = item.Id;
                    returnData.AlarmRuleCatalogId = alarmRuleCatalogId;
                    returnData.Ordering = item.Ordering;
                    returnData.MessageElementId = item.MessageElementId;
                    returnData.EqualOperation = item.EqualOperation;
                    returnData.Value = item.Value;
                    returnData.BitWiseOperation = item.BitWiseOperation;
                    returnData.MessageElementDataType = item.MessageElement1.ElementDataType;

                    if (item.MessageElement != null)
                        returnData.MessageElementFullName = item.MessageElement.ElementName + "_" + item.MessageElement1.ElementName;
                    else
                        returnData.MessageElementFullName = item.MessageElement1.ElementName;

                    returnDataList.Add(returnData);
                }
                return returnDataList;
            }

            public List<AlarmRuleItem> GetAllByAlarmRuleCatalogId(int AlarmRuleCatalogId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.AlarmRuleItem.AsNoTracking()
                             where c.AlarmRuleCatalogId == AlarmRuleCatalogId
                             orderby c.Ordering ascending
                             select c;
                return L2Enty.ToList<AlarmRuleItem>();
            }

            public AlarmRuleItem GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.AlarmRuleItem.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(AlarmRuleItem alarmRuleItem)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.AlarmRuleItem.Add(alarmRuleItem);
                dbEntity.SaveChanges();
                return alarmRuleItem.Id;
            }

            public void Add(List<AlarmRuleItem> alarmRuleItemList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (AlarmRuleItem alarmRuleItem in alarmRuleItemList)
                {
                    dbEntity.AlarmRuleItem.Add(alarmRuleItem);
                }

                dbEntity.SaveChanges();
            }

            public void Update(AlarmRuleItem alarmRuleItem)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.AlarmRuleItem.Attach(alarmRuleItem);
                dbEntity.Entry(alarmRuleItem).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(AlarmRuleItem alarmRuleItem)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.AlarmRuleItem.Attach(alarmRuleItem);
                dbEntity.Entry(alarmRuleItem).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

            public void Delete(List<AlarmRuleItem> alarmRuleItemList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (AlarmRuleItem alarmRuleItem in alarmRuleItemList)
                {
                    dbEntity.AlarmRuleItem.Attach(alarmRuleItem);
                    dbEntity.Entry(alarmRuleItem).State = System.Data.Entity.EntityState.Deleted;
                }

                dbEntity.SaveChanges();
            }
        }

        public class _WidgetClass
        {
            public List<WidgetClass> GetAll()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.WidgetClass.AsNoTracking()
                             select c;
                return L2Enty.ToList<WidgetClass>();
            }

            public List<WidgetClass> GetAllByLevel(string level)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.WidgetClass.AsNoTracking()
                             where c.Level == level.ToLower() && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<WidgetClass>();
            }

            public WidgetClass GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.WidgetClass.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(WidgetClass widgetClass)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                widgetClass.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.WidgetClass.Add(widgetClass);
                dbEntity.SaveChanges();
                return widgetClass.Id;
            }

            public void Update(WidgetClass widgetClass)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                widgetClass.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.WidgetClass.Attach(widgetClass);
                dbEntity.Entry(widgetClass).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(WidgetClass widgetClass)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.WidgetClass.Attach(widgetClass);
                dbEntity.Entry(widgetClass).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

        }

        public class _WidgetCatalog
        {
            public List<WidgetCatalog> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.WidgetCatalog.AsNoTracking()
                             where c.CompanyID == companyId && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<WidgetCatalog>();
            }

            public List<WidgetCatalog> GetAllByCompanyId(int companyId, string level)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.WidgetCatalog.AsNoTracking()
                             where c.CompanyID == companyId && c.Level == level.ToLower() && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<WidgetCatalog>();
            }

            public List<WidgetCatalog> GetAllByLevel(string level)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.WidgetCatalog.AsNoTracking()
                             where c.Level == level.ToLower() && c.DeletedFlag == false
                             select c;
                return L2Enty.ToList<WidgetCatalog>();
            }

            public WidgetCatalog GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.WidgetCatalog.AsNoTracking()
                             where c.Id == id && c.DeletedFlag == false
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(WidgetCatalog widgetCatalog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                widgetCatalog.CreatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.WidgetCatalog.Add(widgetCatalog);
                dbEntity.SaveChanges();
                return widgetCatalog.Id;
            }

            public void Update(WidgetCatalog widgetCatalog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                widgetCatalog.UpdatedAt = DateTime.Parse(DateTime.UtcNow.ToString());
                dbEntity.WidgetCatalog.Attach(widgetCatalog);
                dbEntity.Entry(widgetCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(WidgetCatalog widgetCatalog)
            {
                int id = widgetCatalog.Id;
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                widgetCatalog.DeletedFlag = true;
                dbEntity.WidgetCatalog.Attach(widgetCatalog);
                dbEntity.Entry(widgetCatalog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.DashboardWidgets.RemoveRange(dbEntity.DashboardWidgets.Where(s => s.WidgetCatalogID == id));

                dbEntity.SaveChanges();
            }
        }

        public class _Dashboard
        {
            public List<Dashboard> GetAllByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.CompanyID == companyId
                             select c;
                return L2Enty.ToList<Dashboard>();
            }

            public List<Dashboard> GetAllByCompanyId(int companyId, string type)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.CompanyID == companyId && c.DashboardType == type.ToLower()
                             select c;
                return L2Enty.ToList<Dashboard>();
            }

            public List<Dashboard> GetAllEquipmentClassByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.CompanyID == companyId && c.DashboardType == "EquipmentClass"
                             select c;
                return L2Enty.ToList<Dashboard>();
            }
            public List<Dashboard> GetAllEquipmentByCompanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.CompanyID == companyId && c.DashboardType == "Equipment"
                             select c;
                return L2Enty.ToList<Dashboard>();
            }
            public List<Dashboard> GetAllByFactoryId(int factoryId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.FactoryID == factoryId
                             select c;
                return L2Enty.ToList<Dashboard>();
            }

            public Dashboard GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Dashboard.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(Dashboard dashboard)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.Dashboard.Add(dashboard);
                dbEntity.SaveChanges();
                return dashboard.Id;
            }

            public void Update(Dashboard dashboard)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.Dashboard.Attach(dashboard);
                dbEntity.Entry(dashboard).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(Dashboard dashboard)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.Dashboard.Attach(dashboard);
                dbEntity.Entry(dashboard).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }

        }

        public class _DashboardWidget
        {
            public List<DashboardWidgets> GetAllByDashboardId(int dashboardId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.DashboardWidgets.AsNoTracking()
                             where c.DashboardID == dashboardId
                             orderby c.RowNo ascending, c.ColumnSeq ascending
                             select c;
                return L2Enty.ToList<DashboardWidgets>();
            }

            public DashboardWidgets GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.DashboardWidgets.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(DashboardWidgets dashboardWidget)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.DashboardWidgets.Add(dashboardWidget);
                dbEntity.SaveChanges();
                return dashboardWidget.Id;
            }

            public void Update(DashboardWidgets dashboardWidget)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.DashboardWidgets.Attach(dashboardWidget);
                dbEntity.Entry(dashboardWidget).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }
            public void Update(List<DashboardWidgets> dashboardWidgetList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (DashboardWidgets widget in dashboardWidgetList)
                {
                    dbEntity.DashboardWidgets.Attach(widget);
                    dbEntity.Entry(widget).State = System.Data.Entity.EntityState.Modified;
                }

                dbEntity.SaveChanges();
            }

            public void Delete(DashboardWidgets dashboardWidget)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.DashboardWidgets.Attach(dashboardWidget);
                dbEntity.Entry(dashboardWidget).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }
        }

        public class _UsageLog
        {
            public List<UsageLog> GetAllByCompanyId(int companyId, int days, string order)
            {
                DateTime datetime_0000 = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
                days--;
                if (days > 0)
                    datetime_0000 = datetime_0000.AddDays(-days);

                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                if (order.ToLower().Equals("desc"))
                {
                    var L2Enty = from c in dbEntity.UsageLog.AsNoTracking()
                                 where c.CompanyId == companyId && c.UpdatedAt >= datetime_0000
                                 orderby c.UpdatedAt descending
                                 select c;
                    return L2Enty.ToList<UsageLog>();
                }
                else
                {
                    var L2Enty = from c in dbEntity.UsageLog.AsNoTracking()
                                 where c.CompanyId == companyId && c.UpdatedAt >= datetime_0000
                                 orderby c.UpdatedAt ascending
                                 select c;
                    return L2Enty.ToList<UsageLog>();
                }
            }

            public UsageLog GetLastByCommpanyId(int companyId)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();

                var L2Enty = from c in dbEntity.UsageLog.AsNoTracking()
                             where c.CompanyId == companyId
                             orderby c.UpdatedAt descending
                             select c;
                return L2Enty.FirstOrDefault<UsageLog>();
            }

            public List<UsageLog> GetAll(int days, string order)
            {
                DateTime datetime_0000 = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
                days--;
                if (days > 0)
                    datetime_0000 = datetime_0000.AddDays(-days);

                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                if (order.ToLower().Equals("desc"))
                {
                    var L2Enty = from c in dbEntity.UsageLog.AsNoTracking()
                                 where c.UpdatedAt >= datetime_0000
                                 orderby c.UpdatedAt descending
                                 select c;
                    return L2Enty.ToList<UsageLog>();
                }
                else
                {
                    var L2Enty = from c in dbEntity.UsageLog.AsNoTracking()
                                 where c.UpdatedAt >= datetime_0000
                                 select c;
                    return L2Enty.ToList<UsageLog>();
                }
            }

            public UsageLog GetByid(int id)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.UsageLog.AsNoTracking()
                             where c.Id == id
                             select c;
                return L2Enty.FirstOrDefault();
            }

            public int Add(UsageLog usageLog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.UsageLog.Add(usageLog);
                dbEntity.SaveChanges();
                return usageLog.Id;
            }

            public void Add(List<UsageLog> usageLogList)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                foreach (UsageLog usageLog in usageLogList)
                {
                    dbEntity.UsageLog.Add(usageLog);
                }
                dbEntity.SaveChanges();
            }

            public void Update(UsageLog usageLog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.UsageLog.Attach(usageLog);
                dbEntity.Entry(usageLog).State = System.Data.Entity.EntityState.Modified;

                dbEntity.SaveChanges();
            }

            public void Delete(UsageLog usageLog)
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                dbEntity.UsageLog.Attach(usageLog);
                dbEntity.Entry(usageLog).State = System.Data.Entity.EntityState.Deleted;

                dbEntity.SaveChanges();
            }
        }

        public class _UsageLogSumByDay
        {
            public List<UsageLogSumByDay> GetAll(int days, string order)
            {
                DateTime datetime_0000 = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
                days--;
                if (days > 0)
                    datetime_0000 = datetime_0000.AddDays(-days);

                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                if (order.ToLower().Equals("desc"))
                {
                    var L2Enty = from c in dbEntity.UsageLogSumByDay.AsNoTracking()
                                 where c.UpdatedDateTime >= datetime_0000
                                 orderby c.UpdatedDateTime descending
                                 select c;
                    return L2Enty.ToList<UsageLogSumByDay>();
                }
                else
                {
                    var L2Enty = from c in dbEntity.UsageLogSumByDay.AsNoTracking()
                                 where c.UpdatedDateTime >= datetime_0000
                                 select c;
                    return L2Enty.ToList<UsageLogSumByDay>();
                }
            }
            public UsageLogSumByDay GetLast()
            {
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.UsageLogSumByDay.AsNoTracking()
                             orderby c.UpdatedDateTime descending
                             select c;
                return L2Enty.FirstOrDefault<UsageLogSumByDay>();
            }
        }
    }
}
