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
    public partial class frmReturn : ChildForm
    {
        public frmReturn(bool IsFull)
        {
            InitializeComponent();
            ucReturnProcess1.IS_FULL = IsFull;
            this.Text = IsFull ? "可再銷售退貨" : "不可再銷售退貨";
        }
        
    }
}
