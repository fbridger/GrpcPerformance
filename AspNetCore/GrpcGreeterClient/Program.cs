using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GrpcGreeterClient
{
    class Program
    {
        public const string TestName = ".Net Core 3.1 - Grpc.AspNetCore";
        private static long Min;
        private static long Max;
        private const int TotalRequests = 5000;
        private const bool LogDetails = false;
        private static ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 50 };

        static void Main(string[] args)
        {
            Warmup();

            WriteTableHeaders();
            TestSequential();
            TestSequentialReusingGrpcChannel();
            TestConcurrent();
            TestConcurrentReusingGrpcChannel();


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void Warmup()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var result = client.GetTime(new Empty());
            Console.WriteLine("Warmup done");
        }

        static void ClearMinMax()
        {
            Min = long.MaxValue;
            Max = long.MinValue;
        }

        static void CaptureMetrics(Stopwatch stopwatch)
        {
            if (Min > stopwatch.ElapsedMilliseconds)
                Min = stopwatch.ElapsedMilliseconds;

            if (Max < stopwatch.ElapsedMilliseconds)
                Max = stopwatch.ElapsedMilliseconds;
        }

        private static void WriteTableHeaders()
        {
            Console.WriteLine($"| gRPC Server | Test Type | Total Requests | Total Elapsed (ms) | Min (ms) | Max (ms) | Average (ms) |");
            Console.WriteLine($"| ----------- | --------- | -------------- | ------------------ | -------- | -------- | ------------ |");
        }

        private static void WriteTableRow(string name, Stopwatch stopwatch)
        {
            Console.WriteLine($"| {TestName} | {name} | {TotalRequests} | {stopwatch.ElapsedMilliseconds} | {Min} | {Max} | {100m * stopwatch.ElapsedMilliseconds / TotalRequests / 100m} |");
        }

        static void TestSequential()
        {
            ClearMinMax();
            if (LogDetails) Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestSequential)} {TotalRequests} requests");

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TotalRequests; i++)
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                var stopwatchRequest = Stopwatch.StartNew();
                using var channel = GrpcChannel.ForAddress("https://localhost:5001");
                var client = new Greeter.GreeterClient(channel);
                var result = client.GetTime(new Empty());
                stopwatchRequest.Stop();
                CaptureMetrics(stopwatchRequest);
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End request {i}: {result}");

            }
            stopwatch.Stop();

            if (LogDetails) Console.WriteLine($"{DateTime.Now} - {nameof(TestSequential)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

            WriteTableRow(nameof(TestSequential), stopwatch);
        }

        static void TestSequentialReusingGrpcChannel()
        {
            if (LogDetails) Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestSequentialReusingGrpcChannel)} {TotalRequests} requests");
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TotalRequests; i++)
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                var stopwatchRequest = Stopwatch.StartNew();
                var result = client.GetTime(new Empty());
                stopwatchRequest.Stop();
                CaptureMetrics(stopwatchRequest);
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End request {i}: {result}");

            }
            stopwatch.Stop();

            if (LogDetails) Console.WriteLine($"{DateTime.Now} - {nameof(TestSequentialReusingGrpcChannel)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

            WriteTableRow(nameof(TestSequentialReusingGrpcChannel), stopwatch);
        }

        static void TestConcurrent()
        {
            if (LogDetails) Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestConcurrent)} {TotalRequests} requests");

            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, TotalRequests, parallelOptions, (i) =>
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                var stopwatchRequest = Stopwatch.StartNew();
                using var channel = GrpcChannel.ForAddress("https://localhost:5001");
                var client = new Greeter.GreeterClient(channel);
                var result = client.GetTime(new Empty());
                stopwatchRequest.Stop();
                CaptureMetrics(stopwatchRequest);
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End request {i}: {result}");
            });
            stopwatch.Stop();

            if (LogDetails) Console.WriteLine($"{DateTime.Now} - {nameof(TestConcurrent)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

            WriteTableRow(nameof(TestConcurrent), stopwatch);
        }

        static void TestConcurrentReusingGrpcChannel()
        {
            if (LogDetails) Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestConcurrentReusingGrpcChannel)} {TotalRequests} requests");

            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, TotalRequests, parallelOptions, (i) =>
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                var stopwatchRequest = Stopwatch.StartNew();
                var result = client.GetTime(new Empty());
                stopwatchRequest.Stop();
                CaptureMetrics(stopwatchRequest);
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End request {i}: {result}");
            });
            stopwatch.Stop();

            if (LogDetails) Console.WriteLine($"{DateTime.Now} - {nameof(TestConcurrentReusingGrpcChannel)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

            WriteTableRow(nameof(TestConcurrentReusingGrpcChannel), stopwatch);
        }

    }
}
