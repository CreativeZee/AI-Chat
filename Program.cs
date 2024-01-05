using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("-------Welcome to the Credit Card Chat!---------");
        Console.WriteLine("Type 'exit' to end the conversation.");
        Console.WriteLine();
        Console.WriteLine("GPT: Please tell me everything you want for your credit card, and I will find the correct match.");
        Console.WriteLine();

        while (true)
        {
            Console.Write("You: ");
            var userQuestion = Console.ReadLine().Trim().ToLower();
            Console.WriteLine();
            if (userQuestion == "exit")
            {
                break;
            }
            Console.WriteLine("<---------------------fetching best feature--------------------->");
            if (!string.IsNullOrEmpty(userQuestion))
            {
                string gpt3Response = await GetGPT3Response(userQuestion);

                string bestMatchedFeature = MatchWithPredefinedFeatures(userQuestion, gpt3Response);

                if (bestMatchedFeature != null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Best Match Feature is :");
                    Console.WriteLine(bestMatchedFeature);
                    Console.WriteLine();
                    //Console.WriteLine($"Jaccard Similarity (User): {CalculateJaccardSimilarity(userQuestion, bestMatchedFeature)}");
                    //Console.WriteLine($"Jaccard Similarity (GPT-3): {CalculateJaccardSimilarity(gpt3Response, bestMatchedFeature)}");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("System: No Match Found.");
                }
            }
            else
            {
                Console.WriteLine("System: You need to provide some input.");
            }
        }
    }

    static async Task<string> GetGPT3Response(string userQuestion)
    {
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("authorization", "Your API Key");

        var content = new StringContent("{\"model\": \"gpt-3.5-turbo-instruct\", \"prompt\": \"" + userQuestion + "\",\"temperature\": 0.7,\"max_tokens\": 250}",
                    Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);

        string responseString = await response.Content.ReadAsStringAsync();

        try
        {
            var dyData = JsonConvert.DeserializeObject<dynamic>(responseString);
            return dyData?.choices[0]?.text;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"---> Could not deserialize the JSON: {ex.Message}");
            return null;
        }
    }

    static string MatchWithPredefinedFeatures(string userQuestion, string gpt3Response)
    {
        List<string> predefinedFeatures = new List<string>
        {
            "0% APR offer",
            "Low Interest",
            "Balance Transfer offer",
            "Highest Rewards",
            "Best for building credit",
            "No annual fee",
            "No application fee",
            "Welcome Bonuses"
        };

        double bestJaccardSimilarity = 0.0;
        string bestMatch = null;

        foreach (var feature in predefinedFeatures)
        {
            var predefinedWords = feature.ToLower().Split(' ');

            var intersectionUser = predefinedWords.Intersect(userQuestion.ToLower().Split(' '));
            var unionUser = predefinedWords.Union(userQuestion.ToLower().Split(' '));
            var jaccardSimilarityUser = (double)intersectionUser.Count() / unionUser.Count();

            var intersectionGpt3 = predefinedWords.Intersect(gpt3Response.ToLower().Split(' '));
            var unionGpt3 = predefinedWords.Union(gpt3Response.ToLower().Split(' '));
            var jaccardSimilarityGpt3 = (double)intersectionGpt3.Count() / unionGpt3.Count();

            var averageSimilarity = (jaccardSimilarityUser + jaccardSimilarityGpt3) / 2;

            if (averageSimilarity > bestJaccardSimilarity)
            {
                bestJaccardSimilarity = averageSimilarity;
                bestMatch = feature;
            }
        }

        return bestMatch;
    }

    static double CalculateJaccardSimilarity(string text1, string text2)
    {
        var words1 = text1.ToLower().Split(' ');
        var words2 = text2.ToLower().Split(' ');

        var intersection = words1.Intersect(words2);
        var union = words1.Union(words2);

        return (double)intersection.Count() / union.Count();
    }
}
