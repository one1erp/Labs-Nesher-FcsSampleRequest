using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LSExtensionControlLib;
using LSExtensionWindowLib;
using LSSERVICEPROVIDERLib;
using FcsSampleRequest;
//using InsertData;
//using Oracle.DataAccess.Client;

namespace TestRun
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            activateWorkListWindow();
        }


        private void activateWorkListWindow()
        {
            try
            {

                FcsSampleRequest.FcsSampleRequestCtrl Form1 = new FcsSampleRequest.FcsSampleRequestCtrl(null, null);
                Form1.DEBUG = true;
                Form1.PreDisplay();
                this.Controls.Add(Form1);

                //InsertData
                //InsertData.Form1 Form1 = new InsertData.Form1();
                //Form1.DEBUG = true;
                //Form1.PreDisplay();
                //this.Controls.Add(Form1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
