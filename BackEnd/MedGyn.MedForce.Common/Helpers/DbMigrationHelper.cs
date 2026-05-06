using System;
using System.Collections.Generic;
using System.Text;

namespace MedGyn.MedForce.Common.Helpers
{
    public static class DbMigrationHelper
    {
        public const string DefaultSchema = "dbo";
        public const string AttributeTypeTable = "AttributeType";

        public static string CreateInsertStatement(string schema, string table, IDictionary<string, string> identifiers, IDictionary<string, string> columnValues)
        {
            var isNotFirstRow = false;
            var result = new StringBuilder();

            result.Append("IF NOT EXISTS (SELECT * FROM ");
            result.Append(String.Format("[{0}].[{1}] t WHERE ", schema, table));

            if (identifiers == null)
            {
                result.Append("0 = 1");
            } else
            {
                foreach (var identifier in identifiers)
                {
                    result.Append(isNotFirstRow ? "AND " : "");
                    result.Append(String.Format("t.[{0}] = {1} ", identifier.Key, identifier.Value));
                    isNotFirstRow = true;
                }
            }
            result.Append(String.Format(") BEGIN INSERT INTO [{0}].[{1}] (", schema, table));

            var keys = new StringBuilder();
            var values = new StringBuilder();
            isNotFirstRow = false;
            foreach (var columnValue in columnValues)
            {
                keys.Append(isNotFirstRow ? "," : "");
                keys.Append(String.Format("[{0}]", columnValue.Key));
                values.Append(isNotFirstRow ? "," : "");
                values.Append(String.Format("{0}", columnValue.Value));
                isNotFirstRow = true;
            }
            result.Append(keys.ToString());
            result.Append(") VALUES (");
            result.Append(values.ToString());
            result.Append("); END");

            return result.ToString();
        }
        public static string CreateInsertStatement(string schema, string table, IDictionary<string, string> columnValues)
        {
            return CreateInsertStatement(schema, table, null, columnValues);
        }
    }
}
