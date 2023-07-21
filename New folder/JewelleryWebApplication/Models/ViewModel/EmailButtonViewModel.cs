namespace JewelleryWebApplication.Models.ViewModel
{
    public class EmailButtonViewModel
    {
        public EmailButtonViewModel(string text, string url, string email, string password)
        {
            Text = text;
            Url = url;
            Email = email;
            Password = password;
        }

        public string Text { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
