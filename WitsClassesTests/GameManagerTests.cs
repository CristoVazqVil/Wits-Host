using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WitsClasses;
using WitsClasses.Contracts;

namespace WitsClassesTests
{
    [TestClass]
    public class GameManagerTests
    {
        [TestMethod]
        public void JoinGameSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            int expected = 2;

            // Act
            int result = manager.JoinGame(12345, "test");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JoinGameFailedNotExistingGame()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;

            // Act
            int result = manager.JoinGame(12345, "test");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JoinGameFailedInvalid()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;

            // Act
            int result = manager.JoinGame(0, "");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void JoinGameFailedFull()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            manager.JoinGame(12345, "test");
            manager.JoinGame(12345, "test2");
            manager.JoinGame(12345, "test3");
            int expected = 0;

            // Act
            int result = manager.JoinGame(12345, "test4");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemovePlayerInGameSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            manager.JoinGame(12345, "test");
            int expected = 1;

            // Act
            int result = manager.RemovePlayerInGame(12345, "test");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemovePlayerInGameFailedNotInGamePlayer()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            manager.JoinGame(12345, "test");
            manager.JoinGame(12345, "test2");
            int expected = 0;

            // Act
            int result = manager.RemovePlayerInGame(12345, "test4");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemovePlayerInGameFailedNotExistingGame()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;

            // Act
            int result = manager.RemovePlayerInGame(12345, "test4");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RemovePlayerInGameFailedInvalid()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            int expected = 0;

            // Act
            int result = manager.RemovePlayerInGame(0, "");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPlayerScoreSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            manager.ModifyScore(12345, "CrisCris", 45);
            int expected = 45;

            // Act
            int result = manager.GetPlayerScore(12345, "CrisCris");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPlayerScoreFailedNotExistingGame()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            int expected = 0;

            // Act
            int result = manager.GetPlayerScore(7777, "CrisCris");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPlayerScoreFailedNotInGamePlayer()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            int expected = 0;

            // Act
            int result = manager.GetPlayerScore(12345, "test");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetPlayerScoreFailedInvalid()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            int expected = 0;

            // Act
            int result = manager.GetPlayerScore(0, "");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetGameLeaderSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            string expected = "CrisCris";

            // Act
            string result = manager.GetGameLeader(12345);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetGameLeaderFailedNotExistingGame()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            string expected = null;

            // Act
            string result = manager.GetGameLeader(77777);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetGameLeaderFailedInvalid()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            manager.CreateGame(12345, "CrisCris");
            string expected = null;

            // Act
            string result = manager.GetGameLeader(0);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetQuestionByIdSuccess()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            Question expectedQuestion = new Question();
            int questionToGet = 2;
            expectedQuestion.QuestionES = "¿Cuántos lados tiene un dodecágono?";
            expectedQuestion.QuestionEN = "How many sides does a dodecagon have?";
            expectedQuestion.AnswerES = "Un dodecágono tiene 12 lados, su nombre hace referencia al número de lados";
            expectedQuestion.AnswerEN = "A dodecagon has 12 sides, its name refers to the number of sides";
            expectedQuestion.TrueAnswer = 12;

            // Act
            Question result = manager.GetQuestionByID(questionToGet);

            // Assert
            Assert.AreEqual(expectedQuestion, result);
        }

        [TestMethod]
        public void GetQuestionByIdFailedNotExistingQuestion()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            Question expectedQuestion = new Question();
            int questionToGet = 20000;

            // Act
            Question result = manager.GetQuestionByID(questionToGet);

            // Assert
            Assert.AreEqual(expectedQuestion, result);
        }

        [TestMethod]
        public void GetQuestionByIdFailedInvalid()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            Question expectedQuestion = new Question();
            int questionToGet = 0;

            // Act
            Question result = manager.GetQuestionByID(questionToGet);

            // Assert
            Assert.AreEqual(expectedQuestion, result);
        }

        [TestMethod]
        public void GetQuestionByIdException()
        {
            // Arrange
            PlayerManager manager = new PlayerManager();
            Question expectedQuestion = new Question();
            int questionToGet = 89705;

            // Act
            Question result = manager.GetQuestionByID(questionToGet);

            // Assert
            Assert.AreEqual(expectedQuestion, result);
        }

    }
}
