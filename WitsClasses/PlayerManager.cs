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

            lock (playersnumbers)
            {
                if (playersnumbers.ContainsKey(username))
                {
                    playersnumbers.Remove(username);
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


   

        public int GetRandomQuestionId()
        {
            int newQuestionId;
            do
            {
                newQuestionId = random.Next(1, 16);
            } while (usedQuestionIds.Contains(newQuestionId));

            usedQuestionIds.Add(newQuestionId);

            return newQuestionId;
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
            playersnumbers.Add(gameLeader, newGame.numberOfPlayers);

            games.Add(newGame);
        }

        public int JoinGame(int gameId, string playerId)
        {
            int returnValue = 0;
            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                if (!game.PlayerScores.ContainsKey(playerId))
                {
                    if (game.numberOfPlayers < 4)
                    {
                        game.numberOfPlayers = game.numberOfPlayers + 1;
                        game.PlayerScores.Add(playerId, 0);
                        game.PlayerAnswers.Add(game.numberOfPlayers, "");
                        playersnumbers.Add(playerId, game.numberOfPlayers);
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

        public int RemovePlayerInGame(int gameId, string playerId)
        {
            int returnValue = 0;

            Game game = games.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
                if (game.PlayerScores.ContainsKey(playerId))
                {
                    game.PlayerScores.Remove(playerId);
                    game.numberOfPlayers = game.numberOfPlayers - 1;
                    game.PlayerReadyToWagerStatus.Remove(playersnumbers[playerId]);
                    game.PlayerAnswers.Remove(playersnumbers[playerId]);
                    game.PlayerHasWageredStatus.Remove(playersnumbers[playerId]);
                    usersInGameContexts.Remove(playerId);
                    playersnumbers.Remove(playerId);
                    returnValue = 1;
                }
            }

            return returnValue;
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



        






        private List<int> GenerateRandomQuestionIds()
        {
            List<int> questionIds = new List<int>();
            Random random = new Random();

            // Generar 6 números aleatorios sin repetición
            while (questionIds.Count < 6)
            {
                int newQuestionId = random.Next(1, 17); // El rango debe ser hasta 17 para incluir el 16
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
            return game?.QuestionIds;
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
                // Generar la lista de preguntas
                List<int> questionIds = GenerateRandomQuestionIds();

                // Asignar la lista al contexto del juego
                game.QuestionIds = questionIds;

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
                if (usersInGameContexts.ContainsKey(username))
                {
                    usersInGameContexts.Remove(username);
                }
            }
        }

        public List<string> GetPlayersOfGameExceptLeader(int gameId, string leaderUser)
        {
            OperationContext currentContext = OperationContext.Current;
            List<string> players = new List<string>();

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
            return players;
        }
        public void SavePlayerAnswer(int playerNumber, string answer, int gameId)
        {


            // Aquí puedes realizar cualquier lógica adicional antes de guardar la respuesta en el diccionario
            playerAnswers[playerNumber] = answer;

            // Puedes imprimir el diccionario en la consola si es necesario
          

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
                    catch (Exception ex)
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
                    catch (Exception ex)
                    {
                        witslogger.Error(ex);
                        Console.WriteLine(ex);
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
                    // Crea o actualiza la entrada del jugador en el diccionario de listo/no listo
                    game.PlayerReadyToWagerStatus[playerNumber] = isReady;


                    Console.WriteLine($" JUGADORES: Player {playerNumber}: Ready Status = {isReady}");
                    // Verifica si todos los jugadores están listos
                    bool allPlayersReady = game.PlayerReadyToWagerStatus.All(pair => pair.Value);

                    if (allPlayersReady)
                    {
                        usersInGameContexts[userInGame].ShowEnterWager();
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

            usersInGameContexts[username].BeExpelled();
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
                    // Crea o actualiza la entrada del jugador en el diccionario de listo/no listo
                    game.PlayerHasWageredStatus[playerNumber] = isReady;


                    Console.WriteLine($" JUGADORES: Player {playerNumber}: Ready Status = {isReady}");
                    // Verifica si todos los jugadores están listos
                    bool allPlayersReady = game.PlayerHasWageredStatus.All(pair => pair.Value);

                    if (allPlayersReady)
                    {
                        usersInGameContexts[userInGame].ShowTrueAnswer();
                    }
                }
            }
        }






        public void WhoWon(int gameId, int numberPlayer, string userName, int idCelebration, int score, int idProfilePicture)
        {
            Dictionary<string, object> playerInfo = new Dictionary<string, object>
        {
            { "NumberPlayer", numberPlayer },
            { "UserName", userName },
            { "IdCelebration", idCelebration },
            { "Score", score },
            { "IdProfilePicture", idProfilePicture }
        };

            PlayersFinalScores.Add($"Player{numberPlayer}", playerInfo);
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
                            Console.WriteLine("Hubo un empate:");
                            usersInGameContexts[userInGame].TieBreaker();
                        }
                        else
                        {
                            Console.WriteLine("Ganador:");
                        }

              
                        if (winnersInfo.Count  < 2)
                        {
                            Console.WriteLine("ACTUAL Winner Information:");
                            foreach (var winnerInfo in winnersInfo)
                            {
                                foreach (var kvp in winnerInfo)
                                {
                                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                                }
                                Console.WriteLine();
                            }

                            usersInGameContexts[userInGame].ShowVictoryScreen(
                                (string)winnersInfo[0]["UserName"],
                                (int)winnersInfo[0]["IdProfilePicture"],
                                (int)winnersInfo[0]["IdCelebration"],
                                (int)winnersInfo[0]["Score"]
                            );
                        }
                        else
                        {
                            Console.WriteLine("No winner information available.");
                        }
                    }
                    catch (Exception ex)
                    {
                        witslogger.Error(ex);
                    }
                }
            }
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
                    // Crea o actualiza la entrada del jugador en el diccionario de listo/no listo
                    game.PlayerEnded[playerNumber] = isRegistered;


                    Console.WriteLine($" JUGADORES: Player {playerNumber}: Ended Status = {isRegistered}");
                    // Verifica si todos los jugadores están listos
                    bool allPlayersReady = game.PlayerEnded.All(pair => pair.Value);

                    if (allPlayersReady)
                    {
                        
                        ShowWinner(gameId);
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
                    catch (Exception ex)
                    {
                        witslogger.Error(ex);
                    }
                }
            }

            
        }
    }

    
       /*
        * El problema está en que busca la respuesta cada vez que se llama, si llama un solo jugador el diccionario 
        * tiene un solo jugador
        * 
        * debo de crear el diccionario y después llamo al GameEnded y cuando el gameEnded junte a todos los jugadores
        * Entonces ya busca la respuesta de quién tiene más puntos en el diccionario.
        * 
       */
}
       