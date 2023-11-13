using AcePacific.Common.Constants;
using AcePacific.Common.Enums;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AcePacific.Data.ViewModel
{
    internal class CustomerViewModel
    {
    }
    public class BankModel
    {
        public string BankName { get; set; }
        public string BankCode { get; set; }
    }
    public class BankItem : BankModel
    {
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class BankFilter : BankItem
    {
        public string BankName { get; set; }
        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }
        public static BankFilter Deserialize(string whereCondition)
        {
            var filter = new BankFilter();
            if (!string.IsNullOrEmpty(whereCondition))
            {
                filter = JsonConvert.DeserializeObject<BankFilter>(whereCondition);
            }
            return filter;
        }
    }
}
