using FastOrdering.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
            username.PlaceholderText = "用户名";
            password.PlaceholderText = "密码";
        }

        private async void checkAndLogOn(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //用户名为空
            if(username.Text == "")
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
            else if(password.Password == "")
            {
                ContentDialog userEmpty = new ContentDialog
                {
                    Title = "密码为空",
                    Content = "请输入密码",
                    PrimaryButtonText = "好"
                };
                await userEmpty.ShowAsync();
            }
            //检测用户名密码是否匹配
            else
            {
                if (!UserManagement.getInstance().isEdit)
                {
                    if (username.Text == UserManagement.getInstance().userName &&
                    password.Password == UserManagement.getInstance().password)
                    {
                        UserManagement.getInstance().SupplierLogOn();
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
                else
                {
                    //成功修改用户名密码
                    UserManagement.getInstance().userName = username.Text;
                    UserManagement.getInstance().password = password.Password;
                    title.Text = "商家登录";
                    logIn.Content = "登录";
                    username.Text = "";
                    password.Password = "";
                    username.PlaceholderText = "用户名";
                    password.PlaceholderText = "密码";
                    click1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    click2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    forget.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    UserManagement.getInstance().isEdit = false;
                }
            }
        }

       //图片随着密码正确性变化
        private void passwordChanging(PasswordBox sender, PasswordBoxPasswordChangingEventArgs args)
        {
            if (!UserManagement.getInstance().isEdit && password.Password == UserManagement.getInstance().password)
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
            if (!UserManagement.getInstance().isEdit && username.Text == UserManagement.getInstance().userName)
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
            title.Text = "修改用户名密码";
            logIn.Content = "修改";
            forget.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            click1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            click2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            username.Text = "";
            password.Password = "";
            username.PlaceholderText = "用户名";
            password.PlaceholderText = "密码";
            UserManagement.getInstance().isEdit = true;
        }
    }
}
