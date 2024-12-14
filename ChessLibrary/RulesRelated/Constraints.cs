using ChessLibrary.BoardRelated;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.RulesRelated;

public class Constraints
{
    public bool CanPerfomeThisMove(PieceName pieceName,Square from, Square to)
    {
        switch (pieceName)
        {
            case PieceName.BISHOP: return new Bishop().Movement(from, to);
            case PieceName.ROOK: return new Rook().Movement(from, to);
            case PieceName.QUEEN: return new Queen().Movement(from, to);
            default: return true;
        }
    }
}
