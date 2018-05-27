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
        public static int id = 0;
        public event PropertyChangedEventHandler PropertyChanged;
        public UserOrder()
        {
            SampleItems = new ObservableCollection<SampleOrder>
            {
                new SampleOrder
                    {
                        OrderName = "寿司",
                        Price = 19.99f,
                        Summary = "ddddddddddddddddddd",
                        Details = "aaa",
                    },
            };
            this.OrderId = id;
            id++;
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
        private int pepper_ { get; set; }
        public int Pepper
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

        public ObservableCollection<SampleOrder> SampleItems;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
