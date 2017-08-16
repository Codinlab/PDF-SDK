using DocumentFormat.Pdf.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DocumentFormat.Pdf.Tests.Objects
{
    public class DateObjectTests
    {
        [Fact]
        public void ParsesYearOnly()
        {
            // Arrange
            var expected = new DateTimeOffset(new DateTime(2017, 1, 1));
            var dateString = "D:2017";

            // Act
            var dateObj = new DateObject(dateString);

            // Assert
            Assert.Equal(expected, dateObj.Value);
        }

        [Fact]
        public void ParsesDateOnly()
        {
            // Arrange
            var expected = new DateTimeOffset(new DateTime(2017, 08, 16));
            var dateString = "D:20170816";

            // Act
            var dateObj = new DateObject(dateString);

            // Assert
            Assert.Equal(expected, dateObj.Value);
        }

        [Fact]
        public void ParsesDateTime()
        {
            // Arrange
            var expected = new DateTimeOffset(new DateTime(2017, 08, 16, 15, 14, 13));
            var dateString = "D:20170816151413";

            // Act
            var dateObj = new DateObject(dateString);

            // Assert
            Assert.Equal(expected, dateObj.Value);
        }

        [Fact]
        public void ParsesDateTimeWithOffset()
        {
            // Arrange
            var expected = new DateTimeOffset(new DateTime(2017, 08, 16, 15, 14, 13), TimeSpan.FromHours(-2));
            var dateString = "D:20170816151413-02'00'";

            // Act
            var dateObj = new DateObject(dateString);

            // Assert
            Assert.Equal(expected, dateObj.Value);
        }

        [Fact]
        public void AllowsOmitedPrefix()
        {
            // Arrange
            var expected = new DateTimeOffset(new DateTime(2017, 08, 16, 15, 14, 13));
            var dateString = "20170816151413";

            // Act
            var dateObj = new DateObject(dateString);

            // Assert
            Assert.Equal(expected, dateObj.Value);
        }

        [Fact]
        public void FormatsDateTimeOffset()
        {
            // Arrange
            var date = new DateTimeOffset(new DateTime(2017, 08, 16, 15, 14, 13), TimeSpan.FromHours(-2));
            var expected = "D:20170816151413-02'00'";

            // Act
            var dateObj = new DateObject(date);
            var stringValue = ((StringObject)dateObj).Value;

            // Assert
            Assert.Equal(expected, stringValue);
        }

    }
}
