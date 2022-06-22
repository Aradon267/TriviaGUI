using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Text.Json;
using System.Runtime.Serialization.Json;
namespace triviaGUI
{
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public LoginRequest(string user, string pass)
        {
            this.username = user;
            this.password = pass;
        }
        public LoginRequest()
        {

        }
    }
}
