namespace hellodoc.DAL.ViewModels
{
    public class PayrateVm
    {
        public string? AspId { get; set;}

        public int? Phyid { get; set; }

        public List<PayRateForProviderVm>? PayrateForProvider { get; set;}
    }

    public class PayRateForProviderVm
    {
        public int? Categoryid { get; set; }

        public string? Categoryname { get; set; }

        public int? PayrateValue { get; set; }
    }
}