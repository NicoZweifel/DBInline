using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using DBInline.Classes;
using DBInline.Interfaces;

namespace DBInline
{
    public static class ContextController
    {
        internal static DatabaseContext DefaultContext { get;private set; }

        internal static readonly Dictionary<string, DatabaseContext> ContextByName =
            new Dictionary<string, DatabaseContext>();

        public static bool Connected => DefaultContext != null;


        public static IConnectionSource Connection()
        {
            if (DefaultContext == null) throw new InvalidOperationException("Context is Null");
            return DefaultContext;
        }

        public static IConnectionSource AddContext(Database type, string connectionString, bool setAsDefault = false)
        {
            var context = new DatabaseContext("Default", connectionString, type);
            DefaultContext = context;
            return context;
        }

        public static IConnectionSource AddContext(string name, Database type, string connectionString,
            bool setAsDefault = false)
        {
            var context = new DatabaseContext(name, connectionString, type);
            ContextByName.Add(name, context);
            if (setAsDefault) DefaultContext = context;
            return context;
        }
    }
}
