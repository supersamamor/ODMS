using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace FBSC.ODMS.Application.Helpers;

public record SqlValidationResult(bool IsValid, IReadOnlyCollection<string> Errors);

public static class SQLValidatorHelper
{
    public static SqlValidationResult Validate(string? sqlScript)
    {
        if (string.IsNullOrWhiteSpace(sqlScript))
        {
            return new SqlValidationResult(true, []);
        }

        // TSql160Parser covers SQL Server 2022/2025
        var parser = new TSql160Parser(initialQuotedIdentifiers: false);
        using var reader = new StringReader(sqlScript);

        var fragment = parser.Parse(reader, out var parseErrors);

        if (parseErrors.Count > 0)
        {
            return new SqlValidationResult(false, ["Invalid SQL syntax detected."]);
        }

        var visitor = new RestrictedSqlVisitor();
        fragment.Accept(visitor);

        return new SqlValidationResult(visitor.Violations.Count == 0, visitor.Violations);
    }
}

internal sealed class RestrictedSqlVisitor : TSqlFragmentVisitor
{
    public List<string> Violations { get; } = [];

    // --- DML / DDL Statements ---
    public override void Visit(InsertStatement node) => CheckTarget(node.InsertSpecification.Target, "Insert");
    public override void Visit(DeleteStatement node) => CheckTarget(node.DeleteSpecification.Target, "Delete");
    public override void Visit(UpdateStatement node) => CheckTarget(node.UpdateSpecification.Target, "Update");
    public override void Visit(CreateTableStatement node) => CheckTable(node.SchemaObjectName, "Create");
    public override void Visit(AlterTableStatement node) => CheckTable(node.SchemaObjectName, "Alter");

    public override void Visit(DropTableStatement node)
    {
        foreach (var table in node.Objects)
        {
            CheckTable(table, "Drop");
        }
    }

    // --- Execution Statements ---
    // --- Restriction: Explicitly block EXEC/EXECUTE ---
    public override void Visit(ExecuteStatement node)
    {
        Violations.Add("SQL Script contains forbidden `EXECUTE` operation.");
    }

    private void CheckTarget(TSqlFragment target, string operation)
    {
        if (target is NamedTableReference tableRef)
        {
            CheckTable(tableRef.SchemaObject, operation);
        }
    }

    private void CheckTable(SchemaObjectName? name, string operation)
    {
        string? tableName = name?.BaseIdentifier?.Value;

        // Allow if it is a temporary table (starts with '#')
        if (!string.IsNullOrEmpty(tableName) && tableName.StartsWith('#'))
        {
            return;
        }

        Violations.Add($"Security Violation: {operation} is not permitted on permanent table '{tableName}'.");
    }
}