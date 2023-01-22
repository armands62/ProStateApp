using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Transactions;
using static ProStateApp2.App;

namespace ProStateApp2;

public partial class History : ContentPage
{
    public History()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadTransactions();
    }

    private void LoadTransactions()
    {
        string connectionString = "server=192.168.1.198;user=armands;database=pro_state;password=qwerty;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            var command = new MySqlCommand(@"SELECT t.amount, t.description, t.date, a.number as 'From', b.number as 'To', a.name as 'FromName', b.name as 'ToName'
                                         FROM transaction_history t 
                                         JOIN account a ON t.account_from = a.id 
                                         JOIN account b ON t.account_to = b.id 
                                         WHERE a.user_id = @user_id OR b.user_id = @user_id", connection);
            command.Parameters.AddWithValue("@user_id", GlobalVariables.CurrentUser.id);

            using (var reader = command.ExecuteReader())
            {
                var transactions = new List<Transaction>();
                while (reader.Read())
                {
                    transactions.Add(new Transaction
                    {
                        Amount = reader.GetDecimal("amount"),
                        Description = reader.GetString("description"),
                        Date = reader.GetDateTime("date"),
                        AccountFrom = reader.GetString("From") + " " + reader.GetString("FromName"),
                        AccountTo = reader.GetString("To") + " " + reader.GetString("ToName")
                    });
                }
                TransactionsListView.ItemsSource = transactions;
            }
        }
    }
}