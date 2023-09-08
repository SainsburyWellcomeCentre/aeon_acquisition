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

        public static IObservable<TResult> Query<TResult>(string queryString, MySqlConnection connection)
        {
            return Query(
                queryString,
                connection,
                RecordReader<TResult>.Instance.Validate,
                RecordReader<TResult>.Instance.Select);
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

        public static bool? BooleanField(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetBoolean(ordinal);
        }

        public static char? CharField(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetChar(ordinal);
        }

        public static sbyte? SByteField(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetSByte(ordinal);
        }

        public static byte? ByteField(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetByte(ordinal);
        }

        public static short? Int16Field(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetInt16(ordinal);
        }

        public static ushort? UInt16Field(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetUInt16(ordinal);
        }

        public static int? Int32Field(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        public static uint? UInt32Field(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetUInt32(ordinal);
        }

        public static long? Int64Field(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetInt64(ordinal);
        }

        public static ulong? UInt64Field(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetUInt64(ordinal);
        }

        public static float? FloatField(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetFloat(ordinal);
        }

        public static double? DoubleField(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetDouble(ordinal);
        }

        public static decimal? DecimalField(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);
        }

        public static DateTime? DateTimeField(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
        }

        public static string StringField(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }
    }
}
