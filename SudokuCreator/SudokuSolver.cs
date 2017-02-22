using SudokuUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuGame {

    public class SudokuSolver : SudokuBase {
        protected int ATTEMPTS { get; set; }
        protected HashSet<Point> Coords { get; set; }

        public SudokuSolver( SudokuBase sdk ) : base( sdk.Grid ) {
            this.Coords = new HashSet<Point>();
            this.New( this.Grid );
        }

        public SudokuSolver() : base() {
            this.Coords = new HashSet<Point>();
            this.New();
        }

        protected void New( Cell[ , ] grid ) {
            this.ATTEMPTS = 0;
            this.Coords.Clear();
            foreach( Cell c in this.Grid ) {
                c.InitAdj( this.Row( c.Coordinate ).Union( this.Column( c.Coordinate ) ).Union( this.Square( c.Coordinate ) ) );
                if( c.Current >= 0 ) {
                    c.Permanent = true;
                } else {
                    c.Update();
                    this.Coords.Add( c.Coordinate );
                }
            }
        }

        protected void New() {
            this.ATTEMPTS = 0;
            this.Coords.Clear();
            for( int Row = 0; Row < 9; ++Row ) {
                for( int Column = 0; Column < 9; ++Column ) {
                    this.Grid[ Row , Column ] = new Cell( new Point( Row , Column ) );
                    this.Coords.Add( this.Grid[ Row , Column ].Coordinate );
                }
            }
            foreach( Cell c in this.Grid ) {
                c.InitAdj( this.Row( c.Coordinate ).Union( this.Column( c.Coordinate ) ).Union( this.Square( c.Coordinate ) ) );
            }
        }

        public void RemoveAt( Point picked , int Actual ) {
            foreach( Cell c in this[ picked ].Adjacency ) {
                if( c.Coordinate != picked && c.Current == Actual ) {
                    if( this[ c.Coordinate ].Change( -1 ) ) {
                        Coords.Add( c.Coordinate );
                    }
                }
            }
        }

        public void RemoveAt( Point picked , int Actual , bool depth ) {
            if( !depth ) {
                this.RemoveAt( picked , Actual );
            } else {
                foreach( Cell c in this.Grid ) {
                    if( c.Current == Actual ) {
                        if( this[ c.Coordinate ].Change( -1 ) ) {
                            Coords.Add( c.Coordinate );
                        }
                    }
                }
            }
        }

        public void ReplaceAt( Point picked ) {
            var col = SudokuExt.Numbers.Except(this.Column(picked).Select(x => x.Current));
            var square = SudokuExt.Numbers.Except(this.Square(picked).Select(x => x.Current));
            var row = SudokuExt.Numbers.Except(this.Row(picked).Select(x => x.Current));
            var list = col.Union(square).Union(row);
            int? Attempted = list.Count() > 0 ? list.Random() : (int?)null;
            Console.ForegroundColor = ConsoleColor.Yellow;
            if( list.Count() > 0 ) {
                bool deep = false;
                if( list.Count() <= 2 && ++this.ATTEMPTS >= 20 ) {
                    var templist = SudokuExt.Numbers.Except(list);
                    var randomint = templist.Random();
                    list = list.Union( templist.Where( x => x == randomint ) );
                    deep = true;
                    this.ATTEMPTS = 0;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                foreach( var value in list ) {
                    this.RemoveAt( picked , value , deep );
                }
                if( this[ picked ].Change( Attempted.Value ) ) {
                    this.Coords.Remove( picked );
                }
            }
        }

        public Sudoku Solve() {
            Point picked;
            while( this.Coords.Count > 0 ) {
                SudokuExt.Speed = 1000 * ( 81 - this.Coords.Count ) / 800;
                var list = this.Coords.Where(x=>this[x].Possible.Count > 0 );
                if( list.Count() > 0 ) {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    if( this[ picked = list.Random() ].PickRandom() == -1 || !this.isValid( picked ) ) {
                        this[ picked ].Change( -1 );
                    } else {
                        this.Coords.Remove( picked );
                    }
                } else {
                    this.ReplaceAt( this.Coords.Random() );
                    continue;
                }
            }

            Console.SetCursorPosition( SudokuExt.ConsolePoint.X - 1 , SudokuExt.ConsolePoint.Y - 1 );
            if( this.AllValid() ) {
                return new Sudoku( this );
            }
            return null;
        }

        public Sudoku Solve( Cell[ , ] grid ) {
            this.New( grid );
            return this.Solve();
        }
    }
}