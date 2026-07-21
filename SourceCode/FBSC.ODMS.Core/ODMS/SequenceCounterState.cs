namespace FBSC.ODMS.Core.ODMS;

/// <summary>
/// Per-key monotonic counter backing the auto-generated codes (ProjectCode per
/// BusinessUnit, Risk/Issue code per type). One row per key; the value is the
/// last-issued number. Mutated only through <c>SequenceGenerator</c> via an
/// atomic single-statement increment, so it is safe under high concurrency.
/// Intentionally not a <c>BaseEntity</c> - it has no audit lifecycle, just Key/Value.
/// </summary>
public class SequenceCounterState
{
    public string Key { get; set; } = "";
    public long Value { get; set; }
}
