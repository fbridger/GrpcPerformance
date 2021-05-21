using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;
using Shared_CS;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Client_CS
{
    class Program
    {
        private const int TotalRequests = 500;
        private const bool LogDetails = false;
        private static ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 50 };

        static void Main()
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Warmup();
            TestSequential();
            TestSequentialReusingGrpcChannel();
            TestConcurrent();
            TestConcurrentReusingGrpcChannel();

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }

        static void Warmup()
        {
            using var http = GrpcChannel.ForAddress("http://localhost:10042");
            var calculator = http.CreateGrpcService<ICalculator>();
            var result = calculator.GetTime();
            Console.WriteLine("Warmup done");
        }

        static void TestSequential()
        {
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestSequential)} {TotalRequests} requests");

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TotalRequests; i++)
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                using var http = GrpcChannel.ForAddress("http://localhost:10042");
                var calculator = http.CreateGrpcService<ICalculator>();
                var result = calculator.GetTime();
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End request {i}: {result}");

            }
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestSequential)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestSequentialReusingGrpcChannel()
        {
            using var http = GrpcChannel.ForAddress("http://localhost:10042");
            var calculator = http.CreateGrpcService<ICalculator>();
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestSequentialReusingGrpcChannel)} {TotalRequests} requests");

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TotalRequests; i++)
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                var result = calculator.GetTime();
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End request {i}: {result}");

            }
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestSequentialReusingGrpcChannel)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestConcurrent()
        {
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestConcurrent)} {TotalRequests} requests");

            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, TotalRequests, parallelOptions, (i) =>
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                using var http = GrpcChannel.ForAddress("http://localhost:10042");
                var calculator = http.CreateGrpcService<ICalculator>();
                var result = calculator.GetTime();
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Result request {i} {result.Time}");
            });
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestConcurrent)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestConcurrentReusingGrpcChannel()
        {
            using var http = GrpcChannel.ForAddress("http://localhost:10042");
            var calculator = http.CreateGrpcService<ICalculator>();

            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestConcurrentReusingGrpcChannel)} {TotalRequests} requests");
            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, TotalRequests, parallelOptions, (i) =>
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                var result = calculator.GetTime();
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Result request {i} {result.Time}");
            });
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestConcurrentReusingGrpcChannel)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

        }
    }
}
