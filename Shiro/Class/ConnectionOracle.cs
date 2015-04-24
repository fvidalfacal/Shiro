using System;
using System.Data;
using System.Linq;
using System.Windows;

using Oracle.ManagedDataAccess.Client;

namespace Shiro.Class
{
    public class ConnectionOracle
    {
        private static OracleConnection ConnectionOrcl;
        private static Boolean ConnectionIsStarted;

        private ConnectionOracle()
        {
            ConnectionOrcl = new OracleConnection("user id=SHIRO;password=pw;data source=localhost:1521/xe");
            ConnectionOrcl.Open();
            ConnectionIsStarted = true;
        }

        private static OracleConnection GetConnection()
        {
            if (!ConnectionIsStarted)
            {
                new ConnectionOracle();
            }
            return ConnectionOrcl;
        }

        public static OracleCommand Command(string query)
        {
            return new OracleCommand {Connection = GetConnection(), CommandText = query};
        }

        public static OracleCommand CommandStored(string query)
        {
            return new OracleCommand
            {
                CommandType = CommandType.StoredProcedure,
                Connection = GetConnection(),
                CommandText = query
            };
        }

        public static OracleCommand GetAll(string tableQuery)
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
            var Command = ConnectionOracle.Command(queryInsert);
            Command.Prepare();
            Command.ExecuteNonQuery();
        }

        public static void Delete(string tableQuery, object ID, string param = null)
        {
            var Id_Table = String.Empty;
            Id_Table = param ?? tableQuery;
            var query = String.Format("DELETE FROM {0} WHERE ID_{1} = {2}", tableQuery, Id_Table, ID);
            var Command = ConnectionOracle.Command(query);
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
            var Command = ConnectionOracle.Command(query);
            Command.Prepare();
            Command.ExecuteNonQuery();
        }

        private static Int32 sizeOf(IDbCommand command)
        {
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public static Int32 sizeOf(string query)
        {
            return sizeOf(Command(String.Format("SELECT COUNT(*) FROM ({0})", query)));
        }
    }
}
