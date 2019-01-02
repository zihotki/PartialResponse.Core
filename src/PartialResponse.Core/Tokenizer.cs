// Copyright (c) Arjen Post and contributors. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PartialResponse.Core
{
    /// <summary>
    /// A tokenizer for fields parameter tokenization.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public class Tokenizer
    {
        private readonly TextReader source;
        private readonly IReadOnlyDictionary<char, TokenType> tokensMap;
        private readonly StringBuilder buffer = new StringBuilder();

        private int position = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        /// <param name="source">A <see cref="TextReader"/> representing the input string.</param>
        /// <param name="tokensMap">A map of non-identifier tokens to their character representations.</param>
        public Tokenizer(TextReader source, IReadOnlyDictionary<char, TokenType> tokensMap)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (tokensMap == null)
            {
                throw new ArgumentNullException(nameof(tokensMap));
            }

            this.source = source;
            this.tokensMap = tokensMap;
        }

        /// <summary>
        /// Returns the next token from the input string.
        /// </summary>
        /// <returns>The next token from the input string, or null if the end has been reached.</returns>
        public Token NextToken()
        {
            if (this.IsEndReached())
            {
                return new Token(null, TokenType.Eof, this.position);
            }

            var c = this.GetCurrentCharacter();

            if (this.tokensMap.TryGetValue(c, out var tokenType))
            {
                this.TakeCharacter();

                return this.CreateToken(tokenType);
            }
            else if (char.IsWhiteSpace(this.GetCurrentCharacter()))
            {
                this.TakeCharactersWhile(character => char.IsWhiteSpace(character));

                return this.CreateToken(TokenType.WhiteSpace);
            }
            else
            {
                this.TakeCharactersWhile(character => !this.tokensMap.ContainsKey(character) && !char.IsWhiteSpace(character));

                return this.CreateToken(TokenType.Identifier);
            }
        }

        private Token CreateToken(TokenType tokenType)
        {
            var token = new Token(this.buffer.ToString(), tokenType, this.position);

            this.buffer.Clear();

            return token;
        }

        private void TakeCharactersWhile(Func<char, bool> predicate)
        {
            while (!this.IsEndReached() && predicate(this.GetCurrentCharacter()))
            {
                this.TakeCharacter();
            }
        }

        private void TakeCharacter()
        {
            this.buffer.Append(this.GetCurrentCharacter());

            this.source.Read();

            this.position++;
        }

        private char GetCurrentCharacter()
        {
            var value = this.source.Peek();

            return value == -1 ? '\0' : (char)value;
        }

        private bool IsEndReached()
        {
            return this.source.Peek() == -1;
        }
    }
}