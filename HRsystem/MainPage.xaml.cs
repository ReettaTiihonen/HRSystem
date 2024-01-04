using System.Text.Json;

namespace HRsystem;

public partial class MainPage : ContentPage
{
    //Tehdään reitti ja tiedosto kirjautumislokitiedostolle
    string path = FileSystem.Current.CacheDirectory;
    string filename1 = "LogInformation.json";
    string fullpath1;
    public MainPage()
	{
        InitializeComponent();
        fullpath1 = Path.Combine(path, filename1);
    }
    private bool IsValidUsernameAndPassword(string username, string password)
    {
        //Tehdään käyttäjänimen ja salasanan tarkastus
        return username == password;
    }
    void Login_button_Clicked(System.Object sender, System.EventArgs e)
    {
        //Syötetään käyttäjätunnus ja salasana entry kohtiin
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        if (IsValidUsernameAndPassword(username, password))
        {
            //Jos molemmat oikein siirrytään MainPagelle
            Application.Current.MainPage = new Information();

            //Lisätään kirjautuminen lokiin sekä aika ja käyttäjä 
            string LogUser = $"{DateTime.Now}: User {username} logged in.";

            //Kirjataan json tiedostoon
            string json = JsonSerializer.Serialize(LogUser);
            File.AppendAllText(fullpath1, LogUser + Environment.NewLine); // Sama juttu tässä kuin Information tallennuksessa
        }
        else
        {
            //Jos käyttäjänimi tai salasana väärin, näytetään virhe ilmoitus
            DisplayAlert("Login Failed", "Väärä käyttäjänimi tai salasana", "OK");
        }
    }
}



