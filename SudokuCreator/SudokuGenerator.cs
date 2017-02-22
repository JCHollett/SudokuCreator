using SudokuUtils;
using System.Linq;

namespace SudokuGame {

    public class SudokuGenerator : SudokuSolver {

        public SudokuGenerator() : base() {
        }

        public Sudoku Create() {
            this.New();
            var sdk = this.Solve();
            var N = 81;
            var cells = ToEnumerable( sdk.Grid ).ToList();
            while( N > 20 ) {
                sdk[ cells.RemoveRand().Coordinate ].Current = -1;
                --N;
            }
            return sdk;
        }
    }
}