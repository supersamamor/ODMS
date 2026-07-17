using FBSC.ODMS.Infrastructure.Services.Dashboard;

namespace FBSC.ODMS.Tests;

public class SqlQueryValidatorTests
{
    private readonly SqlQueryValidator _validator = new();

    [Theory]
    [InlineData("SELECT * FROM Employees")]
    [InlineData("SELECT [Id], [Name] FROM [dbo].[Employees] WHERE [IsActive] = 1")]
    [InlineData("WITH Cte AS (SELECT Id FROM Employees) SELECT * FROM Cte")]
    [InlineData("SELECT COUNT(*) AS [Total] FROM Employees GROUP BY DepartmentId")]
    public void Validate_AllowsPlainSelectStatements(string sql)
    {
        var result = _validator.Validate(sql);

        Assert.True(result.IsValid, result.ErrorMessage);
    }

    [Theory]
    [InlineData("INSERT INTO Employees (Name) VALUES ('x')")]
    [InlineData("UPDATE Employees SET Name = 'x'")]
    [InlineData("DELETE FROM Employees")]
    [InlineData("DROP TABLE Employees")]
    [InlineData("TRUNCATE TABLE Employees")]
    [InlineData("EXEC sp_who")]
    [InlineData("CREATE TABLE Foo (Id INT)")]
    [InlineData("ALTER TABLE Employees ADD Foo INT")]
    public void Validate_RejectsNonSelectStatements(string sql)
    {
        var result = _validator.Validate(sql);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsMultipleStatements()
    {
        var result = _validator.Validate("SELECT * FROM Employees; SELECT * FROM Departments");

        Assert.False(result.IsValid);
        Assert.Contains("single SELECT", result.ErrorMessage);
    }

    [Fact]
    public void Validate_RejectsStackedDmlAfterSelect()
    {
        var result = _validator.Validate("SELECT * FROM Employees; DROP TABLE Employees;");

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsSelectInto()
    {
        var result = _validator.Validate("SELECT * INTO NewTable FROM Employees");

        Assert.False(result.IsValid);
        Assert.Contains("INTO", result.ErrorMessage);
    }

    [Fact]
    public void Validate_RejectsEmptyText()
    {
        var result = _validator.Validate("   ");

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsInvalidSyntax()
    {
        var result = _validator.Validate("SELEKT * FROM Employees");

        Assert.False(result.IsValid);
    }
}
