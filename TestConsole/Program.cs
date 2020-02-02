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

        static async Task MainAsync()
        {
            try
            {
                PixeeSharpAppApi api = new PixeeSharpAppApi();
                await api.Login("user_wmxv8884", "Rkd4BeQD4Ynr76u");

                var a = await api.GetTrendingTags();
                //await a.SearchAll();
                //await a.SearchAll();
                //await api.GetUserFollowing("10632654");
                //await a.Illustrations[0].GetImage();
            }
            catch(Exception ex)
            {

            }
        }

    }
}
