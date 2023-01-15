using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using static ProStateApp2.App;

namespace ProStateApp2;

public partial class EditProfile : ContentPage
{
    public EditProfile()
    {
        InitializeComponent();

        NewName.Text = GlobalVariables.CurrentUser.name;

        NewSurname.Text = GlobalVariables.CurrentUser.surname;

        NewEmail.Text = GlobalVariables.CurrentUser.email;

        string Gender = GlobalVariables.CurrentUser.gender;


        if (Gender == "male")
        {
            GenderIcon.Source = "male_user.png";
        }
        else if (Gender == "female")
        {
            GenderIcon.Source = "female_user.png";
        }
        else
        {
            GenderIcon.Source = "question_mark.png";
        }

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
                    GlobalVariables.CurrentUser.gender = reader.GetString("gender");

                    NewName.Text = GlobalVariables.CurrentUser.name;

                    NewSurname.Text = GlobalVariables.CurrentUser.surname;

                    NewEmail.Text = GlobalVariables.CurrentUser.email;

                    string Gender = GlobalVariables.CurrentUser.gender;


                    if (Gender == "male")
                    {
                        GenderIcon.Source = "male_user.png";
                    }
                    else if (Gender == "female")
                    {
                        GenderIcon.Source = "female_user.png";
                    }
                    else
                    {
                        GenderIcon.Source = "question_mark.png";
                    }
                }
            }
        }
    }

     private  void Edit_Confirm(object sender, EventArgs e)
    {
        var emailRegex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
        int Id = GlobalVariables.CurrentUser.id;

        if (!emailRegex.IsMatch(NewEmail.Text))
        {
            emailError.IsVisible = true;
            return;
        }
        else
        {
            emailError.IsVisible = false;
        }

        string connectionString = "server=192.168.1.198;user=armands;database=pro_state;password=qwerty;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            using (MySqlCommand command = new MySqlCommand($"UPDATE user SET name = @value1, surname = @value2, email = @value3 WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@value1", NewName.Text);
                command.Parameters.AddWithValue("@value2", NewSurname.Text);
                command.Parameters.AddWithValue("@value3", NewEmail.Text);
                command.Parameters.AddWithValue("@id", Id);
                int rowsAffected = command.ExecuteNonQuery();

                Console.WriteLine("{0} rows affected.", rowsAffected);
            }

            connection.Close();
        }
        GlobalVariables.CurrentUser.name = NewName.Text;
        GlobalVariables.CurrentUser.surname = NewSurname.Text;
        GlobalVariables.CurrentUser.email = NewEmail.Text;
        Application.Current.MainPage = new AppShell();
    }
}