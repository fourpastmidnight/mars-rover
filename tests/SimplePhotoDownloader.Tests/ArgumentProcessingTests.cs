using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
// ReSharper disable InvokeAsExtensionMethod

namespace SimplePhotoDownloader.Tests
{
    public sealed class ArgumentProcessingTests
    {
        [Theory]
        [InlineData("--api-key", "--api-key")]
        [InlineData("--api-key --date 2017-02-27", "--api-key")]
        [InlineData(@"--output .\path\to\store\photos --api-key --date 2017-02-27", "--api-key")]
        public void TryGetArgumentValue_Return_False_OnBadArgumentSpecification(string argString, string argName)
        {
            List<string> args = ConvertToArgList(argString);

            var result = Helpers.TryGetArgumentValue(args, argName, out _);

            result.Should().BeFalse("--api-key is not followed by a value and/or is followed by another argument");
        }

        [Theory]
        [InlineData("--api-key the-api-key", "--api-key")]
        [InlineData("--api-key the-api-key --date 2017-02-27", "--api-key")]
        [InlineData(@"--output = .\path\to\store\photos --api-key the-api-key --date 2017-02-27", "--api-key")]
        public void TryGetArgumentValue_Returns_True_OnGoodArgumentSpecification(string argString, string argName)
        {
            List<string> args = ConvertToArgList(argString);

            var result = Helpers.TryGetArgumentValue(args, argName, out var actual);

            result.Should().BeTrue();
            actual.Should().Be("the-api-key");
        }

        [Theory]
        [InlineData("--api-key", "--api-key")]
        [InlineData("--api-key --date 2017-02-27", "--api-key")]
        [InlineData(@"--output .\path\to\store\photos --api-key --date 2017-02-27", "--api-key")]
        public void TryGetArgumentValueOrDefault_Returns_False_OnBadArgumentSpecification(string argString, string argName)
        {
            List<string> args = ConvertToArgList(argString);

            var result = Helpers.TryGetArgumentValueOrDefault(args, argName, out _, "the-default-value");

            result.Should().BeFalse("--api-key is not followed by a value and/or is followed by another argument");
        }

        [Theory]
        [InlineData("--api-key the-api-key", "--api-key")]
        [InlineData("--api-key the-api-key --date 2017-02-27", "--api-key")]
        [InlineData(@"--output .\path\to\store\photos --api-key the-api-key --date 2017-02-27", "--api-key")]
        public void TryGetArgumentValueOrDefault_Returns_True_OnGoodArgumentSpecification(string argString, string argName)
        {
            List<string> args = ConvertToArgList(argString);

            var result = Helpers.TryGetArgumentValueOrDefault(args, argName, out var actual, "the-default-value");

            result.Should().BeTrue();
            actual.Should().Be("the-api-key");
        }

        [Theory]
        [InlineData("", "--api-key")]
        [InlineData("--date 2017-02-27", "--api-key")]
        [InlineData(@"--output .\path\to\store\photos --date 2017-02-27", "--api-key")]
        public void TryGetArgumentValueOrDefault_Returns_True_WithDefaultValueWhenArgumentNotSpecified(string argString, string argName)
        {
            List<string> args = ConvertToArgList(argString);

            var result = Helpers.TryGetArgumentValueOrDefault(args, argName, out var actual, "the-default-value");

            result.Should().BeTrue();
            actual.Should().Be("the-default-value");
        }

        [Theory]
        [InlineData("")]
        [InlineData("--help")]
        [InlineData("--api-key the-api-key --help")]
        [InlineData("--api-key the-api-key --help --date 2017-02-27")]
        public void HelpRequest_ReturnsTrue(string argString)
        {
            List<string> args = ConvertToArgList(argString);

            Helpers.HelpRequested(args).Should().BeTrue("--help was specified as an argument or no arguments were specified at all.");
        }

        [Theory]
        [InlineData("--api-key the-api-key", "the-api-key")]
        [InlineData("--api-key the-api-key --date 2017-02-27", "the-api-key")]
        [InlineData(@"--output = .\path\to\store\photos --api-key the-api-key --date 2017-02-27", "the-api-key")]
        [InlineData("", "DEMO_KEY")]
        [InlineData("--date 2017-02-27", "DEMO_KEY")]
        [InlineData(@"--output .\path\to\store\photos --date 2017-02-27", "DEMO_KEY")]
        public void GetApiKey_Returns_SpecifiedApiKey_Or_DefaultValue(string argString, string expected)
        {
            var args = ConvertToArgList(argString);

            var actual = Helpers.GetApiKey(args, "DEMO_KEY");

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("--api-key")]
        [InlineData("--api-key --date 2017-02-27")]
        [InlineData(@"--output .\path\to\store\photos --api-key --date 2017-02-27")]
        public void GetApiKey_Throws_ArgumentException_OnBadArgumentSpecification(string argString)
        {
            var args = ConvertToArgList(argString);

            Action act = () => Helpers.GetApiKey(args, "DEMO_KEY");

            act.Should()
                .Throw<ArgumentException>("the argument was specified but the following argument was another parameter or there were no more arguments.");
        }

        [Theory]
        [InlineData(@"--output .\path\to\store\photos", @".\path\to\store\photos")]
        [InlineData(@"--date 2017-02-27 --output .\path\to\store\photos", @".\path\to\store\photos")]
        [InlineData(@"--api-key the-api-key --output .\path\to\store\photos --date 2017-02-27", @".\path\to\store\photos")]
        [InlineData(@"", @"C:\MarsRoverPhotos")]
        [InlineData(@"--date 2017-02-27", @"C:\MarsRoverPhotos")]
        [InlineData(@"--api-key the-api-key --date 2017-02-27", @"C:\MarsRoverPhotos")]
        public void GetOutputPath_Returns_SpecifiedOutputPath_Or_DefaultValue(string argString, string expected)
        {
            var args = ConvertToArgList(argString);

            var actual = Helpers.GetOutputPath(args, @"C:\MarsRoverPhotos");

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("--output")]
        [InlineData("--output --date 2017-02-27")]
        [InlineData("--api-key the-api-key --output --date 2017-02-27")]
        public void GetOutputPath_Throws_ArgumentException_OnBadArgumentSpecification(string argString)
        {
            var args = ConvertToArgList(argString);

            Action act = () => Helpers.GetOutputPath(args, @"C:\MarsRoverPhotos");

            act.Should()
                .Throw<ArgumentException>("the argument was specified but the following argument was another parameter or there were no more arguments.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("April 31, 2018")]
        [InlineData("27-2-2017")]
        [InlineData("17/2/27")]
        public void GetDateArgValueOrThrow_Throws_IfNoValueProvidedOrDateCannotBeParsed(string argString)
        {
            var args = ConvertToArgList(argString);

            Action act = () => Helpers.GetDateArgValueOrThrow(args);

            act.Should().Throw<ArgumentException>("a value was not provided for the argument")
                .WithMessage("ERROR: No value was provided for --date or the value is not a valid date.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("--date --dates")]
        [InlineData(@"--date --dates .\path\to\dates")]
        [InlineData("--date 2017-02-27 --dates")]
        [InlineData(@"--date 2017-02-27 --dates .\path\to\dates")]
        public void GetDates_ThrowsArgumentException_IfNeitherOrBothDateAndDatesArgumentsAreProvided(string argString)
        {
            var args = ConvertToArgList(argString);

            Action act = () => Helpers.GetDates(args);

            act.Should().Throw<ArgumentException>("neither or both of --date and --dates arguments were provided")
                .WithMessage("ERROR: One of --date or --dates must have a value specified, but not both.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("April 31, 2018")]
        [InlineData("27-2-2017")]
        [InlineData("17/2/27")]
        public void GetDates_ThrowsArgumentException_IfDateCannotBeParsed(string dateString)
        {
            var args = new List<string> { "--date", dateString };

            Action act = () => Helpers.GetDates(args);

            act.Should().Throw<ArgumentException>("the specified date is an invalid date")
                .WithMessage(
                    "ERROR: No value was provided for --date or the value is not a valid date."
                );
        }

        [Theory]
        [InlineData("2017-02-27")]
        [InlineData("February 27, 2017")]
        [InlineData("27-Feb-2017")]
        [InlineData("Feb 27 2017")]
        [InlineData("02/27/2017")]
        [InlineData("2/27/2017")]
        [InlineData("2017-2-27")]
        [InlineData("2/27/17")]
        public void GetDates_ReturnsListOfOneDate_IfDateCanBeParsed(string dateString)
        {
            var args = new List<string> {"--date", dateString};

            var result = Helpers.GetDates(args);

            result.Should().ContainSingle()
                .Which.Should().Be(new DateTime(2017, 2, 27));
        }

        [Fact]
        public void GetDates_ThrowsArgumentException_IfFileWithDatesCannotBeProcessed()
        {
            var nonExistentFile = @".\does\not\exist.txt";
            var args = new List<string> { "--dates", nonExistentFile };

            Action act = () => Helpers.GetDates(args);

            act.Should().Throw<ArgumentException>("the file does not exist and at least one date is required")
                .WithMessage(
                    $"ERROR: Unable to process the file '{nonExistentFile}'.\n" +
                    "       Check that the file exists, is accessible, and contains at least one\n" +
                    "       valid date."
                );
        }

        [Fact]
        public void ParseDates_ReturnsListOfParseableDates()
        {
            Helpers.ParseDates(new[] {"02/27/17", "June 2, 2018", "Jul-13-2016", "April 31, 2018"})
                .Should().HaveCount(3, "the first 3 dates are valid while the last one is not")
                .And.HaveElementAt(0, new DateTime(2017, 2, 27))
                .And.HaveElementAt(1, new DateTime(2018, 6, 2))
                .And.HaveElementAt(2, new DateTime(2016, 7, 13));

        }

        [Fact]
        public void ReadAllLines_ReadsAllLinesFromStream()
        {
            const string fileContents = "02/27/17\nJune 2, 2018\r\n\nJul-13-2016\rApril 31, 2018";
            var expectedLines = fileContents.Split(new [] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            using var sr = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(fileContents)));

            var lines = Helpers.ReadAllLines(sr);

            lines.Should().HaveCount(5).And.Equal(expectedLines);
        }

        private List<string> ConvertToArgList(string argString) => argString.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}