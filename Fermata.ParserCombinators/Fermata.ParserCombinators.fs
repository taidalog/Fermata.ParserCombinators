// Fermata.ParserCombinators Version 0.3.0
// https://github.com/taidalog/Fermata.ParserCombinators
// Copyright (c) 2024 taidalog
// This software is licensed under the MIT License.
// https://github.com/taidalog/Fermata.ParserCombinators/blob/main/LICENSE

namespace Fermata.ParserCombinators

module Parsers =
    type State = State of string * int

    type Parser<'T> =
        | Parser of (State -> Result<'T * State, string * State>)

        static member (*)(parser1: Parser<'T>, parser2: Parser<'U>) : Parser<'T * 'U> =
            fun (state: State) ->
                let (Parser p1) = parser1
                let (Parser p2) = parser2

                match p1 state with
                | Error(e1, _) -> Error(e1, state)
                | Ok(v1, state1) ->
                    match p2 state1 with
                    | Error(e2, _) -> Error(e2, state)
                    | Ok(v2, state2) -> Ok((v1, v2), state2)
            |> Parser

        static member (+)(parser1: Parser<'T>, parser2: Parser<'T>) : Parser<'T> =
            let (Parser p1) = parser1
            let (Parser p2) = parser2

            fun (state: State) ->
                match p1 state with
                | Ok x -> Ok x
                | Error _ ->
                    match p2 state with
                    | Ok y -> Ok y
                    | Error e -> Error e
            |> Parser

        static member (*)(parser: Parser<'T>, count: int) : Parser<'T list> =
            let rec foldWhileOk x acc (list: Parser<'T> list) =
                match list with
                | [] -> Ok(List.rev acc, x)
                | h :: t ->
                    let (Parser h') = h

                    match h' x with
                    | Error e -> Error e
                    | Ok(v, x') -> foldWhileOk x' (v :: acc) t

            fun (state: State) ->
                List.replicate count parser
                |> foldWhileOk state []
                |> function
                    | Ok v -> Ok v
                    | Error(e, _) -> Error(e, state)
            |> Parser

        member x.Many() : Parser<'T list> =
            fun (state: State) ->
                let (Parser parser) = x

                let rec inner (acc: 'T list) (s: State) =
                    match parser s with
                    | Error(_, state') -> Ok(List.rev acc, state')
                    | Ok(v, state') -> inner (v :: acc) state'

                inner [] state
            |> Parser

    let exec (p: Parser<'T>) (s: State) : Result<'T * State, string * State> =
        let (Parser p') = p
        p' s

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
        |> Parser

    let (<&>) (parser1: Parser<'T>) (parser2: Parser<'U>) : Parser<'T * 'U> = parser1 * parser2

    let (<+&>) (parser1: Parser<'T>) (parser2: Parser<'U>) : Parser<'T> =
        fun (state: State) ->
            let (Parser p1) = parser1
            let (Parser p2) = parser2

            match p1 state with
            | Error(e1, _) -> Error(e1, state)
            | Ok(v1, state1) ->
                match p2 state1 with
                | Error(e2, _) -> Error(e2, state)
                | Ok(_, state2) -> Ok(v1, state2)
        |> Parser

    let (<&+>) (parser1: Parser<'T>) (parser2: Parser<'U>) : Parser<'U> =
        fun (state: State) ->
            let (Parser p1) = parser1
            let (Parser p2) = parser2

            match p1 state with
            | Error(e1, _) -> Error(e1, state)
            | Ok(_, state1) ->
                match p2 state1 with
                | Error(e2, _) -> Error(e2, state)
                | Ok(v2, state2) -> Ok(v2, state2)
        |> Parser

    let (<|>) (parser1: Parser<'T>) (parser2: Parser<'T>) : Parser<'T> = parser1 + parser2

    let many (parser: Parser<'T>) : Parser<'T list> = parser.Many()

    let repeat (count: int) (parser: Parser<'T>) : Parser<'T list> = parser * count

    let map' (mapping: 'T -> 'U) (parser: Parser<'T>) : Parser<'U> =
        fun (state: State) ->
            let (Parser p) = parser

            match p state with
            | Ok(v, state') -> Ok(mapping v, state')
            | Error e -> Error e
        |> Parser

    let bind (binder: 'T -> Result<'U, string>) (parser: Parser<'T>) : Parser<'U> =
        fun (state: State) ->
            let (Parser p) = parser

            match p state with
            | Error e -> Error e
            | Ok(v, state') ->
                match binder v with
                | Ok v' -> Ok(v', state')
                | Error e' -> Error(e', state)
        |> Parser

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
        |> Parser

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
        |> Parser

    let end': Parser<unit> =
        fun (State(x, p)) ->
            let len = String.length x

            if p > len then Error(errorsExceeded, State(x, p))
            else if p = len then Ok((), State(x, p))
            else Error("Parsing failed.", State(x, p))
        |> Parser

    let pos (parser: Parser<'T>) : Parser<unit> =
        fun state ->
            let (Parser p) = parser

            match p state with
            | Ok _ -> Ok((), state)
            | Error _ -> Error("Parsing failed.", state)
        |> Parser

    let neg (parser: Parser<'T>) : Parser<unit> =
        fun state ->
            let (Parser p) = parser

            match p state with
            | Ok _ -> Error("Parsing failed.", state)
            | Error _ -> Ok((), state)
        |> Parser

    let any: Parser<char> =
        fun (State(x, p)) ->
            let len = String.length x

            if len = 0 then Error(errorsEmpty, State(x, p))
            else if p >= len then Error(errorsExceeded, State(x, p))
            else Ok(x.[p], State(x, p + 1))
        |> Parser
