using System.Collections.Generic;
using System.ServiceModel;

namespace WitsClasses.Contracts
{
    [ServiceContract]
    public interface IConnectedUsers
    {
        [OperationContract]
        void AddConnectedUser(string username);

        [OperationContract]
        List<string> GetConnectedUsers();

        
        [OperationContract]
        void RemoveConnectedUser(string username);

        [OperationContract]
        string GetCurrentlyLoggedInUser();

        [OperationContract]
        List<string> GetConnectedFriends(string principalPlayer, List<string> allConnectedUsers);
    }
}
