Demos the multithreading design pattern of allowing many threads to read an object simultaneously or, when modifying that object, allowing only a single thread to write to it.

Branches of interest:
- There is a main branch that uses C# lock statements.
- There is a ReaderWriterLockSlim branch that uses System.Threading.ReaderWriterLockSlim which provides the same functionality.  This is about 3 times faster but reading seems to take priority over writing.

The Debug config gathers and prints (to Console) statistics on reader and writer activity.  Both Release and Debug configs print progress messages showing interleaving between reading and writing.

See also similar demos I wrote in other languages:
- C#:  https://github.com/cforden/ReadManyWrite1_py
- C++  https://github.com/cforden/ReadManyWrite1_Cpp
