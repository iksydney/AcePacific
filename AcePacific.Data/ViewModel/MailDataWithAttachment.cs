using Microsoft.AspNetCore.Http;

namespace AcePacific.Data.ViewModel
{
    public class MailDataWithAttachment : MailData
    {
        public IFormFileCollection? EmailAttachments { get; set; }
    }
}
