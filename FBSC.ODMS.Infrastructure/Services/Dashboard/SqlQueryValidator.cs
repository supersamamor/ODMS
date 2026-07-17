using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

public record SqlValidationResult(bool IsValid, string? ErrorMessage);

/// <summary>
/// Parses a widget/query's SQL text with the real T-SQL grammar (ScriptDom) rather than
/// pattern-matching, so validation can't be tricked by comments, string literals, or
/// unusual whitespace the way a regex/keyword-blocklist check could be. Enforces: exactly
/// one batch, exactly one statement, that statement is a plain SELECT (no SELECT...INTO,
/// no nested DDL/DML/EXEC anywhere in the tree).
/// </summary>
public class SqlQueryValidator
{
    public SqlValidationResult Validate(string? sqlQueryText)
    {
        if (string.IsNullOrWhiteSpace(sqlQueryText))
        {
            return new SqlValidationResult(false, "Query text is required.");
        }

        var parser = new TSql160Parser(initialQuotedIdentifiers: true);
        TSqlFragment fragment;
        using (var reader = new StringReader(sqlQueryText))
        {
            fragment = parser.Parse(reader, out var parseErrors);
            if (parseErrors is { Count: > 0 })
            {
                return new SqlValidationResult(false, $"SQL syntax error: {parseErrors[0].Message} (line {parseErrors[0].Line})");
            }
        }

        if (fragment is not TSqlScript script)
        {
            return new SqlValidationResult(false, "Query could not be parsed.");
        }

        var statements = script.Batches.SelectMany(b => b.Statements).ToList();
        if (statements.Count == 0)
        {
            return new SqlValidationResult(false, "Query text is required.");
        }
        if (script.Batches.Count > 1 || statements.Count > 1)
        {
            return new SqlValidationResult(false, "Only a single SELECT statement is allowed - remove any additional statements or GO separators.");
        }

        if (statements[0] is not SelectStatement selectStatement)
        {
            return new SqlValidationResult(false, "Only SELECT statements are allowed.");
        }

        var disallowedFinder = new DisallowedFragmentVisitor();
        selectStatement.Accept(disallowedFinder);
        if (disallowedFinder.Found is not null)
        {
            return new SqlValidationResult(false, $"Statement type '{disallowedFinder.Found.Name}' is not allowed - only read-only SELECT queries are permitted.");
        }

        if (selectStatement.Into is not null)
        {
            return new SqlValidationResult(false, "SELECT INTO is not allowed - queries must not create or modify tables.");
        }

        return new SqlValidationResult(true, null);
    }

    /// <summary>
    /// Overrides one ExplicitVisit per disallowed statement type (rather than the abstract
    /// TSqlStatement base) since that's the guaranteed dispatch surface ScriptDom's visitor
    /// exposes for concrete fragment classes.
    /// </summary>
    private class DisallowedFragmentVisitor : TSqlFragmentVisitor
    {
        public Type? Found { get; private set; }

        public override void ExplicitVisit(InsertStatement node) => Flag(node);
        public override void ExplicitVisit(UpdateStatement node) => Flag(node);
        public override void ExplicitVisit(DeleteStatement node) => Flag(node);
        public override void ExplicitVisit(MergeStatement node) => Flag(node);
        public override void ExplicitVisit(CreateTableStatement node) => Flag(node);
        public override void ExplicitVisit(AlterTableStatement node) => Flag(node);
        public override void ExplicitVisit(DropTableStatement node) => Flag(node);
        public override void ExplicitVisit(CreateProcedureStatement node) => Flag(node);
        public override void ExplicitVisit(CreateFunctionStatement node) => Flag(node);
        public override void ExplicitVisit(CreateViewStatement node) => Flag(node);
        public override void ExplicitVisit(CreateIndexStatement node) => Flag(node);
        public override void ExplicitVisit(DropIndexStatement node) => Flag(node);
        public override void ExplicitVisit(TruncateTableStatement node) => Flag(node);
        public override void ExplicitVisit(ExecuteStatement node) => Flag(node);
        public override void ExplicitVisit(GrantStatement node) => Flag(node);
        public override void ExplicitVisit(DenyStatement node) => Flag(node);
        public override void ExplicitVisit(RevokeStatement node) => Flag(node);
        public override void ExplicitVisit(UseStatement node) => Flag(node);

        private void Flag(TSqlFragment node) => Found ??= node.GetType();
    }
}
