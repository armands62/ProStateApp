using static ProStateApp2.App;
using MySql.Data.MySqlClient;

namespace ProStateApp2;

public partial class Profile : ContentPage
{
    public Profile()
    {
        InitializeComponent();

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RetrieveDataFromDb();
    }


    private void RetrieveDataFromDb()
    {
        string connectionString = "server=192.168.1.198;user=armands;database=pro_state;password=qwerty;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM user WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", GlobalVariables.CurrentUser.id);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    GlobalVariables.CurrentUser.name = reader.GetString("name");
                    GlobalVariables.CurrentUser.surname = reader.GetString("surname");
                    GlobalVariables.CurrentUser.email = reader.GetString("email");
                    GlobalVariables.CurrentUser.birth_date = reader.GetDateTime("birth_date");
                    GlobalVariables.CurrentUser.gender = reader.GetString("gender");

                    NameLabel.Text = GlobalVariables.CurrentUser.name + " " + GlobalVariables.CurrentUser.surname;
                    if (NameLabel.Text.Length > 15)
                    {
                        NameLabel.FontSize = 40;
                    }
                    else
                    {
                        NameLabel.FontSize = 50;
                    }
                    if (NameLabel.Text.Length > 19)
                    {
                        NameLabel.FontSize = 30;
                    }

                    EmailLabel.Text = GlobalVariables.CurrentUser.email;

                    DateTime BirthDate = GlobalVariables.CurrentUser.birth_date;

                    string BirthDateString = BirthDate.ToString("dd/MM/yyyy");

                    BirthDateLabel.Text = BirthDateString;

                    string Gender = GlobalVariables.CurrentUser.gender;
                    string GenderString;

                    if (Gender == "male")
                    {
                        GenderString = "Vīrietis";
                        GenderIcon.Source = "male_user.png";
                    }
                    else if (Gender == "female")
                    {
                        GenderString = "Sieviete";
                        GenderIcon.Source = "female_user.png";
                    }
                    else
                    {
                        GenderString = "Nezinu";
                        GenderIcon.Source = "question_mark.png";
                    }

                    GenderLabel.Text = GenderString;
                }
            }
        }
    }

    private async void Edit_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("EditProfile");
    }
}