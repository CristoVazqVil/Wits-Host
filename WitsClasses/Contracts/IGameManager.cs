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
        Dictionary<string, int> GetScores(int gameId);

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
        public int gameId;
        public string gameLeader;
        public int numberOfPlayers;
        public Dictionary<string, int> playerScores;
        public List<int> questionIds;
        public Dictionary<int, string> playerAnswers;

        [DataMember]
        public int GameId { get { return gameId; } set { gameId = value; } }

        [DataMember]
        public string GameLeader { get { return gameLeader; } set { gameLeader = value; } }

        [DataMember]
        public int NumberOfPlayers { get { return numberOfPlayers; } set { numberOfPlayers = value; } }

        [DataMember]
        public Dictionary<string, int> PlayerScores { get { return playerScores; } set { playerScores = value; } }


        [DataMember]
        public Dictionary<int, bool> PlayerReadyToWagerStatus { get; set; } = new Dictionary<int, bool>();

        [DataMember]
        public Dictionary<int, bool> PlayerHasWageredStatus { get; set; } = new Dictionary<int, bool>();

        [DataMember]
        public Dictionary<int, bool> PlayerEnded { get; set; } = new Dictionary<int, bool>();

       

        [DataMember]
        public Dictionary<int, string> PlayerAnswers{ get { return playerAnswers; } set { playerAnswers = value; } }


        [DataMember]
        public List<int> QuestionIds { get { return questionIds; } set { questionIds = value; } }

        public Game(int gameId, string gameLeader, int numberOfPlayers)
        {
            GameId = gameId;
            GameLeader = gameLeader;
            NumberOfPlayers = numberOfPlayers;
            PlayerScores = new Dictionary<string, int>();
            PlayerAnswers = new Dictionary<int, string>();
            QuestionIds = new List<int>();
        }
    }

    public class Question
    {
        public String questionES;
        public String answerES;
        public String questionEN;
        public String answerEN;
        public int trueAnswer;

        [DataMember]
        public String QuestionES { get { return questionES; } set { questionES = value; } }

        [DataMember]
        public String AnswerES { get { return answerES; } set { answerES = value; } }

        [DataMember]
        public String QuestionEN { get { return questionEN; } set { questionEN = value; } }

        [DataMember]
        public String AnswerEN { get { return answerEN; } set { answerEN = value; } }

        [DataMember]
        public int TrueAnswer { get { return trueAnswer; } set { trueAnswer = value; } }
    }
}
