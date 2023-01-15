namespace ProStateApp2;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using static ProStateApp2.App;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();

    }


    private void Login_Clicked(object sender, EventArgs e)
    {
        string connectionString = "server=192.168.1.198;user=armands;database=pro_state;password=qwerty;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string inputEmail = EpastsEntry.Text;
            string inputPassword = ParoleEntry.Text;

            var emailRegex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");

            // Pārbauda vai vispār kaut kas ir ievadīts inputā
            if (string.IsNullOrEmpty(inputEmail) || string.IsNullOrEmpty(inputPassword))
            {
                inputError.IsVisible = true;
                emailError.IsVisible = false;
                Error.IsVisible = false;
                return;
            }
            else
            {
                inputError.IsVisible = false;
            }

            // Pārbauda vai epasts ir derīgs
            if (!emailRegex.IsMatch(inputEmail))
            {
                emailError.IsVisible = true;
                Error.IsVisible = false;
                return;
            }
            else
            {
                emailError.IsVisible = false;
            }
            using (var command = new MySqlCommand($"SELECT * FROM user WHERE email = \"{inputEmail}\"", connection))
            using (var reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Error.IsVisible = true;
                }
                while (reader.Read())
                {
                    string password = reader.GetString(4);
                    if (BCrypt.Net.BCrypt.Verify(inputPassword, password))
                    {
                        GlobalVariables.CurrentUser = new User { id = Convert.ToInt32(reader.GetString("id")), name = reader.GetString("name"), surname = reader.GetString("surname"), email = inputEmail, password = inputPassword, social_number = reader.GetString("social_number"), birth_date = Convert.ToDateTime(reader.GetString("birth_date")), gender= reader.GetString("gender") };
                        Application.Current.MainPage = new AppShell();
                    }
                    else
                    {
                      Error.IsVisible = true;
                    }
                }
            }
        }
    }
}

