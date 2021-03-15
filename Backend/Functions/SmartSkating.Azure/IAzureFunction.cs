using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Sanet.SmartSkating.Backend.Azure
{
    public interface IAzureFunction
    {
        Task<IActionResult> Run(HttpRequest request, IBinder binder, ILogger logger);
    }
}