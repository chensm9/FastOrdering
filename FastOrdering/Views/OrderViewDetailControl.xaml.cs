using System;

using FastOrdering.Models;
using FastOrdering.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FastOrdering.Views
{
    public sealed partial class OrderViewDetailControl : UserControl
    {
        UserDataService instance = UserDataService.GetInstance();
        public UserOrder MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as UserOrder; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(UserOrder), typeof(OrderViewDetailControl), new PropertyMetadata(null, OnMasterMenuItemPropertyChanged));

        public OrderViewDetailControl()
        {
            InitializeComponent();
        }


        private static void OnMasterMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as OrderViewDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }

        private async void DeleteItem(object sender, RoutedEventArgs e)
        {
            var originalSource = e.OriginalSource as Button;
            int currentID = (int)originalSource.DataContext;
            ContentDialog ErrorDialog = new ContentDialog
            {
                Title = "上菜",
                Content = "你将会从列表中删除该菜品",
                PrimaryButtonText = "取消",
                SecondaryButtonText = "确认上菜"
            };
            ContentDialogResult result = await ErrorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return;
            //利用id的唯一性，寻找该菜品在菜单中的位置
            for (int i = 0; i < MasterMenuItem.SampleItems.Count; ++i)
            {
                if (MasterMenuItem.SampleItems[i].OrderId == currentID)
                {
                    MasterMenuItem.SampleItems.Remove(MasterMenuItem.SampleItems[i]);
                }
            }
            if (MasterMenuItem.SampleItems.Count == 0)
                instance.allItems.Remove(MasterMenuItem);
            UserOrderSQLManagement.GetInstance().delete(MasterMenuItem.OrderId, currentID);
        }

        //结账
        private async void SettleAccount(object sender, RoutedEventArgs e)
        {
            ContentDialog ErrorDialog = new ContentDialog
            {
                Title = "结账",
                Content = "结账后你将会删除这个订单",
                PrimaryButtonText = "取消",
                SecondaryButtonText = "确认结账"
            };
            ContentDialogResult result = await ErrorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return;
            UserOrderSQLManagement.GetInstance().delete(MasterMenuItem.OrderId);
            instance.allItems.Remove(MasterMenuItem);
        }

        //删除订单
        private async void DeleteOrder(object sender, RoutedEventArgs e)
        {
            ContentDialog ErrorDialog = new ContentDialog
            {
                Title = "删除订单",
                Content = "你将会删除这个订单",
                PrimaryButtonText = "取消",
                SecondaryButtonText = "确认删除"
            };
            ContentDialogResult result = await ErrorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return;
            UserOrderSQLManagement.GetInstance().delete(MasterMenuItem.OrderId);
            instance.allItems.Remove(MasterMenuItem);
        }

        private async void ShowDetails(object sender, ItemClickEventArgs e)
        {
            SampleOrder selected = (SampleOrder)e.ClickedItem;
            ContentDialog details = new ContentDialog()
            {
                Title = "菜品详情",
                Content = "菜品名：" + selected.OrderName + "\n简介：" + selected.Summary + "\n详情：" + selected.Details + "\n\n\n件数：" + selected.Ordered,
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await details.ShowAsync();
        }
    }
}
