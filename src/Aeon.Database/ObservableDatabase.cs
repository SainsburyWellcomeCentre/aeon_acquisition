using System;
using System.Reactive.Linq;
using MySqlConnector;

namespace Aeon.Database
{
    public static class ObservableDatabase
    {
        public static IObservable<MySqlDataReader> Query(string queryString, MySqlConnection connection)
        {
            return Query(queryString, connection, reader => reader);
        }

        public static IObservable<TRecord> Query<TRecord>(string queryString, MySqlConnection connection)
        {
            return Query(
                queryString,
                connection,
                RecordReader<TRecord>.Instance.Validate,
                RecordReader<TRecord>.Instance.Select);
        }

        public static IObservable<TRecord> Query<TRecord>(
            string queryString,
            MySqlConnection connection,
            Func<MySqlDataReader, TRecord> selector)
        {
            return Query(queryString, connection, reader => { }, selector);
        }

        public static IObservable<TRecord> Query<TRecord>(
            string queryString,
            MySqlConnection connection,
            Action<MySqlDataReader> validator,
            Func<MySqlDataReader, TRecord> selector)
        {
            return Observable.Create<TRecord>(async (observer, cancellationToken) =>
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
    }
}
