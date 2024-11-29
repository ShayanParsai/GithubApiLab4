using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GitHubApiApp
{
    public class CryptoPrices
    {
        private static readonly string ApiUrl = "https://api.binance.com/api/v3/ticker/price";

        public async Task GetCryptoPricesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    Console.WriteLine("och de senaste priserna på BTC/ETH och XRP, hämtat från Binance Ticker API...");

                    // Hämta data från Binance API
                    var response = await client.GetStringAsync(ApiUrl);

                    // Deserialisera JSON-data
                    var tickers = JsonSerializer.Deserialize<List<CryptoTicker>>(response);

                    if (tickers == null || !tickers.Any())
                    {
                        Console.WriteLine("Inga tickers hittades i API-svaret.");
                        return;
                    }

                    // Filtrera efter de symboler vi är intresserade av
                    var symbolsToCheck = new[] { "BTCUSDT", "ETHUSDT", "XRPUSDT" };
                    var filteredPrices = tickers
                        .Where(t => symbolsToCheck.Contains(t.Symbol, StringComparer.OrdinalIgnoreCase))
                        .ToList();

                    if (!filteredPrices.Any())
                    {
                        Console.WriteLine("Inga matchande symboler hittades.");
                        return;
                    }

                    // Skriv ut priser
                    Console.WriteLine("\nSenaste priserna på kryptomarknaden:");
                    foreach (var ticker in filteredPrices)
                    {
                        if (double.TryParse(ticker.Price, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double parsedPrice))
                        {
                            Console.WriteLine($"{GetReadableName(ticker.Symbol)}: {parsedPrice:F1} USD");
                        }
                        else
                        {
                            Console.WriteLine($"{ticker.Symbol}: Ogiltigt prisformat - {ticker.Price}");
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ett fel uppstod: {ex.Message}");
                }
            }
        }

        private string GetReadableName(string symbol)
        {
            return symbol switch
            {
                "BTCUSDT" => "Bitcoin (BTC)",
                "ETHUSDT" => "Ethereum (ETH)",
                "XRPUSDT" => "XRP",
                _ => symbol
            };
        }
    }

    public class CryptoTicker
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("price")]
        public string Price { get; set; }
    }
}
