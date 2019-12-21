using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{
    /// <summary>
    /// Represents a illustration tag
    /// </summary>
    public class Tag : PixivBaseModel, IEquatable<Tag>
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

    public class BookmarkTag : Tag
    {
        public int Count { get; set; }
    }

}
