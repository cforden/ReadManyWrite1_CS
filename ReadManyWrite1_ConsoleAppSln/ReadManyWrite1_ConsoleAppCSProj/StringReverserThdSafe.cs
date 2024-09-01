using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        AutoResetEvent areFinishedReading = new AutoResetEvent(false);
        ManualResetEvent allowReading = new ManualResetEvent(true);

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
            int dbgLocalCountWriteAttempts = 0;
            bool reading = true;
            bool writeRequested = false;
            while (reading)
            {
                ++dbgLocalCountWriteAttempts;
                lock (unprotectedReverser)
                {
                    if (readerCount == 0)
                    {
                        DbgWriteAttempts(dbgLocalCountWriteAttempts);
                        reading = false;
                        unprotectedReverser.Write();
                        writeRequested = false;
                    }
                    else
                    {
                        writeRequested = true;
                    }
                }
                if (writeRequested)
                {
                    allowReading.Reset();
                    areFinishedReading.WaitOne();
                }
            }
            allowReading.Set();
        }

        public bool ReadTest()
        {
            allowReading.WaitOne();
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
            if (readerCount == 0)
            {
                areFinishedReading.Set();
            }
            return dataReadWasValid;
        }

        #region Read Stats

        private long[] dbgCountStates = new long[10];
        private object readerStatsLock = new();

        /// <summary>
        /// Counts the number of times this many (readerCount) threads were simultaneously trying to read
        /// </summary>
        private void DbgReaderCountStates()
        {
#       if DEBUG
            lock (readerStatsLock)
            {
                DbgInitReaderCounts();
                dbgCountStates[readerCount]++;
            }
#       endif
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

        /// <summary>
        /// Prints stats about the number of threads simultaneously trying to read
        /// </summary>
        public void DbgPrintReaderCountStates()
        {
#       if DEBUG
            DbgInitReaderCounts();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"Number of Readers     Times encountered");
            for (int countIdx = 0; countIdx < Program.MaxReaders + 2; countIdx++)
            {
                sb.AppendFormat($"\n{countIdx,17:N0}{dbgCountStates[countIdx],22:N0}");
            }
            Console.WriteLine($"\n{sb}\n");
#       endif
        }

        #endregion Reader Stats
        #region Writer threads stats

        private SortedDictionary<int, long> dbgWriteAttemptsOccurances = new();
        private long dbgMaxWriteLockAttempts = 0;
        private object dbgWriteStatsLock = new object();

        /// <summary>
        /// Increments the number of times this.Write() has attempted to get a lock on the unprotected data object 
        /// (such as StringReverser) writeAttempts times.
        /// </summary>
        /// <param name="writeAttempts">How many times this.Write() had to attempt 
        /// to lock the unprotected data object in order to get that lock</param>
        private void DbgWriteAttempts(int writeAttempts)
        {
#       if DEBUG
            lock (dbgWriteStatsLock)
            {
                if (dbgMaxWriteLockAttempts < writeAttempts)
                {
                    dbgMaxWriteLockAttempts = writeAttempts;
                }

                if (dbgWriteAttemptsOccurances.TryGetValue(writeAttempts, out long occurances))
                {
                    dbgWriteAttemptsOccurances[writeAttempts] = occurances + 1;
                }
                else
                {
                    if (dbgWriteAttemptsOccurances.Count() < 50)
                    {
                        dbgWriteAttemptsOccurances[writeAttempts] = 1;
                    }
                }
            }
#       endif
        }

        /// <summary>
        /// Prints stats about how many times this.Write() had to attempt to get a lock
        /// </summary>
        public void DbgPrintWriteLockAttempts()
        {
#       if DEBUG
            // Print statement only useful for debugging when this.Write() spins a huge # of times:
            // Console.WriteLine($"Max number of times Write() had to try to acquire lock: {dbgMaxWriteLockAttempts}");

            Console.WriteLine($"");
            Console.WriteLine($"Write statistics:");
            Console.WriteLine($"-----------------");

            long totalOccurances = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"# lock attempts.  Occurances of that #.");
            foreach (KeyValuePair<int, long> kvp in dbgWriteAttemptsOccurances)
            {
                sb.AppendFormat($"\n{kvp.Key,15:N0}{kvp.Value,23:N0}");
                totalOccurances += kvp.Value;
            }
            Console.WriteLine($"{sb}\n");
            Console.WriteLine($"Total occurances: {totalOccurances,20:N0}\n");
#       endif
        }

        #endregion Writer threads stats
    }
} // namespace ReadManyWriteOne
