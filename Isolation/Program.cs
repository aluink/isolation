using System;

namespace ConsoleApplication3 {
	internal static class Extensions {
		public static string PrintPiece(this Board.Piece source) {
			switch (source) {
				case Board.Piece.Black:
					return "B";
				case Board.Piece.Empty:
					return " ";
				case Board.Piece.Block:
					return "X";
				default:
					return "W";
			}
		}
	}

	internal class Program {
		static void Main(string[] args) {
			var b = new Board();
			var player = new Player(b);
			Move[] moves = null;
			while (moves == null || !Board.IsEnd(moves)) {
				moves = b.GetLegalMoves(b.Turn);
				b.PrintBoard();
				foreach (var m in moves) {
					Console.Write(m.PrintMove() + " ");
				}
				Console.Write("\nEnter command: ");
				var command = Console.ReadLine();
				switch (command) {
					case "quit":
						return;
					case "go":
						b.MakeMove(player.GetMove());
						break;
					case "undo":
						b.unMakeMove();
						break;
					default:
						Move move = null;
						if (command.Length == 2) {
							move = new Move((command.ToCharArray()[0] - 'a') + (command.ToCharArray()[1] - '1')*7);
						} else if (command.Length == 4) {
							move = new MoveMove(
								(command.ToCharArray()[0] - 'a') + (command.ToCharArray()[1] - '1')*7,
								(command.ToCharArray()[2] - 'a') + (command.ToCharArray()[3] - '1')*7);
						}
						if (move == null || !Board.IsLegal(moves, move)) {
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
