using FastOrdering.Models;
using FastOrdering.Services;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace FastOrdering.Views
{
    public sealed partial class ManagementPage : Page, INotifyPropertyChanged
    {
        //是否添加了图片
        public bool isAddPic = false;
        //单实例
        SampleDataService instance = SampleDataService.GetInstance();
        //默认的图片uri
        private string defaultpath = "ms-appx:///Assets/newOne.jpg";
        //mySQL
        private SampleOrderSQLManagement mySQL = SampleOrderSQLManagement.GetInstance();
        private bool isEdit = false;

        public ManagementPage()
        {
            InitializeComponent();
            ImageSource Pict = new BitmapImage(new Uri(defaultpath));
            myImg.Source = Pict;
            //Loaded += Management_Loaded;
        }

        //挂起保存
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //判断是否是挂起状态
            bool suspending = ((App)App.Current).issuspend;
            if (suspending)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                composite["title"] = title.Text;
                composite["price"] = price.Text;
                composite["summary"] = summary.Text;
                composite["details"] = details.Text;
                composite["isAddPic"] = isAddPic;
                //如果是编辑模式需要保存当前项目的状态
                composite["isEdit"] = isEdit;
                if (isEdit)
                {
                    composite["current"] = instance.allItems.IndexOf(instance._current);
                }
                ApplicationData.Current.LocalSettings.Values["newpage"] = composite;
                ((App)App.Current).issuspend = false;
            }
            else
            {
                //如果不是挂起状态，移除临时保存图片的Token
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MyToken"))
                    ApplicationData.Current.LocalSettings.Values.Remove("MyToken");
            }
        }

        //恢复挂起状态
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //判断是否是挂起状态保存
            if (e.NavigationMode == NavigationMode.New)
            {
                //如果不是挂起状态，移除newPage和 Token

                ApplicationData.Current.LocalSettings.Values.Remove("newpage");
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MyToken"))
                    ApplicationData.Current.LocalSettings.Values.Remove("MyToken");

            }
            else
            {
                //判断是挂起状态跳转还是NewPage跳转
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("newpage"))
                {
                    //读取之前保存的Composite
                    var composite = ApplicationData.Current.LocalSettings.Values["newpage"] as ApplicationDataCompositeValue;
                    title.Text = (string)composite["title"];
                    price.Text = (string)composite["price"];
                    summary.Text = (string)composite["summary"];
                    details.Text = (string)composite["details"];

                    //判断挂起前是否有新增加了图片
                    isAddPic = (bool)composite["isAddPic"];
                    if (isAddPic)
                    {
                        StorageFile theFile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync((string)ApplicationData.Current.LocalSettings.Values["MyToken"]);
                        BitmapImage bitmap = new BitmapImage();
                        //图片流入
                        using (var stream = await theFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                        {
                            bitmap.SetSource(stream);
                        }
                        this.myImg.Source = bitmap;
                    }

                    //创建磁贴
                    isEdit = (bool)composite["isEdit"];
                    if (isEdit)
                    {
                        int index = (int)composite["current"];
                        instance._current = instance.allItems[index];
                    }
                }
            }
            //读取后删除composite和Token
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("newpage"))
                ApplicationData.Current.LocalSettings.Values.Remove("newpage");
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MyToken"))
                ApplicationData.Current.LocalSettings.Values.Remove("MyToken");
        }



        //开始编辑的函数和变量
        //订单集合
        //public ObservableCollection<SampleOrder> SampleItems { get; private set; } = new ObservableCollection<SampleOrder>();
        //private async void Management_Loaded(object sender, RoutedEventArgs e)
        //{
        //    SampleItems.Clear();

        //    var data = await SampleDataService.GetSampleModelDataAsync();

        //    foreach (var item in data)
        //    {
        //        SampleItems.Add(item);
        //    }
        //}

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

        private async void addPic(object sender, RoutedEventArgs e)
        {
            isAddPic = true;
            //打开资源管理器
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //可以添加的图片了诶性
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".gif");
            Windows.Storage.StorageFile file = await filePicker.PickSingleFileAsync();
            //判断用户是否添加了图片
            if (file != null)
            {
                //新建Token保存图片，以方便挂起状态恢复
                ApplicationData.Current.LocalSettings.Values["MyToken"] = StorageApplicationPermissions.FutureAccessList.Add(file);
                BitmapImage bitmap = new BitmapImage();
                using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                {
                    bitmap.SetSource(stream);
                    this.myImg.Source = bitmap;
                }
                //将图片复制到缓存的文件夹，以解决文件权限的问题
                StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                StorageFolder folder = await applicationFolder.CreateFolderAsync("pic", CreationCollisionOption.OpenIfExists);
                StorageFile saveFile = await folder.CreateFileAsync(file.Name, CreationCollisionOption.OpenIfExists);
                RenderTargetBitmap tempbitmap = new RenderTargetBitmap();
                await tempbitmap.RenderAsync(myImg);
                var pixelBuffer = await tempbitmap.GetPixelsAsync();
                using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Ignore,
                        (uint)tempbitmap.PixelWidth,
                        (uint)tempbitmap.PixelHeight,
                        DisplayInformation.GetForCurrentView().LogicalDpi,
                        DisplayInformation.GetForCurrentView().LogicalDpi,
                        pixelBuffer.ToArray()
                        );
                    await encoder.FlushAsync();
                }
                //记录缓存文件夹的路径
                defaultpath = folder.Path.ToString() + '/' + file.Name;
            }
            else//用户使用默认的图片
            {
                defaultpath = "ms-appx:///Assets/newOne.jpg";
            }
        }

        //清除右边的编辑栏
        private void Clear()
        {
            create.Content = "创建";
            title.Text = "";
            price.Text = "";
            summary.Text = "";
            details.Text = "";
            string ImgSource = "ms-appx:///Assets/newOne.jpg";
            ImageSource Pict = new BitmapImage(new Uri(ImgSource));
            myImg.Source = Pict;
            isAddPic = false;
        }

        //Title为空
        private async void EmptyTitle()
        {
            ContentDialog EmptyTitle = new ContentDialog()
            {
                Title = "标题为空",
                Content = "请添加标题",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await EmptyTitle.ShowAsync();
        }

        //Price为空
        private async void EmptyPrice()
        {
            ContentDialog EmptyPrice = new ContentDialog()
            {
                Title = "价格为空",
                Content = "请添加价格",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await EmptyPrice.ShowAsync();
        }

        //Summary为空
        private async void EmptySummary()
        {
            ContentDialog EmptySummary = new ContentDialog()
            {
                Title = "简介为空",
                Content = "请添加简介",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await EmptySummary.ShowAsync();
        }

        //Detail为空
        private async void EmptyDetails()
        {
            ContentDialog EmptyDetails = new ContentDialog()
            {
                Title = "详情为空",
                Content = "请添加详情",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await EmptyDetails.ShowAsync();
        }

        //价格小于0
        private async void ErrorPrice()
        {
            ContentDialog ErrorPrice = new ContentDialog()
            {
                Title = "非法价格输入",
                Content = "请输入大于0的价格",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await ErrorPrice.ShowAsync();
        }

        //转化错误
        private async void ErrTransform()
        {
            ContentDialog ErrTransform = new ContentDialog()
            {
                Title = "非法价格输入",
                Content = "请输入数字",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await ErrTransform.ShowAsync();
        }

        //成功添加或者修改item
        private async void AccessItem()
        {
            isEdit = false;
            //成功创建item后可以删除临时图片的Token，addPic为false
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MyToken"))
                ApplicationData.Current.LocalSettings.Values.Remove("MyToken");

            isAddPic = false;
            //判断此时是创建还是修改
            if ((string)create.Content == "创建")
            {
                //添加项目，id+1
                SampleOrder newOne = new SampleOrder(true);
                newOne.OrderName = title.Text;
                newOne.Price = Convert.ToSingle(price.Text);
                newOne.Summary = summary.Text;
                newOne.Details = details.Text;
                newOne.Pict = new BitmapImage(new Uri(defaultpath));
                newOne.imgPath = defaultpath;
                mySQL.insert(newOne);
                instance.allItems.Add(newOne);
                //更新磁贴
                //instance.newTile();
                ContentDialog AccessDate = new ContentDialog()
                {
                    Title = "添加成功",
                    Content = "您的菜品已经添加成功",
                    PrimaryButtonText = "好"
                };
                Clear();
                await AccessDate.ShowAsync();
            }
            else//修改item
            {
                instance.allItems[instance.allItems.IndexOf(instance._current)].Pict = new BitmapImage(new Uri(defaultpath));
                instance.allItems[instance.allItems.IndexOf(instance._current)].OrderName = title.Text;
                instance.allItems[instance.allItems.IndexOf(instance._current)].Summary = summary.Text;
                instance.allItems[instance.allItems.IndexOf(instance._current)].Details = details.Text;
                instance.allItems[instance.allItems.IndexOf(instance._current)].Price = Convert.ToSingle(price.Text);
                instance.allItems[instance.allItems.IndexOf(instance._current)].imgPath = defaultpath;
                mySQL.update(instance.allItems[instance.allItems.IndexOf(instance._current)]);
                //隐藏删除按钮
                delete_bar.Visibility = Visibility.Collapsed;
                //清空创建界面
                Clear();
                //更新磁贴
                //instance.newTile();
                ContentDialog AccessDate = new ContentDialog()
                {
                    Title = "更改成功！",
                    Content = "您的菜品已经更改成功",
                    PrimaryButtonText = "好"
                };
                await AccessDate.ShowAsync();
            }
        }

        //按下create按钮创建项目
        private void createBtn(object sender, RoutedEventArgs e)
        {
            //标题为空
            if (title.Text == "")
            {
                EmptyTitle();
                return;
            }
            //价格为空
            else if (price.Text == "")
            {
                EmptyPrice();
                return;
            }
            //简述为空
            else if (summary.Text == "")
            {
                EmptySummary();
                return;
            }
            //详情为空
            else if (details.Text == "")
            {
                EmptyDetails();
                return;
            }
            //转化如错误则提示并返回，否则记录数据
            else
            {
                float f;
                if (!float.TryParse(price.Text, out f))
                {
                    ErrTransform();
                }
                else if (f < 0)
                {
                    ErrorPrice();
                }
                else AccessItem();
            }
        }

        private void cancelBtn(object sender, RoutedEventArgs e)
        {
            Clear();
            if (isEdit) create.Content = "Update";
            else create.Content = "Create";
        }

        private void editItem(object sender, RoutedEventArgs e)
        {
            isEdit = true;
            //获得此项目
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            var originalSource = e.OriginalSource as MenuFlyoutItem;
            SampleOrder data = (SampleOrder)originalSource.DataContext;
            instance._current = data;

            //判断此时是否应该跳转
            if (Window.Current.Bounds.Width >= 1200)
            {
                //显示详情
                create.Content = "修改";
                delete_bar.Visibility = Visibility.Visible;
                title.Text = instance._current.OrderName;
                price.Text = instance._current.Price.ToString();
                summary.Text = instance._current.Summary;
                details.Text = instance._current.Details;
                myImg.Source = instance._current.Pict;
            }
            else//跳转
            {
                Frame.Navigate(typeof(EditPage), "aaaa");
            }
        }

        private async void deleteItem(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            var originalSource = e.OriginalSource as MenuFlyoutItem;
            SampleOrder data = (SampleOrder)originalSource.DataContext;
            instance._current = data;
            ContentDialog ErrorDialog = new ContentDialog
            {
                Title = "注意！删除",
                Content = "你将会删除该菜品",
                PrimaryButtonText = "取消",
                SecondaryButtonText = "确认删除"
            };
            ContentDialogResult result = await ErrorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return;
            mySQL.delete(data.OrderId);
            instance.allItems.Remove(instance._current);
            Clear();
            //instance.newTile();
            delete_bar.Visibility = Visibility.Collapsed;  
        }

        //分享按钮
        private void shareItem(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            var originalSource = e.OriginalSource as MenuFlyoutItem;
            SampleOrder data = (SampleOrder)originalSource.DataContext;
            instance._current = data;
            dataTransferManager.DataRequested += DataRequested;
            DataTransferManager.ShowShareUI();
        }

        //程序之间的沟通
        private async void DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            //标题
            DataRequest request = args.Request;
            request.Data.Properties.Title = "我家小店这一道菜非常棒！\n菜品名：" + instance._current.OrderName + "\n";
            //详情
            string send = instance._current.Summary + "\n" + instance._current.Details;
            request.Data.SetText(send);
            //添加图片到邮件
            RandomAccessStreamReference bitmap;
            //判断是否是默认图片，如果不是需要打开folder以获得图片的权限
            if (instance._current.imgPath == "ms-appx:///Assets/newOne.jpg")
            {
                bitmap = RandomAccessStreamReference.CreateFromUri(new Uri(instance._current.imgPath));
            }
            else
            {
                StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
                StorageFolder folder = await applicationFolder.GetFolderAsync("pic");
                string folder_str = folder.Path.ToString() + '/';
                string myUri = instance._current.imgPath;
                //获得此时的文件名
                string file_name = myUri.ToString().Replace(folder_str.ToString(), "");
                StorageFile logoImage = await folder.GetFileAsync(file_name);
                bitmap = RandomAccessStreamReference.CreateFromFile(logoImage);
            }
            request.Data.SetBitmap(bitmap);

        }

        //清除所有项目
        private async void clearDataBase(object sender, RoutedEventArgs e)
        {
            ContentDialog ErrorDialog = new ContentDialog
            {
                Title = "注意！删除",
                Content = "你将会删除所有菜品信息",
                PrimaryButtonText = "取消",
                SecondaryButtonText = "确认删除"
            };
            ContentDialogResult result = await ErrorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return;
            mySQL.clear();
            instance.allItems.Clear();
            isEdit = false;
            Clear();
            SampleOrder.id = 0;
            //instance.newTile();
        }

        //点击显示详情
        private void showDetails(object sender, ItemClickEventArgs e)
        {
            isEdit = true;
            //获得此项目
            instance._current = (SampleOrder)e.ClickedItem;

            //判断此时是否应该跳转
            if (Window.Current.Bounds.Width >= 1200)
            {
                //显示详情
                create.Content = "修改";
                delete_bar.Visibility = Visibility.Visible;
                title.Text = instance._current.OrderName;
                price.Text = instance._current.Price.ToString();
                summary.Text = instance._current.Summary;
                details.Text = instance._current.Details;
                myImg.Source = instance._current.Pict;
            }
            else//跳转
            {
                Frame.Navigate(typeof(EditPage), "aaaa");
            }
        }

        private void navigate(object sender, RoutedEventArgs e)
        {
            delete_bar.Visibility = Visibility.Collapsed;
            //如果窗口大小比较大则弹出右侧的新建窗口
            if (Window.Current.Bounds.Width >= 1200)
            {
                Clear();

                //隐藏下方bar的删除按钮
                delete_bar.Visibility = Visibility.Collapsed;
            }
            else
            {
                //跳转新页面
                this.Frame.Navigate(typeof(EditPage));
            }
        }

        //点击下方bar的删除按钮
        private async void Delete_btn(object sender, RoutedEventArgs e)
        {
            ContentDialog ErrorDialog = new ContentDialog
            {
                Title = "注意！删除",
                Content = "你将会删除该菜品",
                PrimaryButtonText = "取消",
                SecondaryButtonText = "确认删除"
            };
            ContentDialogResult result = await ErrorDialog.ShowAsync();
            if (result == ContentDialogResult.Primary) return;
            isEdit = false;
            mySQL.delete(instance._current.OrderId);
            instance.allItems.Remove(instance._current);
            Clear();
            //instance.newTile();
            delete_bar.Visibility = Visibility.Collapsed;

        }

        //数据库模糊搜索
        private void queryItem(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var text = SearchBox.Text;
            instance.allItems.Clear();
            if (text != "")
            {
                mySQL.queryItem(text);
                if (mySQL.allItems.Count == 0) return;
            }
            else mySQL.GetAll();
            for (int i = 0; i < mySQL.allItems.Count; i++)
            {
                instance.allItems.Add(mySQL.allItems[i]);
            }
        }

        //项目上移
        private void Upward(object sender, RoutedEventArgs e)
        {
            var originalSource = e.OriginalSource as Button;
            int currentID = (int)originalSource.DataContext;
            int pos = -1;
            //利用id的唯一性，寻找该菜品在菜单中的位置
            for (int i = 0; i < instance.allItems.Count; ++i)
            {
                if (instance.allItems[i].OrderId == currentID)
                {
                    pos = i;
                }
            }
            if(pos == 0)
            {
                return;
            }
            else
            {
                SampleOrder newItem = instance.allItems[pos];
                instance.allItems[pos] = instance.allItems[pos - 1];
                instance.allItems[pos - 1] = newItem;
                mySQL.update(instance.allItems[pos - 1]);
                mySQL.update(instance.allItems[pos]);

            }
        }

        private void Downward(object sender, RoutedEventArgs e)
        {

            var originalSource = e.OriginalSource as Button;
            int currentID = (int)originalSource.DataContext;
            int pos = -1;
            //利用id的唯一性，寻找该菜品在菜单中的位置
            for (int i = 0; i < instance.allItems.Count; ++i)
            {
                if (instance.allItems[i].OrderId == currentID)
                {
                    pos = i;
                }
            }
            if (pos == instance.allItems.Count - 1)
            {
                return;
            }
            else
            {
                SampleOrder newItem = instance.allItems[pos];
                instance.allItems[pos] = instance.allItems[pos + 1];
                instance.allItems[pos + 1] = newItem;
                mySQL.update(instance.allItems[pos + 1]);
                mySQL.update(instance.allItems[pos]);
            }         
        }
    }
}
