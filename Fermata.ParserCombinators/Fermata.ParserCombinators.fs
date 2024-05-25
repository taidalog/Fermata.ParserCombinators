namespace Fermata.ParserCombinators

module Parsers =
    type State = State of string * int
    type Parser<'T> = State -> Result<'T * State, string * State>

    let char' (c: char) (State(x, p)) : Result<char * State, string * State> =
        let len = String.length x

        if len = 0 then Error("", State(x, p))
        else if p >= len then Error("", State(x, p))
        else if x.[p] = c then Ok(c, State(x, p + 1))
        else Error("", State(x, p))

    let (<&>) (p1: Parser<'T>) (p2: Parser<'U>) : Parser<'T * 'U> =
        fun (state: State) ->
            match p1 state with
            | Error(e1, _) -> Error(e1, state)
            | Ok(v1, state1) ->
                match p2 state1 with
                | Error(e2, _) -> Error(e2, state)
                | Ok(v2, state2) -> Ok((v1, v2), state2)

    let (<+&>) (p1: Parser<'T>) (p2: Parser<'U>) : Parser<'T> =
        fun (state: State) ->
            match p1 state with
            | Error(e1, _) -> Error(e1, state)
            | Ok(v1, state1) ->
                match p2 state1 with
                | Error(e2, _) -> Error(e2, state)
                | Ok(_, state2) -> Ok(v1, state2)

    let (<&+>) (p1: Parser<'T>) (p2: Parser<'U>) : Parser<'U> =
        fun (state: State) ->
            match p1 state with
            | Error(e1, _) -> Error(e1, state)
            | Ok(_, state1) ->
                match p2 state1 with
                | Error(e2, _) -> Error(e2, state)
                | Ok(v2, state2) -> Ok(v2, state2)

    let (<|>) (p1: Parser<'T>) (p2: Parser<'T>) : Parser<'T> =
        fun (state: State) ->
            match p1 state with
            | Ok x -> Ok x
            | Error _ ->
                match p2 state with
                | Ok y -> Ok y
                | Error e -> Error e

    let many (p: Parser<'T>) (state: State) : Result<'T list * State, string * State> =
        let rec inner (acc: 'T list) (s: State) =
            match p s with
            | Error(_, state') -> Ok(List.rev acc, state')
            | Ok(v, state') -> inner (v :: acc) state'

        inner [] state

    let rec foldWhileOk x acc list =
        match list with
        | [] -> Ok(List.rev acc, x)
        | h :: t ->
            match h x with
            | Error e -> Error e
            | Ok(v, x') -> foldWhileOk x' (v :: acc) t

    let repN (n: int) (parser: Parser<'T>) (state: State) : Result<'T list * State, string * State> =
        List.replicate n parser
        |> foldWhileOk state []
        |> function
            | Ok x -> Ok x
            | Error(e, s) -> Error(e, state)

    let map' (mapping: 'T -> 'U) (parser: Parser<'T>) (state: State) : Result<'U * State, string * State> =
        match parser state with
        | Ok(x, state') -> Ok(mapping x, state')
        | Error e -> Error e

    let bind
        (binder: 'T -> Result<'U, string>)
        (parser: Parser<'T>)
        (state: State)
        : Result<'U * State, string * State> =
        match parser state with
        | Error e -> Error e
        | Ok(x, state') ->
            match binder x with
            | Ok x' -> Ok(x', state')
            | Error e' -> Error(e', state)

    let string' (s: string) (State(x, p)) : Result<string * State, string * State> =
        let len = String.length s

        if len = 0 then
            Error("", State(x, p))
        else if p >= len then
            Error("", State(x, p))
        else if x.[p .. p + len - 1] = s then
            Ok(s, State(x, p + len))
        else
            Error("", State(x, p))
