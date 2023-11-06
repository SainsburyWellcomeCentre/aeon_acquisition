using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using MySqlConnector;
using YamlDotNet.Serialization.NamingConventions;

namespace Aeon.Database
{
    class RecordReader<T>
    {
        private static readonly ColumnAttribute DefaultColumnAttribute = new ColumnAttribute();
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
                for (int i = 0; i < Members.Length; i++)
                {
                    DbColumn column;
                    var columnAttribute = Members[i].GetCustomAttribute<ColumnAttribute>() ?? DefaultColumnAttribute;
                    if (columnAttribute.Order >= 0) column = schema[columnAttribute.Order];
                    else if (string.IsNullOrEmpty(columnAttribute.Name)) column = schema[i];
                    else column = schema[reader.GetOrdinal(columnAttribute.Name)];

                    if (!string.IsNullOrEmpty(columnAttribute.Name))
                    {
                        if (columnAttribute.Name != column.ColumnName)
                        {
                            throw new ArgumentException(
                                $"The column {column.ColumnName} does not match the corresponding attribute " +
                                $"{columnAttribute.Name} in the record type '{typeof(T).FullName}'.",
                                nameof(reader));
                        }
                    }
                    else
                    {
                        var memberName = PascalCaseNamingConvention.Instance.Apply(column.ColumnName);
                        if (memberName != Members[i].Name)
                        {
                            throw new ArgumentException(
                                $"The name of the column {column.ColumnName} could not be matched to the " +
                                $"corresponding attribute {Members[i].Name} in the record type" +
                                $"'{typeof(T).FullName}'.",
                                nameof(reader));
                        }
                    }

                    var propertyType = Members[i].PropertyType;
                    propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                    if (column.DataType != propertyType)
                    {
                        if (column.DataTypeName == "ENUM")
                        {

                        }
                        else throw new ArgumentException(
                            $"The column data type {column.DataTypeName} {column.ColumnName} does not match the attribute " +
                            $"{Members[i].PropertyType} {Members[i].Name} in the record type " +
                            $"'{typeof(T).FullName}'.",
                            nameof(reader));
                    }
                }
            };
        }

        private static Expression GetField(Expression reader, Expression indexer, string name, bool isNullable)
        {
            return isNullable
                ? Expression.Call(typeof(DataReaderExtensions), $"GetNullable{name}", null, reader, indexer)
                : Expression.Call(reader, $"Get{name}", null, indexer);
        }

        private static IEnumerable<Expression> CreateRecord(ParameterExpression reader, ParameterExpression record)
        {
            yield return Expression.Assign(record, Expression.New(typeof(T)));
            for (int i = 0; i < Members.Length; i++)
            {
                Expression value, indexer;
                var columnAttribute = Members[i].GetCustomAttribute<ColumnAttribute>() ?? DefaultColumnAttribute;
                if (columnAttribute.Order >= 0) indexer = Expression.Constant(columnAttribute.Order);
                else if (string.IsNullOrEmpty(columnAttribute.Name)) indexer = Expression.Constant(i);
                else indexer = Expression.Constant(columnAttribute.Name);

                var member = Expression.PropertyOrField(record, Members[i].Name);
                if (member.Type.IsEnum)
                {
                    var enumType = Expression.Constant(member.Type);
                    var ignoreCase = Expression.Constant(true);
                    value = Expression.Call(reader, nameof(MySqlDataReader.GetChar), null, indexer);
                    yield return Expression.Assign(member, Expression.Convert(value, member.Type));
                    continue;
                }

                var nullableType = Nullable.GetUnderlyingType(member.Type);
                var memberTypeCode = Type.GetTypeCode(nullableType ?? member.Type);
                var isNullable = nullableType != null;
                switch (memberTypeCode)
                {
                    case TypeCode.Boolean:
                        value = GetField(reader, indexer, nameof(Boolean), isNullable);
                        break;
                    case TypeCode.Char:
                        value = GetField(reader, indexer, nameof(Char), isNullable);
                        break;
                    case TypeCode.SByte:
                        value = GetField(reader, indexer, nameof(SByte), isNullable);
                        break;
                    case TypeCode.Byte:
                        value = GetField(reader, indexer, nameof(Byte), isNullable);
                        break;
                    case TypeCode.Int16:
                        value = GetField(reader, indexer, nameof(Int16), isNullable);
                        break;
                    case TypeCode.UInt16:
                        value = GetField(reader, indexer, nameof(UInt16), isNullable);
                        break;
                    case TypeCode.Int32:
                        value = GetField(reader, indexer, nameof(Int32), isNullable);
                        break;
                    case TypeCode.UInt32:
                        value = GetField(reader, indexer, nameof(UInt32), isNullable);
                        break;
                    case TypeCode.Int64:
                        value = GetField(reader, indexer, nameof(Int64), isNullable);
                        break;
                    case TypeCode.UInt64:
                        value = GetField(reader, indexer, nameof(UInt64), isNullable);
                        break;
                    case TypeCode.Single:
                        value = GetField(reader, indexer, "Float", isNullable);
                        break;
                    case TypeCode.Double:
                        value = GetField(reader, indexer, nameof(Double), isNullable);
                        break;
                    case TypeCode.Decimal:
                        value = GetField(reader, indexer, nameof(Decimal), isNullable);
                        break;
                    case TypeCode.DateTime:
                        value = GetField(reader, indexer, nameof(DateTime), isNullable);
                        break;
                    case TypeCode.String:
                        value = GetField(reader, indexer, nameof(String), isNullable: true);
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
