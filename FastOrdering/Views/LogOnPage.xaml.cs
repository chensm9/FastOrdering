using FastOrdering.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace FastOrdering.Views
{
    public sealed partial class LogOnPage : Page, INotifyPropertyChanged
    {
        public LogOnPage()
        {
            InitializeComponent();
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

        private void clearContent(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            username.Text = "";
            password.Password = "";
            if (UserManagement.GetInstance().isEdit)
            {
                username.PlaceholderText = "验证码";
                usrTitle.Text = "验证码";
            }
            else
            {
                username.PlaceholderText = "用户名";
                usrTitle.Text = "用户名";
            }

            password.PlaceholderText = "密码";
        }

        private async void checkAndLogOn(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //检测用户名密码是否匹配
            if (!UserManagement.GetInstance().isEdit)
            {
                if (username.Text == "")
                {
                    ContentDialog userEmpty = new ContentDialog
                    {
                        Title = "用户名为空",
                        Content = "请输入用户名",
                        PrimaryButtonText = "好"
                    };
                    await userEmpty.ShowAsync();
                }
                //密码为空
                else if (password.Password == "")
                {
                    ContentDialog userEmpty = new ContentDialog
                    {
                        Title = "密码为空",
                        Content = "请输入密码",
                        PrimaryButtonText = "好"
                    };
                    await userEmpty.ShowAsync();
                }
                else
                {
                    //登录成功
                    if (username.Text == UserManagement.GetInstance().userName &&
                    password.Password == UserManagement.GetInstance().password)
                    {
                        UserManagement.GetInstance().SupplierLogOn();
                        ContentDialog logOn = new ContentDialog
                        {
                            Title = "登录",
                            Content = "登录成功",
                            PrimaryButtonText = "好"
                        };
                        await logOn.ShowAsync();
                        this.Frame.Navigate(typeof(OrderViewPage));
                    }
                    else
                    {
                        ContentDialog userErr = new ContentDialog
                        {
                            Title = "用户名或密码错误",
                            Content = "请输入用户名或密码",
                            PrimaryButtonText = "好"
                        };
                        await userErr.ShowAsync();
                    }
                }
            }
            //修改用户名密码
            else
            {
                if (username.Text == "")
                {
                    ContentDialog userEmpty = new ContentDialog
                    {
                        Title = "验证码为空",
                        Content = "请输入验证码",
                        PrimaryButtonText = "好"
                    };
                    await userEmpty.ShowAsync();
                }
                //密码为空
                else if (password.Password == "")
                {
                    ContentDialog userEmpty = new ContentDialog
                    {
                        Title = "密码为空",
                        Content = "请输入密码",
                        PrimaryButtonText = "好"
                    };
                    await userEmpty.ShowAsync();
                }
                else
                {
                    if (username.Text == UserManagement.GetInstance().VertificationCode)
                        //成功修改用户名密码
                        UserManagement.GetInstance().password = password.Password;
                        title.Text = "商家登录";
                        logIn.Content = "登录";
                        usrTitle.Text = "用户名";
                        username.Text = "";
                        password.Password = "";
                        username.PlaceholderText = "用户名";
                        password.PlaceholderText = "密码";
                        click1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        click2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        forget.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        UserManagement.GetInstance().isEdit = false;
                }
            }
        }

        //图片随着密码正确性变化
        private void passwordChanging(PasswordBox sender, PasswordBoxPasswordChangingEventArgs args)
        {
            if (!UserManagement.GetInstance().isEdit && password.Password == UserManagement.GetInstance().password)
            {
                click2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                click2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void userNameChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (!UserManagement.GetInstance().isEdit && username.Text == UserManagement.GetInstance().userName)
            {
                click1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                click1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void changePassword(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            UserManagement.GetInstance().GetVertificationCode();
            MessageHelper msh = new MessageHelper(true, "csh1997926", "d41d8cd98f00b204e980", "18664759453", "密码修改验证码【"+ UserManagement.GetInstance().VertificationCode +"】 请妥善保管好验证码，切勿转发。");
            var res = msh.GetSendStr();
            title.Text = "修改用户名密码";
            logIn.Content = "修改";
            usrTitle.Text = "验证码";
            forget.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            click1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            click2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            username.Text = "";
            password.Password = "";
            username.PlaceholderText = "验证码";
            password.PlaceholderText = "密码";
            UserManagement.GetInstance().isEdit = true;
        }

        private void ChangeToArrow(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        }

        private void ChangeToHand(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
        }
    }
}
