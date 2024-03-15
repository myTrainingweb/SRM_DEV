using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data.Common;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using IP.Classess;

public sealed class cDAL
{
    public enum QueryType
    {
        Insert,
        Update,
        Delete
    }

    public enum ConnectionType
    {
       
        INIT,
        ACTIVE,
        PLUS_EXT,
        FIMER,
        REDW,
        TESTREDW,
        BIZTALK_PROD_INBOUND,
        Z001_OUTBOUND,
        Z004_INBOUND,
        Z004_OUTBOUND,
        B2B



    }

    private DbProviderFactory _dbFactory;
    private DbTransaction _transaction;

    private List<cQuery> _batchQueries;

    public int QueryIndex { get; set; }
    public string InternalErrMsg { get; set; }
    public bool HasErrors { get; set; }
    public string ErrMessage { get; set; }
    public DataSet resultSet;
    public static string QueryExecTime { get; set; }
    public static string RowsFetched { get; set; }
    cLog oLog;
    #region "Initialization Routines"

    private readonly ConnectionType _connectionType;

    //public cDAL(ConnectionType conType = ConnectionType.PIX)
    public cDAL(ConnectionType connectionType = ConnectionType.INIT)
    {
        _dbFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");
        _connectionType = connectionType;
        this.Initialize();
    }



    private DbConnection CreateConnection()
    {
        DbConnection newConnection;
        string connectionString = string.Empty;
        
        switch (_connectionType)
        {
            case ConnectionType.INIT:
                connectionString = HttpContext.Current.Session["CONN_INIT"].ToString();
                break;
            case ConnectionType.ACTIVE:
                connectionString = HttpContext.Current.Session["CONN_INIT"].ToString();
                break;
           
            default:
                break;

        }

        newConnection = _dbFactory.CreateConnection();
        newConnection.ConnectionString = BasicEncrypt.Instance.Decrypt(connectionString);

        try
        {
            newConnection.Open();
        }
        catch (Exception ex)
        {
            HasErrors = true;
            ErrMessage = ex.Message;
            oLog = new cLog();
            oLog.RecordError(ex.Message, ex.StackTrace, newConnection.ConnectionString);
        }
        return newConnection;
    }


    public void Initialize()
    {
        HasErrors = false;
        ErrMessage = string.Empty;

        _batchQueries = new List<cQuery>();
    }

    public DbParameter CreateParameter(string parameterName, object parameterValue, DbType parameterDataType = DbType.String)
    {
        DbParameter _parameter = _dbFactory.CreateParameter();

        _parameter.ParameterName = parameterName;
        _parameter.Value = parameterValue;
        _parameter.DbType = parameterDataType;

        return _parameter;
    }

    #endregion

    #region "Data Retrieval Routines"
    public DataTable GetData(string commandText, Dictionary<string, object> parameters)
    {
        DataTable _resultSet = new DataTable();

        DbDataAdapter _dataAdapter = _dbFactory.CreateDataAdapter();
        using (var _connection = CreateConnection())
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                _dataAdapter.SelectCommand = _dbFactory.CreateCommand();
                _dataAdapter.SelectCommand.Connection = _connection;
                _dataAdapter.SelectCommand.CommandText = commandText;

                // parameterized queries avoid sql injection
                foreach (var p in parameters)
                {
                    var newP = _dataAdapter.SelectCommand.CreateParameter();
                    newP.ParameterName = p.Key;
                    newP.Value = p.Value;
                    _dataAdapter.SelectCommand.Parameters.Add(newP);
                }

                _dataAdapter.Fill(_resultSet);
            }
            catch (DbException ex)
            {
                HasErrors = true;
                ErrMessage = ex.Message;

                if (_connection.State != ConnectionState.Open)
                {
                    oLog = new cLog();
                    oLog.RecordError(ex.Message, ex.StackTrace, _connection.ConnectionString);
                }
                else
                {
                    oLog = new cLog();
                    oLog.RecordError(ex.Message, ex.StackTrace, commandText);
                }
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
                _dataAdapter.Dispose();
            }
        }


        return _resultSet;
    }
    public DataTable GetData(string commandText)
    {
        DataTable _resultSet = new DataTable();
        DbDataAdapter _dataAdapter = _dbFactory.CreateDataAdapter();
        QueryExecTime = "00:00";
        RowsFetched = "0";
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        using (var _connection = CreateConnection())
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                _dataAdapter.SelectCommand = _dbFactory.CreateCommand();
                _dataAdapter.SelectCommand.Connection = _connection;
                _dataAdapter.SelectCommand.CommandText = commandText;

                _dataAdapter.Fill(_resultSet);
                stopWatch.Stop();
                TimeSpan elapsed = stopWatch.Elapsed;
                QueryExecTime = $"{(int)elapsed.Minutes:00}:{elapsed.Seconds:00}";
                RowsFetched = _resultSet.Rows.Count.ToString();
            }
            catch (DbException ex)
            {
                HasErrors = true;
                ErrMessage = ex.Message;

                if (_connection.State != ConnectionState.Open)
                {
                    oLog = new cLog();
                    oLog.RecordError(ex.Message, ex.StackTrace, _connection.ConnectionString);
                }
                else
                {
                    oLog = new cLog();
                    oLog.RecordError(ex.Message, ex.StackTrace, commandText);
                }
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
                _dataAdapter.Dispose();
            }
        }

        return _resultSet;
    }

    public object GetObject(string commandText)
    {
        DbCommand command = _dbFactory.CreateCommand();

        using (var _connection = CreateConnection())
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                command.CommandText = commandText;
                command.Connection = _connection;

                return command.ExecuteScalar();
            }
            catch (DbException ex)
            {
                HasErrors = true;
                ErrMessage = ex.Message;

                oLog = new cLog();
                oLog.RecordError(ex.Message, ex.StackTrace, commandText);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

            }
        }

        return null;
    }

    #endregion

    #region "Data Manipulation Routines"

    public void AddQuery(string commandText)
    {
        cQuery oQuery = new cQuery();
        oQuery.CommandText = commandText;

        _batchQueries.Add(oQuery);
    }

    public void AddQuery(QueryType type, string tableName, Dictionary<string, object> tableColumns = null, string whereClause = "", int EmpId = 0, string EmpFullName = "", bool logAudit = false)
    {
        string columnNames = string.Empty;
        string columnValues = string.Empty;
        string columnNvalues = string.Empty;

        List<DbParameter> parameters = new List<DbParameter>();
        cQuery oPara = new cQuery();

        if (type != QueryType.Delete)
        {
            foreach (var itm in tableColumns)
            {
                DbType dataType = DbType.AnsiString;

                if (tableColumns[itm.Key].GetType().Equals(typeof(Int32))) { dataType = DbType.Int32; }
                else if (tableColumns[itm.Key].GetType().Equals(typeof(decimal))) { dataType = DbType.Decimal; }
                else if (tableColumns[itm.Key].GetType().Equals(typeof(DateTime))) { dataType = DbType.DateTime; }
                else if (tableColumns[itm.Key].GetType().Equals(typeof(byte[]))) { dataType = DbType.Binary; }

                if (tableColumns[itm.Key].ToString() != "EMPTY")
                    parameters.Add(CreateParameter(itm.Key, itm.Value, dataType));
            }
        }

        switch (type)
        {
            case QueryType.Insert:
                string[] columnsIns = (from c in parameters select c.ParameterName).ToArray();
                string[] values = (from p in parameters select "@" + p.ParameterName).ToArray();
                columnNames = string.Join(", ", columnsIns);
                columnValues = string.Join(", ", values);
                oPara.CommandText = "INSERT INTO " + tableName + " (" + columnNames + ")" + " VALUES (" + columnValues + ")";
                oPara.Parameters = parameters.ToArray();
                break;

            case QueryType.Update:
                string[] columnsUpd = (from c in parameters select c.ParameterName + " = " + "@" + c.ParameterName).ToArray();
                columnNvalues = string.Join(", ", columnsUpd);
                oPara.CommandText = "UPDATE " + tableName + " SET " + columnNvalues + "" + " WHERE " + whereClause + "";
                oPara.Parameters = parameters.ToArray();
                break;

            case QueryType.Delete:
                oPara.CommandText = "DELETE FROM " + tableName + " WHERE " + whereClause + "";
                oPara.Parameters = null;
                break;

            default:
                break;
        }

        _batchQueries.Add(oPara);

        if (logAudit)
        {
            string auditAffect = string.Empty;
            switch (type)
            {
                case QueryType.Insert:
                    auditAffect = "ADDED";
                    break;
                case QueryType.Update:
                    auditAffect = "MODIFIED";
                    break;
                case QueryType.Delete:
                    auditAffect = "REMOVED";
                    break;
                default:
                    break;
            }

            if (tableColumns != null)
                AddAuditQuery(tableName, tableColumns, whereClause, EmpId, EmpFullName, auditAffect);
            else
                AddAuditQuery(tableName, whereClause, auditAffect, EmpId, EmpFullName);
        }
    }

    public void AddAuditQuery(string tableName, string whereClause, string auditAffect, int EmpId, string EmpFullName)
    {
        string sql = string.Empty;

        sql = "INSERT INTO " + tableName + "_ADT ";
        sql += "SELECT Getdate(), " + EmpId + ", '" + EmpFullName + "', '" + auditAffect + "', * ";
        sql += "FROM " + tableName + " ";
        sql += "WHERE " + whereClause;
       
        cQuery oPara = new cQuery();
        oPara.CommandText = sql;
        oPara.Parameters = null;

        _batchQueries.Add(oPara);
    }

    public void AddAuditQuery(string tableName, Dictionary<string, object> tableColumns, string whereClause,
                                        int auditEmpId, string auditName, string auditAffect)
    {
        string auditValues = string.Empty;
        string columnNames = string.Empty;

        List<DbParameter> parameters = new List<DbParameter>();

        parameters.Add(CreateParameter("ADT_DT", DateTime.Now.ToString()));
        parameters.Add(CreateParameter("ADT_EMP_ID", auditEmpId));
        parameters.Add(CreateParameter("ADT_NAME", auditName));
        parameters.Add(CreateParameter("ADT_AFFECT", auditAffect));


        foreach (var itm in tableColumns)
        {
            if (itm.Key != "RECNUM")
                parameters.Add(CreateParameter(itm.Key, itm.Value));
        }

        string[] auditColumns = (from c in parameters where c.ParameterName.StartsWith("ADT_") select "'" + c.Value.ToString() + "'").ToArray();
        string[] columns = (from c in parameters where !c.ParameterName.StartsWith("ADT_") select c.ParameterName).ToArray();

        auditValues = string.Join(", ", auditColumns);
        columnNames = string.Join(", ", columns);

        string auditQuery = "INSERT INTO " + tableName + "_ADT "
            + "(ADT_DT, ADT_EMP_ID, ADT_NAME, ADT_AFFECT, " + columnNames + ") "
            + "SELECT TOP(1) " + auditValues + ", " + columnNames + " FROM " + tableName + " "
            + "WHERE " + whereClause;

        cQuery oPara = new cQuery();
        oPara.CommandText = auditQuery;
        oPara.Parameters = null;

        _batchQueries.Add(oPara);
    }

    public void Execute(string commandText)
    {
        DbCommand command = _dbFactory.CreateCommand();

        using (var _connection = CreateConnection())
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                command.Connection = _connection;
                command.CommandText = commandText;

                command.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                HasErrors = true;
                InternalErrMsg = ex.Message;

                oLog = new cLog();
                oLog.RecordError(ex.Message, ex.StackTrace, commandText);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

            }
        }

    }

    public void Commit()
    {
        DbCommand command = _dbFactory.CreateCommand();

        using (var _connection = CreateConnection())
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                _transaction = _connection.BeginTransaction();
                command.Connection = _connection;
                command.Transaction = _transaction;

                if (_batchQueries.Count > 0)
                {

                    foreach (cQuery oParam in _batchQueries)
                    {
                        QueryIndex = _batchQueries.IndexOf(oParam);

                        command.CommandText = oParam.CommandText;
                        command.Parameters.Clear();
                        if ((oParam.Parameters != null))
                            command.Parameters.AddRange(oParam.Parameters);

                        command.ExecuteNonQuery();
                    }

                    _transaction.Commit();

                    this.Initialize();
                }
            }
            catch (DbException ex)
            {
                if (_transaction != null)
                    _transaction.Rollback();

                HasErrors = true;
                InternalErrMsg = ex.Message;
                oLog = new cLog();
                oLog.RecordError(ex.Message, ex.StackTrace, _batchQueries[QueryIndex].CommandText);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

            }
        }
    }

    public bool ExecuteProcedure(string procedureName, List<DbParameter> parameters)
    {
        DbCommand command = _dbFactory.CreateCommand();

        using (var _connection = CreateConnection())
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = _connection;

                if (parameters.Count > 0)
                    command.Parameters.AddRange(parameters.ToArray());

                DbDataReader reader = command.ExecuteReader();
                resultSet = new DataSet();
                resultSet.Load(reader, LoadOption.OverwriteChanges, "");
                return true;
            }
            catch (DbException ex)
            {
                HasErrors = true;
                ErrMessage = ex.Message;

                oLog = new cLog();
                oLog.RecordError(ex.Message, ex.StackTrace, command.CommandText);
            }
            finally
            {
                command.Parameters.Clear();
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

            }
        }
        return false;
    }

    public bool ExecuteProcedureWithDS(string procedureName, List<DbParameter> parameters)
    {
        DbCommand command = _dbFactory.CreateCommand();

        using (var _connection = CreateConnection())
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = _connection;

                if (parameters.Count > 0)
                    command.Parameters.AddRange(parameters.ToArray());

                DbDataAdapter adapter = _dbFactory.CreateDataAdapter();
                adapter.SelectCommand = command;
                resultSet = new DataSet();
                adapter.Fill(resultSet);

                return true;
            }
            catch (DbException ex)
            {
                HasErrors = true;
                ErrMessage = ex.Message;

                oLog = new cLog();
                oLog.RecordError(ex.Message, ex.StackTrace, command.CommandText);
            }
            finally
            {
                command.Parameters.Clear();
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

            }
        }

        return false;
    }

    public DataSet GetDataSet(string commandText)
    {
        DataSet _resultSet = new DataSet();
        DbDataAdapter _dataAdapter = _dbFactory.CreateDataAdapter();

        using (var _connection = CreateConnection())
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                _dataAdapter.SelectCommand = _dbFactory.CreateCommand();
                _dataAdapter.SelectCommand.Connection = _connection;
                _dataAdapter.SelectCommand.CommandText = commandText;
                //_dataAdapter.SelectCommand.CommandTimeout = 5;

                _dataAdapter.Fill(_resultSet);
            }
            catch (DbException ex)
            {
                HasErrors = true;
                ErrMessage = ex.Message;

                if (_connection.State != ConnectionState.Open)
                {
                    oLog = new cLog();
                    oLog.RecordError(ex.Message, ex.StackTrace, _connection.ConnectionString);
                }
                else
                {
                    oLog = new cLog();
                    oLog.RecordError(ex.Message, ex.StackTrace, commandText);
                }
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
                _dataAdapter.Dispose();
            }
        }

        return _resultSet;
    }

    #endregion
}

public class cQuery
{
    public string CommandText { get; set; }
    public DbParameter[] Parameters { get; set; }
}