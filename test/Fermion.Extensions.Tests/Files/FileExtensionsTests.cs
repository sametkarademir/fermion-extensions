using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Fermion.Extensions.Files;

public class FileExtensionsTests
{
    private readonly string _testDirectory;
    private readonly string _testFile;
    private readonly string _csvTestFile;
    private readonly string _compressedFile;

    public FileExtensionsTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "FileExtensionsTests");
        _testFile = Path.Combine(_testDirectory, "test.txt");
        _csvTestFile = Path.Combine(_testDirectory, "test.csv");
        _compressedFile = Path.Combine(_testDirectory, "test.txt.gz");

        CleanupTestFiles();

        Directory.CreateDirectory(_testDirectory);

        File.WriteAllText(_testFile, "Test file content");

        File.WriteAllLines(_csvTestFile, [
            "Name,Age,Email",
            "John Doe,30,john@example.com",
            "Jane Smith,25,jane@example.com"
        ]);
    }

    private void CleanupTestFiles()
    {
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, true);
            }
            catch
            {
                // Ignore exceptions during cleanup
            }
        }
    }

    [Fact]
    public void GetContentType_ShouldReturnTextPlain_ForTextFile()
    {
        // Arrange
        const string fileName = "test.txt";

        // Act
        var contentType = FileExtensions.GetContentType(fileName);

        // Assert
        Assert.Equal("text/plain", contentType);
    }

    [Fact]
    public void GetContentType_ShouldReturnApplicationOctetStream_ForUnknownExtension()
    {
        // Arrange
        const string fileName = "test.unknown";

        // Act
        var contentType = FileExtensions.GetContentType(fileName);

        // Assert
        Assert.Equal("application/octet-stream", contentType);
    }

    [Fact]
    public async Task ReadAsStringAsync_ShouldReturnFileContent_WhenFileExists()
    {
        // Arrange
        const string expectedContent = "Test file content";

        // Act
        var content = await FileExtensions.ReadAsStringAsync(_testFile);

        // Assert
        Assert.Equal(expectedContent, content);
    }

    [Fact]
    public async Task ReadAsStringAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            FileExtensions.ReadAsStringAsync(nonExistentFile));
    }

    [Fact]
    public async Task ReadAsBytesAsync_ShouldReturnFileContent_WhenFileExists()
    {
        // Arrange
        var expectedContent = Encoding.UTF8.GetBytes("Test file content");

        // Act
        var content = await FileExtensions.ReadAsBytesAsync(_testFile);

        // Assert
        Assert.Equal(expectedContent, content);
    }

    [Fact]
    public async Task ReadAsBytesAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            FileExtensions.ReadAsBytesAsync(nonExistentFile));
    }

    [Fact]
    public void CalculateFileHash_ShouldReturnConsistentHash_ForSameFile()
    {
        // Arrange & Act
        var hash1 = FileExtensions.CalculateFileHash(_testFile);
        var hash2 = FileExtensions.CalculateFileHash(_testFile);

        // Assert
        Assert.NotNull(hash1);
        Assert.NotEmpty(hash1);
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void CalculateFileHash_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() =>
            FileExtensions.CalculateFileHash(nonExistentFile));
    }

    [Fact]
    public async Task CompressFileAsync_ShouldCreateCompressedFile()
    {
        // Arrange
        var sourceFilePath = _testFile;
        var destinationFilePath = _compressedFile;

        // Act
        await FileExtensions.CompressFileAsync(sourceFilePath, destinationFilePath);

        // Assert
        Assert.True(File.Exists(destinationFilePath));
        Assert.True(new FileInfo(destinationFilePath).Length > 0);
    }

    [Fact]
    public async Task CompressFileAsync_ShouldThrowFileNotFoundException_WhenSourceFileDoesNotExist()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");
        var destinationFilePath = Path.Combine(_testDirectory, "nonexistent.txt.gz");

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            FileExtensions.CompressFileAsync(nonExistentFile, destinationFilePath));
    }

    [Fact]
    public async Task DecompressFileAsync_ShouldRecreateOriginalFile()
    {
        // Arrange
        var sourceFilePath = _testFile;
        var compressedFilePath = _compressedFile;
        var decompressedFilePath = Path.Combine(_testDirectory, "decompressed.txt");

        await FileExtensions.CompressFileAsync(sourceFilePath, compressedFilePath);

        // Act
        await FileExtensions.DecompressFileAsync(compressedFilePath, decompressedFilePath);

        // Assert
        Assert.True(File.Exists(decompressedFilePath));

        var originalContent = await File.ReadAllTextAsync(sourceFilePath);
        var decompressedContent = await File.ReadAllTextAsync(decompressedFilePath);
        Assert.Equal(originalContent, decompressedContent);
    }

    [Fact]
    public async Task DecompressFileAsync_ShouldThrowFileNotFoundException_WhenCompressedFileDoesNotExist()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt.gz");
        var destinationFilePath = Path.Combine(_testDirectory, "nonexistent_decompressed.txt");

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            FileExtensions.DecompressFileAsync(nonExistentFile, destinationFilePath));
    }

    [Fact]
    public async Task ReadCsvFileAsync_ShouldReturnParsedData_WhenFileExists()
    {
        // Arrange
        var expectedRows = new List<string[]>
        {
            new[] { "Name", "Age", "Email" },
            new[] { "John Doe", "30", "john@example.com" },
            new[] { "Jane Smith", "25", "jane@example.com" }
        };

        // Act
        var data = await FileExtensions.ReadCsvFileAsync(_csvTestFile);

        // Assert
        Assert.Equal(expectedRows.Count, data.Count);

        for (var i = 0; i < expectedRows.Count; i++)
        {
            Assert.Equal(expectedRows[i], data[i]);
        }
    }

    [Fact]
    public async Task ReadCsvFileAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.csv");

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            FileExtensions.ReadCsvFileAsync(nonExistentFile));
    }

    [Fact]
    public async Task WriteToCsvFileAsync_ShouldCreateCsvFile_WithProvidedData()
    {
        // Arrange
        var people = new List<Person>
        {
            new Person { Name = "John", Age = 30, Email = "john@example.com" },
            new Person { Name = "Jane", Age = 25, Email = "jane@example.com" }
        };

        var outputFilePath = Path.Combine(_testDirectory, "output.csv");

        // Act
        await FileExtensions.WriteToCsvFileAsync(
            outputFilePath,
            people,
            p => new[] { p.Name, p.Age.ToString(), p.Email }!);

        // Assert
        Assert.True(File.Exists(outputFilePath));

        var lines = await File.ReadAllLinesAsync(outputFilePath);
        Assert.Equal(2, lines.Length);
        Assert.Equal("John,30,john@example.com", lines[0]);
        Assert.Equal("Jane,25,jane@example.com", lines[1]);
    }

    [Fact]
    public async Task ToByteArrayAsync_ShouldReturnEmptyArray_WhenFileLengthIsZero()
    {
        // Arrange
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.Length).Returns(0);

        // Act
        var result = await formFileMock.Object.ToByteArrayAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ToByteArrayAsync_ShouldReturnFileContent_WhenFileHasContent()
    {
        // Arrange
        var content = "Test file content";
        var contentBytes = Encoding.UTF8.GetBytes(content);

        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.Length).Returns(contentBytes.Length);
        formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, _) =>
            {
                stream.Write(contentBytes, 0, contentBytes.Length);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await formFileMock.Object.ToByteArrayAsync();

        // Assert
        Assert.Equal(contentBytes, result);
    }

    [Theory]
    [InlineData(1, 1 * 1024 * 1024, true)]
    public void CheckFileSize_ShouldReturnExpectedResult(long fileSizeInBytes, int maxSizeInBytes, bool expected)
    {
        // Arrange
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.Length).Returns(fileSizeInBytes);

        // Act
        var result = formFileMock.Object.CheckFileSize(maxSizeInBytes / (1024 * 1024));

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("test.jpg", new[] { ".jpg", ".png" }, true)]
    [InlineData("test.PDF", new[] { ".jpg", ".png", ".pdf" }, true)]
    [InlineData("test.gif", new[] { ".jpg", ".png" }, false)]
    [InlineData("", new[] { ".jpg", ".png" }, false)]
    [InlineData("test", new[] { ".jpg", ".png" }, false)]
    public void HasValidExtension_ShouldReturnExpectedResult(string fileName, string[] allowedExtensions, bool expected)
    {
        // Arrange
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.FileName).Returns(fileName);

        // Act
        var result = formFileMock.Object.HasValidExtension(allowedExtensions.ToList());

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task SaveToLocalAsync_ShouldReturnEmptyString_WhenFileLengthIsZero()
    {
        // Arrange
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.Length).Returns(0);

        // Act
        var result = await formFileMock.Object.SaveToLocalAsync(_testDirectory);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task SaveToLocalAsync_ShouldSaveFile_WithGeneratedFileName_WhenFileNameNotProvided()
    {
        // Arrange
        var content = "Test file content";
        var contentBytes = Encoding.UTF8.GetBytes(content);

        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.Length).Returns(contentBytes.Length);
        formFileMock.Setup(f => f.FileName).Returns("original.txt");
        formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, _) =>
            {
                stream.Write(contentBytes, 0, contentBytes.Length);
            })
            .Returns(Task.CompletedTask);

        // Act
        var savedFileName = await formFileMock.Object.SaveToLocalAsync(_testDirectory);

        // Assert
        Assert.NotEmpty(savedFileName);
        Assert.EndsWith(".txt", savedFileName);
        Assert.True(File.Exists(Path.Combine(_testDirectory, savedFileName)));
    }

    [Fact]
    public async Task SaveToLocalAsync_ShouldSaveFile_WithProvidedFileName()
    {
        // Arrange
        var content = "Test file content";
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var fileName = "custom.txt";

        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.Length).Returns(contentBytes.Length);
        formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, _) =>
            {
                stream.Write(contentBytes, 0, contentBytes.Length);
            })
            .Returns(Task.CompletedTask);

        // Act
        var savedFileName = await formFileMock.Object.SaveToLocalAsync(_testDirectory, fileName);

        // Assert
        Assert.Equal(fileName, savedFileName);
        Assert.True(File.Exists(Path.Combine(_testDirectory, fileName)));
    }

    [Fact]
    public async Task SaveToLocalAsync_ShouldCreateDirectory_WhenDirectoryDoesNotExist()
    {
        // Arrange
        var content = "Test file content";
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var fileName = "test.txt";
        var newDirectory = Path.Combine(_testDirectory, "newDir");

        if (Directory.Exists(newDirectory))
        {
            Directory.Delete(newDirectory, true);
        }

        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.Length).Returns(contentBytes.Length);
        formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, _) =>
            {
                stream.Write(contentBytes, 0, contentBytes.Length);
            })
            .Returns(Task.CompletedTask);

        // Act
        var savedFileName = await formFileMock.Object.SaveToLocalAsync(newDirectory, fileName);

        // Assert
        Assert.Equal(fileName, savedFileName);
        Assert.True(Directory.Exists(newDirectory));
        Assert.True(File.Exists(Path.Combine(newDirectory, fileName)));
    }

    private class Person
    {
        public string? Name { get; init; }
        public int Age { get; init; }
        public string? Email { get; init; }
    }
}