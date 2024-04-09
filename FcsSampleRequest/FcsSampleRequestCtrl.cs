using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MSXML;
using LSExtensionWindowLib;
using LSSERVICEPROVIDERLib;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using Oracle.DataAccess.Client;
using DAL;
using Common;

using FCS_OBJECTS;
//using MisradHabriutService;

namespace FcsSampleRequest
{
    [ComVisible(true)]
    [ProgId("FcsSampleRequest.FcsSampleRequestCtrl")]
    public partial class FcsSampleRequestCtrl : UserControl
    {

        #region variables

        public IExtensionWindowSite2 _ntlsSite;
        private IDataLayer _dal;
        public INautilusServiceProvider _sp;
        private INautilusProcessXML _processXml;
        private NautilusUser _ntlsUser;
        private INautilusDBConnection _ntlsCon;

        public string txtMsg = "";


        public OracleConnection oraCon;
        private OracleCommand cmd;
       

        public event Action<string> SdgCreated;
        public bool DEBUG;
        public string barcode;
        public string U_FCS_MSG_ID;
        public int sdgId;
        public string sdgName;
        public bool _return;
        public string statusMsg;
        private List<PhraseEntry> phrases = null;

        //   private string[] classes = new string[] { "Attached_Document", "ATTACHED_DOCUMENT", "Report_Notes", "Test_Results", "Test_Result", "Report_Notes", "Lab_Notes" };
        ResponseToDB ResponseDB = new ResponseToDB();

        #endregion


        public FcsSampleRequestCtrl(INautilusServiceProvider _sp, IExtensionWindowSite2 _ntlsSite, IDataLayer dal)
        {
            this._sp = _sp;
            this._ntlsSite = _ntlsSite;
            _dal = dal;
            InitializeComponent();
       
      
        
        }


        public void Init()
        {

            if (!DEBUG)
            {
                _ntlsCon = _sp.QueryServiceProvider("DBConnection") as NautilusDBConnection;
                _processXml = Common.Utils.GetXmlProcessor(_sp);
                oraCon = GetConnection(_ntlsCon);
            }
            else
            {
                _processXml = null;
                oraCon = new OracleConnection("Data Source=MICROB;user id=lims_sys;password=lims_sys;");
                if (oraCon.State != ConnectionState.Open)
                {
                    oraCon.Open();
                }
                _dal = new MockDataLayer();
                _dal.Connect();
                phrases = _dal.GetPhraseByName("FCS Parameters").PhraseEntries.ToList();

            }

        }

        public void textBoxBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendRequest();
            }
        }

        private void SendRequest()
        {
            string sql;

            try
            {

                if (string.IsNullOrEmpty(textBoxBarcode.Text)) return;

                _return = false;

                barcode = textBoxBarcode.Text;

                lblMsg.Text = "נא להמתין...";
                txtMsg = "";
                SendToMSB SendToMSB = new SendToMSB();

                
                string url = _dal.GetPhraseByName("UrlService_FCS").PhraseEntries.Where(p => p.PhraseName == "FcsSampleRequest").FirstOrDefault().PhraseDescription;

                string fullURL = url + barcode;


                ResponseReq responseReq = SendToMSB.SendRequest(fullURL);
                if (responseReq.Msg.Contains(";"))
                {
                    string[] resMsg = responseReq.Msg.Split(';');
                    U_FCS_MSG_ID = resMsg[1];

                    if (responseReq.success == true && U_FCS_MSG_ID != "")
                    {

                        statusMsg = FCS_MSG_STATUS.New;

                        //יצירת דרישה
                        CreateSdg();

                        //MessageBox.Show(resMsg[0] + "התהליך בוצע בהצלחה");
                        txtMsg += resMsg[0] + "התהליך בוצע בהצלחה";
                    }
                    else
                    {

                        //MessageBox.Show(resMsg[0] + "התהליך נכשל - ");
                        txtMsg += resMsg[0] + " - התהליך נכשל - ";

                    }
                }
                else
                {
                    txtMsg += " - התהליך נכשל - ";
                }

            }

            catch (Exception EXP)
            {
                
                MessageBox.Show(EXP.Message + "התהליך נכשל - ");
                txtMsg += EXP.Message + " - התהליך נכשל - ";
                Logger.WriteLogFile(EXP.Message);
                sql = string.Format("UPDATE lims_sys.U_FCS_MSG_USER SET U_ERROR = U_ERROR || '{0}' WHERE U_FCS_MSG_ID = '{1}'", EXP.Message, U_FCS_MSG_ID);
                RunSql(sql);
                _return = true;
            }
            lblMsg.Text = txtMsg;
        }

        public string GetPhraseEntry(string phraseName)
        {
            try
            {
                return phrases.Find(x => x.PhraseName == phraseName).PhraseDescription;
            }
            catch( Exception e)
            {
                string retStr = "";
                if (phraseName== "resEntity")
                {
                    retStr = "C:\\temp\\resENTITY";
                }
                else
                {
                    retStr = "C:\\temp\\docENTITY";

                }
                return retStr;
            }
        }

        private void RunSql(string sql)
        {
            cmd = new OracleCommand(sql, oraCon);
            cmd.ExecuteNonQuery();
            Logger.WriteLogFile(sql);

        }


        private void SelectDataFcsMsg()
        {
            string sql;
            try
            {
                sql = string.Format("SELECT * FROM lims_sys.U_FCS_MSG_USER WHERE U_FCS_MSG_ID = '{0}'", U_FCS_MSG_ID);
                cmd = new OracleCommand(sql, oraCon);
                OracleDataReader reader3 = cmd.ExecuteReader();

                if (!reader3.HasRows)
                {
                    Logger.WriteLogFile("The fcsMsg does not exist!");
                    MessageBox.Show("The fcsMsg does not exist!", "Nautilus", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    sql = string.Format("UPDATE lims_sys.U_FCS_MSG_USER SET U_ERROR = U_ERROR || '{0}' WHERE U_FCS_MSG_ID = '{1}'", "The fcsMsg does not exist!", U_FCS_MSG_ID);
                    RunSql(sql);
                    _return = true;
                }
                else
                    while (reader3.Read())
                    {
                        ResponseDB = new ResponseToDB(
                        int.Parse(reader3["U_Return_Code"].ToString()),
                        reader3["U_Return_Code_Desc"].ToString(),
                        int.Parse(reader3["U_Barcode"].ToString()),
                        int.Parse(reader3["U_Sample_Form_Num"].ToString()),
                        reader3["U_Is_Vet"].ToString(),
                        reader3["U_Product_Group_Desc"].ToString(),
                        int.Parse(reader3["U_Product_Group_Code"].ToString()),
                        reader3["U_Product_name_heb"].ToString(),
                        reader3["U_Product_name_eng"].ToString(),
                        reader3["U_Product_Brand_Name"].ToString(),
                        reader3["U_Organization"].ToString(),
                        int.Parse(reader3["U_Payer_ID"].ToString()),
                        reader3["U_Producer_Name"].ToString(),
                        int.Parse(reader3["U_Producer_Country"].ToString()),
                        reader3["U_Country_Name"].ToString(),
                        reader3["U_Manufacture_Date"].ToString(),
                        reader3["U_Sampling_Time"].ToString(),
                        double.Parse(reader3["U_Sampling_Temp"].ToString()),
                        reader3["U_Expiry_Date"].ToString(),
                        reader3["U_Batch_Num"].ToString(),
                        reader3["U_Property_Plus"].ToString(),
                        reader3["U_Sampling_Place"].ToString(),
                        reader3["U_Sampling_Reason"].ToString(),
                        reader3["U_Packing_Type"].ToString(),
                        reader3["U_Delivery_To_Lab"].ToString(),
                        reader3["U_Sampling_Inspector"].ToString(),
                        reader3["U_Inspector_Title"].ToString(),
                        reader3["U_Container_Num"].ToString(),
                        reader3["U_Num_Of_Samples"].ToString(),
                        int.Parse(reader3["U_Num_Of_Samples_Vet"].ToString()),
                        int.Parse(reader3["U_Del_File_Num"].ToString()),
                        reader3["U_Sampling_Date"].ToString(),
                        reader3["U_Importer_Store"].ToString(),
                        reader3["U_Remark"].ToString(),
                        int.Parse(reader3["U_Test_Sub_Code"].ToString()),
                        reader3["U_Test_Description"].ToString()
                        );
                    }
            }
            catch (Exception EXP)
            {
                MessageBox.Show(EXP.Message);
                Logger.WriteLogFile(EXP.Message);
                sql = string.Format("UPDATE lims_sys.U_FCS_MSG_USER SET U_ERROR = U_ERROR || '{0}' WHERE U_FCS_MSG_ID = '{1}'", EXP.Message, U_FCS_MSG_ID);
                RunSql(sql);
                _return = true;
            }
        }

        private void CreateSdg()
        {
            string sql;
            try
            {
                if (_return == false && (statusMsg == FCS_MSG_STATUS.New || statusMsg == FCS_MSG_STATUS.Create_SDG_Failed))
                {
                    //if (statusMsg == FCS_MSG_STATUS.Create_SDG_Failed)
                    SelectDataFcsMsg();

                    var cw = new CreateNewSdg();
                    ResultCreateSdj resultCreateSdj = cw.RunEvent(ResponseDB, _processXml, oraCon, cmd, barcode, U_FCS_MSG_ID, 
                        GetPhraseEntry("resEntity"), GetPhraseEntry("docEntity"));//יצירת דרישה (sdg)
                    sdgId = resultCreateSdj.sdgId;
                    if (sdgId != 0)
                    {
                        sql = string.Format("UPDATE lims_sys.U_FCS_MSG_USER SET U_STATUS = '{0}' WHERE U_FCS_MSG_ID = '{1}'", FCS_MSG_STATUS.Sdg_Created, U_FCS_MSG_ID);//עדכון סטטוס - נוצר sdg
                        RunSql(sql);

                        sql = string.Format("UPDATE lims_sys.SDG_USER SET U_FCS_MSG_ID = '{0}' WHERE SDG_ID = '{1}'", U_FCS_MSG_ID, sdgId);
                        RunSql(sql);


                        sql = string.Format("SELECT NAME FROM lims_sys.SDG WHERE SDG_ID = '{0}'", sdgId);
                        cmd = new OracleCommand(sql, oraCon);
                        OracleDataReader reader1 = cmd.ExecuteReader();

                        if (!reader1.HasRows)
                        {
                            txtMsg = txtMsg.Trim() == "" ? "" : txtMsg + "<br/>";
                            txtMsg += resultCreateSdj.message;
                            
                            MessageBox.Show("The sdg created does not exist!");
                            Logger.WriteLogFile("The sdg created does not exist!");
                            sql = string.Format("UPDATE lims_sys.U_FCS_MSG_USER SET U_ERROR = U_ERROR || 'The sdg created does not exist!' WHERE U_FCS_MSG_ID = '{0}'", U_FCS_MSG_ID);
                            RunSql(sql);
                            _return = true;
                        }
                        else
                        {
                            if (reader1.Read())
                            {
                                sdgName = reader1["NAME"].ToString();
                                
                                txtMsg += "- הדרישה נוצרה בהצלחה -";
                                //MessageBox.Show("הדרישה נוצרה בהצלחה");
                                SdgCreated(sdgName);
                            }
                        }
                    }
                    else
                    {
                        
                        txtMsg += "- לא נוצרה דרישה -";
                        //MessageBox.Show("לא נוצרה דרישה");
                        Logger.WriteLogFile("The sdg created does not exist!");
                        _return = true;
                        sql = string.Format("UPDATE lims_sys.U_FCS_MSG_USER SET U_STATUS = '{0}',U_ERROR = U_ERROR || '{1}' WHERE U_FCS_MSG_ID = '{2}'", FCS_MSG_STATUS.Create_SDG_Failed, resultCreateSdj.message, U_FCS_MSG_ID);//עדכון סטטוס - נכשל ביצירת sdg
                        RunSql(sql);
                    }
                }
            }
            catch (Exception EXP)
            {
                txtMsg = txtMsg.Trim() == "" ? "" : txtMsg + "<br/>";
                txtMsg += EXP.Message;
                MessageBox.Show(EXP.Message);
                Logger.WriteLogFile(EXP.Message);
                sql = string.Format("UPDATE lims_sys.U_FCS_MSG_USER SET U_ERROR = U_ERROR || '{0}' WHERE U_FCS_MSG_ID = '{1}'", EXP.Message, U_FCS_MSG_ID);
                RunSql(sql);
                _return = true;
            }
        }



        private void FcsSampleRequestCtrl_Resize(object sender, EventArgs e)
        {
            if (Parent == null) return;

            panel1.Location = new Point(Parent.Width / 2 - Parent.Width / 2, Parent.Location.Y);
        }

        private void btnok_Click(object sender, EventArgs e)
        {
            SendRequest();
        }
        public OracleConnection GetConnection(INautilusDBConnection ntlsCon)
        {

            OracleConnection connection = null;

            if (ntlsCon != null)
            {


                // Initialize variables
                String roleCommand;
                // Try/Catch block
                try
                {



                    var _connectionString = ntlsCon.GetADOConnectionString();

                    var splited = _connectionString.Split(';');

                    var cs = "";

                    for (int i = 1; i < splited.Count(); i++)
                    {
                        cs += splited[i] + ';';
                    }

                    var username = ntlsCon.GetUsername();
                    if (string.IsNullOrEmpty(username))
                    {
                        var serverDetails = ntlsCon.GetServerDetails();
                        cs = "User Id=/;Data Source=" + serverDetails + ";";
                    }


                    //Create the connection
                    connection = new OracleConnection(cs);

                    // Open the connection
                    connection.Open();

                    // Get lims user password
                    string limsUserPassword = ntlsCon.GetLimsUserPwd();

                    // Set role lims user
                    if (limsUserPassword == "")
                    {
                        // LIMS_USER is not password protected
                        roleCommand = "set role lims_user";
                    }
                    else
                    {
                        // LIMS_USER is password protected.
                        roleCommand = "set role lims_user identified by " + limsUserPassword;
                    }

                    // set the Oracle user for this connecition
                    cmd = new OracleCommand(roleCommand, connection);

                    // Try/Catch block
                    try
                    {
                        // Execute the command
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception f)
                    {
                        // Throw the exception
                        throw new Exception("Inconsistent role Security : " + f.Message);
                    }

                    // Get the session id
                    var sessionId = _ntlsCon.GetSessionId();

                    // Connect to the same session
                    string sSql = string.Format("call lims.lims_env.connect_same_session({0})", sessionId);

                    // Build the command
                    cmd = new OracleCommand(sSql, connection);

                    // Execute the command
                    cmd.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    // Throw the exception
                    throw e;
                }

                // Return the connection
            }

            return connection;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnExt_Click(object sender, EventArgs e)
        {

            ((Form)this.TopLevelControl).Close();
        }
    }
}
