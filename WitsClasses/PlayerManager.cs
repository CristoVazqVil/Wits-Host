using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using WitsClasses.Contracts;
using System.Windows;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Media;


namespace WitsClasses
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class PlayerManager : IPlayerManager, IConnectedUsers, IGameManager
    {
        private const int PENDING = 0;
        private const int ACCEPTED = 1;
        private const int REJECTED = 2;
        private static PlayerManager instance;
        private static readonly ILog witslogger = LogManager.GetLogger(typeof(PlayerManager));
        private Dictionary<string, IConnectedUsersCallback> connectedUsersInMenu = new Dictionary<string, IConnectedUsersCallback>();
        private Dictionary<string, IChatManagerCallback> usersInLobbyContexts = new Dictionary<string, IChatManagerCallback>();
        private Dictionary<string, int> playersnumbers = new Dictionary<string, int>();
        private Dictionary<string, IActiveGameCallback> usersInGameContexts = new Dictionary<string, IActiveGameCallback>();
        private List<string> allConnectedUsers = new List<string>();
        private List<Game> games = new List<Game>();
        private List<int> usedQuestionIds = new List<int>();
        private Random random = new Random();

        public PlayerManager()
        {
        }

        public void RemoveFromEverywhere(string user)
        {
            try
            {
                RemoveConnectedUserInMenu(user);
                RemoveInPlayersNumbers(user);
                RemoveUserInLobbyContext(user);
                RemoveUserInGameContext(user);
                RemoveFromAllConnectedUsers(user);
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }
            catch (InvalidOperationException ex)
            {
                witslogger.Error(ex);
            }
        }

        public void RemoveUserInLobbyContext(string user)
        {
            try
            {
                lock (usersInLobbyContexts)
                {
                    if (usersInLobbyContexts.ContainsKey(user))
                    {
                        usersInLobbyContexts.Remove(user);
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }
            catch (InvalidOperationException ex)
            {
                witslogger.Error(ex);
            }
        }

        public void RemoveUserInGameContext(string user)
        {
            try
            {
                lock (usersInGameContexts)
                {
                    if (usersInGameContexts.ContainsKey(user))
                    {
                        usersInGameContexts.Remove(user);
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }
            catch (InvalidOperationException ex)
            {
                witslogger.Error(ex);
            }
        }

        public void RemoveFromAllConnectedUsers(string user)
        {
            try
            {
                lock (allConnectedUsers)
                {
                    if (allConnectedUsers.Contains(user))
                    {
                        allConnectedUsers.Remove(user);
                    }
                }
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }
            catch (InvalidOperationException ex)
            {
                witslogger.Error(ex);
            }
        }

        public static PlayerManager GetInstance()
        {
            if (instance == null)
            {
                instance = new PlayerManager();
            }
            return instance;
        }

        public int AddPlayer(Player player)
        {
            int affectedTables = 0;
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;

                Players newUser = new Players();
                newUser.username = player.Username;
                newUser.email = player.Email;
                newUser.userPassword = player.UserPassword;
                newUser.highestScore = player.HighestScore;
                newUser.profilePictureId = player.ProfilePictureId;
                newUser.celebrationId = player.CelebrationId;

                try
                {
                    context.Players.Add(newUser);
                    affectedTables = context.SaveChanges();
                }
                catch (DataException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
            }

            return affectedTables;
        }

        public int DeletePlayer(string username)
        {
            int affectedRows = 0;

            using (var context = new WitsAndWagersEntities())
            {
                try
                {
                    var playerToDelete = context.Players.FirstOrDefault(p => p.username == username);

                    if (playerToDelete != null)
                    {
                        context.Players.Remove(playerToDelete);
                        affectedRows = context.SaveChanges();
                    }
                    else
                    {
                        affectedRows = 0;
                    }
                }
                catch (DataException ex)
                {
                    witslogger.Error(ex);
                    affectedRows = 0;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    affectedRows = 0;
                }
            }

            return affectedRows;

        }

        public bool IsPlayerLogged(string username)
        {
            return allConnectedUsers.Contains(username);
        }

        public int UpdatePassword(string username, string password)
        {
            int affectedTables = 0;

            using (var context = new WitsAndWagersEntities())
            {
                try
                {
                    var playerToUpdate = context.Players
                        .FirstOrDefault(p => p.username == username);

                    if (playerToUpdate != null)
                    {
                        playerToUpdate.userPassword = password;
                        affectedTables = context.SaveChanges();
                    }
                    else
                    {
                        affectedTables = 0;
                    }
                }
                catch (DataException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
            }

            return affectedTables;
        }

        public void AddConnectedUser(string username)
        {
            
            if (!allConnectedUsers.Contains(username))
            {
                allConnectedUsers.Add(username);
                UpdateFriendsForAll();
            }

            AddConnectedUserInMenu(username);
        }

        public void AddConnectedUserInMenu(string username)
        {
            try
            {
                if (!connectedUsersInMenu.ContainsKey(username))
                {
                    IConnectedUsersCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IConnectedUsersCallback>();
                    connectedUsersInMenu.Add(username, currentUserCallbackChannel);
                    connectedUsersInMenu[username].UpdateConnectedFriends();
                }
            }
            catch (CommunicationException ex)
            {
                witslogger.Error(ex);
                RemoveFromEverywhere(username);
            }
            catch (TimeoutException ex)
            {
                witslogger.Error(ex);
            }
            catch (InvalidOperationException ex)
            {
                witslogger.Error(ex);
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }
        }

        public void RemoveConnectedUserInMenu(string username)
        {
            try
            {
                lock (connectedUsersInMenu)
                {
                    if (connectedUsersInMenu.ContainsKey(username))
                    {
                        connectedUsersInMenu.Remove(username);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                witslogger.Error(ex);
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }
        }

        public void RemoveInPlayersNumbers(string username)
        {
            try
            {
                lock (playersnumbers)
                {
                    if (playersnumbers.ContainsKey(username))
                    {
                        playersnumbers.Remove(username);
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                witslogger.Error(ex);
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }

        }

        public void UpdateFriendsForAll()
        {
            foreach (string connectedUser in allConnectedUsers)
            {
                try
                {
                    connectedUsersInMenu[connectedUser].UpdateConnectedFriends();
                }
                catch (KeyNotFoundException ex)
                {
                    witslogger.Error(ex);
                }
                catch (CommunicationException ex)
                {
                    witslogger.Error(ex);
                    RemoveFromEverywhere(connectedUser);
                }
                catch (TimeoutException ex)
                {
                    witslogger.Error(ex);
                }
            }
        }

        public void RemoveConnectedUser(string username)
        {
            try
            {
                allConnectedUsers.Remove(username);
                RemoveConnectedUserInMenu(username);
                RemoveInPlayersNumbers(username);
            }
            catch (ArgumentNullException ex)
            {
                witslogger.Error(ex);
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }

            UpdateFriendsForAll();
        }


        public Player GetPlayerByUser(string username)
        {
            Player foundPlayer = new Player();
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var player = context.Players.Find(username);

                    if (player != null)
                    {
                        foundPlayer.Username = player.username;
                        foundPlayer.Email = player.email;
                        foundPlayer.UserPassword = player.userPassword;
                        foundPlayer.HighestScore = (int)player.highestScore;
                        foundPlayer.ProfilePictureId = (int)player.profilePictureId;
                        foundPlayer.CelebrationId = (int)player.celebrationId;
                    }
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                }
            }

            return foundPlayer;
        }

        public Player GetPlayerByUserAndPassword(string username, string userPassword)
        {
            Player foundPlayer = new Player();
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var player = context.Players.FirstOrDefault(p => p.username == username && p.userPassword == userPassword);

                    if (player != null)
                    {
                        foundPlayer.Username = player.username;
                        foundPlayer.Email = player.email;
                        foundPlayer.UserPassword = player.userPassword;
                        foundPlayer.HighestScore = (int)player.highestScore;
                        foundPlayer.ProfilePictureId = (int)player.profilePictureId;
                        foundPlayer.CelebrationId = (int)player.celebrationId;
                    }
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                }
            }

            return foundPlayer;
        }

        public List<string> GetPlayerFriends(string playerUsername)
        {
            List<string> friendUsernames = new List<string>();

            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    friendUsernames = context.Friends
                        .Where(f => f.principalPlayer == playerUsername)
                        .Select(f => f.friend)
                        .ToList();
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                }
            }

            return friendUsernames;
        }

        public List<string> GetAllPlayerRequests(string playerUsername)
        {
            List<string> requests = new List<string>();

            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    requests = context.Notifications
                        .Where(n => n.receiverPlayer == playerUsername && n.notificationState == PENDING)
                        .Select(n => n.senderPlayer)
                        .ToList();
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                }
            }

            return requests;
        }

        public string GetPlayerRequest(string playerFrom, string playerTo)
        {
            string found = null;
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var request = context.Notifications
                        .Where(n => n.senderPlayer == playerFrom && n.receiverPlayer == playerTo && n.notificationState == PENDING)
                        .Select(n => n.senderPlayer);

                    if (request != null && !request.Equals(""))
                    {
                        found = "Found";
                    }
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                }
            }

            return found;
        }

        public int AddRequest(string from, string to)
        {
            int affectedTables = 0;

            using (var context = new WitsAndWagersEntities())
            {
                try
                {
                    bool existingNotification = context.Notifications
                    .Any(n => ((n.senderPlayer == from && n.receiverPlayer == to) || (n.receiverPlayer == from && n.senderPlayer == to)) && (n.notificationState == ACCEPTED || n.notificationState == PENDING));

                    if (!existingNotification)
                    {
                        Notifications newNotification = new Notifications
                        {
                            senderPlayer = from,
                            receiverPlayer = to,
                            notificationState = PENDING
                        };

                        context.Notifications.Add(newNotification);
                        affectedTables = context.SaveChanges();
                    }
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
                catch (DataException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }

            }
            return affectedTables;
        }

        public int AcceptRequest(string receiver, string sender)
        {
            int affectedTables = 0;

            using (var context = new WitsAndWagersEntities())
            {
                try
                {
                    var notificationToUpdate = context.Notifications
                        .FirstOrDefault(n => n.senderPlayer == sender && n.receiverPlayer == receiver && n.notificationState == PENDING);

                    if (notificationToUpdate != null)
                    {
                        notificationToUpdate.notificationState = ACCEPTED;
                        affectedTables = context.SaveChanges();
                    }
                    else
                    {
                        affectedTables = 0;
                    }
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
                catch (DataException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
            }

            return affectedTables;
        }

        public int RejectRequest(string receiver, string sender)
        {
            int affectedTables = 0;

            using (var context = new WitsAndWagersEntities())
            {
                try
                {
                    var notificationToUpdate = context.Notifications
                        .FirstOrDefault(n => n.senderPlayer == sender && n.receiverPlayer == receiver && n.notificationState == PENDING);

                    if (notificationToUpdate != null)
                    {
                        notificationToUpdate.notificationState = REJECTED;
                        affectedTables = context.SaveChanges();
                    }
                    else
                    {
                        affectedTables = 0;
                    }
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
                catch (DataException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
            }

            return affectedTables;
        }

        public int DeleteRequest(string receiver, string sender, int status)
        {
            int affectedTables = 0;

            using (var context = new WitsAndWagersEntities())
            {
                try
                {
                    var notificationToDelete = context.Notifications
                        .FirstOrDefault(n => n.senderPlayer == sender && n.receiverPlayer == receiver && n.notificationState == status);

                    if (notificationToDelete != null)
                    {
                        context.Notifications.Remove(notificationToDelete);
                        affectedTables = context.SaveChanges();
                    }
                    else
                    {
                        affectedTables = 0;
                    }
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
                catch (DataException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                }
            }

            return affectedTables;
        }

        public int AddFriendship(string player, string friend)
        {
            int affectedTables = 0;

            using (var context = new WitsAndWagersEntities())
            {
                if (player == friend)
                {
                    affectedTables = 0;
                }
                else
                {
                    try
                    {
                        Friends newFriendship = new Friends
                        {
                            principalPlayer = player,
                            friend = friend
                        };

                        context.Friends.Add(newFriendship);
                        affectedTables = context.SaveChanges();
                    }
                    catch (EntityException ex)
                    {
                        witslogger.Error(ex);
                        affectedTables = 0;
                    }
                    catch (DataException ex)
                    {
                        witslogger.Error(ex);
                        affectedTables = 0;
                    }
                    catch (SqlException ex)
                    {
                        witslogger.Error(ex);
                        affectedTables = 0;
                    }
                }
            }

            return affectedTables;
        }

        public int DeleteFriendship(string player, string friend)
        {
            int affectedTables = 0;

            using (var context = new WitsAndWagersEntities())
            {
                if (player == friend)
                {
                    affectedTables = 0;
                }
                else
                {
                    try
                    {
                        var friendshipToDelete = context.Friends
                            .FirstOrDefault(f => f.principalPlayer == player && f.friend == friend);

                        if (friendshipToDelete != null)
                        {
                            context.Friends.Remove(friendshipToDelete);
                            affectedTables = context.SaveChanges();
                        }
                        else
                        {
                            affectedTables = 0;
                        }
                    }
                    catch (EntityException ex)
                    {
                        witslogger.Error(ex);
                        affectedTables = 0;
                    }
                    catch (DataException ex)
                    {
                        witslogger.Error(ex);
                        affectedTables = 0;
                    }
                    catch (SqlException ex)
                    {
                        witslogger.Error(ex);
                        affectedTables = 0;
                    }
                }
            }

            return affectedTables;
        }

        public bool IsPlayerBlocked(string player, string blockedPlayer)
        {
            bool isBlocked = false;

            using (var context = new WitsAndWagersEntities())
            {
                try
                {
                    isBlocked = context.BlockedPlayers
                        .Any(bp => bp.player == player && bp.blockedPlayer == blockedPlayer);
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                }
                catch (DataException ex)
                {
                    witslogger.Error(ex);
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                }
            }

            return isBlocked;
        }

        public int BlockPlayer(string player, string blockedPlayer)
        {
            int affectedTables = 0;

            using (var context = new WitsAndWagersEntities())
            {
                if (player == blockedPlayer)
                {
                    affectedTables = 0;
                }
                else
                {
                    try
                    {
                        bool isAlreadyBlocked = context.BlockedPlayers
                            .Any(bp => bp.player == player && bp.blockedPlayer == blockedPlayer);

                        if (!isAlreadyBlocked)
                        {
                            BlockedPlayers newBlockedPlayer = new BlockedPlayers
                            {
                                player = player,
                                blockedPlayer = blockedPlayer
                            };

                            context.BlockedPlayers.Add(newBlockedPlayer);
                            affectedTables = context.SaveChanges();
                        }
                    }
                    catch (EntityException ex)
                    {
                        witslogger.Error(ex);
                        affectedTables = 0;
                    }
                    catch (DataException ex)
                    {
                        witslogger.Error(ex);
                        affectedTables = 0;
                    }
                    catch (SqlException ex)
                    {
                        witslogger.Error(ex);
                        affectedTables = 0;
                    }
                }
            }

            return affectedTables;
        }

        public bool UpdateProfilePicture(string username, int profilePictureId)
        {
            bool validator = false;

            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var player = context.Players.FirstOrDefault(p => p.username == username);

                    if (player != null)
                    {
                        player.profilePictureId = profilePictureId;
                        context.SaveChanges();
                        return true;
                    }
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                    validator = false;
                }
                catch (DataException ex)
                {
                    witslogger.Error(ex);
                    validator = false;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    validator = false;
                }
            }

            return validator;
        }


        public bool UpdateCelebration(string username, int celebrationId)
        {
            bool validator = false;
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var player = context.Players.FirstOrDefault(p => p.username == username);

                    if (player != null)
                    {
                        player.celebrationId = celebrationId;
                        context.SaveChanges();
                        return true;
                    }
                }
                catch(EntityException ex)
                {
                    witslogger.Error(ex);
                    validator = false;
                }
                catch (DataException ex)
                {
                    witslogger.Error(ex);
                    validator = false;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    validator = false;
                }
            }

            return validator;
        }

        public int GetRandomQuestionId()
        {
            int newQuestionId;
            do
            {
                newQuestionId = random.Next(1, 150);
            } while (usedQuestionIds.Contains(newQuestionId));

            usedQuestionIds.Add(newQuestionId);

            return newQuestionId;
        }

        public Question GetQuestionByID(int questionId)
        {
            Question foundQuestion = new Question();

            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var question = context.Questions.Find(questionId);

                    if (question != null)
                    {
                        foundQuestion.QuestionES = question.questionES;
                        foundQuestion.QuestionEN = question.questionEN;
                        foundQuestion.AnswerES = question.answerES;
                        foundQuestion.AnswerEN = question.answerEN;
                        foundQuestion.TrueAnswer = (int)question.trueAnswer;
                    }
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                }
            }

            return foundQuestion;
        }

        public List<string> GetConnectedFriends(string principalPlayer)
        {
            List<string> connectedFriends = new List<string>();

            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    foreach (string friend in allConnectedUsers)
                    {
                        var friendship = context.Friends.FirstOrDefault(p => p.principalPlayer == principalPlayer && p.friend == friend);
                        if (friendship != null)
                        {
                            connectedFriends.Add(friend);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                }
            }

            return connectedFriends;
        }

        public void CreateGame(int gameId, string gameLeader)
        {
            if (games.Any(g => g.GameId == gameId))
            {
                throw new InvalidOperationException("Used ID");
            }

            var newGame = new Game(gameId, gameLeader, 1);
            newGame.PlayerScores.Add(gameLeader, 0);
            newGame.PlayerAnswers.Add(newGame.NumberOfPlayers, "");
            playersnumbers.Add(gameLeader, newGame.NumberOfPlayers);

            games.Add(newGame);
        }

        public int JoinGame(int gameId, string playerId)
        {
            int returnValue = 0;

            Game game = games.FirstOrDefault(g => g.GameId == gameId);

            if (game != null && !game.PlayerScores.ContainsKey(playerId) && game.NumberOfPlayers < 4 && game.GameStatus == 0)
            {
                game.NumberOfPlayers++;
                game.PlayerScores.Add(playerId, 0);
                game.PlayerAnswers.Add(game.NumberOfPlayers, "");
                playersnumbers.Add(playerId, game.NumberOfPlayers);

                returnValue = game.NumberOfPlayers;
            }

            return returnValue;
        }

        public int RemovePlayerInGame(int gameId, string playerId)
        {
            int returnValue = 0;
            Game game = games.FirstOrDefault(g => g.GameId == gameId);

            if (game != null && game.PlayerScores.ContainsKey(playerId))
            {
                try
                {
                    game.PlayerScores.Remove(playerId);
                    game.NumberOfPlayers--;
                    game.PlayerReadyToWagerStatus.Remove(playersnumbers[playerId]);
                    game.PlayerAnswers.Remove(playersnumbers[playerId]);
                    game.PlayerHasWageredStatus.Remove(playersnumbers[playerId]);
                    usersInGameContexts.Remove(playerId);
                    playersnumbers.Remove(playerId);
                    returnValue = 1;
                }
                catch (InvalidOperationException ex)
                {
                    witslogger.Error(ex);
                }
                catch (KeyNotFoundException ex)
                {
                    witslogger.Error(ex);
                }
                
            }

            return returnValue;
        }

        public Dictionary<string, int> GetScores(int gameId)
        {
            var game = games.FirstOrDefault(g => g.GameId == gameId);

            if (game != null)
            {
                return game?.PlayerScores;
            }
            else
            {
                return new Dictionary<string, int>();
            }
            
        }

        public void ModifyScore(int gameId, string playerId, int credits)
        {
            try
            {
                var game = games.FirstOrDefault(g => g.GameId == gameId);
                if (game != null && game.PlayerScores.ContainsKey(playerId))
                {
                    game.PlayerScores[playerId] += credits;
                }
            }
            catch (InvalidOperationException ex)
            {
                witslogger.Error(ex);
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }
        }

        public int GetPlayerScore(int gameId, string playerId)
        {
            int returnValue = 0;

            try
            {
                var game = games.First(g => g.GameId == gameId);
                returnValue = game.PlayerScores[playerId];
            }
            catch (InvalidOperationException ex)
            {
                witslogger.Error(ex);
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }

            return returnValue;
        }

        public string GetGameLeader(int gameId)
        {
            string gameLeader = null;

            try
            {
                Game game = games.FirstOrDefault(g => g.GameId == gameId);
                if (game != null)
                {
                    gameLeader = game.GameLeader;
                }
            }
            catch (InvalidOperationException ex)
            {
                witslogger.Error(ex);
            }

            return gameLeader;
        }


    }

    public partial class PlayerManager : IChatManager, IActiveGame
    {

        private Dictionary<int, string> playerAnswers = new Dictionary<int, string>();
        private Dictionary<int, PlayerSelectedAnswer> playerSelectedAnswers = new Dictionary<int, PlayerSelectedAnswer>();
        private Dictionary<string, object> PlayersFinalScores = new Dictionary<string, object>();

        public List<string> FilterPlayersByGame(Game game, int gameId)
        {
            List<string> playerIds = new List<string>();

            foreach (string playerName in game.PlayerScores.Keys)
            {
                playerIds.Add(playerName);
            }

            return playerIds;
        }

        public void RegisterUserContext(string username)
        {
            if (!usersInLobbyContexts.ContainsKey(username))
            {
                IChatManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IChatManagerCallback>();
                usersInLobbyContexts.Add(username, currentUserCallbackChannel);
            }
        }

        public void UnregisterUserContext(string username)
        {
            lock (usersInLobbyContexts)
            {
                try
                {
                    if (usersInLobbyContexts.ContainsKey(username))
                    {
                        usersInLobbyContexts.Remove(username);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    witslogger.Error(ex);
                }
                catch (KeyNotFoundException ex)
                {
                    witslogger.Error(ex);
                }
                
            }
        }

        public void SendNewMessage(string message, int gameId)
        {
            OperationContext currentContext = OperationContext.Current;

            if (currentContext == null)
            {
                return;
            }

            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                List<string> playerIds = FilterPlayersByGame(game, gameId);

                foreach (string userInGame in playerIds)
                {
                    try
                    {
                        usersInLobbyContexts[userInGame].UpdateChat(message);
                    }
                    catch (TimeoutException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                    }
                    catch (CommunicationException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                    }
                    catch (InvalidOperationException ex)
                    {
                        witslogger.Error(ex);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        witslogger.Error(ex);
                    }
                }
            }
        }

        private List<int> GenerateRandomQuestionIds()
        {
            List<int> questionIds = new List<int>();
            Random randomNum = new Random();

            while (questionIds.Count < 80)
            {
                int newQuestionId = randomNum.Next(1, 150); 

                if (!questionIds.Contains(newQuestionId))
                {
                    questionIds.Add(newQuestionId);
                }
            }

            return questionIds;
        }

        public List<int> GetQuestionIds(int gameId)
        {
            Game game = games.FirstOrDefault(g => g.GameId == gameId);

            if (game != null)
            {
                return game?.QuestionIds;
            }
            else
            {
                return new List<int>();
            }
            
        }

        public void StartGame(int gameId)
        {
            OperationContext currentContext = OperationContext.Current;

            if (currentContext == null)
            {
                return;
            }

            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                List<int> questionIds = GenerateRandomQuestionIds();
                List<string> playerIds = FilterPlayersByGame(game, gameId);
                
                game.QuestionIds = questionIds;
                game.GameStatus = 1;

                foreach (string userInGame in playerIds)
                {
                    try
                    {
                        usersInLobbyContexts[userInGame].StartGamePage();
                    }
                    catch (TimeoutException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (CommunicationException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        witslogger.Error(ex);
                    }
                }
            }
        }

        public void RegisterUserInGameContext(string username)
        {
            if (!usersInGameContexts.ContainsKey(username))
            {
                IActiveGameCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IActiveGameCallback>();
                usersInGameContexts.Add(username, currentUserCallbackChannel);
            }
        }

        public void UnregisterUserInGameContext(string username)
        {
            lock (usersInGameContexts)
            {
                try
                {
                    if (usersInGameContexts.ContainsKey(username))
                    {
                        usersInGameContexts.Remove(username);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    witslogger.Error(ex);
                }
                catch (KeyNotFoundException ex)
                {
                    witslogger.Error(ex);
                }
                
            }
        }

        public List<string> GetPlayersOfGameExceptLeader(int gameId, string leaderUser)
        {
            List<string> players = new List<string>();

            try
            {
                OperationContext currentContext = OperationContext.Current;

                if (currentContext == null)
                {
                    return players;
                }

                Game game = games.FirstOrDefault(g => g.GameId == gameId);

                if (game != null)
                {
                    foreach (string player in game.PlayerScores.Keys)
                    {
                        players.Add(player);
                    }
                    players.Remove(leaderUser);
                }
            }
            catch (InvalidOperationException ex)
            {
                witslogger.Error(ex);
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }

            return players;
        }

        public void SavePlayerAnswer(int playerNumber, string answer, int gameId)
        {
            playerAnswers[playerNumber] = answer;

            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                List<string> playerIds = FilterPlayersByGame(game, gameId);

                foreach (string userInGame in playerIds)
                {
                    try
                    {
                        usersInGameContexts[userInGame].UpdateAnswers(playerAnswers);
                    }
                    catch (TimeoutException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (CommunicationException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (InvalidOperationException ex)
                    {
                        witslogger.Error(ex);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        witslogger.Error(ex);
                    }
                }
            }
        }

        public void ReceivePlayerSelectedAnswer(int playerNumber, int selectedAnswer, int idProfilePicture, int gameId)
        {
            playerSelectedAnswers[playerNumber] = new PlayerSelectedAnswer { SelectedAnswer = selectedAnswer, IdProfilePicture = idProfilePicture };

            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                List<string> playerIds = FilterPlayersByGame(game, gameId);

                foreach (string userInGame in playerIds)
                {
                    try
                    {
                        usersInGameContexts[userInGame].UpdateSelection(playerSelectedAnswers);
                    }
                    catch (TimeoutException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (CommunicationException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        witslogger.Error(ex);
                    }
                }
            }
        }

        public void ReadyToWager(int gameId, int playerNumber, bool isReady)
        {
            OperationContext currentContext = OperationContext.Current;

            if (currentContext == null)
            {
                return;
            }

            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                List<string> playerIds = FilterPlayersByGame(game, gameId);

                foreach (string userInGame in playerIds)
                {
                    try
                    {
                        game.PlayerReadyToWagerStatus[playerNumber] = isReady;

                        bool allPlayersReady = game.PlayerReadyToWagerStatus.All(pair => pair.Value);

                        if (allPlayersReady)
                        {
                            usersInGameContexts[userInGame].ShowEnterWager();
                        }
                    }
                    catch (TimeoutException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (CommunicationException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (InvalidOperationException ex)
                    {
                        witslogger.Error(ex);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        witslogger.Error(ex);
                    }

                }
            }
        }

        public void ExpelPlayer(string username)
        {
            OperationContext currentContext = OperationContext.Current;

            if (currentContext == null)
            {
                return;
            }

            try
            {
                usersInGameContexts[username].BeExpelled();
            }
            catch (TimeoutException ex)
            {
                witslogger.Error(ex);
                RemoveFromEverywhere(username);
            }
            catch (CommunicationException ex)
            {
                witslogger.Error(ex);
                RemoveFromEverywhere(username);
            }
            catch (KeyNotFoundException ex)
            {
                witslogger.Error(ex);
            }
        }

        public void ReadyToShowAnswer(int gameId, int playerNumber, bool isReady)
        {
            OperationContext currentContext = OperationContext.Current;

            if (currentContext == null)
            {
                return;
            }

            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                List<string> playerIds = FilterPlayersByGame(game, gameId);

                foreach (string userInGame in playerIds)
                {
                    try
                    {
                        game.PlayerHasWageredStatus[playerNumber] = isReady;

                        bool allPlayersReady = game.PlayerHasWageredStatus.All(pair => pair.Value);

                        if (allPlayersReady)
                        {
                            usersInGameContexts[userInGame].ShowTrueAnswer();
                        }
                    }
                    catch (TimeoutException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (CommunicationException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        witslogger.Error(ex);
                    }
                }
            }
        }

        public void WhoWon(int gameId, int numberPlayer, string userName, int idCelebration, int score, int idProfilePicture)
        {
            OperationContext currentContext = OperationContext.Current;

            if (currentContext == null)
            {
                return;
            }

            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                List<string> playerIds = FilterPlayersByGame(game, gameId);

                foreach (string userInGame in playerIds)
                {

                    Dictionary<string, object> playerInfo = new Dictionary<string, object>
                    {
                        { "NumberPlayer", numberPlayer },
                        { "UserName", userName },
                        { "IdCelebration", idCelebration },
                        { "Score", score },
                        { "IdProfilePicture", idProfilePicture }
                    };

                    try
                    {
                        PlayersFinalScores.Add($"Player{numberPlayer}", playerInfo);
                    }
                    catch (ArgumentException ex)
                    {
                        witslogger.Error(ex);
                    }
                    catch (InvalidOperationException ex)
                    {
                        witslogger.Error(ex);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        witslogger.Error(ex);
                    }
                }
            }
        }

        public void ShowWinner(int gameId)
        {
            OperationContext currentContext = OperationContext.Current;

            if (currentContext == null)
            {
                return;
            }

            int maxScore = int.MinValue;
            List<Dictionary<string, object>> winnersInfo = new List<Dictionary<string, object>>();

            foreach (var playerInfo in PlayersFinalScores.Values)
            {
                int score = (int)((Dictionary<string, object>)playerInfo)["Score"];

                if (score > maxScore)
                {
                    maxScore = score;
                    winnersInfo.Clear();
                    winnersInfo.Add((Dictionary<string, object>)playerInfo);
                }
                else if (score == maxScore)
                {
                    winnersInfo.Add((Dictionary<string, object>)playerInfo);
                }
            }

            Game game = games.FirstOrDefault(g => g.GameId == gameId);

            if (game != null)
            {
                List<string> playerIds = FilterPlayersByGame(game, gameId);

                foreach (string userInGame in playerIds)
                {
                    try
                    {
                        if (winnersInfo.Count > 1)
                        {
                            usersInGameContexts[userInGame].TieBreaker();
                        }

                        if (winnersInfo.Count < 2)
                        {
                            usersInGameContexts[userInGame].ShowVictoryScreen(
                                (string)winnersInfo[0]["UserName"],
                                (int)winnersInfo[0]["IdProfilePicture"],
                                (int)winnersInfo[0]["IdCelebration"],
                                (int)winnersInfo[0]["Score"]
                            );

                            using (var context = new WitsAndWagersEntities())
                            {
                                context.Database.Log = Console.WriteLine;
                                try
                                {
                                    foreach (var winnerInfo in winnersInfo)
                                    {
                                        string username = (string)winnerInfo["UserName"];

                                        var player = context.Players.Find(username);

                                        if (player != null)
                                        {
                                            Player highestScorePlayer = new Player
                                            {
                                                HighestScore = (int)player.highestScore,
                                            };


                                            if (highestScorePlayer.HighestScore < (int)winnerInfo["Score"])
                                            {
                                                context.Database.Log = Console.WriteLine;

                                                player.highestScore = (int)winnerInfo["Score"];
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                }
                                catch (EntityException ex)
                                {
                                    witslogger.Error(ex);
                                }
                                catch (DataException ex)
                                {
                                    witslogger.Error(ex);
                                }
                                catch (SqlException ex)
                                {
                                    witslogger.Error(ex);
                                }
                            }
                        }
                    }
                    catch (TimeoutException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (CommunicationException ex)
                    {
                        witslogger.Error(ex);
                        RemoveFromEverywhere(userInGame);
                        RemovePlayerInGame(gameId, userInGame);
                    }
                    catch (InvalidOperationException ex)
                    {
                        witslogger.Error(ex);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        witslogger.Error(ex);
                    }
                }
            }

            winnersInfo.Clear();
            PlayersFinalScores.Clear();
            playerAnswers.Clear();
            playerSelectedAnswers.Clear();
        }

        public void GameEnded(int gameId, int playerNumber, bool isRegistered)
        {
            OperationContext currentContext = OperationContext.Current;

            if (currentContext == null)
            {
                return;
            }

            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                List<string> playerIds = FilterPlayersByGame(game, gameId);

                foreach (string userInGame in playerIds)
                {
                    try
                    {
                        game.PlayerEnded[playerNumber] = isRegistered;

                        bool allPlayersReady = game.PlayerEnded.All(pair => pair.Value);

                        if (allPlayersReady)
                        {
                            ShowWinner(gameId);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        witslogger.Error(ex);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        witslogger.Error(ex);
                    }
                    
                }
            }
        }

        public void CleanWinners(int gameId)
        {
            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                List<string> playerIds = FilterPlayersByGame(game, gameId);

                foreach (string userInGame in playerIds)
                {
                    try
                    {
                        PlayersFinalScores.Clear();
                    }
                    catch (InvalidOperationException ex)
                    {
                        witslogger.Error(ex);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        witslogger.Error(ex);
                    }
                }
            }
        }
    }
}
       