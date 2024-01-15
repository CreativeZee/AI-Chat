using Newtonsoft.Json;
using System.Text;
using AI_Chat;
using Google.Protobuf;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("-------Welcome to the Credit Card Chat!---------");
        Console.WriteLine("Type 'exit' to end the conversation.");
        Console.WriteLine();
        Console.WriteLine("GPT: Please tell me everything you want for your credit card, and I will find the correct match.");
        Console.WriteLine();
        Disctionary disctionary = new Disctionary();

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

        while (true)
        {
            Console.Write("You: ");
            var userQuestion = Console.ReadLine()?.Trim();
            Console.WriteLine();

            if (userQuestion == "exit")
            {
                break;
            }

            Console.WriteLine("<---------------------fetching matched feature--------------------->");

            if (!string.IsNullOrEmpty(userQuestion))
            {
                string jsonData = JsonConvert.SerializeObject(predefinedFeatures);
                var Prompt = ". Kindly matched question with Data and then give me matched predefined features";
                string requestMessage = userQuestion + " " + Prompt + " " + $"Data: {jsonData}";
                string gpt3Response = await GetGPT3Response(requestMessage);
                List<string> response = ExtractFeaturesFromGPT3Response(userQuestion, gpt3Response, predefinedFeatures);
                List<string> matchedFeatures = MatchWithPredefinedFeatures(userQuestion,gpt3Response, disctionary.keywordFeatureMapping);

                if (matchedFeatures != null && matchedFeatures.Count > 0)
                {
                    HashSet<string> uniqueFeatures = new HashSet<string>(matchedFeatures);

                    Console.WriteLine();
                    Console.WriteLine("Matched Features are:");
                    Console.WriteLine();

                    foreach (var feature in uniqueFeatures)
                    {
                        Console.WriteLine(feature);
                    }

                    Console.WriteLine();
                }
                else if (response != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Matched Features are:");
                    Console.WriteLine(response);
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

    static List<string> MatchWithPredefinedFeatures(string userQuestion,string gptResponse, Dictionary<string, string> keywordFeatureMapping)
    {
        string userQuestionLower = userQuestion.ToLower();
        var matchedFeatures = new List<string>();
        foreach (var mapping in keywordFeatureMapping)
        {
            if (userQuestionLower.Contains(mapping.Key))
            {
                matchedFeatures.Add(mapping.Value);
            }
        }

        return matchedFeatures;
    }
    static List<string> ExtractFeaturesFromGPT3Response(string userQuestion, string gpt3Response, List<string> predefinedFeatures)
    {
        var extractedFeatures = gpt3Response?.Split(' ').Select(f => f.Trim()).ToList();
        var matchedFeatures = new List<string>();

        foreach (var predefinedFeature in predefinedFeatures)
        {
            var featureParts = predefinedFeature.ToLower().Split(' ');
            if (featureParts.Any(part => userQuestion.ToLower().Contains(part)) && featureParts.Any(part => extractedFeatures.Any(extractedWord => extractedWord.ToLower().Contains(part))))
            {
                matchedFeatures.Add(predefinedFeature);
            }
        }

        return matchedFeatures;
    }

    static async Task<string> GetGPT3Response(string userQuestion)
    {
        try
        {
            string GPTmodel = "gpt-3.5-turbo-instruct";
            int Temperature = 1;
            int maximumTokens = 1000;
            string OpenaiApiKey = "API KEY";
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenaiApiKey}");

                var requestObject = new
                {
                    model = GPTmodel,
                    prompt = userQuestion,
                    temperature = Temperature,
                    max_tokens = maximumTokens
                };
                var content = new StringContent(JsonConvert.SerializeObject(requestObject), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/completions", content);

                string responseString = await response.Content.ReadAsStringAsync();

                try
                {
                    var dyData = JsonConvert.DeserializeObject<dynamic>(responseString);

                    if (dyData != null && dyData.choices != null && dyData.choices.Count > 0 && dyData.choices[0] != null)
                    {
                        string generatedText = dyData.choices[0].text;
                        return generatedText;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }

}
