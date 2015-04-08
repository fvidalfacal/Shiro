// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from MANAGER INC. team.
//  
// Copyrights (c) 2014 MANAGER INC. All rights reserved.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Shiro.Class
{
    public class Connection
    {
        private static SqlConnection ConnectionSql;
        private static Boolean ConnectionIsStarted;

        private Connection()
        {
            ConnectionSql = new SqlConnection("Data Source=FLORIAN-PC; Initial Catalog=Shiro; User id=Florian-PC/Florian;Password=pwsio;");
            ConnectionSql.Open();
            ConnectionIsStarted = true;
        }

        private static SqlConnection GetConnection()
        {
            if (!ConnectionIsStarted)
            {
                new Connection();
            }
            return ConnectionSql;
        }

        public static SqlCommand Command(string query)
        {
            return new SqlCommand {Connection = GetConnection(), CommandText = query};
        }

        public static SqlCommand CommandStored(string query)
        {
            return new SqlCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = GetConnection(),
                CommandText = query
            };
        }

        public static SqlCommand GetAll(string tableQuery)
        {
            return Command(String.Format("SELECT * FROM {0}", tableQuery));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableQuery">string : Nom de la table</param>
        /// <param name="value">Tableau de string à double entré {{"nom de la colonne","valeur"}{"nom de la colonne","valeur"}}</param>
        public static void Insert(string tableQuery, params Object[] value)
        {
            var query = value.Aggregate(String.Empty, (current, field) => current + ("'" + field + "',"));
            query = query.Substring(0, query.Length - 1);
            var queryInsert = String.Format("INSERT INTO {0} VALUES ({1})", tableQuery, query);
            var Command = Connection.Command(queryInsert);
            Command.Prepare();
            Command.ExecuteNonQuery();
        }

        public static void Delete(string tableQuery, object ID, string param = null)
        {
            var Id_Table = String.Empty;
            Id_Table = param ?? tableQuery;
            var query = String.Format("DELETE FROM {0} WHERE ID_{1} = {2}", tableQuery, Id_Table, ID);
            var Command = Connection.Command(query);
            Command.Prepare();
            Command.ExecuteNonQuery();
        }

        public static void Update(string tableQuery, int ID, String[,] value)
        {
            var query = String.Empty;
            var size = value.Length/2;
            for (var i = 0; i < size; i++)
            {
                query += String.Format("{0} = '{1}' ,", value[i, 0], value[i, 1]);
            }
            query = String.Format("UPDATE {0} SET {2} WHERE ID_{0} = {1}", tableQuery, ID,
                query.Substring(0, query.Length - 1));
            var Command = Connection.Command(query);
            Command.Prepare();
            Command.ExecuteNonQuery();
        }
    }
}