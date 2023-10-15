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
    interface IPlayerManager
    {
        [OperationContract]
        int AddPlayer(Player player);

        [OperationContract]
        Player GetPlayerByUser(String username);

        [OperationContract]
        Player GetPlayerByUserAndPassword(String username, String userPassword);
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
