using System.Collections.Generic;

namespace Roomies
{
    public class Optiuni
    {
        public static List<string> ZonePreferate { get; } = new()
        {
            "Grigorescu",
            "Manastur",
            "Dambul Rotund",
            "Centru",
            "Marasti",
            "Andrei Muresanu",
            "Iris",
            "Someseni",
            "Gheorgheni",
            "Borhanci",
            "Zorilor",
            "Buna Ziua",
            "Floresti"
        };
        public static List<string> Genuri { get; } = new()
        {
            "Masculin",
            "Feminin",
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
            "Fara galagie seara"
        };

    }
}
