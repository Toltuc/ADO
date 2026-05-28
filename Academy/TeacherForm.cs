using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academy
{
	public partial class TeacherForm : HumanForm
	{
		public TeacherForm()
		{
			InitializeComponent();
		}
		protected override void btnOK_Click(object sender, EventArgs e)
		{
			base.btnOK_Click(sender, e);
			DataBase.Connector.Insert($"INSERT Teachers({human.GetNames()}) VALUES({human.GetValues()})");
		}
	}
}
