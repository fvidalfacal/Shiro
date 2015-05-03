
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using FirstFloor.ModernUI.Windows.Controls;

using Shiro.Class;

namespace Shiro.Pages
{
    /// <summary>
    /// Interaction logic for AddClient.xaml
    /// </summary>
    public partial class AddClient
    {
        private static readonly List<CUSTOMER> ListCustomer = new List<CUSTOMER>();

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderCustomer.Width = CustomerCreator.ActualWidth - 340;
            BorderCustomer.Height = CustomerCreator.ActualHeight - 70;

            var nbCustomer = ListCustomer.Count;
            for (var i = 0; i < nbCustomer; i++)
            {
                ListCustomer[i].Border.Width = BorderCustomer.Width - 6;
            }
        }

        private void CustomerCreator_Loaded(object sender, RoutedEventArgs e)
        {
            //
            DisplayAll();
        }

        private void DisplayAll()
        {
            PanelCustomer.Children.Clear();
            var command = ConnectionOracle.GetAll("CUSTOMER");
            var resultat = command.ExecuteReader();
            while (resultat.Read())
            {
                showCustomer(Convert.ToInt32(resultat["ID_CUSTOMER"]), resultat["MAIL"].ToString(), resultat["NAME"].ToString(), resultat["FIRSTNAME"].ToString(), resultat["COMPANY"].ToString(),
                    resultat["TELEPHONE"].ToString());
            }
        }

        private void TextBoxMail_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged();
        }

        private void TextBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged();
        }

        private void TextBoxPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged();
        }

        private static Boolean isInt(string str)
        {
            int value;
            return (str.Trim() != string.Empty) && int.TryParse(str, out value);
        }

        private static Boolean validMail(string mailString)
        {
            try
            {
                new MailAddress(mailString);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void TextChanged()
        {
            BtnAdd.IsEnabled = TextBoxMail.Text != String.Empty && validMail(TextBoxMail.Text) && TextBoxName.Text != String.Empty && isInt(TextBoxPhone.Text);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var queryVerify = String.Format("SELECT * FROM {0} WHERE {1}='{2}' OR {3}='{4}'", "CUSTOMER", "TELEPHONE",
                    TextBoxPhone.Text, "MAIL", TextBoxMail.Text);
                if (ConnectionOracle.sizeOf(queryVerify) == 0)
                {
                    var querySelect = String.Format("SELECT max(ID_{0}) FROM {0}", "CUSTOMER");
                    var Command = ConnectionOracle.GetFirst(querySelect);
                    var idCustomer = Command.ToString() == String.Empty ? 1 : Convert.ToInt32(Command) + 1;
                    ConnectionOracle.Insert("CUSTOMER", idCustomer, TextBoxFirstName.Text, TextBoxName.Text, TextBoxCompany.Text,  TextBoxPhone.Text,TextBoxMail.Text);
                    ModernDialog.ShowMessage("Utilisateur " + TextBoxName.Text + TextBoxFirstName.Text +" ajouté avec succès", "Succès", MessageBoxButton.OK);
                    showCustomer(idCustomer, TextBoxMail.Text, TextBoxName.Text, TextBoxFirstName.Text, TextBoxCompany.Text, TextBoxPhone.Text);
                    TextBoxMail.Text = TextBoxPhone.Text = TextBoxName.Text = String.Empty;
                }
                else
                {
                    ModernDialog.ShowMessage("Ce client existe deja", "Erreur", MessageBoxButton.OK);
                }
            }
            catch
            {
                ModernDialog.ShowMessage("Erreur de connection à la base de donnée", "Erreur", MessageBoxButton.OK);
            }
        }

        private void showCustomer(int ID, string Mail, string Name, string FirstName, string company, string Phone)
        {
            var panelCustomer = new StackPanel();
            var thick = new Thickness(5, 2, 0, 0);

            // New border
            var border = new Border
            {
                BorderBrush = BtnAdd.BorderBrush,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(2, 2, 1, 0),
                BorderThickness = new Thickness(1),
                Width = BorderCustomer.Width - 5,
                Child = panelCustomer,
                Height = 70
            };

            // Customer's name
            panelCustomer.Children.Add(new TextBlock { Margin = thick, Text = Name, Height = 16 });

            // Mail
            panelCustomer.Children.Add(new TextBlock { Text = Mail, Margin = thick, Height = 16 });

            // Phone
            panelCustomer.Children.Add(new TextBlock { Text = Phone, Margin = thick, Height = 16 });

            // Button
            var BTN_Delete = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "Supprimer le client",
                Margin = new Thickness(9, -30, 67, 50),
                BorderBrush = Brushes.Red,
                Tag = ID
            };

            panelCustomer.Children.Add(BTN_Delete);
            BTN_Delete.Click += BTN_Delete_Click;
            var newCustomer = new CUSTOMER(ID, Phone, Name, FirstName, Mail, company);
            PanelCustomer.Children.Add(border);
            newCustomer.Border = border;
            ListCustomer.Add(newCustomer);
        }

        private void BTN_Delete_Click(object sender, EventArgs e)
        {
            var ID = ((Button)sender).Tag.ToString();
            var query = String.Format("SELECT {0} FROM {1} WHERE ID_{1} = {2}", "NAME", "CUSTOMER", ID);
            var name = ConnectionOracle.Command(query);
            if (ModernDialog.ShowMessage("Supprimer le client "+ name, "Etes vous sur ?", MessageBoxButton.YesNo)
               != MessageBoxResult.Yes)
            {
                return;
            }
            ConnectionOracle.Delete("APPOINTMENT", ID, "CUSTOMER");
            ConnectionOracle.Delete("CUSTOMER", ID);
            DisplayAll();
        }
    }
}
