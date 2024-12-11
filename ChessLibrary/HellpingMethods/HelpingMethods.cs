using ChessLibrary.BoardRelated;
using ChessLibrary.PieceRelated;
using System.Diagnostics;

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
                            Apiece = new Rook { Color = PieceInfo.WHITE, Name = PieceName.ROOK },
                            ASquare = new Square {Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 1 || j == 6)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Knight { Color = PieceInfo.WHITE, Name = PieceName.KNIGHT },
                            ASquare = new Square {Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 2 || j == 5)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Bishop { Color = PieceInfo.WHITE, Name = PieceName.BISHOP },
                            ASquare = new Square {Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 3)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Queen { Color = PieceInfo.WHITE, Name = PieceName.QUEEN },
                            ASquare = new Square {Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 4)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new King { Color = PieceInfo.WHITE, Name = PieceName.KING },
                            ASquare = new Square { Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                }
                else if (i == 6)
                    board[i, j] = new BoardRelatedInfo
                    {
                        Apiece = new Pawn { Color = PieceInfo.WHITE, Name = PieceName.PAWN },
                        ASquare = new Square {Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                    };
                else if (i == 1)
                    board[i, j] = new BoardRelatedInfo
                    {
                        Apiece = new Pawn { Color = PieceInfo.BLACK, Name = PieceName.PAWN },
                        ASquare = new Square {Color = j % 2 == 1 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                    };
                else if (i == 0)
                {
                    if (j == 0 || j == 7)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Rook { Color = PieceInfo.BLACK, Name = PieceName.ROOK },
                            ASquare = new Square {Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 1 || j == 6)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Knight { Color = PieceInfo.BLACK, Name = PieceName.KNIGHT },
                            ASquare = new Square {Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 2 || j == 5)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Bishop { Color = PieceInfo.BLACK, Name = PieceName.BISHOP },
                            ASquare = new Square {Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 3)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new Queen { Color = PieceInfo.BLACK, Name = PieceName.QUEEN },
                            ASquare = new Square { Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                    else if (j == 4)
                    {
                        board[i, j] = new BoardRelatedInfo
                        {
                            Apiece = new King { Color = PieceInfo.BLACK, Name = PieceName.KING },
                            ASquare = new Square { Color = j % 2 == 0 ? SquareColor.WHITE : SquareColor.BLACK, Letter = j.ColumnConverter(), Number = i.RowConverter() }
                        };
                    }
                }
                else
                {
                    board[i, j] = new BoardRelatedInfo() { Apiece = new Piece { Name = PieceName.NONE }, ASquare = new Square() };
                }
            }
        }
        return board;
    }
    public static void InternalCoordinatesOperation(this Square from, Square to, out (int xFrom, int yFrom) pseudoCoorFrom, out (int xTo, int yTo) pseudoCoorTo)
    {
        pseudoCoorFrom = (from.Letter + from.Number.ToString()).FromRealToProgrammingCoordinates();
        pseudoCoorTo = (to.Letter + to.Number.ToString()).FromRealToProgrammingCoordinates();
    }
    public static char ColumnConverter(this int y)
    {
        switch (y)
        {
            case 0: return 'a';
            case 1: return 'b';
            case 2: return 'c';
            case 3: return 'd';
            case 4: return 'e';
            case 5: return 'f';
            case 6: return 'g';
            case 7: return 'h';
            default: return '\0';
        }
    }
    public static int RowConverter(this int x)
    {
        switch (x)
        {
            case 0: return 8;
            case 1: return 7;
            case 2: return 6;
            case 3: return 5;
            case 4: return 4;
            case 5: return 3;
            case 6: return 2;
            case 7: return 1;
            default: return 0;
        }
    }
    public static (int x, int y) FromRealToProgrammingCoordinates(this String real)
    {
        int y = -1;
        switch (real[0])
        {
            case 'a': y = 0; break; 
            case 'b': y = 1; break; 
            case 'c': y = 2; break; 
            case 'd': y = 3; break; 
            case 'e': y = 4; break; 
            case 'f': y = 5; break; 
            case 'g': y = 6; break; 
            case 'h': y = 7; break; 
        }
        int x = -1;
        switch (int.Parse(real[1].ToString()))
        {
            case 1: x = 7; break;
            case 2: x = 6; break;
            case 3: x = 5; break;
            case 4: x = 4; break;
            case 5: x = 3; break;
            case 6: x = 2; break;
            case 7: x = 1; break;
            case 8: x = 0; break;
        }
        return (x, y);
    }
}
