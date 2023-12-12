using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WitsClasses.Contracts
{
    [ServiceContract]
    interface IPlayerManager
    {
        [OperationContract]
        int AddPlayer(Player player);

        [OperationContract]
        int DeletePlayer(string username);

        [OperationContract]
        bool IsPlayerLogged(string username);

        [OperationContract]
        Player GetPlayerByUser(String username);

        [OperationContract]
        Player GetPlayerByUserAndPassword(String username, String userPassword);

        [OperationContract]
        int UpdatePassword(string username, string password);

        [OperationContract]
        List<string> GetPlayerFriends(string playerUsername);

        [OperationContract]
        List<string> GetAllPlayerRequests(string playerUsername);

        [OperationContract]
        string GetPlayerRequest(string playerFrom, string playerTo);

        [OperationContract]
        int AddRequest(string from, string to);

        [OperationContract]
        int AcceptRequest(string receiver, string sender);

        [OperationContract]
        int RejectRequest(string receiver, string sender);

        [OperationContract]
        int DeleteRequest(string receiver, string sender, int status);

        [OperationContract]
        int AddFriendship(string player, string friend);

        [OperationContract]
        int DeleteFriendship(string player, string friend);

        [OperationContract]
        bool IsPlayerBlocked(string player, string blockedPlayer);

        [OperationContract]
        int BlockPlayer(string player, string blockedPlayer);

        [OperationContract]
        bool UpdateProfilePicture(string username, int profilePictureId);

        [OperationContract]
        bool UpdateCelebration(string username, int celebrationId);

    }

    [DataContract]
    public class Player
    {
        private string username;
        private string email;
        private string userPassword;
        private int highestScore;
        private int profilePictureId;
        private int celebrationId;

        [DataMember]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [DataMember]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        [DataMember]
        public string UserPassword
        {
            get { return userPassword; }
            set { userPassword = value; }
        }

        [DataMember]
        public int HighestScore
        {
            get { return highestScore; }
            set { highestScore = value; }
        }

        [DataMember]
        public int ProfilePictureId
        {
            get { return profilePictureId; }
            set { profilePictureId = value; }
        }

        [DataMember]
        public int CelebrationId
        {
            get { return celebrationId; }
            set { celebrationId = value; }
        }
    }
}
