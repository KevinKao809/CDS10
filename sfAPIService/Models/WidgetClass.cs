using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class WidgetClassModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public int Key { get; set; }
            public string Name { get; set; }
            public string Level { get; set; }
            public string PhotoURL { get; set; }
            public int? MinWidth { get; set; }
            public int? MinHeight { get; set; }
            public bool DeletedFlag { get; set; }
        }
        public class Add
        {
            [Required]
            public int Key { get; set; }
            [Required]
            [MaxLength(50)]
            public string Name { get; set; }
            [Required]
            [MaxLength(50)]
            public string Level { get; set; }
            public string PhotoURL { get; set; }
            public int MinWidth { get; set; }
            public int MinHeight { get; set; }
            [Required]
            public bool DeletedFlag { get; set; }
        }

        public class Update
        {
            [Required]
            [MaxLength(50)]
            public string Name { get; set; }
            [Required]
            [MaxLength(50)]
            public string Level { get; set; }
            public string PhotoURL { get; set; }
            public int MinWidth { get; set; }
            public int MinHeight { get; set; }
            [Required]
            public bool DeletedFlag { get; set; }
        }
        public List<Detail> getAllwidgetClasses()
        {
            DBHelper._WidgetClass dbhelp = new DBHelper._WidgetClass();

            return dbhelp.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                Key = s.Key,
                Name = s.Name,
                Level = s.Level,
                PhotoURL = s.PhotoURL,
                MinWidth = s.MinWidth,
                MinHeight = s.MinHeight,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();

        }

        public List<Detail> getAllWidgetClassesByLevel(string level)
        {
            DBHelper._WidgetClass dbhelp = new DBHelper._WidgetClass();

            return dbhelp.GetAllByLevel(level).Select(s => new Detail()
            {
                Id = s.Id,
                Key = s.Key,
                Name = s.Name,
                Level = s.Level,
                PhotoURL = s.PhotoURL,
                MinWidth = s.MinWidth,
                MinHeight = s.MinHeight,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();
        }
        public Detail getWidgetClassById(int id)
        {
            DBHelper._WidgetClass dbhelp = new DBHelper._WidgetClass();
            WidgetClass widgetClass = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = widgetClass.Id,
                Key = widgetClass.Key,
                Name = widgetClass.Name,
                Level = widgetClass.Level,
                PhotoURL = widgetClass.PhotoURL,
                MinWidth = widgetClass.MinWidth,
                MinHeight = widgetClass.MinHeight,
                DeletedFlag = widgetClass.DeletedFlag
            };
        }

        public int addWidgetClass(Add widgetClass)
        {
            DBHelper._WidgetClass dbhelp = new DBHelper._WidgetClass();
            var newWidgetClass = new WidgetClass()
            {
                Key = widgetClass.Key,
                Name = widgetClass.Name,
                PhotoURL = widgetClass.PhotoURL,
                Level = widgetClass.Level,
                MinWidth = widgetClass.MinWidth,
                MinHeight = widgetClass.MinHeight,
                DeletedFlag = widgetClass.DeletedFlag
            };
            return dbhelp.Add(newWidgetClass);
        }

        public void updateWidgetClass(int id, Update widgetClass)
        {
            DBHelper._WidgetClass dbhelp = new DBHelper._WidgetClass();
            WidgetClass existingWidgetClass = dbhelp.GetByid(id);
            existingWidgetClass.Name = widgetClass.Name;
            existingWidgetClass.PhotoURL = widgetClass.PhotoURL;
            existingWidgetClass.Level = widgetClass.Level;
            existingWidgetClass.MinHeight = widgetClass.MinHeight;
            existingWidgetClass.MinWidth = widgetClass.MinWidth;
            existingWidgetClass.DeletedFlag = widgetClass.DeletedFlag;

            dbhelp.Update(existingWidgetClass);
        }

        public void deleteWidgetClass(int id)
        {
            DBHelper._WidgetClass dbhelp = new DBHelper._WidgetClass();
            WidgetClass existingWidgetClass = dbhelp.GetByid(id);

            dbhelp.Delete(existingWidgetClass);
        }

        public void updateWidgetClassLogoURL(int id, string url)
        {
            DBHelper._WidgetClass dbhelp = new DBHelper._WidgetClass();
            WidgetClass existingWidgetClass = dbhelp.GetByid(id);
            existingWidgetClass.PhotoURL = url;
            dbhelp.Update(existingWidgetClass);
        }
    }
}