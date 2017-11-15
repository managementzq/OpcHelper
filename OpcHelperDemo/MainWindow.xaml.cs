using OpcHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpcHelperDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            intiOpcClientHelper();
            InitOpcDataitems();

        }

        private const string dateString = "yyyy-MM-dd HH:mm:ss ffff ";

        private void InitOpcDataitems()
        {
            StringBuilder sb = new StringBuilder();

#if DEBUG
            sb.Append("Channel_1.Device_1.Bool_1;1000;False;True;Unknow");
            sb.AppendLine();
            sb.Append("Channel_1.Device_1.Tag_1;1000;0;0;Unknow");
            sb.AppendLine();
            sb.Append("Channel_1.Device_1.Tag_2;1000;0;0;Unknow");
            sb.AppendLine();
            sb.Append("Channel_1.Device_1.Tag_3;1000;0;0;Unknow");
            sb.AppendLine();
            sb.Append("Channel_1.Device_1.Tag_4;1000;0;0;Unknow");
            sb.AppendLine();
            sb.Append("Channel_1.Device_1.Tag_5;1000;0;0;Unknow");
            sb.AppendLine();
            sb.Append("Channel1.Device1.Tag1;1000;0;0;Unknow");
            sb.AppendLine();
            sb.Append("Channel1.Device1.Tag2;1000;0;0;Unknow");
            sb.AppendLine();
            sb.Append("S7:[S7 connection_52]DB800,X0.1;1000;0;0;Unknow");
            sb.AppendLine();
            sb.Append("S7 [S7_connection_52]DB800,X0.2;1000;0;0;Unknow");
            sb.AppendLine();
            txtOpcDataItems.Text = sb.ToString();
#else 
            txtOpcDataItems.Text = System.IO.File.ReadAllText("数据点.txt");
#endif
            sb.Clear();
            sb = null;
        }

        private OpcHelper.OpcClientHelper opcClienthelper = new OpcHelper.OpcClientHelper();

        private const int updateRateGroup1 = 1000;
        private const int updateRateGroup2 = 1000;
        private const int updateRateGroup3 = 1000;

        private void intiOpcClientHelper()
        {
            opcClienthelper.OnLogHappened += OpcClienthelper_OnLogHappened;
            opcClienthelper.OnErrorHappened += OpcClienthelper_OnErrorHappened;
            opcClienthelper.OnDataChanged += OpcClienthelper_OnDataChanged;
            //var servers1 = OpcHelper.OpcClientHelper.GetOpcServers();
            //var servers2 = OpcHelper.OpcClientHelper.GetOpcServers("127.0.0.1");
            //clienthelper.Connect(servers1.First());
        }

        private void OpcClienthelper_OnLogHappened(object sender, OpcHelper.OpcLogEventArgs e)
        {
            string message = DateTime.Now.ToString(dateString) + e.Log + System.Environment.NewLine;
            try
            {
                UpMessage(message);
            }
            catch (AggregateException ex)
            {
                UpMessage(DateTime.Now.ToString(dateString) + ex.Message + System.Environment.NewLine);
            }
        }


        private void UpMessage(string message)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.txtMessage.Text = txtMessage.Text.Insert(0, message);
                int index = this.txtMessage.Text.LastIndexOf('\n');
                if (this.txtMessage.Text.LastIndexOf('\n') > 20000)
                {
                    this.txtMessage.Text = this.txtMessage.Text.Remove(index);
                }
            }));
        }


        private void UpOpcDataItemsMessage(IEnumerable <OpcDataItem > opcDataItem)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                gvOpcDataItems.ItemsSource = null;
                gvOpcDataItems.ItemsSource = opcDataItem;
                this.txtb.Text = "(" + opcDataItem.Count (a=>a.Quality == OpcResult.S_OK) + "/"+opcDataItem.Count()+")";
            }));
        }

        private void OpcClienthelper_OnErrorHappened(object sender, OpcHelper.OpcErrorEventArgs e)
        {
            string message = DateTime.Now.ToString(dateString) + e.Message + (e.Exception == null ? "" : e.Exception.StackTrace) + System.Environment.NewLine;
            try
            {
                UpMessage(message);
            }
            catch (AggregateException ex)
            {
                UpMessage(DateTime.Now.ToString(dateString) + ex.Message + System.Environment.NewLine);
            }
        }

        private void OpcClienthelper_OnDataChanged(object sender, OpcHelper.OpcDataEventArgs e)
        {
            string message = DateTime.Now.ToString(dateString) + e.OpcResult + " " + (e.OpcDataItem == null ? " " : e.OpcDataItem.ToString()) + System.Environment.NewLine;
            try
            {
                UpMessage(message);

                UpOpcDataItemsMessage(this.opcClienthelper.OpcDataItems);
            }
            catch (AggregateException ex)
            {
                UpMessage(DateTime.Now.ToString(dateString) + ex.Message + System.Environment.NewLine);
            }
        }



        /// <summary>
        /// 查询服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchOpcServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                
                var servers1 = OpcHelper.OpcClientHelper.GetOpcServers();
                //var servers2 = OpcHelper.OpcClientHelper.GetOpcServers("127.0.0.1");
                if (!Equals(null, servers1) && servers1.Count() > 0)
                {
                    foreach (var v in servers1)
                    {
                        string message = DateTime.Now.ToString(dateString) + "可用的OPC服务器：" + v + System.Environment.NewLine;
                        UpMessage(message);
                    }

                    cboxOpcServers.ItemsSource = null;

                    cboxOpcServers.ItemsSource = servers1;
                    if (servers1.Count() > 0)
                    {
                        cboxOpcServers.SelectedIndex = 0;
                    }
                }
                else
                {
                    UpMessage(DateTime.Now.ToString(dateString) + "未找到可用的OPC服务器。" + System.Environment.NewLine);
                }

            }
            catch (Exception ex)
            {
                UpMessage(DateTime.Now.ToString(dateString) + ex.Message + System.Environment.NewLine);
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnectOpcServer_Click(object sender, RoutedEventArgs e)
        {
            opcClienthelper.Connect(cboxOpcServers.SelectedItem == null ? null : cboxOpcServers.SelectedItem.ToString());
            //opcClienthelper.OnLogHappened += OpcClienthelper_OnLogHappened;
            //opcClienthelper.OnErrorHappened += OpcClienthelper_OnErrorHappened;
            //opcClienthelper.OnDataChanged += OpcClienthelper_OnDataChanged;

            //opcClienthelper.OnLogHappened += OpcClienthelper_OnLogHappened;
            //opcClienthelper.OnErrorHappened += OpcClienthelper_OnErrorHappened;
            //opcClienthelper.OnDataChanged += OpcClienthelper_OnDataChanged;
        }

        /// <summary>
        /// 断开服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDisConnectOpcServer_Click(object sender, RoutedEventArgs e)
        {
            //opcClienthelper.OnDataChanged -= OpcClienthelper_OnDataChanged;
            //opcClienthelper.OnErrorHappened -= OpcClienthelper_OnErrorHappened;
            //opcClienthelper.OnLogHappened -= OpcClienthelper_OnLogHappened;
            opcClienthelper.DisConnect();
            //opcClienthelper.OnLogHappened += OpcClienthelper_OnLogHappened;
            //opcClienthelper.OnErrorHappened += OpcClienthelper_OnErrorHappened;
            //opcClienthelper.OnDataChanged += OpcClienthelper_OnDataChanged;
        }

        /// <summary>
        /// 订阅数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddDataItems_Click(object sender, RoutedEventArgs e)
        {

            opcClienthelper.RegisterOpcDataItems(new List<OpcHelper.OpcDataItem> {
                new OpcHelper.OpcDataItem ("Channel_1.Device_1.Tag_1",updateRateGroup3,"","", OpcHelper.OpcResult.Unknow),
                new OpcHelper.OpcDataItem ("Channel_1.Device_1.Bool_1",updateRateGroup1,"","",OpcHelper.OpcResult.Unknow),
            });
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDispose_Click(object sender, RoutedEventArgs e)
        {
            opcClienthelper.Dispose();
        }

        /// <summary>
        /// 增加订阅数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>;
        private void btnReAddDataItems_Click(object sender, RoutedEventArgs e)
        {
            opcClienthelper.RegisterOpcDataItems(new List<OpcHelper.OpcDataItem> {
                new OpcHelper.OpcDataItem ("Channel_1.Device_1.Tag_1",updateRateGroup1,"","", OpcHelper.OpcResult.Unknow),
                new OpcHelper.OpcDataItem ("Channel_1.Device_1.Tag_2",updateRateGroup2,"","", OpcHelper.OpcResult.Unknow),
                new OpcHelper.OpcDataItem ("Channel_1.Device_1.Bool_1",updateRateGroup2,"","",OpcHelper.OpcResult.Unknow),
            });
        }

        /// <summary>
        /// 减少订阅数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>;
        private void btnDeleteDataItems_Click(object sender, RoutedEventArgs e)
        {
            opcClienthelper.RegisterOpcDataItems(new List<OpcHelper.OpcDataItem> {
                new OpcHelper.OpcDataItem ("Channel_1.Device_1.Tag_2",updateRateGroup1,"","", OpcHelper.OpcResult.Unknow),
                new OpcHelper.OpcDataItem ("Channel_1.Device_1.Bool_1",updateRateGroup1,"","",OpcHelper.OpcResult.Unknow),
            });
        }

        /// <summary>
        /// 取消所有订阅数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>;
        private void btnbtnNoDataItems_Click(object sender, RoutedEventArgs e)
        {
            opcClienthelper.RegisterOpcDataItems(new List<OpcHelper.OpcDataItem>
            {
                //new OpcHelper.OpcDataItem ("Channel_1.Device_1.Tag_2",100,"","", OpcHelper.OpcResult.Unknow),
                //new OpcHelper.OpcDataItem ("Channel_1.Device_1.Bool_1",200,"","",OpcHelper.OpcResult.Unknow),
            });
        }

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (opcClienthelper.IsConnected)
            {
                if (MessageBox.Show("正在通讯中，确定要退出么？\r\r退出后所有通讯将关闭！", "OPC测试助手", MessageBoxButton.OKCancel, MessageBoxImage.Question)
                      == MessageBoxResult.OK)
                {
                    //e.Cancel = true;
                    opcClienthelper.Dispose();
                }
                else
                {
                    e.Cancel = true;
                }
            }

            //opcClienthelper.OnDataChanged -= OpcClienthelper_OnDataChanged;
            //opcClienthelper.OnLogHappened -= OpcClienthelper_OnLogHappened;
            //opcClienthelper.OnDataChanged -= OpcClienthelper_OnDataChanged;
            //opcClienthelper.DisConnect();
            //opcClienthelper.Dispose();
        }

        /// <summary>
        /// 增加无效订阅数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>;
        private void btnAddInvalidDataItems_Click(object sender, RoutedEventArgs e)
        {
            opcClienthelper.RegisterOpcDataItems(new List<OpcHelper.OpcDataItem> {
                new OpcHelper.OpcDataItem ("Channel_1.Device_1.Tag_20",updateRateGroup1,"","", OpcHelper.OpcResult.Unknow),
                new OpcHelper.OpcDataItem ("Channel_1.Device_1.Bool_1",updateRateGroup1,"","",OpcHelper.OpcResult.Unknow),

            });
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>;
        private void btnWriteDataItem_Click(object sender, RoutedEventArgs e)
        {
            string message = null;
            if (!opcClienthelper.IsConnected)
            {
                message = DateTime.Now.ToString(dateString) + "请先连接服务器" + System.Environment.NewLine;
                UpMessage(message);
                return;
            }
            OpcDataItem opcDataItem = null;
            OpcResult opcResult = OpcResult.Unknow;
            if (Equals(null, opcClienthelper.OpcDataItems) || opcClienthelper.OpcDataItems.Count < 1)
            {
                message = DateTime.Now.ToString(dateString) + "没有数据点" + System.Environment.NewLine;
            }
            else
            {
                opcDataItem = opcClienthelper.OpcDataItems.FirstOrDefault().Clone() as OpcDataItem;
                //bool newValue = (DateTime.Now.Millisecond % 2) == 0 ? true : false;
                //bool newValue = !tmpValue;
                //tmpValue = newValue;
                //System.Diagnostics.Debug.Print(tmpValue.ToString());
                opcResult = opcClienthelper.Write(opcDataItem, 1);
                message = DateTime.Now.ToString(dateString) + "写入完成 " + opcResult + " " + (opcDataItem == null ? " " : opcDataItem.ToString()) + System.Environment.NewLine;

            }
            UpMessage(message);
        }

        /// <summary>
        /// 读取实时数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadDataItem_Click(object sender, RoutedEventArgs e)
        {
            string message = null;
            if (!opcClienthelper.IsConnected)
            {
                message = DateTime.Now.ToString(dateString) + "请先连接服务器" + System.Environment.NewLine;
                UpMessage(message);
                return;
            }
            OpcDataItem opcDataItem;
            if (Equals(null, opcClienthelper.OpcDataItems) || opcClienthelper.OpcDataItems.Count < 1)
            {
                message = DateTime.Now.ToString(dateString) + "没有数据点" + System.Environment.NewLine;
            }
            else
            {
                //正常读取
                opcDataItem = opcClienthelper.OpcDataItems.FirstOrDefault().Clone() as OpcDataItem;
                opcDataItem.Name = opcDataItem.Name;
                opcDataItem = opcClienthelper.Read(opcDataItem);
                message = DateTime.Now.ToString("HH:mm:ss ffff ") + "读完成 " + (opcDataItem == null ? " " : opcDataItem.ToString()) + System.Environment.NewLine;
            }
            UpMessage(message);
            if (!Equals(null, opcClienthelper.OpcDataItems) && opcClienthelper.OpcDataItems.Count > 0)
            {
                //无效读取
                var opcDataItem2 = opcClienthelper.OpcDataItems.LastOrDefault().Clone() as OpcDataItem;
                opcDataItem2.Name = opcDataItem2.Name + "xxx";
                opcDataItem2 = opcClienthelper.Read(opcDataItem2);
                message = DateTime.Now.ToString(dateString) + "读完成 " + (opcDataItem2 == null ? " " : opcDataItem2.ToString()) + System.Environment.NewLine;
                UpMessage(message);
            }
        }

        /// <summary>
        /// 读取缓存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadCacheDataItems_Click(object sender, RoutedEventArgs e)
        {
            string message;
            if (!opcClienthelper.IsConnected)
            {
                message = DateTime.Now.ToString(dateString) + "请先连接服务器" + System.Environment.NewLine;
                UpMessage(message);
                return;
            }
            if (Equals(null, opcClienthelper.OpcDataItems) || opcClienthelper.OpcDataItems.Count < 1)
            {
                message = DateTime.Now.ToString(dateString) + "没有数据点" + System.Environment.NewLine;
            }
            else
            {
                var opcDataItem = opcClienthelper.OpcDataItems.FirstOrDefault().Clone() as OpcDataItem;
                message = DateTime.Now.ToString(dateString) + "读完成 " + (opcDataItem == null ? " " : opcDataItem.ToString()) + System.Environment.NewLine;
            }
            UpMessage(message);
        }

        private void btnUpdateDataItems_Click(object sender, RoutedEventArgs e)
        {
            string message;
            if (!opcClienthelper.IsConnected)
            {
                message = DateTime.Now.ToString(dateString) + "请先连接服务器" + System.Environment.NewLine;
                UpMessage(message);
                return;
            }
            var strList = txtOpcDataItems.Text.Split('\r', '\n');

            List<OpcDataItem> opcDataItems = new List<OpcDataItem>(strList.Count());
            //txtOpcDataItems .Text.Split (System.Environment.NewLine ):
            foreach (var strOpcDataItem in strList)
            {
                var strOpcDataItemTmp = strOpcDataItem.Split(';');
                if (strOpcDataItemTmp.Count() < 2)
                {
                    continue;
                }
                OpcDataItem opcDataItem =
                    new OpcDataItem(strOpcDataItemTmp[0], int.Parse(strOpcDataItemTmp[1]), strOpcDataItemTmp[2], strOpcDataItemTmp[3], (OpcResult)Enum.Parse(typeof(OpcResult), strOpcDataItemTmp[4]));
                opcDataItems.Add(opcDataItem);
            }
            opcClienthelper.RegisterOpcDataItems(opcDataItems);
        }
    }
}
