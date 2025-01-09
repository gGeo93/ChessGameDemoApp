using ChessLibrary.HellpingMethods;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.BoardRelated;

public class ChessBoard
{
    public BoardRelatedInfo[,] Board { get; set; }
    public List<BoardRelatedInfo[,]> WholeGameChessBoard { get; set; } = new List<BoardRelatedInfo[,]>();
    public ChessBoard()
    {
        Board = new BoardRelatedInfo[8, 8];
        Board.ChessBoardFill();
        WholeGameChessBoard.Add(Board);
    }
}
