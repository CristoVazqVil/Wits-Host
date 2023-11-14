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
    public interface IGameService
    {
        [OperationContract]
        void CreateGame(int gameId, string gameLeader, int numberOfPlayers);

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

    }

    [DataContract]
    public class Game
    {
        public int gameId;
        public string gameLeader;
        public int numberOfPlayers;
        public Dictionary<string, int> playerScores;

        [DataMember]
        public int GameId { get { return gameId; } set { gameId = value; } }

        [DataMember]
        public string GameLeader { get { return gameLeader; } set { gameLeader = value; } }

        [DataMember]
        public int NumberOfPlayers { get { return numberOfPlayers; } set { numberOfPlayers = value; } }

        [DataMember]
        public Dictionary<string, int> PlayerScores { get { return playerScores; } set { playerScores = value; } }

        public Game(int gameId, string gameLeader, int numberOfPlayers)
        {
            GameId = gameId;
            GameLeader = gameLeader;
            NumberOfPlayers = numberOfPlayers;
            PlayerScores = new Dictionary<string, int>();
        }
    }
}
