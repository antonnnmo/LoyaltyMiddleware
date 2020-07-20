using System;
using System.Data;

namespace RedmondLoyaltyMiddleware.DBProviders
{
	public static class ReaderExtension
	{
        public static T GetValue<T>(this IDataReader reader, string column, T defaultValue)
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
                catch (InvalidCastException)
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
