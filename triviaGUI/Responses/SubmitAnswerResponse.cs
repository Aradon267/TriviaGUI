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
    class SubmitAnswerResponse
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public int correctAnswerID { get; set; }
        public SubmitAnswerResponse() { }
        public SubmitAnswerResponse(int status, int correct) {
            this.status = status;
            this.correctAnswerID = correct;
        }
    }
}
