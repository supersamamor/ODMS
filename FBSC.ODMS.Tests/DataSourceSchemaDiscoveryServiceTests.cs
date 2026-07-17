using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Infrastructure.Services.Dashboard;

namespace FBSC.ODMS.Tests;

public class DataSourceSchemaDiscoveryServiceTests
{
    [Theory]
    [InlineData("Id", "int", SemanticType.Identifier)]
    [InlineData("EmployeeId", "int", SemanticType.Identifier)]
    [InlineData("ProductCode", "nvarchar", SemanticType.Identifier)]
    [InlineData("RowGuid", "uniqueidentifier", SemanticType.Identifier)]
    [InlineData("HireDate", "datetime", SemanticType.Date)]
    [InlineData("CreatedAt", "date", SemanticType.Date)]
    [InlineData("Salary", "decimal", SemanticType.Measure)]
    [InlineData("Quantity", "int", SemanticType.Measure)]
    [InlineData("DepartmentName", "nvarchar", SemanticType.Dimension)]
    [InlineData("Status", "varchar", SemanticType.Dimension)]
    public void InferSemanticType_MatchesExpectedRole(string columnName, string sqlDataType, string expected)
    {
        var result = DataSourceSchemaDiscoveryService.InferSemanticType(columnName, sqlDataType);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void InferSemanticType_DatePrecedesIdentifierHeuristic()
    {
        // "ModifiedDate" ends up neither Id/Code/Guid/Key-suffixed nor numeric, but it must
        // resolve via the date-type check first regardless of column-name wording.
        var result = DataSourceSchemaDiscoveryService.InferSemanticType("ModifiedDate", "datetime2");

        Assert.Equal(SemanticType.Date, result);
    }
}
