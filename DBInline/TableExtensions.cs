using System.Data;

namespace DBInline
{
    public static partial class Extensions
    {
     
        public static string ToJson(this DataTable @this)
        {
            var res = "";
            foreach (DataRow row in @this.Rows)
            {
                res += "{";
                var columnIndex = 0;
                foreach (DataColumn column in @this.Columns)
                {
                    res += $"\"{column.ColumnName}\":";
                    res += $"{row[columnIndex]}";
                    columnIndex += 1;
                    if (columnIndex != @this.Columns.Count)
                    {
                        res += ",";
                    }
                }
                res += "}";
            }
            res += "}";
            return res;
        }
    }
}
