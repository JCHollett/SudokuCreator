using SudokuGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuUtils {

    public static class SudokuExt {
        public static Point ConsolePoint = new Point(0,0);
        public static List<int> Numbers = new List<int>{ 1,2,3,4,5,6,7,8,9 };
        public static Random rand = new Random((int)( DateTime.Now.Ticks));

        public static Int32 Speed = 0;

        public static T Remove<T>( this IList<T> src , int index ) {
            if( index >= src.Count || index < 0 ) throw new IndexOutOfRangeException( "Index out of range" );
            T temp = src.ElementAt(index);
            src.RemoveAt( index );
            return temp;
        }

        public static IEnumerable<T> Range<T>( this IEnumerable<T> list , int start , int end ) {
            if( list.Count() == 0 ) throw new InvalidOperationException( "Sequence contains no elements" );
            if( start >= 0 && start < list.Count() && end >= start && end <= list.Count() ) {
                for( int i = start; i < end; ++i ) {
                    yield return list.ElementAt( i );
                }
            } else throw new InvalidOperationException( "Sequence contains no elements" );
        }

        public static IEnumerable<T> Limit<T>( this IEnumerable<T> list , int end ) {
            return list.Range( 0 , end );
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
}