using System;
using System.Collections.Generic;
using System.Data;
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
        Drop = 5,
        Create =6,
        InsertFrom =7
    }


    internal class CommandTextBuilder
    {
        private readonly List<string> _whereClauses = new List<string>();
        
        private string _commandText = "";

        private string _currentTable;

        private string _fromTable;
        
        private QueryType _currentCommand;

        private readonly List<string> _columns = new List<string>();

        private readonly List<string> _values = new List<string>();

        private bool _addIfExists;

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
            command.CommandText += _commandText;
        }

        private void BuildClauses()
        {
            var whereStr = _whereClauses.Any() ? $" WHERE ({string.Join(" AND ", _whereClauses)})" : "";
            var orderStr = _orderClause.Length != 0 ? $"ORDER BY {_orderClause}" : "";
            var limitStr = _limit > 0 ? $" LIMIT {_limit}" : "";
            _commandText = $"{_commandText} {whereStr} {orderStr} {limitStr};";
            _whereClauses.Clear();
            _orderClause = "";
            _limit = 0;
        }

        public void AddColumns(IEnumerable<string> columnNames)
        {
            if (_currentCommand == QueryType.InsertFrom)
            {
                _values.AddRange(columnNames ?? new string[] { });
            }
            else
            {
                _columns.AddRange(columnNames ?? new string[] { });
            }
        }

        private void CheckCurrentCommand(string tableName,QueryType newCommand )
        {
            if (_currentCommand == 0)
            {
                _currentTable = tableName;
                _currentCommand = newCommand;
                return;
            }
            BuildCommandText();
            BuildClauses();
            if(newCommand != QueryType.InsertFrom) _commandText += $";{Environment.NewLine}";
            _currentCommand = newCommand;
            _currentTable = tableName;
        }

        public void AddSelect()
        {
            CheckCurrentCommand("",QueryType.Select);
        }

        public void AddValues(IEnumerable<string> values)
        {
            _values.AddRange(values ?? new string[] { });
        }

        public void AddTableName(string tableName)
        {
            _currentTable = tableName;
        }

        public void AddFrom(string tableName)
        {
            if (_currentCommand == QueryType.InsertFrom)
            {
                _fromTable = tableName;
            }
            else
            {
                 _currentTable = tableName;  
            }
        }

        public void AddInsert(string tableName)
        {
            CheckCurrentCommand(tableName,QueryType.Insert);
        }

        public void AddUpdate(string tableName)
        {
            CheckCurrentCommand(tableName, QueryType.Update);
        }

        public void AddDrop(string tableName)
        {
            CheckCurrentCommand(tableName,QueryType.Drop);
        }

        public void AddDelete(string tableName)
        {
            CheckCurrentCommand(tableName,QueryType.Delete);
        }
        
        public void AddCreate(string tableName)
        {
            CheckCurrentCommand(tableName,QueryType.Create);
        }
        
        public void AddToRow<TIn>(TIn value)
        {
            var val = value is string ? $"'{value}'" : value.ToString();
            if (_values[^1].Any()) _values[^1] += ",";
            _values[^1] += $"{val}";
        }

        public void AddRow()
        {
            _values.Add("");
        }

        public void AddColumnDefinition(string column, SqlDbType type, in int charCount)
        {
            var def = charCount > 0 ? $"({charCount})" : "";
            _values.Add($"{column} {type}{def}");
        }
        
        public void AddInsertFromColumns(IEnumerable<string> columns)
        {
            _currentCommand = QueryType.InsertFrom;
            _values.AddRange(columns);
        }

        public void AddUpdateValue(string columnName, string name)
        {
            _values.Add($"{columnName}={name}");
        }
        
        public void AddIfExists()
        {
            _addIfExists = true;
        }
        
        public void AddSelectFrom(IEnumerable<string> columns)
        {
            _currentCommand = QueryType.InsertFrom;
            _values.AddRange(columns);
        }
        
        private void BuildCommandText()
        {
            switch (_currentCommand)
            {
                case QueryType.None:
                    break;
                case QueryType.Select:
                    var columns = _columns.Any() ? string.Join(",", _columns) : "*";
                    _commandText += $" SELECT {columns} FROM \"{_currentTable}\" ";
                    break;
                case QueryType.Insert:
                    _commandText +=
                        $" INSERT INTO \"{_currentTable}\" ({string.Join(",", _columns)}) VALUES {string.Join(",", _values.Select(x=>$"({x})"))} ";
                    break;
                case QueryType.Update:
                    _commandText += $" UPDATE \"{_currentTable}\" SET {string.Join(",",_values)} ";
                    break;
                case QueryType.Delete:
                    _commandText += $" DELETE FROM \"{_currentTable}\" ";
                    break;
                case QueryType.Drop:
                    var ifExists = _addIfExists ? " IF EXISTS " : "";
                    _commandText += $" DROP TABLE {ifExists} \"{_currentTable}\" ";
                    break;
                case QueryType.Create:
                    _commandText += $" CREATE TABLE \"{_currentTable}\" ({string.Join(",", _values)}) ";
                    break;
                case QueryType.InsertFrom:
                    _commandText += $" INSERT INTO \"{_currentTable}\" ({string.Join(",", _columns)}) SELECT  {string.Join(",", _values)} FROM \"{_fromTable}\" ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _currentTable = "";
            _columns.Clear();
            _values.Clear();
            _fromTable = "";
            _addIfExists = false;
        }
    }
}