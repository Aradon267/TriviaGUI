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
    class AddQuestionRequest
    {
        [DataMember]
        public string question { get; set; }
        [DataMember]
        public string correct { get; set; }
        [DataMember]
        public string ans1 { get; set; }
        [DataMember]
        public string ans2 { get; set; }
        [DataMember]
        public string ans3 { get; set; }
        public AddQuestionRequest()
        {

        }
        public AddQuestionRequest(string quest, string corr, string ans1, string ans2, string ans3)
        {
            this.question = quest;
            this.correct = corr;
            this.ans1 = ans1;
            this.ans2 = ans2;
            this.ans3 = ans3;
        }
    }
}
