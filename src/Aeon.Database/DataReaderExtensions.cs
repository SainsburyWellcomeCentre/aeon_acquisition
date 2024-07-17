using System;
using System.Collections.Generic;
using MySqlConnector;

namespace Aeon.Database
{
    internal static class DataReaderExtensions
    {
        public static IEnumerable<TRecord> GetRecords<TRecord>(this MySqlDataReader reader)
        {
            return GetRecords(
                reader,
                RecordReader<TRecord>.Instance.Validate,
                RecordReader<TRecord>.Instance.Select);
        }

        public static IEnumerable<TRecord> GetRecords<TRecord>(
            this MySqlDataReader reader,
            Func<MySqlDataReader, TRecord> selector)
        {
            return GetRecords(reader, reader => { }, selector);
        }

        public static IEnumerable<TRecord> GetRecords<TRecord>(
            this MySqlDataReader reader,
            Action<MySqlDataReader> validator,
            Func<MySqlDataReader, TRecord> selector)
        {
            validator(reader);
            while (reader.Read())
            {
                yield return selector(reader);
            }
        }

        public static bool? GetNullableBoolean(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetBoolean(ordinal);
        }

        public static bool? GetNullableBoolean(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetBoolean(name);
        }

        public static char? GetNullableChar(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetChar(ordinal);
        }

        public static char? GetNullableChar(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetChar(name);
        }

        public static sbyte? GetNullableSByte(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetSByte(ordinal);
        }

        public static sbyte? GetNullableSByte(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetSByte(name);
        }

        public static byte? GetNullableByte(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetByte(ordinal);
        }

        public static byte? GetNullableByte(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetByte(name);
        }

        public static short? GetNullableInt16(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetInt16(ordinal);
        }

        public static short? GetNullableInt16(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetInt16(name);
        }

        public static ushort? GetNullableUInt16(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetUInt16(ordinal);
        }

        public static ushort? GetNullableUInt16(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetUInt16(name);
        }

        public static int? GetNullableInt32(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
        }

        public static int? GetNullableInt32(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetInt32(name);
        }

        public static uint? GetNullableUInt32(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetUInt32(ordinal);
        }

        public static uint? GetNullableUInt32(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetUInt32(name);
        }

        public static long? GetNullableInt64(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetInt64(ordinal);
        }

        public static long? GetNullableInt64(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetInt64(name);
        }

        public static ulong? GetNullableUInt64(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetUInt64(ordinal);
        }

        public static ulong? GetNullableUInt64(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetUInt64(name);
        }

        public static float? GetNullableFloat(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetFloat(ordinal);
        }

        public static float? GetNullableFloat(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetFloat(name);
        }

        public static double? GetNullableDouble(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetDouble(ordinal);
        }

        public static double? GetNullableDouble(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetDouble(name);
        }

        public static decimal? GetNullableDecimal(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);
        }

        public static decimal? GetNullableDecimal(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetDecimal(name);
        }

        public static DateTime? GetNullableDateTime(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
        }

        public static DateTime? GetNullableDateTime(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetDateTime(name);
        }

        public static string GetNullableString(this MySqlDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        public static string GetNullableString(this MySqlDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetString(name);
        }
    }
}
