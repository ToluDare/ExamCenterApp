namespace ExamCenterApp.Helpers
{
    public class User_Helper: IUser_Helper
    {
        public string GeneratePassword()
        {
            string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int stringLength = 8;

            Random random = new Random();
            char[] result = new char[stringLength];
            for (int i = 0; i < stringLength; i++)
            {
                result[i] = characters[random.Next(characters.Length)];
            }
            var feedback = $"{new string(result)}{new Random().Next(100):00}";
            return feedback;
        }
    }

}
