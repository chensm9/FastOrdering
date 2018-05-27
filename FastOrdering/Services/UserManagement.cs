using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FastOrdering.Services
{
    public class UserManagement : INotifyPropertyChanged
    {
        private static UserManagement instance;
        //判断是否是顾客或者是供应者
        public bool isUser, isSupplier, isLogOn, isEdit;
        public string userName, password;
        public event PropertyChangedEventHandler PropertyChanged;

        //页面导航元素是否可见
        private Visibility sampleOrderVisible_;
        private Visibility shoppingCartVisible_;
        private Visibility logOnVisible_;
        private Visibility orderViewVisible_;
        private Visibility managementVisible_;

        //单例模式
        public static UserManagement getInstance()
        {
            if (instance == null)
            {
                instance = new UserManagement();
            }
            return instance;
        }

        private UserManagement()
        {
            //登录状态默认为消费者
            isUser = true;
            isSupplier = false;
            isLogOn = false;
            //处于改密码的状态
            isEdit = false;
            //缺省密码
            userName = "admin";
            password = "123456";

            //页面导航元素可见性
            SampleOrderVisible = Visibility.Collapsed;
            ShoppingCartVisible = Visibility.Collapsed;
            LogOnVisible = Visibility.Collapsed;
            OrderViewVisible = Visibility.Collapsed;
            ManagementVisible = Visibility.Collapsed;
        }


        public void UserLogOn()
        {
            //用户登录
            isUser = true;
            isSupplier = false;
            isLogOn = false;

            SampleOrderVisible = Visibility.Visible;
            ShoppingCartVisible = Visibility.Visible;
            LogOnVisible = Visibility.Collapsed;
            OrderViewVisible = Visibility.Collapsed;
            ManagementVisible = Visibility.Collapsed;
        }
        public void SupplierLogOn()
        {
            //管理者登录
            isUser = false;
            isSupplier = true;
            isLogOn = true;

            //页面导航元素可见性
            SampleOrderVisible = Visibility.Collapsed;
            ShoppingCartVisible = Visibility.Collapsed;
            LogOnVisible = Visibility.Collapsed;
            OrderViewVisible = Visibility.Visible;
            ManagementVisible = Visibility.Visible;
        }
        public void SupplierLogOut()
        {
            //管理者登出
            isUser = true;
            isSupplier = false;
            isLogOn = false;

            //页面导航元素可见性
            SampleOrderVisible = Visibility.Collapsed;
            ShoppingCartVisible = Visibility.Collapsed;
            LogOnVisible = Visibility.Collapsed;
            OrderViewVisible = Visibility.Collapsed;
            ManagementVisible = Visibility.Collapsed;
        }

        public Visibility SampleOrderVisible {
            get {
                return this.sampleOrderVisible_;
            }
            set {
                this.sampleOrderVisible_ = value;
                NotifyPropertyChanged("SampleOrderVisible");
            }
        }

        public Visibility ShoppingCartVisible {
            get {
                return this.shoppingCartVisible_;
            }
            set {
                this.shoppingCartVisible_ = value;
                NotifyPropertyChanged("ShoppingCartVisible");
            }
        }
        public Visibility LogOnVisible {
            get {
                return this.logOnVisible_;
            }
            set {
                this.logOnVisible_ = value;
                NotifyPropertyChanged("LogOnVisible");
            }
        }
        public Visibility OrderViewVisible {
            get {
                return this.orderViewVisible_;
            }
            set {
                this.orderViewVisible_ = value;
                NotifyPropertyChanged("OrderViewVisible");
            }
        }
        public Visibility ManagementVisible {
            get {
                return this.managementVisible_;
            }
            set {
                this.managementVisible_ = value;
                NotifyPropertyChanged("ManagementVisible");
            }
        }
        public void NotifyPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
