using Polly;
using Polly.Extensions.Http;

namespace Inventory.Service.Policies;

public static class PollyPolicies
{
   public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
   => HttpPolicyExtensions
       .HandleTransientHttpError()  
       .WaitAndRetryAsync(
           retryCount: 5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) 
           + TimeSpan.FromMinutes(Random.Shared.Next(0,1000)),
           onRetry: (outcome, timespan, attempt, _) =>
               Console.WriteLine(
                   $"[Retry] Attempt {attempt} after {timespan.TotalSeconds:F1}s — " +
                   $"{outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}"));

   
   public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
       HttpPolicyExtensions
           .HandleTransientHttpError()
           .CircuitBreakerAsync(
               handledEventsAllowedBeforeBreaking: 3,
               durationOfBreak: TimeSpan.FromSeconds(15),
               onBreak: (outcome, duration) =>
                   Console.WriteLine($"[Circuit] OPEN for {duration.TotalSeconds}s"),
               onReset: () =>
                   Console.WriteLine("[Circuit] CLOSED"),
               onHalfOpen: () =>
                   Console.WriteLine("[Circuit] HALF-OPEN"));

   public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy() =>
       Policy.TimeoutAsync<HttpResponseMessage>(1);
}