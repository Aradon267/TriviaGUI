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
    class CloseRoomResponse
    {
        [DataMember]
        public int status { get; set; }
        public CloseRoomResponse()
        {

        }
        public CloseRoomResponse(int _status)
        {
            this.status = _status;
        }
    }
}
