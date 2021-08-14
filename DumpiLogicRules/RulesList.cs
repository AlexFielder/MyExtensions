using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DumpiLogicRules
{
    public partial class RulesList : Form
    {
        public RulesList()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (ListBox1.SelectedItems.Count == 1)
            {
                ProcessFiles.selectedRule = ListBox1.SelectedItem.ToString();
                Close();
            }

        }

        private void RulesList_Load(object sender, EventArgs e)
        {
            StatusStrip1.Text = String.Empty;
            System.Reflection.Assembly assemblyNet = System.Reflection.Assembly.GetExecutingAssembly();
            StatusStrip1.Text = "Version: " + assemblyNet.GetName().Version;
        }
    }
}
