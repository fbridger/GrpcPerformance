
# Overview
Our team implemented Grpc using the protobuf-net package in our backend services to avoid having to mantain .proto files.

We are using a Rest Web API that communicates with our Grpc Services. 

During the Load Tests we performed using https://locust.io/ we encountered unpredictable and very high response times when being executed by concurrent users (simulated by locust)

# Backend services
Here is a list of the services that we have:

- Rest API (.Net Core 3.1)
- Rest Admin API (.Net Core 3.1)
- gRPC Service 1 (.Net Core 3.1)
- gRPC Service 2 (.Net Core 3.1)
- gRPC Service 3 (.Net Framework 4.7.2)

## Services dependency tree

- Rest API
	- gRPC Service 1
		- gRPC Service 3
	- gRPC Service 2
		- gRPC Service 3
- Rest Admin API
	- gRPC Service 1
		- gRPC Service 3


# Performance Tests
To understand the cause of this problem we try to simulate a simple concurrency scenarios using the existing gRPC samples available with some modifications.

We obviously generated 2 types of tests:
- Sequential: one call after another
- Concurrent: multiple calls at the same time

**Note:** Four our concurrent tests we used a `Parallel.For` with a MaxDegreeOfParallelism of 50

Each one of the tests mentioned above were executed in the following way:
- Single Channel to the gRPC Server for all requests
- Creating a new Channel for each request
 
So we are actually doing 4 type of tests:

- Sequential reusing the same gRPC channel
- Sequential using a new gRPC channel
- Concurrent reusing the same gRPC channel
- Concurrent using a new gRPC channel

# Conclusions

 - It seems that using Asp.Net Core to host a gRPC server slows severely the performance of the application.
 - We encountered **unpredictable and slow** (specially in conncurrent requests) response times when hosting gRPC in ASP.NET Core
 - Using Grpc.Core Server seems to be more performant


# Available gRPC Samples used

## GrpcGreeter - .Net Core 3.1 - Grpc.AspNetCore

Official Microsoft Asp.Net Core sample: https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/tutorials/grpc

## grpc - .Net Core 3.1 - Grpc.Core

protobuf-net sample: https://github.com/protobuf-net/protobuf-net.Grpc/tree/main/examples/grpc

## pb-grpc - .Net Framework 4.7.2 - protobuf-net.Grpc.Native

protobuf-net sample: https://github.com/protobuf-net/protobuf-net.Grpc/tree/main/examples/pb-grpc

## pb-net-grpc - .Net Core 3.1/5.0 - protobuf-net.Grpc.AspNetCore

protobuf-net sample: https://github.com/protobuf-net/protobuf-net.Grpc/tree/main/examples/pb-net-grpc

# Results

## GrpcGreeter - .Net Core 3.1 - Grpc.AspNetCore

Run # 1
```
Warmup done
5/21/2021 9:43:33 AM - Starting TestSequential 500 requests
5/21/2021 9:43:36 AM - TestSequential for 500 requests took: 3177ms
5/21/2021 9:43:36 AM - Starting TestSequentialReusingGrpcChannel 500 requests
5/21/2021 9:43:37 AM - TestSequentialReusingGrpcChannel for 500 requests took: 1095ms
5/21/2021 9:43:37 AM - Starting TestConcurrent 500 requests
5/21/2021 9:43:46 AM - TestConcurrent for 500 requests took: 8987ms
5/21/2021 9:43:46 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 9:43:50 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 3868ms
Press any key to exit...
```

Run # 2
```
Warmup done
5/21/2021 9:44:27 AM - Starting TestSequential 500 requests
5/21/2021 9:44:30 AM - TestSequential for 500 requests took: 3066ms
5/21/2021 9:44:30 AM - Starting TestSequentialReusingGrpcChannel 500 requests
5/21/2021 9:44:31 AM - TestSequentialReusingGrpcChannel for 500 requests took: 1107ms
5/21/2021 9:44:31 AM - Starting TestConcurrent 500 requests
5/21/2021 9:44:47 AM - TestConcurrent for 500 requests took: 15118ms
5/21/2021 9:44:47 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 9:44:54 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 7104ms
Press any key to exit...
```

## grpc - .Net Core 3.1 - Grpc.Core

Run # 1
```
Warmup done
5/21/2021 10:21:35 AM - Starting TestSequential 500 requests
5/21/2021 10:21:40 AM - TestSequential for 500 requests took: 5066ms
5/21/2021 10:21:40 AM - Starting TestSequentialReusingGrpcChannel 500 requests
5/21/2021 10:21:41 AM - TestSequentialReusingGrpcChannel for 500 requests took: 288ms
5/21/2021 10:21:41 AM - Starting TestConcurrent 500 requests
5/21/2021 10:21:41 AM - TestConcurrent for 500 requests took: 297ms
5/21/2021 10:21:41 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 10:21:41 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 83ms
Press [Enter] to exit
```

Run # 2
```
Warmup done
5/21/2021 10:23:35 AM - Starting TestSequential 500 requests
5/21/2021 10:23:40 AM - TestSequential for 500 requests took: 5260ms
5/21/2021 10:23:40 AM - Starting TestSequentialReusingGrpcChannel 500 requests
5/21/2021 10:23:40 AM - TestSequentialReusingGrpcChannel for 500 requests took: 264ms
5/21/2021 10:23:40 AM - Starting TestConcurrent 500 requests
5/21/2021 10:23:41 AM - TestConcurrent for 500 requests took: 199ms
5/21/2021 10:23:41 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 10:23:41 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 54ms
Press [Enter] to exit
```
## pb-grpc - .Net Framework 4.7.2 - protobuf-net.Grpc.Native


Run # 1
```
Warmup done
5/21/2021 10:20:08 AM - Starting TestSequential 500 requests
5/21/2021 10:20:09 AM - TestSequential for 500 requests took: 870ms
5/21/2021 10:20:09 AM - Starting TestSequentialReusingGrpcChannel 500 requests
5/21/2021 10:20:10 AM - TestSequentialReusingGrpcChannel for 500 requests took: 306ms
5/21/2021 10:20:10 AM - Starting TestConcurrent 500 requests
5/21/2021 10:20:10 AM - TestConcurrent for 500 requests took: 184ms
5/21/2021 10:20:10 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 10:20:10 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 62ms
Press [Enter] to exit
```

Run # 2
```
Warmup done
5/21/2021 10:37:12 AM - Starting TestSequential 100 requests
5/21/2021 10:37:13 AM - TestSequential for 100 requests took: 166ms
5/21/2021 10:37:13 AM - Starting TestSequentialReusingGrpcChannel 100 requests
5/21/2021 10:37:13 AM - TestSequentialReusingGrpcChannel for 100 requests took: 63ms
5/21/2021 10:37:13 AM - Starting TestConcurrent 100 requests
5/21/2021 10:37:13 AM - TestConcurrent for 100 requests took: 42ms
5/21/2021 10:37:13 AM - Starting TestConcurrentReusingGrpcChannel 100 requests
5/21/2021 10:37:13 AM - TestConcurrentReusingGrpcChannel for 100 requests took: 13ms
Press [Enter] to exit
```

## pb-net-grpc - .Net Core 3.1/5.0 - protobuf-net.Grpc.AspNetCore


Run # 1
```
Warmup done
5/21/2021 10:46:46 AM - Starting TestSequential 500 requests
5/21/2021 10:46:47 AM - TestSequential for 500 requests took: 1834ms
5/21/2021 10:46:47 AM - Starting TestSequentialReusingGrpcChannel 500 requests
5/21/2021 10:46:48 AM - TestSequentialReusingGrpcChannel for 500 requests took: 347ms
5/21/2021 10:46:48 AM - Starting TestConcurrent 500 requests
5/21/2021 10:46:58 AM - TestConcurrent for 500 requests took: 10009ms
5/21/2021 10:46:58 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 10:46:58 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 93ms
Press [Enter] to exit
```

Run # 2
```
Warmup done
5/21/2021 10:47:40 AM - Starting TestSequential 500 requests
5/21/2021 10:47:41 AM - TestSequential for 500 requests took: 1927ms
5/21/2021 10:47:41 AM - Starting TestSequentialReusingGrpcChannel 500 requests
5/21/2021 10:47:42 AM - TestSequentialReusingGrpcChannel for 500 requests took: 337ms
5/21/2021 10:47:42 AM - Starting TestConcurrent 500 requests
5/21/2021 10:47:51 AM - TestConcurrent for 500 requests took: 8745ms
5/21/2021 10:47:51 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 10:47:54 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 3951ms
Press [Enter] to exit
```
