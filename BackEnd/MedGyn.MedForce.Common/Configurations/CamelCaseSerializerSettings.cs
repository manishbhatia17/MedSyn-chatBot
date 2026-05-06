using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MedGyn.MedForce.Common.Configurations
{
    public static class CamelCaseSerializerSettings
    {
        public static JsonSerializerSettings Settings
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Formatting = Formatting.None
                };
            }
        }
    }
}
