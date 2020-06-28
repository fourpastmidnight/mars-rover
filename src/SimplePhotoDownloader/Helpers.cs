using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimplePhotoDownloader
{
    public static class Helpers
    {
        // Taken from 'Concurrency in .NET' by Ricardo Terrell, Manning, 2018, p. 304
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int maxDegreeOfParallelism, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(maxDegreeOfParallelism)
                select Task.Run((Func<Task?>)(async () =>
                {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                })));
        }

        public static bool HelpRequested(this List<string> args) => args.Count == 0 || args.GetArgumentIndex("--help") > -1;

        public static string GetApiKey(this List<string> args, string defaultValue = "")
        {
            if (!args.TryGetArgumentValueOrDefault("--api-key", out var apiKey, defaultValue))
            {
                throw new ArgumentException("ERROR: No value for --api-key was specified.");
            }

            return apiKey;
        }

        public static List<DateTime> GetDates(this List<string> args)
        {
            var hasDateArg = args.GetArgumentIndex("--date") > -1;
            var hasDatesArg = args.GetArgumentIndex("--dates") > -1;
            if (!(hasDateArg ^ hasDatesArg))
            {
                throw new ArgumentException("ERROR: One of --date or --dates must have a value specified, but not both.");
            }

            args.TryGetArgumentValue("--dates", out var dateListFile);

            var errorMessage = hasDateArg
                ? "ERROR: No value was provided for --date or the value is not a valid date."
                : $"ERROR: Unable to process the file '{dateListFile}'.\n" +
                  "       Check that the file exists, is accessible, and contains at least one\n" +
                  "       valid date.";

            var dates = (hasDateArg
                ? new [] {args.GetDateArgValueOrThrow()}
                : args.GetDatesFromFileOrThrow(dateListFile)
            ).ParseDates();

            if (dates.Count == 0)
            {
                throw new ArgumentException(errorMessage);
            }

            return dates;
        }

        /// <summary>
        /// If <paramref name="args"/> contains a <c>--date</c> argument, get
        /// the required value if present or throw an exception.
        /// </summary>
        /// <param name="args">The arguments to search</param>
        /// <returns>
        /// The string value for the <c>--date</c> value.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <c>--date</c> is specified but no value was provided.
        /// </exception>
        internal static string GetDateArgValueOrThrow(this List<string> args)
        {
            if (!args.TryGetArgumentValue("--date", out var theDate))
            {
                throw new ArgumentException("ERROR: No value was provided for --date or the value is not a valid date.");
            }

            return theDate;
        }

        private static IEnumerable<string> GetDatesFromFileOrThrow(this List<string> args, string dateListFile)
        {
            try
            {
                using var sr = new StreamReader(Path.GetFullPath(dateListFile));
                return sr.ReadAllLines();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Given a sequence of strings, parses dates from the given strings. Strings in the
        /// sequence which cannot be parsed as a date are simply ignored.
        /// </summary>
        /// <param name="dateStrings">A sequence of strings from which to parse dates</param>
        /// <returns>
        /// A <see cref="List{DateTime}"/> for those strings which could be parsed as a date. If
        /// no strings could be parsed as a <see cref="DateTime"/>, an empty list is returned.
        /// </returns>
        public static List<DateTime> ParseDates(this IEnumerable<string> dateStrings)
        {
            return dateStrings
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Aggregate(
                    new List<DateTime>(),
                    (ds, d) =>
                    {
                        if (DateTime.TryParse(d, out var date)) ds.Add(date);
                        return ds;
                    });
        }

        public static string GetOutputPath(this List<string> args, string defaultValue = "")
        {
            if (!args.TryGetArgumentValueOrDefault("--output", out var output, defaultValue))
            {
                throw new ArgumentException("ERROR: You must provide a value for --output.");
            }

            return output;
        }

        private static int GetArgumentIndex(this List<string> args, string argName) =>
            args.FindIndex(a => a.Equals(argName, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Attempts to get the specified argument's value.
        /// </summary>
        /// <param name="args">A list of arguments.</param>
        /// <param name="argName">An argument name</param>
        /// <param name="value">The parameter into which the argument value will be stored.</param>
        /// <returns>
        /// <c>true</c> if the argument was specified and a value was provided; otherwise <c>false</c>.
        /// </returns>
        public static bool TryGetArgumentValue(this List<string> args, string argName, out string value)
        {
            value = null;

            var index = args.GetArgumentIndex(argName);

            if (index == -1) return false;
            if (args.Count <= index + 1) return false;

            var argValue = args[index + 1];
            if (argValue.StartsWith("--")) return false;

            value = argValue;
            return true;
        }

        /// <summary>
        /// Attempts to get the specified argument's value, or returns a default value if the
        /// argument was not specified. This can be used for optional arguments which have a
        /// default value.
        /// </summary>
        /// <param name="args">a list of arguments.</param>
        /// <param name="argName">An argument name.</param>
        /// <param name="value">The parameter into which the argument value will be stored.</param>
        /// <param name="defaultValue">If the argument was not specified, the default value to return for the argument value.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="argName"/> was not specified and <paramref name="defaultValue"/> is returned in <paramref name="value"/> or
        /// if <paramref name="args"/> contains <paramref name="argName"/> and a value was provided for the argument; otherwise <c>false</c>.
        /// </returns>
        public static bool TryGetArgumentValueOrDefault(this List<string> args, string argName, out string value, string defaultValue = "")
        {
            value = null;

            var index = args.GetArgumentIndex(argName);

            if (index == -1)
            {
                value = defaultValue;
                return true;
            }

            if (args.Count <= index + 1) return false;

            var argValue = args[index + 1];
            if (argValue.StartsWith("--")) return false;

            value = argValue;
            return true;
        }

        public static IEnumerable<string> ReadAllLines(this StreamReader reader)
        {
            var lines = new List<string>();
            while (!reader.EndOfStream) lines.Add(reader.ReadLine());

            return lines;
        }
    }
}
