using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static async Task CopyDataRows(string sourcePath, string destinationPath, Func<string, int> headerRowLocator, int bufferSize)
    {
	using var source = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
	using var destination = new FileStream(destinationPath, FileMode.Create);

	var buffer = new Memory<byte>(new byte[bufferSize]);
	var accumulated = new byte[bufferSize];
	var encoder = Encoding.UTF8;
	int accumulatedLength = 0;
	bool headerFound = false;

	for (int inputLength; (inputLength = await source.ReadAsync(buffer)) > 0;)
	{
	    if (headerFound)
	    {
		await destination.WriteAsync(buffer[..inputLength]);
	    }
	    else
	    {
		if (accumulatedLength + inputLength > accumulated.Length)
		{
		    var resized = new byte[accumulated.Length * 2];
		    Buffer.BlockCopy(accumulated, 0, resized, 0, accumulatedLength);
		    accumulated = resized;
		}

		buffer[..inputLength].Span.CopyTo(accumulated.AsSpan(accumulatedLength));
		accumulatedLength += inputLength;

		var text = encoder.GetString(accumulated, 0, accumulatedLength);
		var index = headerRowLocator(text);

		if (index > -1)
		{
		    headerFound = true;
		    var remaining = encoder.GetBytes(text[index..]);
		    await destination.WriteAsync(remaining);
		}
	    }
	}
    }

    public static async Task<int> Main(string[] args)
    {
	const string usage = "Usage: dotnet run <source path> <destination path> <header pattern> <header length>";

	if (args.Length != 4)
	{
	    Console.WriteLine(usage);
	    return 1;
	}

	var (sourcePath, destinationPath, headerRowPattern) = (args[0], args[1], args[2]);

	if (!int.TryParse(args[3], out var bufferSize))
	{
	    Console.WriteLine($"ERROR: The value {args[3]} is not a number");
	    Console.WriteLine(usage);
	    Console.WriteLine("<header length> must be a whole number");
	    return 1;
	}

	var regex = new Regex(headerRowPattern, RegexOptions.Multiline);

	try
	{
	    await CopyDataRows(sourcePath, destinationPath, text =>
	    {
		var matches = regex.Matches(text);
		return matches.Count > 0 ? matches[0].Index : -1;
	    }, bufferSize);
	}
	catch (Exception ex)
	{
	    Console.Error.WriteLine($"ERROR: {ex.Message}");
	    return 1;
	}

	return 0;
    }
}
