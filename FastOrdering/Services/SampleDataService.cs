using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using FastOrdering.Models;
using Windows.UI.Notifications;

namespace FastOrdering.Services
{
    // SampleDataService用于存放菜品集合和临时的菜品数据，还存取首页与购物车的推荐信息
    // TODO WTS: Delete this file once your app is using real data.
    public class SampleDataService
    {
        //临时菜品数据
        public SampleOrder _current = new SampleOrder();
        //菜品集合
        public ObservableCollection<SampleOrder> allItems = new ObservableCollection<SampleOrder>();
        //购物车页面的推荐数据
        public ObservableCollection<SampleOrder> shoppingCartViewItems = new ObservableCollection<SampleOrder>();
        //主页推荐数据
        public ObservableCollection<SampleOrder> mainPageViewItems = new ObservableCollection<SampleOrder>();

        //临时类用于排序的数据
        private ObservableCollection<SampleOrder> tmpItems = new ObservableCollection<SampleOrder>();

        //购物车下方温度提示
        public String tips = "";
        private static SampleDataService instance;
        //记录当前温度
        private int current = 25;
        //单例模式
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
            //网络错误返回
            if (!UserManagement.GetInstance().isInternetConnected)
            {
                this.tips = "不能根据当前温度推荐菜品";
                return;
            }
            String currentTem = UserManagement.GetInstance().currentTemporature;
            int pos = currentTem.IndexOf("℃");
            //截取信息中的温度
            current = int.Parse(currentTem.Substring(0, pos));
            if (current < 15)
            {
                this.tips = "天气转冷，如下菜式吃了不怕冷哦";
            }
            else if (current < 25 && current > 15)
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
            for (int i = 0; i < SampleOrderSQLManagement.GetInstance().allItems.Count; ++i)
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
            //冒泡排序
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
            //从数据库中获得所有菜品数据
            SampleOrderSQLManagement.GetInstance().GetAll();
            for (int i = 0; i < SampleOrderSQLManagement.GetInstance().allItems.Count; ++i)
            {
                mainPageViewItems.Add(SampleOrderSQLManagement.GetInstance().allItems[i]);
            }
            //冒泡排序
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

        public void UpdateTile()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            SampleOrderSQLManagement.GetInstance().GetAll();
            ObservableCollection<SampleOrder> all_item = new ObservableCollection<SampleOrder>();
            for (int i = 0; i < SampleOrderSQLManagement.GetInstance().allItems.Count; ++i)
            {
                all_item.Add(SampleOrderSQLManagement.GetInstance().allItems[i]);
            }
            for (int i = 0; i < all_item.Count; i++)
            {
                for (int j = i + 1; j < all_item.Count; j++)
                {
                    if (all_item[i].Collected < all_item[j].Collected)
                    {
                        SampleOrder tmp = all_item[i];
                        all_item[i] = all_item[j];
                        all_item[j] = tmp;
                    }
                }
            }
            int count = all_item.Count >= 10 ? 10 : all_item.Count;
            for(int i = 0; i < count; i++)
            {
                createTile(all_item[i]);
            }

        }

        private void createTile(SampleOrder item)
        {
            var xmlDoc = TileService.CreateTiles(item);
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            TileNotification notification = new TileNotification(xmlDoc);
            updater.Update(notification);
        }

        //得到购物车推荐列表
        public void GetListView()
        {
            if (!UserManagement.GetInstance().isInternetConnected)
            {
                return;
            }
            SortObservableCollectionByVisited();
            shoppingCartViewItems.Clear();
            //低温推荐热菜
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

                if (shoppingCartViewItems.Count < 10)
                {
                    //将空位填满，菜品足够情况下确保有10个推荐菜品
                    int pos = 0;
                    while (shoppingCartViewItems.Count < 10 && pos < tmpItems.Count)
                    {
                        shoppingCartViewItems.Add(tmpItems[pos]);
                        pos++;
                    }
                }
            }
            else if (current < 25 && current > 15)
            {
                //温度适宜提供热菜
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
                //温度较高提供冷饮
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
            if (shoppingCartViewItems.Count <= 1)
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

            //删除多余的菜品，保留10个
            while (shoppingCartViewItems.Count > 10)
            {
                shoppingCartViewItems.Remove(shoppingCartViewItems[shoppingCartViewItems.Count - 1]);
            }
        }
    }
}
