using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace Chat.System.Core
{
    /// <summary>
    /// 用于封装SqlDBConnection的分页结果集
    /// </summary>
    public class PageData
    {
        /// <summary>
        /// 数据总量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 当前页索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 每页数据量
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 分页量
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public DataTable Data { get; set; }
    }

    public class MySqlDbConnection : IDisposable
    {
        public enum OrderBy
        {
            /// <summary>
            /// 降序
            /// </summary>
            DESC,
            /// <summary>
            /// 升序
            /// </summary>
            ASC
        }

        private MySqlConnection con;
        private MySqlCommand com;

        /// <summary>
        /// 链接到默认数据库
        /// </summary>
        /// <returns></returns>
        public static MySqlDbConnection Connect()
        {
            ConnectionStringSettings conSet = ConfigurationManager.ConnectionStrings["HidistroSqlServer"];

            if (conSet != null)
            {
                return new MySqlDbConnection(conSet);
            }

            throw new Exception("找不到数据库链接配置项HidistroSqlServer，无法链接数据库，请检查Web.config");
        }

        private MySqlDbConnection(ConnectionStringSettings conSet)
        {
            con = new MySqlConnection(conSet.ConnectionString);
            con.Open();
            com = new MySqlCommand()
            {
                Connection = con
            };

        }

        /// <summary>
        /// 添加查询参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddParameters(string name, object value)
        {
            com.Parameters.Add(new MySqlParameter(name, value));
        }

        /// <summary>
        /// 清除参数
        /// </summary>
        public void ClearParameters()
        {
            com.Parameters.Clear();
        }

        /// <summary>
        /// 获取或设置查询语句
        /// </summary>
        public string CommandText
        {
            get;
            set;
        }

        /// <summary>
        /// 执行无返回结果集的语句,将返回语句所影响的数据行数
        /// </summary>
        /// <returns></returns>
        public int Exec()
        {
            com.CommandText = CommandText;
            int result = com.ExecuteNonQuery();
            com.Parameters.Clear();
            return result;

        }

        /// <summary>
        /// 执行返回结果集的语句
        /// </summary>
        /// <returns></returns>
        public DataSet Query()
        {
            com.CommandText = this.CommandText;

            MySqlDataAdapter sda = new MySqlDataAdapter(com);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            com.Parameters.Clear();
            return ds;
        }

        /// <summary>
        /// 返回第一行第一列
        /// </summary>
        /// <returns></returns>
        public Object ExecuteScalar()
        {
            com.CommandText = CommandText;

            var ret = com.ExecuteScalar();
            com.Parameters.Clear();
            return ret;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="index">页索引 第一页为1</param>
        /// <param name="size">每页数据量</param>
        /// <param name="orderByColumn">排序字段名</param>
        /// <param name="orderby">排序规则</param>
        /// <returns></returns>
        public PageData Pageing(int index, int size, string orderByColumn, OrderBy orderby)
        {
            string sql = string.Format(@"select count(1) as c from ({0}) as o;
    select * from ({0}) as t order by {1} {2} limit {3}, {4}
", CommandText, orderByColumn, orderby == OrderBy.DESC ? "desc" : "asc", (index - 1) * size, index * size);

            com.CommandText = sql;

            MySqlDataAdapter msa = new MySqlDataAdapter(com);
            DataSet ds = new DataSet();
            msa.Fill(ds);
            com.Parameters.Clear();

            int dataCount = (int)ds.Tables[0].Rows[0]["c"];
            int pageCount = (dataCount / size) + (dataCount % size > 0 ? 1 : 0);

            return new PageData
            {
                PageCount = pageCount,
                Index = index,
                Size = size,
                Count = dataCount,
                Data = ds.Tables[1]
            };
        }

        public void Dispose()
        {

        }
    }
}
