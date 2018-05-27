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
            InitializeComponent();
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/background.jpg", UriKind.Absolute));
            ContentArea.Background = imageBrush;
            Loaded += MainPage_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null) {
            if (Equals(storage, value)) {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //开始编辑的函数和变量
        public ObservableCollection<SampleOrder> SampleItems { get; private set; } = new ObservableCollection<SampleOrder>();
        private async void MainPage_Loaded(object sender, RoutedEventArgs e) {
            SampleItems.Clear();

            var data = await SampleDataService.GetMyOrderAsync();

            foreach (var item in data) {
                SampleItems.Add(item);
            }
        }
        //点击图片跳转
        private void navigateToLogOn(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            //跳转新页面
            this.Frame.Navigate(typeof(LogOnPage));
        }

        private void navigateToOrderPage(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            this.Frame.Navigate(typeof(OrderingPage));
            UserManagement.getInstance().UserLogOn();
        }

        //鼠标在上面，图标变化
        private void mouseChangeBuyer(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            string ImgSource = "ms-appx:///Assets/buyerOn.png";
            BitmapImage bitmap = new BitmapImage(new Uri(ImgSource));
            buyer.Source = bitmap;
        }

        private void mouseChangeSupplier(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            string ImgSource = "ms-appx:///Assets/supplierOn.png";
            BitmapImage bitmap = new BitmapImage(new Uri(ImgSource));
            supplier.Source = bitmap;
        }

        private async void logOnOrLogOut(object sender, RoutedEventArgs e) {
            if (UserManagement.getInstance().isLogOn == false) {
                this.Frame.Navigate(typeof(LogOnPage));
            } else {
                UserManagement.getInstance().SupplierLogOut();
                ContentDialog logOut = new ContentDialog {
                    Title = "退出",
                    Content = "退出成功",
                    PrimaryButtonText = "好"
                };
                await logOut.ShowAsync();
            }
        }
    }
}
