using System.Threading.Tasks;
using Sanet.SmartSkating.Models;

namespace Sanet.SmartSkating.Services.Storage
{
    public interface IStorageService
    {
        Task SaveCoordinateAsync(Coordinate coordinate);
    }
}