using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                lock (this)
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
            lock (this)
            {
                readerCount++;
            }
            bool dataReadWasValid = unprotectedReverser.ReadTest();
            lock (this)
            {
                readerCount--;
            }
            return dataReadWasValid;
        }
    }
}
