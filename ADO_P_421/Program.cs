//#define SELECT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;

namespace ADO_P_421
{
	class Program
	{
		static SqlConnection connection;
		static void Main(string[] args)
		{
			string connection_string = "Data Source=(localdb)\\MSSQLLocalDB;" +
				"Initial Catalog=Movies_P_421;" +
				"Integrated Security=True;" +
				"Connect Timeout=30;" +
				"Encrypt=False;" +
				"TrustServerCertificate=False;" +
				"ApplicationIntent=ReadWrite;" +
				"MultiSubnetFailover=False";
			Console.WriteLine(connection_string);
			connection = new SqlConnection(connection_string);

#if SELECT
			string cmd = "SELECT first_name,last_name FROM Directors";
			Select(cmd);
			Console.WriteLine($"Количество записей: {Scalar("SELECT COUNT(*) FROM Directors")}");
			Console.WriteLine("\n------------------------------\n");
			Select("SELECT title,release_date,first_name,last_name FROM Movies JOIN Directors ON (director=director_id)");
			Console.WriteLine("\n------------------------------\n");

			Select("title,release_date,first_name,last_name", "Movies,Directors", "director=director_id");
			Console.WriteLine($"Количество записей: {Scalar("SELECT COUNT(*) FROM Movies")}");
#endif
			//Insert($"INSERT Directors(director_id,first_name,last_name) VALUES ({GetNextPrimaryKey("Directors")}, N'Gray', N'Scott')");
			//Insert
			//(
			//	"Directors",
			//	"director_id,first_name,last_name",
			//	$"{GetNextPrimaryKey("Directors")},N'Sheldon',N'Letich'"
			//);
			//Select("*", "Directors");
			//Console.WriteLine(GetPrimaryKeyColumnName("Movies"));
			//Console.WriteLine(GetLastPrimaryKey("Movies"));
			//Console.WriteLine(GetNextPrimaryKey("Movies"));
			Insert
			(
				"Movies",
				"movie_id,title,release_date,director",
				$"{GetNextPrimaryKey("Movies")},N'Avatar',N'2009-12-17',1"
			);
			Select
			(
			"movie_id,title,release_date,first_name,last_name",
			"Movies,Directors",
			"director=director_id"
			);
		}
		static string GetPrimaryKeyColumnName(string table)
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
		static object GetLastPrimaryKey(string table)
		{
			string cmd = $"SELECT MAX({GetPrimaryKeyColumnName(table)}) FROM {table}";
			return Scalar(cmd);
		}
		static object GetNextPrimaryKey(string table)
		{
			return (int)GetLastPrimaryKey(table) + 1;
		}
		static void Insert(string table, string fields, string values)
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
		static void Insert(string cmd)
		{
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();
			command.ExecuteNonQuery();
			connection.Close();
		}
		static object Scalar(string cmd)
		{
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();
			object value = command.ExecuteScalar();
			connection.Close();
			return value;
		}
		static void Select(string fields, string tables, string condition = "")
		{
			string cmd = $"SELECT {fields} FROM {tables} ";
			if (condition != "" && condition != " ") cmd += $" WHERE {condition}";
			Select(cmd);
		}
		static void Select(string cmd)
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
