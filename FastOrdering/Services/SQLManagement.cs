using FastOrdering.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace FastOrdering
{
    public class SampleOrderSQLManagement
    {

        //Item
        public ObservableCollection<SampleOrder> allItems = new ObservableCollection<SampleOrder>();
        //SQL查询模板
        private static String DB_NAME = "SampleOrder.db";
        private static String TABLE_NAME = "SampleOrder";
        private static String SQL_CREATE_TABLE = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " (ORDERID TEXT, ORDERNAME TEXT, SOLD TEXT, " +
            "VISITED TEXT, COLLECTED TEXT, PRICE TEXT, PICT TEXT, DETAILS TEXT, SUMMARY TEXT, ORDERED TEXT)";
        private static String SQL_INSERT = "INSERT INTO " + TABLE_NAME + " (ORDERID, ORDERNAME, SOLD, VISITED, COLLECTED, PRICE, PICT, DETAILS, SUMMARY, ORDERED) VALUES(?,?,?,?,?,?,?,?,?,?)";
        private static String SQL_UPDATE = "UPDATE " + TABLE_NAME + " SET ORDERID = ?, ORDERNAME = ?, SOLD = ?, VISITED = ?, COLLECTED = ?, PRICE = ?, PICT = ?, DETAILS = ?, SUMMARY = ?, ORDERED = ? WHERE ORDERID = ?";
        private static String SQL_DELETE = "DELETE FROM " + TABLE_NAME + " WHERE ORDERID = ?";
        private static String SQL_ALL_VALUE = "SELECT * FROM " + TABLE_NAME;
        private static String SQL_CLEAR = "DELETE FROM " + TABLE_NAME;
        private static String SQL_ACCURATE_QUERY_VALUE = "SELECT * FROM " + TABLE_NAME + " WHERE ORDERNAME = ?";

        private SQLiteConnection mySQL;
        private static SampleOrderSQLManagement instance;

        //单例模式
        public static SampleOrderSQLManagement GetInstance()
        {
            if (instance == null)
            {
                instance = new SampleOrderSQLManagement();
            }
            return instance;
        }

        //创建表单
        private SampleOrderSQLManagement()
        {
            mySQL = new SQLiteConnection(DB_NAME);
            using (var statement = mySQL.Prepare(SQL_CREATE_TABLE))
            {
                statement.Step();
            }
        }

        //清空
        public void clear()
        {
            using (var statement = mySQL.Prepare(SQL_CLEAR))
            {
                statement.Step();
            }
        }
        //插入表单
        public void insert(SampleOrder newOne)
        {
            //获得传入实例的值
            int orderId = newOne.OrderId;
            string orderName = newOne.OrderName;
            int sold = newOne.Sold;
            int visited = newOne.Visited;
            int collected = newOne.Collected;
            float price = newOne.Price;
            string path = newOne.imgPath;
            string details = newOne.Details;
            string summary = newOne.Summary;
            int ordered = newOne.Ordered;

            
            using (var statement = mySQL.Prepare(SQL_INSERT))
            {
                statement.Bind(1, orderId.ToString());
                statement.Bind(2, orderName);
                statement.Bind(3, sold.ToString());
                statement.Bind(4, visited.ToString());
                statement.Bind(5, collected.ToString());
                statement.Bind(6, price.ToString());
                statement.Bind(7, path);
                statement.Bind(8, details);
                statement.Bind(9, summary);
                statement.Bind(10, ordered.ToString());
                statement.Step();
            }
        }

        //删除表单
        public void delete(int id)
        {
            using (var statement = mySQL.Prepare(SQL_DELETE))
            {
                statement.Bind(1, id.ToString());
                statement.Step();
            }
        }

        //更新表单
        public void update(SampleOrder newOne)
        {
            int orderId = newOne.OrderId;
            string orderName = newOne.OrderName;
            int sold = newOne.Sold;
            int visited = newOne.Visited;
            int collected = newOne.Collected;
            float price = newOne.Price;
            string path = newOne.imgPath;
            string details = newOne.Details;
            string summary = newOne.Summary;
            int ordered = newOne.Ordered;
            using (var statement = mySQL.Prepare(SQL_UPDATE))
            {
                statement.Bind(1, orderId.ToString());
                statement.Bind(2, orderName);
                statement.Bind(3, sold.ToString());
                statement.Bind(4, visited.ToString());
                statement.Bind(5, collected.ToString());
                statement.Bind(6, price.ToString());
                statement.Bind(7, path);
                statement.Bind(8, details);
                statement.Bind(9, summary);
                statement.Bind(10, ordered.ToString());
                statement.Bind(11, orderId.ToString());
                statement.Step();
            }
        }

        //模糊查询
        public void queryItem(string orderName)
        {
            this.allItems.Clear();
            String SQL_QUERY_VALUE = "SELECT * FROM " + TABLE_NAME + " WHERE ORDERNAME LIKE '%" + orderName + "%';";
            using (var statement = mySQL.Prepare(SQL_QUERY_VALUE))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    int getOrderId = int.Parse((string)statement[0]);
                    string getOrderName = (string)statement[1];
                    int getSold = int.Parse((string)statement[2]);
                    int getVisited = int.Parse((string)statement[3]);
                    int getCollected = int.Parse((string)statement[4]);
                    float getPrice = float.Parse((string)statement[5]);
                    string getPath = (string)statement[6];
                    string getDetails = (string)statement[7];
                    string getSummary = (string)statement[8];
                    int getOrdered = int.Parse((string)statement[9]);
                    SampleOrder newOne = new SampleOrder
                    {
                        OrderId = getOrderId,
                        OrderName = getOrderName,
                        Sold = getSold,
                        Visited = getVisited,
                        Collected = getCollected,
                        Price = getPrice,
                        imgPath = getPath,
                        Details = getDetails,
                        Summary = getSummary,
                        Pict = new BitmapImage(new Uri(getPath)),
                        Ordered = getOrdered,
                };
                    allItems.Add(newOne);
                }
            }
        }

        ////精准查询
        ////查询
        //public void accurateQueryItem(string orderName)
        //{
        //    this.allItems.Clear();
        //    using (var statement = mySQL.Prepare(SQL_ACCURATE_QUERY_VALUE))
        //    {
        //        statement.Bind(1, orderName);
        //        while (SQLiteResult.ROW == statement.Step())
        //        {
        //            int getOrderId = int.Parse((string)statement[0]);
        //            string getOrderName = (string)statement[1];
        //            int getSold = int.Parse((string)statement[2]);
        //            int getVisited = int.Parse((string)statement[3]);
        //            int getCollected = int.Parse((string)statement[4]);
        //            float getPrice = float.Parse((string)statement[5]);
        //            string getPath = (string)statement[6];
        //            string getDetails = (string)statement[7];
        //            string getSummary = (string)statement[8];
        //            int getOrdered = int.Parse((string)statement[9]);
        //            SampleOrder newOne = new SampleOrder
        //            {
        //                OrderId = getOrderId,
        //                OrderName = getOrderName,
        //                Sold = getSold,
        //                Visited = getVisited,
        //                Collected = getCollected,
        //                Price = getPrice,
        //                imgPath = getPath,
        //                Details = getDetails,
        //                Summary = getSummary,
        //                Pict = new BitmapImage(new Uri(getPath)),
        //                Ordered = getOrdered,
        //            };
        //            allItems.Add(newOne);
        //        }
        //    }
        //}

        //得到全部
        public void GetAll()
        {
            this.allItems.Clear();
            using (var statement = mySQL.Prepare(SQL_ALL_VALUE))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    int getOrderId = int.Parse((string)statement[0]);
                    string getOrderName = (string)statement[1];
                    int getSold = int.Parse((string)statement[2]);
                    int getVisited = int.Parse((string)statement[3]);
                    int getCollected = int.Parse((string)statement[4]);
                    float getPrice = float.Parse((string)statement[5]);
                    string getPath = (string)statement[6];
                    string getDetails = (string)statement[7];
                    string getSummary = (string)statement[8];
                    int getOrdered = int.Parse((string)statement[9]);
                    SampleOrder newOne = new SampleOrder
                    {
                        OrderId = getOrderId,
                        OrderName = getOrderName,
                        Sold = getSold,
                        Visited = getVisited,
                        Collected = getCollected,
                        Price = getPrice,
                        imgPath = getPath,
                        Details = getDetails,
                        Summary = getSummary,
                        Pict = new BitmapImage(new Uri(getPath)),
                        Ordered = getOrdered,
                    };
                    allItems.Add(newOne);
                }
            }
        }
    }
    
    //public class UserOrderSQLManagement
    //{

    //    //Item
    //    public ObservableCollection<UserOrder> allItems = new ObservableCollection<UserOrder>();

    //    //SQL查询模板
    //    private static String DB_NAME = "UserOder.db";
    //    private static String TABLE_NAME = "UserOrder";
    //    private static String SQL_CREATE_TABLE = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " (ORDERID TEXT, USERNUM TEXT, TABLE TEXT, PEPPER TEXT, PRICE TEXT, DETAILS TEXT)";
    //    private static String SQL_INSERT = "INSERT INTO " + TABLE_NAME + " (ORDERID, USERNUM, TABLE, PEPPER, PRICE, DETAILS) VALUES(?,?,?,?,?,?)";
    //    private static String SQL_UPDATE = "UPDATE " + TABLE_NAME + " SET ORDERID = ?, USERNUM  = ?, TABLE = ?, PEPPER = ?, PRICE = ?, DETAILS = ? WHERE ORDERID = ?";
    //    private static String SQL_DELETE = "DELETE FROM " + TABLE_NAME + " WHERE ORDERID = ?";
    //    private static String SQL_ALL_VALUE = "SELECT * FROM " + TABLE_NAME;
    //    private static String SQL_CLEAR = "DELETE FROM " + TABLE_NAME;
    //    private static String SQL_ACCURATE_QUERY_VALUE = "SELECT * FROM " + TABLE_NAME + " WHERE ORDERNAME = ?";

    //    private SQLiteConnection mySQL;
    //    private static UserOrderSQLManagement instance;

    //    //单例模式
    //    public static UserOrderSQLManagement GetInstance()
    //    {
    //        if (instance == null)
    //        {
    //            instance = new UserOrderSQLManagement();
    //        }
    //        return instance;
    //    }

    //    //创建表单
    //    private UserOrderSQLManagement()
    //    {
    //        mySQL = new SQLiteConnection(DB_NAME);
    //        using (var statement = mySQL.Prepare(SQL_CREATE_TABLE))
    //        {
    //            statement.Step();
    //        }
    //    }

    //    //清空
    //    public void clear()
    //    {
    //        using (var statement = mySQL.Prepare(SQL_CLEAR))
    //        {
    //            statement.Step();
    //        }
    //    }
    //    //插入表单
    //    public void insert(UserOrder newOne)
    //    {
    //        //获得传入实例的值
    //        int orderId = newOne.OrderId;
    //        int userNum = newOne.UserNum;
    //        int table = newOne.Table;
    //        int pepper = newOne.Pepper;
    //        float price = newOne.Price;
    //        string details = newOne.Details;


    //        using (var statement = mySQL.Prepare(SQL_INSERT))
    //        {
    //            statement.Bind(1, orderId.ToString());
    //            statement.Bind(2, userNum.ToString());
    //            statement.Bind(3, table.ToString());
    //            statement.Bind(4, pepper.ToString());
    //            statement.Bind(5, price.ToString());
    //            statement.Bind(6, details);
    //            statement.Step();
    //        }
    //    }

    //    //删除表单
    //    public void delete(int id)
    //    {
    //        using (var statement = mySQL.Prepare(SQL_DELETE))
    //        {
    //            statement.Bind(1, id.ToString());
    //            statement.Step();
    //        }
    //    }

    //    //更新表单
    //    public void update(UserOrder newOne)
    //    {
    //        //获得传入实例的值
    //        int orderId = newOne.OrderId;
    //        int userNum = newOne.UserNum;
    //        int table = newOne.Table;
    //        int pepper = newOne.Pepper;
    //        float price = newOne.Price;
    //        string details = newOne.Details;
    //        using (var statement = mySQL.Prepare(SQL_UPDATE))
    //        {
    //            statement.Bind(1, orderId.ToString());
    //            statement.Bind(2, userNum.ToString());
    //            statement.Bind(3, table.ToString());
    //            statement.Bind(4, pepper.ToString());
    //            statement.Bind(5, price.ToString());
    //            statement.Bind(6, details);
    //            statement.Step();
    //        }
    //    }

    //    //得到全部
    //    public void GetAll()
    //    {
    //        this.allItems.Clear();
    //        using (var statement = mySQL.Prepare(SQL_ALL_VALUE))
    //        {
    //            while (SQLiteResult.ROW == statement.Step())
    //            {
    //                int getOrderId = int.Parse((string)statement[0]);
    //                int getUserNum = int.Parse((string)statement[1]);
    //                int getTable = int.Parse((string)statement[2]);
    //                int getPepper = int.Parse((string)statement[3]);
    //                float getPrice = float.Parse((string)statement[4]);
    //                string getDetails = (string)statement[5];
    //                UserOrder newOne = new UserOrder
    //                {
    //                    OrderId = getOrderId,
    //                    UserNum = getUserNum,
    //                    Table = getTable,
    //                    Pepper = getPepper,
    //                    Price = getPrice,
    //                    Details = getDetails,
                        
    //                };
    //                allItems.Add(newOne);
    //            }
    //        }
    //    }
    //}
}
