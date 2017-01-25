using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication3 {

	static class Extensions {
		public static string PrintPiece(this Board.Piece source) {
			switch (source) {
				case Board.Piece.Black: return "B";
				case Board.Piece.Empty: return " ";
				case Board.Piece.Block: return "X";
				default: return "W";
			}
		}
	}

	 class Move {
		public int destination;

		public Move(int destination) {
			this.destination = destination;
		}

    public virtual string PrintMove() {
      return $"{(char)(destination%7+'a')}{(char)(destination/7+'1')}";
    }

    public virtual bool Compare(Move m) {
      return m.GetType() == typeof(Move) && this.destination == m.destination;
    }
	}

	class MoveMove : Move {
		public int initial;

		public MoveMove(int initial, int destination) : base(destination) {
			this.initial = initial;
		}

    public override string PrintMove() {
      return $"{(char)(initial%7+'a')}{(char)(initial/7+'1')}{(char)(destination%7+'a')}{(char)(destination/7+'1')}";
    }

    public override bool Compare(Move m) {
      return m.GetType() == typeof(MoveMove) &&
        this.destination == ((MoveMove)m).destination && 
        this.initial == ((MoveMove)m).initial;
    }
	}

	class Board {
		public enum Piece { Black = -1, Empty, White, Block }

    Stack<Move> mMoves;
		readonly Piece [] pos;
		Piece mTurn = Piece.White;
		int mMoveCount = 0;
		
		public Board() {
			pos = new Piece[49];
      mMoves = new Stack<Move>();
			for(int i = 0;i < 49;i++) {
				pos[i] = Board.Piece.Empty;
 			}
		}

		public void SwitchTurn() {
			mTurn = (Piece)((int)mTurn * -1);
		}

		public void MakeMove(Move m) {
			mMoveCount++;
			pos[m.destination] = mTurn;
      if(m.GetType() == typeof(MoveMove))
        pos[((MoveMove)m).initial] = Piece.Block;
      SwitchTurn();
      mMoves.Push(m);
		}

		public void unMakeMove() {
      var m = mMoves.Pop();
			mMoveCount--;
			pos[m.destination] = Piece.Empty;
      SwitchTurn();
      if(m.GetType() == typeof(MoveMove))
        pos[((MoveMove)m).initial] = Piece.Block;
		}

    public static bool IsLegal(Move [] moves, Move m) {
      return moves.Any(mv => m.Compare(mv));
    }

    public static bool IsEnd(Move [] moves) {
      return !moves.Any();
    }

		public Move[] GetLegalMoves() {
			var moves = new List<Move>();
      if(mMoveCount < 4) {
        return pos.Select((x, i) => new { a = x == Piece.Empty, i })
          .Where(x => x.a)
          .Select(x => new Move(x.i))
          .ToArray();
      }
			for(int i = 0;i < 49;i++) {
				while(i < 49 && pos[i] != mTurn) i++;
        if(i == 49) break;
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
      Move [] moves = null;
      while(moves == null || !Board.IsEnd(moves)) {
        moves = b.GetLegalMoves();
        b.PrintBoard();
        foreach(var m in moves) { Console.Write(m.PrintMove() + " "); }
        Console.Write("\nEnter command: ");
        var command = Console.ReadLine();
        switch(command) {
          case "quit": return;
          case "undo": b.unMakeMove(); break;
          default:
            Move move = null;
            if(command.Length == 2) {
              move = new Move((command.ToCharArray()[0] - 'a')+(command.ToCharArray()[1] - '1')*7);
            } else if (command.Length == 4) {
              move = new MoveMove(
                (command.ToCharArray()[0] - 'a')+(command.ToCharArray()[1] - '1')*7,
                (command.ToCharArray()[2] - 'a')+(command.ToCharArray()[3] - '1')*7);
            }
            if(move == null || !Board.IsLegal(moves, move)) {
              Console.WriteLine("Illegal move");
              continue;
            }

            b.MakeMove(move);
            break;
        }
      }
		}
	}
}
