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

namespace WitsClasses
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class PlayerManager : IPlayerManager, IConnectedUsers, IGameManager
    {
        private const int PENDING = 0;
        private const int ACCEPTED = 1;
        private const int REJECTED = 2;
        private static readonly ILog witslogger = LogManager.GetLogger(typeof(PlayerManager));
        private Dictionary<string, IConnectedUsersCallback> connectedUsersInMenu = new Dictionary<string, IConnectedUsersCallback>();
        private Dictionary<string, IChatManagerCallback> usersInLobbyContexts = new Dictionary<string, IChatManagerCallback>();
        private List<string> allConnectedUsers = new List<string>();
        private static PlayerManager instance;
        private List<Game> games = new List<Game>();

        private PlayerManager()
        {
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
                catch (DbUpdateException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                    return affectedTables;
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
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                    affectedRows = 0;
                    return affectedRows;
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
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                    affectedTables = 0;
                    return affectedTables;
                }
            }

            return affectedTables;
        }

        public void AddConnectedUser(string username)
        {
            if (!connectedUsersInMenu.ContainsKey(username))
            {
                IConnectedUsersCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IConnectedUsersCallback>();
                connectedUsersInMenu.Add(username, currentUserCallbackChannel);
                connectedUsersInMenu[username].UpdateConnectedFriends();
            }

            if (!allConnectedUsers.Contains(username))
            {
                allConnectedUsers.Add(username);

                try
                {
                    foreach (string connectedUser in allConnectedUsers)
                    {
                        connectedUsersInMenu[connectedUser].UpdateConnectedFriends();
                    }

                }
                catch (Exception ex)
                {
                    witslogger.Error(ex);
                }
            }
        }

        public void RemoveConnectedUserInMenu(string username)
        {
            lock (connectedUsersInMenu)
            {
                if (connectedUsersInMenu.ContainsKey(username))
                {
                    connectedUsersInMenu.Remove(username);
                }
            }
        }

        public void RemoveConnectedUser(string username)
        {
            allConnectedUsers.Remove(username);
            lock (connectedUsersInMenu)
            {
                if (connectedUsersInMenu.ContainsKey(username))
                {
                    connectedUsersInMenu.Remove(username);
                }
            }

            try
            {
                foreach (string connectedUser in allConnectedUsers)
                {
                    connectedUsersInMenu[connectedUser].UpdateConnectedFriends();
                }

            }
            catch (Exception ex)
            {
                witslogger.Error(ex);
            }
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
                    witslogger.Error(ex);
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
                    witslogger.Error(ex);
                    return null;
                }
            }
        }

        public List<string> GetPlayerFriends(string playerUsername)
        {
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var friendUsernames = context.Friends
                        .Where(f => f.principalPlayer == playerUsername)
                        .Select(f => f.friend)
                        .ToList();

                    return friendUsernames;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    return new List<string>();
                }
            }
        }

        public List<string> GetAllPlayerRequests(string playerUsername)
        {
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var requests = context.Notifications
                        .Where(n => n.receiverPlayer == playerUsername && n.notificationState == PENDING)
                        .Select(n => n.senderPlayer)
                        .ToList();

                    return requests;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    return new List<string>();
                }
            }
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

                    if (request != null)
                    {
                        found = "Found";
                    }

                    return found;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    return null;
                }
            }
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
                    return affectedTables;
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
                    return affectedTables;
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
                    return affectedTables;
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
                    return affectedTables;
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
                        return affectedTables;
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
                        return affectedTables;
                    }
                }
            }

            return affectedTables;
        }

        public bool IsPlayerBlocked(string player, string blockedPlayer)
        {
            using (var context = new WitsAndWagersEntities())
            {
                try
                {
                    bool isBlocked = context.BlockedPlayers
                        .Any(bp => bp.player == player && bp.blockedPlayer == blockedPlayer);

                    return isBlocked;
                }
                catch (EntityException ex)
                {
                    witslogger.Error(ex);
                    return false;
                }
            }
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
                }
            }

            return affectedTables;
        }

        public bool UpdateProfilePicture(string username, int profilePictureId)
        {
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
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return false;
        }


        public bool UpdateCelebration(string username, int celebrationId)
        {
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
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return false;
        }

        public Question GetQuestionByID(int questionId)
        {
            using (var context = new WitsAndWagersEntities())
            {
                context.Database.Log = Console.WriteLine;
                try
                {
                    var question = context.Questions.Find(questionId);
                    Question foundQuestion = new Question();

                    if (question != null)
                    {
                        foundQuestion.questionES = question.questionES;
                        foundQuestion.questionEN = question.questionEN;
                        foundQuestion.answerES = question.answerES;
                        foundQuestion.answerEN = question.answerEN;
                        foundQuestion.trueAnswer = (int)question.trueAnswer;
                    }
                    return foundQuestion;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    return null;
                }
            }
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

                    return connectedFriends;
                }
                catch (SqlException ex)
                {
                    witslogger.Error(ex);
                    return null;
                }
            }
        }

        public void CreateGame(int gameId, string gameLeader)
        {
            if (games.Any(g => g.GameId == gameId))
            {
                throw new ApplicationException("Used ID");
            }

            var newGame = new Game(gameId, gameLeader, 1);
            newGame.PlayerScores.Add(gameLeader, 0);
            newGame.PlayerAnswers.Add(newGame.numberOfPlayers, "");

            games.Add(newGame);
        }

        public int JoinGame(int gameId, string playerId)
        {
            int returnValue = 0;
            var game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                if (!game.PlayerScores.ContainsKey(playerId))
                {
                    if (game.numberOfPlayers < 4)
                    {
                        game.numberOfPlayers = game.numberOfPlayers + 1;
                        game.PlayerScores.Add(playerId, 0);
                        game.PlayerAnswers.Add(game.numberOfPlayers, "");
                        returnValue = game.numberOfPlayers;
                    }
                }
                return returnValue;
            }
            else
            {
                return returnValue;
            }
        }

        public Dictionary<string, int> GetScores(int gameId)
        {
            var game = games.FirstOrDefault(g => g.GameId == gameId);
            return game?.PlayerScores;
        }

        public void ModifyScore(int gameId, string playerId, int points)
        {
            var game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game?.PlayerScores.ContainsKey(playerId) == true)
            {
                game.PlayerScores[playerId] += points;
            }
        }

        public int GetPlayerScore(int gameId, string playerId)
        {
            var game = games.First(g => g.GameId == gameId);
            return game.PlayerScores[playerId];
        }

        public string GetGameLeader(int gameId)
        {
            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            return game.GameLeader;
        }

        
    }

    public partial class PlayerManager : IChatManager
    {
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
                if (usersInLobbyContexts.ContainsKey(username))
                {
                    usersInLobbyContexts.Remove(username);
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
                    catch (Exception ex)
                    {
                        witslogger.Error(ex);
                    }
                }
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
                List<string> playerIds = FilterPlayersByGame(game, gameId);

                foreach (string userInGame in playerIds)
                {
                    try
                    {
                        usersInLobbyContexts[userInGame].StartGamePage();
                    }
                    catch (Exception ex)
                    {
                        witslogger.Error(ex);
                    }

                }

            }
        }

    }
}
