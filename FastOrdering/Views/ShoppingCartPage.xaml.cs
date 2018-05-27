using FastOrdering.Models;
using FastOrdering.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FastOrdering.Views
{
    public sealed partial class ShoppingCartPage : Page, INotifyPropertyChanged
    {
        public ShoppingCartPage()
        {
            InitializeComponent();
            Loaded += ShoppingCart_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //开始编辑的函数和变量
        //订单集合
        public ObservableCollection<SampleOrder> SampleItems { get; private set; } = new ObservableCollection<SampleOrder>();
        //点菜总数
        public int Sum = 100;
        public float TotalPrice = 100.0f;
        public float currentTemporature = 30.0f;
        public string tips = "多喝冷饮哦";
        private async void ShoppingCart_Loaded(object sender, RoutedEventArgs e)
        {
            SampleItems.Clear();

            var data = await SampleDataService.GetMyOrderAsync();

            foreach (var item in data)
            {
                SampleItems.Add(item);
            }
        }
    }
}
