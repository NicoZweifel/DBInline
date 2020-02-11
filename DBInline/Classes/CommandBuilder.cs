using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DBInline.Classes
{

    public enum QueryType
    {
        None = 0,
        Select = 1,
        Insert = 2,
        Update = 3,
        Delete = 4,
        Drop = 5
    }


    internal class CommandBuilder
    {
        private readonly List<string> _whereClauses = new List<string>();
        public string CommandText { get; set; }

        private string _currentTable;
        
        private QueryType _currentCommand;

        private readonly List<string> _currentColumns = new List<string>();

        private readonly List<string> _currentValues = new List<string>();

        public List<string> CurrentFields = new List<string>();

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

        public CommandBuilder(string currentTable)
        {
            this._currentTable = currentTable;
        }

        public void AddWhere(string whereClause)
        {
            _whereClauses.Add(whereClause);
        }

        public void AddOr(string whereClause)
        {
            _whereClauses[^1] += $" OR {whereClause} ";
        }

        internal void SetCommandText(Command command)
        {
            BuildCommandText();
            BuildClauses();
            command.CommandText = CommandText;
        }

        private void BuildClauses()
        {
            var whereStr = _whereClauses.Any() ? $" WHERE ({string.Join(" AND ", _whereClauses)})" : "";
            var orderStr = _orderClause.Length != 0 ? $"ORDER BY {_orderClause}" : "";
            var limitStr = _limit > 0 ? $" LIMIT {_limit}" : "";
            CommandText = $"{CommandText} {whereStr} {orderStr} {limitStr};";
            _whereClauses.Clear();
            _orderClause = "";
            _limit = 0;
        }
        
        public void AddColumns(IEnumerable<string> columnNames)
        {
            _currentColumns.AddRange(columnNames ?? new string[] { });
        }

        private void CheckCurrentCommand(string tableName)
        {
            if (_currentCommand == 0) return;
            BuildCommandText();
            BuildClauses();
            CommandText += $";{Environment.NewLine}";
        }

        public void StartSelect()
        {
            CheckCurrentCommand("");
            _currentCommand = QueryType.Select;
        }

        public void AddValues(IEnumerable<string> values)
        {
            _currentValues.AddRange(values ?? new string[] { });
        }

        public void AddTableName(string tableName)
        {
            _currentTable = tableName;
        }

        public void AddFrom(string tableName)
        {
            _currentTable = tableName;
        }

        public void AddInsert(string tableName)
        {
            CheckCurrentCommand(tableName);
            _currentCommand = QueryType.Insert;
        }

        public void AddUpdate(string tableName)
        {
            CheckCurrentCommand(tableName);
            _currentCommand = QueryType.Update;
        }

        public void AddDrop(string tableName)
        {
            CheckCurrentCommand(tableName);
            _currentCommand = QueryType.Drop;
        }


        private void BuildCommandText()
        {
            switch (_currentCommand)
            {
                case QueryType.None:
                    break;
                case QueryType.Select:
                    CommandText = $"SELECT {string.Join(",", _currentColumns)} FROM {_currentTable} ";
                    break;
                case QueryType.Insert:
                    CommandText = $"INSERT INTO {_currentTable} {string.Join(",",_currentColumns)} VALUES {string.Join(",",_currentValues)}";
                    break;
                case QueryType.Update:
                    CommandText = $"UPDATE {_currentTable} ";
                    break;
                case QueryType.Delete:
                    break;
                case QueryType.Drop:
                    CommandText = $"DROP TABLE {_currentTable} ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}