using System;
using System.Reactive.Linq;
using MySqlConnector;

namespace Aeon.Database
{
    static class ObservableDatabase
    {
        public static IObservable<MySqlDataReader> Query(string queryString, MySqlConnection connection)
        {
            return Query(queryString, connection, reader => reader);
        }

        public static IObservable<TResult> Query<TResult>(
            string queryString,
            MySqlConnection connection,
            Func<MySqlDataReader, TResult> selector)
        {
            return Query(queryString, connection, reader => { }, selector);
        }

        public static IObservable<TResult> Query<TResult>(
            string queryString,
            MySqlConnection connection,
            Action<MySqlDataReader> validator,
            Func<MySqlDataReader, TResult> selector)
        {
            return Observable.Create<TResult>(async (observer, cancellationToken) =>
            {
                using var command = new MySqlCommand(queryString, connection);
                using var reader = await command.ExecuteReaderAsync(cancellationToken);

                validator(reader);
                while (await reader.ReadAsync(cancellationToken))
                {
                    var result = selector(reader);
                    observer.OnNext(result);
                }
            });
        }

        public static bool GetBooleanOrDefault(this MySqlDataReader reader, int ordinal, bool defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetBoolean(ordinal);
        }

        public static char GetCharOrDefault(this MySqlDataReader reader, int ordinal, char defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetChar(ordinal);
        }

        public static sbyte GetSByteOrDefault(this MySqlDataReader reader, int ordinal, sbyte defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetSByte(ordinal);
        }

        public static byte GetByteOrDefault(this MySqlDataReader reader, int ordinal, byte defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetByte(ordinal);
        }

        public static short GetInt16OrDefault(this MySqlDataReader reader, int ordinal, short defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt16(ordinal);
        }

        public static ushort GetUInt16OrDefault(this MySqlDataReader reader, int ordinal, ushort defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetUInt16(ordinal);
        }

        public static int GetInt32OrDefault(this MySqlDataReader reader, int ordinal, int defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
        }

        public static uint GetUInt32OrDefault(this MySqlDataReader reader, int ordinal, uint defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetUInt32(ordinal);
        }

        public static long GetInt64OrDefault(this MySqlDataReader reader, int ordinal, long defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt64(ordinal);
        }

        public static ulong GetUInt64OrDefault(this MySqlDataReader reader, int ordinal, ulong defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetUInt64(ordinal);
        }

        public static float GetFloatOrDefault(this MySqlDataReader reader, int ordinal, float defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetFloat(ordinal);
        }

        public static double GetDoubleOrDefault(this MySqlDataReader reader, int ordinal, double defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDouble(ordinal);
        }

        public static decimal GetDecimalOrDefault(this MySqlDataReader reader, int ordinal, decimal defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDecimal(ordinal);
        }

        public static DateTime GetDateTimeOrDefault(this MySqlDataReader reader, int ordinal, DateTime defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDateTime(ordinal);
        }

        public static string GetStringOrDefault(this MySqlDataReader reader, int ordinal, string defaultValue = default)
        {
            return reader.IsDBNull(ordinal) ? defaultValue : reader.GetString(ordinal);
        }
    }
}
