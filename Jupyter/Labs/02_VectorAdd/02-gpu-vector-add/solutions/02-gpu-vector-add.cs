using System;
using System.Threading.Tasks;
using Hybridizer.Runtime.CUDAImports;

namespace VectorAdd 
{
    public class Program 
    {
		[EntryPoint]
        public static void Add(float[] dst, float[] a, float[] b, int N) 
        {
            Parallel.For(0, N, i => dst[i] = a[i] + b[i]);
        }
    
        public static void Main() 
        {
            const int N = 1024*1024*256;
            float[] a = new float[N];
            float[] b = new float[N];
            float[] dst = new float[N];
            
            for(int i = 0; i < N; ++i) 
            {
                a[i] = (float) i;
                b[i] = 1.0F;
            }
            
			dynamic wrapped = HybRunner.Cuda().Wrap(new Program()); // create a CUDA runner from current program
			
            wrapped.Add(dst, a, b, N); // invoke method on the GPU
            
			cuda.DeviceSynchronize(); // kernel calls are asynchronous. We need to wait for it to terminate.
			
            for(int i = 0; i < N; ++i) 
            {
                if(dst[i] != (float) i + 1.0F) 
                {
                    Console.Error.WriteLine("ERROR at {0} -- {1} != {2}", i, dst[i], i+1);
                    Environment.Exit(6); // abort
                }
            }
            
            Console.Out.WriteLine("OK");
        }
    }
}