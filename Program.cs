using System;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WitsClasses.Contracts;
using System.Security.Cryptography;


namespace WitsHost
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("Log4Net.config"));
            using (ServiceHost host = new ServiceHost(typeof(WitsClasses.PlayerManager)))
            {
                host.Open();
                Console.WriteLine("Wits And Wagers is running");

                Console.ReadLine();

            }
           
        }
    }
}
