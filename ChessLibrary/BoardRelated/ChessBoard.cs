using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.BoardRelated;

public class ChessBoard
{
    public BoardRelatedInfo[,] Board { get; set; }
    public ChessBoard()
    {
        Board = new BoardRelatedInfo[8, 8];
        Board.ChessBoardFill();
    }
}
