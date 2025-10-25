namespace CyberCity.DTOs.Labs
{
    public class LabDto
    {
        public string Uid { get; set; }
        public string ModuleUid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LabType { get; set; } // single_vm, network_vm, external
        public bool IsRequired { get; set; }
        public int OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class LabComponentDto
    {
        public string Uid { get; set; }
        public string LabUid { get; set; }
        public string ComponentName { get; set; }
        public string ComponentType { get; set; } // attacker_vm, target_vm, tool, network_device
        public string OsInfo { get; set; }
        public string AccessDetails { get; set; }
        public bool IsVulnerable { get; set; }
        public int OrderIndex { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class LabWithComponentsDto
    {
        public LabDto Lab { get; set; }
        public List<LabComponentDto> Components { get; set; }
    }
}
