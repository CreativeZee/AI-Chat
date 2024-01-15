using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Chat
{
    public class Disctionary
    {
       public Dictionary<string, string> keywordFeatureMapping = new Dictionary<string, string>
        {
            { "0 apr", "0% APR offer" },
            { "no apr", "0% APR offer" },
            { "0% apr", "0% APR offer" },
           {"0% apr offer", "0% APR offer" },
            { "no apr offer", "0% APR offer" },
            { "no APR offer", "0% APR offer" },
            { "0% APR offer", "0% APR offer" },
            { "low interest", "Low Interest" },
            { "0 interest", "Low Interest" },
            { "no interest", "Low Interest" },
            { "interest is low", "Low Interest" },
            { "balance transfer", "Balance Transfer offer" },
            { "balance transfer offer", "Balance Transfer offer" },
            { "highest rewards", "Highest Rewards" },
            { "high reward", "Highest Rewards" },
            { "high rewards", "Highest Rewards" },
            { "highly rewards", "Highest Rewards" },
            { "best for building credit", "Best for building credit" },
            { "building credit", "Best for building credit" },
            { "no annual fee", "No annual fee" },
            { "0 annual fee", "No annual fee" },
            { "no annual", "No annual fee" },
            { "annual fee 0", "No annual fee" },
            { "no annually fee", "No annual fee" },
            { "no annually charges", "No annual fee" },
            { "no application fee", "No application fee" },
            { "no application ", "No application fee" },
            { "0 application fee", "No application fee" },
            { "application fee is 0", "No application fee" },
            { "welcome bonuses", "Welcome Bonuses" },
            { "welcome bonus", "Welcome Bonuses" }
        };
    }
}
