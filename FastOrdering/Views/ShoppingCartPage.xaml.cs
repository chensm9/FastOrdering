using FastOrdering.Models;
using FastOrdering.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FastOrdering.Views
{
    public sealed partial class ShoppingCartPage : Page, INotifyPropertyChanged
    {
        private SampleOrder selectedOrder;
        public UserDataService instance = UserDataService.GetInstance();
        public SampleDataService sampleInstance = SampleDataService.GetInstance();
        public UserManagement userManagementInstance = UserManagement.GetInstance();

        public ShoppingCartPage()
        {
            sampleInstance.GetTips();
            sampleInstance.GetListView();
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
        //数量修改
        private async void subtractItem(object sender, RoutedEventArgs e)
        {
            var originalSource = e.OriginalSource as Button;
            int currentID = (int)originalSource.DataContext;
            int pos = -1;
            //利用id的唯一性，寻找该菜品在菜单中的位置
            for (int i = 0; i < instance._current.SampleItems.Count; ++i)
            {
                if (instance._current.SampleItems[i].OrderId == currentID)
                {
                    pos = i;
                }
            }
            //大于1则减1
            if(instance._current.SampleItems[pos].Ordered > 1)
            {
                instance._current.SampleItems[pos].Ordered--;
                Calculate();
                //等于1则删除
            } else if(instance._current.SampleItems[pos].Ordered == 1)
            {
                ContentDialog ErrorDialog = new ContentDialog
                {
                    Title = "注意！删除",
                    Content = "你将会从购物车中删除该菜品",
                    PrimaryButtonText = "取消",
                    SecondaryButtonText = "确认删除"
                };
                ContentDialogResult result = await ErrorDialog.ShowAsync();
                if (result == ContentDialogResult.Primary) return;
                instance._current.SampleItems.Remove(instance._current.SampleItems[pos]);
                Calculate();
            } else {
                //报错
                ContentDialog ErrDialog = new ContentDialog
                {
                    Title = "非法数量",
                    Content = "请设置正确的菜品数量",
                    PrimaryButtonText = "好"
                };
                await ErrDialog.ShowAsync();
            }
        }

        private async void addItem(object sender, RoutedEventArgs e)
        {
            var originalSource = e.OriginalSource as Button;
            int currentID = (int)originalSource.DataContext;
            int pos = -1;
            //利用id的唯一性，寻找该菜品在菜单中的位置
            for(int i = 0; i < instance._current.SampleItems.Count; ++i)
            {
                if(instance._current.SampleItems[i].OrderId == currentID)
                {
                    pos = i;
                }
            }
            if(instance._current.SampleItems[pos].Ordered == 99)
            {
                //数量到达上限
                ContentDialog FailedAdd = new ContentDialog()
                {
                    Title = "添加错误",
                    Content = "菜品\"" + instance._current.SampleItems[pos].OrderName + "\"已经存在购物车中，数量大于上限99",
                    PrimaryButtonText = "好"
                };
                ContentDialogResult response = await FailedAdd.ShowAsync();
            }
            else
            {
                instance._current.SampleItems[pos].Ordered++;
                Calculate();
            }
        }

        //显示购物车菜品详情
        private async void showDetails(object sender, ItemClickEventArgs e)
        {
            SampleOrder selected = (SampleOrder)e.ClickedItem;
            ContentDialog details = new ContentDialog()
            {
                Title = "菜品详情",
                Content = "菜品名：" + selected.OrderName + "\n简介：" + selected.Summary + "\n详情：" + selected.Details + "\n\n\n件数：" + selected.Ordered,
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await details.ShowAsync();
        }
        //显示推荐菜品详情
        //点击显示详情
        private async void ShowDetails(object sender, ItemClickEventArgs e)
        {
            SampleOrder selected = (SampleOrder)e.ClickedItem;
            ContentDialog details = new ContentDialog()
            {
                Title = "菜品详情",
                Content = "菜品名：" + selected.OrderName + "\n简介：" + selected.Summary + "\n详情：" + selected.Details + "\n赞数：" + selected.Collected + "\n访问量：" + selected.Visited,
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await details.ShowAsync();
        }

        private async void deleteItem(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            var originalSource = e.OriginalSource as MenuFlyoutItem;
            SampleOrder data = (SampleOrder)originalSource.DataContext;
            ContentDialog ErrorDialog = new ContentDialog
            {
                Title = "注意！删除",
                Content = "你将会从购物车中删除该菜品",
                PrimaryButtonText = "取消",
                SecondaryButtonText = "确认删除"
            };
            ContentDialogResult result = await ErrorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return;
            instance._current.SampleItems.Remove(data);
            Calculate();
        }

        //分享按钮
        private void shareItem(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            var originalSource = e.OriginalSource as MenuFlyoutItem;
            SampleOrder data = (SampleOrder)originalSource.DataContext;
            selectedOrder = data;
            dataTransferManager.DataRequested += DataRequested;
            DataTransferManager.ShowShareUI();
        }

        //程序之间的沟通
        private async void DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            //标题
            DataRequest request = args.Request;
            request.Data.Properties.Title = "我发现这家小店这一道菜非常棒！\n菜品名：" + selectedOrder.OrderName + "\n";
            //详情
            string send = selectedOrder.Summary + "\n" + selectedOrder.Details;
            request.Data.SetText(send);
            //添加图片到邮件
            RandomAccessStreamReference bitmap;
            //判断是否是默认图片，如果不是需要打开folder以获得图片的权限
            if (selectedOrder.imgPath == "ms-appx:///Assets/newOne.jpg")
            {
                bitmap = RandomAccessStreamReference.CreateFromUri(new Uri(selectedOrder.imgPath));
            }
            else
            {
                StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                StorageFolder folder = await applicationFolder.GetFolderAsync("pic");
                string folder_str = folder.Path.ToString() + '/';
                string myUri = selectedOrder.imgPath;
                //获得此时的文件名
                string file_name = myUri.ToString().Replace(folder_str.ToString(), "");
                StorageFile logoImage = await folder.GetFileAsync(file_name);
                bitmap = RandomAccessStreamReference.CreateFromFile(logoImage);
            }
            request.Data.SetBitmap(bitmap);

        }

        private async void DeleteAllItems(object sender, RoutedEventArgs e)
        {
            ContentDialog ErrorDialog = new ContentDialog
            {
                Title = "注意！删除",
                Content = "你将会删除购物车中的所有菜品信息",
                PrimaryButtonText = "取消",
                SecondaryButtonText = "确认删除"
            };
            ContentDialogResult result = await ErrorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return;
            instance._current.SampleItems.Clear();
            Calculate();
            SampleOrder.id = 0;
        }

        private void Clear()
        {
            tableId.Text = "";
            userNum.Text = "";
            pepper.Value = -1;
            details.Text = "";
            Phone.Text = "";
        }

        private void Calculate()
        {
            instance._current.Price = 0;
            for(int i = 0; i < instance._current.SampleItems.Count; ++i)
            {
                instance._current.Price += instance._current.SampleItems[i].Price * instance._current.SampleItems[i].Ordered;
            }
        }


        //清除下方信息
        private void ClearDetails(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        //空购物车
        private async void EmptyCart()
        {
            ContentDialog ErrCart = new ContentDialog()
            {
                Title = "购物车为空",
                Content = "请将菜品添加到购物车中",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await ErrCart.ShowAsync();
        }

        //数量转化错误
        private async void InvalidNum()
        {
            ContentDialog ErrNum = new ContentDialog()
            {
                Title = "非法数量输入",
                Content = "请输入数字",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await ErrNum.ShowAsync();
        }

        //就餐人数小于0
        private async void ErrUsrNum()
        {
            ContentDialog ErrorUsrNum = new ContentDialog()
            {
                Title = "非法就餐人数输入",
                Content = "请输入大于0的人数",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await ErrorUsrNum.ShowAsync();
        }

        //桌号转化错误
        private async void InvalidUsrNum()
        {
            ContentDialog ErrUsrNum = new ContentDialog()
            {
                Title = "非法就餐人数输入",
                Content = "请输入数字",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await ErrUsrNum.ShowAsync();
        }
        
        //桌号小于0
        private async void ErrTbid()
        {
            ContentDialog ErrorTbid = new ContentDialog()
            {
                Title = "非法桌号输入",
                Content = "请输入大于0的桌号",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await ErrorTbid.ShowAsync();
        }

        //桌号转化错误
        private async void InvalidTbid()
        {
            ContentDialog ErrTbid = new ContentDialog()
            {
                Title = "非法桌号输入",
                Content = "请输入数字",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await ErrTbid.ShowAsync();
        }
        //桌号为空
        private async void EmptyTitle()
        {
            ContentDialog EmptyTitle = new ContentDialog()
            {
                Title = "桌号为空",
                Content = "请添加桌号",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await EmptyTitle.ShowAsync();
        }

        //人数为空
        private async void EmptyPrice()
        {
            ContentDialog EmptyPrice = new ContentDialog()
            {
                Title = "就餐人数为空",
                Content = "请添加就餐人数",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await EmptyPrice.ShowAsync();
        }

        //判断手机号合法性
        private bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^1[34578]\\d{9}$");
            return regex.IsMatch(input);
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
        //创建订单
        private void CreateUserOrder(object sender, RoutedEventArgs e)
        {
            if(instance._current.SampleItems.Count == 0)
            {
                EmptyCart();
                return;
            }
            // 桌号
            else if (tableId.Text == "")
            {
                EmptyTitle();
                return;
            }
            //用户数量为空
            else if (userNum.Text == "")
            {
                EmptyPrice();
                return;
            }
            else
            {
                if (!IsMobilePhone(Phone.Text))
                {
                    InvalidPhone();
                    return;
                }
                //桌号、就餐人数、辣味接受程度的临时变量
                int tbId, usrNum;
                //桌号转换为数字
                if (!int.TryParse(tableId.Text, out tbId))
                {
                    InvalidTbid();
                    return;
                } else if (tbId < 0) {
                    ErrTbid();
                    return;
                } else {
                    instance._current.Table = tbId;
                }
                
                //就餐人数转换为数字
                if (!int.TryParse(userNum.Text, out usrNum))
                {
                    InvalidUsrNum();
                    return;
                }
                else if (usrNum < 0)
                {
                    ErrUsrNum();
                    return;
                }
                else
                {
                    instance._current.UserNum = usrNum;
                }
                instance._current.Pepper = pepper.Value;
                instance._current.Details = details.Text;
                AccessData();
            }
        }

        private async void AccessData()
        {
            UserOrder newOne = new UserOrder(true);
            newOne.UserNum = instance._current.UserNum;
            newOne.Table = instance._current.Table;
            newOne.Pepper = instance._current.Pepper;
            newOne.Details = instance._current.Details;
            newOne.Price = instance._current.Price;
            //添加菜品 
            for(int i = 0; i < instance._current.SampleItems.Count; i++)
            {
                SampleOrder newItem = new SampleOrder
                {
                    OrderId = instance._current.SampleItems[i].OrderId,
                    OrderName = instance._current.SampleItems[i].OrderName,
                    Sold = instance._current.SampleItems[i].Sold,
                    Visited = instance._current.SampleItems[i].Visited,
                    Collected = instance._current.SampleItems[i].Collected,
                    Price = instance._current.SampleItems[i].Price,
                    imgPath = instance._current.SampleItems[i].imgPath,
                    Pict = new BitmapImage(new Uri(instance._current.SampleItems[i].imgPath)),
                    Details = instance._current.SampleItems[i].Details,
                    Summary = instance._current.SampleItems[i].Summary,
                    Ordered = instance._current.SampleItems[i].Ordered,
                };
                newOne.SampleItems.Add(newItem);
            }
            //网络错误处理
            if (!UserManagement.GetInstance().isInternetConnected)
            {
                ContentDialog ErrInternet = new ContentDialog
                {
                    Title = "网络错误",
                    Content = "订单已提交但无信息通知",
                    PrimaryButtonText = "好"
                };
                await ErrInternet.ShowAsync();
            }
            else
            {
                MessageHelper msh = new MessageHelper(true, "csh1997926", "d41d8cd98f00b204e980", Phone.Text, "桌号为" + instance._current.Table + "的客人：您的订单已经成功创建，共" + instance._current.SampleItems.Count.ToString() + "个菜品，消费" + instance._current.Price.ToString() + "元。");
                var res = msh.GetSendStr();
                
                ContentDialog AccessDate = new ContentDialog()
                {
                    Title = "订单创建成功！",
                    Content = "您的订单已创建成功",
                    PrimaryButtonText = "好"
                };
                
                await AccessDate.ShowAsync();
            }
            UserOrderSQLManagement.GetInstance().insert(newOne);

            Clear();
            instance._current.Clear();
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
