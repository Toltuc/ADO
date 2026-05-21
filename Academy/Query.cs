using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy
{
	class Query
	{
		string fields;
		string tables;
		string condition;
		public Query(string fields, string tables, string condition = "")
		{
			this.fields = fields;
			this.tables = tables;
			this.condition = condition;
		}
		public override string ToString()
		{
			string cmd = $"SELECT {fields} FROM {tables} ";
			if (condition != "") cmd += $" WHERE {condition}";
			return cmd;
		}
	}
}
