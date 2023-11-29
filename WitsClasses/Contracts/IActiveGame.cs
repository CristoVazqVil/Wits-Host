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
        void SavePlayerAnswer(int playerNumber, string answer, int gameId);

        [OperationContract(IsOneWay = true)]
        void ReceivePlayerSelectedAnswer(int playerNumber, int selectedAnswer,int idProfilePicture, int gameId);

      

        

    }

    [ServiceContract]
    public interface IActiveGameCallback
    {
        [OperationContract]
        void UpdateAnswers(Dictionary<int, string> playerAnswers);

        [OperationContract]
        void UpdateSelection(Dictionary<int, PlayerSelectedAnswer> playerSelectedAnswers);
    }

    public class PlayerSelectedAnswer
    {
        

        [DataMember]
        public int SelectedAnswer { get; set; }

        [DataMember]
        public int IdProfilePicture { get; set; }
    }
}
