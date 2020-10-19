using System;
using System.Collections.Generic;
using System.Text;

namespace AorBaseUtility.Extends
{
    public static class StringBuilderExtends
    {

        private static string calculateIndentString(int indent)
        {
            string t = string.Empty;
            for(int i = 0; i < indent; i++)
            {
                t += "\t";
            }
            return t;
        }

        public static void AppendLine(this StringBuilder builder, int indent, string value)
        {
            builder.AppendLine(calculateIndentString(indent) + value);
        }

    }
}
