using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WitsClassesTests.WitsService;
using Xunit;

namespace WitsClassesTests
{
    public class ChatManagerTests : IDisposable
    {
        /// <seealso cref="WitsService.IChatManager"/>
        public static ChatManagerClient proxyChat;
        public static GameManagerClient proxyGame;
        public static ChatManagerCallbackImplementation chatManagerCallbackImplementation;

        public ChatManagerTests() {
            chatManagerCallbackImplementation = new ChatManagerCallbackImplementation();
            proxyChat = new ChatManagerClient(new InstanceContext(chatManagerCallbackImplementation));
            proxyGame = new GameManagerClient();
            proxyGame.CreateGame(12345, "CrisCris");
            proxyChat.RegisterUserContext("CrisCris");
        }

        public void Dispose()
        {
            proxyChat.UnregisterUserContext("CrisCris");
        }

        [Fact]
        public async void UpdateChatSuccess()
        {
            proxyChat.SendNewMessage("mechach", 12345);

            await Task.Delay(2000);
            Assert.True(chatManagerCallbackImplementation.IsUpdated);
        }

        [Fact]
        public async void UpdateChatFailedNotCalled()
        {
            proxyChat.UnregisterUserContext("CrisCris");

            await Task.Delay(2000);
            Assert.False(chatManagerCallbackImplementation.IsUpdated);
        }

        [Fact]
        public async void StartGamePageSuccess()
        {
            proxyChat.StartGame(12345);

            await Task.Delay(2000);
            Assert.True(chatManagerCallbackImplementation.IsStarted);
        }

        [Fact]
        public async void StartGamePageFailedNotCalled()
        {
            proxyChat.UnregisterUserContext("CrisCris");

            await Task.Delay(2000);
            Assert.False(chatManagerCallbackImplementation.IsStarted);
        }

    }

    public class ChatManagerCallbackImplementation : IChatManagerCallback
    {
        public bool IsUpdated { get; set; }
        public bool IsStarted { get; set; }

        public void StartGamePage()
        {
            IsStarted = true;
        }

        public void UpdateChat(string message)
        {
            IsUpdated = true;
        }
    }
}
