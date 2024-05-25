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
    let expected = Error("", State("fsharp", 0))
    let actual = char' 'c' (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``char' 4`` () =
    let expected = Error("", State("fsharp", 6))
    let actual = char' 'x' (State("fsharp", 6))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&> 1`` () =
    let expected = Ok(('f', 's'), State("fsharp", 2))
    let actual = (char' 'f' <&> char' 's') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&> 2`` () =
    let expected = Error("", State("fsharp", 0))
    let actual = (char' 'f' <&> char' '#') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&> 3`` () =
    let expected = Ok((('f', 's'), 'h'), State("fsharp", 3))
    let actual = (char' 'f' <&> char' 's' <&> char' 'h') (State("fsharp", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``<&> 4`` () =
    let expected = Error("", State("fsharp", 0))
    let actual = (char' 'f' <&> char' 's' <&> char' 's') (State("fsharp", 0))
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
    let expected = Error("", State("sharp", 0))
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
let ``repN 1`` () =
    let expected = Ok([ 'w'; 'w'; 'w' ], State("www.~.com", 3))
    let actual = repN 3 (char' 'w') (State("www.~.com", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``repN 2`` () =
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
    let actual = repN 6 hex (State("#65a2ac", 1))
    Assert.Equal(expected, actual)

[<Fact>]
let ``repN 3`` () =
    let hex = [ '0' .. '9' ] @ [ 'a' .. 'f' ] |> List.map char' |> List.reduce (<|>)
    let expected = Error("", State("#65a2ac", 0))
    let actual = repN 6 hex (State("#65a2ac", 0))
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
    let actual = map' g (char' '#' <&> (repN 6 hex)) (State("#65a2ac", 0))
    Assert.Equal(expected, actual)

[<Fact>]
let ``map' 4`` () =
    let hex = [ '0' .. '9' ] @ [ 'a' .. 'f' ] |> List.map char' |> List.reduce (<|>)
    let f = List.map string >> String.concat "" >> int
    let expected = Error("", State("#65a2ac", 0))
    let actual = map' f (repN 6 hex) (State("#65a2ac", 0))
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
        | [] -> Error ""
        | _ -> x |> List.map string |> String.concat "" |> int |> Ok

    let expected = Error("", State("#65a2ac", 0))
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
    let expected = Error("", State("fsharp", 0))
    let actual = string' "csharp" (State("fsharp", 0))
    Assert.Equal(expected, actual)
