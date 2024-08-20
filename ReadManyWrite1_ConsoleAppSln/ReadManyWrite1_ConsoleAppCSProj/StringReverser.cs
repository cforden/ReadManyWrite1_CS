using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadManyWriteOne
{
    /// <summary>
    /// Simulates reading and writing an arbitrary object
    /// </summary>
    internal class StringReverser
    {
        private const string theOriginalString = "This is the forward string";
        private readonly string theReversedString;

        /// Gets reversed in place, and inspected through public getter
        private char[] theStringAsChars = theOriginalString.ToCharArray();

        public int ErrorCount { get; set; } = 0;

        public StringReverser()
        {
            char[] tempCharsToReverse = new char[theOriginalString.Length];
            tempCharsToReverse = theOriginalString.ToCharArray();
            Reverse(tempCharsToReverse);
            theReversedString = new string (tempCharsToReverse);
        }

        public string GetString()
        {
            return new string (theStringAsChars);
        }

        /// <summary>
        /// Simulates writing by modifying private data
        /// </summary>
        public void Write()
        {
            Reverse(theStringAsChars);
        }

        /// <summary>
        /// Reverses the string in-place then returns a copy
        /// </summary>
        /// <param name="toReverse"></param>
        /// <returns>A copy of the object passed in, but whose chars have been re-ordered</returns>
        private void Reverse(char[] toReverse)
        {
            char[] original = new char[toReverse.Length];
            toReverse.CopyTo(original, 0);
            for (int i = 0; i < toReverse.Length; i++)
            {
                toReverse[i] = original[original.Length -1 - i];
            }
        }

        /// <summary>
        /// Both simulate a read and check whether the data is corrupt
        /// </summary>
        /// <returns></returns>
        public bool ReadTest()
        {
            // Is it correctly forward?
            string tempString = GetString();
            if (theOriginalString == tempString)
            {
                return true;
            }
            if (theReversedString == tempString)
            {
                return true;
            }
            ErrorCount++;
            return false;
        }
    }
}
