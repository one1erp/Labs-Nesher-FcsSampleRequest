using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using MSXML;
using LSSERVICEPROVIDERLib;
using System.Reflection;
using Oracle.DataAccess.Client;
using FCS_OBJECTS;

namespace FcsSampleRequest
{
    class CreateNewSdg
    {
        private INautilusProcessXML _processXml;
        private OracleConnection oraCon;
        private OracleCommand cmd;
        ResponseToDB ResponseToDB;

        private string labName = "Food";
        private string wrkfSdgName;
        private string wrkfSampleName;
        private string operatorname = "הלקוח";
        //private string productId = ".";  //1223//להביא id לפי מוצר שהשם שלו נקודה:(.)
        private string path = string.Empty;
        private ClientObj myClient = new ClientObj("", "", "", "", "", ""/*, ""*/);
        private string U_FCS_MSG_ID;
        private int sdg_id;
        private string resEntity;
        private string docEntity;
        ResultCreateSdj resultCreateSdj = new ResultCreateSdj(0, null);
        private List<AliquotObj> Aliquots = new List<AliquotObj>();
        private string productName = ".";
        private string barcode;

        public ResultCreateSdj RunEvent(ResponseToDB ResponseToDB, INautilusProcessXML _processXml, OracleConnection oraCon, OracleCommand cmd, string barcode, string U_FCS_MSG_ID,
            string resEntity, string docEntity)
        {
            this.barcode = barcode;
            this.U_FCS_MSG_ID = U_FCS_MSG_ID;
            this._processXml = _processXml;
            this.oraCon = oraCon;
            this.cmd = cmd;
            this.ResponseToDB = ResponseToDB;
            this.resEntity = resEntity;
            this.docEntity = docEntity;

            createSdg();

            return resultCreateSdj;
        }

        private void createSdg()
        {
            try
            {
                bool succes = getData();

                if (succes)
                {
                    //creates the wokflow XML
                    var docXml = Create_XML();

                    if (docXml != null)
                    {
                        //creates object for respone
                        var res = new DOMDocument();
                        string resXml = _processXml.ProcessXMLWithResponse(docXml, res);
                        string dt = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");

                        res.save(resEntity + dt + "_" + barcode + ".xml");
                        docXml.save(docEntity + dt +"_"+ barcode + ".xml");

                        if (resXml == null || resXml == "")
                        {
                            var newValue = ((dynamic)res.getElementsByTagName("SDG_ID")[0]).nodeTypedValue;
                            resultCreateSdj.sdgId = int.Parse(newValue.ToString());
                        }
                    }
                }
            }
            catch (Exception EXP)
            {
                MessageBox.Show(EXP.Message);
                resultCreateSdj.message += EXP.Message;
            }
        }

        private bool getData()
        {

            bool result = true;

            result = "NESHER SDG FOOD";// Get_Workflow_Sdg_Name();

            if (result)
            {
                result = "Food Sample Workflow"; Get_Workflow_Sample_Name();

                if (result)
                {
                    result = Get_Client();

                    if (result)
                    {
                        result = Get_Tests();
                        //if (result)
                        //{
                        //    result = Get_Product();
                        //}
                    }
                }
            }

            return result;
        }

        private bool Get_Workflow_Sdg_Name()
        {
            try
            {
                string sql;
                sql = string.Format("SELECT NAME as sdgWorkf FROM lims_sys.WORKFLOW where WORKFLOW_ID =(SELECT U_DEFAULT_SDG_WORKFLOW FROM lims_sys.U_LABS_INFO_USER L where L.U_CLIENT_ADDRESS_CODE = '{0}')", labName);
                cmd = new OracleCommand(sql, oraCon);
                OracleDataReader reader1 = cmd.ExecuteReader();

                if (!reader1.HasRows)
                {
                    MessageBox.Show("sdg workflow לא קיים", "Nautilus", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    resultCreateSdj.message += "Sdg workflow does not exist!";
                    return false;
                }
                else
                    while (reader1.Read()) wrkfSdgName = reader1["sdgWorkf"].ToString();
                return true;
            }
            catch (Exception EXP)
            {
                MessageBox.Show(EXP.Message);
                resultCreateSdj.message += EXP.Message;
                return false;
            }
        }

        private bool Get_Workflow_Sample_Name()
        {
            try
            {
                string sql;
                sql = string.Format("SELECT NAME as sampleWorkf FROM lims_sys.WORKFLOW where WORKFLOW_ID =(SELECT U_DEFAULT_SAMPLE_WF FROM lims_sys.U_LABS_INFO_USER L where L.U_CLIENT_ADDRESS_CODE = '{0}')", labName);
                cmd = new OracleCommand(sql, oraCon);
                OracleDataReader reader2 = cmd.ExecuteReader();

                if (!reader2.HasRows)
                {
                    MessageBox.Show("sample workflow לא קיים", "Nautilus", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    resultCreateSdj.message += "Sample workflow does not exist!";
                    return false;
                }
                else
                    while (reader2.Read()) wrkfSampleName = reader2["sampleWorkf"].ToString();
                return true;
            }
            catch (Exception EXP)
            {
                MessageBox.Show(EXP.Message);
                resultCreateSdj.message += EXP.Message;
                return false;
            }
        }

        private bool Get_Client()
        {
            try
            {
                string sql;
                //aaaaaaaaaaaaaaaaaaaaaaaaaaa
                sql = string.Format("SELECT C.NAME AS U_SDG_CLIENT,AD.PHONE AS U_PHONE,AD.EMAIL AS U_EMAIL,AD.ADDRESS_LINE_2 AS U_ADDRESS,AD.ADDRESS_LINE_1 AS U_CONTECT_NAME,AD.ADDRESS_LINE_4 AS U_CONTACT_PHONE FROM lims_sys.ADDRESS AD INNER JOIN lims_sys.CLIENT C ON AD.ADDRESS_ITEM_ID = C.CLIENT_ID WHERE C.CLIENT_CODE = '{0}' AND ADDRESS_TYPE = '{1}'", ResponseToDB.U_Payer_ID.ToString(), labName);
                cmd = new OracleCommand(sql, oraCon);
                OracleDataReader reader3 = cmd.ExecuteReader();

                if (!reader3.HasRows)
                {
                    MessageBox.Show("לקוח לא קיים, אנא פנה למנהל מערכת", "Nautilus", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    resultCreateSdj.message += "The client does not exist!";
                    return false;
                }
                else
                {
                    while (reader3.Read())
                    {
                        ClientObj newClient = new ClientObj(
                            reader3["U_SDG_CLIENT"].ToString(),
                            reader3["U_PHONE"].ToString(),
                            reader3["U_EMAIL"].ToString(),
                            reader3["U_ADDRESS"].ToString(),
                            reader3["U_CONTECT_NAME"].ToString(),
                            reader3["U_CONTACT_PHONE"].ToString()
                            /*,operatorname*/
                        );
                        myClient = newClient;
                    }
                }
                return true;
            }
            catch (Exception EXP)
            {
                MessageBox.Show(EXP.Message);
                resultCreateSdj.message += EXP.Message;
                return false;
            }
        }

        private bool Get_Tests()
        {
            try
            {
                string sql;
                List<string> listLabTtex = new List<string>();
                sql = string.Format("select FTU.u_lab_ttex FROM lims_sys.U_FCS_TEST FT INNER JOIN lims_sys.U_FCS_TEST_USER FTU ON FTU.U_FCS_TEST_ID=FT.U_FCS_TEST_ID WHERE FT.NAME = '{0}'", ResponseToDB.U_Test_Sub_Code);
                cmd = new OracleCommand(sql, oraCon);
                OracleDataReader reader4 = cmd.ExecuteReader();

                if (!reader4.HasRows)
                {
                    MessageBox.Show("לא נמצאה בדיקה, אנא פנה למנהל מערכת", "Nautilus", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    resultCreateSdj.message += "TEST_TEMPLATE_EX does not exist!";
                    return false;
                }
                else
                {
                    string[] arrLabTtex;

                    while (reader4.Read())
                    {
                        string labTtex = reader4["u_lab_ttex"].ToString();
                        if (labTtex != null && labTtex != "")
                            if (labTtex.Contains(","))
                            {
                                arrLabTtex = labTtex.Split(',');
                                foreach (string item in arrLabTtex)
                                {
                                    listLabTtex.Add(item);
                                }
                            }
                            else
                            {
                                listLabTtex.Add(labTtex);
                            }
                        else
                        {
                            MessageBox.Show("מזהה בדיקה לא קיים, אנא פנה למנהל מערכת", "Nautilus", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            resultCreateSdj.message += "TEST_TEMPLATE_EX does not exist!";
                            return false;
                        }
                    }
                    //aaaaaaaaaaaaaaaaaaaaaaaaaaa
                    sql = string.Format("SELECT W.NAME ALIQWFNAME,TTEX.DESCRIPTION,TTEX.NAME TTEXNAME " +
                        "FROM lims_sys.U_TEST_TEMPLATE_EX_USER TTEXU " +
                        "INNER JOIN lims_sys.U_TEST_TEMPLATE_EX TTEX ON TTEX.U_TEST_TEMPLATE_EX_ID= TTEXU.U_TEST_TEMPLATE_EX_ID " +
                        "INNER JOIN lims_sys.WORKFLOW  W ON TTEXU.U_ALIQ_WORKFLOW=W.WORKFLOW_ID " +
                        "where TTEXU.U_TEST_TEMPLATE_EX_ID in ("
               + string.Join(",", listLabTtex)
               + ")");

                    cmd = new OracleCommand(sql, oraCon);
                    OracleDataReader reader5 = cmd.ExecuteReader();

                    if (!reader5.HasRows)
                    {
                        MessageBox.Show("מזהה בדיקה לא קיים, אנא פנה למנהל מערכת", "Nautilus", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        resultCreateSdj.message += "TEST_TEMPLATE_EX does not exist!";
                        return false;
                    }
                    else
                        while (reader5.Read())
                        {
                            AliquotObj newAliquot = new AliquotObj(
                                reader5["ALIQWFNAME"].ToString(),
                                reader5["DESCRIPTION"].ToString(),
                                reader5["TTEXNAME"].ToString()
                                );

                            Aliquots.Add(newAliquot);
                        }

                }
                return true;
            }
            catch (Exception EXP)
            {
                MessageBox.Show(EXP.Message);
                resultCreateSdj.message += EXP.Message;
                return false;
            }
        }

        private bool Get_Product()
        {
            try
            {
                string sql;
                sql = string.Format("SELECT p.name FROM u_fcs_product FP ,u_fcs_product_user FPU ,PRODUCT p " +
                    "WHERE fp.u_fcs_product_id=fpu.u_fcs_product_id AND fpu.u_lab_product = P.PRODUCT_ID and fp.NAME = '{0}'", U_FCS_MSG_ID);
                cmd = new OracleCommand(sql, oraCon);
                OracleDataReader reader3 = cmd.ExecuteReader();

                if (!reader3.HasRows)
                {
                    MessageBox.Show("מוצר לא קיים, אנא פנה למנהל מערכת", "Nautilus", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    resultCreateSdj.message += "The client does not exist!";
                    return false;
                }
                else
                {
                    while (reader3.Read())
                    {
                        productName = reader3["name"].ToString();
                    }
                }
                return true;
            }
            catch (Exception EXP)
            {
                MessageBox.Show(EXP.Message);
                resultCreateSdj.message += EXP.Message;
                return false;
            }
        }

        private DOMDocument Create_XML()
        {
            try
            {

                //aaaaaaaaaaaaaaaaaaaaaaaaaaa

                DOMDocument objDom = new DOMDocument();

                //Creates lims request element
                var objLimsElem = objDom.createElement("lims-request");
                objDom.appendChild(objLimsElem);

                // Creates login request element
                var objLoginElem = objDom.createElement("login-request");
                objLimsElem.appendChild(objLoginElem);

                // Creates Entity element
                var objSdg = objDom.createElement("SDG");
                objLoginElem.appendChild(objSdg);

                // Creates   create-by-workflow element
                var objCreateByWorkflowElem = objDom.createElement("create-by-workflow");
                objSdg.appendChild(objCreateByWorkflowElem);

                var objWorkflowName = objDom.createElement("workflow-name");
                objCreateByWorkflowElem.appendChild(objWorkflowName);
                objWorkflowName.text = wrkfSdgName;

                var description = objDom.createElement("DESCRIPTION");
                objSdg.appendChild(description);
                description.text = "New Sdg";

                //aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
                var fcsMessageId = objDom.createElement("U_FCS_MSG_ID");
                objSdg.appendChild(fcsMessageId);
                fcsMessageId.text = barcode;

                var sampledBy = objDom.createElement("U_SAMPLED_BY");
                objSdg.appendChild(sampledBy);
                sampledBy.text = operatorname;

                var sdgExternalReference = objDom.createElement("EXTERNAL_REFERENCE");
                objSdg.appendChild(sdgExternalReference);
                sdgExternalReference.text = ResponseToDB.U_Sample_Form_Num.ToString() + " " + ResponseToDB.U_Del_File_Num;

                foreach (PropertyInfo prop in myClient.GetType().GetProperties())
                {
                    var fld = objDom.createElement(prop.Name);
                    objSdg.appendChild(fld);
                    PropertyInfo property = myClient.GetType().GetProperty(prop.Name);
                    string val = property.GetValue(myClient, null).ToString();
                    fld.text = val;
                }

                // Creates sample
                var objSample = objDom.createElement("SAMPLE");
                objSdg.appendChild(objSample);

                objCreateByWorkflowElem = objDom.createElement("create-by-workflow");
                objSample.appendChild(objCreateByWorkflowElem);

                objWorkflowName = objDom.createElement("workflow-name");
                objCreateByWorkflowElem.appendChild(objWorkflowName);
                objWorkflowName.text = wrkfSampleName;

                description = objDom.createElement("DESCRIPTION");
                objSample.appendChild(description);
                description.text = ResponseToDB.U_Product_Brand_Name;

                var product = objDom.createElement("PRODUCT_ID");
                objSample.appendChild(product);
                product.text = productName;

                var dateProduction = objDom.createElement("U_DATE_PRODUCTION");
                objSample.appendChild(dateProduction);
                dateProduction.text = ResponseToDB.U_Manufacture_Date;

                var containerNumber = objDom.createElement("U_CONTAINER_NUMBER");
                objSample.appendChild(containerNumber);
                containerNumber.text = ResponseToDB.U_Container_Num;

                var batchNum = objDom.createElement("U_BATCH");
                objSample.appendChild(batchNum);
                batchNum.text = ResponseToDB.U_Batch_Num;

                var samplingTime = objDom.createElement("U_TXT_SAMPLING_TIME");
                objSample.appendChild(samplingTime);
                samplingTime.text = ResponseToDB.U_Sampling_Time;

                var delFileNum = objDom.createElement("Del_File_Num");
                objSample.appendChild(delFileNum);
                delFileNum.text = ResponseToDB.U_Del_File_Num.ToString();

                var smplExternalReference = objDom.createElement("EXTERNAL_REFERENCE");
                objSample.appendChild(smplExternalReference);
                smplExternalReference.text = ResponseToDB.U_Sample_Form_Num.ToString() + " " + ResponseToDB.U_Del_File_Num;


                foreach (AliquotObj aliq in Aliquots)
                {
                    var objAliquot = objDom.createElement("ALIQUOT");
                    objSample.appendChild(objAliquot);

                    objCreateByWorkflowElem = objDom.createElement("create-by-workflow");
                    objAliquot.appendChild(objCreateByWorkflowElem);

                    objWorkflowName = objDom.createElement("workflow-name");
                    objCreateByWorkflowElem.appendChild(objWorkflowName);
                    objWorkflowName.text = aliq.aliquotWorkf;

                    //description = objDom.createElement("DESCRIPTION");
                    //objAliquot.appendChild(description);
                    //description.text = aliq.DESCRIPTION;
                    var i = false;
                    foreach (PropertyInfo prop in aliq.GetType().GetProperties())
                    {
                        if (i)
                        {
                            var fld = objDom.createElement(prop.Name);
                            objAliquot.appendChild(fld);
                            PropertyInfo property = aliq.GetType().GetProperty(prop.Name);
                            string val = property.GetValue(aliq, null).ToString();
                            fld.text = val;
                        }
                        else
                        {
                            i = true;
                        }
                    }

                }
                return objDom;
            }
            catch (Exception EXP)
            {
                MessageBox.Show(EXP.Message);
                resultCreateSdj.message += EXP.Message;
                return null;
            }
        }
    }
}
