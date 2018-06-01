using FastOrdering.Services;
using System;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace FastOrdering.Models
{
    // TODO WTS: Remove this class once your pages/features are using your data.
    // This is used by the SampleDataService.
    // It is the model class we use to display data on pages like Grid, Chart, and Master Detail.
    public class SampleOrder : INotifyPropertyChanged
    {
        public static int id = GetId();
        public string imgPath;
        public event PropertyChangedEventHandler PropertyChanged;
        public SampleOrder()
        {
            string ImgSource = "ms-appx:///Assets/newOne.jpg";
            this.Pict = new BitmapImage(new Uri(ImgSource));
            Visited = 0;
            Collected = 0;
            Ordered = 0;
        }
        private static int GetId()
        {
            if (SampleDataService.GetInstance().allItems.Count == 0)
            {
                return 0;
            }
            int max = 0;
            for(int i = 0; i < SampleDataService.GetInstance().allItems.Count; ++i)
            {
                if(SampleDataService.GetInstance().allItems[i].OrderId > max)
                {
                    max = SampleDataService.GetInstance().allItems[i].OrderId;
                }
            }
            return (max + 1);
        }

        public SampleOrder(bool flag)
        {
            string ImgSource = "ms-appx:///Assets/newOne.jpg";
            this.Pict = new BitmapImage(new Uri(ImgSource));
            Visited = 0;
            Collected = 0;
            Ordered = 0;
            //添加菜品时创建
            if (flag)
            {
                this.OrderId = id;
                id++;
            }
        }

        //菜品id
        public int OrderId { get; set; }

        //菜品名
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

        //点餐数量
        private int ordered_ { get; set; }
        public int Ordered
        {
            get
            {
                return this.ordered_;
            }
            set
            {
                if(value > 99 || value < 0)
                {
                    this.ordered_ = 1;
                }
                else
                {
                    this.ordered_ = value;  
                }
                NotifyPropertyChanged("Ordered");
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
