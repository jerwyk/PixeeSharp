﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixeeSharp.Models
{

    public interface IPixivModel
    {
        PixeeSharpBaseApi Client { get; set; }
    }

    public class PixivBaseModel : IPixivModel
    {
        public PixeeSharpBaseApi Client { get ; set; }

        public static T Parse<T>(string json, PixeeSharpBaseApi client) where T : IPixivModel
        {
            var res = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });
            res.Client = client;
            return res;
        }
    }
}
