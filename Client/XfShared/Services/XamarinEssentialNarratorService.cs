using System.Threading.Tasks;
using Sanet.SmartSkating.Services.Narration;
using Xamarin.Essentials;

namespace Sanet.SmartSkating.Xf.Services
{
    public class XamarinEssentialNarratorService:INarratorService
    {
        public async Task SpeakText(string text)
        {
            await TextToSpeech.SpeakAsync(text);
        }
    }
}