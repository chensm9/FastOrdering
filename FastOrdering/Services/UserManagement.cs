﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
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
        public bool isInternetConnected = true;
        //用户名
        private string username_ { get; set; }
        public string userName
        {
            get
            {
                return this.username_;
            }
            set
            {
                this.username_ = value;
                NotifyPropertyChanged("userName");
            }
        }

        //预留手机
        private string userphone_ { get; set; }
        public string userPhone
        {
            get
            {
                return this.userphone_;
            }
            set
            {
                this.userphone_ = value;
                NotifyPropertyChanged("userPhone");
            }
        }
        public string MD5password;
        public event PropertyChangedEventHandler PropertyChanged;
        //用于防止多次点赞
        public bool returnMain;

        //页面导航元素是否可见
        private Visibility sampleOrderVisible_;
        private Visibility shoppingCartVisible_;
        private Visibility logOnVisible_;
        private Visibility orderViewVisible_;
        private Visibility managementVisible_;
        //当前天气
        public String currentTemporature = "";
        //验证码
        public String VertificationCode = "";

        public void GetVertificationCode()
        {
            Random rd = new Random();
            VertificationCode = rd.Next(100000, 999999).ToString();
        }

        //单例模式
        public static UserManagement GetInstance()
        {
            if (instance == null)
            {
                instance = new UserManagement();
            }
            return instance;
        }

        //天气查询
        private async void queryWeather()
        {
            try
            {
                RootObject myWeather = await Weather.GetWeather("广州");
                currentTemporature = myWeather.result.today.temperature;
            }
            catch(Exception e)
            {
                currentTemporature = "网络错误";
                Console.WriteLine("断网！无法获取天气！");
                isInternetConnected = false;
            }
        }

        public string EncryptWithMD5(string source)
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
            }
            return strbul.ToString();
        }
        
        private UserManagement()
        {
            //登录状态默认为消费者
            isUser = true;
            isSupplier = false;
            isLogOn = false;
            //处于改密码的状态
            isEdit = false;
            //从数据库中加载用户数据，如若失败则使用缺省的用户信息
            var uInstance = UserSQLManagement.GetInstance();
            uInstance.GetAll();
            if(uInstance.getUsername == "" || uInstance.getPassword == "" || uInstance.getPhone == "")
            {
                uInstance.renew();
                userName = "admin";
                MD5password = EncryptWithMD5("123456");
                userPhone = "18664759453";
                uInstance.insert(userName, MD5password, userPhone);
            }
            else
            {
                userName = uInstance.getUsername;
                MD5password = uInstance.getPassword;
                userPhone = uInstance.getPhone;
            }
            
            //页面导航元素可见性
            SampleOrderVisible = Visibility.Collapsed;
            ShoppingCartVisible = Visibility.Collapsed;
            LogOnVisible = Visibility.Collapsed;
            OrderViewVisible = Visibility.Collapsed;
            ManagementVisible = Visibility.Collapsed;
            queryWeather();
        }

        //用户登录状态
        public void UserLogOn()
        {
            //用户登录
            isUser = true;
            isSupplier = false;
            isLogOn = false;
            //清空购物车菜品
            UserDataService.GetInstance()._current.SampleItems.Clear();
            SampleOrderVisible = Visibility.Visible;
            ShoppingCartVisible = Visibility.Visible;
            LogOnVisible = Visibility.Collapsed;
            OrderViewVisible = Visibility.Collapsed;
            ManagementVisible = Visibility.Collapsed;
        }

        //商家登录状态
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
