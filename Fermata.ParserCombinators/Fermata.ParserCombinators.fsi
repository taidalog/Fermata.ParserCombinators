namespace Fermata.ParserCombinators

module Parsers =

    type State = State of string * int
    type Parser<'T> = State -> Result<'T * State, string * State>
    val char': c: char -> State -> Result<char * State, string * State>
    val (<&>): p1: Parser<'T> -> p2: Parser<'U> -> Parser<'T * 'U>
    val (<+&>): p1: Parser<'T> -> p2: Parser<'U> -> Parser<'T>
    val (<&+>): p1: Parser<'T> -> p2: Parser<'U> -> Parser<'U>
    val (<|>): p1: Parser<'T> -> p2: Parser<'T> -> Parser<'T>
    val many: p: Parser<'T> -> State -> Result<'T list * State, string * State>
    val repN: n: int -> parser: Parser<'T> -> state: State -> Result<'T list * State, string * State>
    val map': mapping: ('T -> 'U) -> parser: Parser<'T> -> state: State -> Result<'U * State, string * State>

    val bind:
        binder: ('T -> Result<'U, string>) -> parser: Parser<'T> -> state: State -> Result<'U * State, string * State>

    val string': s: string -> State -> Result<string * State, string * State>
