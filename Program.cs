using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
