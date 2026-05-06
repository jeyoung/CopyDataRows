# CopyDataRows

A small .NET 6.0 program to copy data rows from a source CSV file to a
destination CSV file, skipping header rows identified by a regex pattern.

## Usage

```
dotnet run <source path> <destination path> <header pattern> <header length>
```

## Parameters

| Parameter          | Description                                                        |
| ------------------ | ------------------------------------------------------------------ |
| `source path`      | Path to the source CSV file                                        |
| `destination path` | Path to the destination CSV file                                   |
| `header pattern`   | Regex pattern used to locate the header row in the source file     |
| `header length`    | Buffer size (in bytes) used when reading the source file           |

## How It Works

1. Reads the source file in chunks of `header length` bytes.
2. Accumulates chunks until the regex `header pattern` matches a header row.
3. From the header row onward, writes all remaining data to the destination
   file.

## Example

```
dotnet run data/source.csv data/destination.csv "^Name,Age,Email" 4096
```
