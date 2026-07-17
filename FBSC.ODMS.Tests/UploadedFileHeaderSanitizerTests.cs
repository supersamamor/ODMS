using FBSC.ODMS.Infrastructure.Services.Dashboard;

namespace FBSC.ODMS.Tests;

public class UploadedFileHeaderSanitizerTests
{
    [Fact]
    public void Sanitize_DeduplicatesRepeatedHeaders()
    {
        var result = UploadedFileHeaderSanitizer.Sanitize(["Name", "Name", "Name"]);

        Assert.Equal(["Name", "Name_2", "Name_3"], result);
    }

    [Fact]
    public void Sanitize_FallsBackToColumnN_ForEmptyHeaders()
    {
        var result = UploadedFileHeaderSanitizer.Sanitize(["", null, "   "]);

        Assert.Equal(["Column_1", "Column_2", "Column_3"], result);
    }

    [Theory]
    [InlineData("select")]
    [InlineData("SELECT")]
    [InlineData("Table")]
    [InlineData("Order")]
    [InlineData("User")]
    public void Sanitize_FallsBackToColumnN_ForReservedWords(string reservedWord)
    {
        var result = UploadedFileHeaderSanitizer.Sanitize([reservedWord]);

        Assert.Equal("Column_1", result[0]);
    }

    [Fact]
    public void Sanitize_NeutralizesSqlInjectionShapedHeader()
    {
        var result = UploadedFileHeaderSanitizer.Sanitize(["Name]; DROP TABLE Users; --"]);

        // Must be a pure [A-Za-z0-9_] token - no brackets, semicolons, spaces, or dashes ever
        // survive into a column name that gets bracket-quoted into generated DDL.
        Assert.Matches("^[A-Za-z0-9_]+$", result[0]);
        Assert.DoesNotContain(';', result[0]);
        Assert.DoesNotContain(']', result[0]);
        Assert.DoesNotContain(' ', result[0]);
        Assert.DoesNotContain('-', result[0]);
    }

    [Fact]
    public void Sanitize_StripsNonAsciiCharacters()
    {
        var result = UploadedFileHeaderSanitizer.Sanitize(["Employé Näme"]);

        Assert.Matches("^[A-Za-z0-9_]+$", result[0]);
        Assert.NotEqual("", result[0]);
    }

    [Fact]
    public void Sanitize_PrefixesNamesStartingWithADigit()
    {
        var result = UploadedFileHeaderSanitizer.Sanitize(["2026Revenue"]);

        Assert.False(char.IsDigit(result[0][0]));
        Assert.StartsWith("Col_", result[0]);
    }

    [Fact]
    public void Sanitize_CollapsesRepeatedSeparatorsIntoSingleUnderscore()
    {
        var result = UploadedFileHeaderSanitizer.Sanitize(["First   Name!!!"]);

        Assert.Equal("First_Name", result[0]);
    }

    [Fact]
    public void Sanitize_PreservesAlreadyCleanHeaders()
    {
        var result = UploadedFileHeaderSanitizer.Sanitize(["EmployeeCode", "HireDate", "Salary"]);

        Assert.Equal(["EmployeeCode", "HireDate", "Salary"], result);
    }
}
