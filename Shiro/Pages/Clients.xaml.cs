// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from Shiro INC. team.
//  
// Copyrights (c) 2014 Shiro INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using FirstFloor.ModernUI.Windows.Controls;

using Shiro.Class;

namespace Shiro.Pages
{
    /// <summary>
    ///   Interaction logic for Clients.xaml
    /// </summary>
    internal sealed partial class Clients
    {
        private static readonly List<Customer> ListClient = new List<Customer>();
        private static readonly List<Customer> SecondListClient = new List<Customer>();

        private void SelectMarchandiseLike(string client)
        {
            PanelClient.Children.Clear();
            ListClient.Clear();

            var nbClient = SecondListClient.Count;
            for(var i = 0; i < nbClient; i++)
            {
                var name = String.Format("{0} {1}", SecondListClient[i].Name, SecondListClient[i].Firstname);
                if(!SecondListClient[i].Mail.ToLower().Contains(client.ToLower())
                   && !name.ToLower().ToString(CultureInfo.InvariantCulture).Contains(client.ToLower())
                   && !SecondListClient[i].Telephone.ToLower().ToString(CultureInfo.InvariantCulture).Contains(client.ToLower())
                   && !SecondListClient[i].Company.ToLower().ToString(CultureInfo.InvariantCulture).Contains(client.ToLower()))
                {
                    continue;
                }
                var id = SecondListClient[i].IdCustomer;
                var newClient = new Customer(id, SecondListClient[i].Telephone, SecondListClient[i].Name, SecondListClient[i].Firstname,
                    SecondListClient[i].Mail, SecondListClient[i].Company);
                Display(newClient);
            }
            if(ListClient.Count != 0)
            {
                return;
            }
            var panelClient = new StackPanel();
            // New border
            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(2, 2, 1, 0),
                BorderThickness = new Thickness(1),
                Width = BorderClient.Width - 5,
                Child = panelClient,
                Height = 100
            };

            PanelClient.Children.Add(border);

            // Client's name
            panelClient.Children.Add(new TextBlock
            {
                Text = "Aucun résultat trouvé",
                Margin = new Thickness(5, 2, 0, 0),
                Height = 40,
                TextAlignment = TextAlignment.Center
            });
        }

        private void TextBoxClientSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            SelectMarchandiseLike(TextBoxClientSearch.Text == String.Empty ? String.Empty : TextBoxClientSearch.Text);
        }

        private void Display(Customer newClient)
        {
            var panelClient = new StackPanel();
            var thick = new Thickness(5, 2, 0, 0);

            // New border
            var border = new Border
            {
                BorderBrush = BorderClient.BorderBrush,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(2, 2, 1, 0),
                BorderThickness = new Thickness(1),
                Width = BorderClient.Width - 5,
                Child = panelClient,
                Height = 80
            };

            PanelClient.Children.Add(border);

            // Client's name
            panelClient.Children.Add(new TextBlock {Text = String.Format("{0} {1}", newClient.Name, newClient.Firstname), Margin = thick, Height = 16});

            // Mail
            panelClient.Children.Add(new TextBlock {Text = String.Format("MAIL : {0}", newClient.Mail), Margin = new Thickness(5, 2, 0, 0), Height = 16});

            // Company
            panelClient.Children.Add(new TextBlock
            {
                Text = String.Format("Entreprise : {0}", newClient.Company),
                Margin = new Thickness(5, 2, 0, 0),
                Height = 16
            });

            // Phone
            panelClient.Children.Add(new TextBlock {Text = String.Format("Numéro de téléphone : {0}", newClient.Telephone), Margin = thick, Height = 16});

            // Button
            var btnDelete = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "Supprimer le client",
                Margin = new Thickness(9, -30, 67, 50),
                BorderBrush = Brushes.Red,
                Tag = newClient.IdCustomer
            };

            panelClient.Children.Add(btnDelete);
            btnDelete.Click += BTN_Delete_Click;

            newClient.Border = border;
            ListClient.Add(newClient);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListClient.Clear();
            SecondListClient.Clear();
            try
            {
                DisplayAll();
            }
            catch
            {
                ModernDialog.ShowMessage("Erreur de connexion avec la base de données", "Erreur", MessageBoxButton.OK);
            }
        }

        private void DisplayAll()
        {
            PanelClient.Children.Clear();
            ListClient.Clear();
            SecondListClient.Clear();
            var command = Connection.GetAll("CUSTOMER");
            var resultat = command.ExecuteReader();
            while(resultat.Read())
            {
                var client = new Customer(Convert.ToInt32(resultat["ID_CUSTOMER"]), resultat["TELEPHONE"].ToString(), resultat["NAME"].ToString(),
                    resultat["FIRSTNAME"].ToString(), resultat["MAIL"].ToString(), resultat["COMPANY"].ToString());
                ListClient.Add(client);
                SecondListClient.Add(client);
                Display(client);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderClient.Width = MenuClient.ActualWidth - 40;
            BorderClient.Height = MenuClient.ActualHeight - 70;

            var nbClient = ListClient.Count;
            for(var i = 0; i < nbClient; i++)
            {
                ListClient[i].Border.Width = BorderClient.Width - 5;
            }
        }

        private void BTN_Delete_Click(object sender, EventArgs e)
        {
            var id = ((Button) sender).Tag.ToString();
            var query = String.Format("SELECT {0} FROM {1} WHERE ID_{1} = {2}", "NAME", "CUSTOMER", id);
            var name = Connection.GetFirst(query);
            if(ModernDialog.ShowMessage("Supprimer le client " + name + " ?", "Etes vous sur ?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            var where = new[,] {{"ID_CUSTOMER", id}};
            Connection.Delete("APPOINTMENT", where);
            Connection.Delete("CUSTOMER", where);
            DisplayAll();
        }
    }
}