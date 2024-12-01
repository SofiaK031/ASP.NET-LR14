using OpenTelemetry;
using System.Diagnostics;

namespace LR14
{
    public class ActivityFilteringProcessor(Func<object, bool> func) : BaseProcessor<Activity>
    {
        private readonly Func<object, bool> func = func;

        public override void OnStart(Activity activity)
        {
            string? priority = activity.Baggage.FirstOrDefault(b => b.Key == "priority").Value;

            if (priority == "high")
            {
                activity.SetTag("priority", "high");
            }

            if (!activity.Tags.Any(tag => tag.Key == "priority" && tag.Value == "high"))
            {
                activity.IsAllDataRequested = false;
            }
        }
    }
}
