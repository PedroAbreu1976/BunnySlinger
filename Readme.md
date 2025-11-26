# BunnySlinger SDK

BunnySlinger is a .NET 9 library for asynchronous message passing ("slinging bunnies") between components or services. It supports in-memory and RabbitMQ-based transport, and is designed for extensibility and testability.

## NuGet Packages

BunnySlinger is distributed as four NuGet packages, each providing specific functionality:

- **BunnySlinger**: Core library for defining bunnies, handlers, interceptors, and in-memory transport.
- **BunnySlinger.Rabbit**: Adds support for RabbitMQ transport, enabling distributed messaging between applications.
- **BunnySlinger.Outbox**: Provides reliable message delivery using the Outbox pattern, backed by Entity Framework. Ensures messages are only sent when transactions succeed.
- **BunnySlinger.Idempotency**: Adds idempotency support using Entity Framework, ensuring that each message is processed only once.

## Features

- **In-Memory Transport**: For local development and testing.
- **RabbitMQ Transport**: For distributed messaging.
- **Extensible Bunny Types**: Define your own message types ("bunnies").
- **Handlers & Interceptors**: Process and intercept messages.
- **Dependency Injection**: Integrates with Microsoft.Extensions.DependencyInjection.

## Getting Started

### 1. Install BunnySlinger

Add the BunnySlinger NuGet package to your project.

### 2. Define a Bunny

Create a class implementing the `IBunny` interface:

```csharp
public class TestBunny : IBunny
{
    public string Message { get; set; }
}
```

### 3. Define a Bunny Catcher

Create a class to handle received bunnies by implementing a handler interface:

```csharp
public class TestBunnyCatcher : IBunnyHandler<TestBunny>
{
    public Task HandleAsync(TestBunny bunny, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Caught bunny with message: {bunny.Message}");
        return Task.CompletedTask;
    }
}
```

---

## In-Memory Usage

### Register BunnySlinger (In-Memory)

```csharp
var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices((hostContext, services) => {
    services.AddBunnyInMemory();
    services.AddBunnies(typeof(TestBunny).Assembly);
    services.AddBunnyHandlers(typeof(TestBunnyCatcher).Assembly);
    services.AddBunnyInterceptors(typeof(RandomBunnyInterceptor).Assembly);
});

var app = builder.Build();
await app.StartBunnyObserver();
```

### Sling a Bunny (In-Memory)

```csharp
var sender = app.Services.GetRequiredService<IBunnySling>();
await sender.SlingBunnyAsync(new TestBunny { Message = "Hello, Bunny!" });
```

### Run the Application

```csharp
await app.RunAsync();
```

---

## RabbitMQ Usage

> **Note:** The NuGet package **BunnySlinger.Rabbit** is required for RabbitMQ transport. Install it in your project before proceeding.

### Register BunnySlinger (RabbitMQ Publisher)

```csharp
var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices((hostContext, services) => {
    services.AddBunnyMq(o => {
        o.HostName = "localhost";
        o.Port = 5672;
    });
    services.AddBunnies(typeof(TestBunny).Assembly);
});

var app = builder.Build();
await app.StartBunnyObserver();
```

### Sling a Bunny (RabbitMQ Publisher)

```csharp
var sender = app.Services.GetRequiredService<IBunnySling>();
await sender.SlingBunnyAsync(new TestBunny { Message = "Hello, Bunny!" });
```

### Run the Publisher Application

```csharp
await app.RunAsync();
```

---

### Register BunnySlinger (RabbitMQ Subscriber)

```csharp
var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices((hostContext, services) => {
    services.AddBunnyMq(c => {
        c.HostName = "localhost";
        c.Port = 5672;
    });
    services.AddBunnies(typeof(TestBunny).Assembly);
    services.AddBunnyHandlers(typeof(TestBunnyCatcher).Assembly);
    services.AddBunnyInterceptors(typeof(TestBunnyCatcher).Assembly);
});

var app = builder.Build();
await app.StartBunnyObserver();
await app.RunAsync();
```

---

## Advanced Usage

- **Interceptors**: Add cross-cutting concerns (logging, validation).
- **Outbox**: Requires the NuGet package **BunnySlinger.Outbox**. Uses Entity Framework for reliable message delivery. To enable, add:

    ```csharp
    services.AddBunnyOutbox<AppDbContext>();
    ```
    After building the host, start the Outbox with:
    ```csharp
    await app.StartBunnyOutbox();
    ```
- **Idempotency**: Requires the NuGet package **BunnySlinger.Idempotency**. Uses Entity Framework to ensure message processing is not repeated. To enable, add:

    ```csharp
    services.AddBunnyIdempotency<AppDbContext>();
    ```

## API Reference

- `IBunnySling`: Main interface for sending bunnies.
- `IBunnyHandler<TBunny>`: Asynchronously handles a bunny.
- `IBunnyInterceptor` and `IBunnyInterceptor<TBunny>`: Intercepts bunny processing.
---

This document provides a concise overview and practical examples for using BunnySlinger in your .NET 9 applications. For more details, refer to the example projects in the repository.