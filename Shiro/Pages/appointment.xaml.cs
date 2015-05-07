﻿// This program is a private software, based on c# source code.
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
                MessageBox.Show("Champs incorrects", "erreur");
            }
            return boolean;
            //MessageBox.Show(TimePickerFinMin.Text +" ,"+ TimePickerFinHeure.Text +" ,"+ TimePickerDebutHeure.Text 
            //    +" ,"+  TimePickerDebutMin.Text +" ,"+  ComboBoxClient.Text+" ,"+ ComboboxSalesMan.Text );
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if(!TextChanged())
            {
                return;
            }
            try
            {
                var queryVerify = String.Empty;

                /* var queryVerify = String.Format("SELECT * FROM {0} WHERE {1} = '{2}' OR {3} = '{4}'", "APPOINTMENT", "TELEPHONE", TextBoxPhone.Text, "MAIL",
                        TextBoxMail.Text);*/
                if(Connection.SizeOf(queryVerify) == 0)
                {
                    var heureDebut = string.Format("{0}:{1}", TimePickerDebutHeure.Text, TimePickerDebutMin.Text);
                    var heureFin = string.Format("{0}:{1}", TimePickerFinHeure.Text, TimePickerFinMin.Text);
                    var idCustomer = ((ComboboxItemCustomer) ComboBoxClient.SelectedItem).Value.IdCustomer;
                    var idSalesman = ((ComboboxItemSalesMan) ComboboxSalesMan.SelectedItem).Value.IdSalesman;

                    var querySelect = String.Format("SELECT max(ID_{0}) FROM {0}", "CUSTOMER");
                    var command = Connection.GetFirst(querySelect);
                    var idAppoint = command.ToString() == String.Empty ? 1 : Convert.ToInt32(command) + 1;
                    Connection.Insert("APPOINTMENT", idAppoint, idCustomer, idSalesman, Date.Fr2Us(DatePickerAppoint.Text), heureDebut, heureFin);
                    ModernDialog.ShowMessage("Le rendez vous à été correctement ajouté", "Succès", MessageBoxButton.OK);
                    ShowAppoint(idAppoint, idSalesman, idCustomer, DatePickerAppoint.Text, heureDebut, heureFin);

                    TimePickerFinMin.Text =
                        TimePickerFinHeure.Text =
                            TimePickerDebutHeure.Text = TimePickerDebutMin.Text = ComboBoxClient.Text = ComboboxSalesMan.Text = String.Empty;
                }
                else
                {
                    ModernDialog.ShowMessage("Ce rendez vous existe deja", "Erreur", MessageBoxButton.OK);
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
            date = string.Format("Le : {0}", Date.Us2Fr(date));
            panelCustomer.Children.Add(new TextBlock {Text = date, Margin = thick, Height = 16});

            // horaire
            var horaire = string.Format("De {0} à {1}", heureDebut, heureFin);
            panelCustomer.Children.Add(new TextBlock {Text = horaire, Margin = thick, Height = 16});

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