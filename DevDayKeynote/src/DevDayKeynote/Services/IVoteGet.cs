using System.Threading.Tasks;
using DevDayKeynote.Models;

namespace DevDayKeynote.Services
{
    public interface IVoteGet
    {
        Task<VotoResult> GetVoteAsync();
    }
}
