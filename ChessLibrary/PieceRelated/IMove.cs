using ChessLibrary.BoardRelated;

namespace ChessLibrary.PieceRelated;

public interface IMove
{
    bool Movement(Square from,Square to);
}
