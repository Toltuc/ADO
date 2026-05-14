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
			Connector connector = new Connector(connection_string);

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
			connector.Insert
			(
				"Movies",
				"movie_id,title,release_date,director",
				$"{connector.GetNextPrimaryKey("Movies")},N'Avatar',N'2009-12-17',1"
			);
			connector.Select
			(
			"movie_id,title,release_date,first_name,last_name",
			"Movies,Directors",
			"director=director_id"
			);
		}
		
	}
}
