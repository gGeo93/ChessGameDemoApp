using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.RulesRelated;

public class Constraints
{
    public bool CanPerfomeThisMove(PieceName pieceName,Square from, Square to, WhoseTurn whoPlays)
    {
        switch (pieceName)
        {
            case PieceName.PAWN: return new Pawn(whoPlays).Movement(from, to);
            case PieceName.KNIGHT: return new Knight().Movement(from, to);
            case PieceName.BISHOP: return new Bishop().Movement(from, to);
            case PieceName.ROOK: return new Rook().Movement(from, to);
            case PieceName.QUEEN: return new Queen().Movement(from, to);
            case PieceName.KING: return new King().Movement(from, to);  
            default: return true;
        }
    }
}
