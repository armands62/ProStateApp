namespace ProStateApp2;

using Microsoft.Maui.Graphics.Text;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Windows.Input;
using static ProStateApp2.App;

public partial class Accounts : ContentPage
{
	public Accounts()
	{
		InitializeComponent();

    }

    private List<Account> GetAccounts()
    {
        string connectionString = "server=192.168.1.198;user=armands;database=pro_state;password=qwerty;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Retrieve the account information from the database
            var accounts = new List<Account>();
            var command = new MySqlCommand("SELECT * FROM account WHERE user_id = @user_id", connection);
            command.Parameters.AddWithValue("@user_id", GlobalVariables.CurrentUser.id);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var account = new Account
                    {
                        AccountNumber = reader.GetString("number"),
                        Name = reader.GetString("name"),
                        AvailableMoney = reader.GetDouble("available"),
                        ReservedMoney = reader.GetDouble("reserved"),
                    };
                    accounts.Add(account);
                }
            }

            return accounts;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        AccountsGrid.Children.Clear();
        AccountsGrid.RowDefinitions.Clear();
        AccountsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });

        var accounts = GetAccounts();

        int row = 0;
        for (int i = 0; i < accounts.Count; i++)
        {
            var account = accounts[i];

            var accountNumber = new Label
            {
                Text = account.AccountNumber + " " + account.Name,
                FontAttributes = FontAttributes.Bold,
                FontSize = 18,
                Margin = new Thickness(left: 0, top: 3, right: 0, bottom: 0),
                TextColor = Color.FromArgb("#535759"),
            };
            Grid.SetRow(accountNumber, row);
            Grid.SetColumn(accountNumber, 0);
            AccountsGrid.Children.Add(accountNumber);


            var money = new Label
            {
                Text = (account.AvailableMoney + account.ReservedMoney).ToString(),
                Margin = new Thickness(left: 0, top: 0, right: 20, bottom: 0),
                HorizontalTextAlignment = TextAlignment.End,
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                TextColor = Colors.Black
            };
            Grid.SetRow(money, row);
            Grid.SetColumn(money, 1);
            AccountsGrid.Children.Add(money);

            var currency = new Label
            {
                Text = "FISC",
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                TextColor = Colors.Black
            };
            Grid.SetRow(currency, row);
            Grid.SetColumn(currency, 2);
            AccountsGrid.Children.Add(currency);

            if (i != accounts.Count - 1)
            {
                AccountsGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
            }
            row++;
            
        }

        totalMoneyGrid.Children.Clear();
        totalMoneyGrid.RowDefinitions.Clear();

        var totalMoneyLabel = new Label
        {
            Text = "Pieejamais atlikums: ",
            FontAttributes = FontAttributes.Bold,
            FontSize = 20,
            TextColor = Colors.Black
        };
        Grid.SetRow(totalMoneyLabel, 0);
        Grid.SetColumn(totalMoneyLabel, 0);
        totalMoneyGrid.Children.Add(totalMoneyLabel);

        var totalMoneyValue = new Label
        {
            Text = accounts.Sum(a => a.AvailableMoney + a.ReservedMoney).ToString(),
            FontAttributes = FontAttributes.Bold,
            FontSize = 20,
            TextColor = Colors.Black,
            Margin = new Thickness(left: 0, top: 0, right: 20, bottom: 0),
            HorizontalTextAlignment = TextAlignment.End
        };
        Grid.SetRow(totalMoneyValue, 0);
        Grid.SetColumn(totalMoneyValue, 1);
        totalMoneyGrid.Children.Add(totalMoneyValue);

        var totalMoneyCurrency = new Label
        {
            Text = "FISC",
            FontAttributes = FontAttributes.Bold,
            FontSize = 20,
            TextColor = Colors.Black,
        };
        Grid.SetRow(totalMoneyCurrency, 0);
        Grid.SetColumn(totalMoneyCurrency, 2);
        totalMoneyGrid.Children.Add(totalMoneyCurrency);
    }

    private async void Add_Account(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("AddAccount");
    }
}