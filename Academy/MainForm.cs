using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Configuration;
using DBtools;

namespace Academy
{
	public partial class MainForm : Form
	{
		Connector connector;
		DataGridView[] tables;
		Query[] queries =
		{
			new Query
				(
"stud_id,FORMATMESSAGE(N'%s %s %s',last_name,first_name,middle_name)AS N'Студент',birth_date,group_name,direction_name",
"Students,Groups,Directions",
"[group]=group_id AND direction=direction_id"
				),
			new Query
				(
"group_id,group_name,direction_name,start_date,start_time,learning_days",
"Groups,Directions",
"direction=direction_id"
				),
			new Query("*", "Directions"),
			new Query("*", "Disciplines"),
			new Query("*", "Teachers"),
		};
		public MainForm()
		{
			AllocConsole();
			InitializeComponent();
			tables = new DataGridView[] { dgvStudents, dgvGroups, dgvDirections, dgvDisciplines, dgvTeachers };
			connector = new Connector(ConfigurationManager.ConnectionStrings["P_421_Import"].ConnectionString);
			//dgvStudents.DataSource = connector.Load("SELECT * FROM Students");
			/*dgvStudents.DataSource = connector.Load
				(
				"stud_id,last_name,first_name,middle_name,birth_date,group_name,direction_name",
				"Students,Groups,Directions",
				"[group]=group_id AND direction=direction_id"
				);
			toolStripStatusLabel.Text = $"Количество записей: {dgvStudents.RowCount - 1}";*/
			tabControl.SelectedIndex = 0;
			tabControl_SelectedIndexChanged(tabControl, null);
			////////////////////////////////////
			cbGroupsDirection.DataSource = connector.Load("SELECT * FROM Directions");
			cbGroupsDirection.DisplayMember = "direction_name";
			cbGroupsDirection.ValueMember = "direction_id";
			//cbGroupsDirection.SelectedValue = 0;
		}
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			int i = tabControl.SelectedIndex;
			tables[i].DataSource = connector.Load(queries[i].ToString());
			//tables[i].DataSource = connector.Select("*", $"{tabControl.SelectedTab.Text}");
			toolStripStatusLabel.Text = $"Количество записей: {tables[i].RowCount - 1}";
			//for (int c = 0; c < tables[i].ColumnCount-1; c++)tables[i].Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			tables[i].Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			tables[i].Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		}


		private void cbGroupsDirection_SelectionChangeCommitted(object sender, EventArgs e)
		{
			tables[1].DataSource = connector.Load(queries[1].ToString() + $" AND direction={cbGroupsDirection.SelectedValue}");
			//Console.WriteLine($"SelectedIndex:{cbGroupsDirection.SelectedIndex}");
			//Console.WriteLine($"SelectedItem:{cbGroupsDirection.SelectedItem}");
			//Console.WriteLine($"SelectedText:{cbGroupsDirection.SelectedText}");
			//Console.WriteLine($"SelectedValue:{cbGroupsDirection.SelectedValue}");
			//Console.WriteLine(cbGroupsDirection.SelectedValue.GetType());
		}
	}
}
