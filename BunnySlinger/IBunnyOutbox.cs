using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnySlinger
{
    public interface IBunnyOutbox
    {
        Task<bool> QueueBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default) where TBunny : IBunny;
    }
}
