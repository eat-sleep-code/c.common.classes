using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;


public class DataSQL: IDisposable
{

	private string connectionString;
	private SqlConnection sqlConnection;
	private SqlDataAdapter sqlDataAdapter;
	private SqlCommand sqlCommand;
	private DriverType sqlDatabaseType;
	private HttpContext httpContext;
	public enum DriverType
	{
		SqlClient
	}

	public DataSQL(string connection, string databaseType)
	{
		httpContext = HttpContext.Current;
		httpContext.Trace.Write("MyDatabase", "New");
		DriverTypeFromString = databaseType;
		connectionString = connection;
		sqlConnection = CreateConnection(connectionString);

	}

	public DataSQL(string connection, DriverType databaseType)
	{
		httpContext = HttpContext.Current;
		httpContext.Trace.Write("MyDatabase", "New");
		this.DatabaseType = databaseType;
		connectionString = connection;
		sqlConnection = CreateConnection(connectionString);
	}

	private void OpenConn()
	{
		httpContext = HttpContext.Current;
		httpContext.Trace.Write("MyDatabase", "OpenConn");
		sqlConnection.Open();
	}

	private void CloseConn()
	{
		httpContext = HttpContext.Current;
		httpContext.Trace.Write("MyDatabase", "CloseConn");
		sqlConnection.Close();
	}



	#region "Setting The Database Driver Type"

		public DriverType DatabaseType
		{
			get
			{
				httpContext.Trace.Write("MyDatabase", "DatabaseType Get");
				return sqlDatabaseType;
			}
			set
			{
				httpContext.Trace.Write("MyDatabase", "DatabaseType Set");
				sqlDatabaseType = value;
			}
		}

		public string DriverTypeFromString
		{
			set
			{
				sqlDatabaseType = DriverType.SqlClient;
			}
		}

	#endregion



	#region "Create Objects - Data Abstraction Items"

		private SqlConnection CreateConnection(string connection)
		{
			httpContext.Trace.Write("MyDatabase", "CreateConnection");
			return new SqlConnection(connection);
		}

		private SqlDataAdapter CreateDataAdapter(SqlCommand command)
		{
			httpContext.Trace.Write("MyDatabase", "CreateDataAdapter");
			return new SqlDataAdapter(command);
		}

	#endregion



	#region "Database Methods"

		public void Execute(ref DataSet dataSet)
		{
			try
			{
				httpContext.Trace.Write("MyDatabase", "Execute");

				sqlDataAdapter = CreateDataAdapter(sqlCommand);
				sqlCommand.Connection = sqlConnection;

				httpContext.Trace.Write("Execute", sqlCommand.CommandText);

				OpenConn();
				sqlDataAdapter.Fill(dataSet);
			}
			catch (Exception ex)
			{
				httpContext.Trace.Write("Error", ex.ToString());
			}
			finally
			{
				CloseConn();
			}
		}

		public void Execute(string sql, ref DataSet dataSet)
		{

			CreateCommand(sql, CommandType.Text);
			Execute(ref dataSet);

		}

		public void Execute(ref DataTable dataTable)
		{

			DataSet dataSet = new DataSet();
			Execute(ref dataSet);
			dataTable = dataSet.Tables[0];
		}


		public void Execute(string sql, ref DataTable dataTable)
		{
			DataSet dataSet = new DataSet();
			CreateCommand(sql, CommandType.Text);
			Execute(ref dataTable);
			dataTable = dataSet.Tables[0];
		}

		public void Execute(ref DataRow dataRow)
		{
			DataSet dataSet = new DataSet();
			Execute(ref dataSet);
			try
			{
				dataRow = dataSet.Tables[0].Rows[0];
			}
			catch
			{
			}
		}


		public void Execute(string sql, ref DataRow dataRow)
		{
			CreateCommand(sql, CommandType.Text);
			Execute(ref dataRow);
		}

		public object ExecuteScalar()
		{
			object o = null;
			try
			{
				httpContext.Trace.Write("MyDatabase", "ExecuteScalar");
				sqlCommand.Connection = sqlConnection;
				OpenConn();
				httpContext.Trace.Write("MyDatabase", sqlCommand.CommandText);
				o = sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{
				httpContext.Trace.Write("Error", ex.ToString());
			}
			finally
			{
				CloseConn();
			}
			return o;
		}

		public object ExecuteScalar(string sql)
		{
			CreateCommand(sql, CommandType.Text);
			return ExecuteScalar();
		}

		public void ExecuteNonQuery()
		{
			try
			{
				httpContext.Trace.Write("MyDatabase", "ExecuteNonQuery");
				sqlCommand.Connection = sqlConnection;
				OpenConn();

				httpContext.Trace.Write("MyDatabase", sqlCommand.CommandText);
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				httpContext.Trace.Write("Error", ex.ToString());
			}
			finally
			{
				CloseConn();
			}
		}


		public void ExecuteNonQuery(string sql)
		{
			CreateCommand(sql, CommandType.Text);
			ExecuteNonQuery();
		}

	#endregion



	#region "Command Methods"

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		public void CreateCommand(string name, CommandType type)
		{
			httpContext.Trace.Write("MyDatabase", "CreateCommand");
			sqlCommand = new SqlCommand(name);
			sqlCommand.CommandType = type;
		}

		public void AddParameter(string name, string value)
		{
			httpContext.Trace.Write("MyDatabase", "AddParameter");
			SqlParameter parameter = null;
			parameter = new SqlParameter(name, value);
			sqlCommand.Parameters.Add(parameter);
		}

		public void AddParameter(string name, ParameterDirection direction, string type, int size)
		{
			httpContext.Trace.Write("MyDatabase", "AddParameter");
			SqlDbType databaseType = (SqlDbType)Enum.Parse(typeof(SqlDbType), type, true);
			SqlParameter parameter = null;
			parameter = new SqlParameter(name, databaseType, size, direction, false, 10, 0, "", DataRowVersion.Current, null);
			sqlCommand.Parameters.Add(parameter);
		}

		public void AddParameter(IDbDataParameter parameter)
		{
			httpContext.Trace.Write("MyDatabase", "AddParameter");
			sqlCommand.Parameters.Add(parameter);
		}

		public object ReadParameter(string parameterName)
		{
			httpContext.Trace.Write("MyDatabase", "ReadParam");
			return sqlCommand.Parameters[parameterName].Value;
		}

	#endregion



	#region "Transaction Methods"

		public void UseTransaction()
		{
			httpContext.Trace.Write("MyDatabase", "UseTransaction");
			SqlTransaction sqlTransaction = null;
			sqlCommand.Transaction = sqlTransaction;
		}

		public void CommitTransaction()
		{
			httpContext.Trace.Write("MyDatabase", "commitTransaction");
			sqlCommand.Transaction.Commit();
		}

		public void RollbackTransaction()
		{
			httpContext.Trace.Write("MyDatabase", "rollbackTransaction");
			if (sqlCommand.Transaction == null == false)
			{
				sqlCommand.Transaction.Rollback();
			}
		}

	#endregion



	#region "Dispose"

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~DataSQL()
		{
			Dispose(false);
		}

		protected virtual void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				sqlCommand.Dispose();
				sqlCommand = null;
			}
		}

	#endregion

}

