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

    /// <summary>
    /// This wrapper is needed for the meta_pages property
    /// </summary>
    public class ImageSizeUrlWrapper
    {
        [JsonProperty("image_urls")]
        private ImageSizeUrl _ImageSizeUrl { get; set; }
        public string Large { get => _ImageSizeUrl.Large; set => _ImageSizeUrl.Large = value; }
        public string Medium { get => _ImageSizeUrl.Medium; set => _ImageSizeUrl.Medium = value; }
        public string SquareMedium { get => _ImageSizeUrl.SquareMedium; set => _ImageSizeUrl.SquareMedium = value; }
        public string Original { get => _ImageSizeUrl.Original; set => _ImageSizeUrl.Original = value; }
        public Uri GetUrl(ImageSize size)
        {
            return _ImageSizeUrl.GetUrl(size);
        }
    }

    //Needed because of the weired way pixiv formats its data
    public class MetaImageUrl
    {
        [JsonProperty("original_image_url")]
        public string Url { get; set; }
    }

    /// <summary>
    /// Represents a illustration tag
    /// </summary>
    public class Tag : IEquatable<Tag>
    {
        public string Name { get; set; }
        [JsonProperty("translated_name")]
        public string TranslatedName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Tag t)
            {
                return Name == t.Name;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Tag left, Tag right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Tag left, Tag right)
        {
            return !(left == right);
        }

        public bool Equals(Tag other)
        {
            return Name == other.Name;
        }
    }

    public class PixivSeries
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    /// <summary>
    /// This wrapper class is need for the GetIllustration method
    /// </summary>
    internal class PixivIllustrationWrapper : PixivBaseModel
    {
        [JsonProperty("illust")]
        public PixivIllustration PixivIllustration { get; set; }
    }

    public class PixivIllustration : PixivBaseModel
    {

        public string Caption { get; set; }
        public string CreateDate { get; set; }
        public int Height { get; set; }
        public long Id { get; set; }
        public ImageSizeUrl ImageUrls { get; set; }
        public bool IsBookmarked { get; set; }
        public bool IsMuted { get; set; }
        public MetaImageUrl MetaSinglePage { get; set; }
        public List<ImageSizeUrlWrapper> MetaPages { get; set; }
        public int PageCount { get; set; }
        public int Restrict { get; set; }
        public int SanityLevel { get; set; }
        public PixivSeries Series { get; set; }
        public List<Tag> Tags { get; set; }
        public string Title { get; set; }
        //Tools property
        public int TotalBookmarks { get; set; }
        public int TotalView { get; set; }
        public string Type { get; set; }
        public PixivUser User { get; set; }
        public bool Visivle { get; set; }
        public int Width { get; set; }
        public int XRestrict { get; set; }

        /// <summary>
        /// Generates a instance of the class from a json string
        /// </summary>
        /// <param name="json">The json string to be deserialized</param>
        /// <param name="client">The client that gets the json</param>
        /// <param name="useWrapper">A wrapper is sometimes required for some apis</param>
        /// <returns>The illustration generated from the json</returns>
        public static PixivIllustration Parse(string json, PixeeSharpBaseApi client = null, bool useWrapper = false)
        {
            if (!useWrapper)
            {
                return Parse<PixivIllustration>(json, client);
            }
            else
            {
                return Parse<PixivIllustrationWrapper>(json, client).PixivIllustration;
            }
        }

        public async Task<Stream> GetImage(ImageSize size = ImageSize.Original, int index = 0)
        {
            if (Client != null)
            {
                Uri imgUrl;
                if (MetaPages.Count == 0)//if illust has only one image
                {
                    imgUrl = size == ImageSize.Original ? new Uri(MetaSinglePage.Url) : ImageUrls.GetUrl(size);
                }
                else//if contains multiple images
                {
                    imgUrl = MetaPages[index].GetUrl(size);                   
                }

                return await Client.DownloadImage(imgUrl).ConfigureAwait(false);

            }
            else
            {
                return null;
            }
        }

    }
}
