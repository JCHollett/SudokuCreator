using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuCreator {

    public static class SudokuExt {
        public static List<int> Numbers = new List<int>{ 1,2,3,4,5,6,7,8,9 };
        public static Random rand = new Random((int)( DateTime.Now.Ticks));

        public static T Remove<T>( this IList<T> src , int index ) {
            if( index >= src.Count || index < 0 ) throw new IndexOutOfRangeException( "Index out of range" );
            T temp = src.ElementAt(index);
            src.RemoveAt( index );
            return temp;
        }

        public static T Random<T>( this IEnumerable<T> src ) {
            if( src.Count() == 0 ) throw new InvalidOperationException( "Sequence contains no elements" );
            return src.ElementAt( rand.Next( 0 , src.Count() ) );
        }

        public static T RemoveRand<T>( this IList<T> src ) {
            if( src.Count() == 0 ) throw new InvalidOperationException( "Sequence contains no elements" );
            return src.Remove( rand.Next( 0 , src.Count() ) );
        }

        public static IList<T> Randomize<T>( this IList<T> list ) {
            var len = list.Count();
            var temp = new List<T> (list);
            T swap = default(T);
            int j = 0;
            for( int i = 0; i < len - 1; ++i ) {
                j = rand.Next( i , len );
                swap = temp[ i ];
                temp[ i ] = temp[ j ];
                temp[ j ] = swap;
            }
            return temp;
        }
    }

    public class Sudoku {
        private Cell[,] Grid;
        private int N = 0;
        private int ATTEMPTS = 0;
        private HashSet<Point> Coords;
        public Cell this[ Point C ] { get { return this.Grid[ C.X , C.Y ]; } set { this.Grid[ C.X , C.Y ] = value; } }

        public Sudoku() {
            this.Grid = new Cell[ 9 , 9 ];
            this.Coords = new HashSet<Point>();
            for( int i = 0; i < 9; ++i ) {
                for( int j = 0; j < 9; ++j ) {
                    this.Grid[ i , j ] = new Cell( new Point( i , j ) );
                    this.Coords.Add( this.Grid[ i , j ].Coordinate );
                }
            }
            foreach( Cell c in this.Grid ) {
                var union = this.Row( c.Coordinate ).Union( this.Column( c.Coordinate ) ).Union( this.Square( c.Coordinate ) );
                this[ c.Coordinate ].InitAdj( union );
            }
        }

        public IEnumerable<Cell> Square( int x , int y ) {
            int x_o = 3*(x / 3);
            int y_o = 3*(y / 3);
            for( int i = x_o; i < x_o + 3; ++i ) {
                for( int j = y_o; j < y_o + 3; ++j ) {
                    yield return Grid[ i , j ];
                }
            }
        }

        public IEnumerable<Cell> Square( Point p ) {
            return Square( p.X , p.Y );
        }

        public IEnumerable<Cell> Row( int x , int y ) {
            for( int i = 0; i < 9; ++i ) {
                yield return Grid[ i , y ];
            }
        }

        public IEnumerable<Cell> Row( Point p ) {
            return Row( p.X , p.Y );
        }

        public IEnumerable<Cell> Column( int x , int y ) {
            for( int j = 0; j < 9; ++j ) {
                yield return Grid[ x , j ];
            }
        }

        public IEnumerable<Cell> Column( Point p ) {
            return Column( p.X , p.Y );
        }

        public void Hide() {
            while( N > 33 ) {
                N--;
            }
        }

        public bool isValidRow( Point p ) {
            int x = 0;
            foreach( Cell s in this.Row( p ) ) {
                if( s.Actual != 0 ) {
                    if( ( x & ( 1 << s.Actual ) ) == 0 ) {
                        x = x | ( 1 << s.Actual );
                    } else {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool isValidSquare( Point p ) {
            int x = 0;
            foreach( Cell s in this.Square( p ) ) {
                if( s.Actual != 0 ) {
                    if( ( x & ( 1 << s.Actual ) ) == 0 ) {
                        x = x | ( 1 << s.Actual );
                    } else {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool isValidColumn( Point p ) {
            int x = 0;
            foreach( Cell s in this.Column( p ) ) {
                if( s.Actual != 0 ) {
                    if( ( x & ( 1 << s.Actual ) ) == 0 ) {
                        x = x | ( 1 << s.Actual );
                    } else {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool isValid( Point p ) {
            bool result = this.isValidColumn( p ) && this.isValidRow( p ) && this.isValidSquare( p );
            return result;
        }

        public void RemoveAt( Point picked , int Actual ) {
            foreach( Cell c in this[ picked ].Adjacency ) {
                if( c.Coordinate != picked && c.Actual == Actual ) {
                    this[ c.Coordinate ].Change( 0 );
                    Coords.Add( c.Coordinate );
                    N--;
                }
            }
        }

        public void RemoveAt( Point picked , int Actual , bool depth ) {
            if( !depth ) {
                this.RemoveAt( picked , Actual );
            } else {
                foreach( Cell c in this.Grid ) {
                    if( c.Actual == Actual ) {
                        this[ c.Coordinate ].Change( 0 );
                        Coords.Add( c.Coordinate );
                        N--;
                    }
                }
            }
        }

        public void ReplaceAt( Point picked ) {
            var col = SudokuExt.Numbers.Where(z=> this.Column(picked).Select(x => x.Actual).Intersect(SudokuExt.Numbers).All(y => z != y));
            var square = SudokuExt.Numbers.Where(z=> this.Square(picked).Select(x => x.Actual).Intersect(SudokuExt.Numbers).All(y => z != y));
            var row = SudokuExt.Numbers.Where(z=> this.Row(picked).Select(x => x.Actual).Intersect(SudokuExt.Numbers).All(y => z != y));
            var list = col.Union(square).Union(row);
            int? Actual = list.Count() > 0 ? list.Random() : (int?)null;
            if( list.Count() > 0 ) {
                bool deep = false;
                if( list.Count() <= 2 && ++this.ATTEMPTS >= 20 ) {
                    var templist = SudokuExt.Numbers.Except(list);
                    var randomint = templist.Random();
                    list = list.Union( templist.Where( x => x == randomint ) );
                    deep = true;
                    this.ATTEMPTS = 0;
                }
                foreach( var value in list ) {
                    this.RemoveAt( picked , value , deep );
                }
                this[ picked ].Change( Actual.Value );
                this.Coords.Remove( picked );
                N++;
            }
        }

        public bool Solve() {
            int i = 0;
            Point picked;
            while( N < 81 ) {
                var list = this.Coords.Where(x=>this[x].Possible.Count > 0 );
                if( list.Count() > 0 ) {
                    if( this[ picked = list.Random() ].PickRandom() == 0 || !this.isValid( picked ) ) {
                        this[ picked ].Change( 0 );
                    } else {
                        this.Coords.Remove( picked );
                        N++;
                    }
                } else {
                    this.ReplaceAt( this.Coords.Random() );
                    continue;
                }
            }

            return this.AllValid();
        }

        public string ToHidden() {
            string str = "\n";
            for( int i = 8; i >= 0; --i ) {
                str += "[" + string.Join( "][" , Row( 0 , i ).Select( x => { return SudokuExt.rand.NextDouble() > 0.65 ? x.Actual.ToString() : " "; } ) ) + "]\n";
            }
            return str += "\n";
        }

        public override string ToString() {
            string str = "\n";
            for( int i = 8; i >= 0; --i ) {
                str += "[" + string.Join( "][" , Row( 0 , i ).Select( x => x.Actual ) ) + "]\n";
            }
            return str;
        }

        public Boolean AllValid() {
            foreach( var Cell in this.Grid ) {
                if( !this.isValid( Cell.Coordinate ) ) {
                    return false;
                }
            }
            return true;
        }
    }

    public struct Point {
        public int X { get; set; }
        public int Y { get; set; }

        public Point( int x , int y ) {
            this.X = x;
            this.Y = y;
        }

        public static bool operator ==( Point A , Point B ) {
            return A.X == B.X && A.Y == B.Y;
        }

        public static bool operator !=( Point A , Point B ) {
            return !( A == B );
        }

        public override Boolean Equals( Object obj ) {
            if( obj is Point ) {
                Point B = (Point) obj;
                return this == B;
            } else {
                return false;
            }
        }

        public override Int32 GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return string.Format( "({0},{1})" , this.X , this.Y );
        }
    }

    public class Cell {
        public Point Coordinate;
        public HashSet<int> Possible;
        public HashSet<Cell> Adjacency;
        public int Actual { get; set; }

        public Cell( Point p ) {
            this.Coordinate = p;
            this.Actual = 0;
            this.Adjacency = new HashSet<Cell>();
            this.Possible = new HashSet<int>() { 1 , 2 , 3 , 4 , 5 , 6 , 7 , 8 , 9 };
            this.Adjacency.Add( this );
        }

        public bool Change( int attempt ) {
            if( attempt == 0 ) {
                this.Actual = 0;
                this.UpdateAdjaceny( 0 );
                return true;
            }
            if( this.Possible.Count > 0 ) {
                if( this.Possible.Contains( attempt ) ) {
                    this.Actual = attempt;
                    foreach( var cell in this.Adjacency ) {
                        cell.UpdateAdjaceny( attempt );
                    }
                    return true;
                }
                return false;
            } else {
                this.UpdateAdjaceny( attempt );
                if( this.Possible.Contains( attempt ) ) {
                    this.Actual = attempt;
                    foreach( var cell in this.Adjacency ) {
                        cell.UpdateAdjaceny( attempt );
                    }
                    return true;
                }
            }
            return false;
        }

        public void UpdateAdjaceny( int attempt ) {
            this.Possible.Remove( attempt );
            this.Possible.UnionWith( SudokuExt.Numbers.Except( this.Adjacency.Select( x => x.Actual ) ) );
        }

        public int PickRandom() {
            int Temp = this.Possible.Random();
            if( this.Change( Temp ) ) {
                return Temp;
            } else {
                return 0;
            }
        }

        public void InitAdj( IEnumerable<Cell> Adj ) {
            foreach( var cell in Adj ) this.Adjacency.Add( cell );
        }

        public override string ToString() {
            return string.Format( "[{0}->{1}]" , this.Coordinate , this.Actual );
        }
    }

    internal class Program {

        private static void Main( string[ ] args ) {
            DateTime before = DateTime.Now;
            for( int i = 0; i < 10; ++i ) {
                DateTime During = DateTime.Now;
                Sudoku sdk = new Sudoku();
                sdk.Solve();
                Console.WriteLine( sdk );
                Console.WriteLine( DateTime.Now - During );
            }
            Console.WriteLine( "Total: {0}" , DateTime.Now - before );

            Console.ReadKey();
        }
    }
}