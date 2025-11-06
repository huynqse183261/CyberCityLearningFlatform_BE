namespace CyberCity.DTOs.Subscriptions
{
    public class ModuleAccessDto
    {
        public bool CanAccess { get; set; }
        public bool HasSubscription { get; set; }
        public int ModuleIndex { get; set; }
        public int MaxFreeModules { get; set; }
        public string? Reason { get; set; }
    }
}
