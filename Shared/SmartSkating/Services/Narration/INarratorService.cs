using System.Threading.Tasks;

namespace Sanet.SmartSkating.Services.Narration
{
    public interface INarratorService
    {
        Task PlayText(string text);
    }
}