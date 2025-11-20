# .NET Service Lifetimes Demo

A hands-on demonstration showing how Dependency Injection service lifetimes work in .NET. This project uses a simple API to visually demonstrate the differences between Singleton, Scoped, and Transient services.

## 🎯 What This Demonstrates

This project answers: **"When does .NET create new service instances vs reuse existing ones?"**

We track service instances using a unique ID to see exactly when new objects are created.

## 🔧 The Test Setup

We have 3 services working together:

```csharp
public class RequestTracker
{
    public string TrackerId = Guid.NewGuid().ToString()[..8]; // Unique ID per instance
}

public class ServiceA(RequestTracker tracker)
{
    public string GetInfo() => $"A -> {tracker.TrackerId}";
}

public class ServiceB(RequestTracker tracker)
{
    public string GetInfo() => $"B -> {tracker.TrackerId}";
}
```

**Key Point**: Both `ServiceA` and `ServiceB` depend on `RequestTracker`. By changing `RequestTracker`'s lifetime, we can see different sharing behaviors.

## 🚀 API Endpoints to Test

### 1. `/check` - Test Both Services Together
```http
GET http://localhost:5039/check
```
**Shows**: How services share the `RequestTracker` instance within the same request.

### 2. `/check2` - Test Single Service  
```http
GET http://localhost:5039/check2
```
**Shows**: How the same service behaves across different requests.

## 📊 The 3 Service Lifetimes - Visual Results

### 🟢 **Singleton Lifetime** 
```csharp
builder.Services.AddSingleton<RequestTracker>();
```

**What happens**: ONE instance for the entire application

**Test Results**:
- First request: `{"a": "A -> 5f4bd685", "b": "B -> 5f4bd685"}`
- Second request: `{"a": "A -> 5f4bd685", "b": "B -> 5f4bd685"}`
- Third request: `{"a": "A -> 5f4bd685", "b": "B -> 5f4bd685"}`

**Observation**: Same ID (`5f4bd685`) appears in ALL requests - one instance shared everywhere.
<img width="1920" height="1080" alt="Screenshot (1270)" src="https://github.com/user-attachments/assets/408cf9ed-dfad-4b8d-8f44-fceb2a2c8340" />
<img width="1920" height="1080" alt="Screenshot (1271)" src="https://github.com/user-attachments/assets/7ad9913e-5878-4cd5-bb68-a7a82adcd2d7" />

### 🟡 **Scoped Lifetime**
```csharp
builder.Services.AddScoped<RequestTracker>();
```

**What happens**: One instance per HTTP request

**Test Results**:
- Request 1: `{"a": "A -> a610ce56", "b": "B -> a610ce56"}`
- Request 2: `{"a": "A -> a610ce56", "b": "B -> a610ce56"}`
- Request 3: `{"a": "A -> e51c5c72", "b": "B -> e51c5c72"}`

**Observation**: 
- Same ID within each request (`ServiceA` and `ServiceB` share the instance)
- Different IDs across different requests (new instance per request)
<img width="1920" height="1080" alt="Screenshot (1274)" src="https://github.com/user-attachments/assets/0b20f004-f1f2-4e45-b1c5-b683f3c42037" />
<img width="1920" height="1080" alt="Screenshot (1275)" src="https://github.com/user-attachments/assets/1a2fb5fa-d3f9-4065-8960-3ef8c310921d" />

### 🔴 **Transient Lifetime**
```csharp
builder.Services.AddTransient<RequestTracker>();
```

**What happens**: New instance every time it's requested

**Test Results**:
- Request: `{"a": "A -> 5dcb28ab", "b": "B -> 18536f4c"}`

**Observation**: Different IDs even within the same request - `ServiceA` and `ServiceB` get different instances!
<img width="1920" height="1080" alt="Screenshot (1276)" src="https://github.com/user-attachments/assets/df2d8123-e4f0-4ceb-84cc-94ad5a5e2879" />

## 🛠️ How to Run This Yourself

1. **Clone and run the project**:
```bash
dotnet run
```

2. **Test the endpoints** (use Postman, browser, or curl):
```bash
# Test first endpoint
curl http://localhost:5039/check

# Test second endpoint  
curl http://localhost:5039/check2
```

3. **Change the lifetime** in `Program.cs` and test again:
```csharp
// Try each of these and see the difference:
builder.Services.AddSingleton<RequestTracker>();    // 🟢
builder.Services.AddScoped<RequestTracker>();       // 🟡  
builder.Services.AddTransient<RequestTracker>();    // 🔴
```

## 💡 Real-World Usage

| Lifetime | Use Case | Example |
|----------|----------|---------|
| **Singleton** | Services that are stateless and expensive to create | `IConfiguration`, `ILogger`, Caching services |
| **Scoped** | Services that need request-specific data | `DbContext`, User session data |
| **Transient** | Lightweight, stateless services | Validators, Calculators |

## 📝 Key Takeaways

- **Singleton**: "Share one instance with everyone, forever"
- **Scoped**: "One instance per conversation (HTTP request)"  
- **Transient**: "Always create a new one, never share"

This demo makes abstract DI concepts concrete by letting you **see** the difference through unique instance IDs!

---
