using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

class Program
{
    async Task CopyDataRows(string sourcePath, string destinationPath, Func<ReadOnlyMemory<byte>, int> headerRowLocator, int bufferSize)
    {
	try
	{
	    using (var source = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
	    using (var destination = new FileStream(destinationPath, FileMode.Create))
	    {
		var buffer = new Memory<byte>(new byte[bufferSize]);

		for (int inputLength = 0, index = -1; (inputLength = await source.ReadAsync(buffer)) > 0;)
		{
		    if (index > -1)
		    {
			await destination.WriteAsync(buffer.Slice(0, inputLength));
		    }
		    else if ((index = headerRowLocator(buffer)) > -1)
		    {
			await destination.WriteAsync(buffer.Slice(index, inputLength-index));
		    }
		}
	    }
	}
	catch (Exception ex)
	{
	    Console.Error.WriteLine($"ERROR: {ex.Message}");
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

	int bufferSize;

	if (int.TryParse(args[3], out bufferSize) == false)
	{
	    Console.WriteLine($"ERROR: The value {args[3]} is not a number");
	    Console.WriteLine(usage);
	    Console.WriteLine("<header length> must be a whole number");
	    return 1;
	}

	var regex = new Regex(headerRowPattern, RegexOptions.Compiled | RegexOptions.Multiline);

	var program = new Program();

	await program.CopyDataRows(sourcePath, destinationPath, buffer =>
	{
	    var matches = regex.Matches(Encoding.Default.GetString(buffer.Span));
	    return matches.Count > 0 ? matches[0].Index : -1;
	}, bufferSize);

	return 0;
    }
}
