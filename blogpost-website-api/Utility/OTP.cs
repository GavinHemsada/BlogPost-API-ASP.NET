namespace blogpost_website_api.Utility
{
    public class OTP
    {
         private static readonly Random _random = new Random();
        private static Dictionary<string, (string otp, DateTime expiry)> _otpStore = new();

        // generate otp
        public static string GenerateOTP(string userid)
        {
            const string digits = "0123456789";
            string otp = new string(Enumerable.Range(0, 6).Select(_ => digits[_random.Next(digits.Length)]).ToArray());
            _otpStore[userid] = (otp, DateTime.UtcNow.AddSeconds(300));
            return otp;
        }

        // check otp
        public static bool ValidateOTP(string userId, string inputOtp)
        {
            if (_otpStore.TryGetValue(userId, out var entry))
            {
                if (DateTime.UtcNow <= entry.expiry && entry.otp == inputOtp)
                {
                    _otpStore.Remove(userId); //  remove after use
                    return true;
                }
            }
            return false;
        }
    }
}
