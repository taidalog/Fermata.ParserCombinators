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
