using FastOrdering.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace FastOrdering.Models
{
    // TODO WTS: Remove this class once your pages/features are using your data.
    // This is used by the SampleDataService.
    // It is the model class we use to display data on pages like Grid, Chart, and Master Detail.
    public class UserOrder : INotifyPropertyChanged
    {
        public static int id = GetId();
        private static int GetId()
        {
            if (UserDataService.GetInstance().allItems.Count == 0)
            {
                return 0;
            }
            int max = 0;
            for (int i = 0; i < UserDataService.GetInstance().allItems.Count; ++i)
            {
                if (UserDataService.GetInstance().allItems[i].OrderId > max)
                {
                    max = SampleDataService.GetInstance().allItems[i].OrderId;
                }
            }
            return (max + 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public UserOrder()
        {
            UserNum = 0;
            Table = 0;
            Pepper = -1;
            Details = "";
            Price = 0;
            SampleItems.Clear();
        }

        public UserOrder(bool flag)
        {
            UserNum = 0;
            Table = 0;
            Pepper = -1;
            Details = "";
            Price = 0;
            SampleItems.Clear();
            if (flag)
            {
                this.OrderId = id;
                id++;
            }
        }

        //订单id
        public int OrderId { get; set; }

        //用户人数
        private int userNum_ { get; set; }
        public int UserNum
        {
            get
            {
                return this.userNum_;
            }
            set
            {
                this.userNum_ = value;
                NotifyPropertyChanged("UserNum");
            }
        }

        //桌号
        private int table_ { get; set; }
        public int Table
        {
            get
            {
                return this.table_;
            }
            set
            {
                this.table_ = value;
                NotifyPropertyChanged("Table");
            }
        }

        //辣椒
        private double pepper_ { get; set; }
        public double Pepper
        {
            get
            {
                return this.pepper_;
            }
            set
            {
                this.pepper_ = value;
                NotifyPropertyChanged("Pepper");
            }
        }

        //总价格
        private float price_ { get; set; }
        public float Price
        {
            get
            {
                return this.price_;
            }
            set
            {
                this.price_ = value;
                NotifyPropertyChanged("Price");
            }
        }
        

        //用户备注
        private String details_ { get; set; }
        public String Details
        {
            get
            {
                return this.details_;
            }
            set
            {
                this.details_ = value;
                NotifyPropertyChanged("Details");
            }
        }

        //重新初始化订单
        public void Clear()
        {
            UserNum = 0;
            Table = 0;
            Pepper = -1;
            Details = "";
            Price = 0;
            SampleItems.Clear();
        }

        public ObservableCollection<SampleOrder> SampleItems = new ObservableCollection<SampleOrder>();

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
