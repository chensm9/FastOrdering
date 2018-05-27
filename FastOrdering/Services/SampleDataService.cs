using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using FastOrdering.Models;

namespace FastOrdering.Services
{
    // This class holds sample data used by some generated pages to show how they can be used.
    // TODO WTS: Delete this file once your app is using real data.
    public class SampleDataService
    {
        public SampleOrder _current = new SampleOrder();
        public ObservableCollection<SampleOrder> allItems = new ObservableCollection<SampleOrder>();
        private static SampleDataService instance;

        public static SampleDataService getInstance()
        {
            if (instance == null)
            {
                instance = new SampleDataService();
            }
            return instance;
        }

        private static IEnumerable<SampleOrder> GetMyOrder()
        {
            // The following is order summary data
            var data = new ObservableCollection<SampleOrder>
            {
                new SampleOrder
                    {
                        OrderName = "寿司",
                        Price = 19.99f,
                        Summary = "ddddddddddddddddddd",
                        Details = "aaa",
                    },
            };

            return data;
        }

        // TODO WTS: Remove this once your MasterDetail pages are displaying real data
        public static async Task<IEnumerable<SampleOrder>> GetSampleModelDataAsync()
        {
            await Task.CompletedTask;

            return GetMyOrder();
        }

        public static ObservableCollection<SampleOrder> GeyMyOrder()
        {
            // The following is order summary data
            var data = new ObservableCollection<SampleOrder>
            {
                new SampleOrder
                    {
                        OrderName = "寿司",
                        Price = 19.99f,
                        Summary = "ddddddddddddddddddd",
                        Details = "aaa",
                    },
                new SampleOrder
                    {
                        OrderName = "寿司",
                        Price = 19.99f,
                        Summary = "ddddddddddddddddddd",
                        Details = "aaa",
                    },

            };
            return data;
        }

        public static async Task<IEnumerable<SampleOrder>> GetMyOrderAsync()
        {
            await Task.CompletedTask;
            return GetMyOrder();
        }
    }
}
