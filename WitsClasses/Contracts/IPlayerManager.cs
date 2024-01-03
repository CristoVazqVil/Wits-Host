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
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string UserPassword { get; set; }

        [DataMember]
        public int HighestScore { get; set; }

        [DataMember]
        public int ProfilePictureId { get; set; }

        [DataMember]
        public int CelebrationId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Player other = (Player)obj;

            return Username == other.Username &&
                   Email == other.Email &&
                   UserPassword == other.UserPassword &&
                   HighestScore == other.HighestScore &&
                   ProfilePictureId == other.ProfilePictureId &&
                   CelebrationId == other.CelebrationId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
