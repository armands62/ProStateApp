using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using MySql.Data.MySqlClient;
using System.Data;
using static ProStateApp2.App;

namespace ProStateApp2;

public partial class MoneyTransfer : ContentPage
{
    public MoneyTransfer()
    {
        InitializeComponent();
    }

    private void LoadAccounts()
    {
        string connectionString = "server=192.168.1.198;user=armands;database=pro_state;password=qwerty;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            var command = new MySqlCommand("SELECT * FROM account WHERE user_id = @user_id", connection);
            command.Parameters.AddWithValue("@user_id", GlobalVariables.CurrentUser.id);

            using (var reader = command.ExecuteReader())
            {
                var accounts = new List<Account>();
                while (reader.Read())
                {
                    accounts.Add(new Account
                    {
                        Id = reader.GetInt32("id"),
                        AccountNumber = reader.GetString("number"),
                        Name = reader.GetString("name"),
                    });
                }
                AccountsPicker.ItemsSource = accounts;
            }
        }
    }

    private void AccountsPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (AccountsPicker.SelectedItem != null)
        {
            var selectedAccount = (Account)AccountsPicker.SelectedItem;
            var accountId = selectedAccount.Id;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadAccounts();
    }

    private void Make_Transfer(object sender, EventArgs e)
    {
        var selectedAccount = (Account)AccountsPicker.SelectedItem;
        if (selectedAccount == null || selectedAccount.Id == null)
        {
            DisplayAlert("Kļūda!", "Lūdzu, izvēlaties kontu pirms turpiniet!", "OK");
            return;
        }
        var accountId = selectedAccount.Id;
        var accountNumber = selectedAccount.AccountNumber;
        var receiverAccountNumber = receiverAccount.Text;
        int receiverAccountId = 0;
        if (enteredAmount.Text == null)
        {
            DisplayAlert("Kļūda!", "Lūdzu, ievadiet summu, kādu vēlaties pārskaitīt!", "OK");
            return;
        }
        decimal amount;
        if (!decimal.TryParse(enteredAmount.Text, out amount))
        {
            DisplayAlert("Ķļūda", "Lūdzu, ievadiet derīgu summu", "OK");
            return;
        }
        if (description.Text == null)
        {
            DisplayAlert("Kļūda!", "Lūdzu, ievadiet maksājuma detaļas!", "OK");
            return;
        }

        string connectionString = "server=192.168.1.198;user=armands;database=pro_state;password=qwerty;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            var command = new MySqlCommand("SELECT available FROM account WHERE id = @accountId", connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var availableBalance = reader.GetDecimal("available");
                    if (availableBalance < amount)
                    {
                        DisplayAlert("Kļūda!", "Nepietiek naudas līdzekļi!", "OK");
                        return;
                    }
                }
                reader.Close();
            }
            if (amount <= 0)
            {
                DisplayAlert("Kļūda!", "Ievadītajai summai jābūt lielākai par 0!", "OK");
                return;
            }

            // Begin a transaction
            var transaction = connection.BeginTransaction();

            try
            {
                command = new MySqlCommand("SELECT id FROM account WHERE number = @accountNumber", connection);
                command.Parameters.AddWithValue("@accountNumber", receiverAccountNumber);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        receiverAccountId = reader.GetInt32("id");
                    }

                    else
                    {
                        DisplayAlert("Kļūda!", "Šāds konta numurs nepastāv!", "OK");
                        return;
                    }
                    reader.Dispose();

                    // Deduct the amount from the selected account
                    command = new MySqlCommand("UPDATE account SET available = available - @amount WHERE id = @accountId", connection);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@accountId", accountId);
                    command.ExecuteNonQuery();

                    // Add the amount to the recipient account
                    command = new MySqlCommand("UPDATE account SET available = available + @amount WHERE id = @receiverAccountId", connection);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@receiverAccountId", receiverAccountId);
                    command.ExecuteNonQuery();

                    // Log the transaction
                    command = new MySqlCommand("INSERT INTO transaction_history (account_from, account_to, amount, description, date) VALUES (@accountId, @receiverAccountId, @amount, @description, @dateTime)", connection);
                    command.Parameters.AddWithValue("@accountId", accountId);
                    command.Parameters.AddWithValue("@receiverAccountId", receiverAccountId);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@description", description.Text);
                    command.Parameters.AddWithValue("@dateTime", DateTime.Now);
                    command.ExecuteNonQuery();

                    // Commit the transaction
                    transaction.Commit();

                    DisplayAlert("Veiksmīgs maksājums!", "Maksājums ir pabeigts!", "OK");
                    Application.Current.MainPage = new AppShell();
                }
            }
            catch (Exception ex)
            {
                // An error occurred, so the transaction is rolled back
                transaction.Rollback();
                DisplayAlert("Error", "An error occurred while making the transfer: " + ex.Message, "OK");
            }
        }
    }
}