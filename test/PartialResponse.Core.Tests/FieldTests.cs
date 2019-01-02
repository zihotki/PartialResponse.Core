// Copyright (c) Arjen Post and contributors. See LICENSE in the project root for license information.

using System;
using Xunit;

namespace PartialResponse.Core.Tests
{
    public class FieldTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfValuesIsNull()
        {
            // Arrange
            string[] value = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => new Field(value));
        }

        [Fact]
        public void TheConstructorShouldThrowIfOneOfValuesIsNull()
        {
            // Arrange
            string value = null;

            // Act
            Assert.Throws<ArgumentException>(() => new Field("bla", value));
        }

        [Fact]
        public void ThePartsPropertyShouldContainSingleValue()
        {
            // Arrange
            // Act
            var field = new Field("foo");

            // Assert
            Assert.Equal(new[] { "foo" }, field.Parts);
        }

        [Fact]
        public void TheMatchesMethodShouldThrowIfPartsIsNull()
        {
            // Arrange
            string[] parts = null;

            var field = new Field("foo");

            // Assert
            Assert.Throws<ArgumentNullException>(() => field.Matches(parts));
        }

        [Fact]
        public void TheMatchesMethodShouldReturnFalseForDifferentValues()
        {
            // Arrange
            var field = new Field("foo");

            // Act
            var result = field.Matches(new[] { "bar" });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnFalseForDifferentNestedValues()
        {
            // Arrange
            var field = new Field("foo", "bar");

            // Act
            var result = field.Matches(new[] { "foo", "baz" });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForSameValues()
        {
            // Arrange
            var field = new Field("foo");

            // Act
            var result = field.Matches(new[] { "foo" });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForSamePrefixValues()
        {
            // Arrange
            var field = new Field("foo");

            // Act
            var result = field.Matches(new[] { "foo", "bar" });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForOtherSuffixValue()
        {
            // Arrange
            var field = new Field("foo", "bar");

            // Act
            var result = field.Matches(new[] { "foo" });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForWildcard()
        {
            // Arrange
            var field = new Field("*");

            // Act
            var result = field.Matches(new[] { "foo" });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TheMatchesMethodShouldIgnoreCase()
        {
            // Arrange
            var field = new Field("foo");

            // Act
            var result = field.Matches(new[] { "FOO" }, true);

            // Assert
            Assert.True(result);
        }
    }
}