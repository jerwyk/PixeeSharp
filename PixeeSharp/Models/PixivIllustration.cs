using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PixeeSharp.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{

    public class ImageSizeUrl
    {
        public string Large { get; set; }
        public string Medium { get; set; }
        public string SquareMedium { get; set; }
        public string Original { get; set; }

        public Uri GetUrl(ImageSize size)
        {
            switch (size)
            {
                case ImageSize.Large:
                    return new Uri(Large);
                case ImageSize.Medium:
                    return new Uri(Medium);
                case ImageSize.SquareMedium:
                    return new Uri(SquareMedium);
                case ImageSize.Original:
                    return new Uri(Original);
                default:
                    return null;
            }
        }

    }

    public struct Tag
    {
        public string Name { get; set; }
        public string TranslatedName { get; set; }
    }

    public class PixivIllustration
    {

        public string Caption { get; set; }
        public string CreateDate { get; set; }
        public int Height { get; set; }
        public long Id { get; set; }
        public ImageSizeUrl ImageUrls { get; set; }
        public bool IsBookmarked { get; set; }
        public bool IsMuted { get; set; }
        public List<ImageSizeUrl> MetaPages { get; set; }
        public int PageCount { get; set; }
        public int Restrict { get; set; }
        public int SanityLevel { get; set; }
        public Dictionary<long, string> Series { get; set; }
        public List<Tag> Tags { get; }
        public string Title { get; set; }
        //Tools property
        public int TotalBookmarks { get; set; }
        public int TotalView { get; set; }
        public string Type { get; set; }
        public PixivUser User { get; set; }
        public bool Visivle { get; set; }
        public int Width { get; set; }
        public int XRestrict { get; set; }

        public PixeeSharpBaseApi Client { get; set; }

        public static PixivIllustration GetIllustrationFromJson(string json)
        {
            return JsonConvert.DeserializeObject<PixivIllustration>(json, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });
        }

        public async Task<Stream> GetImage(ImageSize size = ImageSize.Original, int Index = 0)
        {
            if (Client != null)
            {
                return await Client.DownloadImage(ImageUrls.GetUrl(size)).ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }

    }
}
