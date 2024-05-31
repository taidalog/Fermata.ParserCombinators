# Fermata.ParserCombinators

F# library for operations related to parser combinators. Compatible with Fable.

Version 0.2.0

## Features

- Provides functions for combinatory parsing for F#.
- Works in a Fable project.

## Target Frameworks

- .NET Standard 2.0
- .NET 6
- .NET 7
- .NET 8

## Modules

- Parsers  
   Contains parser combinatior functions.

For more information, see the signature file (`.fsi`).

## Installation

.NET CLI,

```
dotnet add package Fermata.ParserCombinators --version 0.2.0
```

F# Intaractive,

```
#r "nuget: Fermata.ParserCombinators, 0.2.0"
```

For more information, please see [Fermata.ParserCombinators on NuGet Gallery](https://www.nuget.org/packages/Fermata.ParserCombinators).

## Notes

- Don't forget `npm start` before using Fermata.ParserCombinators in a Fable project. I forgot!

## Known Issue

-

## Release Notes

[Releases on GitHub](https://github.com/taidalog/Fermata.ParserCombinators/releases)

## Breaking Changes

### 0.2.0

- Changed `Error` value returned by `Parser<'T>` to hold error information, instead of an empty string.

## Links

- [Repository on GitHub](https://github.com/taidalog/Fermata.ParserCombinators)
- [NuGet Gallery](https://www.nuget.org/packages/Fermata.ParserCombinators)

## License

This product is licensed under the [MIT license](https://github.com/taidalog/Fermata.ParserCombinators/blob/main/LICENSE).
