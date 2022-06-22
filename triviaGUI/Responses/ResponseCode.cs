using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace triviaGUI
{
    public enum ResponseCode
    {
		error = 0,
		success = 1,
		nameExsits = 2,
		wrongPassword = 3,
		invalidName = 4,
		userNotConnected = 5,
		alreadyConnected = 6,
		badPassword = 7,
		badEmail = 8,
		badAddr = 9,
		badPhone = 10,
		noRooms = 11,
		roomFull = 12,
		noRoomFound = 13,
		noMoreQuestions = 14,
		gameNotFinished = 15,
		gameStarted = 16
	}
}
