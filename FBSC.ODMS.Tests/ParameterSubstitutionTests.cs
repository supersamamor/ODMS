using System.Data;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using Microsoft.Data.SqlClient;

namespace FBSC.ODMS.Tests;

public class ParameterSubstitutionTests
{
    // Proves parameter values become real, typed SqlParameter objects (never string-
    // concatenated into the query text) - the constraint the whole execution engine exists
    // to enforce.
    [Fact]
    public void BuildSqlParameter_BindsStringValueAsParameter_NeverConcatenated()
    {
        var declared = new DashboardQueryParameterState { ParameterName = "DepartmentName", DataType = "string" };

        var parameter = DashboardQueryExecutionService.BuildSqlParameter(declared, "Robert'); DROP TABLE Employees;--");

        Assert.Equal("@DepartmentName", parameter.ParameterName);
        Assert.Equal(SqlDbType.NVarChar, parameter.SqlDbType);
        Assert.Equal("Robert'); DROP TABLE Employees;--", parameter.Value);
    }

    [Fact]
    public void BuildSqlParameter_ParsesIntValue()
    {
        var declared = new DashboardQueryParameterState { ParameterName = "DepartmentId", DataType = "int" };

        var parameter = DashboardQueryExecutionService.BuildSqlParameter(declared, "42");

        Assert.Equal(SqlDbType.Int, parameter.SqlDbType);
        Assert.Equal(42, parameter.Value);
    }

    [Fact]
    public void BuildSqlParameter_ParsesDecimalValue()
    {
        var declared = new DashboardQueryParameterState { ParameterName = "MinAmount", DataType = "decimal" };

        var parameter = DashboardQueryExecutionService.BuildSqlParameter(declared, "1234.56");

        Assert.Equal(SqlDbType.Decimal, parameter.SqlDbType);
        Assert.Equal(1234.56m, parameter.Value);
    }

    [Fact]
    public void BuildSqlParameter_ParsesDateValue()
    {
        var declared = new DashboardQueryParameterState { ParameterName = "AsOf", DataType = "date" };

        var parameter = DashboardQueryExecutionService.BuildSqlParameter(declared, "2026-01-15");

        Assert.Equal(SqlDbType.Date, parameter.SqlDbType);
        Assert.Equal(new DateTime(2026, 1, 15), parameter.Value);
    }

    [Fact]
    public void BuildSqlParameter_ParsesBoolValue()
    {
        var declared = new DashboardQueryParameterState { ParameterName = "IsActive", DataType = "bool" };

        var parameter = DashboardQueryExecutionService.BuildSqlParameter(declared, "true");

        Assert.Equal(SqlDbType.Bit, parameter.SqlDbType);
        Assert.Equal(true, parameter.Value);
    }

    [Fact]
    public void BuildSqlParameter_MissingValueBecomesDbNull_NotSkipped()
    {
        var declared = new DashboardQueryParameterState { ParameterName = "OptionalFilter", DataType = "string" };

        var parameter = DashboardQueryExecutionService.BuildSqlParameter(declared, rawValue: null);

        Assert.Equal(DBNull.Value, parameter.Value);
    }

    [Fact]
    public void BuildSqlParameter_UnknownDataTypeDefaultsToNVarChar()
    {
        var declared = new DashboardQueryParameterState { ParameterName = "Weird", DataType = "some-future-type" };

        var parameter = DashboardQueryExecutionService.BuildSqlParameter(declared, "value");

        Assert.Equal(SqlDbType.NVarChar, parameter.SqlDbType);
    }
}
