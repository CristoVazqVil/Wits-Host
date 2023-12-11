using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WitsClasses.Contracts
{
    [ServiceContract(CallbackContract = typeof(IActiveGameCallback))]
    public interface IActiveGame
    {
        [OperationContract(IsOneWay = true)]
        void RegisterUserInGameContext(string username);

        [OperationContract(IsOneWay = true)]
        void UnregisterUserInGameContext(string username);

        [OperationContract(IsOneWay = true)]
        void ExpelPlayer(string username);

        [OperationContract(IsOneWay = true)]
        void SavePlayerAnswer(int playerNumber, string answer, int gameId);

        [OperationContract(IsOneWay = true)]
        void ReceivePlayerSelectedAnswer(int playerNumber, int selectedAnswer,int idProfilePicture, int gameId);

        [OperationContract(IsOneWay = true)]
        void ReadyToWager(int gameId, int playerNumber, bool isReady);

        [OperationContract(IsOneWay = true)]
        void ReadyToShowAnswer(int gameId, int playerNumber, bool isReady);

        [OperationContract(IsOneWay = true)]
        void GameEnded(int gameId, int playerNumber, bool isRegistered);

        [OperationContract(IsOneWay = true)]
        void WhoWon(int gameId,int numberPlayer, string userName, int idCelebration, int score, int idProfilePicture);


        [OperationContract(IsOneWay = true)]
        void ShowWinner(int gameId);


        [OperationContract(IsOneWay = true)]
        void CleanWinners(int gameId);

    }

    [ServiceContract]
    public interface IActiveGameCallback
    {
        [OperationContract]
        void UpdateAnswers(Dictionary<int, string> playerAnswers);

        [OperationContract]
        void UpdateSelection(Dictionary<int, PlayerSelectedAnswer> playerSelectedAnswers);

        [OperationContract]
        void ShowEnterWager();

        [OperationContract]
        void ShowTrueAnswer();

        [OperationContract]
        void BeExpelled();

        [OperationContract]
        void ShowVictoryScreen(string userName, int profilePictureId, int celebrationId, int score);

        [OperationContract]
        void TieBreaker();

    }

    public class PlayerSelectedAnswer
    {
        [DataMember]
        public int SelectedAnswer { get; set; }

        [DataMember]
        public int IdProfilePicture { get; set; }
    }
}
