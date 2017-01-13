/**
* 命名空间: CANNetwork.DataClass
*
* 功 能： N/A
* 类 名： AccessHelper
*
* Ver 变更日期 负责人 变更内容
* ───────────────────────────────────
* V1.0.0 2017/1/9 11:54:09 tsunami(HHH)
*
* Copyright (c) 2016 Lir Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：广州市富元电子有限公司 　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CANNetwork.DataClass
{
        /// <summary>
        /// 数据库功能
        /// </summary>
        public static class AccessHelper
        {
            /// <summary>
            /// mdb文件路径
            /// </summary>
            private static string mdbpath = Directory.GetCurrentDirectory() + "\\dataFile\\dataFile.mdb";

            /// <summary>
            /// 实例化Db连接对象
            /// </summary>
            private static OleDbConnection aConnection;

            /// <summary>
            /// 实例化Db命令对象,数据流数据
            /// </summary>
            private static OleDbCommand aCommand = new OleDbCommand();

            /// <summary>
            /// 绑定报告管理数据表
            /// </summary>
            private static DataTable CharViewDt;

            /// <summary>
            /// 填充数据流
            /// </summary>
            private static DataSet myDataSet;



            /// <summary>
            /// 创建mdb
            /// </summary>
            /// <param name="mdbPath"></param>
            /// <returns></returns>
            private static bool CreateMDBDataBase()
            {
                try
                {

                    //ADOX需添加com程序集,CatalogClass须将ADO属性的嵌入互操作类型置为false.
                    //  FileHelper.CreateDirectory(Directory.GetCurrentDirectory() + "/AccessFile/");
                    if (!File.Exists(@mdbpath))
                    {
                        ADOX.CatalogClass cat = new ADOX.CatalogClass();
                        cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbpath + ";");
                        cat = null;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ei)
                {
                    MessageBox.Show(ei.Message);
                    return false;
                }
            }

            /// <summary>
            /// 新建mdb表，mdbHead是一个ArrayList，存储的是table表中的具体列名
            /// </summary>
            /// <param name="mdbPath"></param>
            /// <param name="tableName"></param>
            /// <param name="mdbHead"></param>
            /// <returns></returns>
            private static bool CreateMDBTable(string tableName, ArrayList mdbHead)
            {
                //ADODB需添加com程序集
                try
                {
                    ADOX.CatalogClass cat = new ADOX.CatalogClass();
                    string sAccessConnection
                        = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbpath;
                    ADODB.Connection cn = new ADODB.Connection();
                    cn.Open(sAccessConnection, null, null, 0);
                    cat.ActiveConnection = cn;

                    //新建一个表   
                    ADOX.TableClass tbl = new ADOX.TableClass();
                    tbl.ParentCatalog = cat;
                    tbl.Name = tableName;

                    int size = mdbHead.Count;

                    for (int i = 0; i < size; i++)
                    {
                        //增加一个文本字段   
                        ADOX.ColumnClass col2 = new ADOX.ColumnClass();
                        ADOX.IndexClass ind2 = new ADOX.IndexClass();
                        col2.ParentCatalog = cat;
                        col2.Name = mdbHead[i].ToString();//列的名称   

                        col2.Properties["Jet OLEDB:Allow Zero Length"].Value = false;
                        col2.DefinedSize = 50;
                        if (i == 0)
                        {

                            //   col2.Type = ADOX.DataTypeEnum.adSingle;
                            //   col2.Type = ADOX.DataTypeEnum.adInteger;
                            //   col2.Properties["AutoIncrement"].Value = true;
                            tbl.Keys.Append("PrimaryKey", ADOX.KeyTypeEnum.adKeyPrimary, "通道");

                        }
                        col2.SortOrder = ADOX.SortOrderEnum.adSortAscending;
                        //   col2.DefinedSize = 20;
                        //  col2.Type = ADOX.DataTypeEnum.adGUID;
                        tbl.Columns.Append(col2, ADOX.DataTypeEnum.adWChar, 50);

                        //    tbl.Indexes.Append(col2 as );
                        //            tbl.Columns.Append(col2, ADOX.KeyTypeEnum.adKeyPrimary, 500);
                    }
                    cat.Tables.Append(tbl);   //这句把表加入数据库(非常重要)  

                    tbl = null;
                    cat = null;
                    cn.Close();
                    return true;
                }
                catch (Exception t)
                {
                    MessageBox.Show(t.Message);
                    return false;
                }
            }

            /// <summary>
            /// 创建数据库
            /// </summary>
            public static void CreateOle(string tableName)
            {
                try
                {
                    if (!File.Exists(@mdbpath))
                    {
                        FileHelper.CreateDirectory(Directory.GetCurrentDirectory() + "\\dataFile");
                        if (CreateMDBDataBase() == true)
                        {
                            string[] arr2 = new string[] { "通道", "ID标定值", "参数a", "参数b", "开始字节", "末始字节", "是否干预" };
                            ArrayList list1 = new ArrayList(arr2);
                            CreateMDBTable(tableName, list1);
                        }

                    }
                    else
                    {
                    string[] arr2 = new string[] { "通道", "ID标定值", "参数a", "参数b", "开始字节", "末始字节", "是否干预" };
                    ArrayList list1 = new ArrayList(arr2);
                    CreateMDBTable(tableName, list1);

                }
            }
                catch (Exception msgerro)
                {
                    MessageBox.Show("创建数据库字段不成功");
                }

            }

            /// <summary>
            /// 插入测试信息
            /// </summary>
            public static void InsertDataToTable_data(string column,string[,] data)
            {
                String[] sqlArray = new String[8];
                string strInsert = "insert into "+ column + "(通道,ID标定值, 参数a, 参数b, 开始字节, 末始字节,是否干预) values";
                for (int k = 0; k < 8; k++)
                {
                    sqlArray[k] = strInsert + "('" + data[k,0] + "','" +
                                     data[k, 1] + "','" + data[k, 2] + "','" + data[k, 3] + "','" + data[k, 4] + "','" +
                                     data[k, 5] + "','" + data[k, 6]+ "')";
                }
                InsertTodataByBatch(sqlArray);
            }


            /// <summary>
            /// 批量插入数据,事务处理
            /// </summary>
            /// <param name="sqlArray"></param>
            private static void InsertTodataByBatch(String[] sqlArray)
            {
                try
                {
                    if (aConnection == null)
                        aConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbpath);

                    if (aConnection.State == ConnectionState.Closed)
                        aConnection.OpenAsync();

                    OleDbTransaction transaction = aConnection.BeginTransaction();


                    aCommand.Connection = aConnection;
                    aCommand.Transaction = transaction;
                    for (int i = 0; i < sqlArray.Length; i++)
                    {
                        aCommand.CommandText = sqlArray[i];
                        //AsyncExecuteNonQuery(sqlArray[i], CallbackAsyncExecuteNonQuery, aConnection);
                        aCommand.ExecuteNonQueryAsync();
                        //LogHelper.log(Convert.ToString(i));
                    }

                    transaction.Commit();
                    //  aConnection.Close();
                }
                catch (Exception e)
                {
                    //     LogHelper.log(e.Message);
                }
            }

            /// <summary>
            /// 数据库复位
            /// </summary>
            public static void StarResetNewAccess()
            {
                if (aConnection != null)
                {
                    if (aConnection.State == ConnectionState.Open)
                    {
                        aConnection.Close();
                        aConnection.Dispose();
                    }
                }

                if (CharViewDt != null)
                { CharViewDt.Dispose(); }
                CharViewDt = null;

                Compact(mdbpath);
                aConnection = null;
            }


            /// <summary>
            ///导入datagrid用
            /// </summary>
            /// <param name="SQL"></param>
            /// <returns></returns>
            public static DataTable SelectToDataTable_DataGrid(string SQL, string mdbpath_1)
            {
                if (mdbpath_1 == "")
                {
                    mdbpath_1 = mdbpath;
                }
                //  if(CharViewDt==null)
                CharViewDt = new DataTable("Char");
                //     if (aConnection==null)
                aConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbpath_1);

                OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, aConnection);
                //   OleDbCommand command = new OleDbCommand(SQL, aConnection);
                //  adapter.SelectCommand = command;
                adapter.Fill(CharViewDt);
                return CharViewDt;


                //  Fill
            }

            /// <summary>
            ///导入曲线用
            /// </summary>
            /// <param name="SQL"></param>
            /// <returns></returns>
            public static DataTable SelectToDataTable_CharView(string SQL, string mdbpath_1)
            {
                if (mdbpath_1 == "")
                {
                    mdbpath_1 = mdbpath;
                }
                //  if(CharViewDt==null)
                CharViewDt = new DataTable("Char");
                //   if (aConnection == null)
                aConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbpath_1);
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                OleDbCommand command = new OleDbCommand(SQL, aConnection);
                adapter.SelectCommand = command;
                adapter.Fill(CharViewDt);
                return CharViewDt;
            }


            /// <summary>
            /// 删除数据库
            /// </summary>
            public static void ThreadDeletetable2()
            {
                deletetable("数据流", "ID", "");
                //  thread1.Abort();
            }


            /// <summary>
            /// 删除表或表中字段、表中字段、表中字段where值
            /// </summary>
            /// <param name="table"></param>
            /// <param name="ziduan"></param>
            public static void deletetable(string table, string ziduan, string mdbpath_1)
            {

                try
                {

                    if (mdbpath_1 == "")
                    {
                        mdbpath_1 = mdbpath;
                    }

                    myDataSet = new DataSet();

                    aConnection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbpath_1);
                    string strDele = "delete from " + table + " where " + ziduan;
                    OleDbDataAdapter myDa = new OleDbDataAdapter();
                    myDa = new OleDbDataAdapter(strDele, aConnection);
                    myDa.Fill(myDataSet, table);
                    myDataSet.Clear();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }

            }


        //获取Access数据库表名
        public static string[] GetTableName()
        {
            string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbpath;
            OleDbConnection conn = new OleDbConnection(connString);

            conn.Open();
            DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

            conn.Close();
            int n = dt.Rows.Count;
            string[] tableName = new string[n];
            int m = dt.Columns.IndexOf("TABLE_NAME");
            for (int i = 0; i < n; i++)
            {
                DataRow dr = dt.Rows[i];
                tableName[i] = dr.ItemArray.GetValue(m).ToString();

               // this.cbxtable.Items.Add(tableName[i].ToString());
            }
            return tableName;
            //  this.cbxtable.SelectedIndex = 0;


        }

        ///压缩修复ACCESS数据库,mdbPath为数据库绝对路径      
        public static void Compact(object mdbPath_data)
            {
                try
                {



                    string mdbPath = (string)mdbPath_data;



                    System.IO.FileInfo f = new FileInfo(mdbPath);
                    //if (f.Length > 50000000)
                    //{
                    //Debug.WriteLine("数据库压缩前的文件大小：" + f.Length.ToString());

                    //if (con != null)
                    //{
                    //    con.Close();
                    //    con.Dispose();
                    //}


                    if (mdbPath == "")
                    {
                        // mdbPath = GetAccessFileName();
                    }

                    if (!File.Exists(mdbPath)) //检查数据库是否已存在  
                    {
                        throw new Exception("目标数据库不存在,无法压缩");
                    }

                    //  Thread.Sleep(1000);

                    //声明临时数据库的名称  
                    string temp = DateTime.Now.Year.ToString();
                    temp += DateTime.Now.Month.ToString();
                    temp += DateTime.Now.Day.ToString();
                    temp += DateTime.Now.Hour.ToString();
                    temp += DateTime.Now.Minute.ToString();
                    temp += DateTime.Now.Second.ToString() + ".bak";
                    temp = mdbPath.Substring(0, mdbPath.LastIndexOf("\\") + 1) + temp;
                    //定义临时数据库的连接字符串  
                    string temp2 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + temp;
                    //定义目标数据库的连接字符串  
                    string mdbPath2 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath + ";";
                    //创建一个JetEngineClass对象的实例  
                    JRO.JetEngineClass jt = new JRO.JetEngineClass();
                    //使用JetEngineClass对象的CompactDatabase方法压缩修复数据库  
                    jt.CompactDatabase(mdbPath2, temp2);
                    //拷贝临时数据库到目标数据库(覆盖)  
                    File.Copy(temp, mdbPath, true);
                    //最后删除临时数据库  
                    File.Delete(temp);
                    //}

                }
                catch (Exception er)
                {
                    // MessageBox.Show(er.Message);
                }
            }


        }
    }
