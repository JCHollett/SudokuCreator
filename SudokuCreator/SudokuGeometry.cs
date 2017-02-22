using SudokuUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace SudokuGame {

    [Serializable]
    public struct Point : ISerializable {
        internal int X;
        internal int Y;

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

        public void GetObjectData( SerializationInfo info , StreamingContext context ) {
            info.AddValue( "X" , this.X , typeof( int ) );
            info.AddValue( "Y" , this.Y , typeof( int ) );
        }
    }

    [Serializable]
    public class Cell : ISerializable, ICloneable {
        public Point Coordinate {
            get;
            private set;
        }
        public HashSet<int> Possible {
            get;
            private set;
        }
        public int cVal;
        public int Current {
            get {
                return this.cVal;
            }
            set {
                Console.SetCursorPosition( SudokuExt.ConsolePoint.X + ( this.Coordinate.X * 3 ) + this.Coordinate.X / 3 , SudokuExt.ConsolePoint.Y + ( 8 - this.Coordinate.Y ) + ( 8 - this.Coordinate.Y ) / 3 );
                this.cVal = value;
                if( value == -1 ) {
                    Console.Write( ' ' );
                } else {
                    Thread.Sleep( SudokuExt.Speed );
                    if( this.Permanent ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write( value );
                        Console.ForegroundColor = ConsoleColor.White;
                    } else {
                        Console.Write( value );
                    }
                }
            }
        }
        public bool Permanent {
            get;
            internal set;
        }
        internal HashSet<Cell> Adjacency {
            get;
            set;
        }

        public Cell() : this( new Point( -1 , -1 ) ) {
        }

        public Cell( Point p ) {
            this.Coordinate = p;
            this.cVal = -1;
            this.Permanent = false;
            this.Adjacency = new HashSet<Cell> { this };
            this.Possible = new HashSet<int>( SudokuExt.Numbers ) { };
        }

        public bool Change( int attempt ) {
            if( this.Permanent ) {
                return false;
            }
            if( attempt == -1 ) {
                this.Current = -1;
                this.UpdateAdjaceny( -1 );
                return true;
            }
            if( this.Possible.Count > 0 ) {
                if( this.Possible.Contains( attempt ) ) {
                    this.Current = attempt;
                    foreach( var cell in this.Adjacency ) {
                        cell.UpdateAdjaceny( attempt );
                    }
                    return true;
                }
                return false;
            } else {
                this.UpdateAdjaceny( attempt );
                if( this.Possible.Contains( attempt ) ) {
                    this.Current = attempt;
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
            this.Possible.UnionWith( SudokuExt.Numbers.Except( this.Adjacency.Select( x => x.Current ) ) );
        }

        public int PickRandom() {
            int Temp = this.Possible.Random();
            if( this.Change( Temp ) ) {
                return Temp;
            } else {
                return -1;
            }
        }

        public void InitAdj( IEnumerable<Cell> Adj ) {
            foreach( var cell in Adj ) this.Adjacency.Add( cell );
        }

        public static bool operator ==( Cell A , Cell B ) {
            object a = A;
            object b = B;
            if( a == null || b == null ) {
                if( a != null )
                    return false;
                if( b != null )
                    return false;
                return true;
            }
            return A.Coordinate == B.Coordinate;
        }

        public static bool operator !=( Cell A , Cell B ) {
            return !( A == B );
        }

        public override Boolean Equals( Object obj ) {
            if( obj == null ) {
                return false;
            } else {
                Cell other = obj as Cell;
                return this == other;
            }
        }

        public override Int32 GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return string.Format( "[{0}->{1}]" , this.Coordinate , this.Current );
        }

        public void GetObjectData( SerializationInfo info , StreamingContext context ) {
            info.AddValue( "Actual" , this.Current , typeof( int ) );
            info.AddValue( "Coordinate" , this.Coordinate , typeof( Point ) );
            //info.AddValue( "Possible" , this.Possible , typeof( HashSet<int> ) );
        }

        internal bool Update() {
            return this.Change( this.Current );
        }

        public Object Clone() {
            var clone = new Cell(this.Coordinate);
            clone.cVal = this.cVal;
            clone.Permanent = this.Permanent;
            clone.Possible.IntersectWith( this.Possible );
            return clone;
        }
    }
}