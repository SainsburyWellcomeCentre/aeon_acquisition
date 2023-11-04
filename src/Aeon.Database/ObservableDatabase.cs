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
    }
}
