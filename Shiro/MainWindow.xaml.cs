using System;
using System.Windows;

using FirstFloor.ModernUI.Windows.Controls;

using Shiro.Class;

namespace Shiro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void userConnection_Click(object sender, RoutedEventArgs e)
        {
            var userPasswordEncrypted = SHA1Util.SHA1HashStringForUTF8String(password.Password);
            //TODO : SQL SERVER
            //var command = Connection.Command(String.Format("SELECT LOGIN, PASSWORD FROM SALESMAN WHERE LOGIN = {0} AND PASSWORD = {1}", login.Text, userPasswordEncrypted));
            var command = ConnectionOracle.sizeOf(String.Format("SELECT LOGIN, PASSWORD FROM SALESMAN WHERE LOGIN = '{0}' AND PASSWORD = '{1}'", login.Text, userPasswordEncrypted));
            if(command == 1)
            {
                var Index = new Index();
                Index.Show();
                Close();
            }
            else
            {
                ModernDialog.ShowMessage("Nom de compte ou mot de passe incorrect", "Erreur", MessageBoxButton.OK);
            }
        }
    }
}
