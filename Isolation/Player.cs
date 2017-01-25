using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3 {
	class Player {
		const int INF = int.MaxValue;
		Board board;
		public Player(Board b) {
			board = b;
		}

		int Eval() {
			var curMoves = board.GetLegalMoves(board.Turn);
			var otherMoves = board.GetLegalMoves(board.OtherTurn);

			return curMoves.Length - otherMoves.Length;
		}

		public Move GetMove() {
			var moves = board.GetLegalMoves(board.Turn);
			var best = -INF;
			Move bestMove = null;
			foreach(var m in moves) {
				board.MakeMove(m);
				var score = Eval();
				if(-score > best) {
					bestMove = m;
				}
				board.unMakeMove();
			}
			return bestMove;
		}
	}
}
