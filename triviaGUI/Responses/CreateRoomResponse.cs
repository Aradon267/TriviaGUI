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
    public class CreateRoomResponse
    {
        [DataMember]
        public int status { get; set; }
        [DataMember]
        public int room_id { get; set; }
        public CreateRoomResponse()
        {

        }
        public CreateRoomResponse(int _status, int _id)
        {
            this.room_id = _id;
            this.status = _status;
        }
    }
}
