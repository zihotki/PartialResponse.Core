// Copyright (c) Arjen Post and contributors. See LICENSE in the project root for license information.

using System;
using System.IO;
using Xunit;

namespace PartialResponse.Core.Tests
{
    public class TokenizerTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfReaderIsNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>("source", () => new Tokenizer(null, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap));
        }

        [Fact]
        public void TheConstructorShouldThrowIfDelimiterMapIsNull()
        {
            // Act
            Assert.Throws<ArgumentNullException>("tokensMap", () => new Tokenizer(new StringReader(string.Empty), null));
        }

        [Theory]
        [InlineData("/")]
        [InlineData(".")]
        public void TheNextTokenMethodShouldReturnTokenTypeAndValueNestedFieldDelimiter(string data)
        {
            // Arrange
            var reader = new StringReader(data);
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.NestedFieldDelimiter, token.Type);
            Assert.Equal(data, token.Value);
        }

        [Theory]
        [InlineData("(")]
        [InlineData("[")]
        public void TheNextTokenMethodShouldReturnTokenTypeAndValueStartGroupDelimiter(string data)
        {
            // Arrange
            var reader = new StringReader(data);
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.FieldGroupStartDelimiter, token.Type);

            Assert.Equal(data, token.Value);
        }

        [Theory]
        [InlineData(")")]
        [InlineData("]")]
        public void TheNextTokenMethodShouldReturnTokenTypeAndValueEndGroupDelimiter(string data)
        {
            // Arrange
            var reader = new StringReader(data);
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.FieldGroupEndDelimiter, token.Type);

            Assert.Equal(data, token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeAndValueFieldsDelimiter()
        {
            // Arrange
            var reader = new StringReader(",");
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.FieldsDelimiter, token.Type);
            Assert.Equal(",", token.Value);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void TheNextTokenMethodShouldReturnTokenTypeAndValueWhiteSpace(string value)
        {
            // Arrange
            var reader = new StringReader(value);
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.WhiteSpace, token.Type);
            Assert.Equal(value, token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeAndValueIdentifier()
        {
            // Arrange
            var reader = new StringReader("foo");
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
            Assert.Equal("foo", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeEofIfEndReached()
        {
            // Arrange
            var reader = new StringReader("foo");
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            tokenizer.NextToken();

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Eof, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeAndValueIdentifierBeforeForwardSlash()
        {
            // Arrange
            var reader = new StringReader("foo/");
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
            Assert.Equal("foo", token.Value);
        }

        [Theory]
        [InlineData("/")]
        [InlineData(".")]
        public void TheNextTokenMethodShouldReturnTokenTypeAndValueIdentifierAfterNestedFieldDelimiter(string delimiter)
        {
            // Arrange
            var reader = new StringReader($"{delimiter}foo");
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            tokenizer.NextToken();

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
            Assert.Equal("foo", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeAndValueIdentifierBeforeWhiteSpace()
        {
            // Arrange
            var reader = new StringReader("foo ");
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
            Assert.Equal("foo", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeAndValueIdentifierAfterWhiteSpace()
        {
            // Arrange
            var reader = new StringReader(" foo");
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            tokenizer.NextToken();

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
            Assert.Equal("foo", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenPosition()
        {
            // Arrange
            var reader = new StringReader("foo/");
            var tokenizer = new Tokenizer(reader, DelimiterOptions.DefaultOptions.DelimiterToTokenTypeMap);

            tokenizer.NextToken();

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(3, token.Position);
        }
    }
}