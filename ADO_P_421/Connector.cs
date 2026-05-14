using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;

namespace ADO_P_421
{
	class Connector
	{
		static SqlConnection connection;
		public Connector(string connection_string)
		{
			Console.WriteLine(connection_string);
			connection = new SqlConnection(connection_string);
		}
		public string GetPrimaryKeyColumnName(string table)
		{
			//@"RAW-строка"
			string cmd = $@"SELECT	COLUMN_NAME
FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE
WHERE   INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME =
(
SELECT  CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE   CONSTRAINT_TYPE = N'PRIMARY KEY' AND TABLE_NAME = N'{table}'
)";
			return Scalar(cmd).ToString();
		}
		public object GetLastPrimaryKey(string table)
		{
			string cmd = $"SELECT MAX({GetPrimaryKeyColumnName(table)}) FROM {table}";
			return Scalar(cmd);
		}
		public object GetNextPrimaryKey(string table)
		{
			return (int)GetLastPrimaryKey(table) + 1;
		}
		public void Insert(string table, string fields, string values)
		{
			string pk = GetPrimaryKeyColumnName(table);
			string[] s_fields = fields.Split(',');
			string[] s_values = values.Split(',');
			if (s_fields.Length != s_values.Length) return;
			string condition = "";
			for (int i = 0; i < s_fields.Length; i++)
			{
				if (s_fields[i] == pk) continue;
				condition += $"{s_fields[i]}={s_values[i]}";
				if (i != s_fields.Length - 1) condition += " AND ";
			}
			if (Scalar($"SELECT {GetPrimaryKeyColumnName(table)} FROM {table} WHERE {condition}") != null) return;
			Insert($"INSERT {table}({fields}) VALUES({values})");
		}
		public void Insert(string cmd)
		{
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();
			command.ExecuteNonQuery();
			connection.Close();
		}
		public object Scalar(string cmd)
		{
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();
			object value = command.ExecuteScalar();
			connection.Close();
			return value;
		}
		public void Select(string fields, string tables, string condition = "")
		{
			string cmd = $"SELECT {fields} FROM {tables} ";
			if (condition != "" && condition != " ") cmd += $" WHERE {condition}";
			Select(cmd);
		}
		public void Select(string cmd)
		{
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();
			SqlDataReader reader = command.ExecuteReader();
			for (int i = 0; i < reader.FieldCount; i++)
				Console.Write(reader.GetName(i));
			Console.WriteLine();
			while (reader.Read())
			{
				//Console.WriteLine($"{reader[0]}\t{reader[1]}\t{reader[2]}");
				for (int i = 0; i < reader.FieldCount; i++)
					Console.Write(reader[i] + "\t");
				Console.WriteLine();
			}
			reader.Close();
			connection.Close();
		}
	}
}
