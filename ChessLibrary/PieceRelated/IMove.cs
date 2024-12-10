using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public interface IMove
{
    void Move(Square to);
}
