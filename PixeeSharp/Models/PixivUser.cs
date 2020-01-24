using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{
    public class PixivUser : PixivBaseModel
    {

        public string Account { get; set; }
        public long ID { get; set; }
        public string Name { get; set; }
        public ImageSizeUrl ProfileImageUrls { get; set; }
        public bool IsMailAuthorized { get; set; }

    }
}
