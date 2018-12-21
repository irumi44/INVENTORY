using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUSHA.form
{
    public partial class ChildForm : Form, GUSHA.Interface.IChildForm
    {
        public ChildForm()
        {
            InitializeComponent();
        }
        
        private void ChildForm_FormClosing(object sender, System.EventArgs e)
        {
            cls.clsCache.SetClose(this);
        }
    }
}
