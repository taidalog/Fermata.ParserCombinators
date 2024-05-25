namespace Fermata.ParserCombinators

module Parsers =

    type State = State of string * int
    val char': c: char -> State -> Result<char * State, string * State>

    val (<&>):
        p1: (State -> Result<'T * State, string * State>) ->
        p2: (State -> Result<'U * State, string * State>) ->
            (State -> Result<('T * 'U) * State, string * State>)

    val (<|>):
        p1: (State -> Result<'T * State, string * State>) ->
        p2: (State -> Result<'T * State, string * State>) ->
            (State -> Result<'T * State, string * State>)

    val many: p: (State -> Result<'T * State, string * State>) -> State -> Result<'T list * State, string * State>

    val repN:
        n: int ->
        parser: (State -> Result<'T * State, string * State>) ->
        state: State ->
            Result<'T list * State, string * State>

    val map':
        f: ('T -> 'U) ->
        parser: (State -> Result<'T * State, string * State>) ->
        state: State ->
            Result<'U * State, string * State>
