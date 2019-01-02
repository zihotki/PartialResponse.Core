// Copyright (c) Arjen Post and contributors. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace PartialResponse.Core
{
    /// <summary>
    /// Contains information used by a <see cref="Parser"/>.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public class ParserContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParserContext"/> class.
        /// </summary>
        /// <param name="source">A <see cref="TextReader"/> representing the input string.</param>
        /// <param name="options">Delimiters options for parser.</param>
        public ParserContext(TextReader source, DelimiterOptions options)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Source = source;
            this.Options = options;
            this.Values = new List<Field>();
        }

        /// <summary>
        /// Gets the error that occured while parsing.
        /// </summary>
        /// <returns>The error if an error occured while parsing; otherwise, null.</returns>
        public UnexpectedTokenError Error { get; internal set; }

        /// <summary>
        /// Gets the <see cref="TextReader"/> representing the input string.
        /// </summary>
        /// <returns>The <see cref="TextReader"/> representing the input string.</returns>
        public TextReader Source { get; }

        /// <summary>
        /// Gets the values that are extracted while parsing.
        /// </summary>
        /// <returns>The values that are extracted while parsing.</returns>
        public List<Field> Values { get; }

        /// <summary>
        /// Gets delimiters options.
        /// </summary>
        public DelimiterOptions Options { get; }
    }
}