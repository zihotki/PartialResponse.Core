// Copyright (c) Arjen Post and contributors. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Linq;
using Xunit;

namespace PartialResponse.Core.Tests
{
    public class ParserTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfContextIsNull()
        {
            // Arrange
            ParserContext context = null;

            // Act
            Assert.Throws<ArgumentNullException>("context", () => new Parser(context));
        }

        [Fact]
        public void TheParseMethodShouldParseEmptySource()
        {
            // Arrange
            var source = new StringReader(string.Empty);
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Empty(context.Values);
        }

        [Theory]
        [InlineData("/", TokenType.NestedFieldDelimiter)]
        [InlineData(".", TokenType.NestedFieldDelimiter)]
        [InlineData("(", TokenType.FieldGroupStartDelimiter)]
        [InlineData("[", TokenType.FieldGroupStartDelimiter)]
        [InlineData(")", TokenType.FieldGroupEndDelimiter)]
        [InlineData("]", TokenType.FieldGroupEndDelimiter)]
        [InlineData(",", TokenType.FieldsDelimiter)]
        public void TheParseMethodShouldSetErrorIfIllegalTokenAtStart(string value, TokenType tokenType)
        {
            // Arrange
            var source = new StringReader(value);
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(tokenType, context.Error.Type);
        }

        [Fact]
        public void TheParseMethodShouldParseSingleIdentifier()
        {
            // Arrange
            var source = new StringReader("foo");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldParseNestedIdentifier()
        {
            // Arrange
            var source = new StringReader("foo/bar");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData(".", "/", TokenType.NestedFieldDelimiter)]
        [InlineData(".", ".", TokenType.NestedFieldDelimiter)]
        [InlineData(".", "(", TokenType.FieldGroupStartDelimiter)]
        [InlineData(".", "[", TokenType.FieldGroupStartDelimiter)]
        [InlineData(".", ")", TokenType.FieldGroupEndDelimiter)]
        [InlineData(".", "]", TokenType.FieldGroupEndDelimiter)]
        [InlineData(".", ",", TokenType.FieldsDelimiter)]
        [InlineData(".", "", TokenType.Eof)]

        [InlineData("/", "/", TokenType.NestedFieldDelimiter)]
        [InlineData("/", ".", TokenType.NestedFieldDelimiter)]
        [InlineData("/", "(", TokenType.FieldGroupStartDelimiter)]
        [InlineData("/", "[", TokenType.FieldGroupStartDelimiter)]
        [InlineData("/", ")", TokenType.FieldGroupEndDelimiter)]
        [InlineData("/", "]", TokenType.FieldGroupEndDelimiter)]
        [InlineData("/", ",", TokenType.FieldsDelimiter)]
        [InlineData("/", "", TokenType.Eof)]
        public void TheParseMethodShouldSetErrorIfIllegalTokenAfterNestedFieldDelimiter(string delimiter, string value, TokenType tokenType)
        {
            // Arrange
            var source = new StringReader($"foo{delimiter}{value}");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(tokenType, context.Error.Type);
        }

        [Fact]
        public void TheParseMethodShouldParseMultipleIdentifiers()
        {
            // Arrange
            var source = new StringReader("foo,bar");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo", "bar" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("/", TokenType.NestedFieldDelimiter)]
        [InlineData(".", TokenType.NestedFieldDelimiter)]
        [InlineData("(", TokenType.FieldGroupStartDelimiter)]
        [InlineData("[", TokenType.FieldGroupStartDelimiter)]
        [InlineData(")", TokenType.FieldGroupEndDelimiter)]
        [InlineData("]", TokenType.FieldGroupEndDelimiter)]
        [InlineData(",", TokenType.FieldsDelimiter)]
        [InlineData("", TokenType.Eof)]
        public void TheParseMethodShouldSetErrorIfIllegalTokenAfterComma(string value, TokenType tokenType)
        {
            // Arrange
            var source = new StringReader($"foo,{value}");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(tokenType, context.Error.Type);
        }

        [Theory]
        [InlineData("(", ")")]
        [InlineData("[", "]")]
        public void TheParseMethodShouldParseGroupedIdentifier(string leftGroupDelimiter, string rightGroupDelimiter)
        {
            // Arrange
            var source = new StringReader($"foo{leftGroupDelimiter}bar{rightGroupDelimiter}");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("(", ")")]
        [InlineData("[", "]")]
        public void TheParseMethodShouldParseGroupedMultipleIdentifiers(string leftGroupDelimiter, string rightGroupDelimiter)
        {
            // Arrange
            var source = new StringReader($"foo{leftGroupDelimiter}bar,baz{rightGroupDelimiter}");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar", "foo/baz" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("(", ")")]
        [InlineData("[", "]")]
        public void TheParseMethodShouldParseIdentifierAfterGroupedMultipleIdentifiers(string leftGroupDelimiter, string rightGroupDelimiter)
        {
            // Arrange
            var source = new StringReader($"foo{leftGroupDelimiter}bar,baz{rightGroupDelimiter},qux");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar", "foo/baz", "qux" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("(", ")", "/")]
        [InlineData("[", "]", "/")]
        [InlineData("(", ")", ".")]
        [InlineData("[", "]", ".")]
        public void TheParseMethodShouldParseGroupedNestedIdentifier(string leftGroupDelimiter, string rightGroupDelimiter, string nestedFieldDelimiter)
        {
            // Arrange
            var source = new StringReader($"foo{leftGroupDelimiter}bar{nestedFieldDelimiter}baz{rightGroupDelimiter}");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar/baz" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("(", ")", "/")]
        [InlineData("[", "]", "/")]
        [InlineData("(", ")", ".")]
        [InlineData("[", "]", ".")]
        public void TheParseMethodShouldParseGroupedMultipleNestedIdentifier(string leftGroupDelimiter, string rightGroupDelimiter, string nestedFieldDelimiter)
        {
            // Arrange
            var source = new StringReader($"foo{leftGroupDelimiter}bar{nestedFieldDelimiter}baz,qux{nestedFieldDelimiter}quux{rightGroupDelimiter}");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar/baz", "foo/qux/quux" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("(")]
        [InlineData("[")]
        public void TheParseMethodShouldSetErrorIfTooManyLeftDelimiters(string leftDelimiter)
        {
            // Arrange
            var source = new StringReader($"foo{leftDelimiter}bar");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(TokenType.Eof, context.Error.Type);
        }

        [Theory]
        [InlineData(")")]
        [InlineData("]")]
        public void TheParseMethodShouldSetErrorIfTooManyRightDelimiters(string rightDelimiter)
        {
            // Arrange
            var source = new StringReader($"foo(bar{rightDelimiter}{rightDelimiter}");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(TokenType.FieldGroupEndDelimiter, context.Error.Type);
        }

        [Theory]
        [InlineData("(", "/", TokenType.NestedFieldDelimiter)]
        [InlineData("(", ".", TokenType.NestedFieldDelimiter)]
        [InlineData("(", "(", TokenType.FieldGroupStartDelimiter)]
        [InlineData("(", "[", TokenType.FieldGroupStartDelimiter)]
        [InlineData("(", ")", TokenType.FieldGroupEndDelimiter)]
        [InlineData("(", "]", TokenType.FieldGroupEndDelimiter)]
        [InlineData("(", ",", TokenType.FieldsDelimiter)]
        [InlineData("(", "", TokenType.Eof)]
        [InlineData("[", "/", TokenType.NestedFieldDelimiter)]
        [InlineData("[", ".", TokenType.NestedFieldDelimiter)]
        [InlineData("[", "(", TokenType.FieldGroupStartDelimiter)]
        [InlineData("[", "[", TokenType.FieldGroupStartDelimiter)]
        [InlineData("[", ")", TokenType.FieldGroupEndDelimiter)]
        [InlineData("[", "]", TokenType.FieldGroupEndDelimiter)]
        [InlineData("[", ",", TokenType.FieldsDelimiter)]
        [InlineData("[", "", TokenType.Eof)]
        public void TheParseMethodShouldSetErrorIfIllegalTokenAfterLeftGroupDelimiter(string delimiter, string value, TokenType tokenType)
        {
            // Arrange
            var source = new StringReader($"foo{delimiter}{value}");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(tokenType, context.Error.Type);
        }

        [Theory]
        [InlineData(")", "/", TokenType.NestedFieldDelimiter)]
        [InlineData("]", "(", TokenType.FieldGroupStartDelimiter)]
        public void TheParseMethodShouldSetErrorIfIllegalTokenAfterRightGroupDelimiter(string delimiter, string value, TokenType tokenType)
        {
            // Arrange
            var source = new StringReader($"foo(bar{delimiter}{value}");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(tokenType, context.Error.Type);
        }

        [Theory]
        [InlineData("(", ")")]
        [InlineData("[", "]")]
        public void TheParseMethodShouldParseIdentifierAfterGroupedIdentifier(string left, string right)
        {
            // Arrange
            var source = new StringReader($"foo{left}bar{right},baz");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar", "baz" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("(", ")")]
        [InlineData("[", "]")]
        public void TheParseMethodShouldParseIdentifierAfterNestedGroupedIdentifiers(string left, string right)
        {
            // Arrange
            var source = new StringReader($"foo{left}bar{left}baz{right}{right},qux");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar/baz", "qux" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldIgnoreSpace()
        {
            // Arrange
            var source = new StringReader(" foo");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldIgnoreTab()
        {
            // Arrange
            var source = new StringReader("\tfoo");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldIgnoreCarriageReturn()
        {
            // Arrange
            var source = new StringReader("\rfoo");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldIgnoreNewLine()
        {
            // Arrange
            var source = new StringReader("\nfoo");
            var context = new ParserContext(source, DelimiterOptions.DefaultOptions);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }
    }
}