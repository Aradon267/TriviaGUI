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
    class GetQuestionResponse
    {
        [DataMember]
        public string question { get; set; }
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public Dictionary<string, string> answers { get; set; }
        public GetQuestionResponse() { }
        public GetQuestionResponse(string question, int status, Dictionary<string,string> answers) {
            this.question = question;
            this.status = status;
            this.answers = answers;
        }
    }
}
