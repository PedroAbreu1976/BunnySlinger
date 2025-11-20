using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BunnySlinger.Outbox.Options
{
    public class BunnyOutboxOptions
    {
        /// <summary>
        /// Gets or sets the frequency in milliseconds at which the outbox processor checks for new messages to dispatch.
        /// </summary>
        public int ProcessorFrequency { get; set; } = TimeSpan.FromSeconds(1).Milliseconds;

        /// <summary>
        /// Gets or sets the maximum number of retry attempts for dispatching a message before it is considered failed and moved to the failed outbox.
        /// </summary>
        public int? MaxRetryCount { get; set; }

        /// <summary>
        /// Gets or sets the age in seconds after which undelivered messages are considered expired and moved to the failed outbox.
        /// </summary>
        public int ExpireOlderThan { get; set; } = TimeSpan.FromHours(1).Seconds;
    }
}
