using System;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WitsClasses.Contracts;

namespace WitsHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(WitsClasses.PlayerManager)))
            {
                host.Open();
                Console.WriteLine("Wits And Wagers is running");

                Console.ReadLine();
            }
        }
    }
}
