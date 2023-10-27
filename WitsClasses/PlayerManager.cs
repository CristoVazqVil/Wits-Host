using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using WitsClasses.Contracts;

namespace WitsClasses
{
    public class PlayerManager : IPlayerManager
    {
        public int AddPlayer(Player player)
        {
            int affectedTables = 0;
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;

                Players newUser = new Players();
                newUser.username = player.username;
                newUser.email = player.email;
                newUser.userPassword = player.userPassword;
                newUser.highestScore = player.highestScore;
                newUser.profilePictureId = player.profilePictureId;
                newUser.celebrationId = player.celebrationId;

                try
                {
                    var newPlayer = context.Players.Add(newUser);
                    affectedTables = context.SaveChanges();
                }
                catch (EntityException ex)
                {
                    Console.WriteLine(ex.Message);
                    affectedTables = 0;
                    return affectedTables;
                }
            }

            return affectedTables;
        }

        public Player GetPlayerByUser(string username)
        {
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var player = context.Players.Find(username);

                    if (player != null)
                    {
                        Player foundPlayer = new Player
                        {
                            username = player.username,
                            email = player.email,
                            userPassword = player.userPassword,
                            highestScore = (int)player.highestScore,
                            profilePictureId = (int)player.profilePictureId,
                            celebrationId = (int)player.celebrationId
                        };

                        return foundPlayer;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public Player GetPlayerByUserAndPassword(string username, string userPassword)
        {
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var player = context.Players.FirstOrDefault(p => p.username == username && p.userPassword == userPassword);

                    if (player != null)
                    {
                        Player foundPlayer = new Player
                        {
                            username = player.username,
                            email = player.email,
                            userPassword = player.userPassword,
                            highestScore = (int)player.highestScore,
                            profilePictureId = (int)player.profilePictureId,
                            celebrationId = (int)player.celebrationId
                        };

                        return foundPlayer;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public Question GetQuestionByID(int questionId)
        {
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var question = context.Questions.Find(questionId);

                    if (question != null)
                    {
                        Question foundQuestion = new Question
                        {
                            questionES = question.questionES,
                            questionEN = question.questionEN,
                            answerES = question.answerES,
                            answerEN = question.answerEN
                        };

                        return foundQuestion;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }
    }
}