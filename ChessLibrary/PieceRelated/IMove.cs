using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public interface IMove
{
    void Movement(Square to);
}
