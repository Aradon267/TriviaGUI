using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace triviaGUI
{
    public class SignupRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string addr { get; set; }
        public SignupRequest()
        {

        }
        public SignupRequest(string user, string pass, string mail, string _phone, string _addr)
        {
            this.username = user;
            this.password = pass;
            this.email = mail;
            this.phone = _phone;
            this.addr = _addr;
        }
    }
}
