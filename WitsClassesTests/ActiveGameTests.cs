using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WitsClasses;
using WitsClassesTests.WitsService;
using Xunit;

namespace WitsClassesTests
{
    public class ActiveGameTests : IDisposable
    {
        /// <seealso cref="WitsService.IActiveGame"/>
        public static ActiveGameClient proxyActiveGame;
        public static GameManagerClient proxyGame;
        public static ActiveGameCallbackImplementation activeGameCallbackImplementation;
        public static ActiveGameClient proxyActiveGame2;
        public static ActiveGameCallbackImplementation activeGameCallbackImplementation2;

        public ActiveGameTests()
        {
            proxyGame = new GameManagerClient();
            activeGameCallbackImplementation = new ActiveGameCallbackImplementation();
            proxyActiveGame = new ActiveGameClient(new InstanceContext(activeGameCallbackImplementation));

            activeGameCallbackImplementation2 = new ActiveGameCallbackImplementation();
            proxyActiveGame2 = new ActiveGameClient(new InstanceContext(activeGameCallbackImplementation2));

            proxyActiveGame.RegisterUserInGameContext("CrisCris");
            proxyActiveGame2.RegisterUserInGameContext("Test");
            proxyGame.CreateGame(12345, "CrisCris");
        }

        public void Dispose()
        {
            proxyActiveGame.UnregisterUserInGameContext("CrisCris");
            proxyActiveGame.UnregisterUserInGameContext("Test");
        }

        [Fact]
        public async void UpdateAnswersSuccess()
        {
            proxyActiveGame.SavePlayerAnswer(1, "Answer", 12345);

            await Task.Delay(2000);
            Assert.True(activeGameCallbackImplementation.IsCalled);
        }

        [Fact]
        public async void UpdateAnswersFailedNotCalled()
        {
            proxyActiveGame.UnregisterUserInGameContext("CrisCris");

            await Task.Delay(2000);
            Assert.False(activeGameCallbackImplementation.IsCalled);
        }

        [Fact]
        public async void ShowEnteredWagerSuccess()
        {
            proxyActiveGame.ReadyToWager(12345, 1, true);

            await Task.Delay(2000);
            Assert.True(activeGameCallbackImplementation.IsCalled);
        }

        [Fact]
        public async void ShowEnteredWagerNotCalled()
        {
            proxyActiveGame.UnregisterUserInGameContext("CrisCris");

            await Task.Delay(2000);
            Assert.False(activeGameCallbackImplementation.IsCalled);
        }

        [Fact]
        public async void ShowTrueAnswerSuccess()
        {
            proxyActiveGame.ReadyToShowAnswer(12345, 1, true);

            await Task.Delay(2000);
            Assert.True(activeGameCallbackImplementation.IsCalled);
        }

        [Fact]
        public async void ShowTrueAnswerNotCalled()
        {
            proxyActiveGame.UnregisterUserInGameContext("CrisCris");

            await Task.Delay(2000);
            Assert.False(activeGameCallbackImplementation.IsCalled);
        }

        [Fact]
        public async void BeExpelledSuccess()
        {
            proxyActiveGame.ExpelPlayer("CrisCris");

            await Task.Delay(2000);
            Assert.True(activeGameCallbackImplementation.IsCalled);
        }

        [Fact]
        public async void BeExpelledNotCalled()
        {
            proxyActiveGame.UnregisterUserInGameContext("CrisCris");

            await Task.Delay(2000);
            Assert.False(activeGameCallbackImplementation.IsCalled);
        }


        [Fact]
        public async void ShowVictoryScreenNotCalled()
        {
            proxyActiveGame.UnregisterUserInGameContext("CrisCris");

            await Task.Delay(2000);
            Assert.False(activeGameCallbackImplementation.IsCalled);
        }


        [Fact]
        public async void TieBreakerNotCalled()
        {
            proxyActiveGame.UnregisterUserInGameContext("CrisCris");

            await Task.Delay(2000);
            Assert.False(activeGameCallbackImplementation.IsCalled);
        }

        [Fact]
        public async void UpdateSelectionSuccess()
        {
            proxyActiveGame.SavePlayerAnswer(1, "Answer", 12345);
            Dictionary<string, object> answersInfo = new Dictionary<string, object>
            {
                { "playerNumber", 1 },
                { "selectedAnswer", 1 },
                { "profilePictureId", 1 },
                { "gameId", 12345 }
            };

            proxyActiveGame.ReceivePlayerSelectedAnswer(answersInfo);

            await Task.Delay(2000);
            Assert.True(activeGameCallbackImplementation.IsCalled);
        }

        [Fact]
        public async void UpdateSelectionNotCalled()
        {
            proxyActiveGame.UnregisterUserInGameContext("CrisCris");

            await Task.Delay(2000);
            Assert.False(activeGameCallbackImplementation.IsCalled);
        }
    }

    public class ActiveGameCallbackImplementation : IActiveGameCallback
    {
        public bool IsCalled { get; set; }

        public ActiveGameCallbackImplementation()
        {
            IsCalled = false;
        }

        public void UpdateAnswers(Dictionary<int, string> playerAnswers)
        {
            IsCalled = true;
        }

        public void UpdateSelection(Dictionary<int, WitsClasses.Contracts.PlayerSelectedAnswer> playerSelectedAnswers)
        {
            IsCalled = true;
        }

        public void ShowEnterWager()
        {
            IsCalled = true;
        }

        public void ShowTrueAnswer()
        {
            IsCalled = true;
        }

        public void BeExpelled()
        {
            IsCalled = true;
        }

        public void ShowVictoryScreen(Dictionary<string, object> winnerInfo)
        {
            IsCalled = true;
        }

        public void TieBreaker()
        {
            IsCalled = true;
        }
    }
}
