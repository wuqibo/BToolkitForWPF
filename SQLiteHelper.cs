using BToolkitForWPF.Crypto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace BToolkitForWPF
{
    /// <summary>
    /// 数据库操作助手（一个数据库new一个新实例）
    /// </summary>
    public class SQLiteHelper
    {
        /// <summary>
        /// 表格里的创建时间的字段名
        /// </summary>
        public const string Key_CreateTime = "CreateTime";

        /// <summary>
        /// 数据库目录
        /// </summary>
        private static string DbPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private string DbName;
        private string DbPassword;

        /// <summary>
        /// 创建一个新的数据库管理助手（一个库对应一个实例）
        /// </summary>
        public SQLiteHelper(string dbName, string password = null)
        {
            DbName = dbName;
            if (password != null)
            {
                DbPassword = MD5.Encrypt("ha" + password + "haha");
            }
            try
            {

                var fullPath = Path.Combine(DbPath, DbName);
                SQLiteConnection conn = new SQLiteConnection("Data Source=" + fullPath);
                conn.Open();
                if (!string.IsNullOrEmpty(DbPassword))
                {
                    conn.ChangePassword(DbPassword);
                }
            }
            catch
            {
                Console.WriteLine("数据库已存在");
            }
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsContainsTable(string tableName)
        {
            var fullPath = Path.Combine(DbPath, DbName);
            SQLiteConnection conn = new SQLiteConnection("data source=" + fullPath);
            CheckOpen(conn);
            string sql = "select count(*) from sqlite_master where type='table' and name='" + tableName + "'";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        /// <summary>
        /// 创建表(自动添加时间字段并启用默认值)
        /// </summary>
        public void CreateTable(string tableName, Dictionary<string, Type> keysAndTypes)
        {
            var fullPath = Path.Combine(DbPath, DbName);
            SQLiteConnection conn = new SQLiteConnection("data source=" + fullPath);
            CheckOpen(conn);
            string keysStr = "id integer primary key autoincrement";
            foreach (var item in keysAndTypes)
            {
                keysStr += "," + item.Key + " " + TypeToString(item.Value);
                string defaultValue = GetDefaultValue(item.Value);
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    keysStr += " default " + GetDefaultValue(item.Value);
                }
            }
            keysStr += ",CreateTime datetime default(datetime('now','localtime'))";
            string sql = "create table if not exists " + tableName + "(" + keysStr + ")";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        /// <summary>
        /// 删除表
        /// </summary>
        public void DeleteTable(string tableName)
        {
            var fullPath = Path.Combine(DbPath, DbName);
            SQLiteConnection conn = new SQLiteConnection("data source=" + fullPath);
            CheckOpen(conn);
            string sql = "drop table if exists " + tableName;
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        /// <summary>
        /// 判断行是否存在
        /// </summary>
        /// <returns></returns>
        public bool IsContainsRow(string tableName, string whereKey, object whereValue)
        {
            DataTable dataTable = SelectAllData(tableName, whereKey, whereValue);
            return dataTable.Rows.Count > 0;
        }

        /// <summary>
        /// 插入数据(枚举必须先转成Int)(返回本条对应的id)
        /// </summary>
        public long InsertData(string tableName, Dictionary<string, object> keysAndValues)
        {
            var fullPath = Path.Combine(DbPath, DbName);
            SQLiteConnection conn = new SQLiteConnection("data source=" + fullPath);
            CheckOpen(conn);
            //准备好key
            string keysStr = "";
            foreach (var item in keysAndValues)
            {
                if (item.Value != null)
                {
                    keysStr += "," + item.Key;
                }
            }
            if (keysStr.StartsWith(","))
            {
                keysStr = keysStr.Substring(1, keysStr.Length - 1);
            }
            else
            {
                Console.WriteLine(">>>>>>>>>>>插入数据失败：没有输入的值");
                return -1;
            }
            //准备好value
            string valuesStr = "";
            foreach (var item in keysAndValues)
            {
                if (item.Value != null)
                {
                    if (item.Value.GetType() == typeof(string))
                    {
                        valuesStr += ",'" + item.Value + "'";
                    }
                    else
                    {
                        valuesStr += "," + item.Value;
                    }
                }
            }
            valuesStr = valuesStr.Substring(1, valuesStr.Length - 1);
            string sql = "insert into " + tableName + "(" + keysStr + ") values(" + valuesStr + ")";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            //得到当前插入后自动累加的id
            DataRow dataRow = SelectOneData(tableName, null, null, "id");
            conn.Close();
            return (long)dataRow["id"];
        }

        /// <summary>
        /// 查询第一个符合的数据
        /// </summary>
        public DataRow SelectOneData(string tableName, string whereKey, object whereValue, string orderDescByKey = null)
        {
            DataTable datas = SelectData(tableName, whereKey, whereValue, orderDescByKey, 0, 1);
            if (datas.Rows.Count > 0)
            {
                return datas.Rows[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 查询所有符合的数据(整型返回int64)
        /// </summary>
        public DataTable SelectAllData(string tableName, string whereKey = null, object whereValue = null, string orderDescByKey = null, int page = 0, int pageSize = 0)
        {
            return SelectData(tableName, whereKey, whereValue, orderDescByKey, page, pageSize);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        private DataTable SelectData(string tableName, string whereKey, object whereValue, string orderDescByKey, int page = 0, int pageSize = 0)
        {
            if (IsContainsTable(tableName))
            {
                DataTable dataTable = new DataTable();
                var fullPath = Path.Combine(DbPath, DbName);
                using (SQLiteConnection conn = new SQLiteConnection("data source=" + fullPath))
                {
                    CheckOpen(conn);
                    string sql = "select * from " + tableName;
                    if (!string.IsNullOrEmpty(whereKey) && whereValue != null)
                    {
                        sql += " where " + whereKey + "='" + whereValue + "'";
                    }
                    if (!string.IsNullOrEmpty(orderDescByKey))
                    {
                        sql += " order by " + orderDescByKey + " desc";
                    }
                    if (page > 0 && pageSize > 0)
                    {
                        sql += " limit " + ((page - 1) * pageSize) + "," + pageSize;
                    }
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        try
                        {
                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                            adapter.Fill(dataTable);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(">>>>>>>>>>Exception:" + e);
                        }
                    }
                    conn.Close();
                }
                return dataTable;
            }
            return null;
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public void UpdateData(string tableName, string whereKey, object whereValue, Dictionary<string, object> keysAndValues)
        {
            var fullPath = Path.Combine(DbPath, DbName);
            SQLiteConnection conn = new SQLiteConnection("data source=" + fullPath);
            CheckOpen(conn);
            string keyValues = "";
            foreach (var item in keysAndValues)
            {
                if (item.Value.GetType() == typeof(string))
                {
                    keyValues += "," + item.Key + "='" + item.Value + "'";
                }
                else
                {
                    keyValues += "," + item.Key + "=" + item.Value;
                }
            }
            keyValues = keyValues.Substring(1, keyValues.Length - 1);
            string sql = "update " + tableName + " set " + keyValues + " where " + whereKey + " = '" + whereValue + "'";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public void DeleteData(string tableName, string key, object value)
        {
            var fullPath = Path.Combine(DbPath, DbName);
            SQLiteConnection conn = new SQLiteConnection("data source=" + fullPath);
            CheckOpen(conn);
            string sql = "delete from " + tableName + " where " + key + " = '" + value + "'";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        /// <summary>
        /// 检查打开状态
        /// </summary>
        private void CheckOpen(SQLiteConnection conn)
        {
            if (conn.State != ConnectionState.Open)
            {
                if (!string.IsNullOrEmpty(DbPassword))
                {
                    conn.SetPassword(DbPassword);
                }
                conn.Open();
            }
        }

        /// <summary>
        /// 类型转sql语句
        /// </summary>
        private string TypeToString(Type type)
        {
            if (type == typeof(string))
            {
                return "text";
            }
            else if (type == typeof(int))
            {
                return "int";
            }
            else if (type == typeof(long))
            {
                return "integer";
            }
            else if (type == typeof(float) || type == typeof(double))
            {
                return "real";
            }
            else if (type == typeof(bool))
            {
                Console.WriteLine(">>>>>>>>>>>>>>出错了：不支持bool类型存储，请使用int代替");
                return "integer";
            }
            return "bolb";//完全根据它的输入存储，二进制可用
        }

        /// <summary>
        /// 类型转sql语句
        /// </summary>
        private string GetDefaultValue(Type type)
        {
            if (type == typeof(string))
            {
                return "";
            }
            else if (type == typeof(int) || type == typeof(float) || type == typeof(double) || type == typeof(bool))
            {
                return "0";
            }
            return null;
        }
    }
}
