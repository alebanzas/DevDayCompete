using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevDayKeynote.Services
{
    public interface IVoteLog
    {
        Task SendVoteAsync(string community, string user);
    }
}
