using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

using FastOrdering.Models;

namespace FastOrdering.Services
{
    // This class holds sample data used by some generated pages to show how they can be used.
    // TODO WTS: Delete this file once your app is using real data.
    public class UserDataService
    {
        public UserOrder _current = new UserOrder();
        public ObservableCollection<UserOrder> allItems = new ObservableCollection<UserOrder>();
        private static UserDataService instance;

        public static UserDataService GetInstance()
        {
            if (instance == null)
            {
                instance = new UserDataService();
            }
            return instance;
        }
        private static IEnumerable<UserOrder> AllOrders()
        {
            // The following is order summary data
            var data = new ObservableCollection<UserOrder>
            {
                new UserOrder
                    {
                        Table = 5,
                        UserNum = 4,
                        Price = 4.3f,
                        Details = "Aaa",
                        Pepper = 3,
                    },
            };

            return data;
        }

        // TODO WTS: Remove this once your MasterDetail pages are displaying real data
        public static async Task<IEnumerable<UserOrder>> GetSampleModelDataAsync()
        {
            await Task.CompletedTask;

            return AllOrders();
        }
    }
}
