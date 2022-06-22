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
    class JoinRoomRequest
    {
        [DataMember]
        public int room_id { get; set; }
        public JoinRoomRequest()
        {

        }
        public JoinRoomRequest(int id)
        {
            this.room_id = id;
        }
    }
}
