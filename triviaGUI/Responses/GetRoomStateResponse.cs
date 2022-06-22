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
    public class GetRoomStateResponse
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public bool hasGameBegun { get; set; }
        [DataMember]
        public List<string> players { get;set; }
        [DataMember]
        public int questionCount { get; set;}
        [DataMember]
        public int answerTimeout { get;set;}
    }
}
