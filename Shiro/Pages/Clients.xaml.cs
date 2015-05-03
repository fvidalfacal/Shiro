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
    /// Interaction logic for Clients.xaml
    /// </summary>
    public partial class Clients
    {
        private static readonly List<CUSTOMER> ListClient = new List<CUSTOMER>();
        private static readonly List<CUSTOMER> SecondListClient = new List<CUSTOMER>();

        private void SelectMarchandiseLike(string Client)
        {
            PanelClient.Children.Clear();
            ListClient.Clear();

            var nbClient = SecondListClient.Count;
            for(var i = 0; i < nbClient; i++)
            {
                var name = String.Format("{0} {1}", SecondListClient[i].NAME, SecondListClient[i].FIRSTNAME);
                if(!SecondListClient[i].MAIL.ToLower().Contains(Client.ToLower())
                   && !name.ToLower().ToString(CultureInfo.InvariantCulture).Contains(Client.ToLower())
                   && !SecondListClient[i].TELEPHONE.ToLower().ToString(CultureInfo.InvariantCulture).Contains(Client.ToLower())
                   && !SecondListClient[i].COMPANY.ToLower().ToString(CultureInfo.InvariantCulture).Contains(Client.ToLower()))
                {
                    continue;
                }
                var ID = SecondListClient[i].ID_CUSTOMER;
                var newClient = new CUSTOMER(ID, SecondListClient[i].TELEPHONE, SecondListClient[i].NAME, SecondListClient[i].FIRSTNAME,
                    SecondListClient[i].MAIL, SecondListClient[i].COMPANY);
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

        private void Display(CUSTOMER newClient)
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
            panelClient.Children.Add(new TextBlock
            {
                Text = String.Format("{0} {1}", newClient.NAME, newClient.FIRSTNAME),
                Margin = thick,
                Height = 16
            });

            // Mail
            panelClient.Children.Add(new TextBlock
            {
                Text = String.Format("MAIL : {0}", newClient.MAIL),
                Margin = new Thickness(5, 2, 0, 0),
                Height = 16
            });

            // Company
            panelClient.Children.Add(new TextBlock
            {
                Text = String.Format("Entreprise : {0}",newClient.COMPANY), 
                Margin = new Thickness(5, 2, 0, 0),
                Height = 16
            });

            // Phone
            panelClient.Children.Add(new TextBlock
            {
                Text = String.Format("Numéro de téléphone : {0}", newClient.TELEPHONE),
                Margin = thick, 
                Height = 16
            });

            // Button
            var BTN_Delete = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "Supprimer le client",
                Margin = new Thickness(9, -30, 67, 50),
                BorderBrush = Brushes.Red,
                Tag = newClient.ID_CUSTOMER
            };

            panelClient.Children.Add(BTN_Delete);
            BTN_Delete.Click += BTN_Delete_Click;

            newClient.Border = border;
            ListClient.Add(newClient);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PanelClient.Children.Clear();
            ListClient.Clear();
            SecondListClient.Clear();
            try
            {
                //TODO : SQL SERVER
                var oCommand = ConnectionOracle.GetAll("CUSTOMER");
                var resultat = oCommand.ExecuteReader();
                while(resultat.Read())
                {
                    var newClient = new CUSTOMER(Convert.ToInt32(resultat["ID_CUSTOMER"]), resultat["TELEPHONE"].ToString(),
                        resultat["NAME"].ToString(), resultat["FIRSTNAME"].ToString(),
                        resultat["MAIL"].ToString(), resultat["COMPANY"].ToString());
                    Display(newClient);
                    SecondListClient.Add(newClient);
                }
                resultat.Close();
            }
            catch
            {
                ModernDialog.ShowMessage("Erreur de connexion avec la base de donnée", "Erreur", MessageBoxButton.OK);
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
            var ID = ((Button)sender).Tag.ToString();
            var query = String.Format("SELECT {0} FROM {1} WHERE ID_{1} = {2}", "NAME", "CUSTOMER", ID);
            var name = ConnectionOracle.Command(query);
            if (ModernDialog.ShowMessage("Supprimer le client " + name, "Etes vous sur ?", MessageBoxButton.YesNo)
               != MessageBoxResult.Yes)
            {
                return;
            }
            ConnectionOracle.Delete("APPOINTMENT", ID, "CUSTOMER");
            ConnectionOracle.Delete("CUSTOMER", ID);
            SelectMarchandiseLike(String.Empty);
        }
        
    }
}
