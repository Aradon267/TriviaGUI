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
    class GetPlayersInRoomResponse
    {
        [DataMember]
        public List<string> PlayersInRoom { get; set; }

        [DataMember]
        public int status { get; set; }

        public GetPlayersInRoomResponse() { }
        public GetPlayersInRoomResponse(List<string> ls, int status)
        {
            PlayersInRoom = ls;
            status = status;
        }
    }
}
