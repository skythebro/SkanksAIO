using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SkanksAIO.Models;

namespace SkanksAIO.Web.Controllers;

[Controller]
internal class IndexController
{
    [Route("/", Methods: new string[] { "GET" })]
    [Template("index.html.twig")]
    public dynamic IndexAction(HttpListenerRequest request)
    {
        return new {
            users = Player.GetRepository.FindAll()
                .OrderByDescending(x => x.KD)
        };
    }

    [Route("/test", Methods: new string[] { "GET" })]
    public async Task<dynamic> TestAction(HttpListenerRequest request)
    {
        await Task.Delay(500);

        return "Routs can also be async!";
    }
}
