// Fermata.ParserCombinators Version 0.1.0
// https://github.com/taidalog/Fermata.ParserCombinators
// Copyright (c) 2024 taidalog
// This software is licensed under the MIT License.
// https://github.com/taidalog/Fermata.ParserCombinators/blob/main/LICENSE

namespace Fermata.ParserCombinators

module Parsers =
    type State = State of string * int
    type Parser<'T> = State -> Result<'T * State, string * State>

    let errorsEmpty = "Input was empty."
    let errorsExceeded = "Position exceeded input length."
    let errorsFailed = "Parsing failed."
    let errorsInvalid = "Argument was invalid."

    let char' (c: char) : Parser<char> =
        fun (State(x, p)) ->
            let len = String.length x

            if len = 0 then Error(errorsEmpty, State(x, p))
            else if p >= len then Error(errorsExceeded, State(x, p))
            else if x.[p] = c then Ok(c, State(x, p + 1))
            else Error(errorsFailed, State(x, p))

    let (<&>) (parser1: Parser<'T>) (parser2: Parser<'U>) : Parser<'T * 'U> =
        fun (state: State) ->
            match parser1 state with
            | Error(e1, _) -> Error(e1, state)
            | Ok(v1, state1) ->
                match parser2 state1 with
                | Error(e2, _) -> Error(e2, state)
                | Ok(v2, state2) -> Ok((v1, v2), state2)

    let (<+&>) (parser1: Parser<'T>) (parser2: Parser<'U>) : Parser<'T> =
        fun (state: State) ->
            match parser1 state with
            | Error(e1, _) -> Error(e1, state)
            | Ok(v1, state1) ->
                match parser2 state1 with
                | Error(e2, _) -> Error(e2, state)
                | Ok(_, state2) -> Ok(v1, state2)

    let (<&+>) (parser1: Parser<'T>) (parser2: Parser<'U>) : Parser<'U> =
        fun (state: State) ->
            match parser1 state with
            | Error(e1, _) -> Error(e1, state)
            | Ok(_, state1) ->
                match parser2 state1 with
                | Error(e2, _) -> Error(e2, state)
                | Ok(v2, state2) -> Ok(v2, state2)

    let (<|>) (parser1: Parser<'T>) (parser2: Parser<'T>) : Parser<'T> =
        fun (state: State) ->
            match parser1 state with
            | Ok x -> Ok x
            | Error _ ->
                match parser2 state with
                | Ok y -> Ok y
                | Error e -> Error e

    let many (parser: Parser<'T>) : Parser<'T list> =
        fun (state: State) ->
            let rec inner (acc: 'T list) (s: State) =
                match parser s with
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

    let repeat (count: int) (parser: Parser<'T>) : Parser<'T list> =
        fun (state: State) ->
            List.replicate count parser
            |> foldWhileOk state []
            |> function
                | Ok v -> Ok v
                | Error(e, _) -> Error(e, state)

    let map' (mapping: 'T -> 'U) (parser: Parser<'T>) : Parser<'U> =
        fun (state: State) ->
            match parser state with
            | Ok(v, state') -> Ok(mapping v, state')
            | Error e -> Error e

    let bind (binder: 'T -> Result<'U, string>) (parser: Parser<'T>) : Parser<'U> =
        fun (state: State) ->
            match parser state with
            | Error e -> Error e
            | Ok(v, state') ->
                match binder v with
                | Ok v' -> Ok(v', state')
                | Error e' -> Error(e', state)

    let string' (s: string) : Parser<string> =
        fun (State(x, p)) ->
            let xLen = String.length x
            let sLen = String.length s

            if xLen = 0 then
                Error(errorsEmpty, State(x, p))
            else if p >= xLen then
                Error(errorsExceeded, State(x, p))
            else if sLen = 0 then
                Error(errorsInvalid, State(x, p))
            else if x.[p .. p + sLen - 1] = s then
                Ok(s, State(x, p + sLen))
            else
                Error("Parsing failed.", State(x, p))

    let regex (pattern: string) : Parser<string> =
        fun (State(x, p)) ->
            let len = String.length x

            if len = 0 then
                Error(errorsEmpty, State(x, p))
            else if p >= len then
                Error(errorsExceeded, State(x, p))
            else
                let m = System.Text.RegularExpressions.Regex.Match(x.[p..], pattern)

                if m.Success then
                    Ok(m.Value, State(x, p + m.Length))
                else
                    Error("Parsing failed.", State(x, p))

    let end': Parser<unit> =
        fun (State(x, p)) ->
            let len = String.length x

            if p > len then Error(errorsExceeded, State(x, p))
            else if p = len then Ok((), State(x, p))
            else Error("Parsing failed.", State(x, p))

    let pos (parser: Parser<'T>) : Parser<unit> =
        fun (State(x, p)) ->
            let len = String.length x

            if len = 0 then
                Error(errorsEmpty, State(x, p))
            else if p >= len then
                Error(errorsExceeded, State(x, p))
            else
                match parser (State(x, p)) with
                | Ok _ -> Ok((), State(x, p))
                | Error _ -> Error("Parsing failed.", State(x, p))

    let neg (parser: Parser<'T>) : Parser<unit> =
        fun (State(x, p)) ->
            let len = String.length x

            if len = 0 then
                Error(errorsEmpty, State(x, p))
            else if p >= len then
                Error(errorsExceeded, State(x, p))
            else
                match parser (State(x, p)) with
                | Ok _ -> Error("Parsing failed.", State(x, p))
                | Error _ -> Ok((), State(x, p))

    let any: Parser<char> =
        fun (State(x, p)) ->
            let len = String.length x

            if len = 0 then Error(errorsEmpty, State(x, p))
            else if p >= len then Error(errorsExceeded, State(x, p))
            else Ok(x.[p], State(x, p + 1))
