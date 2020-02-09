using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace DBInline.Interfaces
{
    public class ClauseBuilder
    {
        private readonly List<string> _whereClauses = new List<string>();

        private string _orderClause ="";
        
        public void AddToWhereString(string whereClause)
        {
            _whereClauses.Add(whereClause);
        }

        public void SetOrderClause(string orderClause)
        {
            _orderClause = orderClause;
        }
        
        private void BuildClauses(IDbCommand command)
        {
            var wherestr = _whereClauses.Any() ? $" WHERE {string.Join(" ", _whereClauses)}" : " ";
            var orderstr = _orderClause.Length != 0 ? $" ORDER BY {_orderClause};" : ";";
            command.CommandText = $"{wherestr}{orderstr}";
        }
    }
}