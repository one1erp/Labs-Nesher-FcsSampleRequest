using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcsSampleRequest
{
    public class Client
    {
        public string U_SDG_CLIENT { get; set; }
        public string U_PHONE { get; set; }
        public string U_EMAIL { get; set; }
        public string U_ADDRESS { get; set; }
        public string U_CONTECT_NAME { get; set; }
        public string U_CONTACT_PHONE { get; set; }
        //public string U_SAMPLED_BY { get; set; }


        public Client(string p1, string p2, string p3, string p4, string p5, string p6/*, string p7*/)
        {
            // TODO: Complete member initialization
            this.U_SDG_CLIENT = p1;
            this.U_PHONE = p2;
            this.U_EMAIL = p3;
            this.U_ADDRESS = p4;
            this.U_CONTECT_NAME = p5;
            this.U_CONTACT_PHONE = p6;
            //this.U_SAMPLED_BY = p7;
        }

    }
}
