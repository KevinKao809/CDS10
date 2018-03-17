using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class MessageMandatoryElementDefModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string ElementName { get; set; }
            public string ElementDataType { get; set; }
            public bool MandatoryFlag { get; set; }
            public string Description { get; set; }
            public bool DeletedFlag { get; set; }
        }
        public class Add
        {
            [Required]
            public string ElementName { get; set; }
            [Required]
            public string ElementDataType { get; set; }
            [Required]
            public bool MandatoryFlag { get; set; }
            public string Description { get; set; }
        }
        public class Update
        {
            [Required]
            public string ElementName { get; set; }
            [Required]
            public string ElementDataType { get; set; }
            [Required]
            public bool MandatoryFlag { get; set; }
            public string Description { get; set; }
            public bool? DeletedFlag { get; set; }
        }

        public List<Detail> GetAllMessageMandatoryElementDef()
        {
            DBHelper._MessageMandatoryElementDef dbhelp = new DBHelper._MessageMandatoryElementDef();

            return dbhelp.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                ElementName = s.ElementName,
                ElementDataType = s.ElementDataType,
                MandatoryFlag = s.MandatoryFlag,
                Description = s.Description,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();

        }

        public List<Detail> GetAllMessageMandatoryElementDefBySuperAdmin()
        {
            DBHelper._MessageMandatoryElementDef dbhelp = new DBHelper._MessageMandatoryElementDef();

            return dbhelp.GetAllBySuperAdmin().Select(s => new Detail()
            {
                Id = s.Id,
                ElementName = s.ElementName,
                ElementDataType = s.ElementDataType,
                MandatoryFlag = s.MandatoryFlag,
                Description = s.Description,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();

        }

        public Detail getMessageMandatoryElementDefById(int id)
        {
            DBHelper._MessageMandatoryElementDef dbhelp = new DBHelper._MessageMandatoryElementDef();
            MessageMandatoryElementDef mMED = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = mMED.Id,
                ElementName = mMED.ElementName,
                ElementDataType = mMED.ElementDataType,
                MandatoryFlag = mMED.MandatoryFlag,
                Description = mMED.Description,
                DeletedFlag = mMED.DeletedFlag
            };
        }

        public void addMessageMandatoryElementDef(Add mMED)
        {
            DBHelper._MessageMandatoryElementDef dbhelp = new DBHelper._MessageMandatoryElementDef();
            var newMessageMandatoryElementDef = new MessageMandatoryElementDef()
            {
                ElementName = mMED.ElementName,
                ElementDataType = mMED.ElementDataType,
                MandatoryFlag =  mMED.MandatoryFlag,
                Description = mMED.Description
            };
            dbhelp.Add(newMessageMandatoryElementDef);
        }

        public void updateMessageMandatoryElementDef(int id, Update mMED)
        {
            DBHelper._MessageMandatoryElementDef dbhelp = new DBHelper._MessageMandatoryElementDef();
            MessageMandatoryElementDef existingMessageMandatoryElementDef = dbhelp.GetByid(id);

            existingMessageMandatoryElementDef.ElementName = mMED.ElementName;
            existingMessageMandatoryElementDef.ElementDataType = mMED.ElementDataType;
            existingMessageMandatoryElementDef.MandatoryFlag = mMED.MandatoryFlag;
            existingMessageMandatoryElementDef.Description = mMED.Description;
            if(mMED.DeletedFlag.HasValue)
                existingMessageMandatoryElementDef.DeletedFlag = (bool) mMED.DeletedFlag;

            dbhelp.Update(existingMessageMandatoryElementDef);
        }

        public void deleteMessageMandatoryElementDef(int id)
        {
            DBHelper._MessageMandatoryElementDef dbhelp = new DBHelper._MessageMandatoryElementDef();
            MessageMandatoryElementDef existingMessageMandatoryElementDef = dbhelp.GetByid(id);

            dbhelp.Delete(existingMessageMandatoryElementDef);
        }
    }
}