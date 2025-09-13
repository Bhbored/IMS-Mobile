namespace IMS_Mobile.MVVM.Views;

using System.Diagnostics;
using System.Net;

public partial class ContactUsPage : ContentPage
{
    public ContactUsPage()
    {
        InitializeComponent();
    }
    private async void SendMessageButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            string name = NameEntry.Text ?? "";
            string senderEmail = EmailEntry.Text ?? "";
            string subject = SubjectEntry.Text ?? "";
            string messageBody = MessageEditor.Text ?? "";

            string recipientEmail = "bhboredbusiness@gmail.com";

            string emailSubject = Uri.EscapeDataString($"[IMS Mobile] {subject}");
            string formattedBody = $"From: {name}\nEmail: {senderEmail}\n\n{messageBody}";
            string emailBody = Uri.EscapeDataString(formattedBody);

            string mailtoUri = $"mailto:{recipientEmail}?subject={emailSubject}&body={emailBody}";

            await Launcher.OpenAsync(new Uri(mailtoUri));

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error opening email client: {ex}");
            await DisplayAlert("Error", "Could not open email client. Please check your device settings.", "OK");
        }
    }
}