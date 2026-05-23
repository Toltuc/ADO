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
			//cbGroupsDirection.SelectedValue = 0;
			LoadComboBoxFromBase(cbGroupsDirection, "Directions");
			LoadComboBoxFromBase(cbStudentsGroup, "Groups");
			LoadComboBoxFromBase(cbStudentsDirection, "Directions");
		}
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		void LoadComboBoxFromBase(ComboBox comboBox, string table, string condition = "")
		{
			string column = table.Substring(0, table.Length - 1).ToLower();
			string cmd = $"SELECT {column}_id,{column}_name FROM {table}";
			if (condition != "") cmd += $" WHERE {condition}";
			DataTable dt = connector.Load(cmd);
			DataRow rowDefault = dt.NewRow();
			rowDefault[0] = 0;
			rowDefault[1] = "Все";
			dt.Rows.InsertAt(rowDefault, 0);
			comboBox.DataSource = dt;
			comboBox.DisplayMember = $"{column}_name";
			comboBox.ValueMember = $"{column}_id";
		}

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			int i = tabControl.SelectedIndex;
			tables[i].DataSource = connector.Load(queries[i].ToString());
			//tables[i].DataSource = connector.Select("*", $"{tabControl.SelectedTab.Text}");
			toolStripStatusLabel.Text = $"Количество записей: {tables[i].RowCount - 1}";
			//for (int c = 0; c < tables[i].ColumnCount-1; c++)tables[i].Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			tables[i].Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			tables[i].Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

			//TODO: drop filters
		}

		private void cbGroupsDirection_SelectionChangeCommitted(object sender, EventArgs e)
		{
			tables[1].DataSource = connector.Load
				(
				queries[1].ToString() +
				(cbGroupsDirection.SelectedIndex == 0 ? "" : $" AND direction={cbGroupsDirection.SelectedValue}")
				);
			//Console.WriteLine($"SelectedIndex:{cbGroupsDirection.SelectedIndex}");
			//Console.WriteLine($"SelectedItem:{cbGroupsDirection.SelectedItem}");
			//Console.WriteLine($"SelectedText:{cbGroupsDirection.SelectedText}");
			//Console.WriteLine($"SelectedValue:{cbGroupsDirection.SelectedValue}");
			//Console.WriteLine(cbGroupsDirection.SelectedValue.GetType());
			toolStripStatusLabel.Text = $"Количество записей: {tables[1].RowCount - 1}";
		}

		private void cbStudentsGroup_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (cbStudentsGroup.SelectedIndex == 0)
				cbStudentsDirection_SelectionChangeCommitted(cbStudentsDirection, null);
			else tables[0].DataSource = connector.Load
				(
				queries[0].ToString() +
				(cbStudentsGroup.SelectedIndex == 0 ? "" : $" AND [group]={cbStudentsGroup.SelectedValue}")
				);
			toolStripStatusLabel.Text = $"Количество записей: {tables[0].RowCount - 1}";
		}

		private void cbStudentsDirection_SelectionChangeCommitted(object sender, EventArgs e)
		{
			tables[0].DataSource = connector.Load
				(
				queries[0] + 
				(cbStudentsDirection.SelectedIndex == 0 ? "" : $" AND direction={cbStudentsDirection.SelectedValue}")
				);
			LoadComboBoxFromBase
			(
				cbStudentsGroup, 
				"Groups", 
				(cbStudentsDirection.SelectedIndex == 0 ? "" : $" direction={cbStudentsDirection.SelectedValue}")
			);
			toolStripStatusLabel.Text = $"Количество записей: {tables[0].RowCount - 1}";
		}
	}
}
