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
    class PlayerResults
    {
        [DataMember]
        public string username { get; set; }
        [DataMember]

        public int correctAnswerCount { get; set; }
        [DataMember]

        public int wrongAnswerCount { get; set; }
        public PlayerResults() { }
        public PlayerResults(string username, int correct, int wrong) { 
            this.username = username;
            this.correctAnswerCount = correct;
            this.wrongAnswerCount = wrong;
        }

        public int CompareTo(PlayerResults comparePart)
        {
            // A null value means that this object is greater.
            if (comparePart == null)
                return 1;

            else
                return this.correctAnswerCount.CompareTo(comparePart.correctAnswerCount);
        }
    }
}
