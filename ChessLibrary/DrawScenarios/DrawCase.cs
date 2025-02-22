using ChessLibrary.BoardRelated;
using ChessLibrary.GamingProcessRelated;

namespace ChessLibrary.DrawScenarios;

public static class DrawCase
{
    public static bool IsStalemate(this ChessBoard chessBoard, WhoseTurn whoPlays)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                //Check if there is at least one legal move
                return false;
            }
        }
        return true;
    }
}
