using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnySlinger.Outbox
{
    public interface IBunnyOutboxProcessor
    {
        Task ProcessAsync(CancellationToken stoppingToken = default);
    }
}
