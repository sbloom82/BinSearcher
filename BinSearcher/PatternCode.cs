using System;
using System.Collections.Generic;
using System.Linq;

namespace BinSearcher
{
    public class Helper
    {
        public static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
        {
            var list = new List<int>();

            for (int i = 0; i < source.Length - pattern.Length; i++)
            {
                if (!IsMatch(source, i, pattern))
                    continue;
                list.Add(i);
            }

            return list;
        }

        public static bool IsMatch(byte[] array, int position, byte[] candidate)
        {
            if (candidate.Length > (array.Length - position))
                return false;

            for (int i = 0; i < candidate.Length; i++)
                if (array[position + i] != candidate[i])
                    return false;

            return true;
        }

        public static bool HasMatch(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length - pattern.Length; i++)
            {
                if (!IsMatch(source, i, pattern))
                    continue;
                return true;
            }
            return false;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static bool checkEqual(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;
            bool retval = true;
            for (int i = 0; i < arr1.Length; i++)
            {
                retval &= (arr1[i] == arr2[i]);
            }
            return retval;
        }
    }
}