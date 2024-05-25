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
