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
	public partial class HumanForm : Form
	{
		internal Models.Human human;
		public HumanForm()
		{
			InitializeComponent();
		}
		protected virtual void Compress()
		{
			// Упаковывает пользовательские данные из формы в объект класса 'Human':
			human = new Models.Human
				(
				Convert.ToInt32(labelID.Text == "" ? "0" : labelID.Text.Split(':').Last()),
				tbLastName.Text,
				tbFirstName.Text,
				tbMiddleName.Text,
				dtpBirthDate.Value.ToString("yyyy-MM-dd"),
				tbEmail.Text,
				tbPhone.Text,
				pictureBoxPhoto.Image
				);
		}

		protected virtual void btnOK_Click(object sender, EventArgs e)
		{
			Compress();
		}
	}
}
