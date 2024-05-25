namespace Fermata.ParserCombinators

module Parsers =
    type State = State of string * int

    let char' (c: char) (State(x, p)) : Result<char * State, string * State> =
        match x with
        | "" -> Error("", State(x, p))
        | _ ->
            if x.[p] = c then
                Ok(c, State(x, p + 1))
            else
                Error("", State(x, p))

    let (<&>)
        (p1: State -> Result<'T * State, string * State>)
        (p2: State -> Result<'U * State, string * State>)
        : (State -> Result<('T * 'U) * State, string * State>) =
        fun (state: State) ->
            match p1 state with
            | Error(e1, _) -> Error(e1, state)
            | Ok(v1, state1) ->
                match p2 state1 with
                | Error(e2, _) -> Error(e2, state)
                | Ok(v2, state2) -> Ok((v1, v2), state2)
    let (<|>)
        (p1: State -> Result<'T * State, string * State>)
        (p2: State -> Result<'T * State, string * State>)
        : (State -> Result<'T * State, string * State>) =
        fun (state: State) ->
            match p1 state with
            | Ok x -> Ok x
            | Error _ ->
                match p2 state with
                | Ok y -> Ok y
                | Error e -> Error e