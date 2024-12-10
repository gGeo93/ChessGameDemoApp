using ChessLibrary.BoardRelated;
using ChessLibrary.PieceRelated;

namespace ChessLibrary.HellpingMethods;

public static class HelpingMethods
{
    public static BoardRelatedInfo[,] ChessBoardFill(this BoardRelatedInfo[,] board)
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (i == 7)
                {
                    if (j == 0 || j == 7)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Rook { Color = PieceColor.WHITE, Name = PieceName.ROOK },
                            ASquare = new Square { Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK }
                        };
                    }
                    else if (j == 1 || j == 6)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Knight { Color = PieceColor.WHITE, Name = PieceName.KNIGHT },
                            ASquare = new Square { Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK }
                        };
                    }
                    else if (j == 2 || j == 5)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Bishop { Color = PieceColor.WHITE, Name = PieceName.BISHOP },
                            ASquare = new Square { Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK }
                        };
                    }
                    else if (j == 3)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Queen { Color = PieceColor.WHITE, Name = PieceName.QUEEN },
                            ASquare = new Square { Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK }
                        };
                    }
                    else if (j == 4)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new King { Color = PieceColor.WHITE, Name = PieceName.KING },
                            ASquare = new Square { Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK }
                        };
                    }
                }
                else if (i == 6)
                    board[i, j] = new BoardRelatedInfo
                    {
                        Apiece = new Pawn { Color = PieceColor.WHITE, Name = PieceName.PAWN },
                        ASquare = new Square { Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK }
                    };
                else if (i == 1)
                    board[i, j] = new BoardRelatedInfo
                    {
                        Apiece = new Pawn { Color = PieceColor.BLACK, Name = PieceName.PAWN },
                        ASquare = new Square { Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK }
                    };
                else if (i == 0)
                {
                    if (j == 0 || j == 7)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Rook { Color = PieceColor.BLACK, Name = PieceName.ROOK },
                            ASquare = new Square { Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK }
                        };
                    }
                    else if (j == 1 || j == 6)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Knight { Color = PieceColor.BLACK, Name = PieceName.KNIGHT },
                            ASquare = new Square { Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK }
                        };
                    }
                    else if (j == 2 || j == 5)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Bishop { Color = PieceColor.BLACK, Name = PieceName.BISHOP },
                            ASquare = new Square { Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK }
                        };
                    }
                    else if (j == 3)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Queen { Color = PieceColor.BLACK, Name = PieceName.QUEEN },
                            ASquare = new Square { Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK }
                        };
                    }
                    else if (j == 4)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new King { Color = PieceColor.BLACK, Name = PieceName.KING },
                            ASquare = new Square { Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK }
                        };
                    }
                }
            }
        }
        return board;
    }
}
