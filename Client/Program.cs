using Albin.GrpcCodeFirst.Shared.Contracts;
using Calzolari.Grpc.Net.Client.Validation;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using ProtoBuf.Grpc.Client;

namespace Albin.GrpcCodeFirst.Client;
internal class Program
{
    private static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7039", new GrpcChannelOptions
        {
            // A service config can be used to configure gRPC retries.
            ServiceConfig = new ServiceConfig { MethodConfigs = { RetryPolicyConfig.GetBaseRetryPolicyConfig() } }
        });

        ConsoleKeyInfo cki;

        do
        {
            Console.Write("Welcome to the Club! Do you want to enter the VIP section? y/n: ");
            var enterVip = Console.ReadKey();

            if (enterVip.Key == ConsoleKey.Y)
            {
                await TryEnterClubVipSectionAsync(channel);
            }
            else
            {
                await TryEnterClubAsync(channel);
            }

            Console.Write("\n\nDo you wish to exit the application? y/n: ");
            cki = Console.ReadKey();
            Console.WriteLine("\n------------\n");
        } while (cki.Key != ConsoleKey.Y);
    }

    private static async Task TryEnterClubVipSectionAsync(GrpcChannel channel)
    {
        try
        {
            Console.Write("\nYou choose to try to enter the VIP section. Show your VIP pass (enter JWT): ");
            var jwt = Console.ReadLine();

            var headers = new Metadata { { "Authorization", $"Bearer {jwt}" } };

            Console.WriteLine("\nPlease wait while the Bouncer validates your VIP pass...");

            var client = channel.CreateGrpcService<IBouncerService>();

            var reply = await client.EnterClubVipSectionAsync(
                new EnterRequest
                {
                    Name = "VIP Guy",
                    Age = 25
                },
                new CallOptions(
                    headers: headers,
                    deadline: DateTime.UtcNow.AddSeconds(5))
            );

            Console.WriteLine(reply.AllowEntry ? "Allowed" : "Denied");
            Console.WriteLine($"The Bouncer responded with: {reply.Message}");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
        {
            Console.WriteLine("Timeout. The call took more than 5 seconds to finish.");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            var errors = ex.GetValidationErrors();

            foreach (var error in errors)
            {
                Console.WriteLine($"{error.ErrorMessage} Attempted value: {error.AttemptedValue}");
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated || ex.StatusCode == StatusCode.PermissionDenied)
        {
            Console.WriteLine($"The Bouncer denied you access to the VIP section: you don't have a valid VIP pass. ({ex.StatusCode})");
        }
        catch (RpcException ex) 
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static async Task TryEnterClubAsync(GrpcChannel channel)
    {
        try
        {
            Console.WriteLine("\nYou choose to NOT enter the VIP section. Say your name and age to enter the club. You have to be 23 years old or above.");

            // Enter nothing (empty string) to get validation error
            Console.Write("\nEnter name: ");
            var name = Console.ReadLine();

            // Enter a negative number to get validation error
            Console.Write("Enter age: ");
            var age = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\nPlease wait while the Bouncer looks at your information...\n");

            var client = channel.CreateGrpcService<IBouncerService>();

            var reply = await client.EnterClubAsync(
                new EnterRequest
                {
                    Name = name,
                    Age = age
                }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(5)));

            Console.WriteLine(reply.AllowEntry ? "Allowed" : "Denied");
            Console.WriteLine($"The Bouncer responded with: {reply.Message}");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
        {
            Console.WriteLine("Timeout. The call took more than 5 seconds to finish.");
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            var errors = ex.GetValidationErrors();

            foreach (var error in errors)
            {
                Console.WriteLine($"{error.ErrorMessage} Attempted value: {error.AttemptedValue}");
            }
        }
        catch (RpcException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
