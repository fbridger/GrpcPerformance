using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MegaCorp;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private const int TotalRequests = 500;
        private const bool LogDetails = false;
        private static ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 50 };

        static void Main()
        {
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
            var channel = new Channel("localhost", 10042, ChannelCredentials.Insecure);
            var clock = new TimeService.TimeServiceClient(channel);
            var result = clock.GetTime(new Empty());
            channel.ShutdownAsync().Wait();
            Console.WriteLine("Warmup done");
        }

        static void TestSequential()
        {
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestSequential)} {TotalRequests} requests");

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TotalRequests; i++)
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");

                var channel = new Channel("localhost", 10042, ChannelCredentials.Insecure);
                var clock = new TimeService.TimeServiceClient(channel);
                var result = clock.GetTime(new Empty());
                channel.ShutdownAsync().Wait();
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End request {i}: {result}");

            }
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestSequential)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestSequentialReusingGrpcChannel()
        {

            var channel = new Channel("localhost", 10042, ChannelCredentials.Insecure);
            var clock = new TimeService.TimeServiceClient(channel);
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestSequentialReusingGrpcChannel)} {TotalRequests} requests");

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TotalRequests; i++)
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                var result = clock.GetTime(new Empty());
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End request {i}: {result}");

            }
            stopwatch.Stop();
            channel.ShutdownAsync().Wait();
            Console.WriteLine($"{DateTime.Now} - {nameof(TestSequentialReusingGrpcChannel)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestConcurrent()
        {
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestConcurrent)} {TotalRequests} requests");

            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, TotalRequests, parallelOptions, (i) =>
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                var channel = new Channel("localhost", 10042, ChannelCredentials.Insecure);
                var clock = new TimeService.TimeServiceClient(channel);
                var result = clock.GetTime(new Empty());
                channel.ShutdownAsync().Wait();
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Result request {i} {result.Time}");
            });
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestConcurrent)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestConcurrentReusingGrpcChannel()
        {
            var channel = new Channel("localhost", 10042, ChannelCredentials.Insecure);
            var clock = new TimeService.TimeServiceClient(channel);

            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestConcurrentReusingGrpcChannel)} {TotalRequests} requests");
            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, TotalRequests, parallelOptions, (i) =>
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start request {i}");
                var result = clock.GetTime(new Empty());
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Result request {i} {result.Time}");
            });
            stopwatch.Stop();
            channel.ShutdownAsync().Wait();
            Console.WriteLine($"{DateTime.Now} - {nameof(TestConcurrentReusingGrpcChannel)} for {TotalRequests} requests took: {stopwatch.ElapsedMilliseconds}ms");

        }
    }

}
