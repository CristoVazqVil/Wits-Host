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

        public class PlayerGameKey
        {
            public int GameId { get; set; }
            public int PlayerNumber { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                PlayerGameKey other = (PlayerGameKey)obj;
                return (GameId == other.GameId) && (PlayerNumber == other.PlayerNumber);
            }

            public override int GetHashCode()
            {
                return (GameId, PlayerNumber).GetHashCode();
            }

            public override string ToString()
            {
                return $"GameId: {GameId}, PlayerNumber: {PlayerNumber}";
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Game other = (Game)obj;

            return GameId == other.GameId &&
                   GameStatus == other.GameStatus &&
                   GameLeader == other.GameLeader &&
                   NumberOfPlayers == other.NumberOfPlayers &&
                   DictionaryEquals(PlayerScores, other.PlayerScores) &&
                   DictionaryEquals(PlayerReadyToWagerStatus, other.PlayerReadyToWagerStatus) &&
                   DictionaryEquals(PlayerHasWageredStatus, other.PlayerHasWageredStatus) &&
                   DictionaryEquals(PlayerEnded, other.PlayerEnded) &&
                   DictionaryEquals(PlayerAnswers, other.PlayerAnswers) &&
                   ListEquals(QuestionIds, other.QuestionIds);
        }

        private bool DictionaryEquals<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            if (dict1 == null && dict2 == null)
                return true;

            if (dict1 == null || dict2 == null)
                return false;

            if (dict1.Count != dict2.Count)
                return false;

            foreach (var kvp in dict1)
            {
                if (!dict2.TryGetValue(kvp.Key, out var value) || !EqualityComparer<TValue>.Default.Equals(kvp.Value, value))
                    return false;
            }

            return true;
        }

        private bool ListEquals<T>(List<T> list1, List<T> list2)
        {
            if (list1 == null && list2 == null)
                return true;

            if (list1 == null || list2 == null)
                return false;

            if (list1.Count != list2.Count)
                return false;

            return Enumerable.SequenceEqual(list1, list2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Question other = (Question)obj;

            return string.Equals(QuestionES, other.QuestionES) &&
                   string.Equals(AnswerES, other.AnswerES) &&
                   string.Equals(QuestionEN, other.QuestionEN) &&
                   string.Equals(AnswerEN, other.AnswerEN) &&
                   TrueAnswer == other.TrueAnswer;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
