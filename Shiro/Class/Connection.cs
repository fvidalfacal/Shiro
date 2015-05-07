// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from Shiro INC. team.
//  
// Copyrights (c) 2014 Shiro INC. All rights reserved.

using System;
using System.Data;
using System.Linq;
using System.Windows;

using Oracle.ManagedDataAccess.Client;

namespace Shiro.Class
{
    internal sealed class Connection
    {
        private static OracleConnection _connectionOrcl;
        private static Boolean _connectionIsStarted;

        private Connection()
        {
            _connectionOrcl = new OracleConnection("user id=SHIRO;password=pw;data source=localhost:1521/xe");
            _connectionOrcl.Open();
            _connectionIsStarted = true;
        }

        private static OracleConnection GetConnection()
        {
            if(!_connectionIsStarted)
            {
                new Connection();
            }
            return _connectionOrcl;
        }

        internal static OracleCommand Command(string query)
        {
            return new OracleCommand {Connection = GetConnection(), CommandText = query};
        }

        public static OracleCommand CommandStored(string query)
        {
            return new OracleCommand {CommandType = CommandType.StoredProcedure, Connection = GetConnection(), CommandText = query};
        }

        internal static OracleCommand GetAll(string tableQuery)
        {
            return Command(String.Format("SELECT * FROM {0}", tableQuery));
        }

        /// <summary>
        ///   Requête d'insertion dans la base de données
        /// </summary>
        /// <param name="tableQuery">string : Nom de la table</param>
        /// <param name="id">id de l'object a modifier</param>
        /// <param name="value">Tableau de string à double entré {{"nom de la colonne","valeur"}{"nom de la colonne","valeur"}}</param>
        internal static void Insert(string tableQuery, params Object[] value)
        {
            var query = value.Aggregate(String.Empty, (current, field) => current + ("'" + field + "',"));
            query = query.Substring(0, query.Length - 1);
            var queryInsert = String.Format("INSERT INTO {0} VALUES ({1})", tableQuery, query);
            var command = Command(queryInsert);
            command.Prepare();
            command.ExecuteNonQuery();
        }

        /// <summary>
        ///   Requête de suppression dans la base de données
        /// </summary>
        /// <param name="tableQuery">string : Nom de la table</param>
        /// <param name="id">id de l'object a modifier</param>
        /// <param name="value">Tableau de string à double entré {{"nom de la colonne","valeur"}{"nom de la colonne","valeur"}}</param>
        public static void Delete(string tableQuery, String[,] value)
        {
            var queryWhere = String.Empty;
            var size = value.Length / 2;
            for(var i = 0; i < size; i++)
            {
                queryWhere += String.Format("{0} = '{1}' ,", value[i, 0], value[i, 1]);
            }
            var query = String.Format("DELETE FROM {0} WHERE {1}", tableQuery, queryWhere.Substring(0, queryWhere.Length - 1));
            var command = Command(query);
            command.Prepare();
            command.ExecuteNonQuery();
        }

        /// <summary>
        ///   Requête de mise à jour dans la base de données
        /// </summary>
        /// <param name="tableQuery">string : Nom de la table</param>
        /// <param name="id">id de l'object a modifier</param>
        /// <param name="value">Tableau de string à double entré {{"nom de la colonne","valeur"}{"nom de la colonne","valeur"}}</param>
        public static void Update(string tableQuery, int id, String[,] value)
        {
            var query = String.Empty;
            var size = value.Length / 2;
            for(var i = 0; i < size; i++)
            {
                query += String.Format("{0} = '{1}' ,", value[i, 0], value[i, 1]);
            }
            query = String.Format("UPDATE {0} SET {2} WHERE ID_{0} = {1}", tableQuery, id, query.Substring(0, query.Length - 1));
            var command = Command(query);
            command.Prepare();
            command.ExecuteNonQuery();
        }

        public static Object GetFirst(string query)
        {
            var command = Command(query);
            return command.ExecuteScalar();
        }


        /// <summary>
        /// Convertis la taille de la requête en int32
        /// </summary>
        /// <param name="command">Instruction SQL</param>
        /// <returns></returns>
        private static int SizeOf(IDbCommand command)
        {

            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// Retourne la taille du résultat de la requête
        /// </summary>
        /// <param name="query">Argument de la requête SQL</param>
        /// <returns></returns>
        public static int SizeOf(string query)
        {
            return SizeOf(Command(string.Format("SELECT COUNT(*) FROM ({0})", query)));
        }
    }
}