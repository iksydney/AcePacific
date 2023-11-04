using System.ComponentModel;

namespace AcePacific.Common.Enums
{
    public enum ApprovalStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Completed")]
        Completed,
        [Description("Action Needed")]
        ActionNeeded
    }
}
