# AGENTS.md

## Project Overview
- Single-file .NET 6.0 console application
- Copies data rows from a source CSV to a destination CSV, skipping header rows identified by regex pattern
- Entry point: `Program.cs`

## Build & Run
```bash
dotnet run <source path> <destination path> <header pattern> <header length>
```

## Code Conventions

### Language & Framework
- C# with .NET 6.0
- `ImplicitUsings` enabled
- `Nullable` enabled

### Structure
- Single `Program` class, no namespace
- All logic in `Program.cs`
- Instance methods for business logic, `static Main` for entry point
- Instantiate `Program` and call instance methods from `Main`

### Naming
- Methods: PascalCase (`CopyDataRows`, `Main`)
- Variables/parameters: camelCase (`sourcePath`, `bufferSize`)
- Constants: `const` with camelCase (`usage`)

### Style
- Tabs for indentation
- `var` for implicit typing
- Tuple deconstruction for multiple assignments: `var (a, b, c) = ...`
- Lambda expressions for inline delegates
- `using` statements with parentheses (not using declarations)

### Async Pattern
- Use `async Task` for async methods
- Use `await` with `ReadAsync`/`WriteAsync` on streams

### CLI Patterns
- Validate `args.Length` upfront
- Print usage string on invalid input
- Error messages prefixed with `ERROR:`
- Return early on validation failure
- Use `int.TryParse` for numeric input validation

### File I/O
- Use `FileStream` with explicit `FileMode`
- Use `Memory<byte>` for buffer operations
- Use `Encoding.Default` for text encoding
- Use `Regex` with `RegexOptions.Multiline` for pattern matching

## Commit Message Style
- Summary: `area: description` format (max 80 chars)
- Detailed messages: wrap at 80 chars
