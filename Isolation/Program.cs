using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication3 {

	static class Extensions {
		public static string PrintPiece(this Board.Piece source) {
			switch (source) {
				case Board.Piece.Black: return "B";
				case Board.Piece.Empty: return " ";
				case Board.Piece.Block: return " X ";
				default: return "W";
			}
		}
	}

	class Move {
		public int destination;
	}

	class PlaceMove : Move {
		public Board.Piece piece;
		public PlaceMove(Board.Piece p, int destination) {
			this.destination = destination;
			piece = p;
		}
	}

	class MoveMove : Move {
		public int initial;

		public MoveMove(int initial, int destination) {
			this.destination = destination;
			this.initial = initial;
		}
	}

	class Board {
		public enum Piece { Black = -1, Empty, White, Block }

		readonly Piece [] pos;
		Piece mTurn = Piece.White;
		int mMoveCount = 0;
		
		public Board() {
			pos = new Piece[49];
			for(int i = 0;i < 49;i++) {
				pos[i] = Board.Piece.Empty;
 			}
		}

		public void SwitchTurn() {
			mTurn = (Piece)((int)mTurn * -1);
		}

		public void MakeMove(PlaceMove m) {
			mMoveCount++;
			pos[m.destination] = m.piece;
		}

		public void MakeMove(MoveMove m) {
			mMoveCount++;
			pos[m.destination] = pos[m.initial];
			pos[m.initial] = Piece.Block;
		}

		public void unMakeMove(PlaceMove m) {
			mMoveCount--;
			pos[m.destination] = Piece.Empty;
		}

		public void unMakeMove(MoveMove m) {
			mMoveCount--;
			pos[m.initial] = pos[m.destination];
			pos[m.destination] = Piece.Empty;
		}

		public Move[] GetLegalMoves() {
			var moves = new List<Move>();
			for(int i = 0;i < 49;i++) {
				while(pos[i] != mTurn) i++;
				int col = i % 7;
				int row = i / 7;
				int tmpCol, tmpRow;
				if(col > 0) {
					tmpCol = col-1;
					while(tmpCol >= 0 && pos[row*7+tmpCol] == Piece.Empty) { moves.Add(new MoveMove(row*7+col, row*7+tmpCol)); tmpCol--; } //WEST
					if(row > 0) {
						tmpRow = row-1;
						tmpCol = col-1;
						while(tmpCol >= 0 && tmpRow >= 0 && pos[tmpRow*7+tmpCol] == Piece.Empty) { moves.Add(new MoveMove(row*7+col, tmpRow*7+tmpCol)); tmpCol--; tmpRow--; } //SOUTH-WEST
					}

					if(row < 7) {
						tmpRow = row+1;
						tmpCol = col-1;
						while(tmpCol >= 0 && tmpRow < 7 && pos[tmpRow*7+tmpCol] == Piece.Empty) { moves.Add(new MoveMove(row*7+col, tmpRow*7+tmpCol)); tmpCol--; tmpRow++; } //NORTH-WEST
					}
				}

				if(row < 7) {
					tmpRow = row+1;
					while(tmpRow < 7 && pos[tmpRow*7+col] == Piece.Empty) { moves.Add(new MoveMove(row*7+col, tmpRow*7+col)); tmpRow++; } //NORTH
					if(col < 7) {
						tmpRow = row+1;
						tmpCol = col+1;
						while(tmpCol < 7 && tmpRow < 7 && pos[tmpRow*7+tmpCol] == Piece.Empty) { moves.Add(new MoveMove(row*7+col, tmpRow*7+tmpCol)); tmpCol++; tmpRow++; } //NORTH-EAST
					}
				}

				if(col < 7) {
					tmpCol = col+1;
					while(tmpCol < 7 && pos[row*7+tmpCol] == Piece.Empty) { moves.Add(new MoveMove(row*7+col, row*7+tmpCol)); tmpCol++; } //EAST
					if(row > 0) {
						tmpRow = row-1;
						tmpCol = col+1;
						while(tmpCol < 7 && tmpRow >= 0 && pos[tmpRow*7+tmpCol] == Piece.Empty) { moves.Add(new MoveMove(row*7+col, tmpRow*7+tmpCol)); tmpCol++; tmpRow--; } //SOUTH-EAST
					}
				}

				if(row >= 0) {
					tmpRow = row-1;
					while(tmpRow >= 0 && pos[tmpRow*7+col] == Piece.Empty) { moves.Add(new MoveMove(row*7+col, tmpRow*7+col)); tmpRow--; } //SOUTH
				}
			}
			return moves.ToArray();
		}

		public void PrintBoard() {
			Console.WriteLine("   +---+---+---+---+---+---+---+");
			for(int row = 6;row >= 0;row--) {
				Console.Write($" {row+1} |");
				for(int col = 0;col < 7;col++) {
					Console.Write($" {pos[row*7+col].PrintPiece()} |");
				}
				Console.WriteLine();
				Console.WriteLine("   +---+---+---+---+---+---+---+");
			}
			Console.WriteLine("     A   B   C   D   E   F   G");
		}
	}

	class Program {
		static void Main(string[] args) {
			var b = new Board();

			b.MakeMove(new PlaceMove(Board.Piece.White, 0));
			b.MakeMove(new PlaceMove(Board.Piece.White, 3));
			b.MakeMove(new PlaceMove(Board.Piece.White, 6));

			b.MakeMove(new PlaceMove(Board.Piece.White, 21));
			b.MakeMove(new PlaceMove(Board.Piece.White, 24));
			b.MakeMove(new PlaceMove(Board.Piece.White, 27));

			b.MakeMove(new PlaceMove(Board.Piece.White, 42));
			b.MakeMove(new PlaceMove(Board.Piece.White, 45));
			b.MakeMove(new PlaceMove(Board.Piece.White, 48));

			b.PrintBoard();


			Console.Read();
		}
	}
}
