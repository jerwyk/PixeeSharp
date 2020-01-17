using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{
    public class PixivUserPreview : PixivBaseModel
    {
        public PixivUser User { get; set; }
        [JsonProperty("illusts")]
        public List<PixivIllustration> IllustPreview { get; set; }
    }
}
