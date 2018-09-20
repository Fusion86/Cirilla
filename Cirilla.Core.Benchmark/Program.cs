using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.IO;

namespace Cirilla.Core.Benchmark
{
    [MemoryDiagnoser]
    public class ByteSwapBenchmark
    {
        private byte[] bytes;

        public ByteSwapBenchmark()
        {
            bytes = File.ReadAllBytes(@"C:/Steam/userdata/112073240/582010/remote/SAVEDATA1000");
        }

        [Benchmark]
        public void ByteSwap()
        {
            bytes = SwapBytes(bytes);

            byte[] SwapBytes(byte[] bytes)
            {
                var swapped = new byte[bytes.Length];
                for (var i = 0; i < bytes.Length; i += 4)
                {
                    swapped[i] = bytes[i + 3];
                    swapped[i + 1] = bytes[i + 2];
                    swapped[i + 2] = bytes[i + 1];
                    swapped[i + 3] = bytes[i];
                }
                return swapped;
            }
        }

        [Benchmark]
        public void ByteSwapUnsafeRef()
        {
            SwapBytes(ref bytes);

            unsafe void SwapBytes(ref byte[] arr)
            {
                fixed (byte* ptr = arr)
                {
                    byte tmp;
                    for (int i = 0; i < arr.Length; i += 4)
                    {
                        // Swap 0 and 3
                        tmp = ptr[i];
                        ptr[i] = ptr[i + 3];
                        ptr[i + 3] = tmp;

                        // Swap 1 and 2
                        tmp = ptr[i + 1];
                        ptr[i + 1] = ptr[i + 2];
                        ptr[i + 2] = tmp;
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ByteSwapBenchmark>();
        }
    }
}
