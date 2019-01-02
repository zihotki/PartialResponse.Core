// Copyright (c) Arjen Post and contributors. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace PartialResponse.Core
{
    /// <summary>
    /// A parser for fields parameter parsing.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public class Parser
    {
        private readonly Stack<List<string>> prefixes = new Stack<List<string>>();
        private readonly Dictionary<TokenType, Action> handlers;
        private readonly ParserContext context;
        private readonly Tokenizer tokenizer;

        private Token currentToken;
        private Token previousToken;
        private int depth;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="context">A <see cref="ParserContext"/> used by the parser.</param>
        public Parser(ParserContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.context = context;

            this.tokenizer = new Tokenizer(context.Source, context.Options.DelimiterToTokenTypeMap);

            this.handlers = new Dictionary<TokenType, Action>
            {
                { TokenType.NestedFieldDelimiter, this.HandleNestedFieldDelimiter },
                { TokenType.FieldGroupStartDelimiter, this.HandleFieldGroupStartDelimiter },
                { TokenType.FieldGroupEndDelimiter, this.HandleFieldGroupEndDelimiter },
                { TokenType.FieldsDelimiter, this.HandleFieldsDelimiter },
                { TokenType.Eof, this.HandleEof }
            };
        }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        public void Parse()
        {
            this.NextToken();
            this.HandleIdentifier(acceptEnd: true);
        }

        private void HandleIdentifier(bool acceptEnd)
        {
            if (this.currentToken.Type == TokenType.Eof)
            {
                if (!acceptEnd)
                {
                    this.context.Error = new UnexpectedTokenError(this.currentToken);
                }

                if (this.depth > 0)
                {
                    this.context.Error = new UnexpectedTokenError(this.currentToken);
                }

                return;
            }

            if (this.currentToken.Type != TokenType.Identifier)
            {
                this.context.Error = new UnexpectedTokenError(this.currentToken);

                return;
            }

            List<string> prefixes;

            if (this.prefixes.Count > 0)
            {
                prefixes = this.depth > 0 && this.previousToken.Type != TokenType.NestedFieldDelimiter
                    ? this.prefixes.Peek().ToList() // creating a copy of existing prefixes when we are diving deep
                    : this.prefixes.Pop();

                prefixes.Add(this.currentToken.Value);
            }
            else
            {
                prefixes = new List<string> { this.currentToken.Value };
            }

            this.prefixes.Push(prefixes);

            this.NextToken();

            if (!this.handlers.TryGetValue(this.currentToken.Type, out var handler))
            {
                this.context.Error = new UnexpectedTokenError(this.currentToken);

                return;
            }

            handler();
        }

        private void HandleNestedFieldDelimiter()
        {
            this.NextToken();
            this.HandleIdentifier(acceptEnd: false);
        }

        private void HandleFieldGroupStartDelimiter()
        {
            this.depth++;

            this.NextToken();
            this.HandleIdentifier(acceptEnd: false);
        }

        private void HandleFieldGroupEndDelimiter()
        {
            do
            {
                var value = this.prefixes.Pop();

                if (this.previousToken.Type == TokenType.Identifier)
                {
                    this.context.Values.Add(new Field(value.ToArray()));
                }

                this.depth--;

                if (this.depth < 0)
                {
                    this.context.Error = new UnexpectedTokenError(this.currentToken);

                    return;
                }

                this.NextToken();
            }
            while (this.currentToken.Type == TokenType.FieldGroupEndDelimiter);

            if (this.currentToken.Type != TokenType.Eof)
            {
                if (this.currentToken.Type != TokenType.FieldsDelimiter)
                {
                    this.context.Error = new UnexpectedTokenError(this.currentToken);

                    return;
                }

                this.prefixes.Pop();

                this.NextToken();
                this.HandleIdentifier(acceptEnd: false);
            }
        }

        private void HandleFieldsDelimiter()
        {
            var value = this.prefixes.Pop();

            this.context.Values.Add(new Field(value.ToArray()));

            this.NextToken();
            this.HandleIdentifier(acceptEnd: false);
        }

        private void HandleEof()
        {
            if (this.depth > 0)
            {
                this.context.Error = new UnexpectedTokenError(this.currentToken);

                return;
            }

            var value = this.prefixes.Pop();

            this.context.Values.Add(new Field(value.ToArray()));
        }

        private void NextToken()
        {
            this.previousToken = this.currentToken;

            while (true)
            {
                var token = this.tokenizer.NextToken();

                if (token.Type != TokenType.WhiteSpace)
                {
                    this.currentToken = token;

                    return;
                }
            }
        }
    }
}