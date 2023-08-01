using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SkanksAIO.Models;
using SkanksAIO.Utils;

namespace SkanksAIO.Web.Controllers;

[Controller]
internal class IndexController
{
    [Route("/", Methods: new[] { "GET" })]
    [Template("index.html.twig")]
    public dynamic IndexAction(HttpListenerRequest request)
    {
        return new {
            users = Player.GetPlayerRepository.FindAll()
                .OrderByDescending(x => x.KD)
        };
    }
}
