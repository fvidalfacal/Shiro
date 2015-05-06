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
using Shiro.ComboBox;

namespace Shiro.Pages
{
    /// <summary>
    ///   Interaction logic for rdv.xaml
    /// </summary>
    internal sealed partial class Appointment
    {
        private static readonly List<Customer> ListCustomer = new List<Customer>();

        private void Appointment_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderCustomer.Width = AppointmentPage.ActualWidth - 340;
            BorderCustomer.Height = AppointmentPage.ActualHeight - 70;

            var nbCustomer = ListCustomer.Count;
            for(var i = 0; i < nbCustomer; i++)
            {
                ListCustomer[i].Border.Width = BorderCustomer.Width - 6;
            }
        }

        private void AppointmentPage_Initialized(object sender, EventArgs e)
        {
            TimePicker.AddTime(24, TimePickerDebutHeure);
            TimePicker.AddTime(24, TimePickerFinHeure);
            TimePicker.AddTime(60, TimePickerDebutMin);
            TimePicker.AddTime(60, TimePickerFinMin);
        }

        private void Appointment_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxClient.Items.Clear();
            ComboboxSalesMan.Items.Clear();

            try
            {
                var commandSalesman = Connection.GetAll("SALESMAN");
                var resultatSalesman = commandSalesman.ExecuteReader();
                while (resultatSalesman.Read())
                {
                    var text = string.Format("{0} {1}", resultatSalesman["FIRSTNAME"], resultatSalesman["NAME"]);
                    ComboboxSalesMan.Items.Add(new ComboboxItemSalesMan
                    {
                        Text = text,
                        Value =
                            new SalesMan(Convert.ToInt32(resultatSalesman["ID_SALESMAN"]), resultatSalesman["TELEPHONE"].ToString(), resultatSalesman["NAME"].ToString(),
                                resultatSalesman["FIRSTNAME"].ToString(), resultatSalesman["MAIL"].ToString())
                    });
                }
                resultatSalesman.Close();
                var commandClient = Connection.GetAll("CUSTOMER");
                var resultatClient = commandClient.ExecuteReader();
                while (resultatClient.Read())
                {
                    var text = string.Format("{0} {1}", resultatClient["FIRSTNAME"], resultatClient["NAME"]);
                    ComboBoxClient.Items.Add(new ComboboxItemCustomer
                    {
                        Text = text,
                        Value =
                            new Customer(Convert.ToInt32(resultatClient["ID_CUSTOMER"]), resultatClient["TELEPHONE"].ToString(), resultatClient["NAME"].ToString(),
                                resultatClient["FIRSTNAME"].ToString(), resultatClient["MAIL"].ToString(), resultatClient["COMPANY"].ToString())
                    });
                }
                resultatClient.Close();
            }
            catch
            {
                ModernDialog.ShowMessage("Connection à la base de donnéess impossible", "Erreur", MessageBoxButton.OK);
            }

            DatePickerAppoint.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);

            DisplayAll();
        }

        private void DisplayAll()
        {
            PanelCustomer.Children.Clear();
            var command = Connection.GetAll("CUSTOMER");
            var resultat = command.ExecuteReader();
            while(resultat.Read())
            {
                ShowCustomer(Convert.ToInt32(resultat["ID_CUSTOMER"]), resultat["MAIL"].ToString(), resultat["NAME"].ToString(),
                    resultat["FIRSTNAME"].ToString(), resultat["COMPANY"].ToString(), resultat["TELEPHONE"].ToString());
            }
        }

        private void Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextChanged();
        }

        private void DatePickerDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var result = DateTime.Compare(Convert.ToDateTime(DatePickerAppoint.Text), DateTime.Now);
            if(result >= 0)
            {
                return;
            }
            DatePickerAppoint.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        }


        private void TextChanged()
        {
            BtnAdd.IsEnabled = TimePickerFinMin.Text != string.Empty && TimePickerFinHeure.Text != string.Empty && TimePickerDebutHeure.Text != string.Empty
                            && TimePickerDebutMin.Text != string.Empty && ComboBoxClient.Text != string.Empty && ComboboxSalesMan.Text != string.Empty;
            //MessageBox.Show(TimePickerFinMin.Text +" ,"+ TimePickerFinHeure.Text +" ,"+ TimePickerDebutHeure.Text 
            //    +" ,"+  TimePickerDebutMin.Text +" ,"+  ComboBoxClient.Text+" ,"+ ComboboxSalesMan.Text );
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                /*var queryVerify = String.Format("SELECT * FROM {0} WHERE {1} = '{2}' OR {3} = '{4}'", "CUSTOMER", "TELEPHONE", TextBoxPhone.Text, "MAIL",
                    TextBoxMail.Text);
                if(Connection.SizeOf(queryVerify) == 0)
                {
                    var querySelect = String.Format("SELECT max(ID_{0}) FROM {0}", "CUSTOMER");
                    var command = Connection.GetFirst(querySelect);
                    var idCustomer = command.ToString() == String.Empty ? 1 : Convert.ToInt32(command) + 1;
                    //Connection.Insert("CUSTOMER", idCustomer, TextBoxFirstName.Text, DatePickerAppoint.Text, TextBoxCompany.Text, TextBoxPhone.Text, TextBoxMail.Text);
                    //ModernDialog.ShowMessage("Utilisateur " + DatePickerAppoint.Text + TextBoxFirstName.Text + " ajouté avec succès", "Succès", MessageBoxButton.OK);
                    //ShowCustomer(idCustomer, TextBoxMail.Text, DatePickerAppoint.Text, TextBoxFirstName.Text, TextBoxCompany.Text, TextBoxPhone.Text);
                    //TextBoxMail.Text = TextBoxPhone.Text = DatePickerAppoint.Text = TextBoxCompany.Text = TextBoxFirstName.Text = String.Empty;
                }
                else
                {
                    ModernDialog.ShowMessage("Ce client existe deja", "Erreur", MessageBoxButton.OK);
                }*/
            }
            catch
            {
                ModernDialog.ShowMessage("Erreur de connection à la base de données", "Erreur", MessageBoxButton.OK);
            }
        }

        private void ShowCustomer(int id, string mail, string name, string firstName, string company, string phone)
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

            name = string.Format("{0} {1}", name, firstName);

            // Customer's name
            panelCustomer.Children.Add(new TextBlock {Margin = thick, Text = name, Height = 16});

            // Mail
            panelCustomer.Children.Add(new TextBlock {Text = mail, Margin = thick, Height = 16});

            // Phone
            panelCustomer.Children.Add(new TextBlock {Text = phone, Margin = thick, Height = 16});

            // Button
            var btnDelete = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "Supprimer le client",
                Margin = new Thickness(9, -30, 67, 50),
                BorderBrush = Brushes.Red,
                Tag = id
            };

            panelCustomer.Children.Add(btnDelete);
            btnDelete.Click += BTN_Delete_Click;
            var newCustomer = new Customer(id, phone, name, firstName, mail, company);
            PanelCustomer.Children.Add(border);
            newCustomer.Border = border;
            ListCustomer.Add(newCustomer);
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
            string[,] where = {{"ID_CUSTOMER", id}};
            Connection.Delete("APPOINTMENT", where);
            Connection.Delete("CUSTOMER", where);
            DisplayAll();
        }

        
    }
}