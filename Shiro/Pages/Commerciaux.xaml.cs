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

using FirstFloor.ModernUI.Windows.Controls;

using Shiro.Class;
using Shiro.ComboBox;

namespace Shiro.Pages
{
    /// <summary>
    ///   Interaction logic for Commerciaux.xaml
    /// </summary>
    internal sealed partial class Commerciaux
    {
        private static readonly List<Class.Appointment> ListAppointment = new List<Class.Appointment>();

        private void ComboBoxSalesMan_Loaded(object sender, EventArgs e)
        {
            PanelSales.Children.Clear();
            ListAppointment.Clear();
            ComboBoxSalesMan.Items.Clear();
            ChangeVisibility(false);
            try
            {
                InitComboClient();
            }
            catch(Exception caught)
            {
                Console.WriteLine(caught.Message);
            }
        }

        private void ComboBoxSalesMan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ComboBoxSalesMan.Items.Count == 0)
            {
                return;
            }
            PanelSales.Children.Clear();
            try
            {
                TextMail.Text = ((ComboboxItemSalesMan) ComboBoxSalesMan.SelectedItem).Value.Mail;
                TextPhone.Text = ((ComboboxItemSalesMan) ComboBoxSalesMan.SelectedItem).Value.Telephone;

                var query = String.Format("SELECT DISTINCT {0} FROM {1} WHERE ID_{2} = {3} ORDER BY {0}", "ID_APPOINTMENT", "APPOINTMENT", "SALESMAN",
                    ((ComboboxItemSalesMan) ComboBoxSalesMan.SelectedItem).Value.IdSalesman);
                var command = Connection.Command(query);
                var resultCommand = command.ExecuteReader();
                while(resultCommand.Read())
                {
                    var query2 = String.Format("APPOINTMENT WHERE ID_APPOINTMENT = {0}", resultCommand["ID_APPOINTMENT"]);
                    var command2 = Connection.GetAll(query2);
                    var resultatAppointment = command2.ExecuteReader();
                    while(resultatAppointment.Read())
                    {
                        var appointment = new Class.Appointment(Convert.ToInt32(resultatAppointment["ID_APPOINTMENT"]),
                            Convert.ToInt32(resultatAppointment["ID_CUSTOMER"]), Convert.ToInt32(resultatAppointment["ID_SALESMAN"]),
                            resultatAppointment["DAY"].ToString(), resultatAppointment["STARTTIME"].ToString(), resultatAppointment["ENDTIME"].ToString());

                        var panelAppoint = new StackPanel();
                        var thick = new Thickness(5, 2, 0, 0);

                        // New border
                        var bordure = new Border
                        {
                            BorderBrush = ComboBoxSalesMan.BorderBrush,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(2, 2, 1, 0),
                            BorderThickness = new Thickness(1),
                            Width = BorderSales.Width - 5,
                            Child = panelAppoint,
                            Height = 70
                        };

                        var name = string.Empty;
                        var queryName = string.Format("SELECT {0}, {1} FROM {2} WHERE {3} = {4}", "NAME", "FIRSTNAME", "CUSTOMER", "ID_CUSTOMER",
                            resultatAppointment["ID_CUSTOMER"]);
                        var commandName = Connection.Command(queryName);
                        var resultatName = commandName.ExecuteReader();
                        while(resultatName.Read())
                        {
                            name = string.Format("{0} {1}", resultatName["NAME"], resultatName["FIRSTNAME"]);
                        }

                        // CUSTOMER's name
                        panelAppoint.Children.Add(new TextBlock {Margin = thick, Text = name, Height = 16});

                        // Day
                        panelAppoint.Children.Add(new TextBlock
                        {
                            Text = "Date : " + resultatAppointment["DAY"].ToString().Split(new[] {' '})[0],
                            Margin = thick,
                            Height = 16
                        });
                        // Time
                        panelAppoint.Children.Add(new TextBlock
                        {
                            Text = string.Format("De {0} à {1}", resultatAppointment["STARTTIME"], resultatAppointment["ENDTIME"]),
                            Margin = thick,
                            Height = 16
                        });

                        appointment.Border = bordure;
                        PanelSales.Children.Add(bordure);
                        ListAppointment.Add(appointment);
                    }
                    resultatAppointment.Close();
                }
                resultCommand.Close();
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
                ModernDialog.ShowMessage("Connection à la base de données impossible", "Erreur", MessageBoxButton.OK);
            }
            finally
            {
                ChangeVisibility(true);
                if(ListAppointment.Count == 0)
                {
                    PanelSales.Children.Clear();
                    var panelSalesMan = new StackPanel();
                    // New border
                    var border = new Border
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(2, 2, 1, 0),
                        BorderThickness = new Thickness(1),
                        Width = BorderSales.Width - 5,
                        Child = panelSalesMan,
                        Height = 100
                    };

                    PanelSales.Children.Add(border);

                    // SalesMan's name
                    panelSalesMan.Children.Add(new TextBlock
                    {
                        Text = "Aucun résultat trouvé",
                        Margin = new Thickness(5, 2, 0, 0),
                        Height = 40,
                        TextAlignment = TextAlignment.Center
                    });
                }
            }
        }

        private void InitComboClient()
        {
            try
            {
                var oCommand = Connection.GetAll("SALESMAN");
                var resultat = oCommand.ExecuteReader();
                while(resultat.Read())
                {
                    var text = string.Format("{0} {1}", resultat["FIRSTNAME"], resultat["NAME"]);
                    ComboBoxSalesMan.Items.Add(new ComboboxItemSalesMan
                    {
                        Text = text,
                        Value =
                            new SalesMan(Convert.ToInt32(resultat["ID_SALESMAN"]), resultat["TELEPHONE"].ToString(), resultat["NAME"].ToString(),
                                resultat["FIRSTNAME"].ToString(), resultat["MAIL"].ToString())
                    });
                }
                resultat.Close();
            }
            catch
            {
                ModernDialog.ShowMessage("Connection à la base de donnéess impossible", "Erreur", MessageBoxButton.OK);
            }
        }

        private void MenuSalesMan_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderSales.Width = MenuSalesMan.ActualWidth - 400;
            BorderSales.Height = MenuSalesMan.ActualHeight - 100;
            try
            {
                var nbMarchandise = ListAppointment.Count;
                for(var i = 0; i < nbMarchandise; i++)
                {
                    ListAppointment[i].Border.Width = BorderSales.Width - 5;
                }
            }
            catch(Exception caught)
            {
                Console.WriteLine(caught.Message);
                Console.Read();
            }
        }

        private void MenuSalesMan_Loaded(object sender, RoutedEventArgs e)
        {
            var nbSalesMan = ListAppointment.Count;

            if(nbSalesMan == 0)
            {
                return;
            }
            for(var i = 0; i < nbSalesMan; i++)
            {
                ListAppointment[i].Border.BorderBrush = BorderSales.BorderBrush;
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var where = new[,]
                {{"ID_SALESMAN", ((ComboboxItemSalesMan) ComboBoxSalesMan.SelectedItem).Value.IdSalesman.ToString(CultureInfo.InvariantCulture)}};
                Connection.Delete("APPOINTMENT", where);
                Connection.Delete("SALESMAN", where);
            }
            catch
            {
                ModernDialog.ShowMessage("Connection à la base de données impossible", "Erreur", MessageBoxButton.OK);
            }
            ComboBoxSalesMan.Items.Clear();
            InitComboClient();
            ChangeVisibility(false);
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            var salesMan = ((ComboboxItemSalesMan) ComboBoxSalesMan.SelectedItem).Value;
            try
            {
                var set = new[,] {{"TELEPHONE", TextPhone.Text}, {"MAIL", TextMail.Text}};
                Connection.Update("SALESMAN", salesMan.IdSalesman, set);
            }
            catch
            {
                ModernDialog.ShowMessage("Connection à la base de données impossible", "Erreur", MessageBoxButton.OK);
            }
            finally
            {
                salesMan.Telephone = TextPhone.Text;
                salesMan.Mail = TextMail.Text;
                ModernDialog.ShowMessage("Informations du commercial " + salesMan.Name + salesMan.Firstname + " modifiée avec succès", "Opération réussie",
                    MessageBoxButton.OK);
            }
        }

        private void ChangeVisibility(bool visibility)
        {
            if(visibility)
            {
                BorderSales.Visibility = Visibility.Visible;
                LabelPhone.Visibility = Visibility.Visible;
                LabelMail.Visibility = Visibility.Visible;

                TextPhone.Visibility = Visibility.Visible;
                TextMail.Visibility = Visibility.Visible;

                BtnDelete.Visibility = Visibility.Visible;
                BtnUpdate.Visibility = Visibility.Visible;
            }
            else
            {
                BorderSales.Visibility = Visibility.Hidden;
                LabelPhone.Visibility = Visibility.Hidden;
                LabelMail.Visibility = Visibility.Hidden;

                TextPhone.Visibility = Visibility.Hidden;
                TextMail.Visibility = Visibility.Hidden;

                BtnDelete.Visibility = Visibility.Hidden;
                BtnUpdate.Visibility = Visibility.Hidden;
            }
        }
    }
}