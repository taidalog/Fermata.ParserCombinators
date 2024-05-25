namespace Fermata.ParserCombinators

module Parsers =

    type State = State of string * int
    val char': c: char -> State -> Result<char * State, string * State>
