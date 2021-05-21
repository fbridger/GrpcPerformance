using Grpc.Core;
using ProtoBuf.Grpc.Client;
using Shared_CS;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Client_CS
{
    class Program
    {

        private const int TotalRequests = 100;
        private const bool LogDetails = false;
        private static ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 50 };

        static void Main()
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Warmup();

            TestSequential();
            TestConcurrentReusingGrpcChannel();
            TestConcurrent();
            TestConcurrentReusingGrpcChannel();

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();
        }

        static void Warmup()
        {
            var channel = new Channel("localhost", 10042, ChannelCredentials.Insecure);
            var calculator = channel.CreateGrpcService<ICalculator>();
            var result = calculator.GetTime();
			Console.WriteLine("Warmup done");
        }

        static void TestSequential()
        {
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestSequential)} {TotalRequests} tasks");

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TotalRequests; i++)
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start task {i}");

                var channel = new Channel("localhost", 10042, ChannelCredentials.Insecure);
                var calculator = channel.CreateGrpcService<ICalculator>();
                var result = calculator.GetTime();
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End task {i}: {result}");

            }
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestSequential)} for {TotalRequests} tasks took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestSequentialReusingGrpcChannel()
        {
            var channel = new Channel("localhost", 10042, ChannelCredentials.Insecure);
            var calculator = channel.CreateGrpcService<ICalculator>();
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestSequentialReusingGrpcChannel)} {TotalRequests} tasks");

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TotalRequests; i++)
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start task {i}");
                var result = calculator.GetTime();
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End task {i}: {result}");

            }
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestSequentialReusingGrpcChannel)} for {TotalRequests} tasks took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestConcurrent()
        {
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestConcurrent)} {TotalRequests} tasks");

            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, TotalRequests, parallelOptions, (i) =>
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start task {i}");
                var channel = new Channel("localhost", 10042, ChannelCredentials.Insecure);
                var calculator = channel.CreateGrpcService<ICalculator>();
                var result = calculator.GetTime();
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Result task {i} {result.Time}");
            });
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestConcurrent)} for {TotalRequests} tasks took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestConcurrentReusingGrpcChannel()
        {
            var channel = new Channel("localhost", 10042, ChannelCredentials.Insecure);
            var calculator = channel.CreateGrpcService<ICalculator>();

            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestConcurrentReusingGrpcChannel)} {TotalRequests} tasks");
            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, TotalRequests, parallelOptions, (i) =>
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start task {i}");
                var result = calculator.GetTime();
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Result task {i} {result.Time}");
            });
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestConcurrentReusingGrpcChannel)} for {TotalRequests} tasks took: {stopwatch.ElapsedMilliseconds}ms");

        }
    }

}
