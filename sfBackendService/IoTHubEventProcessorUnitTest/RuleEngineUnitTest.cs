using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IoTHubEventProcessor;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using IoTHubEventProcessor.Models;
using System.Threading.Tasks;
using sfShareLib;

namespace IoTHubEventProcessorUnitTest
{
    [TestClass]
    public class RuleEngineUnitTest
    {
        [TestMethod]
        public void TestComplieBoolRules()
        {
            Assert.IsFalse(testBoolRule(false, "AND", true));

            Assert.IsTrue(testBoolRule(false, "OR", true));

            Assert.IsTrue(testBoolRule(false, "xOR", true));

            Assert.IsFalse(testBoolRule(true, "xOR", true));
        }

        [TestMethod]
        public void TestSingleRuleBoolItems()
        {
            Assert.IsTrue(testSingleRuleBoolItem(true, "=", true));

            Assert.IsFalse(testSingleRuleBoolItem(true, "=", false));

            Assert.IsFalse(testSingleRuleBoolItem(false, "=", true));

            Assert.IsTrue(testSingleRuleBoolItem(false, "=", false));
        }

        [TestMethod]
        public void TestSingleRuleStringItems()
        {
            Assert.IsTrue(testAlarmRuleStringEqual("1qaz2wsx", "=", "1qaz2wsx", "1qaz2wsx"));

            Assert.IsFalse(testAlarmRuleStringEqual("A", "=", "1qaz2wsx", "hi hi"));

            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "null", null));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "", null));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "NULL", null));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", null, null));

            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "null", "null"));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "", "null"));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "NULL", "null"));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", null, "null"));

            Assert.IsFalse(testAlarmRuleStringEqual("Name", "=", "null", "NULL"));
            Assert.IsFalse(testAlarmRuleStringEqual("Name", "=", "", "NULL"));
            Assert.IsFalse(testAlarmRuleStringEqual("Name", "=", "NULL", "NULL"));
            Assert.IsFalse(testAlarmRuleStringEqual("Name", "=", null, "NULL"));

            Assert.IsFalse(testAlarmRuleStringEqual("Name", "=", "null", "Null"));
            Assert.IsFalse(testAlarmRuleStringEqual("Name", "=", "", "Null"));
            Assert.IsFalse(testAlarmRuleStringEqual("Name", "=", "NULL", "Null"));
            Assert.IsFalse(testAlarmRuleStringEqual("Name", "=", null, "Null"));

            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "null", ""));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "", ""));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "NULL", ""));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", null, ""));

            Assert.IsTrue(testAlarmRuleStringEqual("VMP-45", "=", "VMP-45", "VMP-45"));

            Assert.IsFalse(testAlarmRuleStringEqual("A-B-C", "=", "A-B-C", null));
            Assert.IsTrue(testAlarmRuleStringEqual("A-B-C", "=", "A-B-C", "A-B-C"));
            Assert.IsFalse(testAlarmRuleStringEqual("A-B-C", "=", "A-B-C", "A-B_C"));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "A-B-C", "A-B-C"));

            Assert.IsFalse(testAlarmRuleStringEqual("Name", "!=", "A-B-C", "A-B-C"));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "!=", "A-B-C", "A-B_C"));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "!=", "A-B-C", null));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "!=", "A-B-C", "null"));

            Assert.IsFalse(testAlarmRuleStringEqual("A-B-C", "!=", "A-B-C", "A-B-C"));
            Assert.IsTrue(testAlarmRuleStringEqual("A-B-C", "!=", "A-B-C", "A-B_C"));
            Assert.IsTrue(testAlarmRuleStringEqual("A-B-C", "!=", "A-B-C", null));
            Assert.IsTrue(testAlarmRuleStringEqual("A-B-C", "!=", "A-B-C", "null"));

            Assert.IsFalse(testAlarmRuleStringEqual("Name", "=", "A+B-C", null));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "A+B-C", "A+B-C"));
            Assert.IsFalse(testAlarmRuleStringEqual("Name", "=", "A*B/C", "null"));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "=", "A*B/C", "A*B/C"));

            Assert.IsTrue(testAlarmRuleStringEqual("Name", "!=", "A+B-C", null));
            Assert.IsFalse(testAlarmRuleStringEqual("Name", "!=", "A+B-C", "A+B-C"));
            Assert.IsTrue(testAlarmRuleStringEqual("Name", "!=", "A*B/C", "null"));
            Assert.IsFalse(testAlarmRuleStringEqual("Name", "!=", "A*B/C", "A*B/C"));
        }

        [TestMethod]
        public void TestSingleRuleItemNumeric()
        {
            Assert.IsTrue(testSingleRuleNumericItem(100, "=", 100));

            Assert.IsFalse(testSingleRuleNumericItem(101, "=", 100));

            Assert.IsTrue(testSingleRuleNumericItem(99, "<", 100));

            Assert.IsFalse(testSingleRuleNumericItem(100, "<", 100));

            Assert.IsFalse(testSingleRuleNumericItem(101, "<", 100));

            Assert.IsFalse(testSingleRuleNumericItem(101, "<=", 100));
            Assert.IsTrue(testSingleRuleNumericItem(101, ">=", 100));

            Assert.IsTrue(testSingleRuleNumericItem(99, "<=", 100));
            Assert.IsFalse(testSingleRuleNumericItem(99, ">=", 100));

            Assert.IsTrue(testSingleRuleNumericItem((decimal)0.125, "<=", (decimal)0.25));
            Assert.IsFalse(testSingleRuleNumericItem((decimal)0.125, ">=", (decimal)0.25));
            Assert.IsFalse(testSingleRuleNumericItem((decimal)0.125, "=", (decimal)0.25));
            Assert.IsTrue(testSingleRuleNumericItem((decimal)0.125, "=", (decimal)0.125));

        }

        [TestMethod]
        public void TestAlarmRules()
        {
            Assert.IsFalse(testAlarmRule(4, "Walker", false));
            Assert.IsTrue(testAlarmRule(4, "Walker", true));
            Assert.IsTrue(testAlarmRule(1, "Walker", false));
            Assert.IsTrue(testAlarmRule(3, "Walker", false));
            Assert.IsTrue(testAlarmRule(4, "Walke", false));

            Assert.IsTrue(testAlarmRule2(0, "PASS", true, 2));
            Assert.IsFalse(testAlarmRule2(0, "PASS", true, 3));
            Assert.IsFalse(testAlarmRule2(0, "NG", true, 2));
            Assert.IsTrue(testAlarmRule2(10, "NG", true, 2));
            Assert.IsFalse(testAlarmRule2(10, "NG", false, 2));
            Assert.IsFalse(testAlarmRule2(10, "NG", true, 3));
        }

        private bool testAlarmRule(int a, string b, bool c)
        {
            int messageCatalogId = 1;
            MessageProcessorFactoryModel msgProcessorFactoryModel = new MessageProcessorFactoryModel();
            SfMessageEventProcessor sfMsgEventProcessor = new SfMessageEventProcessor(msgProcessorFactoryModel);

            msgProcessorFactoryModel.MessageIdAlarmRules = getSampleAlarmRules(messageCatalogId);

            JObject payload = getSampleDeviceMessage1(a, b, c);
            Task<bool> alarmTriggered = sfMsgEventProcessor.RunAlarmRulesTest(msgProcessorFactoryModel, messageCatalogId, payload, false, false);

            return alarmTriggered.Result;
        }

        private bool testAlarmRule2(int a, string b, bool c, int d)
        {
            int messageCatalogId = 1;
            MessageProcessorFactoryModel msgProcessorFactoryModel = new MessageProcessorFactoryModel();
            SfMessageEventProcessor sfMsgEventProcessor = new SfMessageEventProcessor(msgProcessorFactoryModel);

            msgProcessorFactoryModel.MessageIdAlarmRules = getSampleAlarmRules(messageCatalogId);

            JObject payload = getSampleDeviceMessage2(a, b, c, d);
            Task<bool> alarmTriggered = sfMsgEventProcessor.RunAlarmRulesTest(msgProcessorFactoryModel, messageCatalogId, payload, false, false);

            return alarmTriggered.Result;
        }

        private bool testAlarmRuleStringEqual(string left, string op, string right, string testTarget)
        {
            int messageCatalogId = 1;
            MessageProcessorFactoryModel msgProcessorFactoryModel = new MessageProcessorFactoryModel();
            SfMessageEventProcessor sfMsgEventProcessor = new SfMessageEventProcessor(msgProcessorFactoryModel);

            msgProcessorFactoryModel.MessageIdAlarmRules = getSampleAlarmRulesStringEqual(messageCatalogId, left, op, right);
            JObject payload = getSampleDeviceMessageForString(left, testTarget);
            Task<bool> alarmTriggered = sfMsgEventProcessor.RunAlarmRulesTest(msgProcessorFactoryModel, messageCatalogId, payload, false, false);

            return alarmTriggered.Result;
        }

        private JObject getSampleDeviceMessageForString(string left, string testTarget)
        {
            JObject deviceMessage = new JObject();
            deviceMessage.Add("companyId", 1);
            deviceMessage.Add("msgTimestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            deviceMessage.Add("equipmentId", "equipmentId");
            deviceMessage.Add("equipmentRunStatus", 1);
            deviceMessage.Add(left, testTarget);
            return deviceMessage;
        }

        private JObject getSampleDeviceMessage2(int a, string b, bool c, int d)
        {
            JObject deviceMessage = new JObject();
            deviceMessage.Add("companyId", 1);
            deviceMessage.Add("msgTimestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            deviceMessage.Add("equipmentId", "equipmentId");
            deviceMessage.Add("equipmentRunStatus", 1);
            deviceMessage.Add("A", 4);
            deviceMessage.Add("B", "Walker");
            deviceMessage.Add("C", false);
            deviceMessage.Add("A1", a);
            deviceMessage.Add("B1", b);
            deviceMessage.Add("C1", c);
            deviceMessage.Add("D1", d);

            return deviceMessage;
        }

        private JObject getSampleDeviceMessage1(int a, string b, bool c)
        {
            JObject deviceMessage = new JObject();
            deviceMessage.Add("companyId", 1);
            deviceMessage.Add("msgTimestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            deviceMessage.Add("equipmentId", "equipmentId");
            deviceMessage.Add("equipmentRunStatus", 1);
            deviceMessage.Add("A", a);
            deviceMessage.Add("B", b);
            deviceMessage.Add("C", c);

            return deviceMessage;
        }

        private Dictionary<int, List<AlarmRuleCatalogEngine>> getSampleAlarmRulesStringEqual(int messageCatalogId, string name, string op, string value)
        {
            Dictionary<int, List<AlarmRuleCatalogEngine>> alarmRulesList = new Dictionary<int, List<AlarmRuleCatalogEngine>>();

            /* rule 1 */
            int alarmRuleCatalogId = 1;
            int keepHappenInSec = 60;
            List<RuleEngineItem> ruleEngineItems1 = new List<RuleEngineItem>();
            ruleEngineItems1.Add(createRuleEngineItem(name, op, value, "string", "END"));
            AlarmRuleCatalogEngine arcEngine1 = createAlarmRuleCatalogEngine(messageCatalogId, alarmRuleCatalogId,
                keepHappenInSec, createRuleEngineItemDictionary(ruleEngineItems1));

            List<AlarmRuleCatalogEngine> arcEngineList = new List<AlarmRuleCatalogEngine>();
            arcEngineList.Add(arcEngine1);

            alarmRulesList.Add(messageCatalogId, arcEngineList);

            return alarmRulesList;
        }

        private Dictionary<int, List<AlarmRuleCatalogEngine>> getSampleAlarmRules(int messageCatalogId)
        {
            Dictionary<int, List<AlarmRuleCatalogEngine>> alarmRulesList = new Dictionary<int, List<AlarmRuleCatalogEngine>>();

            /* rule 1 */
            int alarmRuleCatalogId = 1;
            int keepHappenInSec = 60;
            List<RuleEngineItem> ruleEngineItems = new List<RuleEngineItem>();
            ruleEngineItems.Add(createRuleEngineItem("A", ">", "0", "numeric", "AND"));
            ruleEngineItems.Add(createRuleEngineItem("A", "<=", "3.5", "numeric", "END"));
            AlarmRuleCatalogEngine arcEngine = createAlarmRuleCatalogEngine(messageCatalogId, alarmRuleCatalogId,
                keepHappenInSec, createRuleEngineItemDictionary(ruleEngineItems));

            /* rule 2 */
            alarmRuleCatalogId = 2;
            List<RuleEngineItem> ruleEngineItems2 = new List<RuleEngineItem>();
            ruleEngineItems2.Add(createRuleEngineItem("B", "!=", "Walker", "string", "END"));
            AlarmRuleCatalogEngine arcEngine2 = createAlarmRuleCatalogEngine(messageCatalogId, alarmRuleCatalogId,
                keepHappenInSec, createRuleEngineItemDictionary(ruleEngineItems2));

            /* rule 3 */
            alarmRuleCatalogId = 3;
            List<RuleEngineItem> ruleEngineItems3 = new List<RuleEngineItem>();
            ruleEngineItems3.Add(createRuleEngineItem("C", "!=", "false", "bool", "END"));
            AlarmRuleCatalogEngine arcEngine3 = createAlarmRuleCatalogEngine(messageCatalogId, alarmRuleCatalogId,
                keepHappenInSec, createRuleEngineItemDictionary(ruleEngineItems3));

            /* rule 4 */
            alarmRuleCatalogId = 4;
            List<RuleEngineItem> ruleEngineItems4 = new List<RuleEngineItem>();
            ruleEngineItems4.Add(createRuleEngineItem("A1", ">=", "10", "numeric", "OR"));
            ruleEngineItems4.Add(createRuleEngineItem("B1", "=", "PASS", "string", "AND"));
            ruleEngineItems4.Add(createRuleEngineItem("C1", "=", "true", "bool", "AND"));
            ruleEngineItems4.Add(createRuleEngineItem("D1", "=", "2", "numeric", "END"));
            AlarmRuleCatalogEngine arcEngine4 = createAlarmRuleCatalogEngine(messageCatalogId, alarmRuleCatalogId,
                keepHappenInSec, createRuleEngineItemDictionary(ruleEngineItems4));

            /* Rule list */
            List<AlarmRuleCatalogEngine> arcEngineList = new List<AlarmRuleCatalogEngine>();
            arcEngineList.Add(arcEngine);
            arcEngineList.Add(arcEngine2);
            arcEngineList.Add(arcEngine3);
            arcEngineList.Add(arcEngine4);

            alarmRulesList.Add(messageCatalogId, arcEngineList);

            return alarmRulesList;
        }

        private Dictionary<string, RuleEngineItem> createRuleEngineItemDictionary(List<RuleEngineItem> ruleEngineItems)
        {
            Dictionary<string, RuleEngineItem> ruleEngineItemDictionary = new Dictionary<string, RuleEngineItem>();

            for (int i = 0; i < ruleEngineItems.Count; i++)
            {
                RuleEngineItem rei = ruleEngineItems[i];

                ruleEngineItemDictionary.Add(rei.ElementName + "-" + i, rei);
            }

            return ruleEngineItemDictionary;
        }

        private AlarmRuleCatalogEngine createAlarmRuleCatalogEngine(int messageCatalogId, int alarmRuleCatalogId, int keepHappenInSec, Dictionary<string, RuleEngineItem> ruleEngineItems)
        {
            AlarmRuleCatalog arc = new AlarmRuleCatalog();
            arc.Id = alarmRuleCatalogId;
            arc.KeepHappenInSec = keepHappenInSec;
            arc.Name = "Test Alarm Rule Name";
            arc.Description = "Test Alarm Rule Description";
            arc.MessageCatalogId = messageCatalogId;

            AlarmRuleCatalogEngine are = new AlarmRuleCatalogEngine();
            are.AlarmRuleCatalogId = alarmRuleCatalogId;
            are.RuleEngineItems = ruleEngineItems;
            are.LastTriggerTime = new DateTime(2017, 1, 1);
            are.Triggered = false;
            are.AlarmRuleCatalog = arc;

            return are;
        }

        private RuleEngineItem createRuleEngineItem(string fullElementName, string op, string right, string dateType, string bitWiseOperation)
        {
            IoTHubAliasEventMessageReceiver receiver = new IoTHubAliasEventMessageReceiver("test");

            RuleEngineItem rei = new RuleEngineItem();
            rei.ElementName = fullElementName;
            rei.DataType = AlarmRuleItemEngineUtility.GetSupportDataType(dateType);
            rei.OrderOperation = bitWiseOperation;
            rei.Result = false;

            if (rei.DataType == SupportDataTypeEnum.String &&
                    (string.IsNullOrEmpty(right) || right.ToLower().Equals("null")))
            {
                right = "null";
            }

            if (rei.DataType == SupportDataTypeEnum.Numeric || rei.DataType == SupportDataTypeEnum.Bool)
            {
                rei.Equality = receiver.CreateCompiledRuleFuncTest(rei.DataType, op, right);
            }
            else
            {
                // SupportDataTypeEnum.String
                rei.StringRightValue = right;
                rei.StringEqualOperation = op;
            }

            return rei;
        }

        private bool runSingleRuleItem(SupportDataTypeEnum testDataType, string elementName, string op, string right, JObject testObject)
        {
            MessageProcessorFactoryModel msgProcessorFactoryModel = new MessageProcessorFactoryModel();

            IoTHubAliasEventMessageReceiver receiver = new IoTHubAliasEventMessageReceiver("test");
            Func<DynamicMessageElement, bool> func = receiver.CreateCompiledRuleFuncTest(testDataType, op, right);
            SfMessageEventProcessor sfMsgEventProcessor = new SfMessageEventProcessor(msgProcessorFactoryModel);

            RuleEngineItem ruleEngineItem = new RuleEngineItem
            {
                ElementName = elementName,
                DataType = testDataType,
                OrderOperation = "end",
                Result = false,
                Equality = func
            };

            sfMsgEventProcessor.RunSingleRuleItemTest(ruleEngineItem, testObject);

            return ruleEngineItem.Result;

        }

        private bool testSingleRuleNumericItem(decimal left, string op, decimal right)
        {
            string elementName = "elementName";
            JObject testObject = new JObject();
            testObject.Add(elementName, left);

            SupportDataTypeEnum testDataType = SupportDataTypeEnum.Numeric;

            return runSingleRuleItem(testDataType, elementName, op, right.ToString(), testObject);
        }

        private bool testSingleRuleStringItem(string left, string op, string right)
        {
            string elementName = "elementName";
            JObject testObject = new JObject();
            testObject.Add(elementName, left);

            SupportDataTypeEnum testDataType = SupportDataTypeEnum.String;

            return runSingleRuleItem(testDataType, elementName, op, right, testObject);
        }

        private bool testSingleRuleBoolItem(bool left, string op, bool right)
        {
            string elementName = "elementName";

            JObject testObject = new JObject();
            testObject.Add(elementName, left);

            SupportDataTypeEnum testDataType = SupportDataTypeEnum.Bool;

            return runSingleRuleItem(testDataType, elementName, op, right.ToString(), testObject);
        }

        private bool testBoolRule(bool left, string op, bool right)
        {
            return AlarmRuleItemEngineUtility.ComplieBoolRule(left, op, right);
        }

    }
}
