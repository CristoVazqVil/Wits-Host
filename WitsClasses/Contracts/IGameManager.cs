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
        private int gameId;
        private int gameStatus;
        private string gameLeader;
        private int numberOfPlayers;
        private Dictionary<string, int> playerScores;
        private Dictionary<int, bool> playerReadyToWagerStatus = new Dictionary<int, bool>();
        private Dictionary<int, bool> playerHasWageredStatus = new Dictionary<int, bool>();
        private Dictionary<int, bool> playerEnded = new Dictionary<int, bool>();
        private Dictionary<int, string> playerAnswers;
        private List<int> questionIds;

        [DataMember]
        public int GameId
        {
            get { return gameId; }
            set { gameId = value; }
        }

        [DataMember]
        public int GameStatus
        {
            get { return gameStatus; }
            set { gameStatus = value; }
        }

        [DataMember]
        public string GameLeader
        {
            get { return gameLeader; }
            set { gameLeader = value; }
        }

        [DataMember]
        public int NumberOfPlayers
        {
            get { return numberOfPlayers; }
            set { numberOfPlayers = value; }
        }

        [DataMember]
        public Dictionary<string, int> PlayerScores
        {
            get { return playerScores; }
            set { playerScores = value; }
        }

        [DataMember]
        public Dictionary<int, bool> PlayerReadyToWagerStatus
        {
            get { return playerReadyToWagerStatus; }
            set { playerReadyToWagerStatus = value; }
        }

        [DataMember]
        public Dictionary<int, bool> PlayerHasWageredStatus
        {
            get { return playerHasWageredStatus; }
            set { playerHasWageredStatus = value; }
        }

        [DataMember]
        public Dictionary<int, bool> PlayerEnded
        {
            get { return playerEnded; }
            set { playerEnded = value; }
        }

        [DataMember]
        public Dictionary<int, string> PlayerAnswers
        {
            get { return playerAnswers; }
            set { playerAnswers = value; }
        }

        [DataMember]
        public List<int> QuestionIds
        {
            get { return questionIds; }
            set { questionIds = value; }
        }

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
        private string questionES;
        private string answerES;
        private string questionEN;
        private string answerEN;
        private int trueAnswer;

        [DataMember]
        public string QuestionES
        {
            get { return questionES; }
            set { questionES = value; }
        }

        [DataMember]
        public string AnswerES
        {
            get { return answerES; }
            set { answerES = value; }
        }

        [DataMember]
        public string QuestionEN
        {
            get { return questionEN; }
            set { questionEN = value; }
        }

        [DataMember]
        public string AnswerEN
        {
            get { return answerEN; }
            set { answerEN = value; }
        }

        [DataMember]
        public int TrueAnswer
        {
            get { return trueAnswer; }
            set { trueAnswer = value; }
        }
    }
}
