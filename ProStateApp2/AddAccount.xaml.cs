using MySql.Data.MySqlClient;
using static ProStateApp2.App;

namespace ProStateApp2;

public partial class AddAccount : ContentPage
{
	public AddAccount()
	{
		InitializeComponent();
	}

    private string GenerateAccountNumber()
    {
        var random = new Random();
        var firstTwo = random.Next(10, 99);
        var nextEight = random.Next(10000000, 999999999);
        var lastFive = random.Next(10000, 99999);
        var generatedAccountNumber = "LV" + firstTwo + "PRST" + nextEight + lastFive;
        return generatedAccountNumber;
    }

    private void Account_Created(object sender, EventArgs e)
    {

        var accountNumber = GenerateAccountNumber();
        var accountName = accountNameEntry.Text;
        decimal dailyLimit = 0;
        decimal monthlyLimit = 0;
        var userId = GlobalVariables.CurrentUser.id;

        if (string.IsNullOrEmpty(accountName))
        {
            DisplayAlert("Kļūda!", "Konta nosaukums nevar palikt tukšs!", "OK");
            return;
        }

        if (!Decimal.TryParse(dailyLimitEntry.Text, out dailyLimit) || !Decimal.TryParse(monthlyLimitEntry.Text, out monthlyLimit))
        {
            DisplayAlert("Kļūda!", "Ievadiet derīgu summu!", "OK");
            return;
        }

        if (dailyLimit <= 0 || monthlyLimit <= 0)
        {
            DisplayAlert("Kļūda!", "Dienas limitam un mēneša limitam jābūt lielākam par 0!", "OK");
            return;
        }

        string connectionString = "server=192.168.1.198;user=armands;database=pro_state;password=qwerty;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            var command = new MySqlCommand("INSERT INTO account (number, name, user_id, available, reserved, daily_limit, monthly_limit) VALUES (@accountNumber, @accountName, @userId, 0, 0, @dailyLimit, @monthlyLimit)", connection);
            command.Parameters.AddWithValue("@accountNumber", accountNumber);
            command.Parameters.AddWithValue("@accountName", accountName);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@dailyLimit", dailyLimit);
            command.Parameters.AddWithValue("@monthlyLimit", monthlyLimit);

            try
            {
                command.ExecuteNonQuery();
                DisplayAlert("Success", "Account added successfully", "OK");
                Navigation.PopAsync();
            }
            catch (MySqlException ex)
            {
                DisplayAlert("Error", ex.ToString(), "OK");
            }
        }
    }
}