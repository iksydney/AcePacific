using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcePacific.Data.ViewModel
{
    public class WalletViewModel
    {
        public decimal WalletBalance { get; set; }
        public string UserId { get; set; }
        public string TransactionReference { get; set; }
    }   
    public class WalletItem : WalletViewModel
    {
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public class WalletFilter : WalletItem
    {
        public DateTime? DateCreatedFrom { get; set; }
        public DateTime? DateCreatedTo { get; set; }
        public static WalletFilter Deserialize(string whereCondition)
        {
            var filter = new WalletFilter();
            if (!string.IsNullOrEmpty(whereCondition))
            {
                filter = JsonConvert.DeserializeObject<WalletFilter>(whereCondition);
            }
            return filter;
        }
    }
    public class CreatWalletViewModel
    {
        public string UserId { get; set; }
        public string WalletAccountNumber { get; set; }
    }
}
