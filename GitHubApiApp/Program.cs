using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

namespace GitHubApiApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Chilla lite, hämtar data från GitHub API...");

            Console.WriteLine("");
            Console.WriteLine("Njut av en kaffe...");
            Console.WriteLine("       _______");
            Console.WriteLine("      |       |\\");
            Console.WriteLine("      |       | |");
            Console.WriteLine("      |-~-~-~-| |");
            Console.WriteLine("      |       |/");
            Console.WriteLine("      |_______|");
            Console.WriteLine("       \\_____/");
            Console.WriteLine("");

            var cryptoPrices = new CryptoPrices();
            await cryptoPrices.GetCryptoPricesAsync();

            Thread.Sleep(6000);
            Console.WriteLine("");
            Console.WriteLine("Juste... uppgiften... här kommer det du egentligen är ute efter : ");
            Thread.Sleep(3000);

            // GitHub API-url
            string apiUrl = "https://api.github.com/orgs/dotnet/repos";

            // Anropa API:t och hämta data
            var repositories = await FetchGitHubRepositories(apiUrl);

            // Skriv ut data till konsolen
            if (repositories != null)
            {
                foreach (var repo in repositories)
                {
                    Console.WriteLine("-----------------------------------------------------");
                    Console.WriteLine($"Name: {repo.Name}");
                    Console.WriteLine($"Description: {repo.Description}");
                    Console.WriteLine($"URL: {repo.HtmlUrl}");
                    Console.WriteLine($"Homepage: {repo.Homepage ?? "Ingen hemsida"}");
                    Console.WriteLine($"Watchers: {repo.Watchers}");
                    Console.WriteLine($"Senast pushad: {repo.PushedAt:yyyy-MM-dd HH:mm}");
                    Console.WriteLine("-----------------------------------------------------\n");
                }
            }
            else
            {
                Console.WriteLine(" ¨Näsdykning!¨ Kunde inte hämta data från API:t.");
            }
        }

        private static async Task<List<GitHubRepo>> FetchGitHubRepositories(string apiUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Add("User-Agent", "C# console application");

                    // Gör en HTTP GET-förfrågan
                    var response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    // Läs JSON-data från svaret
                    var jsonData = await response.Content.ReadAsStringAsync();

                    // Deserialisera JSON till en lista av GitHubRepo
                    var repositories = JsonSerializer.Deserialize<List<GitHubRepo>>(jsonData, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    });

                    return repositories;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Något dök på näsan : {ex.Message}");
                return null;
            }
        }
    }
}
