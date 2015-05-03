using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;

using FirstFloor.ModernUI.Windows.Controls;

using Shiro.Class;
using Shiro.ComboBox;

namespace Shiro.Pages
{
    /// <summary>
    /// Interaction logic for Commerciaux.xaml
    /// </summary>
    public partial class Commerciaux
    {
        private static readonly List<Appointment> ListAppointment = new List<Appointment>();

        private void ComboBoxSalesMan_Loaded(object sender, EventArgs e)
        {
            //Default Visibility 

            PanelSales.Children.Clear();
            ChangeVisibility(false);
            try
            {
                ComboBoxSalesMan.Items.Clear();
                InitComboClient();
            }
            catch (Exception caught)
            {
                Console.WriteLine(caught.Message);

            }
        }

        private void ComboBoxSalesMan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PanelSales.Children.Clear();
            try
            {
                TextMail.Text = ((ComboboxItemSalesMan)ComboBoxSalesMan.SelectedItem).Value.MAIL;
                TextPhone.Text = ((ComboboxItemSalesMan)ComboBoxSalesMan.SelectedItem).Value.TELEPHONE;

                var query = String.Format("SELECT DISTINCT {0} FROM {1} WHERE ID_{2} = {3} ORDER BY {0}", "ID_APPOINTMENT", "APPOINTMENT",
                    "SALESMAN", ((ComboboxItemSalesMan)ComboBoxSalesMan.SelectedItem).Value.ID_SALESMAN);

                var Command = ConnectionOracle.Command(query);
                var resultCommand = Command.ExecuteReader();
                while (resultCommand.Read())
                {
                    var query2 = String.Format("APPOINTMENT WHERE ID_APPOINTMENT = {0}", resultCommand["ID_APPOINTMENT"]);
                    var Command2 = ConnectionOracle.GetAll(query2);
                    var resultatAppointment = Command2.ExecuteReader();
                    while (resultatAppointment.Read())
                    {
                        var Appointment = new Appointment(Convert.ToInt32(resultatAppointment["ID_APPOINTMENT"]), Convert.ToInt32(resultatAppointment["ID_CUSTOMER"]),
                            Convert.ToInt32(resultatAppointment["ID_SALESMAN"]), resultatAppointment["DAY"].ToString(),
                            resultatAppointment["STARTTIME"].ToString(), resultatAppointment["ENDTIME"].ToString());

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
                        
                        var name = ((ComboboxItemSalesMan) ComboBoxSalesMan.SelectedItem).Value.NAME + " "
                                   + ((ComboboxItemSalesMan) ComboBoxSalesMan.SelectedItem).Value.FIRSTNAME;

                        // SalesMan's name
                        panelAppoint.Children.Add(new TextBlock { Margin = thick, Text = name, Height = 16 });

                        // Day
                        panelAppoint.Children.Add(new TextBlock
                        {
                            Text = "Date : " + resultatAppointment["DAY"],
                            Margin = thick,
                            Height = 16
                        });
                        // Time
                        panelAppoint.Children.Add(new TextBlock
                        {
                            Text = String.Format("De {0} à {1}", resultatAppointment["STARTTIME"], resultatAppointment["ENDTIME"]),
                            Margin = thick,
                            Height = 16
                        });

                        Appointment.Border = bordure;
                        PanelSales.Children.Add(bordure);
                        ListAppointment.Add(Appointment);
                    }
                    resultatAppointment.Close();
                }
                resultCommand.Close();
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
                ModernDialog.ShowMessage("Connection à la base de donnée impossible", "Erreur", MessageBoxButton.OK);
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
                var oCommand = ConnectionOracle.GetAll("SALESMAN");
                var resultat = oCommand.ExecuteReader();
                while (resultat.Read())
                {
                    ComboBoxSalesMan.Items.Add(new ComboboxItemSalesMan
                    {
                        Text = resultat[2].ToString(),
                        Value = new SalesMan(Convert.ToInt32(resultat["ID_SALESMAN"]), resultat["TELEPHONE"].ToString(), resultat["NAME"].ToString(), resultat["FIRSTNAME"].ToString(), resultat["MAIL"].ToString())
                    });
                }
                resultat.Close();
            }
            catch
            {
                ModernDialog.ShowMessage("Connection à la base de donnée impossible", "Erreur", MessageBoxButton.OK);
            }
        }

        
        private void MenuSalesMan_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderSales.Width = MenuSalesMan.ActualWidth - 400;
            BorderSales.Height = MenuSalesMan.ActualHeight - 100;
            try
            {
                var nbMarchandise = ListAppointment.Count;
                for (var i = 0; i < nbMarchandise; i++)
                {
                    ListAppointment[i].Border.Width = BorderSales.Width - 5;
                }
            }
            catch (Exception caught)
            {
                //On initialisation it don't works so here's a try/catch.
                //Need to figure out how to bypass it since it's not useful.
                Console.WriteLine(caught.Message);
                Console.Read();
            }
        }

        private void MenuSalesMan_Loaded(object sender, RoutedEventArgs e)
        {
            var nbSalesMan = ListAppointment.Count;

            if (nbSalesMan == 0)
            {
                return;
            }

            for (var i = 0; i < nbSalesMan; i++)
            {
                ListAppointment[i].Border.BorderBrush = BorderSales.BorderBrush;
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectionOracle.Delete("APPOINTMENT", ((ComboboxItemSalesMan)ComboBoxSalesMan.SelectedItem).Value.ID_SALESMAN, "SALESMAN");
                ConnectionOracle.Delete("SALESMAN", ((ComboboxItemSalesMan)ComboBoxSalesMan.SelectedItem).Value.ID_SALESMAN);
            }
            catch
            {
                ModernDialog.ShowMessage("Connection à la base de donnée impossible", "Erreur", MessageBoxButton.OK);
            }

            ComboBoxSalesMan.Items.Clear();
            InitComboClient();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            var SalesMan = ((ComboboxItemSalesMan)ComboBoxSalesMan.SelectedItem).Value;
            try
            {
                var set = new[,] { { SalesMan.TELEPHONE, TextPhone.Text }, { SalesMan.MAIL, TextMail.Text } };
                ConnectionOracle.Update("SALESMAN", SalesMan.ID_SALESMAN, set);
            }
            catch
            {
                ModernDialog.ShowMessage("Connection à la base de donnée impossible", "Erreur", MessageBoxButton.OK);
            }
            finally
            {
                SalesMan.TELEPHONE = TextPhone.Text;
                SalesMan.MAIL = TextMail.Text;
                ModernDialog.ShowMessage("Informations du commercial " + SalesMan.NAME + SalesMan.FIRSTNAME +" modifiée avec succès", "Opération réussie", MessageBoxButton.OK);
            }
        }

        private void ChangeVisibility(bool visibility)
        {
            if (visibility)
            {
                BorderSales.Visibility = Visibility.Visible;
                LabelPhone.Visibility = Visibility.Visible;

                TextPhone.Visibility = Visibility.Visible;
                TextMail.Visibility = Visibility.Visible;

                BTN_Delete.Visibility = Visibility.Visible;
                BTN_Update.Visibility = Visibility.Visible;
            }
            else
            {
                BorderSales.Visibility = Visibility.Hidden;

                LabelPhone.Visibility = Visibility.Hidden;

                TextPhone.Visibility = Visibility.Hidden;
                TextMail.Visibility = Visibility.Hidden;

                BTN_Delete.Visibility = Visibility.Hidden;
                BTN_Update.Visibility = Visibility.Hidden;
            }
        }
    }
}
