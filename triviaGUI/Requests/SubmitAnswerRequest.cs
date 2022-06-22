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
    [DataContract]
    class SubmitAnswerRequest
    {
        [DataMember]
        public int answerID { get; set; }
        public SubmitAnswerRequest()
        {

        }
        public SubmitAnswerRequest(int ID)
        {
            this.answerID = ID;
        }
    }
}
