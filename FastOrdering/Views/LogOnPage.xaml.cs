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
        UserManagement instance = UserManagement.GetInstance();
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

        //密码过长过短
        private async void ShortPassword()
        {
            ContentDialog invalid = new ContentDialog
            {
                Title = "非法密码",
                Content = "请输入长度大于等于6且长度小于等于16的密码",
                PrimaryButtonText = "好"
            };
            await invalid.ShowAsync();
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
                    if (username.Text ==instance.userName &&
                    instance.EncryptWithMD5(password.Password) ==instance.MD5password)
                    {
                       instance.SupplierLogOn();
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
                } else if(password.Password.Length < 6 || password.Password.Length > 16)
                {
                    ShortPassword();
                }
                else
                {
                    if (username.Text == instance.VertificationCode)
                        //成功修改用户名密码
                        instance.MD5password = instance.EncryptWithMD5(password.Password);
                        //更新用户数据库
                        UserSQLManagement.GetInstance().update(instance.userName, instance.MD5password, instance.userPhone);
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
                       instance.isEdit = false;
                }
            }
        }

        //图片随着密码正确性变化
        private void passwordChanging(PasswordBox sender, PasswordBoxPasswordChangingEventArgs args)
        {
            if (!UserManagement.GetInstance().isEdit && instance.EncryptWithMD5(password.Password) ==instance.MD5password)
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
            if (!UserManagement.GetInstance().isEdit && username.Text ==instance.userName)
            {
                click1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                click1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void changePassword(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //网络错误
            if (!UserManagement.GetInstance().isInternetConnected)
            {
                ContentDialog ErrInternet = new ContentDialog
                {
                    Title = "网络错误",
                    Content = "无法修改密码",
                    PrimaryButtonText = "好"
                };
                await ErrInternet.ShowAsync();
                return;
            }
            instance.GetVertificationCode();
            MessageHelper msh = new MessageHelper(true, "csh1997926", "d41d8cd98f00b204e980", instance.userPhone, "您的用户名为" + instance.userName +"。验证码为【"+instance.VertificationCode +"】 请妥善保管好验证码，切勿转发。");
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
           instance.isEdit = true;
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
