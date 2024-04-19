using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Security.Policy;

namespace MechanismSimulator.Classes
{
    internal class EntityRegex
    {
        private static Random rand = new Random();
        private static string[] vowels = { "a", "e", "i", "o", "u" };
        private static string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
        private static string[] planetaryEndings = { "nus", "ter", "ria", "don", "rus", "tus", "tara", "thos", "lon", "ara", "thlon", "thon", " Skibidi" };
        private static string[] starEndings = { "us", "a", "is", "ra", "on", "el", "um", "or", "al", "ix" };

        public static string CalculatePlanetaryRegex() 
        {
            int syllables = rand.Next(1, 4); 
            string name = "";

            for (int i = 0; i < syllables; i++)
            {
                if (rand.Next(2) == 0) 
                {
                    name += consonants[rand.Next(consonants.Length)];
                }
                name += vowels[rand.Next(vowels.Length)];
            }

            name = char.ToUpper(name[0]) + name.Substring(1); 

            name += planetaryEndings[rand.Next(planetaryEndings.Length)]; 

            if (rand.Next(4) == 0) // 25% chance of adding alphanumeric
            {
                name += CalculateHex();
            }
            else if (rand.Next(4) == 0)
            {
                name += CalculateRomanNumerals();
            }

            return name;
        }
        public static string CalculateStarRegex()
        {
            int syllables = rand.Next(1, 4); // Random number of syllables (1-3)
            string name = "";

            for (int i = 0; i < syllables; i++)
            {
                if (rand.Next(2) == 0) // 50% chance of adding a consonant
                {
                    name += consonants[rand.Next(consonants.Length)];
                }
                name += vowels[rand.Next(vowels.Length)];
            }

            name = char.ToUpper(name[0]) + name.Substring(1);

            name += starEndings[rand.Next(starEndings.Length)];

            return name;
        }
        private static string CalculateHex()
        {
            string alphanumeric = " ";
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            for (int i = 0; i < 3; i++)
            {
                alphanumeric += chars[rand.Next(chars.Length)];
            }

            return alphanumeric;
        }
        private static string CalculateRomanNumerals()

        {
            Dictionary<int, string> romanNumerals = new Dictionary<int, string>
            {
                {1,"I"},
                {4,"IV"},
                {5,"V"},
                {9,"IX"},
                {10,"X"},
                {40,"XL"},
                {50,"L"},
                {90,"XC"},
                {100,"C"},
                {400,"CD"},
                {500,"D"},
                {900,"CM"},
                {1000,"M"}
            };
            int denarynum = rand.Next(1, 31);
            string romanNumeral = " ";

            foreach (var pair in romanNumerals.Reverse())
            {
                while (denarynum >= pair.Key)
                {
                    romanNumeral += pair.Value;
                    denarynum -= pair.Key;
                }
            }
            return romanNumeral;
        }


    }
}
