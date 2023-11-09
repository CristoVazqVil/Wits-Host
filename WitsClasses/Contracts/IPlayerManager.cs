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

        [OperationContract]
        Question GetQuestionByID(int questionId);

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

    public class Question
    {
        public String questionES;
        public String answerES;
        public String questionEN;
        public String answerEN;

        [DataMember]
        public String QuestionES { get { return questionES; } set { questionES = value; } }

        [DataMember]
        public String AnswerES { get { return answerES; } set { answerES = value; } }

        [DataMember]
        public String QuestionEN { get { return questionEN; } set { questionEN = value; } }

        [DataMember]
        public String AnswerEN { get { return answerEN; } set { answerEN = value; } }
    }
}
