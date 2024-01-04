namespace HRsystem;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Collections.Generic;

public partial class Information : ContentPage
{
    //Luodaan yhteys listview
    private ObservableCollection<PersonInfo> persons;
    //Tämän avulla saadaan listan järjestysnapit toimimaan ja päivitetään listaa tietojen muuttuessa
    public ObservableCollection<PersonInfo> Persons
    {
        //Palauttaa yllä tehdyn yhteyden
        get{ return persons; }
        set
        {
            //Asetetaan arvo jos ei ole arvoa
            if (persons != value)
            {
                persons = value;
                //Ilmoitetaan listalle, että on tullut muutoksia
                OnPropertyChanged(nameof(Persons));
                //Päivitetään listaa
                foreach (var person in persons)
                {
                    OnPropertyChanged(nameof(person.Names));
                    OnPropertyChanged(nameof(person.LastName));
                    OnPropertyChanged(nameof(person.NickName));
                    OnPropertyChanged(nameof(person.Hetu));
                    OnPropertyChanged(nameof(person.Street));
                    OnPropertyChanged(nameof(person.MailNumber));
                    OnPropertyChanged(nameof(person.Location));
                    OnPropertyChanged(nameof(person.JobStart));
                    OnPropertyChanged(nameof(person.JobEnd));
                    OnPropertyChanged(nameof(person.JobTitle));
                    OnPropertyChanged(nameof(person.Department));
                }
            }
        }
    }
    //Tehdään reitti ja tiedosto kirjautumislokitiedostolle
    string path = FileSystem.Current.CacheDirectory;
    string filename1 = "LogInformation.json";
    string fullpath1;
    //Tehdään sama juttu tallennettaville tiedoille
    string filename2 = "PersonAndEmployeeInformation.json";
    string fullpath2;
    //Tehdään päättymispäivä pickeristä sellainen, että sen voi jättää tyhjäksi
    private DateTime? EndDate;
   
    public Information()
    {
        InitializeComponent();
        Persons = new ObservableCollection<PersonInfo>();
        BindingContext = this;
        //Yhdistetään tiedostot ja polku
        fullpath1 = Path.Combine(path, filename1);
        fullpath2 = Path.Combine(path, filename2);
        //Tehdään paikan ehdotus postinumeroa kirjoittaessa
        MailNumberEntry.TextChanged += MailNumberEntry_TextChanged;
        //Liitetään DatePicker funktioon
        EndPicker.DateSelected += EndPicker_DateSelected;
    }

    protected override void OnAppearing()
    {    
        base.OnAppearing();
        //Testataan oliko kirjautumisen tiedot oikein
        if (!IsUserValid())
        {
            //Jos kirjautumisessa väärä käyttäjätunnus tai salasana, mennään takaisin kirjautumissivulle
            Application.Current.MainPage = new MainPage();
        }
    }

    private bool IsUserValid()
    {
        //Tehdään funktio helpottamaan ylemmmän funktion toimintaa
        return true;
    }

    private void SavedChange()
    {
        //Otetaan talteen kaikki sovelluksessa kirjoitettu tieto
        FirstNamesEntry.TextChanged += FirstNamesEntry_TextChanged;
        LastNameEntry.TextChanged += FirstNamesEntry_TextChanged;
        NickNameEntry.TextChanged += FirstNamesEntry_TextChanged;
        HetuEntry.TextChanged += FirstNamesEntry_TextChanged;
        StreetEntry.TextChanged += FirstNamesEntry_TextChanged;
        MailNumberEntry.TextChanged += FirstNamesEntry_TextChanged;
        LocationEntry.TextChanged += FirstNamesEntry_TextChanged;
        JobTitleEntry.TextChanged += FirstNamesEntry_TextChanged;
        UnitEntry.TextChanged += FirstNamesEntry_TextChanged;
    }

    void FirstNamesEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        //Otetaan talteen uusi syötetty teksti
        Entry entry = (Entry)sender;
        string newText = entry.Text;
        //Tehdään tarkistusta varten
        string previousText = string.Empty;
        //Tarkistetaan onko teksti muuttunut
        string activity;
        if (newText != previousText)
        {
            //Otetaan muuttunut teksti talteen lokiin
            activity = $"Changed {entry.Placeholder} to '{newText}'";
            //Kirjataan json tiedostoon
            string json2 = JsonSerializer.Serialize(activity);
            File.AppendAllText(fullpath1, json2 + Environment.NewLine); //Lisätään tallennettuun tiedostoon rivi, jotta lukeminen helpottuu
        }
        //Päivitetään previoustext seuraavaan
        previousText = newText;
    }
    private string EncryptText(string originalText)
    {
        //Lisätään syötettyihin tietoihin "salaus", muutetaan a kirjaimet c kirjaimiksi ja niin edelleen
        char[] chars = originalText.ToCharArray();
        for(int i = 0; i < chars.Length; i++)
        {
            chars[i] = (char)(chars[i] + 2);
        }
        //Palautetaan salatut merkit
        return new string(chars);
    }

    void Savebutton_Clicked(System.Object sender, System.EventArgs e)
    {
        //Luetaan "salatut" tiedot ja tallennetaan tiedostoon (eri tiedosto kuin loki tiedoilla)
        PersonInfo data = new PersonInfo();
        data.Names = EncryptText(FirstNamesEntry.Text);
        data.LastName = EncryptText(LastNameEntry.Text);
        data.NickName = EncryptText(NickNameEntry.Text);
        data.Hetu = EncryptText(HetuEntry.Text);

        //Tarkastetaan onko syötetty hetu oikein
        if(!IsHetuValid(DecryptText(data.Hetu)))
        {
            DisplayAlert("Error", "Hetu väärässä muodossa", "OK");
            return;
        }

        data.Street = EncryptText(StreetEntry.Text);
        data.MailNumber = EncryptText(MailNumberEntry.Text);
        data.Location = EncryptText(LocationEntry.Text);
        data.JobStart = StartPicker.Date;
        data.JobEnd = EndPicker.Date;
        data.JobTitle = EncryptText(JobTitleEntry.Text);
        data.Department = EncryptText(UnitEntry.Text);

        //Lisätään tiedot listaan
        try
        {
            if (File.Exists(fullpath2))
            {
                //Lisätään tiedot olemassaolevaan tiedostoon
                string existingFile = File.ReadAllText(fullpath2);
                List<PersonInfo> existingPerson;
                //Tarkistetaan, onko tiedosto tyhjä
                if (!string.IsNullOrEmpty(existingFile))
                {
                    existingPerson = JsonSerializer.Deserialize<List<PersonInfo>>(existingFile);
                }
                else
                {
                    //Jos tiedosto on tyhjä, tehdään uusi lista
                    existingPerson = new List<PersonInfo>();
                }
                //Lisätään data listaan
                existingPerson.Add(data);
                //Serialisoidaan lista
                string updatejson = JsonSerializer.Serialize(existingPerson);
                //Kirjoitetaan tiedostoon 
                File.WriteAllText(fullpath2, updatejson);
                //Tyhjennetään olemassa oleva data listasta (muuten näytti listassa tiedot tuplina, kun kirjoitti uusia tietoja)
                Persons.Clear();
                //Nollataan päivämäärät
                StartPicker.Date = DateTime.Now;
                EndPicker.Date = DateTime.Now;
                //Lisätään tiedot listaan
                foreach (var person in existingPerson)
                {
                    person.Names = DecryptText(person.Names);
                    person.LastName = DecryptText(person.LastName);
                    person.NickName = DecryptText(person.NickName);
                    person.JobTitle = DecryptText(person.JobTitle);
                    Persons.Add(person);
                }
            }
            if (!File.Exists(fullpath2))
            {
                //Luodaan uusi lista, lisätään data listaan ja kirjoitetaan tiedostoon
                List<PersonInfo> NewFile = new List<PersonInfo>();
                NewFile.Add(data);
                string newjson = JsonSerializer.Serialize(NewFile);
                File.WriteAllText(fullpath2, newjson);
                //Lisätään listaan näkyväksi
                foreach(var person1 in NewFile)
                {
                    person1.Names = DecryptText(person1.Names);
                    person1.LastName = DecryptText(person1.LastName);
                    person1.NickName = DecryptText(person1.NickName);
                    person1.JobTitle = DecryptText(person1.JobTitle);
                    Persons.Add(person1);
                }
            }
            //Nollataan päivämäärät
            StartPicker.Date = DateTime.Now;
            EndPicker.Date = DateTime.Now;
            //Tyhjennetään kentät
            EmptyFields();
        }
        catch(Exception ex)
        {
            DisplayAlert("Error", "Virhe tallennettaessa tiedostoa", "OK");
        }
    }

    void EmptyFields()
    {
        //Tehdään funktio kenttien tyhjentämiselle. Näin sitä ei tarvitse kirjoittaa moneen paikkaan
        FirstNamesEntry.Text = "";
        LastNameEntry.Text = "";
        NickNameEntry.Text = "";
        HetuEntry.Text = "";
        StreetEntry.Text = "";
        MailNumberEntry.Text = "";
        LocationEntry.Text = "";
        JobTitleEntry.Text = "";
        UnitEntry.Text = "";
    }

    private string DecryptText(string encryptedText)
    {
        //Tehdään salauksen purkaminen funktiossa ja käytetään funktiota tiedoston lukemisessa
        char[] chars = encryptedText.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = (char)(chars[i] - 2);
        }
        //Palautetaan puretut merkit
        return new string(chars);
    }

    void MailNumberEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    { 
        //Tehdään yhteys postinumero entryyn
        Entry entry = (Entry)sender;
        string enteredmailnumber = entry.Text;

        PersonInfo match = null;
        //Tehdään ehto, että postinumero on vähintään 2 merkkiä ja, että tiedosto on olemassa
        if(enteredmailnumber.Length >= 2 && File.Exists(fullpath2))
        {
            //Otetaan syötteestä talteen 2 ensimmäistä merkkiä
            string firsttwo = enteredmailnumber.Substring(0, 2);
            //Luetaan tiedosto ja etsitään, onko vastaavia numeroita
            string readFile = File.ReadAllText(fullpath2);
            List<PersonInfo> personsformailnumber = JsonSerializer.Deserialize<List<PersonInfo>>(readFile);

            foreach (PersonInfo person in personsformailnumber)
            {
                if (DecryptText(person.MailNumber).StartsWith(firsttwo))
                {
                    match = person;
                    break;
                }
            }
            if (match != null)
            {
                //Kun samanlainen löydetty, lisätään paikkaehdotus
                LocationEntry.Text = DecryptText(match.Location);
            }
        }
    }

    void EndPicker_DateSelected(System.Object sender, Microsoft.Maui.Controls.DateChangedEventArgs e)
    {
        //Päivitetään picker, kun päivämäärä valitaan (tämä nyt saattaa olla vähän turha)
        EndDate = e.NewDate;
    }

    async void DeleteButton_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            string namesEntry = FirstNamesEntry.Text;
            string lastEntry = LastNameEntry.Text;
            string nickEntry = NickNameEntry.Text;
            string hetuEntry = HetuEntry.Text;
            string streetEntry = StreetEntry.Text;
            string mailnumberEntry = MailNumberEntry.Text;
            string locationEntry = LocationEntry.Text;
            string jobEntry = JobTitleEntry.Text;
            string unitEntry = UnitEntry.Text;

            //Poistetaan tietoja ja näytetään varmistusviesti 
            if (!string.IsNullOrEmpty(namesEntry) || !string.IsNullOrEmpty(lastEntry) || !string.IsNullOrEmpty(nickEntry) || !string.IsNullOrEmpty(hetuEntry) || !string.IsNullOrEmpty(streetEntry)
                || !string.IsNullOrEmpty(mailnumberEntry) || !string.IsNullOrEmpty(locationEntry) || !string.IsNullOrEmpty(jobEntry) || !string.IsNullOrEmpty(unitEntry))
            {
                bool answer = await DisplayAlert("Confirm", "Haluatko varmasti poistaa?", "Yes", "No");
                //Jos vastataan kyllä, käydään tämä ehto läpi
                if (answer)
                {
                    //Luetaan tiedosto
                    string read = File.ReadAllText(fullpath2);
                    List<PersonInfo> removable = JsonSerializer.Deserialize<List<PersonInfo>>(read);
                    //Etsitään tiedostosta sopiva henkilö, kuka halutaan poistaa
                    PersonInfo personsRemove = removable.FirstOrDefault(person =>
                    person.Names == EncryptText(namesEntry) &&
                    person.LastName == EncryptText(lastEntry) &&
                    person.NickName == EncryptText(nickEntry) &&
                    person.Hetu == EncryptText(hetuEntry) &&
                    person.Street == EncryptText(streetEntry) &&
                    person.MailNumber == EncryptText(mailnumberEntry) &&
                    person.Location == EncryptText(locationEntry) &&
                    person.JobTitle == EncryptText(jobEntry) &&
                    person.Department == EncryptText(unitEntry));

                    //Jos löytyy sama henkilö
                    if (personsRemove != null)
                    {
                        //Poistetan listalta
                        removable.Remove(personsRemove);
                        //Päivitetään tiedosto
                        string updatejson = JsonSerializer.Serialize(removable);
                        File.WriteAllText(fullpath2, updatejson);
                        //Päivitetään listanäkymä 
                        Persons.Clear();
                        foreach(var person in removable)
                        {
                            person.Names = DecryptText(person.Names);
                            person.LastName = DecryptText(person.LastName);
                            person.NickName = DecryptText(person.NickName);
                            person.JobTitle = DecryptText(person.JobTitle);
                            Persons.Add(person);
                        }
                        //Nollataan päivämäärät
                        StartPicker.Date = DateTime.Now;
                        EndPicker.Date = DateTime.Now;
                        //Tyhjennetään kentät
                        EmptyFields();
                    }
                    else
                    {
                        DisplayAlert("Error", "Henkilöä ei löytynyt", "OK");
                    }
                }
            }
            else
            {
                DisplayAlert("Error", "Valitse henkilö poistettavaksi", "OK");
            }
        }
        catch(Exception ex)
        {
            DisplayAlert("Error", "$Virhe poistaessa henkilöä: {ex.Message}", "OK");
        }
    }

    void PersonList_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            //Saadaan valittu rivi
            PersonInfo selected = (PersonInfo)e.SelectedItem;
            //Näytetään tiedot lomakkeen kentissä decryptattuina
            FirstNamesEntry.Text = selected.Names;
            LastNameEntry.Text = selected.LastName;
            NickNameEntry.Text = selected.NickName;
            HetuEntry.Text = DecryptText(selected.Hetu);
            StreetEntry.Text = DecryptText(selected.Street);
            MailNumberEntry.Text = DecryptText(selected.MailNumber);
            LocationEntry.Text = DecryptText(selected.Location);
            StartPicker.Date = selected.JobStart;
            EndPicker.Date = selected.JobEnd;
            JobTitleEntry.Text = selected.JobTitle;
            UnitEntry.Text = DecryptText(selected.Department);    
        }
    }

    public static bool IsHetuValid(string hetu)
    {
        if(hetu.Length != 11)
        {
            return false;
        }
        //Erotellaan syötetty hetu osiin
        string dateofbirth = hetu.Substring(0, 6); //Kuusi ensimmäistä merkkiä eli numeroa
        char valimerkki = hetu[6]; //Seitsemännes merkki
        string hetupart = hetu.Substring(7, 3); //3 numeroa kahdeksannesta merkistä eteenpäin
        char tunniste = hetu[10]; //Yhdennestoista merkki
        //Tehdään etu ja loppuosasta merkkijono
        string numbers = hetu.Substring(0,6)+ hetu.Substring(7,3);
        //Tarkistetaan onko päivämäärä syötetty oikein
        if(!IsValid(dateofbirth))
        {
            return false;
        }
        //Tarkistetaan onko väliviiva tai merkki oikein
        if(valimerkki != '+' && valimerkki != '-' && valimerkki != 'A')
        {
            return false;
        }
        //Tarkastetaan onko loppuosa numero
        if(!int.TryParse(hetupart, out _))
        {
            return false;
        }
        //Tehdään laskut ja ehdot tarkastusmerkille
        double tarkistus1 = double.Parse(numbers) / 31;
        //Tällä saadaan tarkistusnumero
        int tarkistus2 = (int)((tarkistus1 - Math.Floor(tarkistus1)) * 31);
        //Luetellaan sallitut merkit
        string sallitut = "0123456789ABCDEFHJKLMNPRSTUVWXY";
        char haluttutarkistus = sallitut[tarkistus2];
        return tunniste == haluttutarkistus;
    }

    private static bool IsValid(string dateofbirth)
    {
        if (dateofbirth.Length != 6)
        {
            //Jos pituus on väärä
            return false;
        }

        //Otetaan syötetystä syntymäajasta erilleen päivä, kuukausi ja vuosi
        string day = dateofbirth.Substring(0, 2);
        string month = dateofbirth.Substring(2, 2);
        string year = dateofbirth.Substring(4, 2);
        //Luodaan intit
        int day1, month1, year1;

        //Tehdään ehto syötteelle
        if (!int.TryParse(day, out day1) || !int.TryParse(month, out month1) || !int.TryParse(year, out year1))
        {
            //Jos ei ole oikein
            return false;
        }
        //Jos on oikein
        return true;
    }

    //Tehdään bool funktio lajittelun järjestämiseen
    private bool IsAscending = true;

    private void SortByNickname()
    {
        //Järjestetään lista kutsumanimen mukaan nousevasti ja laskevasti ja kutsutaan tätä funktiota nappia painaessa
        if (IsAscending)
        {
            Persons = new ObservableCollection<PersonInfo>(Persons.OrderBy(p => p.NickName));
        }
        else
        {
            Persons = new ObservableCollection<PersonInfo>(Persons.OrderByDescending(p => p.NickName));
        }
        //Käännetään boolean toisinpäin
        IsAscending = !IsAscending;
    }

    private void SortByLastname()
    {
        //Tehdään sama sukunimelle
        if (IsAscending)
        {
            Persons = new ObservableCollection<PersonInfo>(Persons.OrderBy(p => p.LastName));
        }
        else
        {
            Persons = new ObservableCollection<PersonInfo>(Persons.OrderByDescending(p => p.LastName));
        }
        IsAscending = !IsAscending;
    }

    private void SortByJobtitle()
    {
        //Ja sama nimikkeelle
        if (IsAscending)
        {
            Persons = new ObservableCollection<PersonInfo>(Persons.OrderBy(p => p.JobTitle));
        }
        else
        {
            Persons = new ObservableCollection<PersonInfo>(Persons.OrderByDescending(p => p.JobTitle));
        }
        IsAscending = !IsAscending;
    }

    void SotrByNi_Clicked(System.Object sender, System.EventArgs e)
    {
        //Kutsutaan tehtyä funktiota
        SortByNickname();
    }    

    void SortByLa_Clicked(System.Object sender, System.EventArgs e)
    {
        //Sama näihin kahteen myös
        SortByLastname();
    }

    void SortByJo_Clicked(System.Object sender, System.EventArgs e)
    {
        SortByJobtitle();
    }
}