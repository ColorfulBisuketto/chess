namespace Chess.Core;

/// <summary>
/// Special Action that has to be taken when a normal action cannot handle the logic or when reversal is not possible.
/// </summary>
public enum SpecialPlyAction
{
    Castle,
    CaptureEnPassant,
    Promote,
}
