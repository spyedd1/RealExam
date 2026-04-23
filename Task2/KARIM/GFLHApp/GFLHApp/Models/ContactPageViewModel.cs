namespace GFLHApp.Models
{
    // Supplies the public contact page with the form fields plus any signed-in user conversation history.
    public class ContactPageViewModel
    {
        // Stores the sender's name entered into the form.
        public string FullName { get; set; } = string.Empty;

        // Stores the sender email, or the signed-in account email when prefilled.
        public string EmailAddress { get; set; } = string.Empty;

        // Stores the form subject entered by the user.
        public string Subject { get; set; } = string.Empty;

        // Stores the message body entered into the contact form.
        public string Message { get; set; } = string.Empty;

        // Stores the signed-in account email when the contact form can be prefilled automatically.
        public string CurrentUserEmail { get; set; } = string.Empty;

        // Lets the view know whether to lock the email field to the signed-in user's account email.
        public bool IsSignedIn { get; set; }

        // Shows the signed-in user's previous inquiries and any admin replies in one place.
        public List<ContactInquiry> MyInquiries { get; set; } = new();
    }
}
