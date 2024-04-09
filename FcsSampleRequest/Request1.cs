using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcsSampleRequest
{
    class Request1
    {
        public string Barcode { get; set; }

        public string Lab_Code { get; set; }

        public Request1(string barcode, string labCode)
        {
            Barcode = barcode;
            Lab_Code = labCode;
        }
    }
}
