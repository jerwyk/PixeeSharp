using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{
    public class PixivContractResolver : DefaultContractResolver
    {
        private Dictionary<string, string> PropertyMappings { get; set; }

        public PixivContractResolver(string resultName)
        {
            this.PropertyMappings = new Dictionary<string, string>
            {
                {"Result", resultName},
            };            
        }

        public PixivContractResolver(Dictionary<string, string> mapping)
        {
            PropertyMappings = mapping;
        }

        public PixivContractResolver()
        {
            PropertyMappings = new Dictionary<string, string>();
            this.NamingStrategy = new SnakeCaseNamingStrategy();
        }

        //Change PascalCase to snake_case
        private string ResolveSnakeCase(string propertyName)
        {
            return string.Concat(propertyName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            string resolvedName = null;
            var resolved = this.PropertyMappings.TryGetValue(propertyName, out resolvedName);
            return (resolved) ? resolvedName : base.ResolvePropertyName(ResolveSnakeCase(propertyName));
        }
    }
}
