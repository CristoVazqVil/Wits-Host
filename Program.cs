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

                // Obtiene la instancia única de PlayerManager
                var connectedUsersService = WitsClasses.PlayerManager.GetInstance();

                // Ciclo en segundo plano para actualizar la lista cada 10 segundos
                Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(10000); // Espera 10 segundos
                        connectedUsersService.PrintConnectedUsers(); // Imprime la lista de usuarios conectados
                    }
                });

                Console.ReadLine();
            }
        }
    }
}
