// Fermata.ParserCombinators Version 0.1.0
// https://github.com/taidalog/Fermata.ParserCombinators
// Copyright (c) 2024 taidalog
// This software is licensed under the MIT License.
// https://github.com/taidalog/Fermata.ParserCombinators/blob/main/LICENSE

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
    /// <param name="parser1">The first input parser.</param>
    /// <param name="parser2">The second input parser.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="<&>-1">
    /// <code lang="fsharp">
    /// (char' 'f' <&> char' 's') (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Ok(('f', 's'), State("fsharp", 2))</c>
    /// </example>
    ///
    /// <example id="<&>-2">
    /// <code lang="fsharp">
    /// (char' 'f' <&> char' '#') (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Error("", State("fsharp", 0))</c>
    /// </example>
    ///
    /// <example id="<&>-3">
    /// <code lang="fsharp">
    /// (char' 'f' <&> char' 's' <&> char' 'h') (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Ok((('f', 's'), 'h'), State("fsharp", 3))</c>
    /// </example>
    ///
    /// <example id="<&>-4">
    /// <code lang="fsharp">
    /// (char' 'f' <&> char' 's' <&> char' 's') (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Error("", State("fsharp", 0))</c>
    /// </example>
    val (<&>): parser1: Parser<'T> -> parser2: Parser<'U> -> Parser<'T * 'U>

    /// <summary>Combines two parsers and returns a new parser that returns <c>Ok</c> and only the first value if both input parsers succeed, otherwise <c>Error</c>.</summary>
    /// <param name="parser1">The first input parser.</param>
    /// <param name="parser2">The second input parser.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="<+&>-1">
    /// <code lang="fsharp">
    /// (char' 'f' <+&> char' 's') (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Ok('f', State("fsharp", 2))</c>
    /// </example>
    ///
    /// <example id="<+&>-2">
    /// <code lang="fsharp">
    /// let digit = [ '0' .. '9' ] |> List.map char' |> List.reduce (<|>)
    /// let number =
    ///     let f = List.map string >> String.concat "" >> int
    ///     map' f (many digit)
    /// (number <+&> string' " yen") (State("100 yen", 0))
    /// </code>
    /// Evaluates to <c>Ok(100, State("100 yen", 7))</c>
    /// </example>
    val (<+&>): parser1: Parser<'T> -> parser2: Parser<'U> -> Parser<'T>

    /// <summary>Combines two parsers and returns a new parser that returns <c>Ok</c> and only the second value if both input parsers succeed, otherwise <c>Error</c>.</summary>
    /// <param name="parser1">The first input parser.</param>
    /// <param name="parser2">The second input parser.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="<&+>-1">
    /// <code lang="fsharp">
    /// (char' 'f' <&+> char' 's') (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Ok('s', State("fsharp", 2))</c>
    /// </example>
    ///
    /// <example id="<&+>-2">
    /// <code lang="fsharp">
    /// let hex = [ '0' .. '9' ] @ [ 'a' .. 'f' ] |> List.map char' |> List.reduce (<|>)
    /// let hexCode =
    ///     let f (x, y) =
    ///         sprintf "%c%s" x ((List.map string >> String.concat "") y)
    ///     map' f (char' '#' <&> repeat 6 hex)
    /// (string' "color: " <&+> hexCode) (State("color: #65a2ac", 0))
    /// </code>
    /// Evaluates to <c>Ok("#65a2ac", State("color: #65a2ac", 14))</c>
    /// </example>
    ///
    /// <example id="<&+>-3">
    /// <code lang="fsharp">
    /// (string' "I'm " <&+> string' "taidalog" <+&> char' '.') (State("I'm taidalog.", 0))
    /// </code>
    /// Evaluates to <c>Ok("taidalog", State("I'm taidalog.", 13))</c>
    /// </example>
    val (<&+>): parser1: Parser<'T> -> parser2: Parser<'U> -> Parser<'U>

    /// <summary>Combines two parsers and returns a new parser that returns <c>Ok</c> if either input parsers succeeds, otherwise <c>Error</c>.</summary>
    /// <param name="parser1">The first input parser.</param>
    /// <param name="parser2">The second input parser.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="<|>-1">
    /// <code lang="fsharp">
    /// (char' 'f' <|> char' 'c') (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Ok('f', State("fsharp", 1))</c>
    /// </example>
    ///
    /// <example id="<|>-2">
    /// <code lang="fsharp">
    /// (char' 'f' <|> char' 'c') (State("csharp", 0))
    /// </code>
    /// Evaluates to <c>Ok('c', State("csharp", 1))</c>
    /// </example>
    ///
    /// <example id="<|>-3">
    /// <code lang="fsharp">
    /// (char' 'f' <|> char' 'c') (State("sharp", 0))
    /// </code>
    /// Evaluates to <c>Error("", State("sharp", 0))</c>
    /// </example>
    val (<|>): parser1: Parser<'T> -> parser2: Parser<'T> -> Parser<'T>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the parser given to <c>many</c> succeeds more than 0 times.</summary>
    /// <param name="parser">The input parser.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="many-1">
    /// <code lang="fsharp">
    /// many (char' 'w') (State("www.taida.com", 0))
    /// </code>
    /// Evaluates to <c>Ok([ 'w'; 'w'; 'w' ], State("www.taida.com", 3))</c>
    /// </example>
    ///
    /// <example id="many-2">
    /// <code lang="fsharp">
    /// let digit =
    ///     char' '0'
    ///     <|> char' '1'
    ///     <|> char' '2'
    ///     <|> char' '3'
    ///     <|> char' '4'
    ///     <|> char' '5'
    ///     <|> char' '6'
    ///     <|> char' '7'
    ///     <|> char' '8'
    ///     <|> char' '9'
    /// many (digit) (State("123 hey!", 0))
    /// </code>
    /// Evaluates to <c>Ok([ '1'; '2'; '3' ], State("123 hey!", 3))</c>
    /// </example>
    ///
    /// <example id="many-3">
    /// <code lang="fsharp">
    /// let abc = char' 'a' <|> char' 'b' <|> char' 'c'
    /// many (abc) (State("123 hey!", 0))
    /// </code>
    /// Evaluates to <c>Ok([], State("123 hey!", 0))</c>
    /// </example>
    val many: parser: Parser<'T> -> Parser<'T list>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the parser given to <c>repeat</c> succeeds just <c>n</c> times, otherwise <c>Error</c>.</summary>
    /// <param name="count">The number of times to parse.</param>
    /// <param name="parser">The input parser.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="repeat-1">
    /// <code lang="fsharp">
    /// repeat 3 (char' 'w') (State("www.~.com", 0))
    /// </code>
    /// Evaluates to <c>Ok([ 'w'; 'w'; 'w' ], State("www.~.com", 3))</c>
    /// </example>
    ///
    /// <example id="repeat-2">
    /// <code lang="fsharp">
    /// let hex =
    ///     char' '0'
    ///     <|> char' '1'
    ///     <|> char' '2'
    ///     <|> char' '3'
    ///     <|> char' '4'
    ///     <|> char' '5'
    ///     <|> char' '6'
    ///     <|> char' '7'
    ///     <|> char' '8'
    ///     <|> char' '9'
    ///     <|> char' 'a'
    ///     <|> char' 'b'
    ///     <|> char' 'c'
    ///     <|> char' 'd'
    ///     <|> char' 'e'
    ///     <|> char' 'f'
    /// repeat 6 hex (State("#65a2ac", 1))
    /// </code>
    /// Evaluates to <c>Ok([ '6'; '5'; 'a'; '2'; 'a'; 'c' ], State("#65a2ac", 7))</c>
    /// </example>
    ///
    /// <example id="repeat-3">
    /// <code lang="fsharp">
    /// let hex = [ '0' .. '9' ] @ [ 'a' .. 'f' ] |> List.map char' |> List.reduce (<|>)
    /// repeat 6 hex (State("#65a2ac", 0))
    /// </code>
    /// Evaluates to <c>Error("", State("#65a2ac", 0))</c>
    /// </example>
    val repeat: count: int -> parser: Parser<'T> -> Parser<'T list>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the parser given to <c>map'</c> succeeds, otherwise <c>Error</c>.</summary>
    /// <param name="mapping">A function to apply to the OK result value.</param>
    /// <param name="parser">The input parser.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="map'-1">
    /// <code lang="fsharp">
    /// let digit =
    ///     char' '0'
    ///     <|> char' '1'
    ///     <|> char' '2'
    ///     <|> char' '3'
    ///     <|> char' '4'
    ///     <|> char' '5'
    ///     <|> char' '6'
    ///     <|> char' '7'
    ///     <|> char' '8'
    ///     <|> char' '9'
    /// map' (string >> int) digit (State("123 hey!", 0))
    /// </code>
    /// Evaluates to <c>Ok(1, State("123 hey!", 1))</c>
    /// </example>
    ///
    /// <example id="map'-2">
    /// <code lang="fsharp">
    /// let digit = [ '0' .. '9' ] |> List.map char' |> List.reduce (<|>)
    /// let f = List.map string >> String.concat "" >> int
    /// map' f (many digit) (State("123 hey!", 0))
    /// </code>
    /// Evaluates to <c>Ok(123, State("123 hey!", 3))</c>
    /// </example>
    ///
    /// <example id="map'-3">
    /// <code lang="fsharp">
    /// let hex =
    ///     char' '0'
    ///     <|> char' '1'
    ///     <|> char' '2'
    ///     <|> char' '3'
    ///     <|> char' '4'
    ///     <|> char' '5'
    ///     <|> char' '6'
    ///     <|> char' '7'
    ///     <|> char' '8'
    ///     <|> char' '9'
    ///     <|> char' 'a'
    ///     <|> char' 'b'
    ///     <|> char' 'c'
    ///     <|> char' 'd'
    ///     <|> char' 'e'
    ///     <|> char' 'f'
    /// let f = List.map string >> String.concat ""
    /// let g (x, y) = sprintf "%c%s" x (f y)
    /// map' g (char' '#' <&> (repeat 6 hex)) (State("#65a2ac", 0))
    /// </code>
    /// Evaluates to <c>Ok("#65a2ac", State("#65a2ac", 7))</c>
    /// </example>
    ///
    /// <example id="map'-4">
    /// <code lang="fsharp">
    /// let hex = [ '0' .. '9' ] @ [ 'a' .. 'f' ] |> List.map char' |> List.reduce (<|>)
    /// let f = List.map string >> String.concat "" >> int
    /// map' f (repeat 6 hex) (State("#65a2ac", 0))
    /// </code>
    /// Evaluates to <c>Error("", State("#65a2ac", 0))</c>
    /// </example>
    val map': mapping: ('T -> 'U) -> parser: Parser<'T> -> Parser<'U>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the parser given to <c>bind</c> succeeds, otherwise <c>Error</c>.</summary>
    /// <param name="binder">A function that takes the value of type T from a result and transforms it into a result containing a value of type U.</param>
    /// <param name="parser">The input parser.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="bind-1">
    /// <code lang="fsharp">
    /// let digit =
    ///     char' '0'
    ///     <|> char' '1'
    ///     <|> char' '2'
    ///     <|> char' '3'
    ///     <|> char' '4'
    ///     <|> char' '5'
    ///     <|> char' '6'
    ///     <|> char' '7'
    ///     <|> char' '8'
    ///     <|> char' '9'
    /// let binder x =
    ///     match x with
    ///     | [] -> Error ""
    ///     | _ -> x |> List.map string |> String.concat "" |> int |> Ok
    /// bind binder (many digit) (State("123 hey!", 0))
    /// </code>
    /// Evaluates to <c>Ok(123, State("123 hey!", 3))</c>
    /// </example>
    ///
    /// <example id="bind-2">
    /// <code lang="fsharp">
    /// let digit = [ '0' .. '9' ] |> List.map char' |> List.reduce (<|>)
    /// let binder x =
    ///     match x with
    ///     | [] -> Error ""
    ///     | _ -> x |> List.map string |> String.concat "" |> int |> Ok
    /// bind binder (many digit) (State("#65a2ac", 0))
    /// </code>
    /// Evaluates to <c>Error("", State("#65a2ac", 0))</c>
    /// </example>
    val bind: binder: ('T -> Result<'U, string>) -> parser: Parser<'T> -> Parser<'U>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the substring from the specified position in the string in <c>State</c> matches <c>s</c>, otherwise <c>Error</c>.</summary>
    /// <param name="s">The input <c>string</c>.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="string'-1">
    /// <code lang="fsharp">
    /// string' "fs" (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Ok("fs", State("fsharp", 2))</c>
    /// </example>
    ///
    /// <example id="string'-2">
    /// <code lang="fsharp">
    /// string' "fsharp" (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Ok("fsharp", State("fsharp", 6))</c>
    /// </example>
    ///
    /// <example id="string'-3">
    /// <code lang="fsharp">
    /// string' "sharp" (State("fsharp", 1))
    /// </code>
    /// Evaluates to <c>Ok("sharp", State("fsharp", 6))</c>
    /// </example>
    ///
    /// <example id="string'-4">
    /// <code lang="fsharp">
    /// string' "csharp" (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Error("", State("fsharp", 0))</c>
    /// </example>
    val string': s: string -> Parser<string>

    /// <summary>Returns a new parser that takes a <c>State</c> and returns <c>Ok(v, State)</c> if the string in <c>State</c> matches <c>pattern</c>, otherwise <c>Error</c>.</summary>
    /// <param name="pattern">Regular Expression pattern.</param>
    /// <returns>The result parser.</returns>
    ///
    /// <example id="regex-1">
    /// <code lang="fsharp">
    /// regex ".sharp" (State("fsharp", 0))
    /// </code>
    /// Evaluates to <c>Ok("fsharp", State("fsharp", 6))</c>
    /// </example>
    ///
    /// <example id="regex-2">
    /// <code lang="fsharp">
    /// regex "^[^\.]+" (State("I'm taidalog.", 4))
    /// </code>
    /// Evaluates to <c>Ok("taidalog", State("I'm taidalog.", 12))</c>
    /// </example>
    ///
    /// <example id="regex-3">
    /// <code lang="fsharp">
    /// regex "^#[0-9a-f]{6}" (State("color: #65a2ac;", 7))
    /// </code>
    /// Evaluates to <c>Ok("#65a2ac", State("color: #65a2ac;", 14))</c>
    /// </example>
    ///
    /// <example id="regex-4">
    /// <code lang="fsharp">
    /// regex "^#[0-9A-F]{6}" (State("color: #65a2ac;", 7))
    /// </code>
    /// Evaluates to <c>Error("", State("color: #65a2ac;", 7))</c>
    /// </example>
    val regex: pattern: string -> Parser<string>
