using PixeeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using PixeeSharp.Models;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            MainAsync().GetAwaiter().GetResult();
            
        }

        class test
        {
            
            public List<Tag> tags { get; set; }
        }

        static async Task MainAsync()
        {
            PixeeSharpAppApi api = new PixeeSharpAppApi();
            await api.Login("jerwyk@126.com", "Jerwyk0526");

            var a = await api.SearchIllustration("東方Project10000users入り 博麗霊夢");
            await a.SearchAll();
            //await api.GetUserFollowing("10632654");
            //await a.Illustrations[0].GetImage();
        }

    }
}
