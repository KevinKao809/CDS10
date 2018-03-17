using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using sfShareLib;

namespace sfAPIService.Models
{
    public class AlarmRuleItemModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public int AlarmRuleCatalogId { get; set; }
            public int Ordering { get; set; }
            public int? MessageElementParentId { get; set; }
            public int MessageElementId { get; set; }
            public string MessageElementName { get; set; }
            public string EqualOperation { get; set; }
            public string Value { get; set; }
            public string BitWiseOperation { get; set; }
        }

        public class Edit_AlarmRuleItem
        {
            [Required]
            public int Ordering { get; set; }
            public int? MessageElementParentId { get; set; }
            [Required]
            public int MessageElementId { get; set; }
            
            [Required]
            [MaxLength(20)]
            public string EqualOperation { get; set; }
            [Required]
            [MaxLength(50)]
            public string Value { get; set; }
            [Required]
            [MaxLength(10)]
            public string BitWiseOperation { get; set; }
        }

        public class Edit
        {
            public Edit_AlarmRuleItem[] AlarmRules { get; set; }
        }


        public List<Detail> GetAllAlarmRuleItemByAlarmRuleCatalogId(int alarmRuleCatalogId)
        {
            DBHelper._AlarmRuleItem dbhelp = new DBHelper._AlarmRuleItem();

            return dbhelp.GetAllByAlarmRuleCatalogId(alarmRuleCatalogId).Select(s => new Detail()
            {
                Id = s.Id,
                AlarmRuleCatalogId = s.AlarmRuleCatalogId,
                Ordering = s.Ordering,
                MessageElementParentId = s.MessageElementParentId,
                MessageElementId = s.MessageElementId,
                MessageElementName = s.MessageElement1.ElementName,
                EqualOperation = s.EqualOperation,
                Value = s.Value,
                BitWiseOperation = s.BitWiseOperation
            }).ToList<Detail>();
        }

        public Detail getAlarmRuleItemById(int id)
        {
            DBHelper._AlarmRuleItem dbhelp = new DBHelper._AlarmRuleItem();
            AlarmRuleItem alarmRuleItem = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = alarmRuleItem.Id,
                AlarmRuleCatalogId = alarmRuleItem.AlarmRuleCatalogId,
                Ordering = alarmRuleItem.Ordering,
                MessageElementParentId = alarmRuleItem.MessageElementParentId,
                MessageElementId = alarmRuleItem.MessageElementId,
                MessageElementName = alarmRuleItem.MessageElement1.ElementName,
                EqualOperation = alarmRuleItem.EqualOperation,
                Value = alarmRuleItem.Value,
                BitWiseOperation = alarmRuleItem.BitWiseOperation
            };
        }

        public void UpdateAllAlarmRules(int alarmRuleCatalogId, Edit input)
        {
            DBHelper._AlarmRuleItem dbhelp = new DBHelper._AlarmRuleItem();

            List<AlarmRuleItem> existAlarmRuleItemList = dbhelp.GetAllByAlarmRuleCatalogId(alarmRuleCatalogId);
            List<AlarmRuleItem> addList = new List<AlarmRuleItem>();
            dbhelp.Delete(existAlarmRuleItemList);

            foreach (var tmp in input.AlarmRules)
            {
                AlarmRuleItem alarmRuleItem = new AlarmRuleItem();
                alarmRuleItem.AlarmRuleCatalogId = alarmRuleCatalogId;
                alarmRuleItem.Ordering = tmp.Ordering;                
                alarmRuleItem.MessageElementId = tmp.MessageElementId;
                alarmRuleItem.EqualOperation = tmp.EqualOperation;
                alarmRuleItem.Value = tmp.Value;
                alarmRuleItem.BitWiseOperation = tmp.BitWiseOperation;

                if (tmp.MessageElementParentId != null)
                    alarmRuleItem.MessageElementParentId = tmp.MessageElementParentId;

                addList.Add(alarmRuleItem);
            }

            dbhelp.Add(addList);
        }
    }
}