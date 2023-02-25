using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breaking.DataBase
{
    public class DataAccess
    {
        public delegate void RefreshListDelegate();
        public static event RefreshListDelegate RefreshList;
        public static List<Client> GetClients()
        {
            return BreakingEntities.GetContext().Clients.ToList();
        }

        public static List<Service> GetServices()
        {
            return BreakingEntities.GetContext().Services.ToList();
        }

        public static List<Gender> GetGenders()
        {
            return BreakingEntities.GetContext().Genders.ToList();
        }

        public static void SaveClient(Client client)
        {
            if (client.ID == 0)
                BreakingEntities.GetContext().Clients.Add(client);
            BreakingEntities.GetContext().SaveChanges();
            RefreshList?.Invoke();
        }

        public static void DeleteClient(Client client)
        {
            BreakingEntities.GetContext().Clients.Remove(client);
            BreakingEntities.GetContext().SaveChanges();
            RefreshList.Invoke();
        }
    }
}
