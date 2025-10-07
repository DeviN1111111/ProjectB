using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

class Program
{
    static void Main()
    {
        LoginAccess.CreateTable();
        Console.Clear();
        StartScreen.Show();
        // LoginUI.Login();
    }
}
