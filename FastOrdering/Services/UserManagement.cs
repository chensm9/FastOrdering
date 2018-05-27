using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FastOrdering.Services
{
    public class UserManagement
    {
        private static UserManagement instance;
        //判断是否是顾客或者是供应者
        public bool isUser, isSupplier, isLogOn, isEdit;
        public string userName, password;

        //页面导航元素是否可见
        public Visibility SampleOrderVisible;
        public Visibility ShoppingCartVisible;
        public Visibility LogOnVisible;
        public Visibility OrderViewVisible;
        public Visibility ManagementVisible;

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
            SampleOrderVisible = Visibility.Visible;
            ShoppingCartVisible = Visibility.Visible;
            LogOnVisible = Visibility.Visible;
            OrderViewVisible = Visibility.Collapsed;
            ManagementVisible = Visibility.Collapsed;
        }

        public void LogOn()
        {
            //登录
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
        public void LogOut()
        {
            //登出
            isUser = true;
            isSupplier = false;
            isLogOn = false;

            //页面导航元素可见性
            SampleOrderVisible = Visibility.Visible;
            ShoppingCartVisible = Visibility.Visible;
            LogOnVisible = Visibility.Visible;
            OrderViewVisible = Visibility.Collapsed;
            ManagementVisible = Visibility.Collapsed;
        }
    }
}
