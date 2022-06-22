using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace triviaGUI
{
    public class RoomData
    {
        public RoomData(int id, string name, int maxPlayers, int timePerQuestion, bool isActive, int questionsCount)
        {
            this.id = id;
            this.name = name;
            this.maxPlayers = maxPlayers;
            this.timePerQuestion = timePerQuestion;
            this.isActive = isActive;
            this.questionsCount = questionsCount;
        }
        public RoomData() { }
        public int id { get; set; }
        public string name { get; set; }
        public int maxPlayers { get; set; }
        public int timePerQuestion { get; set; }
        public int questionsCount { get; set; }
        public bool isActive { get; set; }
    }
}
