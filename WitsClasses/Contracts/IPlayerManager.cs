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
        Player GetPlayerByUser(String username);

        [OperationContract]
        Player GetPlayerByUserAndPassword(String username, String userPassword);

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
        public String username;
        public String email;
        public String userPassword;
        public int highestScore;
        public int profilePictureId;
        public int celebrationId;

        [DataMember]
        public String User { get { return username; } set { username = value; } }

        [DataMember]
        public String Email { get { return email; } set { email = value; } }

        [DataMember]
        public String Password { get { return userPassword; } set { userPassword = value; } }

        [DataMember]
        public int HighestScore { get { return highestScore; } set { highestScore = value; } }

        [DataMember]
        public int ProfilePictureId { get { return profilePictureId; } set { profilePictureId = value; } }

        [DataMember]
        public int CelebrationId { get { return celebrationId; } set { celebrationId = value; } }
    }
}
