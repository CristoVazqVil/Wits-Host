﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;
using WitsClasses;
using WitsClasses.Contracts;

namespace WitsClassesTests
{
    [TestClass]
    public class PlayerManagerTests
    {
        [TestMethod]
        public void AddPlayerSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 1;
            Player newPlayer = new Player();
            newPlayer.Username = "UnitTest2";
            newPlayer.Email = "unitTest@gmail.com";
            newPlayer.UserPassword = "UnitPassword";
            newPlayer.HighestScore = 0;
            newPlayer.ProfilePictureId = 1;
            newPlayer.CelebrationId = 1;

            // Act
            int result = manager.AddPlayer(newPlayer);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AddPlayerFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            Player newPlayer = new Player();
            newPlayer.Username = "CrisCris";
            newPlayer.Email = "Unitemail@hmail.com";
            newPlayer.UserPassword = "Password";
            newPlayer.HighestScore = 0;
            newPlayer.ProfilePictureId = 1;
            newPlayer.CelebrationId = 1;

            // Act
            int result = manager.AddPlayer(newPlayer);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AddPlayerException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            Player newPlayer = new Player();
            newPlayer.Username = null;
            newPlayer.Email = null;
            newPlayer.UserPassword = null;
            newPlayer.HighestScore = 0;
            newPlayer.ProfilePictureId = 1;
            newPlayer.CelebrationId = 1;

            // Act
            int result = manager.AddPlayer(newPlayer);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeletePlayerSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 1;
            string playerToDelete = "UnitTest";

            // Act
            int result = manager.DeletePlayer(playerToDelete);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeletePlayerFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string playerToDelete = "NotExistingPlayer";

            // Act
            int result = manager.DeletePlayer(playerToDelete);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeletePlayerException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string playerToDelete = "NotExistingPlayer";

            // Act
            int result = manager.DeletePlayer(playerToDelete);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPlayerByUserSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            Player expectedPlayer = new Player();
            string playerToGet = "UnitTest";
            expectedPlayer.Username = "UnitTest";
            expectedPlayer.Email = "unitTest@gmail.com";
            expectedPlayer.UserPassword = "UnitPassword";
            expectedPlayer.HighestScore = 0;
            expectedPlayer.ProfilePictureId = 1;
            expectedPlayer.CelebrationId = 1;

            // Act
            Player result = manager.GetPlayerByUser(playerToGet);

            // Assert
            Assert.AreEqual(expectedPlayer, result);
        }

        [TestMethod]
        public void GetPlayerByUserFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            Player expectedPlayer = null;
            string playerToGet = "NotExistingPlayer";

            // Act
            Player result = manager.GetPlayerByUser(playerToGet);

            // Assert
            Assert.AreEqual(expectedPlayer, result);
        }

        [TestMethod]
        public void GetPlayerByUserException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            Player expectedPlayer = null;
            string playerToGet = "CrisCris";

            // Act
            Player result = manager.GetPlayerByUser(playerToGet);

            // Assert
            Assert.AreEqual(expectedPlayer, result);
        }

        [TestMethod]
        public void GetPlayerByUserAndPasswordSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            Player expectedPlayer = new Player();
            string playerToGet = "UnitTest";
            string userPassword = "UnitPassword";
            expectedPlayer.Username = "UnitTest";
            expectedPlayer.Email = "unitTest@gmail.com";
            expectedPlayer.UserPassword = "UnitPassword";
            expectedPlayer.HighestScore = 0;
            expectedPlayer.ProfilePictureId = 1;
            expectedPlayer.CelebrationId = 1;

            // Act
            Player result = manager.GetPlayerByUserAndPassword(playerToGet, userPassword);

            // Assert
            Assert.AreEqual(expectedPlayer, result);
        }

        [TestMethod]
        public void GetPlayerByUserAndPasswordFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            Player expectedPlayer = null;
            string playerToGet = "UnitTest";
            string userPassword = "password";

            // Act
            Player result = manager.GetPlayerByUserAndPassword(playerToGet, userPassword);

            // Assert
            Assert.AreEqual(expectedPlayer, result);
        }

        [TestMethod]
        public void GetPlayerByUserAndPasswordException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            Player expectedPlayer = null;
            string playerToGet = "UnitTest";
            string userPassword = "UnitPassword";

            // Act
            Player result = manager.GetPlayerByUserAndPassword(playerToGet, userPassword);

            // Assert
            Assert.AreEqual(expectedPlayer, result);
        }

        [TestMethod]
        public void UpdatePasswordSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 1;
            string username = "UnitTest";
            string passwordToUpdate = "password";

            // Act
            int result = manager.UpdatePassword(username, passwordToUpdate);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdatePasswordFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string username = "NotExisting";
            string passwordToUpdate = "password";

            // Act
            int result = manager.UpdatePassword(username, passwordToUpdate);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdatePasswordException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string username = "UnitTest";
            string passwordToUpdate = "password";

            // Act
            int result = manager.UpdatePassword(username, passwordToUpdate);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AddRequestSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 1;
            string from = "UnitTest";
            string to = "melus";

            // Act
            int result = manager.AddRequest(from, to);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AddRequestFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string from = "UnitTest";
            string to = "UnitTest452";

            // Act
            int result = manager.AddRequest(from, to);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AddRequestException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string from = "UnitTest";
            string to = "UnitTest2";

            // Act
            int result = manager.AddRequest(from, to);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPlayerRequestSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            string expected = "Found";
            string from = "UnitTest";
            string to = "UnitTest2";

            // Act
            string result = manager.GetPlayerRequest(from, to);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPlayerRequestFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            string expected = null;
            string from = null;
            string to = null;

            // Act
            string result = manager.GetPlayerRequest(from, to);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPlayerRequestException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            string expected = null;
            string from = "UnitTest";
            string to = "UnitTest888";

            // Act
            string result = manager.GetPlayerRequest(from, to);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AcceptRequestSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 1;
            string receiver = "UnitTest2";
            string sender = "UnitTest";

            // Act
            int result = manager.AcceptRequest(receiver, sender);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AcceptRequestFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string receiver = "UnitTest256";
            string sender = "UnitTest45";

            // Act
            int result = manager.AcceptRequest(receiver, sender);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AcceptRequestException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string receiver = "UnitTest985";
            string sender = "UnitTest653";

            // Act
            int result = manager.AcceptRequest(receiver, sender);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RejectRequestSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 1;
            string receiver = "melus";
            string sender = "UnitTest";

            // Act
            int result = manager.RejectRequest(receiver, sender);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RejectRequestFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string receiver = "UnitTest256";
            string sender = "UnitTest45";

            // Act
            int result = manager.RejectRequest(receiver, sender);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RejectRequestException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string receiver = "UnitTest985";
            string sender = "UnitTest653";

            // Act
            int result = manager.RejectRequest(receiver, sender);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeleteRequestSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 1;
            string sender = "UnitTest";
            string receiver = "melus";
            int status = 2;

            // Act
            int result = manager.DeleteRequest(receiver, sender, status);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeleteRequestFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string sender = "UnitTest";
            string receiver = "CrisCris";
            int status = 1;

            // Act
            int result = manager.DeleteRequest(receiver, sender, status);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeleteRequestException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string sender = "CrisCris";
            string receiver = "melus";
            int status = 2;

            // Act
            int result = manager.DeleteRequest(receiver, sender, status);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AddFriendshipSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 1;
            string player = "UnitTest";
            string friend = "UnitTest2";

            // Act
            int result = manager.AddFriendship(player, friend);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AddFriendshipFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string player = "NotExisting";
            string friend = "NotExisting2";

            // Act
            int result = manager.AddFriendship(player, friend);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void AddFriendshipException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string player = "UnitTest9";
            string friend = "UnitTest7";

            // Act
            int result = manager.AddFriendship(player, friend);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeleteFriendshipSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 1;
            string player = "UnitTest";
            string friend = "UnitTest2";

            // Act
            int result = manager.DeleteFriendship(player, friend);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeleteFriendshipFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string player = "UnitTest678";
            string friend = "UnitTest8947";

            // Act
            int result = manager.DeleteFriendship(player, friend);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeleteFriendshipException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string player = "UnitTest";
            string friend = "UnitTest2";

            // Act
            int result = manager.DeleteFriendship(player, friend);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void BlockPlayerSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 1;
            string player = "melus";
            string playerToBlock = "UnitTest";

            // Act
            int result = manager.BlockPlayer(player, playerToBlock);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void BlockPlayerFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string player = "NotExisting";
            string playerToBlock = "NotExisting2";

            // Act
            int result = manager.BlockPlayer(player, playerToBlock);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void BlockPlayerException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;
            string player = "melus";
            string playerToBlock = "UnitTest";

            // Act
            int result = manager.BlockPlayer(player, playerToBlock);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void IsPlayerBlockedSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = true;
            string player = "melus";
            string blockedPlayer = "UnitTest";

            // Act
            bool result = manager.IsPlayerBlocked(player, blockedPlayer);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void IsPlayerBlockedFailed()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = false;
            string player = "NotExisting";
            string blockedPlayer = "NotExisting45";

            // Act
            bool result = manager.IsPlayerBlocked(player, blockedPlayer);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void IsPlayerBlockedException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = false;
            string player = "melus";
            string blockedPlayer = "UnitTest";

            // Act
            bool result = manager.IsPlayerBlocked(player, blockedPlayer);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdateProfilePictureSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = true;
            string player = "melus";
            int profilePicId = 4;

            // Act
            bool result = manager.UpdateProfilePicture(player, profilePicId);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdateProfilePictureFailedNotExistingPlayer()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = false;
            string player = "notExisting";
            int profilePicId = 4;

            // Act
            bool result = manager.UpdateProfilePicture(player, profilePicId);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdateProfilePictureFailedNotExistingId()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = false;
            string player = "melus";
            int profilePicId = 8888;

            // Act
            bool result = manager.UpdateProfilePicture(player, profilePicId);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdateProfilePictureException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = false;
            string player = "melus4";
            int profilePicId = 4;

            // Act
            bool result = manager.UpdateProfilePicture(player, profilePicId);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdateCelebrationSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = true;
            string player = "melus";
            int celebrationId = 6;

            // Act
            bool result = manager.UpdateCelebration(player, celebrationId);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdateCelebrationFailedNotExistingPlayer()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = false;
            string player = "melus678";
            int celebrationId = 6;

            // Act
            bool result = manager.UpdateCelebration(player, celebrationId);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdateCelebrationFailedNotExistingId()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = false;
            string player = "melus";
            int celebrationId = 6785;

            // Act
            bool result = manager.UpdateCelebration(player, celebrationId);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void UpdateCelebrationException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = false;
            string player = "melus678";
            int celebrationId = 6;

            // Act
            bool result = manager.UpdateCelebration(player, celebrationId);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void IsPlayerLoggedSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            bool expected = true;
            //manager.all
        }
    }
}
