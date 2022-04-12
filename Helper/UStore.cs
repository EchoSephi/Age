using System.IO;
using Newtonsoft.Json.Linq;

namespace Age.Helper
{
    public class UStore
    {
        public static string GetUStore(string ConfigurationStirng, string type)
        {
            if (ConfigurationStirng == "")
            {
                using (StreamReader streamReader = new StreamReader("ap.json"))
                {
                    string json = streamReader.ReadToEnd();
                    dynamic val = JArray.Parse(json);
                    dynamic val2 = val[0];
                    switch (type)
                    {
                        case "Root":
                            return val2.Root;
                        case "Issuer":
                            return val2.Issuer;
                        case "SignKey":
                            return val2.SignKey;
                        case "uRI":
                            return val2.uRI;
                        default:
                            return "";
                    }
                }
            }
            return ConfigurationStirng;
        }
    }
}
