﻿namespace ProStateApp2;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(Profile), typeof(Profile));
        Routing.RegisterRoute(nameof(EditProfile), typeof(EditProfile));
    }
}
