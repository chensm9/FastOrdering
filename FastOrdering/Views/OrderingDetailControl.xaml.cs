using System;

using FastOrdering.Models;
using FastOrdering.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FastOrdering.Views
{
    public sealed partial class OrderingDetailControl : UserControl
    {
        public SampleOrder MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as SampleOrder; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static readonly DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(SampleOrder), typeof(OrderingDetailControl), new PropertyMetadata(null, OnMasterMenuItemPropertyChanged));

        public OrderingDetailControl()
        {
            InitializeComponent();
        }

        private static void OnMasterMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as OrderingDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }

        private void Collected(object sender, RoutedEventArgs e)
        {
            if (!UserManagement.GetInstance().returnMain)
            {
                return;
            }
            //查找该项目的位置
            var instance = SampleDataService.GetInstance();
            int pos = 0; ;
            int id = MasterMenuItem.OrderId;
            for (int i = 0; i < instance.allItems.Count; i++)
            {
                if (id == instance.allItems[i].OrderId)
                {
                    pos = i;
                    break;
                }
            }
            //赞＋1并添加到数据库。新建实例以防止Ordered变量被修改
            instance.allItems[pos].Collected++;
            SampleOrder newOne = instance.allItems[pos];
            newOne.Ordered = 0;
            SampleOrderSQLManagement.GetInstance().update(newOne);
            //防止重复点赞
            UserManagement.GetInstance().returnMain = false;
        }

        private async void AddToCart(object sender, RoutedEventArgs e)
        {
            //去重
            int currentID = MasterMenuItem.OrderId;
            for(int i = 0; i < UserDataService.GetInstance()._current.SampleItems.Count; ++i)
            {
                if (UserDataService.GetInstance()._current.SampleItems[i].OrderId == currentID)
                {
                    if (UserDataService.GetInstance()._current.SampleItems[i].Ordered == 99)
                    {
                        //数量到达上限
                        ContentDialog FailedAdd = new ContentDialog()
                        {
                            Title = "添加错误",
                            Content = "菜品\"" + MasterMenuItem.OrderName + "\"已经存在购物车中，数量大于上限99",
                            PrimaryButtonText = "好"
                        };
                        ContentDialogResult response = await FailedAdd.ShowAsync();
                    }
                    else
                    {
                        //数量加上1
                        ContentDialog NumAdd = new ContentDialog()
                        {
                            Title = "添加到购物车",
                            Content = "菜品\"" + MasterMenuItem.OrderName + "\"已经存在购物车中，数量加1",
                            PrimaryButtonText = "好"
                        };
                        UserDataService.GetInstance()._current.SampleItems[i].Ordered++;
                        Calculate();
                        ContentDialogResult response = await NumAdd.ShowAsync();
                    }
                    return;
                }
            }
            //增加访问量
            var instance = SampleDataService.GetInstance();
            int pos = 0;
            int id = MasterMenuItem.OrderId;
            for (int i = 0; i < instance.allItems.Count; i++)
            {
                if (id == instance.allItems[i].OrderId)
                {
                    pos = i;
                    break;
                }
            }
            //赞＋1并添加到数据库
            instance.allItems[pos].Visited++;
            SampleOrder newOne = instance.allItems[pos];
            newOne.Ordered = 0;
            SampleOrderSQLManagement.GetInstance().update(newOne);

            //正常添加购物车
            MasterMenuItem.Ordered++;
            UserDataService.GetInstance()._current.SampleItems.Add(MasterMenuItem);
            ContentDialog SuccessAdd = new ContentDialog()
            {
                Title = "添加到购物车",
                Content = "已将\"" + MasterMenuItem.OrderName + "\"添加到购物车" ,
                PrimaryButtonText = "好"
            };
            Calculate();
            ContentDialogResult result = await SuccessAdd.ShowAsync();
        }

        //价格计算
        private void Calculate()
        {
            UserDataService.GetInstance()._current.Price = 0;
            for (int i = 0; i < UserDataService.GetInstance()._current.SampleItems.Count; ++i)
            {
                UserDataService.GetInstance()._current.Price += UserDataService.GetInstance()._current.SampleItems[i].Price
                    * UserDataService.GetInstance()._current.SampleItems[i].Ordered;
            }
        }
    }
}
