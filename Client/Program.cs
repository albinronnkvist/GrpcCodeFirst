using Albin.GrpcCodeFirst.Shared.Contracts;
using Calzolari.Grpc.Net.Client.Validation;
using Grpc.Core;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace Albin.GrpcCodeFirst.Client;
internal class Program
{
    private static async Task Main(string[] args)
    {
        ConsoleKeyInfo cki;

        do
        {
            Console.WriteLine("Say your name and age to enter the club. You have to be 23 years old or above.");

            // Enter nothing (empty string) to get validation error
            Console.Write("\nEnter name: ");
            var name = Console.ReadLine();

            // Enter a negative number to get validation error
            Console.Write("Enter age: ");
            var age = Convert.ToInt32(Console.ReadLine());

            try
            {
                Console.WriteLine("\nSending request...");

                using var channel = GrpcChannel.ForAddress("https://localhost:7039");
                var client = channel.CreateGrpcService<IBouncerService>();

                var reply = await client.EnterClubAsync(
                    new EnterRequest
                    {
                        Name = name,
                        Age = age
                    }, new CallOptions(deadline: DateTime.UtcNow.AddSeconds(5)));

                Console.WriteLine($"Reply: {reply.Message}");
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
                    Console.WriteLine($"\n{error.ErrorMessage} Attempted value: {error.AttemptedValue}");
                }
            }

            Console.Write("\n\nDo you wish to exit? y/n: ");
            cki = Console.ReadKey();
            Console.WriteLine("\n------------\n");

        } while (cki.Key != ConsoleKey.Y);
    }
}
