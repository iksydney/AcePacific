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
    }
}
