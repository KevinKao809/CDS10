using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;


namespace sfAdmin.Models
{
    public class DashboardModel
    {
        private StringBuilder WidgetJavaScriptFunction = new StringBuilder();
        private StringBuilder AlarmWidgetJavaScriptFunction = new StringBuilder();
        private StringBuilder WidgetHTMLContent = new StringBuilder();
        private int MessageCatalogId = 0;
        private List<MessageElementModel> MessageElements;

        public string GetWidgetHTMLContent()
        {
            return WidgetHTMLContent.ToString();
        }

        public string GetWidgetJavaScriptFunction()
        {
            return WidgetJavaScriptFunction.ToString();
        }

        public string GetAlarmWidgetJavaScriptFunction()
        {
            return AlarmWidgetJavaScriptFunction.ToString();
        }

        private string getMessageElementNameById(string IDList)
        {
            string[] IDs = IDList.Split('_');
            if (IDs.Length == 1)
            {   // Non-Child Message Element
                foreach (var element in MessageElements)
                    if (element.Id == int.Parse(IDs[0]))
                        return element.Name;
            }
            else
            {   // Handle Child Message Element
                foreach (var element in MessageElements)
                    if (element.ParentId == int.Parse(IDs[0]) && element.Id == int.Parse(IDs[1]))
                        return element.Name;
            }
            return null;
        }

        public async void GenerateCompanyWidgetHTMLContent(string widgetJson, string factoryString, CompanySession compSession)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            dynamic widgets = JsonConvert.DeserializeObject(widgetJson);
            string deleteIconHTML = "";
            if (canEdit(0))
                deleteIconHTML = "<div class=\"panel-header-icon-area\"><div class=\"ti-trash header-icon delete-panel-icon\"></div></div>";
            string widgetHeaderTemp = "<div class=\"panel-heading\"><div class=\"drag-area\"><span class=\"grippy\"></span></div><div class=\"title-area\">" +
                                    "<h3 class=\"panel-title\">{0}</h3></div>" +
                                    deleteIconHTML + "</div>";

            foreach (var widget in widgets)
            {
                try
                {

                    string endPoint = Global._widgetCatalogEndPoint + "/" + widget.WidgetCatalogId;
                    string widgetDataDef = await apiHelper.callAPIService("GET", endPoint, null);
                    dynamic widgetDataDefJson = JsonConvert.DeserializeObject(widgetDataDef);

                    if (widgetDataDefJson.MessageCatalogId != null && (int)widgetDataDefJson.MessageCatalogId != MessageCatalogId)
                    {
                        MessageCatalogId = widgetDataDefJson.MessageCatalogId;
                        string elementEndPoint = Global._messageFlatElementEndPoint + "/" + MessageCatalogId + "/Elements";
                        string messageElementString = await apiHelper.callAPIService("GET", elementEndPoint, null);
                        if (!string.IsNullOrEmpty(messageElementString))
                        {
                            var settings = new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore
                            };
                            MessageElements = JsonConvert.DeserializeObject<List<MessageElementModel>>(messageElementString, settings);
                        }
                    }

                    WidgetHTMLContent.AppendLine("<div class=\"grid-stack-item panel-wrapper\" data-companydashboardid=\"" + widget.DashboardId + "\" data-id=\"" + widget.Id + "\" data-gs-x=\"" + widget.RowNo + "\" data-gs-y=\"" + widget.ColumnSeq + "\" data-gs-width=\"" + widget.WidthSpace + "\" data-gs-height=\"" + widget.HeightPixel + "\" data-gs-min-height=\"2\" data-gs-min-width=\"2\"  data-widgetcatlogid=\"" + widget.WidgetCatalogId + "\">");
                    WidgetHTMLContent.AppendLine("<div class=\"grid-stack-item-content\">");

                    string widgetTitle = widgetDataDefJson.Name + ": Content Undefined";
                    if (widgetDataDefJson.Context.Title != null)
                    {
                        widgetTitle = widgetDataDefJson.Context.Title;   // Widget Title
                        WidgetHTMLContent.AppendLine(String.Format(widgetHeaderTemp, widgetTitle));
                    }
                    else
                    {
                        WidgetHTMLContent.AppendLine(String.Format(widgetHeaderTemp, widgetTitle));
                        WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item-content
                        WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item
                        continue;
                    }

                    string divID = "Chart_" + widget.Id;
                    switch ((int)widgetDataDefJson.WidgetClassKey)
                    {
                        case 100:           // "Company Alarm Card":
                            generateCompanyAlarmCardHTMLContent(widget);
                            break;
                        case 101:           // "Company Alarm List":
                            generateCompanyAlarmListHTMLContent(widget, widgetDataDefJson);
                            break;
                        case 102:           // "Company Equipment Card":
                            generateCompanyEquipmentCardHTMLContent(widget);
                            break;
                        case 103:           // "Company Factory Card":
                            generateCompanyFactoryCardHTMLContent(widget);
                            break;
                        case 104:           // "Company Factory List":
                            generateCompanyFactoryListHTMLContent(widget);
                            break;
                        case 105:           // "Company Map Card":
                            generateMapCardHTMLContent(widget, factoryString, widgetDataDefJson, "company");
                            break;
                        case 106:           // "Company Logo Card":
                            generateCompanyLogoCardHTMLContent(widget, compSession);
                            break;
                        case 107:           // "Company HTML Content":
                            generateCompanyHTMLContentHTMLContent(widget, widgetDataDefJson);
                            break;
                        case 108:           // "Preferred OEE Card"
                            generatePreferredOEECardHTMLContent(widgetDataDefJson, divID);
                            break;
                    }

                    WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item
                    WidgetHTMLContent.AppendLine("</div>");  // grid-stack-item-content
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private bool canEdit(int widgetLevel)
        {
            EmployeeSession empSession = null;
            empSession = EmployeeSession.LoadByJsonString(HttpContext.Current.Session["empSession"].ToString());
            string permissionList = empSession.permissions;

            String[] permissionArray = permissionList.Split(',');
            if (permissionArray.Contains("0"))
            {
                return true;
            }
            else
            {
                switch (widgetLevel)
                {
                    case 0: //Company Level
                        if (permissionArray.Contains("63"))
                        {
                            return true;
                        }
                        break;
                    case 1: //Factory Level
                        if (permissionArray.Contains("64"))
                        {
                            return true;
                        }
                        break;
                    case 2: //equipment Level
                        if (permissionArray.Contains("65"))
                        {
                            return true;
                        }
                        break;
                    default:
                        return false;
                }
            }
            return false;
        }

        private string generateDashBoardComponentHeader(string widgetTitle, int widgetLevel)
        {
            EmployeeSession empSession = null;
            empSession = EmployeeSession.LoadByJsonString(HttpContext.Current.Session["empSession"].ToString());

            string deleteIconHTML = "";
            string permissionList = empSession.permissions;

            String[] permissionArray = permissionList.Split(',');

            Boolean canEdit = false;

            if (permissionArray.Contains("0"))
            {
                canEdit = true;
            }
            else
            {
                switch (widgetLevel)
                {
                    case 0: //Company Level
                        if (permissionArray.Contains("63"))
                        {
                            canEdit = true;
                        }
                        break;
                    case 1: //Factory Level
                        if (permissionArray.Contains("64"))
                        {
                            canEdit = true;
                        }
                        break;
                    case 2: //equipment Level
                        if (permissionArray.Contains("65"))
                        {
                            canEdit = true;
                        }
                        break;
                    default:
                        canEdit = false;
                        break;
                }
            }


            if (canEdit)
            {
                deleteIconHTML = "<div class=\"panel-header-icon-area\">" +
                                        "<div class=\"ti-trash header-icon delete-panel-icon\"></div>" +
                                    "</div>";
            }

            string returnHtml = "<div class=\"panel-heading\">" +
                                    "<div class=\"drag-area\">" +
                                        "<span class=\"grippy\"></span>" +
                                    "</div>" +

                                    "<div class=\"title-area\"><h3 class=\"panel-title\">" + widgetTitle + "</h3></div>" +
                                    deleteIconHTML +
                                "</div>";
            return returnHtml;
        }

        private void generateCompanyLogoCardHTMLContent(dynamic widget, CompanySession compSession)
        {
            string scriptCode = "<div class =\"card-content\"><img src=\"" + compSession.photoURL + "\" class=\"img-responsive\" style=\"max-height:90%; max-width:90%; margin:auto\" /></div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateCompanyFactoryCardHTMLContent(dynamic widget)
        {
            string scriptCode = "<div class =\"card-content\"><div class=\"card-box widget-user\"><div class=\"text-center\"><h2 class=\"text-inverse\"><span id=\"ConnectedFactoryCard\">0</span> / <span id=\"FactoryCard\">0</span></h2><h5>connected / total</h5></div></div></div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateCompanyEquipmentCardHTMLContent(dynamic widget)
        {
            string scriptCode = "<div class =\"card-content\"><div class=\"card-box widget-user\"><div class=\"text-center\"><h2 class=\"text-custom\"><span id=\"ConnectedEquipmentCard\">0</span> / <span id=\"EquipmentCard\">0</span></h2><h5>connected / total</h5></div></div></div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateCompanyAlarmCardHTMLContent(dynamic widget)
        {
            string scriptCode = "<div class =\"card-content\"><div class=\"card-box widget-user\"><div class=\"text-center\"><h2 class=\"text-danger\"><span id=\"Alarm1MCard\">0</span> / <span id=\"Alarm24HCard\">0</span></h2><h5>1 min / 24 hours</h5></div></div></div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateCompanyFactoryListHTMLContent(dynamic widget)
        {
            string scriptCode = "<div class =\"card-content\" style=\"overflow-y: scroll;\"><ul class=\"list-group m-b-0 user-list\" id=\"FactoryList\"></ul></div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateCompanyAlarmListHTMLContent(dynamic widget, dynamic widgetDataDefJson)
        {
            string scriptCode = "<div class=\"card-content table-responsive\"><div class=\"card-box\"><table id=\"datatable-responsive\" name=\"alarmListTable\" class=\"table table-striped table-bordered dt-responsive nowrap\"><thead><tr><th>No</th><th>Factory</th><th>Equipment</th><th>Alarm</th><th>Alarm Description</th><th>Occur Time (Local)</th><th>Message Content</th></tr></thead><tbody id=\"AlarmList\"></tbody></table></div></div>";

            scriptCode = scriptCode + "<script type=\"text/javascript\">";
            scriptCode = scriptCode + "var _alarmDataSet = $('#datatable-responsive').DataTable();";
            scriptCode = scriptCode + "function LoadAlarmListIntoTable() {";
            scriptCode = scriptCode + "var endPoint = '/Dashboard/ReqAction?action=GetCompanyAlarm&dataRange=" + widgetDataDefJson.Context.DataRange + "';";
            scriptCode = scriptCode + "$.ajax({";
            scriptCode = scriptCode + "type: 'POST', url: endPoint + '&t=' + Date.now(), cache: false, contentType: false, processData: false,";
            scriptCode = scriptCode + "success: function (data) {";
            scriptCode = scriptCode + "_MessageAlarmObjs = $.parseJSON(jsonStringFilter(data));";
            scriptCode = scriptCode + "_MessageAlarmCount = _MessageAlarmObjs.length;";
            scriptCode = scriptCode + "for (var i in _MessageAlarmObjs) {";
            scriptCode = scriptCode + "_alarmDataSet.row.add([parseInt(i) + 1,getFactoryNameByEquipmentId(_MessageAlarmObjs[i].Message.equipmentId),_MessageAlarmObjs[i].Message.equipmentId,_MessageAlarmObjs[i].AlarmRuleCatalogName,_MessageAlarmObjs[i].AlarmRuleCatalogDescription,_MessageAlarmObjs[i].Message.msgTimestamp,'<input onClick=\"javascript: showMessageContentofAlert(' + i + ')\" type=\"button\" value=\"Message\" />']).draw(false);";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "_alarmDataSet.columns.adjust().order([ 0, 'desc' ]).draw();";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "});";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "function showMessageContentofAlert(item) {";
            scriptCode = scriptCode + "var content = '';";
            scriptCode = scriptCode + "$.each(_MessageAlarmObjs[item].Message, function (k, v) {";
            scriptCode = scriptCode + "content = content + k + ' = ' + v + '\\n';";
            scriptCode = scriptCode + "});";
            scriptCode = scriptCode + "alert(content);";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "LoadAlarmListIntoTable();";
            scriptCode = scriptCode + "</script>";

            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateCompanyHTMLContentHTMLContent(dynamic widget, dynamic widgetDataDefJson)
        {
            string scriptCode = "<div class =\"card-content\">";
            scriptCode = scriptCode + widgetDataDefJson.Context.HTMLContent;
            scriptCode = scriptCode + "</div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        public async void GenerateFactoryWidgetHTMLContent(string widgetJson, string factoryString, CompanySession compSession, string EquipmentString)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            dynamic widgets = JsonConvert.DeserializeObject(widgetJson);
            string deleteIconHTML = "";
            if (canEdit(1))
                deleteIconHTML = "<div class=\"panel-header-icon-area\"><div class=\"ti-trash header-icon delete-panel-icon\"></div></div>";
            string widgetHeaderTemp = "<div class=\"panel-heading\"><div class=\"drag-area\"><span class=\"grippy\"></span></div><div class=\"title-area\">" +
                                    "<h3 class=\"panel-title\">{0}</h3></div>" +
                                    deleteIconHTML + "</div>";

            foreach (var widget in widgets)
            {
                try
                {
                    string endPoint = Global._widgetCatalogEndPoint + "/" + widget.WidgetCatalogId;
                    string widgetDataDef = await apiHelper.callAPIService("GET", endPoint, null);
                    dynamic widgetDataDefJson = JsonConvert.DeserializeObject(widgetDataDef);

                    if (widgetDataDefJson.MessageCatalogId != null && (int)widgetDataDefJson.MessageCatalogId != MessageCatalogId)
                    {
                        MessageCatalogId = widgetDataDefJson.MessageCatalogId;
                        string elementEndPoint = Global._messageFlatElementEndPoint + "/" + MessageCatalogId + "/Elements";
                        string messageElementString = await apiHelper.callAPIService("GET", elementEndPoint, null);
                        if (!string.IsNullOrEmpty(messageElementString))
                        {
                            var settings = new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore
                            };
                            MessageElements = JsonConvert.DeserializeObject<List<MessageElementModel>>(messageElementString, settings);
                        }
                    }

                    WidgetHTMLContent.AppendLine("<div class=\"grid-stack-item panel-wrapper\" data-companydashboardid=\"" + widget.DashboardId + "\" data-id=\"" + widget.Id + "\" data-gs-x=\"" + widget.RowNo + "\" data-gs-y=\"" + widget.ColumnSeq + "\" data-gs-width=\"" + widget.WidthSpace + "\" data-gs-height=\"" + widget.HeightPixel + "\" data-gs-min-height=\"2\" data-gs-min-width=\"2\"  data-widgetcatlogid=\"" + widget.WidgetCatalogId + "\">");
                    WidgetHTMLContent.AppendLine("<div class=\"grid-stack-item-content\">");

                    string widgetTitle = widgetDataDefJson.Name + ": Content Undefined";
                    if (widgetDataDefJson.Context.Title != null)
                    {
                        widgetTitle = widgetDataDefJson.Context.Title;   // Widget Title
                        WidgetHTMLContent.AppendLine(String.Format(widgetHeaderTemp, widgetTitle));
                    }
                    else
                    {
                        WidgetHTMLContent.AppendLine(String.Format(widgetHeaderTemp, widgetTitle));
                        WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item-content
                        WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item
                        continue;
                    }

                    string divID = "Chart_" + widget.Id;
                    switch ((int)widgetDataDefJson.WidgetClassKey)
                    {
                        case 200:           // "Factory Alarm Card":
                            generateFactoryAlarmCardHTMLContent(widget);
                            break;
                        case 201:           // "Factory Alarm List":
                            generateFactoryAlarmListHTMLContent(widget, factoryString, widgetDataDefJson);
                            break;
                        case 202:           // "Factory Company Logo Card":
                            generateFactoryCompanyLogoCardHTMLContent(widget, compSession);
                            break;
                        case 203:           // "Factory Equipment Card":
                            generateFactoryEquipmentCardHTMLContent(widget);
                            break;
                        case 204:           // "Factory Equipment List":
                            generateFactoryEquipmentListHTMLContent(widget);
                            break;
                        case 205:           // "Factory Map Card":
                            generateMapCardHTMLContent(widget, factoryString, widgetDataDefJson, "factory");
                            break;
                        case 206:           // "Factory Photo Card":
                            generateFactoryLogoCardHTMLContent(widget);
                            break;
                        case 207:       // "Factory Video Card":
                            generateFactoryVideoCardHTMLContent(widget, widgetDataDefJson);
                            break;
                        case 208:           // "Factory HTML Content":
                            generateFactoryHTMLContentHTMLContent(widget, widgetDataDefJson);
                            break;
                        case 209:           // "Preferred OEE Card"
                            generatePreferredOEECardHTMLContent(widgetDataDefJson, divID);
                            break;
                        case 210:           // "All Equipment on Circle Map 
                            generateAllEquipmentCircleMapHTMLContent(widgetDataDefJson, divID, EquipmentString);
                            break;
                    }

                    WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item-content
                    WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        private void generateFactoryCompanyLogoCardHTMLContent(dynamic widget, CompanySession compSession)
        {
            string scriptCode = "<div id=\"CompanyDiv\" class =\"card-content\"><img src=\"" + compSession.photoURL + "\" class=\"img-responsive\" style=\"max-height:90%; max-width:90%; margin:auto\" /></div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateFactoryLogoCardHTMLContent(dynamic widget)
        {
            string scriptCode = "<div class =\"card-content\"><img id=\"factoryPhotoURL\" class=\"img-responsive\" style=\"max-height:90%; max-width:90%; margin:auto\" /></div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateFactoryEquipmentCardHTMLContent(dynamic widget)
        {
            string scriptCode = "<div class =\"card-content\"><div class=\"card-box widget-user\"><div class=\"text-center\"><h2 class=\"text-custom\"><span id=\"ConnectedEquipmentCard\">0</span> / <span id=\"EquipmentCard\">0</span></h2><h5>connected / total</h5></div></div></div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateFactoryAlarmCardHTMLContent(dynamic widget)
        {
            string scriptCode = "<div class =\"card-content\"><div class=\"card-box widget-user\"><div class=\"text-center\"><h2 class=\"text-danger\"><span id=\"Alarm1MCard\">0</span> / <span id=\"Alarm24HCard\">0</span></h2><h5>1 min / 24 hours</h5></div></div></div>";
            WidgetHTMLContent.Append(scriptCode);
        }              

        private void generateFactoryEquipmentListHTMLContent(dynamic widget)
        {
            string scriptCode = "<div style=\"overflow-y: scroll;\" class =\"card-content\"><ul class=\"list-group m-b-0 user-list\" id=\"EquipmentList\"></ul></div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateFactoryAlarmListHTMLContent(dynamic widget, string factoryString, dynamic widgetDataDefJson)
        {
            dynamic factoryObj = JObject.Parse(factoryString);

            string scriptCode = "<div class=\"card-box table-responsive card-content\"><div class=\"col-sm-12\"><table id=\"datatable-responsive\" name=\"alarmListTable\" class=\"table table-striped table-bordered dt-responsive nowrap\"><thead><tr><th>No</th><th>Equipment</th><th>Alarm</th><th>Alarm Description</th><th>Occur Time (Local)</th><th>Message Content</th></tr></thead><tbody id=\"AlarmList\"></tbody></table></div></div>";

            scriptCode = scriptCode + "<script type=\"text/javascript\">";
            scriptCode = scriptCode + "var _alarmDataSet = $('#datatable-responsive').DataTable(); var _MessageAlarmObjs=$.parseJSON(\"[]\");";
            scriptCode = scriptCode + "function LoadAlarmListIntoTable() {";
            scriptCode = scriptCode + "var endPoint = '/Dashboard/ReqAction?action=GetFactoryAlarm&Id=" + factoryObj.Id + "&dataRange=" + widgetDataDefJson.Context.DataRange + "';";
            scriptCode = scriptCode + "$.ajax({";
            scriptCode = scriptCode + "type: 'POST', url: endPoint + '&t=' + Date.now(), cache: false, contentType: false, processData: false,";
            scriptCode = scriptCode + "success: function (data) {";
            scriptCode = scriptCode + "_MessageAlarmObjs = $.parseJSON(jsonStringFilter(data));";
            scriptCode = scriptCode + "for (var i in _MessageAlarmObjs) {";
            scriptCode = scriptCode + "_alarmDataSet.row.add([parseInt(i) + 1,_MessageAlarmObjs[i].Message.equipmentId,_MessageAlarmObjs[i].AlarmRuleCatalogName,_MessageAlarmObjs[i].AlarmRuleCatalogDescription,_MessageAlarmObjs[i].Message.msgTimestamp,'<input onClick=\"javascript: showMessageContentofAlert(' + i + ')\" type=\"button\" value=\"Message\" />']).draw(false);";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "_alarmDataSet.columns.adjust().order([0, 'desc']).draw();";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "});";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "function showMessageContentofAlert(item) {";
            scriptCode = scriptCode + "var content = '';";
            scriptCode = scriptCode + "$.each(_MessageAlarmObjs[item].Message, function (k, v) {";
            scriptCode = scriptCode + "content = content + k + ' = ' + v + '\\n';";
            scriptCode = scriptCode + "});";
            scriptCode = scriptCode + "alert(content);";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "LoadAlarmListIntoTable();";
            scriptCode = scriptCode + "</script>";

            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateFactoryVideoCardHTMLContent(dynamic widget, dynamic widgetDataDefJson)
        {
            string scriptCode = "<video class =\"card-content\" style=\"width:100%;\" autoplay loop><source src=\"" + widgetDataDefJson.Context.VideoSource + "\" type=\"video/mp4\"></video>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateFactoryHTMLContentHTMLContent(dynamic widget, dynamic widgetDataDefJson)
        {
            string scriptCode = "<div class =\"card-content\">";
            scriptCode = scriptCode + widgetDataDefJson.Context.HTMLContent;
            scriptCode = scriptCode + "</div>";
            WidgetHTMLContent.Append(scriptCode);
        }

        public async void GenerateWidgetHTMLContent(string equipmentString, string widgetJson)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            dynamic widgets = JsonConvert.DeserializeObject(widgetJson);
            string deleteIconHTML = "";
            if (canEdit(2))
                deleteIconHTML = "<div class=\"panel-header-icon-area\"><div class=\"ti-trash header-icon delete-panel-icon\"></div></div>";
            string widgetHeaderTemp = "<div class=\"panel-heading\"><div class=\"drag-area\"><span class=\"grippy\"></span></div><div class=\"title-area\">" +
                                    "<h3 class=\"panel-title\">{0}</h3></div>" +
                                    deleteIconHTML + "</div>";

            foreach (var widget in widgets)
            {
                try
                {
                    string endPoint = Global._widgetCatalogEndPoint + "/" + widget.WidgetCatalogId;
                    string widgetDataDef = await apiHelper.callAPIService("GET", endPoint, null);
                    dynamic widgetDataDefJson = JsonConvert.DeserializeObject(widgetDataDef);

                    if (widgetDataDefJson.MessageCatalogId != null && (int)widgetDataDefJson.MessageCatalogId != MessageCatalogId)
                    {
                        MessageCatalogId = widgetDataDefJson.MessageCatalogId;
                        string elementEndPoint = Global._messageFlatElementEndPoint + "/" + MessageCatalogId + "/Elements";
                        string messageElementString = await apiHelper.callAPIService("GET", elementEndPoint, null);
                        if (!string.IsNullOrEmpty(messageElementString))
                        {
                            var settings = new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                MissingMemberHandling = MissingMemberHandling.Ignore
                            };
                            MessageElements = JsonConvert.DeserializeObject<List<MessageElementModel>>(messageElementString, settings);
                        }


                    }
                    WidgetHTMLContent.AppendLine("<div class=\"grid-stack-item panel-wrapper\" data-companydashboardid=\"" + widget.DashboardId + "\" data-id=\"" + widget.Id + "\" data-gs-x=\"" + widget.RowNo + "\" data-gs-y=\"" + widget.ColumnSeq + "\" data-gs-width=\"" + widget.WidthSpace + "\" data-gs-height=\"" + widget.HeightPixel + "\" data-gs-min-height=\"2\" data-gs-min-width=\"2\"  data-widgetcatlogid=\"" + widget.WidgetCatalogId + "\">");
                    WidgetHTMLContent.AppendLine("<div class=\"grid-stack-item-content\">");

                    string widgetTitle = widgetDataDefJson.Name + ": Content Undefined";
                    if (widgetDataDefJson.Context.Title != null)
                    {
                        widgetTitle = widgetDataDefJson.Context.Title;   // Widget Title
                        WidgetHTMLContent.AppendLine(String.Format(widgetHeaderTemp, widgetTitle));
                    }
                    else
                    {
                        WidgetHTMLContent.AppendLine(String.Format(widgetHeaderTemp, widgetTitle));
                        WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item-content
                        WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item
                        continue;
                    }

                    dynamic equipmentObj = JObject.Parse(equipmentString);
                    string divID = "Chart_" + widget.Id;
                    switch ((int)widgetDataDefJson.WidgetClassKey)
                    {
                        case 300:           // "Alarm Card":
                            generateAlarmCardHTMLContent(widget, widgetDataDefJson, divID, equipmentObj);
                            break;
                        case 301:           // "Bar Chart":
                            generateBarChartHTMLContent(widget, widgetDataDefJson, divID);
                            break;
                        case 302:           // "Basic Card":
                            generateBasicCardHTMLContent(widget, widgetDataDefJson, divID, equipmentObj);
                            break;
                        case 303:           // "Map Card":
                            generateMapCardHTMLContent(widget, equipmentString, widgetDataDefJson, "equipment");
                            break;
                        case 305:           // "Line Chart":
                            generateLineChartHTMLContent(widgetDataDefJson, divID);
                            break;
                        case 306:           // "Percentage Card":
                            generatePercentageCardHTMLContent(widget, widgetDataDefJson, divID);
                            break;
                        case 307:           // "Pie Chart":
                            generatePieChartHTMLContent(widget, widgetDataDefJson, divID);
                            break;
                        case 308:           // "Text Card":
                            generateTextCardHTMLContent(widget, widgetDataDefJson, divID);
                            break;
                        case 309:           // "Text List Card":
                            generateTextListCardHTMLContent(widget, widgetDataDefJson, divID);
                            break;
                        case 310:           // "Alarm List"
                            generateAlarmListHTMLContent(widget, widgetDataDefJson, divID, equipmentObj);
                            break;
                        case 311:           // "Map Location Track"
                            generateMapLocationTraceHTMLContent(widget, widgetDataDefJson, divID, equipmentObj);
                            break;
                        case 312:           // "Simple OEE Card"
                            generateSimpleOEECardHTMLContent(widgetDataDefJson, divID);
                            break;
                        case 313:           // "Preferred OEE Card"
                            generatePreferredOEECardHTMLContent(widgetDataDefJson, divID);
                            break;
                        case 314:           // "HTML Content Card"
                            generateHTMLContentCardHTMLContent(widgetDataDefJson, divID);
                            break;
                        case 315:           // " Map Circle"
                            generateSingleEquipmentCircleMapHTMLContent(widgetDataDefJson, divID, equipmentString);
                            break;
                    }
                    WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item-content
                    WidgetHTMLContent.AppendLine("</div>");  // End of grid-stack-item
                }
                catch
                {
                    ;
                }
            }
        }

        private void generateAlarmCardHTMLContent(dynamic widget, dynamic widgetDataDefJson, string divID, dynamic equipmentObj)
        {
            int showAlarmDuration = 30000;

            if (widgetDataDefJson.Context.ShowAlarmDuration != null)
            {
                string durationSec = widgetDataDefJson.Context.ShowAlarmDuration;
                showAlarmDuration = int.Parse(durationSec) * 1000;
            }

            string scriptCode = "<div id=\"AlarmCardDiv1_" + divID + "\" style =\"background-color:#" + widgetDataDefJson.Context.NormalBGColorCode + "\" class =\"card-content\">";
            scriptCode = scriptCode + "<div id=\"AlarmCardDiv2_" + divID + "\" class=\"card-box widget-user\" style=\"background-color:#" + widgetDataDefJson.Context.NormalBGColorCode + "\">";
            scriptCode = scriptCode + "<div class=\"text-center\">";
            scriptCode = scriptCode + "<h3 id=\"AlarmText_" + divID + "\" style=\"color:#" + widgetDataDefJson.Context.TextColorCode + "\"></h3>";
            scriptCode = scriptCode + "<h5 id=\"AlarmTimeText_" + divID + "\" style=\"color:#" + widgetDataDefJson.Context.TextColorCode + "\"></h5>";
            scriptCode = scriptCode + "</div></div></div>";
            AlarmWidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            scriptCode = scriptCode + "<script type=\"text/javascript\">";
            scriptCode = scriptCode + "var alarm_" + divID + "=0;";
            scriptCode = scriptCode + "function update_" + divID + "(message) {";
            scriptCode = scriptCode + "if (message.AlarmRuleCatalogId != null) {";
            scriptCode = scriptCode + "if (message.Message.equipmentId == '" + equipmentObj.EquipmentId + "') {";
            scriptCode = scriptCode + "$('#AlarmText_" + divID + "').html(message.AlarmRuleCatalogName);";
            scriptCode = scriptCode + "$('#AlarmTimeText_" + divID + "').html(message.Message.msgTimestamp); ";
            scriptCode = scriptCode + "$('#AlarmCardDiv1_" + divID + "').css('background-color', '#" + widgetDataDefJson.Context.BGColorCode + "');";
            scriptCode = scriptCode + "$('#AlarmCardDiv2_" + divID + "').css('background-color', '#" + widgetDataDefJson.Context.BGColorCode + "');";
            scriptCode = scriptCode + "clearTimeout(alarm_" + divID + "); ";
            scriptCode = scriptCode + "alarm_" + divID + "= 0;";
            scriptCode = scriptCode + "alarm_" + divID + "= setTimeout(function () { $('#AlarmCard').html(''); $('#AlarmDT').html('');";
            scriptCode = scriptCode + "$('#AlarmCardDiv1_" + divID + "').css('background-color', '#" + widgetDataDefJson.Context.NormalBGColorCode + "');";
            scriptCode = scriptCode + "$('#AlarmCardDiv2_" + divID + "').css('background-color', '#" + widgetDataDefJson.Context.NormalBGColorCode + "');";
            scriptCode = scriptCode + "}, " + showAlarmDuration + "); ";
            scriptCode = scriptCode + "}}}</script>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateTextListCardHTMLContent(dynamic widget, dynamic widgetDataDefJson, string divID)
        {
            string scriptCode = "<div class=\"card-content\"><ul class=\"list-group m-b-0 user-list\">";
            string updateMessageScript = "";
            foreach (var item in widgetDataDefJson.Context.Texts)
            {
                scriptCode = scriptCode + "<li class=\"list-group-item\"><a href=\"#\" class=\"user-list-item\"><div class=\"avatar text-center\">";
                scriptCode = scriptCode + "<i class=\"zmdi zmdi-circle\" style=\"color:#" + item.BallColorCode + "\"></i></div>";
                scriptCode = scriptCode + "<div class=\"user-desc\">";
                scriptCode = scriptCode + "<span class=\"name\" id=\"" + item.Field + "_" + divID + "\"></span>";
                scriptCode = scriptCode + "<span class=\"desc\">" + item.Display + "</span>";
                scriptCode = scriptCode + "</div></a></li>";

                string elementName = getMessageElementNameById((string)item.Field);
                updateMessageScript = updateMessageScript + "if (message." + elementName + " != null)";
                updateMessageScript = updateMessageScript + "document.getElementById('" + item.Field + "_" + divID + "').innerHTML = message." + elementName + ";";
            }
            scriptCode = scriptCode + "</ul></div>";
            WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            scriptCode = scriptCode + "<script type=\"text/javascript\">";
            scriptCode = scriptCode + "function update_" + divID + "(message)";
            scriptCode = scriptCode + "{";
            scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
            scriptCode = scriptCode + updateMessageScript;
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "</script>";

            WidgetHTMLContent.Append(scriptCode);
        }

        private async Task<List<double>> getEquipmentLocation(dynamic equipmentObj)
        {
            List<double> location = new List<double>();
            if (equipmentObj.Latitude != null && equipmentObj.Longitude != null)
            {
                location.Add((double)equipmentObj.Latitude);
                location.Add((double)equipmentObj.Longitude);
            }
            else
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string endPoint = Global._factoryEndPoint + "/" + (string)equipmentObj.FactoryId;
                string factoryJson = await apiHelper.callAPIService("get", endPoint, null);
                dynamic factoryObj = JsonConvert.DeserializeObject(factoryJson);
                if (factoryObj.Latitude != null && factoryObj.Longitude != null)
                {
                    location.Add((double)factoryObj.Latitude);
                    location.Add((double)factoryObj.Longitude);
                }
                else
                {
                    location.Add(0);
                    location.Add(0);
                }
            }
            return location;
        }      

        private async void generateMapLocationTraceHTMLContent(dynamic widget, dynamic widgetDataDefJson, string divID, dynamic equipmentObj)
        {
            List<double> location = await getEquipmentLocation(equipmentObj);
            string scriptCode = "<div id='mapLT' class =\"card-content\"></div>";
            scriptCode = scriptCode + "<script>var mapLT, infowindowLT, equipmentPointLT, equipmentMarkerLT;";
            scriptCode = scriptCode + "var equipmentIdLT = '" + equipmentObj.Name + "';";
            scriptCode = scriptCode + "var equipmentLat = " + location[0] + ";";
            scriptCode = scriptCode + "var equipmentLng = " + location[1] + ";";
            scriptCode = scriptCode + "var movingPath;";
            scriptCode = scriptCode + "var mapZoomLT = " + widgetDataDefJson.Context.MapZoom + ";";
            scriptCode = scriptCode + "function initmapLT() { equipmentPointLT = new google.maps.LatLng(parseFloat(equipmentLat), parseFloat(equipmentLng)); mapLT = new google.maps.Map(document.getElementById('mapLT'), {center: equipmentPointLT, zoom: mapZoomLT});";
            scriptCode = scriptCode + "var pinColor = '01DF01';";
            scriptCode = scriptCode + "var pinImage = new google.maps.MarkerImage('https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|' + pinColor, new google.maps.Size(20, 34), new google.maps.Point(0, 0), new google.maps.Point(10, 17));";
            scriptCode = scriptCode + "equipmentMarkerLT = new google.maps.Marker({position: equipmentPointLT,map: mapLT,icon: pinImage,id: equipmentIdLT});";
            scriptCode = scriptCode + "google.maps.event.addListener(equipmentMarkerLT, 'click', function () {showInfoBox(mapLT, equipmentMarkerLT.id);});";
            scriptCode = scriptCode + "movingPath = new google.maps.Polyline({geodesic: true, strokeColor: '#FF0000', strokeOpacity: 1.0, strokeWeight: 2});";
            scriptCode = scriptCode + "infowindowLT = new google.maps.InfoWindow({content: ''});}";
            scriptCode = scriptCode + "function showInfoBox(mapLT, Id) {infowindowLT.open(mapLT); infowindowLT.setContent(Id); infowindowLT.setPosition(equipmentPointLT);}";
            WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            scriptCode = scriptCode + "function update_" + divID + "(message) {";
            scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
            scriptCode = scriptCode + "var latField = message." + getMessageElementNameById((string)widgetDataDefJson.Context.Latitude) + ";";
            scriptCode = scriptCode + "var lngField = message." + getMessageElementNameById((string)widgetDataDefJson.Context.Longitude) + ";";
            scriptCode = scriptCode + "if (latField == null || lngField == null) { return; }";
            scriptCode = scriptCode + "equipmentPointLT = new google.maps.LatLng(parseFloat(latField), parseFloat(lngField)); mapLT.setCenter(equipmentPointLT); equipmentMarkerLT.setPosition(equipmentPointLT); movingPath.getPath().push(equipmentPointLT); movingPath.setMap(mapLT);}";
            scriptCode = scriptCode + "</script>";
            scriptCode = scriptCode + "<script src='https://maps.googleapis.com/maps/api/js?key=" + Global._sfGoogleMapAPIKey + "&callback=initmapLT' async defer></script>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateBasicCardHTMLContent(dynamic widget, dynamic widgetDataDefJson, string divID, dynamic equipmentObj)
        {
            string equipPhoto = "/assets/images/default/equipment-128.png";
            if (equipmentObj.PhotoUrl != null)
                equipPhoto = (string)equipmentObj.PhotoUrl;
            string scriptCode = "<div class=\"col-sm-12 card-content\" style=\"background-color: white;\">";
            scriptCode = scriptCode + "<div>";
            scriptCode = scriptCode + "<table border=\"0\" width=\"100%\"><tr><td width=\"40%\" align=\"center\" valign=\"middle\"></td><td width=\"20%\" align=\"center\" valign=\"middle\">Online</td><td width=\"20%\" align=\"center\" valign=\"middle\">Message</td><td width=\"20%\" align=\"center\" valign=\"middle\">Alarm</td></tr>";
            scriptCode = scriptCode + "<tr><td rowspan=\"4\" align=\"center\" valign=\"middle\"><img src=\"" + equipPhoto + "\" class=\"img-responsive\" class=\"img-responsive\" style=\"padding: 2px 2px 2px 2px;\" /></td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\"><div class=\"radio radio-success\"><input id=\"lightConnected\" type=\"radio\" disabled /><label></label></div></td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\"><div class=\"radio radio-warning\"><input id=\"lightOnMessage\" type=\"radio\" disabled /><label></label></div></td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\"><div class=\"radio radio-danger\"><input id=\"lightAlert\" type=\"radio\" disabled /><label></label></div></td></tr>";
            scriptCode = scriptCode + "<tr><td colspan=\"3\" height=\"5\"></td></tr><tr><td align=\"center\" valign=\"middle\">Run</td><td align=\"center\" valign=\"middle\">Idle</td><td align=\"center\" valign=\"middle\">Stop</td></tr>";
            scriptCode = scriptCode + "<tr><td align=\"center\" valign=\"middle\" height=\"20\" bgcolor=\"#298A08\"><div id=\"statusRun\"></div></td><td align=\"center\" valign=\"middle\" height=\"20\" bgcolor=\"#B45F04\"><div id=\"statusIdle\"></div></td><td align=\"center\" valign=\"middle\" height=\"20\" bgcolor=\"#8A0808\"><div id=\"statusStop\"></div></td></tr></table><br />";
            scriptCode = scriptCode + "<table border=\"1\" width=\"100%\"><tr><td width=\"33%\" align=\"left\" valign=\"middle\" style=\"padding: 2px 2px 2px 2px;\">Factory</td>";
            scriptCode = scriptCode + "<td width=\"67%\" align=\"right\" valign=\"middle\" style=\"padding: 2px 2px 2px 2px;\">" + (string)equipmentObj.FactoryName + "</td></tr>";
            scriptCode = scriptCode + "<tr><td width=\"33%\" align=\"left\" valign=\"middle\" style=\"padding: 2px 2px 2px 2px;\">Equipment Class</td>";
            scriptCode = scriptCode + "<td width=\"67%\" align=\"right\" valign=\"middle\" style=\"padding: 2px 2px 2px 2px;\">" + (string)equipmentObj.EquipmentClassName + "</td></tr>";
            scriptCode = scriptCode + "<tr><td width=\"33%\" align=\"left\" valign=\"middle\" style=\"padding: 2px 2px 2px 2px;\">Name</td>";
            scriptCode = scriptCode + "<td width=\"67%\" align=\"right\" valign=\"middle\" style=\"padding: 2px 2px 2px 2px;\">" + (string)equipmentObj.Name + "</td></tr></table></div></div>";
            scriptCode = scriptCode + "<script type=\"text/javascript\">";
            scriptCode = scriptCode + "var lastUpdateDT, maxIdleSec=300;";
            WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            AlarmWidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            scriptCode = scriptCode + "function update_" + divID + "(message)";
            scriptCode = scriptCode + "{";
            scriptCode = scriptCode + "lastUpdateDT = new Date();";
            scriptCode = scriptCode + "document.getElementById('lightConnected').checked = true;";
            scriptCode = scriptCode + "document.getElementById('lightOnMessage').checked = true;";
            scriptCode = scriptCode + "setTimeout(function () { document.getElementById('lightOnMessage').checked = false; }, 1500);";
            scriptCode = scriptCode + "if (message.AlarmRuleCatalogId != null) {";
            scriptCode = scriptCode + "document.getElementById('lightAlert').checked = true;";
            scriptCode = scriptCode + "setTimeout(function () { document.getElementById('lightAlert').checked = false; }, 3000);}";
            scriptCode = scriptCode + "if (message.equipmentRunStatus == -1) {$('#statusRun').removeClass('flashRun');$('#statusIdle').removeClass('flashIdle');$('#statusStop').addClass('flashStop');} ";
            scriptCode = scriptCode + "else if (message.equipmentRunStatus == 0) {$('#statusRun').removeClass('flashRun');$('#statusIdle').addClass('flashIdle');$('#statusStop').removeClass('flashStop');} ";
            scriptCode = scriptCode + "else if (message.equipmentRunStatus == 1) {$('#statusRun').addClass('flashRun');$('#statusIdle').removeClass('flashIdle');$('#statusStop').removeClass('flashStop');}";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "function CheckEquipmentDisconnect()";
            scriptCode = scriptCode + "{";
            scriptCode = scriptCode + "var nowDT = new Date();";
            scriptCode = scriptCode + "var difInSec = (nowDT.getTime() - lastUpdateDT.getTime()) / 1000;";
            scriptCode = scriptCode + "if (difInSec > maxIdleSec) {";
            scriptCode = scriptCode + "$('#statusRun').removeClass('flashRun');$('#statusIdle').removeClass('flashIdle');$('#statusStop').removeClass('flashStop');";
            scriptCode = scriptCode + "document.getElementById('lightConnected').checked = false;";
            scriptCode = scriptCode + "}}";
            scriptCode = scriptCode + "setInterval(function () { CheckEquipmentDisconnect() }, 20000);";
            scriptCode = scriptCode + " </script>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateTextCardHTMLContent(dynamic widget, dynamic widgetDataDefJson, string divID)
        {
            string scriptCode = "<div class =\"card-content\">";
            scriptCode = scriptCode + "<div class=\"card-box widget-user\">";
            scriptCode = scriptCode + "<div class=\"text-center\">";
            scriptCode = scriptCode + "<h2 id=\"" + divID + "_h2\" style=\"color:#" + widgetDataDefJson.Context.FGColorCode + "\" ></h2>";
            scriptCode = scriptCode + "<h5>" + widgetDataDefJson.Context.Title + "</h5>";
            scriptCode = scriptCode + "</div></div></div>";
            scriptCode = scriptCode + "<script type=\"text/javascript\">";
            WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            scriptCode = scriptCode + "function update_" + divID + "(message)";
            scriptCode = scriptCode + "{";
            scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
            scriptCode = scriptCode + "if (message." + getMessageElementNameById((string)widgetDataDefJson.Context.Field) + " == null) { return; }";
            scriptCode = scriptCode + "document.getElementById('" + divID + "_h2').innerHTML = message." + getMessageElementNameById((string)widgetDataDefJson.Context.Field) + ";";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "</script>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateAlarmListHTMLContent(dynamic widget, dynamic widgetDataDefJson, string divID, dynamic equipmentObj)
        {
            string scriptCode = "<div class=\"card-content\"><div class=\"card-box table-responsive\"><div class=\"col-sm-12\"><table id=\"datatable-responsive\" name=\"alarmListTable\" class=\"table table-striped table-bordered dt-responsive nowrap\"><thead><tr><th>No</th><th>Equipment</th><th>Alarm</th><th>Alarm Description</th><th>Occur Time (Local)</th><th>Message Content</th></tr></thead><tbody id=\"AlarmList\"></tbody></table></div></div></div>";

            scriptCode = scriptCode + "<script type=\"text/javascript\">";
            scriptCode = scriptCode + "var _alarmDataSet = $('#datatable-responsive').DataTable(); var _MessageAlarmObjs=$.parseJSON(\"[]\");";
            scriptCode = scriptCode + "function LoadAlarmListIntoTable() {";
            scriptCode = scriptCode + "var endPoint = '/Dashboard/ReqAction?action=GetEquipmentAlarm&Id=" + equipmentObj.EquipmentId + "&dataRange=" + widgetDataDefJson.Context.DataRange + "';";
            scriptCode = scriptCode + "$.ajax({";
            scriptCode = scriptCode + "type: 'POST', url: endPoint + '&t=' + Date.now(), cache: false, contentType: false, processData: false,";
            scriptCode = scriptCode + "success: function (data) {";
            scriptCode = scriptCode + "_MessageAlarmObjs = $.parseJSON(jsonStringFilter(data));";
            scriptCode = scriptCode + "for (var i in _MessageAlarmObjs) {";
            scriptCode = scriptCode + "_alarmDataSet.row.add([parseInt(i) + 1,_MessageAlarmObjs[i].Message.equipmentId,_MessageAlarmObjs[i].AlarmRuleCatalogName,_MessageAlarmObjs[i].AlarmRuleCatalogDescription,_MessageAlarmObjs[i].Message.msgTimestamp,'<input onClick=\"javascript: showMessageContentofAlert(' + i + ')\" type=\"button\" value=\"Message\" />']).draw(false);";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "_alarmDataSet.columns.adjust().order([0, 'desc']).draw();";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "});";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "function showMessageContentofAlert(item) {";
            scriptCode = scriptCode + "var content = '';";
            scriptCode = scriptCode + "$.each(_MessageAlarmObjs[item].Message, function (k, v) {";
            scriptCode = scriptCode + "content = content + k + ' = ' + v + '\\n';";
            scriptCode = scriptCode + "});";
            scriptCode = scriptCode + "alert(content);";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "LoadAlarmListIntoTable();";
            AlarmWidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            scriptCode = scriptCode + "function update_" + divID + "(DeviceMessage) {";
            scriptCode = scriptCode + "_alarmDataSet.row.add([";
            scriptCode = scriptCode + "_MessageAlarmObjs.length + 1,";
            scriptCode = scriptCode + "DeviceMessage.Message.equipmentId,";
            scriptCode = scriptCode + "DeviceMessage.AlarmRuleCatalogName,";
            scriptCode = scriptCode + "DeviceMessage.AlarmRuleCatalogDescription,";
            scriptCode = scriptCode + "DeviceMessage.Message.msgTimestamp,'<input onClick=\"javascript: showMessageContentofAlert(' + _MessageAlarmObjs.length + ')\" type=\"button\" value=\"Message\" />']).order([0, 'desc']).draw(false);";
            scriptCode = scriptCode + "_MessageAlarmObjs.push(DeviceMessage);";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "</script>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateBarChartHTMLContent(dynamic widget, dynamic widgetDataDefJson, string divID)
        {
            string dataVariableScript = "", datasetVariableMemeber = "", seriesSetting = "", xaxisSetting = "", yaxisSetting = "", barsSetting = "", legendSetting = "", gridSetting = "", optionVariable = "";
            string x_DateType = ((string)widgetDataDefJson.Context.Axis_X_DataType).ToLower();
            int i = 0;
            foreach (var item in widgetDataDefJson.Context.Axis_Y_Values)   //多個 Y 軸資料
            {
                i++;
                //建立每個 Y 軸資料陣列變數
                dataVariableScript = dataVariableScript + "var data_" + divID + "_" + (string)item.Axis_Y + " = [];";
                //設定每個 Y 軸資料來源, 以及顯示外觀. 來源 : 上一行 data, 外觀 : Bar Chart
                datasetVariableMemeber = datasetVariableMemeber + "{label: \"" + item.Display + "\", data: data_" + divID + "_" + (string)item.Axis_Y + ", color: \"#" + item.ColorCode + "\" },";
                //設定每個 Y 軸刻度顯示外觀 (第一個刻度顯示於左側, 其餘顯示於右側
                if (i == 1)
                    yaxisSetting = yaxisSetting + "{position: \"left\", color: \"black\",  axisLabelUseCanvas: true, axisLabelFontSizePixels: 12, axisLabelFontFamily: 'Verdana, Arial', axisLabelPadding: 3},";
                else
                    yaxisSetting = yaxisSetting + "{position: \"right\", color: \"black\",  axisLabelUseCanvas: true, axisLabelFontSizePixels: 12, axisLabelFontFamily: 'Verdana, Arial', axisLabelPadding: 3},";
            }
            datasetVariableMemeber = datasetVariableMemeber.Substring(0, datasetVariableMemeber.Length - 1); // 去掉多餘的 ','
            yaxisSetting = "yaxis: [" + yaxisSetting.Substring(0, yaxisSetting.Length - 1) + "]"; // 去掉多餘的 ','

            //設定 X 軸顯示外觀.                        
            if (x_DateType == "datetime")
            {
                xaxisSetting = "xaxis: {mode: \"time\", timeformat: \"%M:%S\",tickSize: [10, \"second\"], tickLength: 2, ";// X 軸為時間資料時, 設定 X 軸屬性   
                barsSetting = "bars:{barWidth:" + widgetDataDefJson.Context.BarSpace * 500 + "}";
            }
            else
            {
                xaxisSetting = "xaxis: {";
                barsSetting = "bars:{barWidth:" + widgetDataDefJson.Context.BarSpace + "}";
            }
            xaxisSetting = xaxisSetting + "axisLabel: \"" + widgetDataDefJson.Context.Axis_X_Display + "\", axisLabelUseCanvas: true, axisLabelFontSizePixels: 12, axisLabelFontFamily: 'Verdana, Arial', axisLabelPadding: 10, color: \"black\"}";  //設定 X 軸顯示屬性

            //設定 Series, Legend 以及 Grid
            seriesSetting = "series: {bars: {show:true}}";
            legendSetting = "legend: {noColumns: 0, labelBoxBorderColor: \"#000000\", position: \"nw\"}";
            gridSetting = "grid: {hoverable: true, borderWidth: 3, backgroundColor: { colors: [\"#ffffff\", \"#EDF5FF\"] },  axisMargin: 20}";

            //組合 Option Variable
            optionVariable = "var options_" + divID + " = {" + seriesSetting + "," + barsSetting + "," + xaxisSetting + "," + yaxisSetting + "," + legendSetting + "," + gridSetting + "};";

            //組合 Script
            string scriptCode = "<script type=\"text/javascript\">";
            scriptCode = scriptCode + dataVariableScript;
            scriptCode = scriptCode + optionVariable;
            scriptCode = scriptCode + "var totalPoints = 20;";         //設定此 Widget 最多顯示的 X 軸資料數量
            WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱

            //以下為 JavaScript Function
            scriptCode = scriptCode + "function update_" + divID + "(message)";     // 函數開始, 必須使用 'message' 當變數名稱
            scriptCode = scriptCode + "{";
            scriptCode = scriptCode + "if (data_" + divID + "_" + (string)widgetDataDefJson.Context.Axis_Y_Values[0].Axis_Y + ".length == totalPoints) {";
            foreach (var item in widgetDataDefJson.Context.Axis_Y_Values)
            {
                scriptCode = scriptCode + "data_" + divID + "_" + (string)item.Axis_Y + ".shift();";        //如果目前 X 軸筆數已滿, 刪除第一筆
            }
            scriptCode = scriptCode + "}";
            foreach (var item in widgetDataDefJson.Context.Axis_Y_Values)
            {
                if (x_DateType == "datetime")
                {       // gd 是 JavaScript 函數, 轉換 DateTime Sting 成 TimeStamp
                    scriptCode = scriptCode + "var data = [gd(message." + getMessageElementNameById((string)widgetDataDefJson.Context.Axis_X) + ")" + ", message." + getMessageElementNameById((string)item.Axis_Y) + "];";
                }
                else
                    scriptCode = scriptCode + "var data = [message." + getMessageElementNameById((string)widgetDataDefJson.Context.Axis_X) + ", message." + getMessageElementNameById((string)item.Axis_Y) + "];";
                scriptCode = scriptCode + "data_" + divID + "_" + (string)item.Axis_Y + ".push(data);";     //Push 目前資料到各個陣列裡
            }
            scriptCode = scriptCode + "var dataSet_" + divID + " = [" + datasetVariableMemeber + "];";
            scriptCode = scriptCode + "$.plot($(\"#" + divID + "\"), dataSet_" + divID + ", options_" + divID + ");";   // 呼叫 plot 更新 UI
            scriptCode = scriptCode + "}";
            // JavaScript Function End
            scriptCode = scriptCode + "</script>";
            string flotChartDiv = "<div id=\"" + divID + "\" class=\"flot-chart card-content\"></div>";
            WidgetHTMLContent.Append(flotChartDiv);
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generatePercentageCardHTMLContent(dynamic widget, dynamic widgetDataDefJson, string divID)
        {
            string scriptCode = "<div style=\"padding: 20px;\" class =\"card-content\">";
            scriptCode = scriptCode + "<div class=\"widget-chart-1\"><div class=\"widget-chart-box-1\">";
            scriptCode = scriptCode + "<input id=\"" + divID + "_input\" data-plugin=\"knob\" data-width=\"80\" data-height=\"80\" value=\"0\" data-fgColor=\"#" + widgetDataDefJson.Context.FGColorCode + "\" data-bgColor=\"#" + widgetDataDefJson.Context.BGColorCode + "\" data-skin=\"tron\" data-angleOffset=\"180\" data-readOnly=true data-thickness=\".15\" /> ";
            scriptCode = scriptCode + "</div>";
            scriptCode = scriptCode + "<div class=\"widget-detail-1\"><h2 id=\"" + divID + "_h2\" class=\"p-t-10 m-b-0\"> 0/0 </h2><p class=\"text-muted\">Current/Total</p>";
            scriptCode = scriptCode + "</div></div></div>";
            scriptCode = scriptCode + "<script type=\"text/javascript\">";
            WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            scriptCode = scriptCode + "function update_" + divID + "(message)";
            scriptCode = scriptCode + "{";
            scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
            scriptCode = scriptCode + "if (message." + getMessageElementNameById((string)widgetDataDefJson.Context.Field) + " == null) { return; }";
            if (((string)widgetDataDefJson.Context.TotalNumberFrom).ToLower() == "inputvalue")
                scriptCode = scriptCode + "var total = " + (string)widgetDataDefJson.Context.TotalNumber + ";";
            else
            {
                scriptCode = scriptCode + "if (message." + getMessageElementNameById((string)widgetDataDefJson.Context.TotalNumber) + " == null) { return; }";
                scriptCode = scriptCode + "var total = message." + getMessageElementNameById((string)widgetDataDefJson.Context.TotalNumber) + ";";
            }
            scriptCode = scriptCode + "var current = message." + getMessageElementNameById((string)widgetDataDefJson.Context.Field) + ";";
            scriptCode = scriptCode + "var percentage = current / total * 100;";
            scriptCode = scriptCode + "$(\"#" + divID + "_input\").val(percentage.toFixed(2));";
            scriptCode = scriptCode + "$(\"#" + divID + "_input\").trigger('change');";
            scriptCode = scriptCode + "document.getElementById('" + divID + "_h2').innerHTML = current + \"/\" + total;";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "</script>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generatePieChartHTMLContent(dynamic widget, dynamic widgetDataDefJson, string divID)
        {
            string scriptCode = "<script type=\"text/javascript\">";
            scriptCode = scriptCode + "var options_" + divID + " = {series: {pie: {show: true, innerRadius: 0.5, label: {show: true}}}};";
            WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            scriptCode = scriptCode + "function update_" + divID + "(message)";
            scriptCode = scriptCode + "{";
            scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
            scriptCode = scriptCode + "var dataSet_" + divID + " = [";
            string pieItems = "";
            foreach (var item in widgetDataDefJson.Context.Values)
            {
                pieItems = pieItems + "{ label: \"" + item.Display + "\", data: message." + getMessageElementNameById((string)item.Field) + ", color: \"#" + item.ColorCode + "\" },";
            }
            pieItems = pieItems.Substring(0, pieItems.Length - 1);
            scriptCode = scriptCode + pieItems;
            scriptCode = scriptCode + "];";
            scriptCode = scriptCode + "$.plot($(\"#" + divID + "\"), dataSet_" + divID + ", options_" + divID + ");";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "</script>";
            string flotChartDiv = "<div id=\"" + divID + "\" class=\"flot-chart card-content \"></div>";
            WidgetHTMLContent.Append(flotChartDiv);
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateLineChartHTMLContent(dynamic widgetDataDefJson, string divID)
        {
            string dataVariableScript = "", datasetVariableMemeber = "", seriesSetting = "", xaxisSetting = "", yaxesSetting = "", legendSetting = "", gridSetting = "", optionVariable = "";
            string x_DateType = ((string)widgetDataDefJson.Context.Axis_X_DataType).ToLower();
            int i = 0;
            foreach (var item in widgetDataDefJson.Context.Axis_Y_Values)   //多個 Y 軸資料
            {
                i++;
                //建立每個 Y 軸資料陣列變數
                dataVariableScript = dataVariableScript + "var data_" + divID + "_" + (string)item.Axis_Y + " = [];";
                //設定每個 Y 軸資料來源, 以及顯示外觀. 來源 : 上一行 data, 外觀 : Line Chart
                string Scale_Y = item.Scale_Y != null ? item.Scale_Y : "1";
                datasetVariableMemeber = datasetVariableMemeber + "{label: \"" + item.Display + "\", data: data_" + divID + "_" + (string)item.Axis_Y + ", yaxis:" + Scale_Y + ", color: \"#" + item.ColorCode + "\", points: { symbol: \"circle\", fillColor: \"#" + item.ColorCode + "\", show: true }},";
            }
            datasetVariableMemeber = datasetVariableMemeber.Substring(0, datasetVariableMemeber.Length - 1); // 去掉多餘的 ','

            yaxesSetting = "yaxes: [" + "{position: \"left\", axisLabelUseCanvas: true, axisLabelFontSizePixels: 12, axisLabelFontFamily: 'Verdana, Arial', axisLabelPadding: 3}," + "{position: \"right\", axisLabelUseCanvas: true, axisLabelFontSizePixels: 12, axisLabelFontFamily: 'Verdana, Arial', axisLabelPadding: 3}" + "]";

            //設定 X 軸顯示外觀.                        
            if (x_DateType == "datetime")
                xaxisSetting = "xaxis: {mode: \"time\", timeformat: \"%M:%S\",tickSize: [10, \"second\"], ";// X 軸為時間資料時, 設定 X 軸屬性                            
            else
                xaxisSetting = "xaxis: {";
            xaxisSetting = xaxisSetting + "axisLabel: \"" + widgetDataDefJson.Context.Axis_X_Display + "\", axisLabelUseCanvas: true, axisLabelFontSizePixels: 12, axisLabelFontFamily: 'Verdana, Arial', axisLabelPadding: 10}";  //設定 X 軸顯示屬性

            //設定 Series, Legend 以及 Grid
            seriesSetting = "series: {lines: {show:true}}";
            legendSetting = "legend: {noColumns: 0, labelBoxBorderColor: \"#858585\", position: \"se\"}";
            gridSetting = "grid: {hoverable: true, borderWidth: 2, backgroundColor: { colors: [\"#ffffff\", \"#EDF5FF\"] }}";

            //組合 Option Variable
            optionVariable = "var options_" + divID + " = {" + seriesSetting + "," + xaxisSetting + "," + yaxesSetting + "," + legendSetting + "," + gridSetting + "};";

            //組合 Script
            string scriptCode = "<script type=\"text/javascript\">";
            scriptCode = scriptCode + dataVariableScript;
            scriptCode = scriptCode + optionVariable;
            scriptCode = scriptCode + "var totalPoints = 20;";         //設定此 Widget 最多顯示的 X 軸資料數量
            WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱

            //以下為 JavaScript Function
            scriptCode = scriptCode + "function update_" + divID + "(message)";     // 函數開始, 必須使用 'message' 當變數名稱
            scriptCode = scriptCode + "{";
            scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
            foreach (var item in widgetDataDefJson.Context.Axis_Y_Values)
            {
                scriptCode = scriptCode + "if (data_" + divID + "_" + (string)item.Axis_Y + ".length == totalPoints) {";
                scriptCode = scriptCode + "data_" + divID + "_" + (string)item.Axis_Y + ".shift(); }";        //如果目前 X 軸筆數已滿, 刪除第一筆
            }
            foreach (var item in widgetDataDefJson.Context.Axis_Y_Values)
            {
                scriptCode = scriptCode + "if (message." + getMessageElementNameById((string)item.Axis_Y) + " != null) {";
                if (x_DateType == "datetime")
                {       // gd 是 JavaScript 函數, 轉換 DateTime Sting 成 TimeStamp                    
                    scriptCode = scriptCode + "var data = [gd(message." + getMessageElementNameById((string)widgetDataDefJson.Context.Axis_X) + ")" + ", message." + getMessageElementNameById((string)item.Axis_Y) + "];";
                }
                else
                    scriptCode = scriptCode + "var data = [message." + getMessageElementNameById((string)widgetDataDefJson.Context.Axis_X) + ", message." + getMessageElementNameById((string)item.Axis_Y) + "];";
                scriptCode = scriptCode + "data_" + divID + "_" + (string)item.Axis_Y + ".push(data); }";     //Push 目前資料到各個陣列裡
            }
            scriptCode = scriptCode + "var dataSet_" + divID + " = [" + datasetVariableMemeber + "];";
            scriptCode = scriptCode + "$.plot($(\"#" + divID + "\"), dataSet_" + divID + ", options_" + divID + ");";   // 呼叫 plot 更新 UI
            scriptCode = scriptCode + "}";
            // JavaScript Function End
            scriptCode = scriptCode + "</script>";
            string flotChartDiv = "<div id=\"" + divID + "\" class=\"flot-chart card-content\"></div>";
            WidgetHTMLContent.Append(flotChartDiv);
            WidgetHTMLContent.Append(scriptCode);
        }
        private void generateSimpleOEECardHTMLContent(dynamic widgetDataDefJson, string divID)
        {
            string scriptCode = "<div class=\"card-content\"><div style=\"padding: 20px;\">";
            scriptCode = scriptCode + "<div class=\"widget-chart-1\"><div class=\"widget-chart-box-1\">";
            scriptCode = scriptCode + "<input id=\"" + divID + "_input\" data-plugin=\"knob\" data-width=\"100\" data-height=\"100\" value=\"0\" data-fgColor=\"#000000\" data-bgColor=\"#FFFFFF\" data-skin=\"tron\" data-angleOffset=\"180\" data-readOnly=true data-thickness=\".25\" /> ";
            scriptCode = scriptCode + "</div>";
            scriptCode = scriptCode + "<div class=\"widget-detail-1\"><h2 id=\"" + divID + "_h2\" class=\"p-t-10 m-b-0\"></h2>";
            scriptCode = scriptCode + "<p class=\"text-muted\"></p><p class=\"text-muted\"><div id=\"" + divID + "_simpleOEEFomula\"></div><div>(Good Units * Ideal Cycle Time) / Planned Production Time</div></p></div></div></div></div>";
            scriptCode = scriptCode + "<script type=\"text/javascript\">";
            scriptCode = scriptCode + "var simpleOEEGoodUnits_" + divID + " = 0, simpleOEEIdealCycleTime_" + divID + " = 0, simpleOEEPlannedProductionTime_" + divID + " = 0;";
            WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            scriptCode = scriptCode + "function update_" + divID + "(message)";
            scriptCode = scriptCode + "{";
            scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
            scriptCode = scriptCode + "if (typeof message." + getMessageElementNameById((string)widgetDataDefJson.Context.GoodCount) + " != 'undefined') simpleOEEGoodUnits_" + divID + " = message." + getMessageElementNameById((string)widgetDataDefJson.Context.GoodCount) + ";";
            scriptCode = scriptCode + "if (typeof message." + getMessageElementNameById((string)widgetDataDefJson.Context.IdealCycleTime) + " != 'undefined') simpleOEEIdealCycleTime_" + divID + " = message." + getMessageElementNameById((string)widgetDataDefJson.Context.IdealCycleTime) + ";";
            scriptCode = scriptCode + "if (typeof message." + getMessageElementNameById((string)widgetDataDefJson.Context.PlannedProductionTime) + " != 'undefined') simpleOEEPlannedProductionTime_" + divID + " = message." + getMessageElementNameById((string)widgetDataDefJson.Context.PlannedProductionTime) + ";";
            scriptCode = scriptCode + "if (simpleOEEPlannedProductionTime_" + divID + " > 0) {";
            scriptCode = scriptCode + "var percentage = (simpleOEEGoodUnits_" + divID + "*simpleOEEIdealCycleTime_" + divID + ")/simpleOEEPlannedProductionTime_" + divID + "*100;";
            scriptCode = scriptCode + "$(\"#" + divID + "_input\").val(percentage.toFixed(2));";
            scriptCode = scriptCode + "var dataFGColor = JSON.parse('{\"fgColor\":\"#04B404\"}');";
            scriptCode = scriptCode + "if (percentage < " + widgetDataDefJson.Context.ThreadsLow + ") dataFGColor = JSON.parse('{\"fgColor\":\"#DF0101\"}');";
            scriptCode = scriptCode + "else if (percentage >= " + widgetDataDefJson.Context.ThreadsLow + " && percentage < " + widgetDataDefJson.Context.ThreadsHigh + " ) dataFGColor = JSON.parse('{\"fgColor\":\"#FE9A2E\"}');";

            scriptCode = scriptCode + "$(\"#" + divID + "_input\").trigger('change');";
            scriptCode = scriptCode + "$(\"#" + divID + "_input\").trigger('configure', dataFGColor);";
            scriptCode = scriptCode + "document.getElementById('" + divID + "_simpleOEEFomula').innerHTML = '( ' + simpleOEEGoodUnits_" + divID + " + ' * ' + simpleOEEIdealCycleTime_" + divID + " + ' ) / ' + simpleOEEPlannedProductionTime_" + divID + " ;";
            scriptCode = scriptCode + "document.getElementById('" + divID + "_h2').innerHTML = ' OEE: ' + percentage.toFixed(2) + '%';";
            scriptCode = scriptCode + "}}";
            scriptCode = scriptCode + "</script>";
            WidgetHTMLContent.Append(scriptCode);
        }
        private void generatePreferredOEECardHTMLContent(dynamic widgetDataDefJson, string divID)
        {
            string scriptCode = "<div class=\"card-content\"><div style=\"padding: 20px;\">";
            scriptCode = scriptCode + "<table border=\"1\" width=\"100%\"><tr><td><table border=\"0\" width=\"100%\">";
            scriptCode = scriptCode + "<tr><td colspan=\"2\" rowspan=\"2\" align=\"center\" valign=\"middle\">";
            scriptCode = scriptCode + "<input id=\"" + divID + "_OEE_input\" data-plugin=\"knob\" data-width=\"100\" data-height=\"100\" value=\"0\" data-fgColor=\"#000000\" data-bgColor=\"#FFFFFF\" data-skin=\"tron\" data-angleOffset=\"180\" data-readOnly=true data-thickness=\".25\" /></td>";
            scriptCode = scriptCode + "<td colspan=\"2\" align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\"><h4>Availability</h4></td>";
            scriptCode = scriptCode + "<td colspan=\"2\" align=\"center\" valign=\"middle\"><h4>Performance</h4></td>";
            scriptCode = scriptCode + "<td colspan=\"2\" align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\"><h4>Quality</h4></td></tr>";
            scriptCode = scriptCode + "<tr><td colspan=\"2\" align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\">";
            scriptCode = scriptCode + "<input id=\"" + divID + "_Availability_input\" data-plugin=\"knob\" data-width=\"70\" data-height=\"70\" value=\"0\" data-fgColor=\"#000000\" data-bgColor=\"#FFFFFF\" data-skin=\"tron\" data-angleOffset=\"180\" data-readOnly=true data-thickness=\".20\" /></td>";
            scriptCode = scriptCode + "<td colspan=\"2\" align=\"center\" valign=\"middle\">";
            scriptCode = scriptCode + "<input id=\"" + divID + "_Performance_input\" data-plugin=\"knob\" data-width=\"70\" data-height=\"70\" value=\"0\" data-fgColor=\"#000000\" data-bgColor=\"#FFFFFF\" data-skin=\"tron\" data-angleOffset=\"180\" data-readOnly=true data-thickness=\".20\" /></td>";
            scriptCode = scriptCode + "<td colspan=\"2\" align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\">";
            scriptCode = scriptCode + "<input id=\"" + divID + "_Quality_input\" data-plugin=\"knob\" data-width=\"70\" data-height=\"70\" value=\"0\" data-fgColor=\"#000000\" data-bgColor=\"#FFFFFF\" data-skin=\"tron\" data-angleOffset=\"180\" data-readOnly=true data-thickness=\".20\" /></td></tr>";
            scriptCode = scriptCode + "<tr><td colspan=\"2\" rowspan=\"2\" align=\"center\" valign=\"middle\"><h3 id=\"" + divID + "_OEE\"></h3></td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\">Run Time</td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\"><div id=\"" + divID + "_RunTime\"></div></td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\">Produced Units</td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\"><div id=\"" + divID + "_ProducedUnits\"></div></td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\">Good Units</td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\"><div id=\"" + divID + "_GoodUnits\"></div></td></tr>";
            scriptCode = scriptCode + "<tr><td align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\">Planned Time</td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\"><div id=\"" + divID + "_PlannedTime\"></div></td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\">Expected Units</td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\"><div id=\"" + divID + "_ExpectedUnits\"></div></td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\">Total Units</td>";
            scriptCode = scriptCode + "<td align=\"center\" valign=\"middle\" bgcolor=\"#F2F2F2\"><div id=\"" + divID + "_TotalUnits\"></div></td></tr></table></td></tr></table></div></div>";
            scriptCode = scriptCode + "<script type=\"text/javascript\">";
            scriptCode = scriptCode + "var preferredOEERunTime_" + divID + "=0, preferredOEEPlannedTime_" + divID + "=0, preferredOEEProducedUnits_" + divID + "=0, preferredOEEExpectedUnits_" + divID + "=0, preferredOEEGoodUnits_" + divID + "=0, preferredOEERejectedUnits_" + divID + "=0, preferredOEETotalUnits_" + divID + "=0;";
            WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
            scriptCode = scriptCode + "function update_" + divID + "(message) {";
            scriptCode = scriptCode + "if (typeof message.MessageCatalogId == 'undefined') { return; }";
            scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
            scriptCode = scriptCode + "if (typeof message." + getMessageElementNameById((string)widgetDataDefJson.Context.RunTime) + " != 'undefined') preferredOEERunTime_" + divID + " = message." + getMessageElementNameById((string)widgetDataDefJson.Context.RunTime) + ";";
            scriptCode = scriptCode + "if (typeof message." + getMessageElementNameById((string)widgetDataDefJson.Context.PlannedProductionTime) + " != 'undefined') preferredOEEPlannedTime_" + divID + " = message." + getMessageElementNameById((string)widgetDataDefJson.Context.PlannedProductionTime) + ";";
            scriptCode = scriptCode + "if (typeof message." + getMessageElementNameById((string)widgetDataDefJson.Context.ProducedUnits) + " != 'undefined') preferredOEEProducedUnits_" + divID + " = message." + getMessageElementNameById((string)widgetDataDefJson.Context.ProducedUnits) + ";";
            scriptCode = scriptCode + "if (typeof message." + getMessageElementNameById((string)widgetDataDefJson.Context.ExpectedUnits) + " != 'undefined') preferredOEEExpectedUnits_" + divID + " = message." + getMessageElementNameById((string)widgetDataDefJson.Context.ExpectedUnits) + ";";
            scriptCode = scriptCode + "if (typeof message." + getMessageElementNameById((string)widgetDataDefJson.Context.GoodUnits) + " != 'undefined') preferredOEEGoodUnits_" + divID + " = message." + getMessageElementNameById((string)widgetDataDefJson.Context.GoodUnits) + ";";
            scriptCode = scriptCode + "if (typeof message." + getMessageElementNameById((string)widgetDataDefJson.Context.RejectedUnits) + " != 'undefined') preferredOEERejectedUnits_" + divID + " = message." + getMessageElementNameById((string)widgetDataDefJson.Context.RejectedUnits) + ";";
            scriptCode = scriptCode + "preferredOEETotalUnits_" + divID + " = preferredOEEGoodUnits_" + divID + "+preferredOEERejectedUnits_" + divID + ";";
            scriptCode = scriptCode + "var percentageA = 0; if (preferredOEEPlannedTime_" + divID + " != 0) percentageA=preferredOEERunTime_" + divID + "*100/preferredOEEPlannedTime_" + divID + ";";
            scriptCode = scriptCode + "var percentageP = 0; if (preferredOEEExpectedUnits_" + divID + " != 0) percentageP=preferredOEEProducedUnits_" + divID + "*100/preferredOEEExpectedUnits_" + divID + ";";
            scriptCode = scriptCode + "var percentageQ = 0; if (preferredOEETotalUnits_" + divID + " != 0) percentageQ=preferredOEEGoodUnits_" + divID + "*100/preferredOEETotalUnits_" + divID + ";";

            scriptCode = scriptCode + "var percentageOEE = percentageA*percentageP*percentageQ/10000;";
            scriptCode = scriptCode + "$(\"#" + divID + "_OEE_input\").val(percentageOEE.toFixed(2));";
            scriptCode = scriptCode + "var OEEDataFGColor = JSON.parse('{\"fgColor\":\"#04B404\"}');";
            scriptCode = scriptCode + "if (percentageOEE < " + widgetDataDefJson.Context.PreferredOEEThreadsLow + ") OEEDataFGColor = JSON.parse('{\"fgColor\":\"#DF0101\"}');";
            scriptCode = scriptCode + "else if (percentageOEE >= " + widgetDataDefJson.Context.PreferredOEEThreadsLow + " && percentageOEE < " + widgetDataDefJson.Context.PreferredOEEThreadsHigh + " ) OEEDataFGColor = JSON.parse('{\"fgColor\":\"#FE9A2E\"}');";
            scriptCode = scriptCode + "$(\"#" + divID + "_OEE_input\").trigger('change');";
            scriptCode = scriptCode + "$(\"#" + divID + "_OEE_input\").trigger('configure', OEEDataFGColor);";

            scriptCode = scriptCode + "$(\"#" + divID + "_Availability_input\").val(percentageA.toFixed(2));";
            scriptCode = scriptCode + "var ADataFGColor = JSON.parse('{\"fgColor\":\"#04B404\"}');";
            scriptCode = scriptCode + "if (percentageA < " + widgetDataDefJson.Context.AvailabilityThreadsLow + ") ADataFGColor = JSON.parse('{\"fgColor\":\"#DF0101\"}');";
            scriptCode = scriptCode + "else if (percentageA >= " + widgetDataDefJson.Context.AvailabilityThreadsLow + " && percentageA < " + widgetDataDefJson.Context.AvailabilityThreadsHigh + " ) ADataFGColor = JSON.parse('{\"fgColor\":\"#FE9A2E\"}');";
            scriptCode = scriptCode + "$(\"#" + divID + "_Availability_input\").trigger('change');";
            scriptCode = scriptCode + "$(\"#" + divID + "_Availability_input\").trigger('configure', ADataFGColor);";

            scriptCode = scriptCode + "$(\"#" + divID + "_Performance_input\").val(percentageP.toFixed(2));";
            scriptCode = scriptCode + "var PDataFGColor = JSON.parse('{\"fgColor\":\"#04B404\"}');";
            scriptCode = scriptCode + "if (percentageP < " + widgetDataDefJson.Context.PerformanceThreadsLow + ") PDataFGColor = JSON.parse('{\"fgColor\":\"#DF0101\"}');";
            scriptCode = scriptCode + "else if (percentageP >= " + widgetDataDefJson.Context.PerformanceThreadsLow + " && percentageP < " + widgetDataDefJson.Context.PerformanceThreadsHigh + " ) PDataFGColor = JSON.parse('{\"fgColor\":\"#FE9A2E\"}');";
            scriptCode = scriptCode + "$(\"#" + divID + "_Performance_input\").trigger('change');";
            scriptCode = scriptCode + "$(\"#" + divID + "_Performance_input\").trigger('configure', PDataFGColor);";

            scriptCode = scriptCode + "$(\"#" + divID + "_Quality_input\").val(percentageQ.toFixed(2));";
            scriptCode = scriptCode + "var QDataFGColor = JSON.parse('{\"fgColor\":\"#04B404\"}');";
            scriptCode = scriptCode + "if (percentageQ < " + widgetDataDefJson.Context.QualityThreadsLow + ") QDataFGColor = JSON.parse('{\"fgColor\":\"#DF0101\"}');";
            scriptCode = scriptCode + "else if (percentageQ >= " + widgetDataDefJson.Context.QualityThreadsLow + " && percentageQ < " + widgetDataDefJson.Context.QualityThreadsHigh + " ) QDataFGColor = JSON.parse('{\"fgColor\":\"#FE9A2E\"}');";
            scriptCode = scriptCode + "else $(\"#" + divID + "_Quality_input\").attr('data-fgColor','#04B404');";
            scriptCode = scriptCode + "$(\"#" + divID + "_Quality_input\").trigger('change');";
            scriptCode = scriptCode + "$(\"#" + divID + "_Quality_input\").trigger('configure', QDataFGColor);";

            scriptCode = scriptCode + "document.getElementById('" + divID + "_OEE').innerHTML = 'OEE: ' + percentageOEE.toFixed(2) + '%';";
            scriptCode = scriptCode + "document.getElementById('" + divID + "_RunTime').innerHTML = preferredOEERunTime_" + divID + ";";
            scriptCode = scriptCode + "document.getElementById('" + divID + "_PlannedTime').innerHTML = preferredOEEPlannedTime_" + divID + ";";
            scriptCode = scriptCode + "document.getElementById('" + divID + "_ProducedUnits').innerHTML = preferredOEEProducedUnits_" + divID + ";";
            scriptCode = scriptCode + "document.getElementById('" + divID + "_ExpectedUnits').innerHTML = preferredOEEExpectedUnits_" + divID + ";";
            scriptCode = scriptCode + "document.getElementById('" + divID + "_GoodUnits').innerHTML = preferredOEEGoodUnits_" + divID + ";";
            scriptCode = scriptCode + "document.getElementById('" + divID + "_TotalUnits').innerHTML = preferredOEETotalUnits_" + divID + ";";
            scriptCode = scriptCode + "}";
            scriptCode = scriptCode + "</script>";
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateHTMLContentCardHTMLContent(dynamic widgetDataDefJson, string divID)
        {
            string scriptCode = "<div id=\"" + divID + "\" class =\"card-content\">";
            if (widgetDataDefJson.Context.HTMLContentSourceFrom == "inputValue")
            {
                scriptCode = scriptCode + widgetDataDefJson.Context.HTMLContent;
            }
            scriptCode = scriptCode + "</div>";
            if (widgetDataDefJson.Context.HTMLContentSourceFrom == "messageField")
            {
                scriptCode = scriptCode + "<script type=\"text/javascript\">";
                WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
                scriptCode = scriptCode + "function update_" + divID + "(message)";
                scriptCode = scriptCode + "{";
                scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
                scriptCode = scriptCode + "if (message." + getMessageElementNameById((string)widgetDataDefJson.Context.HTMLContent) + " == null) { return; }";
                scriptCode = scriptCode + "document.getElementById('" + divID + "').innerHTML = message." + getMessageElementNameById((string)widgetDataDefJson.Context.HTMLContent) + ";";
                scriptCode = scriptCode + "}";
                scriptCode = scriptCode + "</script>";
            }
            WidgetHTMLContent.Append(scriptCode);
        }

        private void generateAllEquipmentCircleMapHTMLContent(dynamic widgetDataDefJson, string divID, string EquipmentString)
        {
            generateEquipmentCircleMap(widgetDataDefJson, divID, EquipmentString, "factory");
        }

        private void generateSingleEquipmentCircleMapHTMLContent(dynamic widgetDataDefJson, string divID, string equipmentString)
        {
            equipmentString = "[" + equipmentString + "]";
            generateEquipmentCircleMap(widgetDataDefJson, divID, equipmentString, "equipment");
        }

        // Share Code across company, factory and equipment
        private void generateMapCardHTMLContent(dynamic widget, string entityString, dynamic widgetDataDefJson, string entityType)
        {
            string entitiesString = entityString;
            var token = JToken.Parse(entityString);
            if (token is JObject)
                entitiesString = "[" + entityString + "]";

            string initZoom = widgetDataDefJson.Context.MapZoom == null ? "8" : widgetDataDefJson.Context.MapZoom;
            if (widgetDataDefJson.Context.MapProvider == "Baidu")
            {
                string scriptCode = "<div id='allmap' class =\"card-content\"></div>";
                scriptCode = scriptCode + "<script> var entityJSONObjs = $.parseJSON(jsonStringFilter('" + entitiesString + "'));";
                scriptCode = scriptCode + "var entityPoints=[], entityMarkers=[];";
                scriptCode = scriptCode + "var allmap;";
                scriptCode = scriptCode + "var cdsLogoIcon;";
                scriptCode = scriptCode + "var initZoom = " + initZoom + ";";
                scriptCode = scriptCode + "function loadBaiduMapJScript() {";
                scriptCode = scriptCode + "var script = document.createElement('script');";
                scriptCode = scriptCode + "script.type = 'text/javascript';";
                scriptCode = scriptCode + "script.src = 'https://api.map.baidu.com/api?v=2.0&ak=" + Global._sfBaiduMapAPIKey + "&callback=initMap';";
                scriptCode = scriptCode + "document.body.appendChild(script); } ";
                scriptCode = scriptCode + "function initMap() {";
                scriptCode = scriptCode + "cdsLogoIcon = new BMap.Icon('/assets/images/widget/CDSLogo34.png', new BMap.Size(34, 34));";
                scriptCode = scriptCode + "allmap = new BMap.Map('allmap');";
                scriptCode = scriptCode + "var point = new BMap.Point(entityJSONObjs[0].Longitude, entityJSONObjs[0].Latitude);";
                scriptCode = scriptCode + "allmap.centerAndZoom(point, initZoom);";
                scriptCode = scriptCode + "allmap.enableScrollWheelZoom();";
                scriptCode = scriptCode + "allmap.addControl(new BMap.NavigationControl({ anchor: BMAP_ANCHOR_TOP_RIGHT }));";
                scriptCode = scriptCode + "allmap.addControl(new BMap.MapTypeControl({ anchor: BMAP_ANCHOR_TOP_LEFT }));";
                scriptCode = scriptCode + "for (var i in entityJSONObjs) {";
                scriptCode = scriptCode + "entityPoints[entityJSONObjs[i].Id] = new BMap.Point(entityJSONObjs[i].Longitude, entityJSONObjs[i].Latitude);";
                scriptCode = scriptCode + "entityMarkers[entityJSONObjs[i].Id] = new BMap.Marker(entityPoints[entityJSONObjs[i].Id], { icon: cdsLogoIcon });";
                if (entityType.ToLower() == "company")
                {
                    scriptCode = scriptCode + "entityMarkers[entityJSONObjs[i].Id].setTitle(" + "'Navigate to '+" + "entityJSONObjs[i].Name" + "+' board.');";
                    scriptCode = scriptCode + "(function (marker, Id) {";
                    scriptCode = scriptCode + "marker.addEventListener('click', function () {";
                    scriptCode = scriptCode + "NavToFactoryDashboard(Id); });";
                    scriptCode = scriptCode + "})(entityMarkers[entityJSONObjs[i].Id], entityJSONObjs[i].Id);";
                }
                else
                {
                    scriptCode = scriptCode + "entityMarkers[entityJSONObjs[i].Id].setTitle(" + "''+" + "entityJSONObjs[i].Name" + "+'');";
                }                
                scriptCode = scriptCode + "allmap.addOverlay(entityMarkers[entityJSONObjs[i].Id]);";               
                scriptCode = scriptCode + "}}";
                scriptCode = scriptCode + "window.onload = loadBaiduMapJScript;";
                scriptCode = scriptCode + "</script>";
                WidgetHTMLContent.Append(scriptCode);
            }
            else
            {
                string scriptCode = "<div id='map' class =\"card-content\"></div>";
                scriptCode = scriptCode + "<script> var entityJSONObjs = $.parseJSON(jsonStringFilter('" + entitiesString + "'));";
                scriptCode = scriptCode + "var entityPoints=[], entityMarkers=[];";
                scriptCode = scriptCode + "var map, cdsLogoIcon, infoBox;";
                scriptCode = scriptCode + "var initZoom = " + initZoom + ";";
                scriptCode = scriptCode + "function initmap() {";
                scriptCode = scriptCode + "cdsLogoIcon = new google.maps.MarkerImage('/assets/images/widget/CDSLogo34.png', new google.maps.Size(34, 34), new google.maps.Point(0, 0), new google.maps.Point(17, 17));";
                scriptCode = scriptCode + "map = new google.maps.Map(document.getElementById('map'), { center: new google.maps.LatLng(parseFloat(entityJSONObjs[0].Latitude), parseFloat(entityJSONObjs[0].Longitude)), zoom: initZoom});";
                scriptCode = scriptCode + "infoBox = new google.maps.InfoWindow({content: ''});";
                scriptCode = scriptCode + "for (var i in entityJSONObjs) {";
                scriptCode = scriptCode + "entityPoints[entityJSONObjs[i].Id] = new google.maps.LatLng(parseFloat(entityJSONObjs[i].Latitude), parseFloat(entityJSONObjs[i].Longitude));";
                scriptCode = scriptCode + "entityMarkers[entityJSONObjs[i].Id] = new google.maps.Marker({ position: entityPoints[entityJSONObjs[i].Id], map: map, icon: cdsLogoIcon, id: entityJSONObjs[i].Id });";
                scriptCode = scriptCode + "(function (marker, Id, Name) {";
                scriptCode = scriptCode + "google.maps.event.addListener(marker, 'mouseover', function (e) {";
                if (entityType.ToLower() == "company")
                {
                    scriptCode = scriptCode + "infoBox.setContent('Navigate to ' + Name + ' board.'); infoBox.open(map, marker); });";
                    scriptCode = scriptCode + "google.maps.event.addListener(marker, 'click', function (e) {";
                    scriptCode = scriptCode + "NavToFactoryDashboard(Id); });";
                }
                else
                {
                    scriptCode = scriptCode + "infoBox.setContent('' + Name + ''); infoBox.open(map, marker); });";
                }
                scriptCode = scriptCode + "google.maps.event.addListener(marker, 'mouseout', function (e) {";
                scriptCode = scriptCode + "infoBox.close(); });";
                scriptCode = scriptCode + "})(entityMarkers[entityJSONObjs[i].Id], entityJSONObjs[i].Id, entityJSONObjs[i].Name);";
                scriptCode = scriptCode + "}}";
                scriptCode = scriptCode + "</script>";
                scriptCode = scriptCode + "<script src='https://maps.googleapis.com/maps/api/js?key=" + Global._sfGoogleMapAPIKey + "&callback=initmap' async defer></script>";
                WidgetHTMLContent.Append(scriptCode);
            }
        }

        private static string calNewRaduisFunction = "function calNewRaduis(inRaduis, baseZoom, currentZoom) {var numRaduis = Number(inRaduis); var diffLevel = currentZoom - baseZoom; if (diffLevel == 0) return numRaduis; var newRaduis = numRaduis; if (diffLevel >= 1) { while (diffLevel > 0) {newRaduis = newRaduis / 2; diffLevel--;}} if (diffLevel <= -1) {while (diffLevel < 0) {newRaduis = newRaduis * 2; diffLevel++;} } return newRaduis; }";
        private void generateEquipmentCircleMap(dynamic widgetDataDefJson, string divID, string equipmentString, string entityType)
        {
            if (widgetDataDefJson.Context.MapProvider == "Baidu")
            {
                string scriptCode = "<div id='allmap' class =\"card-content\"></div>";
                scriptCode = scriptCode + "<script> var equipmentJSONObjs = $.parseJSON(jsonStringFilter('" + equipmentString + "'));";
                scriptCode = scriptCode + "var equipmentPoints=[], equipmentMarkers=[], equipmentCircles=[], radiusValues=[], radiusTexts=[];";
                scriptCode = scriptCode + "var allmap, equipDict = {};";
                scriptCode = scriptCode + "var runIcon, idleIcon, stopIcon, offlineIcon;";
                scriptCode = scriptCode + "var runColor = 'green', idleColor = 'orange', stopColor = 'red', offlineColor = 'gray';";
                scriptCode = scriptCode + "var baseZoom = " + widgetDataDefJson.Context.oneByoneZoom + ";";
                scriptCode = scriptCode + "var currentZoom = " + widgetDataDefJson.Context.MapZoom + ";";
                scriptCode = scriptCode + "var radiusTextPrefix = '" + widgetDataDefJson.Context.Prefix + "';";
                scriptCode = scriptCode + "var radiusTextSuffix = '" + widgetDataDefJson.Context.Suffix + "';";
                scriptCode = scriptCode + "function loadBaiduMapJScript() {";
                scriptCode = scriptCode + "var script = document.createElement('script');";
                scriptCode = scriptCode + "script.type = 'text/javascript';";
                scriptCode = scriptCode + "script.src = 'https://api.map.baidu.com/api?v=2.0&ak=" + Global._sfBaiduMapAPIKey + "&callback=initMap';";
                scriptCode = scriptCode + "document.body.appendChild(script); } ";
                scriptCode = scriptCode + "function initMap() {";
                scriptCode = scriptCode + "runIcon = new BMap.Icon('/assets/images/widget/run34.png', new BMap.Size(34, 34));";
                scriptCode = scriptCode + "idleIcon = new BMap.Icon('/assets/images/widget/idle34.png', new BMap.Size(34, 34));";
                scriptCode = scriptCode + "stopIcon = new BMap.Icon('/assets/images/widget/stop34.png', new BMap.Size(34, 34));";
                scriptCode = scriptCode + "offlineIcon = new BMap.Icon('/assets/images/widget/offline34.png', new BMap.Size(34, 34));";
                scriptCode = scriptCode + "allmap = new BMap.Map('allmap');";
                scriptCode = scriptCode + "var point = new BMap.Point(equipmentJSONObjs[0].Longitude, equipmentJSONObjs[0].Latitude);";
                scriptCode = scriptCode + "allmap.centerAndZoom(point, currentZoom);";
                scriptCode = scriptCode + "allmap.enableScrollWheelZoom();";
                scriptCode = scriptCode + "allmap.addControl(new BMap.NavigationControl({ anchor: BMAP_ANCHOR_TOP_RIGHT }));";
                scriptCode = scriptCode + "allmap.addControl(new BMap.MapTypeControl({ anchor: BMAP_ANCHOR_TOP_LEFT }));";
                scriptCode = scriptCode + "for (var i in equipmentJSONObjs) {";
                scriptCode = scriptCode + "equipDict[equipmentJSONObjs[i].EquipmentId] = equipmentJSONObjs[i];";
                scriptCode = scriptCode + "equipmentPoints[equipmentJSONObjs[i].Id] = new BMap.Point(equipmentJSONObjs[i].Longitude, equipmentJSONObjs[i].Latitude);";
                scriptCode = scriptCode + "equipmentMarkers[equipmentJSONObjs[i].Id] = new BMap.Marker(equipmentPoints[equipmentJSONObjs[i].Id], { icon: offlineIcon });";
                
                if (entityType.ToLower() == "company" || entityType.ToLower() == "factory")
                {
                    scriptCode = scriptCode + "equipmentMarkers[equipmentJSONObjs[i].Id].setTitle(" + "'Navigate to '+" + "equipmentJSONObjs[i].Name" + "+' board.');";
                    scriptCode = scriptCode + "(function (marker, Id) {";
                    scriptCode = scriptCode + "marker.addEventListener('click', function () {";
                    scriptCode = scriptCode + "NavToEquipmentDashboard(Id); });";
                    scriptCode = scriptCode + "})(equipmentMarkers[equipmentJSONObjs[i].Id], equipmentJSONObjs[i].Id);";
                }
                else
                {
                    scriptCode = scriptCode + "equipmentMarkers[equipmentJSONObjs[i].Id].setTitle(" + "''+" + "equipmentJSONObjs[i].Name" + "+'');";
                }
                scriptCode = scriptCode + "allmap.addOverlay(equipmentMarkers[equipmentJSONObjs[i].Id]);";
                scriptCode = scriptCode + "radiusValues[equipmentJSONObjs[i].Id] = 0;";
                scriptCode = scriptCode + "equipmentCircles[equipmentJSONObjs[i].Id] = new BMap.Circle(equipmentPoints[equipmentJSONObjs[i].Id], calNewRaduis(radiusValues[equipmentJSONObjs[i].Id], baseZoom, currentZoom), { strokeColor: 'white', strokeWeight: 3, strokeOpacity: 0, fillColor: offlineColor, fillOpacity: 0.4 });";
                scriptCode = scriptCode + "allmap.addOverlay(equipmentCircles[equipmentJSONObjs[i].Id]);";
                scriptCode = scriptCode + "radiusTexts[equipmentJSONObjs[i].Id] = new BMap.Label(radiusTextPrefix + radiusValues[equipmentJSONObjs[i].Id].toFixed(2) + radiusTextSuffix, { position: equipmentPoints[equipmentJSONObjs[i].Id], offset: new BMap.Size(0, 20) });";
                scriptCode = scriptCode + "radiusTexts[equipmentJSONObjs[i].Id].setStyle({ color: 'black', fontSize: '14px', backgroundColor: 'white', borderColor: 'white' });";
                scriptCode = scriptCode + "allmap.addOverlay(radiusTexts[equipmentJSONObjs[i].Id]);";
                scriptCode = scriptCode + "}";
                scriptCode = scriptCode + "allmap.addEventListener('zoomend', function () {";
                scriptCode = scriptCode + "currentZoom = this.getZoom();";
                scriptCode = scriptCode + "for (var i in equipmentJSONObjs) {";
                scriptCode = scriptCode + "equipmentCircles[equipmentJSONObjs[i].Id].setRadius(calNewRaduis(radiusValues[equipmentJSONObjs[i].Id], baseZoom, currentZoom));";
                scriptCode = scriptCode + "}});} ";
                scriptCode = scriptCode + calNewRaduisFunction;
                WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱
                scriptCode = scriptCode + "function update_" + divID + "(message) {";
                scriptCode = scriptCode + "var equipmentObj = null;";
                scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
                scriptCode = scriptCode + "if (message.equipmentId != null) equipmentObj = equipDict[message.equipmentId];";
                scriptCode = scriptCode + "if (equipmentObj != null) {";
                if (widgetDataDefJson.Context.LocationSourceFrom == "messageField")
                {
                    scriptCode = scriptCode + "var latField = message." + getMessageElementNameById((string)widgetDataDefJson.Context.Latitude) + ";";
                    scriptCode = scriptCode + "var lngField = message." + getMessageElementNameById((string)widgetDataDefJson.Context.Longitude) + ";";
                    scriptCode = scriptCode + "if (latField != null && lngField != null) {";
                    scriptCode = scriptCode + "equipmentPoints[equipmentObj.Id] = new BMap.Point(lngField, latField);";
                    scriptCode = scriptCode + "equipmentMarkers[equipmentObj.Id].setPosition(equipmentPoints[equipmentObj.Id]);";
                    scriptCode = scriptCode + "equipmentCircles[equipmentObj.Id].setCenter(equipmentPoints[equipmentObj.Id]);";
                    scriptCode = scriptCode + "radiusTexts[equipmentObj.Id].setPosition(equipmentPoints[equipmentObj.Id]);";
                    scriptCode = scriptCode + "radiusTexts[equipmentObj.Id].setContent(radiusTextPrefix+radiusValues[equipmentObj.Id].toFixed(2)+radiusTextSuffix);";
                    scriptCode = scriptCode + "allmap.centerAndZoom(equipmentPoints[equipmentObj.Id], currentZoom);";
                    scriptCode = scriptCode + "}";
                }
                scriptCode = scriptCode + "if (message." + getMessageElementNameById((string)widgetDataDefJson.Context.Radius) + " != null) {";
                scriptCode = scriptCode + "radiusValues[equipmentObj.Id] = message." + getMessageElementNameById((string)widgetDataDefJson.Context.Radius) + ";";
                scriptCode = scriptCode + "equipmentCircles[equipmentObj.Id].setRadius(calNewRaduis(radiusValues[equipmentObj.Id], baseZoom, currentZoom));";
                scriptCode = scriptCode + "radiusTexts[equipmentObj.Id].setContent(radiusTextPrefix+radiusValues[equipmentObj.Id].toFixed(2)+radiusTextSuffix); ";
                scriptCode = scriptCode + "} \n";
                scriptCode = scriptCode + "switch (message.equipmentRunStatus) {";
                scriptCode = scriptCode + "case 1: equipmentMarkers[equipmentObj.Id].setIcon(runIcon); equipmentCircles[equipmentObj.Id].setFillColor(runColor);";
                scriptCode = scriptCode + "break;";
                scriptCode = scriptCode + "case 0: equipmentMarkers[equipmentObj.Id].setIcon(idleIcon); equipmentCircles[equipmentObj.Id].setFillColor(idleColor);";
                scriptCode = scriptCode + "break;";
                scriptCode = scriptCode + "case -1: equipmentMarkers[equipmentObj.Id].setIcon(stopIcon); equipmentCircles[equipmentObj.Id].setFillColor(stopColor);";
                scriptCode = scriptCode + "break;";
                scriptCode = scriptCode + "}}}";
                scriptCode = scriptCode + "window.onload = loadBaiduMapJScript;";
                scriptCode = scriptCode + "</script>";
                WidgetHTMLContent.Append(scriptCode);
            }
            else
            {
                string scriptCode = "<div id='mapRD' class =\"card-content\"></div>";
                scriptCode = scriptCode + "<script> var equipmentJSONObjs = $.parseJSON(jsonStringFilter('" + equipmentString + "'));";
                scriptCode = scriptCode + "var equipmentPoints=[], equipmentMarkers=[], equipmentCircles=[], radiusValues=[], radiusTexts=[];";
                scriptCode = scriptCode + "var mapRD, equipDict = {}, infoBox;";
                scriptCode = scriptCode + "var RunColor = '00FF00', IdleColor = 'FFBF00', StopColor = 'FF0000', OfflineColor = 'A4A4A4';";
                scriptCode = scriptCode + "var RunIcon, IdleIcon, StopIcon, OfflineIcon;";
                scriptCode = scriptCode + "var baseZoom = " + widgetDataDefJson.Context.oneByoneZoom + ";";
                scriptCode = scriptCode + "var currentZoom = " + widgetDataDefJson.Context.MapZoom + ";";
                scriptCode = scriptCode + "var radiusTextPrefix = '" + widgetDataDefJson.Context.Prefix + "';";
                scriptCode = scriptCode + "var radiusTextSuffix = '" + widgetDataDefJson.Context.Suffix + "';";
                scriptCode = scriptCode + "function initmapRD() {";
                scriptCode = scriptCode + "RunIcon = new google.maps.MarkerImage('/assets/images/widget/run34.png', new google.maps.Size(34, 34), new google.maps.Point(0, 0), new google.maps.Point(17, 17));";
                scriptCode = scriptCode + "IdleIcon = new google.maps.MarkerImage('/assets/images/widget/idle34.png', new google.maps.Size(34, 34), new google.maps.Point(0, 0), new google.maps.Point(17, 17));";
                scriptCode = scriptCode + "StopIcon = new google.maps.MarkerImage('/assets/images/widget/stop34.png', new google.maps.Size(34, 34), new google.maps.Point(0, 0), new google.maps.Point(17, 17));";
                scriptCode = scriptCode + "OfflineIcon = new google.maps.MarkerImage('/assets/images/widget/offline34.png', new google.maps.Size(34, 34), new google.maps.Point(0, 0), new google.maps.Point(17, 17));";
                scriptCode = scriptCode + "equipmentPointRD = new google.maps.LatLng(parseFloat(equipmentJSONObjs[0].Latitude), parseFloat(equipmentJSONObjs[0].Longitude));";
                scriptCode = scriptCode + "mapRD = new google.maps.Map(document.getElementById('mapRD'), { center: new google.maps.LatLng(parseFloat(equipmentJSONObjs[0].Latitude), parseFloat(equipmentJSONObjs[0].Longitude)), zoom: currentZoom});";
                scriptCode = scriptCode + "infoBox = new google.maps.InfoWindow({content: ''});";
                scriptCode = scriptCode + "TxtOverlay.prototype = new google.maps.OverlayView();";
                scriptCode = scriptCode + "TxtOverlay.prototype.onAdd = function () {var div = document.createElement('DIV');div.className = this.cls_;div.innerHTML = this.txt_;div.id = this.eId + '_text';this.div_ = div;var overlayProjection = this.getProjection();var position = overlayProjection.fromLatLngToDivPixel(this.pos);div.style.left = position.x + 'px';div.style.top = position.y + 'px';var panes = this.getPanes();panes.floatPane.appendChild(div);";
                scriptCode = scriptCode + "var me = this; this.listeners_ = [google.maps.event.addListener(this, 'position_changed', function () { me.draw(); })]; }\n";
                scriptCode = scriptCode + "TxtOverlay.prototype.draw = function () {var overlayProjection = this.getProjection();var position = overlayProjection.fromLatLngToDivPixel(this.pos);var div = this.div_;var p_y = position.y + 17;div.style.left = position.x  + 'px';div.style.top = p_y + 'px';} \n";
                scriptCode = scriptCode + "for (var i in equipmentJSONObjs) {";
                scriptCode = scriptCode + "equipDict[equipmentJSONObjs[i].EquipmentId] = equipmentJSONObjs[i];";
                scriptCode = scriptCode + "equipmentPoints[equipmentJSONObjs[i].Id] = new google.maps.LatLng(parseFloat(equipmentJSONObjs[i].Latitude), parseFloat(equipmentJSONObjs[i].Longitude));";
                scriptCode = scriptCode + "equipmentMarkers[equipmentJSONObjs[i].Id] = new google.maps.Marker({ position: equipmentPoints[equipmentJSONObjs[i].Id], map: mapRD, icon: OfflineIcon, id: equipmentJSONObjs[i].Id });";

                scriptCode = scriptCode + "(function (marker, Id, Name) {";
                scriptCode = scriptCode + "google.maps.event.addListener(marker, 'mouseover', function (e) {";
                if (entityType.ToLower() == "company" || entityType.ToLower() == "factory")
                {
                    scriptCode = scriptCode + "infoBox.setContent('Navigate to ' + Name + ' board.'); infoBox.open(mapRD, marker); });";
                    scriptCode = scriptCode + "google.maps.event.addListener(marker, 'click', function (e) {";
                    scriptCode = scriptCode + "NavToEquipmentDashboard(Id); });";
                }
                else
                {
                    scriptCode = scriptCode + "infoBox.setContent('' + Name + ''); infoBox.open(mapRD, marker); });";
                }
                scriptCode = scriptCode + "google.maps.event.addListener(marker, 'mouseout', function (e) {";
                scriptCode = scriptCode + "infoBox.close(); });";                
                scriptCode = scriptCode + "})(equipmentMarkers[equipmentJSONObjs[i].Id], equipmentJSONObjs[i].Id, equipmentJSONObjs[i].Name);";
                
                //if (entityType.ToLower() == "company" || entityType.ToLower() == "factory")
                //    scriptCode = scriptCode + "google.maps.event.addListener(equipmentMarkers[equipmentJSONObjs[i].Id], 'click', function () {NavToEquipmentDashboard(equipmentMarkers[this.id].id);});";

                scriptCode = scriptCode + "radiusValues[equipmentJSONObjs[i].Id] = 0;";
                scriptCode = scriptCode + "equipmentCircles[equipmentJSONObjs[i].Id] = new google.maps.Circle({ strokeColor: '#FFFFFF', strokeOpacity: 0.8, strokeWeight: 2, fillColor: '#' + OfflineColor, fillOpacity: 0.35, map: mapRD, center: equipmentPoints[equipmentJSONObjs[i].Id], radius: calNewRaduis(radiusValues[equipmentJSONObjs[i].Id], baseZoom, currentZoom) });";
                scriptCode = scriptCode + "radiusTexts[equipmentJSONObjs[i].Id] = new TxtOverlay(equipmentJSONObjs[i].Id, equipmentPoints[equipmentJSONObjs[i].Id], \"<div style='font-size:15px'>\" + radiusTextPrefix + radiusValues[equipmentJSONObjs[i].Id].toFixed(2) + radiusTextSuffix + \"</div>\", \"customBox\", mapRD);";
                scriptCode = scriptCode + "}";
                scriptCode = scriptCode + "mapRD.addListener('zoom_changed', function () {";
                scriptCode = scriptCode + "currentZoom = mapRD.getZoom();";
                scriptCode = scriptCode + "for (var i in equipmentJSONObjs) {";
                scriptCode = scriptCode + "equipmentCircles[equipmentJSONObjs[i].Id].setRadius(calNewRaduis(radiusValues[equipmentJSONObjs[i].Id], baseZoom, currentZoom));";
                scriptCode = scriptCode + "}";
                scriptCode = scriptCode + "});";
                scriptCode = scriptCode + "}";
                scriptCode = scriptCode + "function TxtOverlay(eId, pos, txt, cls, map) {this.eId = eId;this.pos = pos;this.txt_ = txt;this.cls_ = cls;this.map_ = map;this.div_ = null;this.setMap(map);} ";
                scriptCode = scriptCode + calNewRaduisFunction;
                WidgetJavaScriptFunction.Append("update_" + divID + "(DeviceMessage);");       // 開放給外面更新資料的函數, 必須使用 'DeviceMessage' 當變數名稱            
                scriptCode = scriptCode + "function update_" + divID + "(message) {";
                scriptCode = scriptCode + "var equipmentObj=null;";
                scriptCode = scriptCode + "if (message.MessageCatalogId !=" + widgetDataDefJson.MessageCatalogId + ") { return; }";
                scriptCode = scriptCode + "if (message.equipmentId != null) equipmentObj = equipDict[message.equipmentId];";
                scriptCode = scriptCode + "if (equipmentObj != null) {";
                if (widgetDataDefJson.Context.LocationSourceFrom == "messageField")
                {
                    scriptCode = scriptCode + "var latField = message." + getMessageElementNameById((string)widgetDataDefJson.Context.Latitude) + ";";
                    scriptCode = scriptCode + "var lngField = message." + getMessageElementNameById((string)widgetDataDefJson.Context.Longitude) + ";";
                    scriptCode = scriptCode + "if (latField != null && lngField != null) {";
                    scriptCode = scriptCode + "equipmentPoints[equipmentObj.Id] = new google.maps.LatLng(parseFloat(latField), parseFloat(lngField));";
                    scriptCode = scriptCode + "equipmentMarkers[equipmentObj.Id].setPosition(equipmentPoints[equipmentObj.Id]);";
                    scriptCode = scriptCode + "equipmentCircles[equipmentObj.Id].setCenter(equipmentPoints[equipmentObj.Id]);";
                    scriptCode = scriptCode + "radiusTexts[equipmentObj.Id].pos=equipmentPoints[equipmentObj.Id];";
                    scriptCode = scriptCode + "document.getElementById(equipmentObj.Id + '_text').innerHTML = \"<div style='font-size:15px'>\" + radiusTextPrefix + radiusValues[equipmentObj.Id].toFixed(2) + radiusTextSuffix + \"</div>\";";
                    scriptCode = scriptCode + "}";
                }

                scriptCode = scriptCode + "if (message." + getMessageElementNameById((string)widgetDataDefJson.Context.Radius) + " != null) {";
                scriptCode = scriptCode + "radiusValues[equipmentObj.Id] = message." + getMessageElementNameById((string)widgetDataDefJson.Context.Radius) + ";";
                scriptCode = scriptCode + "equipmentCircles[equipmentObj.Id].setRadius(calNewRaduis(radiusValues[equipmentObj.Id], baseZoom, currentZoom));";
                scriptCode = scriptCode + "document.getElementById(equipmentObj.Id + '_text').innerHTML = \"<div style='font-size:15px'>\" + radiusValues[equipmentObj.Id].toFixed(2) + \"</div>\"; ";
                scriptCode = scriptCode + "} \n";

                scriptCode = scriptCode + "switch (message.equipmentRunStatus) {";
                scriptCode = scriptCode + "case 1:";
                scriptCode = scriptCode + "equipmentMarkers[equipmentObj.Id].setIcon(RunIcon);";
                scriptCode = scriptCode + "equipmentCircles[equipmentObj.Id].setOptions({ fillColor: '#' + RunColor, strokeColor: '#' + RunColor });";
                scriptCode = scriptCode + "break; ";
                scriptCode = scriptCode + "case 0:";
                scriptCode = scriptCode + "equipmentMarkers[equipmentObj.Id].setIcon(IdleIcon);";
                scriptCode = scriptCode + "equipmentCircles[equipmentObj.Id].setOptions({ fillColor: '#' + IdleColor, strokeColor: '#' + IdleColor });";
                scriptCode = scriptCode + "break; ";
                scriptCode = scriptCode + "case -1:";
                scriptCode = scriptCode + "equipmentMarkers[equipmentObj.Id].setIcon(StopIcon);";
                scriptCode = scriptCode + "equipmentCircles[equipmentObj.Id].setOptions({ fillColor: '#' + StopColor, strokeColor: '#' + StopColor });";
                scriptCode = scriptCode + "break; }}}";
                scriptCode = scriptCode + "</script>";
                scriptCode = scriptCode + "<style>.customBox {background-color: white;border: 0px solid black;position: absolute;}</style>";
                scriptCode = scriptCode + "<script src='https://maps.googleapis.com/maps/api/js?key=" + Global._sfGoogleMapAPIKey + "&callback=initmapRD' async defer></script>";
                WidgetHTMLContent.Append(scriptCode);
            }            
        }
    }

    
}