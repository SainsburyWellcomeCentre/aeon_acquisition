using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using MySqlConnector;
using YamlDotNet.Serialization.NamingConventions;

namespace Aeon.Database
{
    class RecordReader<T>
    {
        private static readonly PropertyInfo[] Members = typeof(T).GetProperties();
        public static readonly RecordReader<T> Instance = new();

        private RecordReader()
        {
            Validate = CreateSchemaValidator();
            Select = CreateSelector();
        }


        public Action<MySqlDataReader> Validate { get; }

        public Func<MySqlDataReader, T> Select { get; }

        private static Action<MySqlDataReader> CreateSchemaValidator()
        {
            return reader =>
            {
                if (!reader.CanGetColumnSchema())
                {
                    throw new ArgumentException($"Unable to get column schema from reader.");
                }

                var schema = reader.GetColumnSchema();
                if (schema.Count != Members.Length)
                {
                    throw new ArgumentException(
                        $"The number of attributes in the reader does not match the record type +" +
                        $"'{typeof(T).FullName}'.",
                        nameof(reader));
                }

                for (int i = 0; i < Members.Length; i++)
                {
                    var column = schema[i];
                    var memberName = PascalCaseNamingConvention.Instance.Apply(column.ColumnName);
                    if (memberName != Members[i].Name)
                    {
                        throw new ArgumentException(
                            $"The column {memberName} does not match the corresponding attribute " +
                            $"{Members[i].Name} in the record type '{typeof(T).FullName}'.",
                            nameof(reader));
                    }

                    if (column.DataType != Members[i].PropertyType)
                    {
                        if (column.DataTypeName == "ENUM")
                        {

                        }
                        else throw new ArgumentException(
                            $"The type of the column {memberName} does not match the attribute " +
                            $"{Members[i].PropertyType} {Members[i].Name} in the record type " +
                            $"'{typeof(T).FullName}'.",
                            nameof(reader));
                    }
                }
            };
        }

        private static IEnumerable<Expression> CreateRecord(ParameterExpression reader, ParameterExpression record)
        {
            yield return Expression.Assign(record, Expression.New(typeof(T)));
            for (int i = 0; i < Members.Length; i++)
            {
                Expression value;
                var ordinal = Expression.Constant(i);
                var member = Expression.PropertyOrField(record, Members[i].Name);
                if (member.Type.IsEnum)
                {
                    var enumType = Expression.Constant(member.Type);
                    var ignoreCase = Expression.Constant(true);
                    value = Expression.Call(reader, nameof(MySqlDataReader.GetString), null, ordinal);
                    value = Expression.Call(typeof(Enum), nameof(Enum.Parse), null, enumType, value, ignoreCase);
                    yield return Expression.Assign(member, Expression.Convert(value, member.Type));
                    continue;
                }

                var memberTypeCode = Type.GetTypeCode(member.Type);
                switch (memberTypeCode)
                {
                    case TypeCode.Boolean:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetBooleanOrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.Char:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetCharOrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.SByte:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetSByteOrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.Byte:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetByteOrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.Int16:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetInt16OrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.UInt16:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetUInt16OrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.Int32:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetInt32OrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.UInt32:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetUInt32OrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.Int64:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetInt64OrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.UInt64:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetUInt64OrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.Single:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetFloatOrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.Double:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetDoubleOrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.Decimal:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetDecimalOrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.DateTime:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetDateTimeOrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    case TypeCode.String:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.GetStringOrDefault),
                            null, reader, ordinal, Expression.Default(member.Type));
                        break;
                    default:
                        throw new NotSupportedException(
                            "The specified primitive record type is not supported.");
                }

                yield return Expression.Assign(member, value);
            }
            yield return record;
        }

        private static Func<MySqlDataReader, T> CreateSelector()
        {
            ParameterExpression reader;
            ParameterExpression record;
            reader = Expression.Parameter(typeof(MySqlDataReader), nameof(reader));
            record = Expression.Variable(typeof(T), nameof(record));

            var body = Expression.Block(
                typeof(T),
                new[] { record },
                CreateRecord(reader, record));
            var lambda = Expression.Lambda<Func<MySqlDataReader, T>>(body, reader);
            return lambda.Compile();
        }
    }
}
