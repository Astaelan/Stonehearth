using Stonehearth.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Stonehearth
{
    public static class DB
    {
        public sealed class QueryBuilder
        {
            private StringBuilder mQuery = new StringBuilder();
            private List<object> mParameters = new List<object>();

            public void Clear()
            {
                mQuery = new StringBuilder();
                mParameters.Clear();
            }

            public void Append(string pFormat, params object[] pArgs)
            {
                for (int index = pArgs.Length - 1; index >= 0; --index)
                    pFormat = pFormat.Replace("@" + index.ToString(), "@" + (mParameters.Count + index).ToString());
                mParameters.AddRange(pArgs);
                mQuery.Append(pFormat);
            }

            public object[] Parameters { get { return mParameters.ToArray(); } }

            public override string ToString() { return mQuery.ToString(); }
        }

        public static SqlConnection Open()
        {
            SqlConnection connection = new SqlConnection(Settings.Default.Database);
            connection.Open();
            return connection;
        }

        public static int Execute(this SqlConnection pThis, SqlTransaction pTransaction, string pFormat, params object[] pArgs)
        {
            SqlCommand cmd = pThis.CreateCommand();
            if (pTransaction != null) cmd.Transaction = pTransaction;
            cmd.CommandText = pFormat;
            for (int index = 0; index < pArgs.Length; ++index) cmd.Parameters.AddWithValue("@" + index, pArgs[index] ?? DBNull.Value);
            return cmd.ExecuteNonQuery();
        }
        public static int Execute(this SqlConnection pThis, SqlTransaction pTransaction, QueryBuilder pQueryBuilder)
        {
            return Execute(pThis, pTransaction, pQueryBuilder.ToString(), pQueryBuilder.Parameters);
        }

        public static T ExecuteScalar<T>(this SqlConnection pThis, SqlTransaction pTransaction, string pFormat, params object[] pArgs)
        {
            SqlCommand cmd = pThis.CreateCommand();
            if (pTransaction != null) cmd.Transaction = pTransaction;
            cmd.CommandText = pFormat;
            for (int index = 0; index < pArgs.Length; ++index) cmd.Parameters.AddWithValue("@" + index, pArgs[index] ?? DBNull.Value);
            return (T)cmd.ExecuteScalar();
        }
        public static T ExecuteScalar<T>(this SqlConnection pThis, SqlTransaction pTransaction, QueryBuilder pQueryBuilder)
        {
            return ExecuteScalar<T>(pThis, pTransaction, pQueryBuilder.ToString(), pQueryBuilder.Parameters);
        }

        public static SqlDataReader ExecuteReader(this SqlConnection pThis, SqlTransaction pTransaction, string pFormat, params object[] pArgs)
        {
            SqlCommand cmd = pThis.CreateCommand();
            if (pTransaction != null) cmd.Transaction = pTransaction;
            cmd.CommandText = pFormat;
            for (int index = 0; index < pArgs.Length; ++index) cmd.Parameters.AddWithValue("@" + index, pArgs[index] ?? DBNull.Value);
            return cmd.ExecuteReader();
        }
        public static SqlDataReader ExecuteReader(this SqlConnection pThis, SqlTransaction pTransaction, QueryBuilder pQueryBuilder)
        {
            return ExecuteReader(pThis, pTransaction, pQueryBuilder.ToString(), pQueryBuilder.Parameters);
        }
    }
}
