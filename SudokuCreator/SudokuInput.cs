using SudokuUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGame {

    public class SudokuInput : SudokuSolver {
        protected Point Cursor;

        public SudokuInput() : base() {
            Console.Write( this );
            Console.SetCursorPosition( SudokuExt.ConsolePoint.X , SudokuExt.ConsolePoint.Y );
            bool stop = false;
            this.Cursor = new Point( 0 , 8 );
            while( !stop ) {
                switch( Console.ReadKey( true ).Key ) {
                    case ConsoleKey.UpArrow:
                        this.Cursor.Y += ( this.Cursor.Y < 8 ) ? 1 : 0;

                        break;

                    case ConsoleKey.DownArrow:
                        this.Cursor.Y -= ( this.Cursor.Y > 0 ) ? 1 : 0;
                        break;

                    case ConsoleKey.LeftArrow:
                        this.Cursor.X -= ( this.Cursor.X > 0 ) ? 1 : 0;
                        break;

                    case ConsoleKey.RightArrow:
                        this.Cursor.X += ( this.Cursor.X < 8 ) ? 1 : 0;

                        break;

                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        if( !this[ this.Cursor ].Adjacency.Select( x => x.Current ).Contains( 1 ) ) {
                            this[ this.Cursor ].Permanent = true;
                            this[ this.Cursor ].Current = 1;
                        }

                        break;

                    case ConsoleKey.NumPad2:
                    case ConsoleKey.D2:
                        if( !this[ this.Cursor ].Adjacency.Select( x => x.Current ).Contains( 2 ) ) {
                            this[ this.Cursor ].Permanent = true;
                            this[ this.Cursor ].Current = 2;
                        }
                        break;

                    case ConsoleKey.NumPad3:
                    case ConsoleKey.D3:
                        if( !this[ this.Cursor ].Adjacency.Select( x => x.Current ).Contains( 3 ) ) {
                            this[ this.Cursor ].Permanent = true;
                            this[ this.Cursor ].Current = 3;
                        }
                        break;

                    case ConsoleKey.NumPad4:
                    case ConsoleKey.D4:
                        if( !this[ this.Cursor ].Adjacency.Select( x => x.Current ).Contains( 4 ) ) {
                            this[ this.Cursor ].Permanent = true;
                            this[ this.Cursor ].Current = 4;
                        }
                        break;

                    case ConsoleKey.NumPad5:
                    case ConsoleKey.D5:
                        if( !this[ this.Cursor ].Adjacency.Select( x => x.Current ).Contains( 5 ) ) {
                            this[ this.Cursor ].Permanent = true;
                            this[ this.Cursor ].Current = 5;
                        }
                        break;

                    case ConsoleKey.NumPad6:
                    case ConsoleKey.D6:
                        if( !this[ this.Cursor ].Adjacency.Select( x => x.Current ).Contains( 6 ) ) {
                            this[ this.Cursor ].Permanent = true;
                            this[ this.Cursor ].Current = 6;
                        }

                        break;

                    case ConsoleKey.NumPad7:
                    case ConsoleKey.D7:
                        if( !this[ this.Cursor ].Adjacency.Select( x => x.Current ).Contains( 7 ) ) {
                            this[ this.Cursor ].Permanent = true;
                            this[ this.Cursor ].Current = 7;
                        }
                        break;

                    case ConsoleKey.NumPad8:
                    case ConsoleKey.D8:
                        if( !this[ this.Cursor ].Adjacency.Select( x => x.Current ).Contains( 8 ) ) {
                            this[ this.Cursor ].Permanent = true;
                            this[ this.Cursor ].Current = 8;
                        }
                        break;

                    case ConsoleKey.NumPad9:
                    case ConsoleKey.D9:
                        if( !this[ this.Cursor ].Adjacency.Select( x => x.Current ).Contains( 9 ) ) {
                            this[ this.Cursor ].Permanent = true;
                            this[ this.Cursor ].Current = 9;
                        }
                        break;

                    case ConsoleKey.Delete:
                    case ConsoleKey.Backspace:
                        this[ this.Cursor ].Permanent = false;
                        this[ this.Cursor ].Current = -1;

                        break;

                    case ConsoleKey.Enter:
                        stop = true;
                        break;
                }
                Console.SetCursorPosition( SudokuExt.ConsolePoint.X + ( this.Cursor.X * 3 ) + this.Cursor.X / 3 , SudokuExt.ConsolePoint.Y + ( 8 - this.Cursor.Y ) + ( 8 - this.Cursor.Y ) / 3 );
            }
            this.New( this.Grid );
            Console.SetCursorPosition( SudokuExt.ConsolePoint.X - 1 , SudokuExt.ConsolePoint.Y - 1 );
            SudokuExt.ConsolePoint = new Point( Console.CursorLeft + 1 , Console.CursorTop + 1 );
        }
    }
}