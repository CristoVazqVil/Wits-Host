using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WitsClasses.Contracts
{
    [ServiceContract(CallbackContract = typeof(IChatManagerCallback))]
    public interface IChatManager
    {
        [OperationContract(IsOneWay = true)]
        void SendNewMessage(string message, int gameId);

        [OperationContract(IsOneWay = true)]
        void RegisterUserContext(string username);

        [OperationContract(IsOneWay = true)]
        void UnregisterUserContext(string username);

    }

    [ServiceContract]
    public interface IChatManagerCallback
    {
        [OperationContract]
        void UpdateChat(string message);
    }

}
