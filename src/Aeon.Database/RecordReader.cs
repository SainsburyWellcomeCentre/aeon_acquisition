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

                    var propertyType = Members[i].PropertyType;
                    propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                    if (column.DataType != propertyType)
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
                    value = Expression.Call(reader, nameof(MySqlDataReader.GetChar), null, ordinal);
                    yield return Expression.Assign(member, Expression.Convert(value, member.Type));
                    continue;
                }

                var nullableType = Nullable.GetUnderlyingType(member.Type);
                var memberTypeCode = Type.GetTypeCode(nullableType ?? member.Type);
                var isNullable = nullableType != null;
                switch (memberTypeCode)
                {
                    case TypeCode.Boolean:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.BooleanField)
                                : nameof(MySqlDataReader.GetBoolean),
                            null, reader, ordinal);
                        break;
                    case TypeCode.Char:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.CharField)
                                : nameof(MySqlDataReader.GetChar),
                            null, reader, ordinal);
                        break;
                    case TypeCode.SByte:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.SByteField)
                                : nameof(MySqlDataReader.GetSByte),
                            null, reader, ordinal);
                        break;
                    case TypeCode.Byte:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.ByteField)
                                : nameof(MySqlDataReader.GetByte),
                            null, reader, ordinal);
                        break;
                    case TypeCode.Int16:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.Int16Field)
                                : nameof(MySqlDataReader.GetInt16),
                            null, reader, ordinal);
                        break;
                    case TypeCode.UInt16:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.UInt16Field)
                                : nameof(MySqlDataReader.GetUInt16),
                            null, reader, ordinal);
                        break;
                    case TypeCode.Int32:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.Int32Field)
                                : nameof(MySqlDataReader.GetInt32),
                            null, reader, ordinal);
                        break;
                    case TypeCode.UInt32:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.UInt32Field)
                                : nameof(MySqlDataReader.GetUInt32),
                            null, reader, ordinal);
                        break;
                    case TypeCode.Int64:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.Int64Field)
                                : nameof(MySqlDataReader.GetInt64),
                            null, reader, ordinal);
                        break;
                    case TypeCode.UInt64:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.UInt64Field)
                                : nameof(MySqlDataReader.GetUInt64),
                            null, reader, ordinal);
                        break;
                    case TypeCode.Single:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.FloatField)
                                : nameof(MySqlDataReader.GetFloat),
                            null, reader, ordinal);
                        break;
                    case TypeCode.Double:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.DoubleField)
                                : nameof(MySqlDataReader.GetDouble),
                            null, reader, ordinal);
                        break;
                    case TypeCode.Decimal:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.DecimalField)
                                : nameof(MySqlDataReader.GetDecimal),
                            null, reader, ordinal);
                        break;
                    case TypeCode.DateTime:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            isNullable
                                ? nameof(ObservableDatabase.DateTimeField)
                                : nameof(MySqlDataReader.GetDateTime),
                            null, reader, ordinal);
                        break;
                    case TypeCode.String:
                        value = Expression.Call(
                            typeof(ObservableDatabase),
                            nameof(ObservableDatabase.StringField),
                            null, reader, ordinal);
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
