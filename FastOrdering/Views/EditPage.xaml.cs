using FastOrdering.Models;
using FastOrdering.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FastOrdering.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EditPage : Page
    {
        //是否添加了图片
        public bool isAddPic = false;
        //单实例
        SampleDataService instance = SampleDataService.getInstance();
        //默认的图片uri
        private string defaultpath = "ms-appx:///Assets/newOne.jpg";
        //mySQL
        private SampleOrderSQLManagement mySQL = SampleOrderSQLManagement.getInstance();

        public EditPage()
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
                ApplicationData.Current.LocalSettings.Values["Newpage"] = composite;
                ((App)App.Current).issuspend = false;
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("ThisMyToken"))
                    ApplicationData.Current.LocalSettings.Values.Remove("ThisMyToken");
            }
        }

        //恢复挂起状态
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("NewPage"))
                    ApplicationData.Current.LocalSettings.Values.Remove("Newpage");
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("ThisMyToken"))
                    ApplicationData.Current.LocalSettings.Values.Remove("ThisMyToken");
                if (e.Parameter != null)
                {
                    Frame rootFrame = Window.Current.Content as Frame;
                    base.OnNavigatedTo(e);
                    create.Content = "修改";
                    delete_bar.Visibility = Visibility.Visible;
                    title.Text = instance._current.OrderName;
                    price.Text = instance._current.Price.ToString();
                    summary.Text = instance._current.Summary;
                    details.Text = instance._current.Details;
                    myImg.Source = instance._current.Pict;
                }
                else
                {
                    delete_bar.Visibility = Visibility.Collapsed;
                }

            }
            else
            {
                var composite = ApplicationData.Current.LocalSettings.Values["Newpage"] as ApplicationDataCompositeValue;
                title.Text = (string)composite["title"];
                price.Text = (string)composite["price"];
                summary.Text = (string)composite["summary"];
                details.Text = (string)composite["details"];
                isAddPic = (bool)composite["isAddPic"];
                if (isAddPic)
                {
                    StorageFile theFile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync((string)ApplicationData.Current.LocalSettings.Values["ThisMyToken"]);
                    BitmapImage bitmap = new BitmapImage();
                    using (var stream = await theFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                    {
                        bitmap.SetSource(stream);
                    }
                    this.myImg.Source = bitmap;
                }
            }
            ApplicationData.Current.LocalSettings.Values.Remove("Newpage");
            ApplicationData.Current.LocalSettings.Values.Remove("ThisMyToken");
        }

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
        private void clear()
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
                Title = "非法价格",
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
                Title = "非法价格",
                Content = "请输入数字",
                PrimaryButtonText = "好"
            };
            ContentDialogResult result = await ErrTransform.ShowAsync();
        }

        //成功添加或者修改item
        private async void AccessDate()
        {
            //成功创建item后可以删除临时图片的Token，addPic为false
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MyToken"))
                ApplicationData.Current.LocalSettings.Values.Remove("MyToken");

            isAddPic = false;
            //判断此时是创建还是修改
            if ((string)create.Content == "创建")
            {
                SampleOrder newOne = new SampleOrder
                {
                    OrderName = title.Text,
                    Price = Convert.ToSingle(price.Text),
                    Summary = summary.Text,
                    Details = details.Text,
                    Pict = new BitmapImage(new Uri(defaultpath)),
                    imgPath = defaultpath
                };
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
                clear();
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
                clear();
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
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
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
                else AccessDate();
            }
        }

        private void cancelBtn(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private async void deleteItem(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            var originalSource = e.OriginalSource as MenuFlyoutItem;
            SampleOrder data = (SampleOrder)originalSource.DataContext;
            instance._current = data;
            mySQL.delete(data.OrderId);
            instance.allItems.Remove(instance._current);
            clear();
            //instance.newTile();
            delete_bar.Visibility = Visibility.Collapsed;
            ContentDialog Delete_btn = new ContentDialog
            {
                Title = "删除成功",
                Content = "您的菜品已经删除成功",
                PrimaryButtonText = "好"
            };
            await Delete_btn.ShowAsync();
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        //点击下方bar的删除按钮
        private async void Delete_btn(object sender, RoutedEventArgs e)
        {
            mySQL.delete(instance._current.OrderId);
            instance.allItems.Remove(instance._current);
            clear();
            //instance.newTile();
            delete_bar.Visibility = Visibility.Collapsed;
            ContentDialog Delete_btn = new ContentDialog
            {
                Title = "删除成功",
                Content = "成功删除菜品",
                PrimaryButtonText = "好"
            };
            await Delete_btn.ShowAsync();
        }
    }
}
