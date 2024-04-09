using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcsSampleRequest
{
    public class ResponseToDB
    {
        public int U_Barcode { get; set; }

        public string U_Batch_Num { get; set; }

        public string U_Container_Num { get; set; }

        public string U_Country_Name { get; set; }

        public int U_Del_File_Num { get; set; }

        public string U_Delivery_To_Lab { get; set; }

        public string U_Expiry_Date { get; set; }

        public string U_Importer_Store { get; set; }

        public string U_Inspector_Title { get; set; }

        public string U_Is_Vet { get; set; }

        public string U_Manufacture_Date { get; set; }

        public string U_Num_Of_Samples { get; set; }

        public int U_Num_Of_Samples_Vet { get; set; }

        public string U_Organization { get; set; }

        public string U_Packing_Type { get; set; }

        public int U_Payer_ID { get; set; }

        public int U_Producer_Country { get; set; }

        public string U_Producer_Name { get; set; }

        public string U_Product_Brand_Name { get; set; }

        public int U_Product_Group_Code { get; set; }

        public string U_Product_Group_Desc { get; set; }

        public string U_Product_name_eng { get; set; }

        public string U_Product_name_heb { get; set; }

        public string U_Property_Plus { get; set; }

        public string U_Remark { get; set; }

        public int U_Return_Code { get; set; }

        public string U_Return_Code_Desc { get; set; }

        public int U_Sample_Form_Num { get; set; }

        public string U_Sampling_Date { get; set; }

        public string U_Sampling_Inspector { get; set; }

        public string U_Sampling_Place { get; set; }

        public string U_Sampling_Reason { get; set; }

        public double U_Sampling_Temp { get; set; }

        public string U_Sampling_Time { get; set; }

        public string U_Test_Description { get; set; }

        public int U_Test_Sub_Code { get; set; }

        public ResponseToDB()
        { }


        public ResponseToDB(
                int Return_Code,
                string Return_Code_Desc,
                int Barcode,
                int Sample_Form_Num,
                string Is_Vet,
                string Product_Group_Description,
                int Product_Group_Code,
                string Product_name_heb,
                string Product_name_eng,
                string Product_Brand_Name,
                string Organization,
                int Payer_ID,
                string Producer_Name,
                int Producer_Country,
                string Country_Name,
                string Manufacture_Date,
                string Sampling_Time,
                double Sampling_Temp,
                string Expiry_Date,
                string Batch_Num,
                string Property_Plus,
                string Sampling_Place,
                string Sampling_Reason,
                string Packing_Type,
                string Delivery_To_Lab,
                string Sampling_Inspector,
                string Inspector_Title,
                string Container_Num,
                string Num_Of_Samples,
                int Num_Of_Samples_Vet,
                int Del_File_Num,
                string Sampling_Date,
                string Importer_Store,
                string Remark,
                int Test_Sub_Code,
                string Test_Description
        )
        {
            //Importer_Details 
            //Amil_Details
            this.U_Return_Code = Return_Code;
            this.U_Return_Code_Desc = Return_Code_Desc;
            this.U_Barcode = Barcode;
            this.U_Sample_Form_Num = Sample_Form_Num;
            if (Is_Vet =="true")
            {
                this.U_Is_Vet = "T";
            }
            else if(Is_Vet == "false")
            {
                this.U_Is_Vet = "F";
            }
            this.U_Product_Group_Desc = Product_Group_Description;
            this.U_Product_Group_Code = Product_Group_Code;
            this.U_Product_name_heb = Product_name_heb;
            this.U_Product_name_eng = Product_name_eng;
            this.U_Product_Brand_Name = Product_Brand_Name;
            this.U_Organization = Organization;
            this.U_Payer_ID = Payer_ID;
            this.U_Producer_Name = Producer_Name;
            this.U_Producer_Country = Producer_Country;
            this.U_Country_Name = Country_Name;
            this.U_Manufacture_Date = Manufacture_Date;
            this.U_Sampling_Time = Sampling_Time;
            this.U_Sampling_Temp = Sampling_Temp;
            this.U_Expiry_Date = Expiry_Date;
            this.U_Batch_Num = Batch_Num;
            this.U_Property_Plus = Property_Plus;
            this.U_Sampling_Place = Sampling_Place;
            this.U_Sampling_Reason = Sampling_Reason;
            this.U_Packing_Type = Packing_Type;
            this.U_Delivery_To_Lab = Delivery_To_Lab;
            this.U_Sampling_Inspector = Sampling_Inspector;
            this.U_Inspector_Title = Inspector_Title;
            this.U_Container_Num = Container_Num;
            this.U_Num_Of_Samples = Num_Of_Samples;
            this.U_Num_Of_Samples_Vet = Num_Of_Samples_Vet;
            this.U_Del_File_Num = Del_File_Num;
            this.U_Sampling_Date = Sampling_Date;
            this.U_Importer_Store = Importer_Store;
            this.U_Remark = Remark;
            this.U_Test_Sub_Code = Test_Sub_Code;
            this.U_Test_Description = Test_Description;
        }

    }
}
