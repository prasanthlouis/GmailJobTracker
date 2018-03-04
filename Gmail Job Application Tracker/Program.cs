using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gmail_Job_Application_Tracker
{
    public class Program
    {

        //I should be using regex to parse this. Meh.
        static List<string> JobsAppliedToKeyWords = new List<string>
        {
            "Your Application Has Been Successfully Submitted",
            "submit your application",
            "We Have Received Your Application",
            "interest in joining our team",
            "Thanks for applying",
            "Your application for",
            "job was submitted successfully",
            "Thank you for your application!",
            "We have received your job application",
            "Your application was submitted"
        };

            static string[] Scopes = { GmailService.Scope.GmailReadonly };
            static string ApplicationName = "Gmail API .NET Quickstart";

            static void Main(string[] args)
            {
                UserCredential credential;

                using (var stream =
                    new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = Environment.GetEnvironmentVariable("LocalAppData");
                    credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }


                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });


                UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List("me");


                var response = request.Execute();
                Console.WriteLine("Jobs Applied To:");
                if (response != null && response.Messages.Count > 0)
                {
                    foreach (var messages in response.Messages)
                    {
                    var email = service.Users.Messages.Get("me", messages.Id).Execute();
                    if (JobsAppliedToKeyWords.Any(x => email.Snippet.Contains(x)))
                        Console.WriteLine(email.Snippet + "\n");
                    }
                }
                else
                {
                    Console.WriteLine("No messages found.");
                }
                Console.Read();
        }
    }
}
