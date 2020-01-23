using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanet.SmartSkating.Dto.Services
{
    public interface IResourceReader
    {
        Task<List<TR>> ReadEmbeddedResourceAsync<T, TR>(string filename);
    }
}