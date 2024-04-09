using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcsSampleRequest
{

    public class Amil_Details
    {

        public string Address { get; set; }

        public string City { get; set; }

        public string Company_ID { get; set; }

        public string Company_Name { get; set; }

        public string Email { get; set; }

        public string Fax { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        public string ZIP_Code { get; set; }
    }

    public class Expiry_Date
    {

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public string _date { get; set; }

    }

    public class Importer_Details
    {

        public string Address { get; set; }

        public string City { get; set; }

        public string Company_ID { get; set; }

        public string Company_Name { get; set; }

        public string Email { get; set; }

        public string Fax { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        public string ZIP_Code { get; set; }
    }

    public class Manufacture_Date
    {

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public string _date { get; set; }
    }

    public class Sampling_Date 
    {

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public string _date { get; set; }
    }
    //class DateBlalal
    //{
        
    //}

    public class Response
    {

        public Amil_Details Amil_Details = new Amil_Details();

        public int barcode { get; set; }
        public string Barcode
        {
            get { return ""; }  //get { return barcode.HasValue ? barcode.ToString() : string.Empty; }
            set { if (!string.IsNullOrEmpty(value)) barcode = Int32.Parse(value); }
        }

        public string Batch_Num { get; set; }

        public string Container_Num { get; set; }

        public string Country_Name { get; set; }

        public int del_File_Num { get; set; }
        public string Del_File_Num
        {
            get { return ""; }
            set { if (!string.IsNullOrEmpty(value)) del_File_Num = Int32.Parse(value); }
        }

        public string Delivery_To_Lab { get; set; }

        public Expiry_Date Expiry_Date = new Expiry_Date();

        public Importer_Details Importer_Details = new Importer_Details();

        public string Importer_Store { get; set; }

        public string Inspector_Title { get; set; }

        public string Is_Vet { get; set; }

        public Manufacture_Date Manufacture_Date = new Manufacture_Date();

        public string Num_Of_Samples { get; set; }

        public int num_Of_Samples_Vet { get; set; }
        public string Num_Of_Samples_Vet
        {
            get { return ""; }
            set { if (!string.IsNullOrEmpty(value)) num_Of_Samples_Vet = Int32.Parse(value); }
        }

        public string Organization { get; set; }

        public string Packing_Type { get; set; }

        public int payer_ID { get; set; }
        public string Payer_ID
        {
            get { return ""; }
            set { if (!string.IsNullOrEmpty(value)) payer_ID = Int32.Parse(value); }
        }

        public int producer_Country { get; set; }
        public string Producer_Country
        {
            get { return ""; }
            set { if (!string.IsNullOrEmpty(value)) producer_Country = Int32.Parse(value); }
        }

        public string Producer_Name { get; set; }

        public string Product_Brand_Name { get; set; }

        public int product_Group_Code { get; set; }
        public string Product_Group_Code
        {
            get { return ""; }
            set { if (!string.IsNullOrEmpty(value)) product_Group_Code = Int32.Parse(value); }
        }

        public string Product_Group_Description { get; set; }

        public string Product_name_eng { get; set; }

        public string Product_name_heb { get; set; }

        public string Property_Plus { get; set; }

        public string Remark { get; set; }

        public int return_Code { get; set; }
        public string Return_Code
        {
            get { return ""; }
            set { if (!string.IsNullOrEmpty(value)) return_Code = Int32.Parse(value); }
        }

        public string Return_Code_Desc { get; set; }

        public int sample_Form_Num { get; set; }
        public string Sample_Form_Num
        {
            get { return ""; }
            set { if (!string.IsNullOrEmpty(value)) sample_Form_Num = Int32.Parse(value); }
        }

        public Sampling_Date Sampling_Date = new Sampling_Date();

        public string Sampling_Inspector { get; set; }

        public string Sampling_Place { get; set; }

        public string Sampling_Reason { get; set; }

        public double sampling_Temp { get; set; }
        public string Sampling_Temp
        {
            get { return ""; }
            set { if (!string.IsNullOrEmpty(value)) sampling_Temp = double.Parse(value); }
        }

        public string Sampling_Time { get; set; }

        public string Test_Description { get; set; }

        public int test_Sub_Code { get; set; }
        public string Test_Sub_Code
        {
            get { return ""; }
            set { if (!string.IsNullOrEmpty(value)) test_Sub_Code = Int32.Parse(value); }
        }

    }
}
