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
