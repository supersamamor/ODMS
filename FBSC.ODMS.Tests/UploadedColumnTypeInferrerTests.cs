using FBSC.ODMS.Infrastructure.Services.Dashboard;

namespace FBSC.ODMS.Tests;

public class UploadedColumnTypeInferrerTests
{
    [Fact]
    public void InferSqlType_AllIntegers_InfersInt()
    {
        var result = UploadedColumnTypeInferrer.InferSqlType(["1", "2", "300"]);

        Assert.Equal("int", result);
    }

    [Fact]
    public void InferSqlType_MixedIntAndDecimal_InfersDecimal()
    {
        var result = UploadedColumnTypeInferrer.InferSqlType(["1", "2.5", "300"]);

        Assert.Equal("decimal(18,4)", result);
    }

    [Fact]
    public void InferSqlType_DateLikeStrings_InfersDatetime()
    {
        var result = UploadedColumnTypeInferrer.InferSqlType(["2026-01-15", "2026-02-01", "2026-03-20"]);

        Assert.Equal("datetime2", result);
    }

    [Fact]
    public void InferSqlType_MixedText_FallsBackToNvarchar()
    {
        var result = UploadedColumnTypeInferrer.InferSqlType(["Engineering", "Sales", "HR"]);

        Assert.Equal("nvarchar(450)", result);
    }

    [Fact]
    public void InferSqlType_BlankCellsAreIgnoredWhenInferringFromTheRest()
    {
        var result = UploadedColumnTypeInferrer.InferSqlType(["1", "", null, "  ", "42"]);

        Assert.Equal("int", result);
    }

    [Fact]
    public void InferSqlType_AllBlank_FallsBackToNvarchar()
    {
        var result = UploadedColumnTypeInferrer.InferSqlType(["", null, "   "]);

        Assert.Equal("nvarchar(450)", result);
    }

    [Fact]
    public void InferSqlType_OneNonNumericValueAmongIntegers_FallsBackToNvarchar()
    {
        // A single "N/A"-style value in an otherwise-numeric column must not silently coerce
        // to a numeric type that would truncate/reject it on bulk load.
        var result = UploadedColumnTypeInferrer.InferSqlType(["1", "2", "N/A", "4"]);

        Assert.Equal("nvarchar(450)", result);
    }

    [Theory]
    [InlineData("int", typeof(int))]
    [InlineData("decimal(18,4)", typeof(decimal))]
    [InlineData("datetime2", typeof(DateTime))]
    [InlineData("nvarchar(450)", typeof(string))]
    public void ToClrType_MatchesInferredSqlType(string sqlType, Type expectedClrType)
    {
        Assert.Equal(expectedClrType, UploadedColumnTypeInferrer.ToClrType(sqlType));
    }
}
