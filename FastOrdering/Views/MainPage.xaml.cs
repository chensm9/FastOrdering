using FastOrdering.Models;
using FastOrdering.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace FastOrdering.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged {
        //是否登录
        public MainPage() {
            instance.GetCollectedListView();
            InitializeComponent();
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/background.jpg", UriKind.Absolute));
            ContentArea.Background = imageBrush;
            UserManagement.GetInstance().returnMain = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public SampleDataService instance = SampleDataService.GetInstance();
        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null) {
            if (Equals(storage, value)) {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        //点击图片跳转
        private void navigateToLogOn(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            //跳转新页面
            this.Frame.Navigate(typeof(LogOnPage));
        }

        private async void navigateToOrderPage(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            if (UserManagement.GetInstance().isLogOn)
            {
                ContentDialog IsLogOn = new ContentDialog()
                {
                    Title = "跳转失败！",
                    Content = "请先退出登录",
                    PrimaryButtonText = "好",
                    SecondaryButtonText = "退出登录",
                };
                ContentDialogResult result = await IsLogOn.ShowAsync();
                if (result == ContentDialogResult.Secondary)
                {
                    this.Frame.Navigate(typeof(OrderingPage));
                    UserManagement.GetInstance().UserLogOn();
                }
                return;
            }else
            {
                this.Frame.Navigate(typeof(OrderingPage));
                UserManagement.GetInstance().UserLogOn();
            }
        }

        //鼠标在上面，图标变化
        private void mouseChangeBuyer(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            //string ImgSource = "ms-appx:///Assets/buyerOn.png";
            //BitmapImage bitmap = new BitmapImage(new Uri(ImgSource));
            //buyer.Source = bitmap;
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
        }

        private void mouseChangeSupplier(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
        }

        private async void logOnOrLogOut(object sender, RoutedEventArgs e) {
            if (UserManagement.GetInstance().isLogOn == false) {
                this.Frame.Navigate(typeof(LogOnPage));
            } else {
                UserManagement.GetInstance().SupplierLogOut();
                ContentDialog logOut = new ContentDialog {
                    Title = "退出",
                    Content = "退出成功",
                    PrimaryButtonText = "好"
                };
                await logOut.ShowAsync();
            }
        }

        private void changeToArrow(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }

        private void ChangeToArrow2(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }

        //点击显示详情
        private async void ShowDetails(object sender, ItemClickEventArgs e)
        {
            SampleOrder selected = (SampleOrder)e.ClickedItem;
            ContentDialog details = new ContentDialog()
            {
                Title = "菜品详情",
                Content = "菜品名：" + selected.OrderName + "\n简介：" + selected.Summary + "\n详情：" + selected.Details + "\n赞数：" + selected.Collected + "\n访问量：" + selected.Visited,
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await details.ShowAsync();
        }
    }
}
