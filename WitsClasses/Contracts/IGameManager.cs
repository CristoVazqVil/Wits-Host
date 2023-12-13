using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WitsClasses.Contracts
{
    [ServiceContract]
    public interface IGameManager
    {
        [OperationContract]
        void CreateGame(int gameId, string gameLeader);

        [OperationContract]
        int JoinGame(int gameId, string playerId);

        [OperationContract]
        int RemovePlayerInGame(int gameId, string playerId);

        [OperationContract]
        Dictionary<string, int> GetScores(int gameId);

        [OperationContract]
        List<string> GetPlayersOfGameExceptLeader(int gameId, string leaderUser);

        [OperationContract]
        void ModifyScore(int gameId, string playerId, int credits);

        [OperationContract]
        int GetPlayerScore(int gameId, string playerId);

        [OperationContract]
        string GetGameLeader(int gameId);

        [OperationContract]
        Question GetQuestionByID(int questionId);

        [OperationContract]
        int GetRandomQuestionId();

        [OperationContract]
        List<int> GetQuestionIds(int gameId);
    }

    [DataContract]
    public class Game
    {
        [DataMember]
        public int GameId { get; set; }

        [DataMember]
        public int GameStatus { get; set; }

        [DataMember]
        public string GameLeader { get; set; }

        [DataMember]
        public int NumberOfPlayers { get; set; }

        [DataMember]
        public Dictionary<string, int> PlayerScores { get; set; }

        [DataMember]
        public Dictionary<int, bool> PlayerReadyToWagerStatus { get; set; } = new Dictionary<int, bool>();

        [DataMember]
        public Dictionary<int, bool> PlayerHasWageredStatus { get; set; } = new Dictionary<int, bool>();

        [DataMember]
        public Dictionary<int, bool> PlayerEnded { get; set; } = new Dictionary<int, bool>();

        [DataMember]
        public Dictionary<int, string> PlayerAnswers { get; set; }

        [DataMember]
        public List<int> QuestionIds { get; set; }

        public Game(int gameId, string gameLeader, int numberOfPlayers)
        {
            GameId = gameId;
            GameStatus = 0;
            GameLeader = gameLeader;
            NumberOfPlayers = numberOfPlayers;
            PlayerScores = new Dictionary<string, int>();
            PlayerAnswers = new Dictionary<int, string>();
            QuestionIds = new List<int>();
        }
    }

    public class Question
    {
        [DataMember]
        public string QuestionES { get; set; }

        [DataMember]
        public string AnswerES { get; set; }

        [DataMember]
        public string QuestionEN { get; set; }

        [DataMember]
        public string AnswerEN { get; set; }

        [DataMember]
        public int TrueAnswer { get; set; }
    }
}
