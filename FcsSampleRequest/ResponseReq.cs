namespace FcsSampleRequest
{
    public class ResponseReq
    {
        public bool success { get; set; }
        public string Msg {     get; set; }

        public ResponseReq() { }

        public ResponseReq(bool _success, string _msg)
        {    
            success = _success;
            Msg = _msg;

        }
    }


}









