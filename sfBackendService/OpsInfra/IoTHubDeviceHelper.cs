using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sfShareLib;

namespace OpsInfra
{
    class IoTHubDeviceMessageModel
    {
        public string taskId { get; set; }
        public string oldPrimaryIothubConnectionString { get; set; }
        //public string oldSecondaryIothubConnectionString { get; set; }
        public string primaryIothubConnectionString { get; set; }
        //public string secondaryIothubConnectionString { get; set; }
        public string iothubDeviceId { get; set; }
        public string authenticationType { get; set; }
        public string iothubDeviceKey { get; set; }
        public string certificateThumbprint { get; set; }
        
    }
    class IoTHubDeviceHelper
    {
        public string _IoTHubDeviceId;
        public string _IoTHubDeviceKey;
        public string _OldPrimaryIoTHubConnectionString;
        //public string _OldSecondaryIoTHubConnectionString;
        public string _PrimaryIoTHubConnectionString;
        //public string _SecondaryIoTHubConnectionString;
        public string _AuthenticationType;
        public string _CertificateThumbprint;
        public int _TaskId;
        public string _Action;
        private IoTHubDeviceMessageModel _Message;
        private int _Status = 0;


        public IoTHubDeviceHelper(IoTHubDeviceMessageModel message, string action)
        {
            _Message = message;
            _TaskId = Int32.Parse(message.taskId);
            _IoTHubDeviceId = message.iothubDeviceId;
            _IoTHubDeviceKey = message.iothubDeviceKey;
            _OldPrimaryIoTHubConnectionString = message.oldPrimaryIothubConnectionString;
            //_OldSecondaryIoTHubConnectionString = message.oldSecondaryIothubConnectionString;
            _PrimaryIoTHubConnectionString = message.primaryIothubConnectionString;
            //_SecondaryIoTHubConnectionString = message.secondaryIothubConnectionString;
            _AuthenticationType = message.authenticationType;
            _CertificateThumbprint = message.certificateThumbprint;
            
            if (action.StartsWith("create"))
                _Action = "create";
            else if (action.StartsWith("update"))
                _Action = "update";
            else
                _Action = "remove";         
        }

        public async void ThreadProc()
        {
            try
            {
                string exceptionString = "";
                switch (_Action)
                {
                    case "create":
                        exceptionString += await CreateIoTHubDeviceAsync(_PrimaryIoTHubConnectionString, "primary");
                        //exceptionString += await CreateIoTHubDeviceAsync(_SecondaryIoTHubConnectionString, "secondary");
                        UpdateDBIoTDeviceKey(this._IoTHubDeviceKey);
                        break;
                    case "update":
                        if (string.IsNullOrEmpty(_OldPrimaryIoTHubConnectionString))
                        {
                            exceptionString += await UpdateIoTHubDeviceAsync(_PrimaryIoTHubConnectionString, "primary");
                            //exceptionString += await UpdateIoTHubDeviceAsync(_SecondaryIoTHubConnectionString, "secondary");
                        }
                        else
                        {
                            exceptionString += await DeleteIoTHubDeviceAsync(_OldPrimaryIoTHubConnectionString, "old primary");
                            //exceptionString += await DeleteIoTHubDeviceAsync(_OldSecondaryIoTHubConnectionString, "old secondary");
                            exceptionString += await CreateIoTHubDeviceAsync(_PrimaryIoTHubConnectionString, "primary");
                            //exceptionString += await CreateIoTHubDeviceAsync(_SecondaryIoTHubConnectionString, "secondary");
                        }
                        UpdateDBIoTDeviceKey(this._IoTHubDeviceKey);
                        break;
                    case "remove":
                        exceptionString += await DeleteIoTHubDeviceAsync(_PrimaryIoTHubConnectionString, "primary");
                        //exceptionString += await DeleteIoTHubDeviceAsync(_SecondaryIoTHubConnectionString, "secondary");
                        break;
                }
                if (exceptionString == "")
                {
                    Program.UpdateTaskBySuccess(_TaskId);
                    Console.WriteLine("[IoTHubDevice] " + _Action + " Success: IoTHubDeviceId-" + _IoTHubDeviceId);
                }
                else
                    throw new Exception(exceptionString);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("[IoTHubDevice] " + _Action + " Failed: IoTHubDeviceId-" + _IoTHubDeviceId);
                logMessage.AppendLine("\tMessage:" + JsonConvert.SerializeObject(this));
                logMessage.AppendLine("\tException:" + ex.Message);
                Program._sfAppLogger.Error(logMessage);
                Program.UpdateTaskByFail(_TaskId, Program.FilterErrorMessage(ex.Message), _Status);
                Console.WriteLine(logMessage);
                _Status = 0;
            }
        }
        public async System.Threading.Tasks.Task<string> CreateIoTHubDeviceAsync(string connectionString, string iotHubType)
        {
            RegistryManager registryManager;
            Device device;

            try
            {
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            }
            catch(Exception ex)
            {
                _Status++;
                return "\t【CREATE】Cann't connect " + iotHubType + " IoTHub:" + Program.GetNonJsonExceptionMessage(ex.Message) + "\n";
            }
            
            if (_AuthenticationType.ToLower() == "key")
            {
                device = new Device(_IoTHubDeviceId)
                {
                    Authentication = new AuthenticationMechanism()
                    {
                        SymmetricKey = new SymmetricKey()
                        {
                            PrimaryKey = _IoTHubDeviceKey,
                            SecondaryKey = _IoTHubDeviceKey
                        }
                    }
                };
            }
            else
            {
                device = new Device(_IoTHubDeviceId)
                {
                    Authentication = new AuthenticationMechanism()
                    {
                        X509Thumbprint = new X509Thumbprint()
                        {
                            PrimaryThumbprint = this._CertificateThumbprint
                        }
                    }
                };
            }

            try 
            {
                await registryManager.AddDeviceAsync(device);
                this._IoTHubDeviceKey = device.Authentication.SymmetricKey.PrimaryKey;
            }
            catch (Exception ex)
            {
                _Status++;
                return "\t【CREATE】Cann't create IoTDevice(" + iotHubType + " IoTHub, IoTDeviceId: " + this._IoTHubDeviceId + "): " + Program.GetNonJsonExceptionMessage(ex.Message) + "\n";
            }
            return "";
        }
        
        public async System.Threading.Tasks.Task<string> DeleteIoTHubDeviceAsync(string connectionString, string iotHubType)
        {
            RegistryManager registryManager;

            try
            {
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            }
            catch(Exception ex)
            {
                return "\t【DELETE】Cann't connect " + iotHubType + " IoTHub: " + Program.GetNonJsonExceptionMessage(ex.Message) + "\n";
            }

            try
            {
                await registryManager.RemoveDeviceAsync(_IoTHubDeviceId);
            }
            catch (Exception ex)
            {
                return "\t【DELETE】Cann't delete IoTDevice(" + iotHubType + " IoTHub, IoTDeviceId: " + this._IoTHubDeviceId + "): " + Program.GetNonJsonExceptionMessage(ex.Message) + "\n";
            }
            return "";
        }

        public async System.Threading.Tasks.Task<string> UpdateIoTHubDeviceAsync(string connectionString, string iotHubType)
        {
            RegistryManager registryManager;
            Device device;

            try
            {
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            }
            catch (Exception ex)
            {
                _Status++;
                return "\t【UPDATE】Cann't connect " + iotHubType + " IoTHub: " + Program.GetNonJsonExceptionMessage(ex.Message) + "\n";
            }


            try
            {
                bool updateFlag = false;
                device = await registryManager.GetDeviceAsync(_IoTHubDeviceId);
                if (_AuthenticationType.ToLower() == "key" && String.IsNullOrEmpty(device.Authentication.SymmetricKey.PrimaryKey))
                {
                    device.Authentication = new AuthenticationMechanism()
                    {
                        SymmetricKey = new SymmetricKey()
                        {
                            PrimaryKey = _IoTHubDeviceKey,
                            SecondaryKey = _IoTHubDeviceKey
                        }
                    };
                    updateFlag = true;
                }
                else if (_AuthenticationType.ToLower() == "certificate" && String.IsNullOrEmpty(device.Authentication.X509Thumbprint.PrimaryThumbprint))
                {
                    device.Authentication = new AuthenticationMechanism()
                    {
                        X509Thumbprint = new X509Thumbprint()
                        {
                            PrimaryThumbprint = this._CertificateThumbprint
                        }
                    };
                    updateFlag = true;
                }

                if (updateFlag)
                {
                    device = await registryManager.UpdateDeviceAsync(device);
                }
                _IoTHubDeviceKey = device.Authentication.SymmetricKey.PrimaryKey;
            }
            catch (Exception ex)
            {
                _Status++;
                return "\t【UPDATE】Cann't update IoTDeice(" + iotHubType + " IoTHub, IoTDeviceId: " + this._IoTHubDeviceId + "): " + Program.GetNonJsonExceptionMessage(ex.Message) + "\n";
            }
            return "";
        }

        public void UpdateDBIoTDeviceKey(string deviceKey)
        {
            DBHelper._IoTDevice iotDeviceHelper = new DBHelper._IoTDevice();
            IoTDevice iotDevice = iotDeviceHelper.GetByid(this._IoTHubDeviceId);

            iotDevice.IoTHubDeviceKey = deviceKey;
            iotDeviceHelper.Update(iotDevice);
        }

        //public async System.Threading.Tasks.Task<string> UpdateIoTHubDeviceAsync(string connectionString, string iotHubType)
        //{
        //    RegistryManager registryManager;
        //    Device device;

        //    try
        //    {
        //        registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        //    }
        //    catch (Exception ex)
        //    {
        //        return "\tCann't connect " + iotHubType + " IoTHub: " + Program.GetNonJsonExceptionMessage(ex.Message) + "\n";
        //    }


        //    try
        //    {
        //        bool updateFlag = false;
        //        device = await registryManager.GetDeviceAsync(_IoTHubDeviceId);
        //        if (_AuthenticationType.ToLower() == "key" && String.IsNullOrEmpty(device.Authentication.SymmetricKey.PrimaryKey))
        //        {
        //            device.Authentication = new AuthenticationMechanism()
        //            {
        //                SymmetricKey = new SymmetricKey()
        //                {
        //                    PrimaryKey = _IoTHubDeviceKey,
        //                    SecondaryKey = _IoTHubDeviceKey
        //                }
        //            };
        //            updateFlag = true;
        //        }
        //        else if (_AuthenticationType.ToLower() == "certificate" && String.IsNullOrEmpty(device.Authentication.X509Thumbprint.PrimaryThumbprint))
        //        {
        //            device.Authentication = new AuthenticationMechanism()
        //            {
        //                X509Thumbprint = new X509Thumbprint()
        //                {
        //                    PrimaryThumbprint = this._CertificateThumbprint
        //                }
        //            };
        //            updateFlag = true;
        //        }

        //        if (updateFlag)
        //        {
        //            device = await registryManager.UpdateDeviceAsync(device);
        //        }
        //        _IoTHubDeviceKey = device.Authentication.SymmetricKey.PrimaryKey;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "\tCann't update IoTDeice(" + iotHubType + " IoTHub, IoTDeviceId: " + this._IoTHubDeviceId + "): " + Program.GetNonJsonExceptionMessage(ex.Message) + "\n";
        //    }
        //    return "";
        //}
    }
}