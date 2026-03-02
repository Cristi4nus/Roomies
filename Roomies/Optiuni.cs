using System.Collections.Generic;

namespace Roomies
{
    public class Optiuni
    {
        public static List<string> ZonePreferate { get; } = new()
        {
            "Marasti",
            "Iris",
            "Gheorgheni",
            "Zorilor",
            "Manastur",
            "Centru",
            "Grigorescu",
            "Someseni",
            "Buna Ziua",
            "Dambul Rotund",
            "Floresti",
            "Andrei Muresanu",
            "Borhanci"
        };
        public static List<string> Genuri { get; } = new()
        {
            "Masculin",
            "Feminin",
            "Non binar",
            "Altul"
        };


        public static List<string> StiluriDeViata { get; } = new()
        {
            "Introvertit",
            "Extrovertit"
        };

        public static List<string> PreferinteDeTrai { get; } = new()
        {
            "Fara animale",
            "Nefumator",
            "Fara petreceri",
            "Fara galagie seara",
            "Fara oaspeti peste noapte"
        };

    }
}
