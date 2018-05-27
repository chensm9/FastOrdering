using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using FastOrdering.Models;
using FastOrdering.Services;

using Microsoft.Toolkit.Uwp.UI.Controls;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace FastOrdering.Views
{
    public sealed partial class OrderViewPage : Page, INotifyPropertyChanged
    {
        private UserOrder _selected;

        public UserOrder Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ObservableCollection<UserOrder> SampleItems { get; private set; } = new ObservableCollection<UserOrder>();

        public OrderViewPage()
        {
            InitializeComponent();
            Loaded += OrderViewPage_Loaded;
        }

        private async void OrderViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            SampleItems.Clear();

            var data = await UserDataService.GetSampleModelDataAsync();

            foreach (var item in data)
            {
                SampleItems.Add(item);
            }

            if (MasterDetailsViewControl.ViewState == MasterDetailsViewState.Both)
            {
                Selected = SampleItems.First();
            }
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
    }
}
