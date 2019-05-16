using BToolkitForWPF.Crypto;
using System;
using System.Collections.Generic;
using System.Data;

namespace BToolkitForWPF
{
    /// <summary>
    /// 用于账号密码登录的记住密码
    /// </summary>
    class RememberPassword
    {
        private const string Db_Name = "Data100";//数据库文件名
        private const string Db_PW = "saMf345dffhG34s";//数据库密码
        private const string Table_Name = "Login";//数据库文件名
        //字段
        private const string Key_string = "Key";
        private const string Value_string = "Value";
        //keys
        private const string Key_Account = "Account";
        private const string Key_Password = "Password";
        private const string Key_Remember = "Remember";
        private const string Key_AutoLogin = "AutoLogin";
        //是否使用加密存储
        private static bool encryption = true;
        private static SQLiteHelper db;

        /// <summary>
        /// 当前是否记住密码
        /// </summary>
        public static bool IsRemember
        {
            get
            {
                CheckDB();
                DataRow dataRow = db.SelectOneData(Table_Name, Key_string, Key_Remember);
                if (dataRow != null)
                {
                    return "1".Equals(dataRow[Value_string]);
                }
                return false;
            }
            set
            {
                if (db.IsContainsRow(Table_Name, Key_string, Key_Remember))
                {
                    db.UpdateData(Table_Name, Key_string, Key_Remember, new Dictionary<string, object>() { { Value_string, value ? "1" : "0" } });
                }
                else
                {
                    db.InsertData(Table_Name, new Dictionary<string, object>() { { Key_string, Key_Remember }, { Value_string, value ? "1" : "0" } });
                }
            }
        }

        /// <summary>
        /// 是否自动登录
        /// </summary>
        public static bool IsAutoLogin
        {
            get
            {
                CheckDB();
                DataRow dataRow = db.SelectOneData(Table_Name, Key_string, Key_AutoLogin);
                if (dataRow != null)
                {
                    return "1".Equals(dataRow[Value_string]);
                }
                return false;
            }
            set
            {
                if (db.IsContainsRow(Table_Name, Key_string, Key_AutoLogin))
                {
                    db.UpdateData(Table_Name, Key_string, Key_AutoLogin, new Dictionary<string, object>() { { Value_string, value ? "1" : "0" } });
                }
                else
                {
                    db.InsertData(Table_Name, new Dictionary<string, object>() { { Key_string, Key_AutoLogin }, { Value_string, value ? "1" : "0" } });
                }
            }
        }

        /// <summary>
        /// 读取账号
        /// </summary>
        public static string Account
        {
            get
            {
                CheckDB();
                DataRow dataRow = db.SelectOneData(Table_Name, Key_string, Key_Account);
                if (dataRow != null)
                {
                    return (string)dataRow[Value_string];
                }
                return null;
            }
            set
            {
                if (db.IsContainsRow(Table_Name, Key_string, Key_Account))
                {
                    db.UpdateData(Table_Name, Key_string, Key_Account, new Dictionary<string, object>() { { Value_string, value } });
                }
                else
                {
                    db.InsertData(Table_Name, new Dictionary<string, object>() { { Key_string, Key_Account }, { Value_string, value } });
                }
            }
        }

        /// <summary>
        /// 读取密码
        /// </summary>
        public static string Password
        {
            get
            {
                CheckDB();
                DataRow dataRow = db.SelectOneData(Table_Name, Key_string, Key_Password);
                if (dataRow != null)
                {
                    if (encryption)
                    {
                        return AES.Decrypt((string)dataRow[Value_string]);
                    }
                    else
                    {
                        return (string)dataRow[Value_string];
                    }
                }
                return null;
            }
            set
            {
                string saveValue = value;
                if (encryption)
                {
                    saveValue = AES.Encrypt(value);
                }
                if (db.IsContainsRow(Table_Name, Key_string, Key_Password))
                {
                    db.UpdateData(Table_Name, Key_string, Key_Password, new Dictionary<string, object>() { { Value_string, saveValue } });
                }
                else
                {
                    db.InsertData(Table_Name, new Dictionary<string, object>() { { Key_string, Key_Password }, { Value_string, saveValue } });
                }
            }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        private static void CheckDB()
        {
            if (db == null)
            {
                db = new SQLiteHelper(Db_Name, Db_PW);
                Dictionary<string, Type> keysAndTypes = new Dictionary<string, Type>();
                keysAndTypes.Add(Key_string, typeof(string));
                keysAndTypes.Add(Value_string, typeof(string));
                db.CreateTable(Table_Name, keysAndTypes);
            }
        }

    }
}
