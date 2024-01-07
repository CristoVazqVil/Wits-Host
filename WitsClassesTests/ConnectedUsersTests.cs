using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WitsClasses;
using WitsClassesTests.WitsService;
using System.ServiceModel;
using Xunit;

namespace WitsClassesTests
{
    public class ConnectedUsersTests : IDisposable
    {
        /// <seealso cref="WitsService.IConnectedUsers"/>
        public static ConnectedUsersClient proxyConnectedUsers;
        public static ConnectedUsersCallbackImplementation connectedUsersCallbackImplementation;

        public ConnectedUsersTests()
        {
            connectedUsersCallbackImplementation = new ConnectedUsersCallbackImplementation();
            proxyConnectedUsers = new ConnectedUsersClient(new InstanceContext(connectedUsersCallbackImplementation));
        }
        public void Dispose()
        {
            proxyConnectedUsers.RemoveConnectedUser("CrisCris");
        }

        [Fact]
        public async void UpdateConnectedFriendsSuccess()
        {
            proxyConnectedUsers.AddConnectedUserInMenu("CrisCris");

            await Task.Delay(2000);
            Assert.True(connectedUsersCallbackImplementation.IsUpdated);
        }

        [Fact]
        public async void UpdateConnectedFriendsException()
        {
            proxyConnectedUsers.RemoveConnectedUser("Test");

            await Task.Delay(2000);
            Assert.False(connectedUsersCallbackImplementation.IsUpdated);
        }
    }

    public class ConnectedUsersCallbackImplementation : IConnectedUsersCallback
    {
        public bool IsUpdated { get; set; }

        public ConnectedUsersCallbackImplementation() { 
            IsUpdated = false;
        }
        public void UpdateConnectedFriends()
        {
            IsUpdated = true;
        }
    }
}
