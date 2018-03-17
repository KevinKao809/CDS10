using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;

namespace sfAPIService.Models
{
    public class AccountModels
    {
        

        public class PasswordSet
        {
            [Required]
            public string OldPassword { get; set; }
            [Required]
            public string NewPassword { get; set; }
        }
        
        public void ResetEmployeePassword(int id, string newPassword)
        {
            DBHelper._Employee dbhelp = new DBHelper._Employee();
            Employee existingEmployee = dbhelp.GetByid(id);

            existingEmployee.Password = Crypto.HashPassword(newPassword);
            dbhelp.Update(existingEmployee);
        }

        public void ChangeEmployeePassword(int id, ChangePasswordModels model)
        {
            DBHelper._Employee dbhelp = new DBHelper._Employee();
            Employee existingEmployee = dbhelp.GetByid(id);

            if (existingEmployee == null)
                throw new Exception("404");

            if (Crypto.VerifyHashedPassword(existingEmployee.Password, model.OldPassword))
            {
                existingEmployee.Password = Crypto.HashPassword(model.NewPassword);
                dbhelp.Update(existingEmployee);
            }
            else
            {
                throw new Exception("401");
            }
        }
                
        public bool CheckIoTDevicePassword(string deviceId, string password)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            IoTDevice iotDevice = dbhelp.GetByid(deviceId);

            if (Crypto.VerifyHashedPassword(iotDevice.IoTHubDevicePW, password))
                return true;
            else
                return false;
        }

        public void ChangeIoTDevicePassword(string deviceId, PasswordSet passwordSet)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            IoTDevice iotDevice = dbhelp.GetByid(deviceId);

            if (iotDevice == null)
                throw new Exception("404");

            if (Crypto.VerifyHashedPassword(iotDevice.IoTHubDevicePW, passwordSet.OldPassword))
            {
                iotDevice.IoTHubDevicePW = Crypto.HashPassword(passwordSet.NewPassword);
                dbhelp.Update(iotDevice);
            }
            else
            {
                throw new Exception("401");
            }
        }

        public void ResetIoTDevicePassword(string deviceId, string newPassword)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            IoTDevice iotDevice = dbhelp.GetByid(deviceId);

            iotDevice.IoTHubDevicePW = Crypto.HashPassword(newPassword);
            dbhelp.Update(iotDevice);
        }
    }

    public class ChangePasswordModels
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
    public class ResetPasswordModels
    {
        [Required]
        public string NewPassword { get; set; }
    }
}