using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReadManyWriteOne
{
    /// <summary>
    /// Adds thread safety to the simulation of reading and writing an arbitrary object
    /// </summary>
    internal class StringReverserThdSafe
    {
        private StringReverser unprotectedReverser = new StringReverser();
        private int readerCount = 0; // How many threads are still trying to read the data?

        public int ErrorCount
        {
            get
            {
                return unprotectedReverser.ErrorCount;
            }
        }

        public StringReverserThdSafe()
        {
        }

        public string GetString()
        {
            return unprotectedReverser.GetString();
        }

        public void Write()
        {
            bool reading = true;
            while (reading)
            {
                lock (unprotectedReverser)
                {
                    if (readerCount == 0)
                    {
                        reading = false;
                        unprotectedReverser.Write();
                    }
                }
            }
        }

        public bool ReadTest()
        {
            lock (unprotectedReverser)
            {
                readerCount++;
                DbgReaderCountStates();
            }
            bool dataReadWasValid = unprotectedReverser.ReadTest();
            lock (unprotectedReverser)
            {
                readerCount--;
            }
            return dataReadWasValid;
        }

        long[] dbgCountStates;

        /// <summary>
        /// Counts the number of times this many (readerCount) threads were simultaneously trying to read
        /// </summary>
        private void DbgReaderCountStates()
        {
            DbgInitReaderCounts();
            dbgCountStates[readerCount]++;
        }

        /// <summary>
        /// Initializes the array of counts of reader threads encountered, if not init'd yet.
        /// </summary>
        private void DbgInitReaderCounts()
        {
            if (null == dbgCountStates || dbgCountStates.Length == 0)
            {
                dbgCountStates = new long[Program.MaxReaders + 3];
            }
        }

        public void PrintReaderCountStates()
        {
            DbgInitReaderCounts();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"Number of Readers     Times encountered");
            for (int countIdx = 0; countIdx < Program.MaxReaders + 2; countIdx++)
            {
                sb.AppendFormat($"\n{countIdx,17:N0}{dbgCountStates[countIdx],22:N0}");
            }
            sb.AppendFormat($"\n");

            Console.WriteLine($"\n{sb}\n");
        }

    }
}
