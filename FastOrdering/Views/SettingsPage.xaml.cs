using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
<<<<<<< HEAD

=======
using System.Text.RegularExpressions;
>>>>>>> CT
using FastOrdering.Helpers;
using FastOrdering.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FastOrdering.Views
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings-codebehind.md
    // TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

<<<<<<< HEAD
=======
        public UserManagement instance = UserManagement.GetInstance();

        //成员信息
        public string DetailDescription = "小组成员：陈思敏 陈思航 陈涛 陈谱一\n" +
                                          "©2018 FastOrdering";
>>>>>>> CT
        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        public SettingsPage()
        {
            InitializeComponent();
<<<<<<< HEAD
=======
            if (instance.isLogOn)
            {
                AboutMe.Visibility = Visibility.Collapsed;
                UserSettings.Visibility = Visibility.Visible;
            }
            else
            {
                AboutMe.Visibility = Visibility.Visible;
                UserSettings.Visibility = Visibility.Collapsed;
            }
>>>>>>> CT
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Initialize();
        }

        private void Initialize()
        {
            VersionDescription = GetVersionDescription();
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async void ThemeChanged_CheckedAsync(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null)
            {
                await ThemeSelectorService.SetThemeAsync((ElementTheme)param);
            }
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
<<<<<<< HEAD
=======

        //用户名为空
        private async void EmptyUser()
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
        private async void EmptyPassword()
        {
            ContentDialog userEmpty = new ContentDialog
            {
                Title = "密码为空",
                Content = "请输入密码",
                PrimaryButtonText = "好"
            };
            await userEmpty.ShowAsync();
        }

        //再次输入密码为空
        private async void EmptyPasswordAgain()
        {
            ContentDialog userEmpty = new ContentDialog
            {
                Title = "密码为空",
                Content = "请再次输入密码",
                PrimaryButtonText = "好"
            };
            await userEmpty.ShowAsync();
        }

        //手机为空
        private async void EmptyPhone()
        {
            ContentDialog userEmpty = new ContentDialog
            {
                Title = "手机号为空",
                Content = "请输入手机号",
                PrimaryButtonText = "好"
            };
            await userEmpty.ShowAsync();
        }

        //密码不一致
        private async void InvalidPassword()
        {
            ContentDialog invalid = new ContentDialog
            {
                Title = "密码不一致",
                Content = "两次输入的密码不一致",
                PrimaryButtonText = "好"
            };
            await invalid.ShowAsync();
        }

        //手机不合法
        private async void InvalidPhone()
        {
            ContentDialog invalid = new ContentDialog
            {
                Title = "非法手机号",
                Content = "请输入正确格式的手机号",
                PrimaryButtonText = "好"
            };
            await invalid.ShowAsync();
        }

        //用户名长度限制
        private async void InvalidUsername()
        {
            ContentDialog invalid = new ContentDialog
            {
                Title = "用户名长度不合法",
                Content = "请输入长度大于等于5且长度小于等于16的用户名",
                PrimaryButtonText = "好"
            };
            await invalid.ShowAsync();
        }

        //密码过长过短
        private async void ShortPassword()
        {
            ContentDialog invalid = new ContentDialog
            {
                Title = "非法密码",
                Content = "请输入长度大于等于6且小于等于16的密码",
                PrimaryButtonText = "好"
            };
            await invalid.ShowAsync();
        }

        //判断手机号合法性
        private bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^1[34578]\\d{9}$");
            return regex.IsMatch(input);
        }

        private async void Confirm()
        {
            //确认修改
            ContentDialog ErrorDialog = new ContentDialog
            {
                Title = "注意！修改",
                Content = "你将会修改你的用户设置",
                PrimaryButtonText = "取消",
                SecondaryButtonText = "确认修改"
            };
            var result = await ErrorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return;
            //修改信息
            instance.userName = Username.Text;
            instance.MD5password = instance.EncryptWithMD5(Password.Password);
            instance.userPhone = Phone.Text;

            //更新用户数据库
            var uInstance = UserSQLManagement.GetInstance();
            uInstance.renew();
            uInstance.insert(instance.userName, instance.MD5password, instance.userPhone);

            if (!UserManagement.GetInstance().isInternetConnected)
            {
                return;
            }
            //发送信息
            MessageHelper msh = new MessageHelper(true, "csh1997926", "d41d8cd98f00b204e980", instance.userPhone, "您的用户名改为【" + instance.userName + "】。密码为【" + Password.Password + "】 请妥善保管好用户名密码，切勿转发。");
            var res = msh.GetSendStr();
            Clear();
            return;

        }

        private void Clear()
        {
            Username.Text = "";
            Phone.Text = "";
            Password.Password = "";
            PasswordAgain.Password = "";
            this.Frame.Navigate(typeof(MainPage));
        }

        //修改用户设置
        private void EditUserSettings(object sender, RoutedEventArgs e)
        {
            if (Username.Text == "")
            {
                EmptyUser();
                return;
            }
            // 桌号
            else if (Password.Password == "")
            {
                EmptyPassword();
                return;
            }
            //用户数量为空
            else if (PasswordAgain.Password == "")
            {
                EmptyPasswordAgain();
                return;
            }else if (Password.Password.Length < 6 || Password.Password.Length > 16)
            {
                ShortPassword();
                return;
            }
            else if(Phone.Text == "")
            {
                EmptyPhone();
                return;
            }
            else if(Password.Password != PasswordAgain.Password)
            {
                InvalidPassword();
                return;
            }
            else if (Username.Text.Length < 5 || Username.Text.Length > 16)
            {
                InvalidUsername();
                return;
            }
            else if (!IsMobilePhone(Phone.Text))
            {
                InvalidPhone();
                return;
            }
            Confirm();
        }
>>>>>>> CT
    }
}
