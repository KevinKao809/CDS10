using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yare.Lib.Enums;

namespace IoTHubEventProcessor
{
    public class AlarmRuleItemEngineUtility
    {
        public static bool ComplieBoolRule(bool left, string op, bool right)
        {
            switch (op.ToLower())
            {
                case "and":
                    return left & right;
                case "or":
                    return left | right;
                case "xor":
                    return left ^ right;
                default:
                    throw new InvalidOperationException();
            }
        }

        public static EqualityOperationEnum GetEqualityOperation(string operation)
        {
            EqualityOperationEnum ret;
            switch (operation)
            {
                case "=":
                    ret = EqualityOperationEnum.Equal;
                    break;
                case ">":
                    ret = EqualityOperationEnum.GreaterThan;
                    break;
                case "<":
                    ret = EqualityOperationEnum.LessThan;
                    break;
                case "<=":
                    ret = EqualityOperationEnum.LessThanOrEqual;
                    break;
                case ">=":
                    ret = EqualityOperationEnum.GreaterThanOrEqual;
                    break;
                case "!=":
                    ret = EqualityOperationEnum.NotEqual;
                    break;
                default:
                    throw new ArgumentNullException();
            }

            return ret;
        }

        public static SupportDataTypeEnum GetSupportDataType(string type)
        {
            switch (type.ToLower())
            {
                case "numeric":
                    return SupportDataTypeEnum.Numeric;
                case "datetime":
                case "string":
                    return SupportDataTypeEnum.String;
                case "bool":
                    return SupportDataTypeEnum.Bool;
                default:
                    throw new NotSupportedException("Type Not supported = " + type);
            }
        }
    }
}
