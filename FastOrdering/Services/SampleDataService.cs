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
        //购物车页面的推荐
        public ObservableCollection<SampleOrder> shoppingCartViewItems = new ObservableCollection<SampleOrder>();
        //主页推荐
        public ObservableCollection<SampleOrder> mainPageViewItems = new ObservableCollection<SampleOrder>();

        //临时类用于排序
        private ObservableCollection<SampleOrder> tmpItems = new ObservableCollection<SampleOrder>();

        public String tips = "";
        private static SampleDataService instance;
        private int current = 25;

        public static SampleDataService GetInstance()
        {
            if (instance == null)
            {
                instance = new SampleDataService();
            }
            return instance;
        }

        //得到提示
        public void GetTips()
        {
            String currentTem = UserManagement.GetInstance().currentTemporature;
            int pos = currentTem.IndexOf("℃");
            //截取信息中的温度
            current = int.Parse(currentTem.Substring(0, pos));
            if(current < 15)
            {
                this.tips = "天气转冷，如下菜式吃了不怕冷哦";
            }else if (current < 25 && current > 15)
            {
                this.tips = "刚刚好的天气，来点佳肴吧";
            }
            else
            {
                this.tips = "天气炎热，来点冷饮哦";
            }
        }

        //根据字符串查找相符合的菜品
        private void GetItemsAccordingToString(String str, ObservableCollection<SampleOrder> collection)
        {
            SampleOrderSQLManagement.GetInstance().queryItem(str);
            for(int i = 0; i < SampleOrderSQLManagement.GetInstance().allItems.Count; ++i)
            {
                collection.Add(SampleOrderSQLManagement.GetInstance().allItems[i]);
            }
        }

        //根据访问量对ObservableCollection进行排序
        private void SortObservableCollectionByVisited()
        {
            tmpItems.Clear();
            SampleOrderSQLManagement.GetInstance().GetAll();
            for (int i = 0; i < SampleOrderSQLManagement.GetInstance().allItems.Count; ++i)
            {
                tmpItems.Add(SampleOrderSQLManagement.GetInstance().allItems[i]);
            }
            for (int i = 0; i < tmpItems.Count; i++)
            {
                for (int j = i + 1; j < tmpItems.Count; j++)
                {
                    if (tmpItems[i].Visited < tmpItems[j].Visited)
                    {
                        SampleOrder tmp = tmpItems[i];
                        tmpItems[i] = tmpItems[j];
                        tmpItems[j] = tmp;
                    }
                }
            }
        }

        //根据点赞数量对ObservableCollection进行排序
        private void SortObservableCollectionByCollected()
        {
            mainPageViewItems.Clear();
            SampleOrderSQLManagement.GetInstance().GetAll();
            for (int i = 0; i < SampleOrderSQLManagement.GetInstance().allItems.Count; ++i)
            {
                mainPageViewItems.Add(SampleOrderSQLManagement.GetInstance().allItems[i]);
            }
            for (int i = 0; i < mainPageViewItems.Count; i++)
            {
                for (int j = i + 1; j < mainPageViewItems.Count; j++)
                {
                    if (mainPageViewItems[i].Collected < mainPageViewItems[j].Collected)
                    {
                        SampleOrder tmp = mainPageViewItems[i];
                        mainPageViewItems[i] = mainPageViewItems[j];
                        mainPageViewItems[j] = tmp;
                    }
                }
            }
        }

        //获得首页的推荐
        public void GetCollectedListView()
        {
            SortObservableCollectionByCollected();
            //删除多余的推荐
            while (mainPageViewItems.Count > 10)
            {
                mainPageViewItems.Remove(mainPageViewItems[mainPageViewItems.Count - 1]);
            }
        }

        //得到购物车推荐列表
        public void GetListView()
        {
            SortObservableCollectionByVisited();
            shoppingCartViewItems.Clear();
            if (current < 15)
            {

                GetItemsAccordingToString("热", shoppingCartViewItems);
                GetItemsAccordingToString("炸", shoppingCartViewItems);
                GetItemsAccordingToString("煮", shoppingCartViewItems);
                GetItemsAccordingToString("煎", shoppingCartViewItems);
                GetItemsAccordingToString("煲", shoppingCartViewItems);
                GetItemsAccordingToString("炖", shoppingCartViewItems);
                GetItemsAccordingToString("炒", shoppingCartViewItems);
                GetItemsAccordingToString("焖", shoppingCartViewItems);
                GetItemsAccordingToString("爆", shoppingCartViewItems);

                if(shoppingCartViewItems.Count < 10)
                {
                    //将空位填满
                    int pos = 0;
                    while(shoppingCartViewItems.Count < 10 && pos < tmpItems.Count)
                    {
                        shoppingCartViewItems.Add(tmpItems[pos]);
                        pos++;
                    }
                }
            }
            else if (current < 25 && current > 15)
            {
                GetItemsAccordingToString("煮", shoppingCartViewItems);
                GetItemsAccordingToString("煲", shoppingCartViewItems);
                GetItemsAccordingToString("炖", shoppingCartViewItems);
                GetItemsAccordingToString("炒", shoppingCartViewItems);
                GetItemsAccordingToString("焖", shoppingCartViewItems);
                if (shoppingCartViewItems.Count < 10)
                {
                    //将空位填满
                    int pos = 0;
                    while (shoppingCartViewItems.Count < 10 && pos < tmpItems.Count)
                    {
                        shoppingCartViewItems.Add(tmpItems[pos]);
                        pos++;
                    }
                }
            }
            else
            {
                GetItemsAccordingToString("冰", shoppingCartViewItems);
                GetItemsAccordingToString("雪", shoppingCartViewItems);
                GetItemsAccordingToString("汁", shoppingCartViewItems);
                GetItemsAccordingToString("苏打", shoppingCartViewItems);
                GetItemsAccordingToString("凉", shoppingCartViewItems);
                GetItemsAccordingToString("饮", shoppingCartViewItems);
                GetItemsAccordingToString("酸奶", shoppingCartViewItems);
                GetItemsAccordingToString("薄荷", shoppingCartViewItems);
                if (shoppingCartViewItems.Count < 10)
                {
                    //将空位填满
                    int pos = 0;
                    while (shoppingCartViewItems.Count < 10 && pos < tmpItems.Count)
                    {
                        shoppingCartViewItems.Add(tmpItems[pos]);
                        pos++;
                    }
                }
            }

            //去重
            if(shoppingCartViewItems.Count <= 1)
            {
                return;
            }
            for (int i = 0; i < shoppingCartViewItems.Count; i++)
            {
                for (int j = i + 1; j < shoppingCartViewItems.Count; j++)
                {
                    if (shoppingCartViewItems[i].OrderId == shoppingCartViewItems[j].OrderId && i != j)
                    {
                        shoppingCartViewItems.Remove(shoppingCartViewItems[j]);
                        j = i + 1;
                        i = 0;
                    }
                }
            }

            //删除多余的
            while( shoppingCartViewItems.Count > 10)
            {
                shoppingCartViewItems.Remove(shoppingCartViewItems[shoppingCartViewItems.Count - 1]);
            }
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
