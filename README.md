
# Overview
Our team implemented Grpc using the protobuf-net package in our backend services to avoid having to mantain .proto files.

We are using a Rest Web API that communicates with our Grpc Services. 

During the Load Tests we performed using https://locust.io/ we encountered unpredictable and very response times when being executed by concurrent users (simulated by locust)

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

# Conclusion



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
5/21/2021 9:26:44 AM - Starting TestSequential 500 requests
5/21/2021 9:26:47 AM - TestSequential for 500 requests took: 3392ms
5/21/2021 9:26:47 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 9:26:47 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 48ms
5/21/2021 9:26:47 AM - Starting TestConcurrent 500 requests
5/21/2021 9:26:47 AM - TestConcurrent for 500 requests took: 169ms
5/21/2021 9:26:47 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 9:26:47 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 42ms
Press [Enter] to exit
```

Run # 2
```
Warmup done
5/21/2021 9:27:38 AM - Starting TestSequential 500 requests
5/21/2021 9:27:42 AM - TestSequential for 500 requests took: 3471ms
5/21/2021 9:27:42 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 9:27:42 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 40ms
5/21/2021 9:27:42 AM - Starting TestConcurrent 500 requests
5/21/2021 9:27:42 AM - TestConcurrent for 500 requests took: 126ms
5/21/2021 9:27:42 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 9:27:42 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 34ms
Press [Enter] to exit
```
## pb-grpc - .Net Framework 4.7.2 - protobuf-net.Grpc.Native


Run # 1
```
Warmup done
5/21/2021 9:34:43 AM - Starting TestSequential 500 tasks
5/21/2021 9:34:43 AM - TestSequential for 500 tasks took: 873ms
5/21/2021 9:34:43 AM - Starting TestConcurrentReusingGrpcChannel 500 tasks
5/21/2021 9:34:44 AM - TestConcurrentReusingGrpcChannel for 500 tasks took: 615ms
5/21/2021 9:34:44 AM - Starting TestConcurrent 500 tasks
5/21/2021 9:34:44 AM - TestConcurrent for 500 tasks took: 316ms
5/21/2021 9:34:44 AM - Starting TestConcurrentReusingGrpcChannel 500 tasks
5/21/2021 9:34:45 AM - TestConcurrentReusingGrpcChannel for 500 tasks took: 241ms
Press [Enter] to exit
```

Run # 2
```
Warmup done
5/21/2021 9:35:26 AM - Starting TestSequential 500 tasks
5/21/2021 9:35:27 AM - TestSequential for 500 tasks took: 581ms
5/21/2021 9:35:27 AM - Starting TestConcurrentReusingGrpcChannel 500 tasks
5/21/2021 9:35:27 AM - TestConcurrentReusingGrpcChannel for 500 tasks took: 48ms
5/21/2021 9:35:27 AM - Starting TestConcurrent 500 tasks
5/21/2021 9:35:27 AM - TestConcurrent for 500 tasks took: 150ms
5/21/2021 9:35:27 AM - Starting TestConcurrentReusingGrpcChannel 500 tasks
5/21/2021 9:35:27 AM - TestConcurrentReusingGrpcChannel for 500 tasks took: 49ms
Press [Enter] to exit
```

## pb-net-grpc - .Net Core 3.1/5.0 - protobuf-net.Grpc.AspNetCore


Run # 1
```
Warmup done
5/21/2021 9:32:17 AM - Starting TestSequential 500 requests
5/21/2021 9:32:19 AM - TestSequential for 500 requests took: 1349ms
5/21/2021 9:32:19 AM - Starting TestSequentialReusingGrpcChannel 500 requests
5/21/2021 9:32:19 AM - TestSequentialReusingGrpcChannel for 500 requests took: 276ms
5/21/2021 9:32:19 AM - Starting TestConcurrent 500 requests
5/21/2021 9:32:27 AM - TestConcurrent for 500 requests took: 8059ms
5/21/2021 9:32:27 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 9:32:34 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 6895ms
Press [Enter] to exit
```

Run # 2
```
Warmup done
5/21/2021 9:33:23 AM - Starting TestSequential 500 requests
5/21/2021 9:33:25 AM - TestSequential for 500 requests took: 1449ms
5/21/2021 9:33:25 AM - Starting TestSequentialReusingGrpcChannel 500 requests
5/21/2021 9:33:25 AM - TestSequentialReusingGrpcChannel for 500 requests took: 296ms
5/21/2021 9:33:25 AM - Starting TestConcurrent 500 requests
5/21/2021 9:33:33 AM - TestConcurrent for 500 requests took: 7899ms
5/21/2021 9:33:33 AM - Starting TestConcurrentReusingGrpcChannel 500 requests
5/21/2021 9:33:40 AM - TestConcurrentReusingGrpcChannel for 500 requests took: 6895ms
Press [Enter] to exit
```