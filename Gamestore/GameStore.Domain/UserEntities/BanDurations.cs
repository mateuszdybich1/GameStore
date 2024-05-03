using System.ComponentModel;

namespace GameStore.Domain.UserEntities;
public enum BanDurations
{
    [Description("1 hour")]
    Hour,
    [Description("1 week")]
    Week,
    [Description("1 month")]
    Month,
    [Description("permanent")]
    Permanent,
}
