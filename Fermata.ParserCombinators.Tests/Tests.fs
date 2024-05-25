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
