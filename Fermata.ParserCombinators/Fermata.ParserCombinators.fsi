namespace Fermata.ParserCombinators

module Parsers =

    type State = State of string * int
    type Parser<'T> = State -> Result<'T * State, string * State>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the charactor at the specified position in the string in <c>State</c> matches <c>c</c>, otherwise <c>Error</c>.</summary>
    /// <param name="c">The input <c>char</c>.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="char'-1">
    /// <code lang="fsharp">
    /// char' 'f' (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Ok('f', State("fsharp", 1))</c>
    /// </example>
    ///
    /// <example id="char'-2">
    /// <code lang="fsharp">
    /// char' 's' (State("fsharp", 1))
    /// </code>
    /// Evaluates to <c>Ok('s', State("fsharp", 2))</c>
    /// </example>
    ///
    /// <example id="char'-3">
    /// <code lang="fsharp">
    /// char' 'c' (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Error("", State("fsharp", 0))</c>
    /// </example>
    ///
    /// <example id="char'-4">
    /// <code lang="fsharp">
    /// char' 'x' (State("fsharp", 6))
    /// </code>
    /// Evaluates to <c>Error("", State("fsharp", 6))</c>
    /// </example>
    val char': c: char -> Parser<char>

    /// <summary>Combines two parsers and returns a new parser that returns <c>Ok</c> if both input parsers succeed, otherwise <c>Error</c>.</summary>
    /// <param name="p1">The first input parser.</param>
    /// <param name="p2">The second input parser.</param>
    /// <returns>The result parser.</returns>
    val (<&>): p1: Parser<'T> -> p2: Parser<'U> -> Parser<'T * 'U>

    /// <summary>Combines two parsers and returns a new parser that returns <c>Ok</c> and only the first value if both input parsers succeed, otherwise <c>Error</c>.</summary>
    /// <param name="p1">The first input parser.</param>
    /// <param name="p2">The second input parser.</param>
    /// <returns>The result parser.</returns>
    val (<+&>): p1: Parser<'T> -> p2: Parser<'U> -> Parser<'T>

    /// <summary>Combines two parsers and returns a new parser that returns <c>Ok</c> and only the second value if both input parsers succeed, otherwise <c>Error</c>.</summary>
    /// <param name="p1">The first input parser.</param>
    /// <param name="p2">The second input parser.</param>
    /// <returns>The result parser.</returns>
    val (<&+>): p1: Parser<'T> -> p2: Parser<'U> -> Parser<'U>

    /// <summary>Combines two parsers and returns a new parser that returns <c>Ok</c> if either input parsers succeeds, otherwise <c>Error</c>.</summary>
    /// <param name="p1">The first input parser.</param>
    /// <param name="p2">The second input parser.</param>
    /// <returns>The result parser.</returns>
    val (<|>): p1: Parser<'T> -> p2: Parser<'T> -> Parser<'T>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the parser given to <c>many</c> succeeds more than 0 times.</summary>
    /// <param name="parser">The input parser.</param>
    /// <returns>The result parser.</returns>
    val many: parser: Parser<'T> -> Parser<'T list>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the parser given to <c>repN</c> succeeds just <c>n</c> times, otherwise <c>Error</c>.</summary>
    /// <param name="n">The number of times to parse.</param>
    /// <param name="parser">The input parser.</param>
    /// <returns>The result parser.</returns>
    val repN: n: int -> parser: Parser<'T> -> Parser<'T list>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the parser given to <c>map'</c> succeeds, otherwise <c>Error</c>.</summary>
    /// <param name="mapping">A function to apply to the OK result value.</param>
    /// <param name="parser">The input parser.</param>
    /// <returns>The result parser.</returns>
    val map': mapping: ('T -> 'U) -> parser: Parser<'T> -> Parser<'U>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the parser given to <c>bind</c> succeeds, otherwise <c>Error</c>.</summary>
    /// <param name="binder">A function that takes the value of type T from a result and transforms it into a result containing a value of type U.</param>
    /// <param name="parser">The input parser.</param>
    /// <returns>The result parser.</returns>
    val bind: binder: ('T -> Result<'U, string>) -> parser: Parser<'T> -> Parser<'U>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the substring from the specified position in the string in <c>State</c> matches <c>s</c>, otherwise <c>Error</c>.</summary>
    /// <param name="s">The input <c>string</c>.</param>
    /// <returns>The result parser.</returns>
    val string': s: string -> Parser<string>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the string in <c>State</c> matches <c>pattern</c>, otherwise <c>Error</c>.</summary>
    /// <param name="pattern">Regular Expression pattern.</param>
    /// <returns>The result parser.</returns>
    val regex: pattern: string -> Parser<string>
