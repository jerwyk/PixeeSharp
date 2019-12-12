using PixeeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            PixeeSharpAppApi api = new PixeeSharpAppApi();
            await api.Auth("jerwyk@126.com", "Jerwyk0526");

            var a = await api.SearchIllust("touhou");
            await a.Illustrations[0].GetImage();
        }

    }
}
