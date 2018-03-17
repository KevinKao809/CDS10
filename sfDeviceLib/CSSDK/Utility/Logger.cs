namespace Microsoft.CDS.Devices.Client.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    static class Logger
    {
        [Conditional("DEBUG")]
        public static void showDebug(string className, string description)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[CSSDK - {0}] {1}", className, description);
            Console.ResetColor();
        }

        [Conditional("DEBUG")]
        public static void showError(string className, string description)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[CSSDK - {0}] {1}", className, description);
            Console.ResetColor();
        }

        public static void showDebug(string className, StringBuilder sb)
        {
            showDebug(className, sb.ToString());
        }

        
        public static void showError(string className, StringBuilder sb)
        {
            showError(className, sb.ToString());
        }
    }
}
