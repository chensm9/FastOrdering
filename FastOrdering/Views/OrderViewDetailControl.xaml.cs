using System;

using FastOrdering.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FastOrdering.Views
{
    public sealed partial class OrderViewDetailControl : UserControl
    {
        public UserOrder MasterMenuItem
        {
            get {return new UserOrder(); }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static readonly DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(UserOrder), typeof(OrderViewDetailControl), new PropertyMetadata(null, OnMasterMenuItemPropertyChanged));

        public OrderViewDetailControl()
        {
            InitializeComponent();
        }


        private static void OnMasterMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as OrderViewDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
