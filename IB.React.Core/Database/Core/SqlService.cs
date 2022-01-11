using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace IB.React.Core.Database.Core
{
    public class SqlService : IDisposable
    {
        #region - Member Variable Define & Property -

        /// <summary>
        /// 데이터베이스 연결자
        /// </summary>
        private DbConnection connection;

        /// <summary>
        /// 매개변수 집합
        /// </summary>
        private List<DbParameterWrapper> parameterWrapperCollection;

        /// <summary>
        /// 트랜잭션
        /// </summary>
        private DbTransaction transaction;

        /// <summary>
        /// 생성된 쿼리 구문을 위한 매개변수 치환 시 작은 따옴표를 표시 할 형식들입니다.
        /// </summary>
        private readonly DbType[] quotedParams = { 
            DbType.AnsiString, DbType.AnsiStringFixedLength, DbType.String, DbType.StringFixedLength,
            DbType.Date, DbType.DateTime, DbType.DateTime2, DbType.DateTimeOffset,
            DbType.Guid
        };

        /// <summary>
        /// 생성된 쿼리 구문을 위한 매개변수 치환 시 치환이 불가능한 형식들입니다.
        /// </summary>
        private readonly DbType[] notReplaceParams = {
            DbType.Binary, DbType.Object
        };

        /// <summary>
        /// 공급자 인스턴스 생성
        /// </summary>
        public DbProviderFactory ProviderFactory { get; private set; }

        /// <summary>
        /// 명령 실행 대기시간 (기본 30초)
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// 데이터베이스 연결 상태의 Open 여부
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if (connection != null)
                {
                    if (connection.State == System.Data.ConnectionState.Open) return true;
                    else return false;
                }
                else return false;
            }
        }

        /// <summary>
        /// 데이터베이스 연결 상태의 Closed 여부
        /// </summary>
        public bool IsClosed
        {
            get
            {
                if (connection != null)
                {
                    if (connection.State != System.Data.ConnectionState.Closed) return false;
                    else return true;
                }
                else return true;
            }
        }

        public Dictionary<string, object> OutputCollection { get; private set; }

        #endregion


        #region - Constructor -

        /// <summary>
        /// <para>DBHelper 클래스의 새 인스턴스를 초기화합니다.</para>
        /// <para>Configuration에 설정된 값을 사용합니다.</para>
        /// </summary>
        /// <param name="name">&lt;ConnectionStrings&gt;에 추가한 name 값을 사용합니다.</param>
        public SqlService(string name = "SqlServer")
        {
            OutputCollection = new Dictionary<string, object>();

            System.Configuration.ConnectionStringSettings setting = System.Configuration.ConfigurationManager.ConnectionStrings[name];

            InitConnection(setting.ConnectionString, setting.ProviderName);
        }

        /// <summary>
        /// DBHelper 클래스의 새 인스턴스를 초기화합니다.
        /// appsettings.json의 값을 참조합니다.
        /// </summary>
        /// <param name="configuration">settings</param>
        /// <param name="name">ConnectionStrings에 추가한 데이터의 키</param>
        public SqlService(IConfiguration configuration, string name = "SqlServer")
        {
            OutputCollection = new Dictionary<string, object>();

            var item = configuration.ConnectionStrings()[name];
            
            InitConnection(item.ConnectionString, item.ProviderName!);
        }

        /// <summary>
        /// DBHelper 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="connectionString">연결 문자열</param>
        /// <param name="providerName">공급자 고정 이름</param>
        public SqlService(string connectionString, string providerName)
        {
            OutputCollection = new Dictionary<string, object>();

            InitConnection(connectionString, providerName);
        }

        /// <summary>
        /// DBHelper 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="connectionString">연결 문자열</param>
        /// <param name="provider">공급자</param>
        public SqlService(string connectionString, Providers provider)
        {
            OutputCollection = new Dictionary<string, object>();

            InitConnection(connectionString, ConvertProviderToString(provider));
        }

        /// <summary>
        /// DBHelper 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="dataSource">연결 대상 서버</param>
        /// <param name="initialCatalog">연결 대상 데이터베이스</param>
        /// <param name="userID">연결 아이디</param>
        /// <param name="password">연결 아이디의 비밀번호</param>
        /// <param name="provider">선택인자 : 공급자 고정 이름</param>
        public SqlService(string dataSource, string initialCatalog, string userID, string password, string provider = "System.Data.SqlClient")
        {
            OutputCollection = new Dictionary<string, object>();

            DbConnectionStringBuilder connectionBuilder = new DbConnectionStringBuilder();
            connectionBuilder["Data Source"] = dataSource;
            connectionBuilder["Initial Catalog"] = initialCatalog;
            connectionBuilder["User ID"] = userID;
            connectionBuilder["Password"] = password;
            connectionBuilder["Enlist"] = false;

            InitConnection(connectionBuilder.ConnectionString, provider);
        }

        /// <summary>
        /// DBHelper 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="dataSource">연결 대상 서버</param>
        /// <param name="initialCatalog">연결 대상 데이터베이스</param>
        /// <param name="userID">연결 아이디</param>
        /// <param name="password">연결 아이디의 비밀번호</param>
        /// <param name="provider">공급자</param>
        public SqlService(string dataSource, string initialCatalog, string userID, string password, Providers provider)
        {
            OutputCollection = new Dictionary<string, object>();

            var connectionBuilder = new DbConnectionStringBuilder
            {
                ["Data Source"] = dataSource,
                ["Initial Catalog"] = initialCatalog,
                ["User ID"] = userID,
                ["Password"] = password,
                ["Enlist"] = false
            };

            InitConnection(connectionBuilder.ConnectionString, ConvertProviderToString(provider));
        }

        #endregion


        #region - Private Util -

        /// <summary>
        /// <para>대상 공급자와 연결 문자열로 연결 인스턴스를 초기화 합니다.</para>
        /// <para>명령 실행 대기시간은 기본 30초로 설정합니다. 이 설정은 CommandTimeout 속성을 통해 변경 가능합니다.</para>
        /// </summary>
        /// <param name="connectionString">연결 문자열</param>
        /// <param name="providerName">공급자 고정 이름</param>
        private void InitConnection(string connectionString, string providerName)
        {
            CommandTimeout = 30;

            parameterWrapperCollection = new List<DbParameterWrapper>();

            ProviderFactory = DbProviderFactories.GetFactory(providerName);
            connection = ProviderFactory.CreateConnection();
            connection.ConnectionString = connectionString;
        }

        /// <summary>
        /// 입력된 공급자의 고정 이름을 가져옵니다.
        /// </summary>
        /// <param name="provider">공급자</param>
        /// <returns>고정 이름</returns>
        private string ConvertProviderToString(Providers provider)
        {
            switch (provider)
            {
                case Providers.SqlClient:
                    return "System.Data.SqlClient";
                case Providers.OleDb:
                    return "System.Data.OleDb";
                case Providers.Odbc:
                    return "System.Data.Odbc";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// SQL Server의 데이터 형식에 해당되는 일반 DbType을 가져옵니다.
        /// </summary>
        /// <param name="sqlDbType">SQL Server의 데이터 형식</param>
        /// <returns>해당되는 일반 DbType</returns>
        private DbType GetDbType(SqlDbType sqlDbType)
        {
            switch (sqlDbType)
            {
                case SqlDbType.BigInt: return DbType.Int64;
                case SqlDbType.Binary: return DbType.Binary;
                case SqlDbType.Bit: return DbType.Boolean;
                case SqlDbType.Char: return DbType.AnsiStringFixedLength;
                case SqlDbType.DateTime: return DbType.DateTime;
                case SqlDbType.Decimal: return DbType.Decimal;
                case SqlDbType.Float: return DbType.Double;
                case SqlDbType.Image: return DbType.Binary;
                case SqlDbType.Int: return DbType.Int32;
                case SqlDbType.Money: return DbType.Currency;
                case SqlDbType.NChar: return DbType.StringFixedLength;
                case SqlDbType.NText: return DbType.String;
                case SqlDbType.NVarChar: return DbType.String;
                case SqlDbType.Real: return DbType.Single;
                case SqlDbType.UniqueIdentifier: return DbType.Guid;
                case SqlDbType.SmallDateTime: return DbType.DateTime;
                case SqlDbType.SmallInt: return DbType.Int16;
                case SqlDbType.SmallMoney: return DbType.Currency;
                case SqlDbType.Text: return DbType.AnsiString;
                case SqlDbType.Timestamp: return DbType.Binary;
                case SqlDbType.TinyInt: return DbType.Byte;
                case SqlDbType.VarBinary: return DbType.Binary;
                case SqlDbType.VarChar: return DbType.AnsiString;
                case SqlDbType.Variant: return DbType.Object;
                case SqlDbType.Xml: return DbType.Xml;
                case SqlDbType.Udt: return DbType.Object;
                case SqlDbType.Structured: return DbType.Object;
                case SqlDbType.Date: return DbType.Date;
                case SqlDbType.Time: return DbType.Time;
                case SqlDbType.DateTime2: return DbType.DateTime2;
                case SqlDbType.DateTimeOffset: return DbType.DateTimeOffset;
                default: throw new NotSupportedException("해당 형식은 현재 구현되지 않았습니다.");
            }
        }

        /// <summary>
        /// OLE DB의 데이터 형식에 해당되는 일반 DbType을 가져옵니다.
        /// </summary>
        /// <param name="oleDbType">OLE DB의 데이터 형식</param>
        /// <returns>해당되는 일반 DbType</returns>
        private DbType GetDbType(OleDbType oleDbType)
        {
            switch (oleDbType)
            {
                case OleDbType.Empty: return DbType.Object;
                case OleDbType.SmallInt: return DbType.Int16;
                case OleDbType.Integer: return DbType.Int32;
                case OleDbType.Single: return DbType.Single;
                case OleDbType.Double: return DbType.Double;
                case OleDbType.Currency: return DbType.Currency;
                case OleDbType.Date: return DbType.DateTime;
                case OleDbType.BSTR: return DbType.String;
                case OleDbType.IDispatch: return DbType.Object;
                case OleDbType.Error: return DbType.Int32;
                case OleDbType.Boolean: return DbType.Boolean;
                case OleDbType.Variant: return DbType.Object;
                case OleDbType.IUnknown: return DbType.Object;
                case OleDbType.Decimal: return DbType.Decimal;
                case OleDbType.TinyInt: return DbType.SByte;
                case OleDbType.UnsignedTinyInt: return DbType.Byte;
                case OleDbType.UnsignedSmallInt: return DbType.UInt16;
                case OleDbType.UnsignedInt: return DbType.UInt32;
                case OleDbType.BigInt: return DbType.Int64;
                case OleDbType.UnsignedBigInt: return DbType.UInt64;
                case OleDbType.Filetime: return DbType.DateTime;
                case OleDbType.Guid: return DbType.Guid;
                case OleDbType.Binary: return DbType.Binary;
                case OleDbType.Char: return DbType.AnsiStringFixedLength;
                case OleDbType.WChar: return DbType.StringFixedLength;
                case OleDbType.Numeric: return DbType.Decimal;
                case OleDbType.DBDate: return DbType.Date;
                case OleDbType.DBTime: return DbType.Time;
                case OleDbType.DBTimeStamp: return DbType.DateTime;
                case OleDbType.PropVariant: return DbType.Object;
                case OleDbType.VarNumeric: return DbType.VarNumeric;
                case OleDbType.VarChar: return DbType.AnsiString;
                case OleDbType.LongVarChar: return DbType.AnsiString;
                case OleDbType.VarWChar: return DbType.String;
                case OleDbType.LongVarWChar: return DbType.String;
                case OleDbType.VarBinary: return DbType.Binary;
                case OleDbType.LongVarBinary: return DbType.Binary;
                default: throw new NotSupportedException("해당 형식은 현재 구현되지 않았습니다.");
            }
        }

        /// <summary>
        /// ODBC의 데이터 형식에 해당되는 일반 DbType을 가져옵니다.
        /// </summary>
        /// <param name="odbcType">ODBC의 데이터 형식</param>
        /// <returns>해당되는 일반 DbType</returns>
        private DbType GetDbType(OdbcType odbcType)
        {
            switch (odbcType)
            {
                case OdbcType.BigInt: return DbType.Int64;
                case OdbcType.Binary: return DbType.Binary;
                case OdbcType.Bit: return DbType.Boolean;
                case OdbcType.Char: return DbType.AnsiStringFixedLength;
                case OdbcType.DateTime: return DbType.DateTime;
                case OdbcType.Decimal: return DbType.Decimal;
                case OdbcType.Numeric: return DbType.Decimal;
                case OdbcType.Double: return DbType.Double;
                case OdbcType.Image: return DbType.Binary;
                case OdbcType.Int: return DbType.Int32;
                case OdbcType.NChar: return DbType.StringFixedLength;
                case OdbcType.NText: return DbType.String;
                case OdbcType.NVarChar: return DbType.String;
                case OdbcType.Real: return DbType.Single;
                case OdbcType.UniqueIdentifier: return DbType.Guid;
                case OdbcType.SmallDateTime: return DbType.DateTime;
                case OdbcType.SmallInt: return DbType.Int16;
                case OdbcType.Text: return DbType.AnsiString;
                case OdbcType.Timestamp: return DbType.Binary;
                case OdbcType.TinyInt: return DbType.Byte;
                case OdbcType.VarBinary: return DbType.Binary;
                case OdbcType.VarChar: return DbType.AnsiString;
                case OdbcType.Date: return DbType.Date;
                case OdbcType.Time: return DbType.Time;
                default: throw new NotSupportedException("해당 형식은 현재 구현되지 않았습니다.");
            }
        }

        /// <summary>
        /// 현재 프로바이더로부터 DbCommand 인스턴스를 생성하고, 설정된 값(Timeout, Parameter, Transaction)을 설정합니다.
        /// </summary>
        /// <param name="commandText">SQL 구문 또는 프로시져명</param>
        /// <param name="commandType">명령의 속성</param>
        /// <returns>설정된 DbCommand</returns>
        private DbCommand GetDbCommand(string commandText, CommandType commandType = CommandType.Text)
        {
            DbCommand command = ProviderFactory.CreateCommand();

            command.Connection = connection;
            command.CommandTimeout = CommandTimeout;
            command.CommandType = commandType;

            if (transaction != null) command.Transaction = transaction;

            if (parameterWrapperCollection.Count > 0)
            {
                foreach (DbParameterWrapper item in parameterWrapperCollection)
                {
                    if (item.InClause)
                    {
                        string pattern = string.Format(@"\b{0}\b", Regex.Replace(Regex.Replace(item.ParameterName, @"^@", @"*@"), @"^:", @"*:"));

                        if (item.Value != null && item.Value != DBNull.Value)
                        {

                            var enumerable = item.Value as System.Collections.IEnumerable;  // value type이 존재하기 때문에 IEnumerable<object> 캐스팅이 안됨                            

                            if (enumerable != null)
                            {
                                int paramIndex = 0;
                                StringBuilder arrayParamString = new StringBuilder();
                                foreach (var enumItem in enumerable)
                                {
                                    arrayParamString.Append(string.Format("{0}{1},", item.ParameterName, ++paramIndex));
                                }

                                arrayParamString.Remove(arrayParamString.Length - 1, 1);     // 끝 "," 제거

                                commandText = Regex.Replace(commandText, pattern, arrayParamString.ToString());

                                paramIndex = 0;

                                foreach (var enumItem in enumerable)
                                {
                                    DbParameter arrayParam = command.CreateParameter();
                                    arrayParam.ParameterName = string.Format("{0}{1}", item.ParameterName, ++paramIndex);
                                    arrayParam.DbType = item.DbType;
                                    arrayParam.Value = enumItem;
                                    arrayParam.Direction = item.Direction;

                                    command.Parameters.Add(arrayParam);
                                }

                                continue;
                            }
                        }
                    }

                    command.Parameters.Add(item.GetParameter(command.CreateParameter()));
                }
            }

            command.CommandText = commandText;

            return command;
        }


        #endregion


        #region - Execute -

        /// <summary>
        /// <para>입력된 구문의 매개변수 명을 추가된 미리 설정된 값으로 치환하여 생성합니다.</para>
        /// <para>Binary와 Object 형식은 치환 하지 않습니다.</para>
        /// <para>*. 이 메서드는 디버깅 목적 이외에 사용되서는 안됩니다. (안정성 보장 못함)</para> 
        /// <para>*. ODBC등 매개변수 기호가 '?' 순서대로 대입되는 형태에서는 치환 대상 선택이 되지않아 동작 하지 않습니다.</para>
        /// </summary>
        /// <param name="commandText">SQL 구문</param>
        /// <returns>생성된 구문값</returns>
        public string DebuggingQuery(string commandText)
        {
            if (parameterWrapperCollection.Count > 0)
            {
                foreach (DbParameterWrapper item in parameterWrapperCollection)
                {
                    string pattern = string.Format(@"\b{0}\b", Regex.Replace(Regex.Replace(item.ParameterName, @"^@", @"*@"), @"^:", @"*:"));

                    if (item.Value == null || item.Value == DBNull.Value)
                        commandText = Regex.Replace(commandText, pattern, "null");
                    else
                    {
                        if (item.InClause)
                        {
                            var enumerable = item.Value as System.Collections.IEnumerable;  // value type이 존재하기 때문에 IEnumerable<object> 캐스팅이 안됨                            

                            if (enumerable != null)
                            {
                                StringBuilder arrayParamString = new StringBuilder();

                                foreach (var enumItem in enumerable)
                                {
                                    if (quotedParams.Contains(item.DbType))
                                        arrayParamString.Append("'" + enumItem + "', ");
                                    else if (!notReplaceParams.Contains(item.DbType))
                                        arrayParamString.Append(enumItem + ", ");
                                }

                                arrayParamString.Remove(arrayParamString.Length - 2, 2); // 끝 ", " 제거

                                commandText = Regex.Replace(commandText, pattern, arrayParamString.ToString());
                            }
                            else
                            {
                                // IEnumerable 변환이 안되는 값은 null로 표시
                                commandText = Regex.Replace(commandText, pattern, "null");
                            }
                        }
                        else
                        {
                            if (quotedParams.Contains(item.DbType))
                                commandText = Regex.Replace(commandText, pattern, "'" + item.Value + "'");
                            else if (!notReplaceParams.Contains(item.DbType))
                                commandText = Regex.Replace(commandText, pattern, item.Value.ToString());
                        }
                    }
                }
            }

            return commandText;
        }

        /// <summary>
        /// <para>SQL문이나 프로시저를 실행합니다.</para>
        /// <para>manualConnection 속성을 지정 시 Connection의 Open Close는 직접 실행 해야 합니다.</para>
        /// </summary>
        /// <param name="commandText">SQL 구문 또는 프로시져명</param>
        /// <param name="manualConnection">연결제어 true : 직접 Open, Close를 실행 / false(기본값) : 구문 실행 전 Open, 후 Close 자동 실행</param>
        /// <param name="commandType">명령 구문의 형식입니다.</param>
        /// <returns>영향 받는 행의 수</returns>
        public int ExecuteQuery(string commandText, bool manualConnection = false, CommandType commandType = CommandType.Text)
        {
            DbCommand command = GetDbCommand(commandText, commandType);

            try
            {
                if (!manualConnection) Open();

                int result = command.ExecuteNonQuery();

                if (OutputCollection.Count != 0)
                {
                    foreach (DbParameter item in command.Parameters)
                    {
                        if (OutputCollection.Keys.Where(f => f.Equals(item.ParameterName)).Count() == 1)
                            OutputCollection[item.ParameterName] = item.Value;
                    }
                }

                return result;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                command.Dispose();
                if (!manualConnection) Close();
            }
        }

        /// <summary>
        /// <para>SQL문이나 프로시저를 실행합니다.</para>
        /// <para>manualConnection 속성을 지정 시 Connection의 Open Close는 직접 실행 해야 합니다.</para>
        /// </summary>
        /// <param name="commandText">SQL 구문 또는 프로시져명</param>
        /// <param name="manualConnection">연결제어 true : 직접 Open, Close를 실행 / false(기본값) : 구문 실행 전 Open, 후 Close 자동 실행</param>
        /// <param name="commandType">명령 구문의 형식입니다.</param>
        /// <returns>반환 결과의 1행 1열의 값</returns>
        public object ExecuteScalar(string commandText, bool manualConnection = false, CommandType commandType = CommandType.Text)
        {
            DbCommand command = GetDbCommand(commandText, commandType);

            try
            {
                if (!manualConnection) Open();

                object result = command.ExecuteScalar();

                if (OutputCollection.Count != 0)
                {
                    foreach (DbParameter item in command.Parameters)
                    {
                        if (OutputCollection.Keys.Where(f => f.Equals(item.ParameterName)).Count() == 1)
                            OutputCollection[item.ParameterName] = item.Value;
                    }
                }

                return result;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                command.Dispose();
                if (!manualConnection) Close();
            }
        }

        /// <summary>
        /// <para>SQL문이나 프로시저를 실행합니다.</para>
        /// <para>manualConnection 속성을 지정 시 Connection의 Open Close는 직접 실행 해야 합니다.</para>
        /// </summary>
        /// <param name="commandText">SQL 구문 또는 프로시져명</param>
        /// <param name="tableName">반환되는 DataSet의 Table명</param>
        /// <param name="manualConnection">연결제어 true : 직접 Open, Close를 실행 / false(기본값) : 구문 실행 전 Open, 후 Close 자동 실행</param>        
        /// <param name="commandType">명령 구문의 형식입니다.</param>
        /// <returns>결과 DataSet</returns>
        public DataSet ExecuteQueryDataSet(string commandText, string tableName = "", bool manualConnection = false, CommandType commandType = CommandType.Text)
        {
            DbCommand command = GetDbCommand(commandText, commandType);

            DataSet ds = new DataSet();
            DbDataAdapter adapter = ProviderFactory.CreateDataAdapter();
            adapter.SelectCommand = command;

            try
            {
                if (!manualConnection) Open();

                if (string.IsNullOrEmpty(tableName))
                    adapter.Fill(ds);
                else
                    adapter.Fill(ds, tableName);

                if (OutputCollection.Count != 0)
                {
                    foreach (DbParameter item in command.Parameters)
                    {
                        if (OutputCollection.Keys.Where(f => f.Equals(item.ParameterName)).Count() == 1)
                            OutputCollection[item.ParameterName] = item.Value;
                    }
                }

                return ds;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (ds != null) ds.Dispose();
                command.Dispose();
                adapter.Dispose();
                if (!manualConnection) Close();
            }
        }

        /// <summary>
        /// <para>SQL문이나 프로시저를 실행합니다.</para>
        /// <para>manualConnection 속성을 지정 시 Connection의 Open Close는 직접 실행 해야 합니다.</para>
        /// </summary>
        /// <param name="commandText">SQL 구문 또는 프로시져명</param>
        /// <param name="dataKey">전체 Json 데이터의 Key 값</param>
        /// <param name="manualConnection">연결제어 true : 직접 Open, Close를 실행 / false(기본값) : 구문 실행 전 Open, 후 Close 자동 실행</param>        
        /// <param name="commandType">명령 구문의 형식입니다.</param>
        /// <returns>결과 Json</returns>
        public string ExecuteJson(string commandText, string dataKey = "", bool manualConnection = false, CommandType commandType = CommandType.Text)
        {
            DbCommand command = GetDbCommand(commandText, commandType);

            DbDataReader reader = null;
            try
            {
                if (!manualConnection) Open();

                reader = command.ExecuteReader();

                DataTable schemaTable = reader.GetSchemaTable();

                StringBuilder json = new StringBuilder();

                if (!reader.HasRows)
                {
                    if (string.IsNullOrEmpty(dataKey)) return "[]";
                    else return string.Format("{{\"{0}\":[]}}", dataKey);
                }

                if (string.IsNullOrEmpty(dataKey))
                    json.Append("[");
                else
                {
                    json.Append(string.Format("{{\"{0}\":[", dataKey));
                }

                while (reader.Read())
                {
                    json.Append("{");
                    int fieldcount = reader.FieldCount;
                    object[] values = new object[fieldcount];
                    reader.GetValues(values);


                    for (int i = 0; i < fieldcount; i++)
                    {
                        if (values[i] != null)
                        {
                            switch (Type.GetTypeCode(values[i].GetType()))
                            {
                                case TypeCode.Boolean:
                                    json.Append(string.Format("\"{0}\":{1}", reader.GetName(i), (bool)values[i] ? "true" : "false"));
                                    break;
                                case TypeCode.Byte:
                                case TypeCode.Decimal:
                                case TypeCode.Double:
                                case TypeCode.Int16:
                                case TypeCode.Int32:
                                case TypeCode.Int64:
                                case TypeCode.SByte:
                                case TypeCode.Single:
                                case TypeCode.UInt16:
                                case TypeCode.UInt32:
                                case TypeCode.UInt64:
                                    json.Append(string.Format("\"{0}\":{1}", reader.GetName(i), values[i]));
                                    break;
                                case TypeCode.DateTime:
                                    json.Append(string.Format("\"{0}\":\"{1}\"", reader.GetName(i), ((DateTime)values[i]).ToString("yyyy-MM-dd HH:mm:ss.fff")));
                                    break;
                                case TypeCode.Object:
                                case TypeCode.Char:
                                case TypeCode.String:
                                    json.Append(string.Format("\"{0}\":\"{1}\"", reader.GetName(i), values[i]));
                                    break;
                                case TypeCode.Empty:
                                case TypeCode.DBNull:
                                    json.Append(string.Format("\"{0}\":null", reader.GetName(i)));
                                    break;
                                default:
                                    json.Append(string.Format("\"{0}\":\"{1}\"", reader.GetName(i), values[i]));
                                    break;
                            }
                        }
                        else
                        {
                            json.Append(string.Format("\"{0}\":null", reader.GetName(i)));
                        }

                        if (i + 1 != fieldcount)
                            json.Append(",");

                    }

                    json.Append("},");
                }

                json.Remove(json.Length - 1, 1);

                if (string.IsNullOrEmpty(dataKey))
                    json.Append("]");
                else
                    json.Append("]}");

                if (OutputCollection.Count != 0)
                {
                    foreach (DbParameter item in command.Parameters)
                    {
                        if (OutputCollection.Keys.Where(f => f.Equals(item.ParameterName)).Count() == 1)
                            OutputCollection[item.ParameterName] = item.Value;
                    }
                }

                return json.ToString();

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                command.Dispose();
                if (reader != null) reader.Dispose();
                if (!manualConnection) Close();
            }
        }

        #endregion


        #region - Parameter -

        /// <summary>
        /// <para>SQL문이나 프로시저에 명령에 대한 매개변수를 설정합니다.</para>
        /// </summary>
        /// <param name="name">매개변수명</param>
        /// <param name="dataType">매개변수 형식</param>
        /// <param name="value">값</param>
        /// <param name="inClause">매개변수가 IN 구문에 사용되는지 여부</param>
        /// <param name="checkNullValue">
        ///     <para>true : value인자의 null값은 오류로 발생됩니다.</para>
        ///     <para>false : value에 null값이 들어오면 자동으로 인자값에 DBNull값을 대입합니다.</para>
        /// </param>
        /// <param name="direction">입력/출력 매개변수</param>
        /// <param name="size">매개변수 크기</param>
        public void AddParameter(string name, DbType dataType, object value, bool inClause = false, bool checkNullValue = false, ParameterDirection direction = ParameterDirection.Input, int size = -1)
        {
            DbParameterWrapper param = new DbParameterWrapper();
            param.ParameterName = name;
            param.DbType = dataType;
            param.Value = !checkNullValue && value == null ? DBNull.Value : value;
            param.Direction = direction;
            param.InClause = inClause;
            param.Size = size;

            parameterWrapperCollection.Add(param);

            if (direction != ParameterDirection.Input)
                OutputCollection.Add(name, null);
        }

        /// <summary>
        /// <para>SQL문이나 프로시저에 명령에 대한 매개변수를 설정합니다.</para>
        /// </summary>
        /// <param name="name">매개변수명</param>
        /// <param name="dataType">매개변수 형식</param>
        /// <param name="value">값</param>
        /// <param name="inClause">매개변수가 IN 구문에 사용되는지 여부</param>
        /// <param name="checkNullValue">
        ///     <para>true : value인자의 null값은 오류로 발생됩니다.</para>
        ///     <para>false : value에 null값이 들어오면 자동으로 인자값에 DBNull값을 대입합니다.</para>
        /// </param>
        /// <param name="direction">입력/출력 매개변수</param>
        /// <param name="size">매개변수 크기</param>
        public void AddParameter(string name, SqlDbType dataType, object value, bool inClause = false, bool checkNullValue = false, ParameterDirection direction = ParameterDirection.Input, int size = -1)
        {
            AddParameter(name, GetDbType(dataType), value, inClause, checkNullValue, direction, size);
        }

        /// <summary>
        /// <para>SQL문이나 프로시저에 명령에 대한 매개변수를 설정합니다.</para>
        /// </summary>
        /// <param name="name">매개변수명</param>
        /// <param name="dataType">매개변수 형식</param>
        /// <param name="value">값</param>
        /// <param name="inClause">매개변수가 IN 구문에 사용되는지 여부</param>
        /// <param name="checkNullValue">
        ///     <para>true : value인자의 null값은 오류로 발생됩니다.</para>
        ///     <para>false : value에 null값이 들어오면 자동으로 인자값에 DBNull값을 대입합니다.</para>
        /// </param>
        /// <param name="direction">입력/출력 매개변수</param>
        /// <param name="size">매개변수 크기</param>
        public void AddParameter(string name, OleDbType dataType, object value, bool inClause = false, bool checkNullValue = false, ParameterDirection direction = ParameterDirection.Input, int size = -1)
        {
            AddParameter(name, GetDbType(dataType), value, inClause, checkNullValue, direction);
        }

        /// <summary>
        /// <para>SQL문이나 프로시저에 명령에 대한 매개변수를 설정합니다.</para>
        /// </summary>
        /// <param name="name">매개변수명</param>
        /// <param name="dataType">매개변수 형식</param>
        /// <param name="value">값</param>
        /// <param name="inClause">매개변수가 IN 구문에 사용되는지 여부</param>
        /// <param name="checkNullValue">
        ///     <para>true : value인자의 null값은 오류로 발생됩니다.</para>
        ///     <para>false : value에 null값이 들어오면 자동으로 인자값에 DBNull값을 대입합니다.</para>
        /// </param>
        /// <param name="direction">입력/출력 매개변수</param>
        /// <param name="size">매개변수 크기</param>
        public void AddParameter(string name, OdbcType dataType, object value, bool inClause = false, bool checkNullValue = false, ParameterDirection direction = ParameterDirection.Input, int size = -1)
        {
            AddParameter(name, GetDbType(dataType), value, inClause, checkNullValue, direction);
        }

        #endregion


        #region - Helper controller -

        /// <summary>
        /// <para>데이터베이스 연결을 엽니다.</para>
        /// <para>이미 연결되어있으면 무시합니다.</para>        
        /// </summary>
        public void Open()
        {
            if (!IsOpen) connection.Open();
        }

        /// <summary>
        /// <para>데이터베이스 연결을 닫습니다.</para>
        /// <para>이미 닫혀있으면 무시합니다.</para>        
        /// </summary>
        public void Close()
        {
            parameterWrapperCollection.Clear();

            if (!IsClosed) connection.Dispose();
        }

        /// <summary>
        /// 설정된 매개변수 목록을 초기화 합니다.
        /// </summary>
        public void ClearParameter()
        {
            parameterWrapperCollection.Clear();
            OutputCollection.Clear();
        }

        /// <summary>
        /// 트랜잭션을 시작합니다.
        /// </summary>
        public void BeginTransaction()
        {
            transaction = connection.BeginTransaction();
        }

        /// <summary>
        /// 현재의 트랜잭션을 커밋합니다.
        /// </summary>
        public void Commit()
        {
            if (transaction != null)
                transaction.Commit();
            else
                throw new InvalidOperationException("Transaction값이 널(null)입니다. 이 문제는 대부분 BeginTransaction이 실행 되지 않은 상태에서 Commit을 호출 하였을때 발생합니다.");
        }

        /// <summary>
        /// 현재의 트랜잭션을 롤백합니다.
        /// </summary>
        public void Rollback()
        {
            if (transaction != null)
                transaction.Rollback();
            else
                throw new InvalidOperationException("Transaction값이 널(null)입니다. 이 문제는 대부분 BeginTransaction이 실행 되지 않은 상태에서 Rollback을 호출 하였을때 발생합니다.");
        }

        /// <summary>
        /// DBHelper에서 사용한 모든 자원을 해제합니다.
        /// </summary>
        public void Dispose()
        {
            connection.Dispose();
            OutputCollection.Clear();
            if (transaction != null) transaction.Dispose();
        }

        #endregion
    }
}
