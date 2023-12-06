using System.Security.Cryptography;

namespace AcePacific.Common.Helpers
{
    public static class Helper
    {
        public static string GenerateRandomAccountNumber()
        {
            Random random = new Random();
            // Generate a random 9-digit number.
            string randomNumberString = random.Next(100000000, 999999999).ToString();
            if (randomNumberString.Length < 10)
            {
                randomNumberString = '0' + randomNumberString;
            }
            return randomNumberString;
        }
        public static string GenerateTransactionReference()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string randomComponent = Guid.NewGuid().ToString().Substring(0, 6);
            string reference = $"{timestamp}-{randomComponent}";
            return reference;
        }
        public static string ComputeHash(string plainText)
        {
            SHA1 HashTool = new SHA1Managed();
            Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(string.Concat(plainText));
            Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
            HashTool.Clear();
            return Convert.ToBase64String(EncryptedBytes);
        }
        public static string ComputeInitials(string firstName, string lastName)
        {
            var firstNameInitial = firstName.Substring(0, 1).ToUpper();
            var lastNameInitial = lastName.Substring(0, 1).ToUpper();
            return firstNameInitial + lastNameInitial;
        }
        public static decimal ComputeCharge(decimal amount)
        {
            var chargeAmount = 0m;
            if(amount > 0 && amount <= 1000)
            {
                chargeAmount = amount / 0.05m;
            }else if(amount > 1000 && amount <= 10000)
            {
                chargeAmount = amount / 0.07m;
            }
            else
            {
                chargeAmount = amount / 0.1m;
            }
            return chargeAmount;
        }
    }
}
