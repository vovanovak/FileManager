using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileManager
{
    public enum TypeOfDialog
    {
        Add, Rename
    }
    public partial class NewFileOrFolder : Form
    {
        public string nameOfNewFileOrFolder { get; set; }
        public NewFileOrFolder(TypeOfDialog dlg)
        {
            InitializeComponent();
            if (dlg == TypeOfDialog.Rename)
            {
                Text = "FileManager";
                label1.Text = "Rename file or folder: ";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            nameOfNewFileOrFolder = textBox1.Text;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

    
    }
}
