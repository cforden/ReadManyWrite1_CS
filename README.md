Demos the multithreading design pattern of allowing many threads to read an object simultaneously or, 
when modifying that object, allowing only a single thread to write to it.

There is a main branch that uses C# lock statements.
There is a ReaderWriterLockSlim branch that uses 
System.Threading.ReaderWriterLockSlim which provides the same functionality.

See also similar demos I wrote in other languages:
- C#:  https://github.com/cforden/ReadManyWrite1_py
- C++  https://github.com/cforden/ReadManyWrite1_Cpp
