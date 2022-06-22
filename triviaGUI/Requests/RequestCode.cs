using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace triviaGUI
{
	enum RequestCode
	{
		LoginCode = 1,
		SignupCode = 2,
		CreateRoomRequestCode = 3,
		GetRoomsRequestCode = 4,
		GetPlayersInRoomRequestCode = 5,
		JoinRoomRequestCode = 6,
		GetStatisticsRequestCode = 7,
		GetPersonalStatsCode = 8,
		logoutCode = 229,
		AddQuestionCode = 9,
		LeaveRoomCode = 10,
		GetRoomStateCode = 11,
		CloseRoomCode = 12,
		StartGameCode = 13,
		LeaveGameCode = 14,
		GetQuestionCode = 15,
		SubmitAnswerCode = 16,
		GetGameResultCode = 17,
	}
}
