using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class DashboardWidgetModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public int DashboardId { get; set; }
            public int WidgetCatalogId { get; set; }
            public string WidgetCatalogName { get; set; }
            public int? RowNo { get; set; }
            public int? ColumnSeq { get; set; }
            public int WidthSpace { get; set; }
            public int HeightPixel { get; set; }
        }
        public class Add
        {
            [Required]
            public int DashboardId { get; set; }
            [Required]
            public int WidgetCatalogId { get; set; }
            public int RowNo { get; set; }
            public int ColumnSeq { get; set; }
            [Required]
            public int WidthSpace { get; set; }
            [Required]
            public int HeightPixel { get; set; }
        }

        public class Schema_widgetCatalog
        {
            [Required]
            public int Id { get; set; }
            [Required]
            public int WidgetCatalogId { get; set; }
            public int RowNo { get; set; }
            public int ColumnSeq { get; set; }
            [Required]
            public int WidthSpace { get; set; }
            [Required]
            public int HeightPixel { get; set; }
        }

        public class Update
        {
            [Required]
            public int DashboardId { get; set; }
            public List<Schema_widgetCatalog> Widget { get; set; }
        }
        public List<Detail> getAllDashboardWidgetByDashboardId(int dashboardId)
        {
            DBHelper._DashboardWidget dbhelp = new DBHelper._DashboardWidget();

            return dbhelp.GetAllByDashboardId(dashboardId).Select(s => new Detail()
            {
                Id = s.Id,
                DashboardId = s.DashboardID,
                WidgetCatalogId = s.WidgetCatalogID,
                WidgetCatalogName = (s.WidgetCatalog == null ? "" : s.WidgetCatalog.Name),
                RowNo = s.RowNo,
                ColumnSeq = s.ColumnSeq,
                WidthSpace = s.WidthSpace,
                HeightPixel = s.HeightPixel                
            }).ToList<Detail>();

        }

        public Detail getDashboardWidgetById(int id)
        {
            DBHelper._DashboardWidget dbhelp = new DBHelper._DashboardWidget();
            DashboardWidgets dashboardWidget = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = dashboardWidget.Id,
                DashboardId = dashboardWidget.DashboardID,
                WidgetCatalogId = dashboardWidget.WidgetCatalogID,
                WidgetCatalogName = (dashboardWidget.WidgetCatalog == null ? "" : dashboardWidget.WidgetCatalog.Name),
                RowNo = dashboardWidget.RowNo,
                ColumnSeq = dashboardWidget.ColumnSeq,
                WidthSpace = dashboardWidget.WidthSpace,
                HeightPixel = dashboardWidget.HeightPixel
            };
        }

        public int addDashboardWidget(Add dashboardWidget)
        {
            DBHelper._DashboardWidget dbhelp = new DBHelper._DashboardWidget();
            var newDashboardWidget = new DashboardWidgets()
            {
                DashboardID = dashboardWidget.DashboardId,
                WidgetCatalogID = dashboardWidget.WidgetCatalogId,
                RowNo = dashboardWidget.RowNo,
                ColumnSeq = dashboardWidget.ColumnSeq,
                WidthSpace = dashboardWidget.WidthSpace,
                HeightPixel = dashboardWidget.HeightPixel
            };
            return dbhelp.Add(newDashboardWidget);
        }

        public void updateDashboardWidget(Update dashboardWidget)
        {            
            if (dashboardWidget != null && dashboardWidget.Widget != null)
            {
                DBHelper._DashboardWidget dbhelp = new DBHelper._DashboardWidget();
                List<DashboardWidgets> dashboardWidgetList = new List<DashboardWidgets>();

                foreach (var widget in dashboardWidget.Widget)
                {
                    DashboardWidgets existingWidget = new DashboardWidgets();
                    existingWidget.Id = widget.Id;
                    existingWidget.DashboardID = dashboardWidget.DashboardId;
                    existingWidget.WidgetCatalogID = widget.WidgetCatalogId;
                    existingWidget.RowNo = widget.RowNo;
                    existingWidget.ColumnSeq = widget.ColumnSeq;
                    existingWidget.WidthSpace = widget.WidthSpace;
                    existingWidget.HeightPixel = widget.HeightPixel;
                    dashboardWidgetList.Add(existingWidget);
                }

                dbhelp.Update(dashboardWidgetList);
            }            
        }

        public void deleteDashboardWidget(int id)
        {
            DBHelper._DashboardWidget dbhelp = new DBHelper._DashboardWidget();
            DashboardWidgets existingDashboardWidget = dbhelp.GetByid(id);

            dbhelp.Delete(existingDashboardWidget);
        }
    }
}