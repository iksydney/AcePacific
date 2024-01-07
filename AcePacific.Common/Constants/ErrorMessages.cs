namespace AcePacific.Common.Constants
{
    public static class ErrorMessages
    {
        public static string GenericError = "Something went wrong, please try again Later";
        public static string UserEmailAlreadyExists = "User with that email already exists";
        public static string UserCreationFailed = "User cration failed";
        public static string UserAlreadyDeactivated = "User already deactivated";
        public static string UserAlreadyActivated = "User already activated";
        public static string UserPasswordResetFailed = "User Password reset failed";
        public static string UserPasswordChangefailed = "User Password change failed";
        public static string UserDoesntExist = "User Doesnt Exist";
        public static string InvalidUserNameOrPassword = "Invalid Username or password";

        public static string OTPNotFound = "OTP Not found";
        public static string ErrorCreatingSecurityQuestion = "Error Creating Security Question";

        public static string Old_Password_Required = "Old Password Required";
        public static string NewPassword_Required = "New Password Required";
        public static string UserNotFound = "User Not Found";
        public static string InvalidCredentials = "Invalid Credentials";
        public static string FailedToRegisterUser = "Failed To Register User";
        public static string PasswordMismatchError = "Password do not Match";
        public static string UserNameOrPasswordIncorrect = "User name or Password is Incorrect";
        public static string phoneNumberExists = "Phone number exists";
        public static string UserNameExists = "User Name exists";
        public static string FailedToRetrieveWallet = "Failed to retrieve wallet";
        public static string AccountNumberDoesntExist = "Account Number does not exist";
        public static string BalanceIsLow = "Balance is low";
        public static string InvalidTransferAmount = "Invalid Transfer Amount";
        public static string FailedToValidateBalance = "Failed To Validate Balance";
        public static string PinNotSetup = "Pin Not Setup";
        public static string IncorrectPin = "Incorrect pin provided";
        public static string Unauthorized = "You are not authorized to view the resource";

        public static string PinNotMatch = "Old Pin and New Pin Do not Match";
    }


    public static class ResponseMessage
    {
        public static string ValidationSuccessfulTransferCanProceed = "Validation successful. Transfer can proceed.";
    }
}
