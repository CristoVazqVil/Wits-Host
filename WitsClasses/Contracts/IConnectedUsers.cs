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
        void PrintConnectedUsers();
    }
}
