using Microsoft.Azure.Devices.Shared;
using Microsoft.CDS.Devices.Client.SfDeviceTwin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CDS.Devices.Client.Models
{
    public class SfTwinProperties
    {
        public const string SF_LASTUPDATED_TIMESTAMP = "SF_LastUpdatedTimestamp";
        public const string SF_SYSTEM_CONFIG = "SF_SystemConfig";
        public const string SF_CUSTOMIZED_CONFIG = "SF_CustomizedConfig";
        public const string SF_UPDATED_TYPE = "SF_UpdatedType";

        public class SfProperties
        {
            public JObject SystemConfig { get; set; }
            public JObject CustomerConfig { get; set; }
            public int LastUpdatedTimestamp { get; set; }

            public SfProperties(JObject systemConfig, JObject customerConfig, int lastUpdatedTimestamp)
            {
                this.SystemConfig = systemConfig;
                this.CustomerConfig = customerConfig;
                this.LastUpdatedTimestamp = lastUpdatedTimestamp;
            }
        }

        public SfProperties Desired { get; set; }
        public SfProperties Reported { get; set; }

        public static SfTwinProperties CreateSfTwinProperties(Twin twin)
        {
            if(twin != null)
            {
                JObject desiredSystemPropertie = GetDesiredSystemProperties(twin);
                JObject desiredCustomerProperties = GetDesiredCustomerProperties(twin);
                int desiredLastUpdatedTimestamp = GetDesiredLastUpdatedTimestamp(twin);
                SfProperties desired = new SfProperties(desiredSystemPropertie, desiredCustomerProperties, desiredLastUpdatedTimestamp);

                JObject reportedSystemPropertie = GetReportedSystemProperties(twin);
                JObject reportedCustomerProperties = GetReportedCustomerProperties(twin);
                int reportedLastUpdatedTimestamp = GetReportedLastUpdatedTimestam(twin);
                SfProperties reported = new SfProperties(reportedSystemPropertie, reportedCustomerProperties, reportedLastUpdatedTimestamp);

                return new SfTwinProperties(desired, reported);
            }
            else
            {
                return new SfTwinProperties();
            }
            
        }

        public SfTwinProperties(SfProperties desired, SfProperties reported)
        {
            this.Desired = desired;
            this.Reported = reported;
        }

        public SfTwinProperties()
        {
            this.Desired = new SfProperties(new JObject(), new JObject(), 0);
            this.Reported = new SfProperties(new JObject(), new JObject(), 0);
        }

        private static int GetReportedLastUpdatedTimestam(Twin twin)
        {
            TwinCollection reported = getReportedProperties(twin);
            if (reported != null)
            {
                return getLastUpdatedTimestamp(reported);
            }

            return 0;
        }

        public static JObject GetReportedCustomerProperties(Twin twin)
        {
            TwinCollection reported = getReportedProperties(twin);
            if (reported != null)
            {
                return getCustomerProperties(reported);
            }

            return null;
        }

        private static JObject GetReportedSystemProperties(Twin twin)
        {
            TwinCollection reported = getReportedProperties(twin);
            if (reported != null)
            {
                return getSystemProperties(reported);
            }

            return null;
        }

        private static int GetDesiredLastUpdatedTimestamp(Twin twin)
        {
            TwinCollection desired = getDesiredProperties(twin);
            if (desired != null)
            {
                return getLastUpdatedTimestamp(desired);
            }

            return 0;
        }

        public static JObject GetDesiredCustomerProperties(Twin twin)
        {
            TwinCollection desired = getDesiredProperties(twin);

            if (desired != null)
            {
                return getCustomerProperties(desired);
            }

            return null;
        }

        public static JObject GetDesiredSystemProperties(Twin twin)
        {
            TwinCollection desired = getDesiredProperties(twin);
            if (desired != null)
            {
                return getSystemProperties(desired);
            }

            return null;
        }

        private static TwinCollection getDesiredProperties(Twin twin)
        {
            if (twin != null && twin.Properties != null)
            {
                return twin.Properties.Desired;
            }

            return null;
        }

        private static TwinCollection getReportedProperties(Twin twin)
        {
            if (twin != null && twin.Properties != null)
            {
                return twin.Properties.Reported;
            }

            return null;
        }

        private static int getLastUpdatedTimestamp(TwinCollection properties)
        {
            TwinCollection tc = getTwinCollectionProperties(SF_LASTUPDATED_TIMESTAMP, properties);
            if (tc != null)
                return tc[SF_LASTUPDATED_TIMESTAMP];

            return 0;
        }

        private static JObject getSystemProperties(TwinCollection properties)
        {
            TwinCollection tc = getTwinCollectionProperties(SF_SYSTEM_CONFIG, properties);
            if (tc != null)
                return tc[SF_SYSTEM_CONFIG];

            return null;
        }

        private static JObject getCustomerProperties(TwinCollection properties)
        {
            TwinCollection tc = getTwinCollectionProperties(SF_CUSTOMIZED_CONFIG, properties);
            if (tc != null)
            {
                return tc[SF_CUSTOMIZED_CONFIG];
            }


            return null;
        }

        private static TwinCollection getTwinCollectionProperties(string key, TwinCollection properties)
        {
            if (properties.Contains(key) == false)
                return null;

            TwinCollection tc = new TwinCollection();
            tc[key] = properties[key];

            return tc;
        }
    }
}
