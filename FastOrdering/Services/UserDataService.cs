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
        public UserOrder _current;
        public ObservableCollection<UserOrder> allItems;
        private static UserDataService instance;

        public static UserDataService GetInstance()
        {
            if (instance == null)
            {
                instance = new UserDataService();
            }
            return instance;
        }

        private UserDataService() {
            _current = new UserOrder();
            //allItems = new ObservableCollection<UserOrder>();
            allItems = UserOrderSQLManagement.GetInstance().allItems;
        }

        public static UserDataService GetInstance()
        {
            if (instance == null)
            {
                instance = new UserDataService();
            }
            return instance;
        }
    }
}
