using Newtonsoft.Json;
using SudokuGame;
using System;

namespace SudokuCreator {

    internal class Program {

        private static void Main( string[ ] args ) {
            JsonSerializer jserial = new JsonSerializer();

            Console.WriteLine( Convert.ToUInt32( "1234" ).ToString( "X2" ) );
            Console.SetWindowSize( 30 , 16 );
            Console.ForegroundColor = ConsoleColor.White;
            jserial.NullValueHandling = NullValueHandling.Ignore;
            Console.WriteLine( "Input From a Website:" );
            SudokuInput sin = new SudokuInput();
            sin.Solve();
            //jserial.Serialize( Console.Out , sdk.Column( 1 , 1 ) );
            Console.ReadKey();
        }
    }
}