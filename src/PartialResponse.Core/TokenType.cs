// Copyright (c) Arjen Post and contributors. See LICENSE in the project root for license information.

namespace PartialResponse.Core
{
    /// <summary>
    /// Specifies the token types that can be identified and returned by the <see cref="Tokenizer"/>.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public enum TokenType
    {
        /// <summary>
        /// An identifier. Typically, a contiguous run of characters not matching a forward slash, left parenthesis,
        /// right parenthesis, comma, or whitespace. For example, 'foo/bar' results in two identifiers; 'foo' and
        /// 'bar'.
        /// </summary>
        Identifier,

        /// <summary>
        /// A nested field delimiter, for example - forward slash ('/').
        /// </summary>
        NestedFieldDelimiter,

        /// <summary>
        /// A field group start delimiter, for example - opening parenthesis ('(').
        /// </summary>
        FieldGroupStartDelimiter,

        /// <summary>
        /// A field group end delimiter, for example - closing parenthesis (')').
        /// </summary>
        FieldGroupEndDelimiter,

        /// <summary>
        /// A fields delimiter, for example - comma (',').
        /// </summary>
        FieldsDelimiter,

        /// <summary>
        /// A space, horizontal tab, new line, or carriage return. Typically, a contiguous run of whitespace is a
        /// single whitespace token. For example, the two spaces in 'foo  bar' result in a single whitespace token.
        /// </summary>
        WhiteSpace,

        /// <summary>
        /// The end of the source has been reached.
        /// </summary>
        Eof
    }
}
