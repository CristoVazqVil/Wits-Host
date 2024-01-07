using System.Collections.Generic;
using System.ServiceModel;

namespace WitsClasses.Contracts
{
    [ServiceContract(CallbackContract = typeof(IConnectedUsersCallback))]
    public interface IConnectedUsers
    {
        [OperationContract(IsOneWay = true)]
        void AddConnectedUser(string username);

        [OperationContract(IsOneWay = true)]
        void AddConnectedUserInMenu(string username);

        [OperationContract(IsOneWay = true)]
        void RemoveConnectedUserInMenu(string username);

        [OperationContract(IsOneWay = true)]
        void RemoveConnectedUser(string username);

        [OperationContract]
        void RemoveFromEverywhere(string user);

        [OperationContract]
        void UpdateFriendsForAll();

        [OperationContract]
        List<string> GetConnectedFriends(string principalPlayer);
    }

    [ServiceContract]
    public interface IConnectedUsersCallback
    {
        [OperationContract]
        void UpdateConnectedFriends();
    }
}
