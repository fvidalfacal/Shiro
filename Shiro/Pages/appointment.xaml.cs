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
        private static readonly List<Class.Appointment> ListAppointment = new List<Class.Appointment>();

        private void Appointment_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderCustomer.Width = AppointmentPage.ActualWidth - 340;
            BorderCustomer.Height = AppointmentPage.ActualHeight - 70;

            var nbCustomer = ListAppointment.Count;
            for(var i = 0; i < nbCustomer; i++)
            {
                ListAppointment[i].Border.Width = BorderCustomer.Width - 6;
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
                while(resultatSalesman.Read())
                {
                    var text = string.Format("{0} {1}", resultatSalesman["FIRSTNAME"], resultatSalesman["NAME"]);
                    ComboboxSalesMan.Items.Add(new ComboboxItemSalesMan
                    {
                        Text = text,
                        Value =
                            new SalesMan(Convert.ToInt32(resultatSalesman["ID_SALESMAN"]), resultatSalesman["TELEPHONE"].ToString(),
                                resultatSalesman["NAME"].ToString(), resultatSalesman["FIRSTNAME"].ToString(), resultatSalesman["MAIL"].ToString())
                    });
                }
                resultatSalesman.Close();
                var commandClient = Connection.GetAll("CUSTOMER");
                var resultatClient = commandClient.ExecuteReader();
                while(resultatClient.Read())
                {
                    var text = string.Format("{0} {1}", resultatClient["FIRSTNAME"], resultatClient["NAME"]);
                    ComboBoxClient.Items.Add(new ComboboxItemCustomer
                    {
                        Text = text,
                        Value =
                            new Customer(Convert.ToInt32(resultatClient["ID_CUSTOMER"]), resultatClient["TELEPHONE"].ToString(),
                                resultatClient["NAME"].ToString(), resultatClient["FIRSTNAME"].ToString(), resultatClient["MAIL"].ToString(),
                                resultatClient["COMPANY"].ToString())
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
            var command = Connection.GetAll("APPOINTMENT");
            var resultat = command.ExecuteReader();
            while(resultat.Read())
            {
                ShowAppoint(Convert.ToInt32(resultat["ID_APPOINTMENT"]), Convert.ToInt32(resultat["ID_CUSTOMER"]), Convert.ToInt32(resultat["ID_SALESMAN"]),
                    resultat["DAY"].ToString(), resultat["STARTTIME"].ToString(), resultat["ENDTIME"].ToString());
            }
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

        private bool TextChanged()
        {
            var boolean = TimePickerFinMin.Text != string.Empty && TimePickerFinHeure.Text != string.Empty && TimePickerDebutHeure.Text != string.Empty
                          && TimePickerDebutMin.Text != string.Empty && ComboBoxClient.Text != string.Empty && ComboboxSalesMan.Text != string.Empty;
            if(!boolean)
            {
                MessageBox.Show("Champs incorrects ou incomplets", "erreur");
            }
            return boolean;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(!TextChanged())
            {
                return;
            }
            try
            {
                var heureDebut = TimePickerDebutHeure.Text;
                if(Convert.ToInt32(TimePickerDebutHeure.Text) < 10)
                {
                    heureDebut = string.Format("0{0}", TimePickerDebutHeure.Text);
                }

                var minDebut = TimePickerDebutMin.Text;
                if (Convert.ToInt32(TimePickerDebutMin.Text) < 10)
                {
                    minDebut = string.Format("0{0}", TimePickerDebutMin.Text);
                }
                var debut = string.Format("{0}:{1}", heureDebut, minDebut);

                var heureFin = TimePickerFinHeure.Text;
                if (Convert.ToInt32(TimePickerFinHeure.Text) < 10)
                {
                    heureFin = string.Format("0{0}", TimePickerFinHeure.Text);
                }

                var minFin = TimePickerFinMin.Text;
                if (Convert.ToInt32(TimePickerFinMin.Text) < 10)
                {
                    minFin = string.Format("0{0}", TimePickerFinMin.Text);
                }
                var fin = string.Format("{0}:{1}", heureFin, minFin);

                var idCustomer = ((ComboboxItemCustomer) ComboBoxClient.SelectedItem).Value.IdCustomer;
                var idSalesman = ((ComboboxItemSalesMan) ComboboxSalesMan.SelectedItem).Value.IdSalesman;
                 var queryVerifyCustomer = string.Format("SELECT * FROM {0} WHERE {1} = '{2}' AND {3} = '{4}' AND {5} BETWEEN '{6}' AND '{7}' AND {8} BETWEEN '{9}' AND '{10}'", "APPOINTMENT", "ID_CUSTOMER", idCustomer, "DAY",
                        DatePickerAppoint.Text ,"STARTTIME", debut, fin , "ENDTIME", debut, fin) ;
                 var queryVerifySalesman = string.Format("SELECT * FROM {0} WHERE {1} = '{2}' AND {3} = '{4}' AND {5} BETWEEN '{6}' AND '{7}' AND {8} BETWEEN '{9}' AND '{10}'", "APPOINTMENT", "ID_SALESMAN", idSalesman, "DAY",
                        DatePickerAppoint.Text, "STARTTIME", debut, fin, "ENDTIME", debut, fin);
                if(Connection.SizeOf(queryVerifyCustomer) == 0 && Connection.SizeOf(queryVerifySalesman) == 0)
                {
                    var querySelect = string.Format("SELECT max(ID_{0}) FROM {0}", "APPOINTMENT");
                    var command = Connection.GetFirst(querySelect);
                    var idAppoint = command.ToString() == string.Empty ? 1 : Convert.ToInt32(command) + 1;
                    Connection.Insert("APPOINTMENT", idAppoint, idCustomer, idSalesman, DatePickerAppoint.Text, heureDebut, heureFin);
                    ModernDialog.ShowMessage("Le rendez vous à été correctement ajouté", "Succès", MessageBoxButton.OK);
                    ShowAppoint(idAppoint, idCustomer, idSalesman, DatePickerAppoint.Text, debut, fin);

                    TimePickerFinMin.Text =
                        TimePickerFinHeure.Text =
                            TimePickerDebutHeure.Text = TimePickerDebutMin.Text = ComboBoxClient.Text = ComboboxSalesMan.Text = string.Empty;
                }
                else
                {
                    ModernDialog.ShowMessage("Ce rendez vous existe deja ou un des protagoniste à déjà un rendez vous pour cette plage horaire", "Erreur", MessageBoxButton.OK);
                }
            }
            catch
            {
                ModernDialog.ShowMessage("Erreur de connection à la base de données", "Erreur", MessageBoxButton.OK);
            }
        }

        private void ShowAppoint(int id, int idClient, int idSalesman, string date, string heureDebut, string heureFin)
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
            //NomClient
            var cli = string.Empty;
            var commandCli = Connection.Command(string.Format("SELECT NAME, FIRSTNAME FROM CUSTOMER WHERE ID_CUSTOMER ={0}", idClient));
            var resultatClient = commandCli.ExecuteReader();
            while(resultatClient.Read())
            {
                cli = string.Format("{0} {1}", resultatClient["NAME"], resultatClient["FIRSTNAME"]);
            }
            resultatClient.Close();
            //NomCom
            var com = string.Empty;
            var commandCom = Connection.Command(string.Format("SELECT NAME, FIRSTNAME FROM SALESMAN WHERE ID_SALESMAN ={0}", idSalesman));
            var resultatCom = commandCom.ExecuteReader();
            while(resultatCom.Read())
            {
                com = string.Format("{0} {1}", resultatCom["NAME"], resultatCom["FIRSTNAME"]);
            }
            resultatCom.Close();
            // Libelle
            var lib = string.Format("Le rendez vous entre le client '{0}' et le commercial '{1}'", cli, com);
            panelCustomer.Children.Add(new TextBlock {Margin = thick, Text = lib, Height = 16});

            // Date
            date = string.Format("Le : {0}", date.Split(new[] { ' ' })[0]);
            panelCustomer.Children.Add(new TextBlock {Text = date, Margin = thick, Height = 16});

            // horaire
            var horaire = string.Format("De {0} à {1}", heureDebut, heureFin);
            panelCustomer.Children.Add(new TextBlock {Text = horaire, Margin = thick, Height = 16});

            // Button
            var btnDelete = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "Supprimer le rendez vous",
                Margin = new Thickness(9, -30, 67, 50),
                BorderBrush = Brushes.Red,
                Tag = id
            };

            panelCustomer.Children.Add(btnDelete);
            btnDelete.Click += BTN_Delete_Click;
            var newAppoint = new Class.Appointment(id, idClient, idSalesman, date, heureDebut, heureFin);
            PanelCustomer.Children.Add(border);
            newAppoint.Border = border;
            ListAppointment.Add(newAppoint);
        }

        private void BTN_Delete_Click(object sender, EventArgs e)
        {
            var id = ((Button) sender).Tag.ToString();
            if(ModernDialog.ShowMessage("Supprimer ce rendez vous ?", "Etes vous sur ?", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            string[,] where = {{"ID_APPOINTMENT", id}};
            Connection.Delete("APPOINTMENT", where);
            DisplayAll();
        }
    }
}