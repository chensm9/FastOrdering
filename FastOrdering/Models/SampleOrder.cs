using System;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace FastOrdering.Models
{
    // TODO WTS: Remove this class once your pages/features are using your data.
    // This is used by the SampleDataService.
    // It is the model class we use to display data on pages like Grid, Chart, and Master Detail.
    public class SampleOrder : INotifyPropertyChanged
    {
        public static int id = 0;
        public string imgPath;
        public event PropertyChangedEventHandler PropertyChanged;
        public SampleOrder()
        {
            string ImgSource = "ms-appx:///Assets/newOne.jpg";
            this.Pict = new BitmapImage(new Uri(ImgSource));
            this.OrderId = id;
            id++;
            Visited = 0;
            Collected = 0;
        }

        //商品id
        public int OrderId { get; set; }

        //商品名
        private string orderName_ { get; set; }
        public string OrderName
        {
            get
            {
                return this.orderName_;
            }
            set
            {
                this.orderName_ = value;
                NotifyPropertyChanged("OrderName");
            }
        }

        private int orderNum_ { get; set; }
        public int OrderNum
        {
            get
            {
                return this.orderNum_;
            }
            set
            {
                this.orderNum_ = value;
                NotifyPropertyChanged("OrderNum");
            }
        }

        //销量
        private int sold_ { get; set; }
        public int Sold
        {
            get
            {
                return this.sold_;
            }
            set
            {
                this.sold_ = value;
                NotifyPropertyChanged("Sold");
            }
        }

        //人气
        private int visited_ { get; set; }
        public int Visited
        {
            get
            {
                return this.visited_;
            }
            set
            {
                this.visited_ = value;
                NotifyPropertyChanged("Visited");
            }
        }

        //收藏
        private int collected_ { get; set; }
        public int Collected
        {
            get
            {
                return this.collected_;
            }
            set
            {
                this.collected_ = value;
                NotifyPropertyChanged("Collected");
            }
        }


        //价格
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
        
        //图片
        private ImageSource pict_ { get; set; }
        public ImageSource Pict
        {
            get
            {
                return this.pict_;
            }
            set
            {
                this.pict_ = value;
                NotifyPropertyChanged("Pict");
            }
        }

        //长描述
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

        //短描述
        private String summary_ { get; set; }
        public String Summary
        {
            get
            {
                return this.summary_;
            }
            set
            {
                this.summary_ = value;
                NotifyPropertyChanged("Summary");
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
