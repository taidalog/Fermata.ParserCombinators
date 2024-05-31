// Fermata.ParserCombinators Version 0.2.0
// https://github.com/taidalog/Fermata.ParserCombinators
// Copyright (c) 2024 taidalog
// This software is licensed under the MIT License.
// https://github.com/taidalog/Fermata.ParserCombinators/blob/main/LICENSE

module Tests

open System
open Xunit
open Fermata.ParserCombinators.Parsers

[<Fact>]
let ``char' 1`` () =
    let expected = Ok('f', State("fsharp", 1))
    let actual = char' 'f' (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``char' 2`` () =
    let expected = Ok('s', State("fsharp", 2))
    let actual = char' 's' (State("fsharp", 1))
    Assert.Equal(expected, actual)

[<Fact>]
let ``char' 3`` () =
    let expected = Error("Parsing failed.", State("fsharp", 0))
    let actual = char' 'c' (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``char' 4`` () =
    let expected = Error("Position exceeded input length.", State("fsharp", 6))
    let actual = char' 'x' (State("fsharp", 6))
    Assert.Equal(expected, actual)

[<Fact>]
let ``char' 5`` () =
    let expected = Error("Input was empty.", State("", 0))
    let actual = char' 'x' (State("", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&> 1`` () =
    let expected = Ok(('f', 's'), State("fsharp", 2))
    let actual = (char' 'f' <&> char' 's') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&> 2`` () =
    let expected = Error("Parsing failed.", State("fsharp", 0))
    let actual = (char' 'f' <&> char' '#') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&> 3`` () =
    let expected = Ok((('f', 's'), 'h'), State("fsharp", 3))
    let actual = (char' 'f' <&> char' 's' <&> char' 'h') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&> 4`` () =
    let expected = Error("Parsing failed.", State("fsharp", 0))
    let actual = (char' 'f' <&> char' 's' <&> char' 's') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<+&> 1`` () =
    let expected = Ok('f', State("fsharp", 2))
    let actual = (char' 'f' <+&> char' 's') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<+&> 2`` () =
    let digit = [ '0' .. '9' ] |> List.map char' |> List.reduce (<|>)

    let number =
        let f = List.map string >> String.concat "" >> int
        map' f (many digit)

    let expected = Ok(100, State("100 yen", 7))
    let actual = (number <+&> string' " yen") (State("100 yen", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&+> 1`` () =
    let expected = Ok('s', State("fsharp", 2))
    let actual = (char' 'f' <&+> char' 's') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&+> 2`` () =
    let hex = [ '0' .. '9' ] @ [ 'a' .. 'f' ] |> List.map char' |> List.reduce (<|>)

    let hexCode =
        let f (x, y) =
            sprintf "%c%s" x ((List.map string >> String.concat "") y)

        map' f (char' '#' <&> repeat 6 hex)

    let expected = Ok("#65a2ac", State("color: #65a2ac", 14))
    let actual = (string' "color: " <&+> hexCode) (State("color: #65a2ac", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&+> 3`` () =
    let expected = Ok("taidalog", State("I'm taidalog.", 13))

    let actual =
        (string' "I'm " <&+> string' "taidalog" <+&> char' '.') (State("I'm taidalog.", 0))

    Assert.Equal(expected, actual)

[<Fact>]
let ``<|> 1`` () =
    let expected = Ok('f', State("fsharp", 1))
    let actual = (char' 'f' <|> char' 'c') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<|> 2`` () =
    let expected = Ok('c', State("csharp", 1))
    let actual = (char' 'f' <|> char' 'c') (State("csharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<|> 3`` () =
    let expected = Error("Parsing failed.", State("sharp", 0))
    let actual = (char' 'f' <|> char' 'c') (State("sharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``many 1`` () =
    let expected = Ok([ 'w'; 'w'; 'w' ], State("www.taida.com", 3))
    let actual = many (char' 'w') (State("www.taida.com", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``many 2`` () =
    let digit =
        char' '0'
        <|> char' '1'
        <|> char' '2'
        <|> char' '3'
        <|> char' '4'
        <|> char' '5'
        <|> char' '6'
        <|> char' '7'
        <|> char' '8'
        <|> char' '9'

    let expected = Ok([ '1'; '2'; '3' ], State("123 hey!", 3))
    let actual = many (digit) (State("123 hey!", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``many 3`` () =
    let abc = char' 'a' <|> char' 'b' <|> char' 'c'
    let expected = Ok([], State("123 hey!", 0))
    let actual = many (abc) (State("123 hey!", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``repeat 1`` () =
    let expected = Ok([ 'w'; 'w'; 'w' ], State("www.~.com", 3))
    let actual = repeat 3 (char' 'w') (State("www.~.com", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``repeat 2`` () =
    let hex =
        char' '0'
        <|> char' '1'
        <|> char' '2'
        <|> char' '3'
        <|> char' '4'
        <|> char' '5'
        <|> char' '6'
        <|> char' '7'
        <|> char' '8'
        <|> char' '9'
        <|> char' 'a'
        <|> char' 'b'
        <|> char' 'c'
        <|> char' 'd'
        <|> char' 'e'
        <|> char' 'f'

    let expected = Ok([ '6'; '5'; 'a'; '2'; 'a'; 'c' ], State("#65a2ac", 7))
    let actual = repeat 6 hex (State("#65a2ac", 1))
    Assert.Equal(expected, actual)

[<Fact>]
let ``repeat 3`` () =
    let hex = [ '0' .. '9' ] @ [ 'a' .. 'f' ] |> List.map char' |> List.reduce (<|>)
    let expected = Error("Parsing failed.", State("#65a2ac", 0))
    let actual = repeat 6 hex (State("#65a2ac", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``map' 1`` () =
    let digit =
        char' '0'
        <|> char' '1'
        <|> char' '2'
        <|> char' '3'
        <|> char' '4'
        <|> char' '5'
        <|> char' '6'
        <|> char' '7'
        <|> char' '8'
        <|> char' '9'

    let expected = Ok(1, State("123 hey!", 1))
    let actual = map' (string >> int) digit (State("123 hey!", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``map' 2`` () =
    let digit = [ '0' .. '9' ] |> List.map char' |> List.reduce (<|>)
    let f = List.map string >> String.concat "" >> int
    let expected = Ok(123, State("123 hey!", 3))
    let actual = map' f (many digit) (State("123 hey!", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``map' 3`` () =
    let hex =
        char' '0'
        <|> char' '1'
        <|> char' '2'
        <|> char' '3'
        <|> char' '4'
        <|> char' '5'
        <|> char' '6'
        <|> char' '7'
        <|> char' '8'
        <|> char' '9'
        <|> char' 'a'
        <|> char' 'b'
        <|> char' 'c'
        <|> char' 'd'
        <|> char' 'e'
        <|> char' 'f'

    let f = List.map string >> String.concat ""

    let g (x, y) = sprintf "%c%s" x (f y)

    let expected = Ok("#65a2ac", State("#65a2ac", 7))
    let actual = map' g (char' '#' <&> (repeat 6 hex)) (State("#65a2ac", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``map' 4`` () =
    let hex = [ '0' .. '9' ] @ [ 'a' .. 'f' ] |> List.map char' |> List.reduce (<|>)
    let f = List.map string >> String.concat "" >> int
    let expected = Error("Parsing failed.", State("#65a2ac", 0))
    let actual = map' f (repeat 6 hex) (State("#65a2ac", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``bind 1`` () =
    let digit =
        char' '0'
        <|> char' '1'
        <|> char' '2'
        <|> char' '3'
        <|> char' '4'
        <|> char' '5'
        <|> char' '6'
        <|> char' '7'
        <|> char' '8'
        <|> char' '9'

    let binder x =
        match x with
        | [] -> Error ""
        | _ -> x |> List.map string |> String.concat "" |> int |> Ok

    let expected = Ok(123, State("123 hey!", 3))
    let actual = bind binder (many digit) (State("123 hey!", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``bind 2`` () =
    let digit = [ '0' .. '9' ] |> List.map char' |> List.reduce (<|>)

    let binder x =
        match x with
        | [] -> Error "Parsing failed."
        | _ -> x |> List.map string |> String.concat "" |> int |> Ok

    let expected = Error("Parsing failed.", State("#65a2ac", 0))
    let actual = bind binder (many digit) (State("#65a2ac", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``string' 1`` () =
    let expected = Ok("fs", State("fsharp", 2))
    let actual = string' "fs" (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``string' 2`` () =
    let expected = Ok("fsharp", State("fsharp", 6))
    let actual = string' "fsharp" (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``string' 3`` () =
    let expected = Ok("sharp", State("fsharp", 6))
    let actual = string' "sharp" (State("fsharp", 1))
    Assert.Equal(expected, actual)

[<Fact>]
let ``string' 4`` () =
    let expected = Error("Parsing failed.", State("fsharp", 0))
    let actual = string' "csharp" (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``string' 5`` () =
    let expected = Error("Argument was invalid.", State("fsharp", 0))
    let actual = string' "" (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``string' 6`` () =
    let expected = Error("Input was empty.", State("", 0))
    let actual = string' "fsharp" (State("", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``string' 7`` () =
    let expected = Error("Input was empty.", State("", 0))
    let actual = string' "" (State("", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``regex 1`` () =
    let expected = Ok("fsharp", State("fsharp", 6))
    let actual = regex ".sharp" (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``regex 2`` () =
    let expected = Ok("taidalog", State("I'm taidalog.", 12))
    let actual = regex "^[^\.]+" (State("I'm taidalog.", 4))
    Assert.Equal(expected, actual)

[<Fact>]
let ``regex 3`` () =
    let expected = Ok("#65a2ac", State("color: #65a2ac;", 14))
    let actual = regex "^#[0-9a-f]{6}" (State("color: #65a2ac;", 7))
    Assert.Equal(expected, actual)

[<Fact>]
let ``regex 4`` () =
    let expected = Error("Parsing failed.", State("color: #65a2ac;", 7))
    let actual = regex "^#[0-9A-F]{6}" (State("color: #65a2ac;", 7))
    Assert.Equal(expected, actual)

[<Fact>]
let ``end' 1`` () =
    let expected = Ok((), State("fsharp", 6))
    let actual = end' (State("fsharp", 6))
    Assert.Equal(expected, actual)

[<Fact>]
let ``end' 2`` () =
    let expected = Error("Parsing failed.", State("fsharp", 0))
    let actual = end' (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``end' 3`` () =
    let expected = Error("Position exceeded input length.", State("fsharp", 7))
    let actual = end' (State("fsharp", 7))
    Assert.Equal(expected, actual)

[<Fact>]
let ``pos 1`` () =
    let expected = Ok((), State("fsharp", 0))
    let actual = pos (char' 'f') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``pos 2`` () =
    let expected = Error("Parsing failed.", State("fsharp", 0))
    let actual = pos (char' 'c') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``neg 1`` () =
    let expected = Ok((), State("fsharp", 0))
    let actual = neg (char' 'c') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``neg 2`` () =
    let expected = Error("Parsing failed.", State("fsharp", 0))
    let actual = neg (char' 'f') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``any 1`` () =
    let expected = Ok('f', State("fsharp", 1))
    let actual = any (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``any 2`` () =
    let expected = Error("Position exceeded input length.", State("fsharp", 6))
    let actual = any (State("fsharp", 6))
    Assert.Equal(expected, actual)
