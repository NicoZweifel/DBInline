using System.Collections.Generic;
using System.Data;
using System.Linq;
using DBInline.Classes;


namespace DBInline.Interfaces
{
    public class ClauseBuilder
    {
        private readonly List<string> _whereClauses = new List<string>();

        public string CommandText { get; set; }

        public string OrderClause
        {
            set => _orderClause = value;
        }

        public int Limit
        {
            set => _limit = value;
        }

        private string _orderClause = "";
        private int _limit;

        public void AddToWhereString(string whereClause)
        {
            _whereClauses.Add(whereClause);
        }

        internal void BuildClauses(Command command)
        {
            if (CommandText.Trim().EndsWith(";"))
                CommandText =
                    CommandText.Substring(0, CommandText.Length - 1);
            var whereStr = _whereClauses.Any() ? $" WHERE {string.Join(" AND ", _whereClauses)}" : "";
            var orderStr = _orderClause.Length != 0 ? $"ORDER BY {_orderClause}" : "";
            var limitStr = _limit > 0 ? $" LIMIT {_limit}" : "";
            command.DbCommand.CommandText = $"{CommandText} {whereStr} {orderStr} {limitStr};";
        }
    }
}