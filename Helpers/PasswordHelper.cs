using System;

namespace Identity.Helpers
{
    public static class PasswordHelper
    {
        public static string GeneratePassword()
        {
            Random res = new Random();
            String str = "abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int size = 10;
            String passwordString = "";
            for (int i = 0; i < size; i++)
                passwordString = passwordString + str[res.Next(26)];
            return passwordString;
        }
    }
}