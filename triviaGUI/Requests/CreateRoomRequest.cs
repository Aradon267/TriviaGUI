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
    class CreateRoomRequest
    {
        [DataMember]
        public string room_name { get; set; }
        [DataMember]
        public int max_users { get; set; }
        [DataMember]
        public int question_count { get; set; }
        [DataMember]
        public int answer_timeout { get; set; }
        public CreateRoomRequest()
        {

        }
        public CreateRoomRequest(string name, int max, int count, int time)
        {
            this.room_name = name;
            this.max_users = max;
            this.question_count = count;
            this.answer_timeout = time;
        }
    }
}
