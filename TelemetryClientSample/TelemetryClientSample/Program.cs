using Microsoft.Identity.Client;
using Microsoft.IdentityModel.S2S.Telemetry;

namespace TelemetryClientSample
{
    class Program
    {
        private static string s_clientId = "1d18b3b0-251b-4714-a02a-9956cec86c2d";
        private static readonly IEnumerable<string> s_scopes = new[] {
            "user.read", "openid" };

        static async Task Main(string[] args)
        {
            try
            {
                var pca = CreateApp();

                Console.WriteLine("====Acquire token interactively====");
                var result = await pca.AcquireTokenInteractive(s_scopes)
                .ExecuteAsync().ConfigureAwait(false);
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Access Token = " + result?.AccessToken);
                Console.ResetColor();

                Console.WriteLine("=====Acquire token silently======");
                var account = (await pca.GetAccountsAsync().ConfigureAwait(false)).Single();

                result = await pca.AcquireTokenSilent(s_scopes, account).ExecuteAsync().ConfigureAwait(false);
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Access Token = " + result?.AccessToken);
                Console.ResetColor();
            }
            catch (MsalException e)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Error: ErrorCode=" + e.ErrorCode + "ErrorMessage=" + e.Message);
                Console.ResetColor();
            }

            Console.Read();
        }

        private static IPublicClientApplication CreateApp()
        {
            return PublicClientApplicationBuilder.Create(s_clientId)
                 .WithTenantId("72f988bf-86f1-41af-91ab-2d7cd011db47")
                 .WithExperimentalFeatures()
                 .WithDefaultRedirectUri()
                 .WithLogging(MyLoggingMethod, LogLevel.Info, true, false)
                 .WithTelemetryClient(new AriaTelemetryClient(s_clientId))
                 .Build();
        }

        static void MyLoggingMethod(LogLevel level, string message, bool containsPii)
        {
            Console.WriteLine($"MSALTest {level} {containsPii} {message}");
        }
    }
}
