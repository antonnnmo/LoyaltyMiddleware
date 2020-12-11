using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RedmondIntegrationService
{
	public static class ReaderExtension
	{
		public static string GetStringValue<T>(this SqlDataReader reader, string column, T defaultValue)
		{
			return reader.GetValue<T>(column, defaultValue).ToString().ToUpper();
		}

		public static T GetValue<T>(this SqlDataReader reader, string column, T defaultValue)
		{
			var obj = reader[column];

			if (obj is System.DBNull || obj == null)
			{
				return defaultValue;
			}
			else
			{
				try
				{
					return (T)obj;
				}
				catch (InvalidCastException e)
				{
					throw new InvalidCastException("Invalid type for column " + column);
				}
				catch (Exception e)
				{
					throw new Exception("Error in column reading: " + column, e);
				}
			}
		}
	}
}
