using System;
using System.Linq;

namespace Alaris.Irc
{
    /// <summary>
    ///   Generates random, made-up names. The names appear to be language neutral (sort of).
    /// </summary>
    /// <remarks>
    ///   This is a port of the orginal Javascript written by John Ahlschwede, ahlschwede@hotmail.com
    /// </remarks>
    public sealed class NameGenerator
    {
        private readonly int[] _numSyllables = new[] {1, 2, 3, 4, 5};
        private readonly int[] _numSyllablesChance = new[] {150, 500, 80, 10, 1};
        private readonly int[] _numConsonants = new[] {0, 1, 2, 3, 4};
        private readonly int[] _numConsonantsChance = new[] {80, 350, 25, 5, 1};
        private readonly int[] _numVowels = new[] {1, 2, 3};
        private readonly int[] _numVowelsChance = new[] {180, 25, 1};
        private readonly char[] _vowel = new[] {'a', 'e', 'i', 'o', 'u', 'y'};
        private readonly int[] _vowelChance = new[] {10, 12, 10, 10, 8, 2};

        private readonly char[] _consonant = new[]
                                                {
                                                    'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r',
                                                    's', 't', 'v', 'w', 'x', 'y', 'z'
                                                };

        private readonly int[] _consonantChance = new[]
                                                     {
                                                         10, 10, 10, 10, 10, 10, 10, 10, 12, 12, 12, 10, 5, 12, 12, 12, 8,
                                                         8, 3, 4, 3
                                                     };

        private readonly Random _random;

        /// <summary>
        ///   Create an instance.
        /// </summary>
        public NameGenerator()
        {
            _random = new Random();
        }

        private int IndexSelect(int[] intArray)
        {
            var totalPossible = intArray.Aggregate(0, (current, t) => current + t);
            var chosen = _random.Next(totalPossible);
            var chancesSoFar = 0;
            for (var j = 0; j < intArray.Length; j++)
            {
                chancesSoFar = chancesSoFar + intArray[j];
                if (chancesSoFar > chosen)
                {
                    return j;
                }
            }
            return 0;
        }

        private string MakeSyllable()
        {
            return MakeConsonantBlock() + MakeVowelBlock() + MakeConsonantBlock();
        }

        private string MakeConsonantBlock()
        {
            string newName = "";
            int numberConsonants = _numConsonants[IndexSelect(_numConsonantsChance)];
            for (int i = 0; i < numberConsonants; i++)
            {
                newName += _consonant[IndexSelect(_consonantChance)];
            }
            return newName;
        }

        private string MakeVowelBlock()
        {
            string newName = "";
            int numberVowels = _numVowels[IndexSelect(_numVowelsChance)];
            for (int i = 0; i < numberVowels; i++)
            {
                newName += _vowel[IndexSelect(_vowelChance)];
            }
            return newName;
        }

        /// <summary>
        ///   Generates a name randomly using certain construction rules. The name
        ///   will be different each time it is called.
        /// </summary>
        /// <returns>A name string.</returns>
        public string MakeName()
        {
            int numberSyllables = _numSyllables[IndexSelect(_numSyllablesChance)];
            string newName = "";
            for (int i = 0; i < numberSyllables; i++)
            {
                newName = newName + MakeSyllable();
            }
            return char.ToUpper(newName[0]) + newName.Substring(1);
        }
    }
}