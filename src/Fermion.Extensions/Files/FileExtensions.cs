using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace Fermion.Extensions.Files;

/// <summary>
/// Provides extension methods for file operations.
/// </summary>
public static class FileExtensions
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    #region Helper

    /// <summary>
    /// Gets the content type of a file based on its extension.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The content type of the file.</returns>
    /// <remarks>
    /// This method uses the <see cref="FileExtensionContentTypeProvider"/> to determine the content type based on the file extension.
    /// If the content type cannot be determined, it defaults to "application/octet-stream".
    /// </remarks>
    /// <example>
    /// <code>
    /// var contentType = FileExtensions.GetContentType("example.txt");
    /// Console.WriteLine(contentType); // Output: text/plain
    /// </code>
    /// </example>
    public static string GetContentType(string fileName)
    {
        if (!ContentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }

    /// <summary>
    /// Reads the contents of a file asynchronously and returns it as a string.
    /// </summary>
    /// <param name="filePath">The path to the file to read.</param>
    /// <returns>A task that represents the asynchronous read operation, containing the file contents as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    /// <remarks>
    /// This method uses a <see cref="StreamReader"/> to read the file contents asynchronously.
    /// </remarks>
    /// <example>
    /// <code>
    /// string content = await FileExtensions.ReadAsStringAsync("example.txt");
    /// Console.WriteLine(content);
    /// </code>
    /// </example>
    public static async Task<string> ReadAsStringAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        using var reader = new StreamReader(filePath);

        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Reads the contents of a file asynchronously and returns it as a byte array.
    /// </summary>
    /// <param name="filePath">The path to the file to read.</param>
    /// <returns>A task that represents the asynchronous read operation, containing the file contents as a byte array.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    /// <remarks>
    /// This method uses <see cref="File.ReadAllBytesAsync"/> to read the file contents asynchronously.
    /// </remarks>
    /// <example>
    /// <code>
    /// byte[] bytes = await FileExtensions.ReadAsBytesAsync("example.jpg");
    /// // Process the byte array
    /// </code>
    /// </example>
    public static async Task<byte[]> ReadAsBytesAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        return await File.ReadAllBytesAsync(filePath);
    }

    /// <summary>
    /// Calculates the MD5 hash of a file.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The MD5 hash of the file as a lowercase hexadecimal string without dashes.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    /// <remarks>
    /// This method uses the <see cref="System.Security.Cryptography.MD5"/> algorithm to calculate the hash.
    /// The hash is returned as a lowercase hexadecimal string without dashes.
    /// </remarks>
    /// <example>
    /// <code>
    /// string hash = FileExtensions.CalculateFileHash("example.txt");
    /// Console.WriteLine(hash); // Output: a hash like "d41d8cd98f00b204e9800998ecf8427e"
    /// </code>
    /// </example>
    public static string CalculateFileHash(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        using var md5 = System.Security.Cryptography.MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Compresses a file using GZip compression.
    /// </summary>
    /// <param name="sourceFilePath">The path to the source file to compress.</param>
    /// <param name="destinationFilePath">The path where the compressed file will be saved.</param>
    /// <returns>A task that represents the asynchronous compression operation.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
    /// <remarks>
    /// This method uses <see cref="System.IO.Compression.GZipStream"/> to compress the file.
    /// </remarks>
    /// <example>
    /// <code>
    /// await FileExtensions.CompressFileAsync("example.txt", "example.txt.gz");
    /// </code>
    /// </example>
    public static async Task CompressFileAsync(string sourceFilePath, string destinationFilePath)
    {
        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException("Source file not found", sourceFilePath);
        }

        await using var sourceStream = new FileStream(sourceFilePath, FileMode.Open);
        await using var destinationStream = File.Create(destinationFilePath);
        await using var compressStream = new System.IO.Compression.GZipStream(destinationStream, System.IO.Compression.CompressionMode.Compress);

        await sourceStream.CopyToAsync(compressStream);
    }

    /// <summary>
    /// Decompresses a GZip compressed file.
    /// </summary>
    /// <param name="compressedFilePath">The path to the compressed file.</param>
    /// <param name="destinationFilePath">The path where the decompressed file will be saved.</param>
    /// <returns>A task that represents the asynchronous decompression operation.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the compressed file does not exist.</exception>
    /// <remarks>
    /// This method uses <see cref="System.IO.Compression.GZipStream"/> to decompress the file.
    /// </remarks>
    /// <example>
    /// <code>
    /// await FileExtensions.DecompressFileAsync("example.txt.gz", "example_decompressed.txt");
    /// </code>
    /// </example>
    public static async Task DecompressFileAsync(string compressedFilePath, string destinationFilePath)
    {
        if (!File.Exists(compressedFilePath))
        {
            throw new FileNotFoundException("Compressed file not found", compressedFilePath);
        }

        await using var sourceStream = new FileStream(compressedFilePath, FileMode.Open);
        await using var decompressStream = new System.IO.Compression.GZipStream(sourceStream, System.IO.Compression.CompressionMode.Decompress);
        await using var destinationStream = File.Create(destinationFilePath);

        await decompressStream.CopyToAsync(destinationStream);
    }

    /// <summary>
    /// Reads a CSV file asynchronously and returns its contents as a list of string arrays.
    /// </summary>
    /// <param name="filePath">The path to the CSV file.</param>
    /// <param name="delimiter">The delimiter character used in the CSV file. Defaults to comma (',').</param>
    /// <returns>A task that represents the asynchronous read operation, containing a list of string arrays where each array contains the values of a row.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
    /// <remarks>
    /// Each string array in the returned list represents a row in the CSV file, with each element representing a field.
    /// </remarks>
    /// <example>
    /// <code>
    /// List&lt;string[]&gt; csvData = await FileExtensions.ReadCsvFileAsync("data.csv");
    /// foreach (var row in csvData)
    /// {
    ///     Console.WriteLine(string.Join(", ", row));
    /// }
    /// </code>
    /// </example>
    public static async Task<List<string[]>> ReadCsvFileAsync(string filePath, char delimiter = ',')
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("CSV file not found", filePath);
        }

        var lines = await File.ReadAllLinesAsync(filePath);

        return lines.Select(line => line.Split(delimiter)).ToList();
    }

    /// <summary>
    /// Writes data to a CSV file asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of objects in the data collection.</typeparam>
    /// <param name="filePath">The path where the CSV file will be saved.</param>
    /// <param name="data">The collection of data objects to write to the CSV file.</param>
    /// <param name="converter">A function that converts each data object to an array of strings representing the fields.</param>
    /// <param name="delimiter">The delimiter character to use in the CSV file. Defaults to comma (',').</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <remarks>
    /// This method writes each object in the data collection as a row in the CSV file.
    /// The converter function is used to convert each object to an array of strings representing the fields.
    /// </remarks>
    /// <example>
    /// <code>
    /// var people = new List&lt;Person&gt;
    /// {
    ///     new Person { Name = "John", Age = 30 },
    ///     new Person { Name = "Jane", Age = 25 }
    /// };
    /// 
    /// await FileExtensions.WriteToCsvFileAsync("people.csv", people, 
    ///     person => new[] { person.Name, person.Age.ToString() });
    /// </code>
    /// </example>
    public static async Task WriteToCsvFileAsync<T>(string filePath, IEnumerable<T> data, Func<T, string[]> converter, char delimiter = ',')
    {
        await using var writer = new StreamWriter(filePath, false);
        foreach (var item in data)
        {
            var values = converter(item);
            var line = string.Join(delimiter, values);
            await writer.WriteLineAsync(line);
        }
    }

    #endregion

    #region Extensions

    /// <summary>
    /// Converts an IFormFile to a byte array asynchronously.
    /// </summary>
    /// <param name="file">The IFormFile to convert.</param>
    /// <returns>A task that represents the asynchronous conversion operation, containing the file contents as a byte array.</returns>
    /// <remarks>
    /// This method copies the file contents to a memory stream and then converts it to a byte array.
    /// If the file is empty, an empty array is returned.
    /// </remarks>
    /// <example>
    /// <code>
    /// // In a controller action
    /// public async Task&lt;IActionResult&gt; Upload(IFormFile file)
    /// {
    ///     byte[] fileBytes = await file.ToByteArrayAsync();
    ///     // Process the byte array
    /// }
    /// </code>
    /// </example>
    public static async Task<byte[]> ToByteArrayAsync(this IFormFile file)
    {
        if (file.Length == 0)
        {
            return [];
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    /// <summary>
    /// Checks if the file size is within the specified limit.
    /// </summary>
    /// <param name="file">The IFormFile to check.</param>
    /// <param name="maxSizeInMb">The maximum allowed file size in megabytes.</param>
    /// <returns>True if the file is not empty and its size is within the specified limit; otherwise, false.</returns>
    /// <remarks>
    /// This method calculates the maximum size in bytes by multiplying the specified size in megabytes by 1024 * 1024.
    /// </remarks>
    /// <example>
    /// <code>
    /// // In a controller action
    /// public IActionResult Upload(IFormFile file)
    /// {
    ///     if (!file.CheckFileSize(5)) // 5 MB limit
    ///     {
    ///         return BadRequest("File size exceeds the limit.");
    ///     }
    ///     // Process the file
    /// }
    /// </code>
    /// </example>

    public static bool CheckFileSize(this IFormFile file, int maxSizeInMb)
    {
        return file.Length > 0 && file.Length <= maxSizeInMb * 1024 * 1024;
    }

    /// <summary>
    /// Checks if the file extension is in the list of allowed extensions.
    /// </summary>
    /// <param name="file">The IFormFile to check.</param>
    /// <param name="allowedExtensions">A list of allowed file extensions (e.g., [".jpg", ".png"]).</param>
    /// <returns>True if the file has a valid extension that is in the allowed list; otherwise, false.</returns>
    /// <remarks>
    /// The method converts the file extension to lowercase before comparing it with the allowed extensions.
    /// </remarks>
    /// <example>
    /// <code>
    /// // In a controller action
    /// public IActionResult Upload(IFormFile file)
    /// {
    ///     var allowedExtensions = new List&lt;string&gt; { ".jpg", ".png", ".pdf" };
    ///     if (!file.HasValidExtension(allowedExtensions))
    ///     {
    ///         return BadRequest("File type not allowed.");
    ///     }
    ///     // Process the file
    /// }
    /// </code>
    /// </example>
    public static bool HasValidExtension(this IFormFile file, List<string> allowedExtensions)
    {
        if (string.IsNullOrEmpty(file.FileName))
        {
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return !string.IsNullOrEmpty(extension) && allowedExtensions.Contains(extension);
    }

    /// <summary>
    /// Saves an IFormFile to the local file system asynchronously.
    /// </summary>
    /// <param name="file">The IFormFile to save.</param>
    /// <param name="folderPath">The folder path where the file will be saved.</param>
    /// <param name="fileName">Optional. The name to save the file as. If not provided, a new GUID will be used with the original file extension.</param>
    /// <returns>A task that represents the asynchronous save operation, containing the file name that was used to save the file.</returns>
    /// <remarks>
    /// If the specified folder does not exist, it will be created.
    /// If no file name is provided, a new GUID will be used with the original file extension.
    /// If the file is empty, an empty string is returned.
    /// </remarks>
    /// <example>
    /// <code>
    /// // In a controller action
    /// public async Task&lt;IActionResult&gt; Upload(IFormFile file)
    /// {
    ///     var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    ///     var savedFileName = await file.SaveToLocalAsync(folderPath);
    ///     // Process the saved file
    /// }
    /// </code>
    /// </example>
    public static async Task<string> SaveToLocalAsync(this IFormFile file, string folderPath, string? fileName = null)
    {
        if (file.Length == 0)
            return string.Empty;

        if (string.IsNullOrEmpty(fileName))
        {
            fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var filePath = Path.Combine(folderPath, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }

    #endregion
}