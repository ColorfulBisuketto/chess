using Chess.Core.Pieces;

namespace Chess.Core;

/// <summary>
/// A move that has already been validated and is ready  to be executed.
/// </summary>
/// <param name="Piece">The Piece to move.</param>
/// <param name="StartPosition">The starting position</param>
/// <param name="EndPosition">The starting position</param>
/// <param name="CapturedPiece">The Piece that will be captured. Null if no piece is to be captured.</param>
/// <param name="SpecialPlyAction">The special action that will be taken. Null if no special action will be taken this ply.</param>
/// <param name="PromoteToPiece">The Piece that the current Piece will be promoted to. Null if <paramref name="SpecialPlyAction"/> is not Promote</param>
public record ValidMove(
    Piece Piece,
    Position StartPosition,
    Position EndPosition,
    Piece? CapturedPiece,
    SpecialPlyAction? SpecialPlyAction,
    Piece? PromoteToPiece);
