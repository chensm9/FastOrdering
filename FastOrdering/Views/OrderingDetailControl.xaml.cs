using System;

using FastOrdering.Models;

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
    }
}
