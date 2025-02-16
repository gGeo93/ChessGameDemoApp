﻿using ChessLibrary.PieceRelated;

namespace ChessLibrary.GamingProcessRelated;

public class MaterialLeft
{
    public List<Pawn> W_Pawns { get; set; }
    public List<Pawn> B_Pawns { get; set; }
    public List<Rook> W_Rooks { get; set; }
    public List<Rook> B_Rooks { get; set; }
    public List<Knight> W_Knights { get; set; }
    public List<Knight> B_Knights { get; set; }
    public List<Bishop> W_Bishops { get; set; }
    public List<Bishop> B_Bishops { get; set; }
    public List<Queen> W_Queen { get; set; }
    public List<Queen> B_Queen { get; set; }
    public King W_King { get; set; }
    public King B_King { get; set; }

    public MaterialLeft()
    {
        W_Pawns = new List<Pawn>(8);
        B_Pawns = new List<Pawn>(8);
        W_Rooks = new List<Rook>(2);
        B_Rooks = new List<Rook>(2);
        W_Knights = new List<Knight>(2);
        B_Knights = new List<Knight>(2);
        W_Bishops = new List<Bishop>(2);
        B_Bishops = new List<Bishop>(2);
        W_Queen = new List<Queen>(1);
        B_Queen = new List<Queen>(1);
        W_King = new King();
        B_King = new King();
    }
}
