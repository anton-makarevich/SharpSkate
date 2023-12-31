using System.Threading.Tasks;
using Sanet.SmartSkating.Models.Training;

namespace Sanet.SmartSkating.Services.Narration
{
    public interface INarratorService
    {
        Task SpeakText(string text);
    }
}