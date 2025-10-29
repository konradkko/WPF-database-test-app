using cryptDLL;
using Microsoft.Data.Sqlite;
using System.Windows;
using System.Windows.Controls;


namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Click_Encrypt(object sender, RoutedEventArgs e)
        {
            MainVM.SelectedItem = null;

            MainVM.MasterKey = PasswordBoxMasterKey.Password;
            if (MainVM.Name == "") Log("Pole 'Nazwa' nie może być puste!", true);
            else if (MainVM.UserEntry == "") Log("Pole 'Dane' nie może być puste!", true);
            else if (MainVM.MasterKey == "") Log("Pole 'Klucz' nie może być puste!", true);
            else
            {
                Log(String.Format("Szyfrowanie:'{0}' DATA:'{1}' KEY:'{2}'", MainVM.Name, MainVM.UserEntry, MainVM.MasterKey));
                string encryptedData = Crypt.Encrypt(MainVM.UserEntry, MainVM.MasterKey);
                //Add to list
                var obiekt = new EncryptedInformation();
                obiekt.Id = MainVM.SendQuerryAndReturn("SELECT IFNULL((SELECT MAX(id)+1 from WPF), 1) from WPF;");
                obiekt.Name = MainVM.Name;
                obiekt.Key = MainVM.MasterKey;
                obiekt.EncryptedData = encryptedData;
                //Add to table in db
                try
                {
                    MainVM.SendQuerry(String.Format("INSERT INTO WPF values(IFNULL((SELECT MAX(id)+1 from WPF), 1), '{0}', '{1}', '{2}');",
                        obiekt.Name, obiekt.EncryptedData, obiekt.Key));
                }
                catch (Exception ex)
                {
                    Log("Błąd bazy danych, obiekt nie dodany. \n" + ex.Message, true);
                }
                MainVM.ListaDanychZaszyfrowanych.Add(obiekt);
            }
        }

        private void DataListBox_ItemSelected(object sender, RoutedEventArgs e)
        {
            if (MainVM.SelectedItem != null)
            {
                MainVM.SelectedNameLabel = $"{MainVM.SelectedItem.Id}. {MainVM.SelectedItem.Name}";
                MainVM.SelectedDataLabel = MainVM.SelectedItem.EncryptedData;
                MainVM.SelectedKeyLabel = MainVM.SelectedItem.Key;

                Log(String.Format("Wybrano: '{0}'", MainVM.SelectedItem.Name));
            }
        }

        private void Click_Decrypt(object sender, RoutedEventArgs e)
        {
            if (MainVM.SelectedItem != null)
            {
                MainVM.Result = Crypt.Decrypt(MainVM.SelectedItem.EncryptedData, MainVM.SelectedItem.Key);
                Log(String.Format("Odszyfrowanie:'{0}' DATA:'{1}' KEY:'{2}'", MainVM.SelectedItem.Name, MainVM.SelectedItem.EncryptedData, MainVM.SelectedItem.Key));
            }
        }

        private void Click_DeleteRecord(object sender, RoutedEventArgs e)
        {
            if (MainVM.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show($"Czy na pewno chcesz usunąć wpis:'{MainVM.SelectedItem.Name}'", "Usuwanie", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                {
                    Log($"Usunięto wpis: '{MainVM.SelectedItem.Id}'");
                    MainVM.SendQuerry($"DELETE FROM WPF WHERE id = '{MainVM.SelectedItem.Id}';");
                    MainVM.ListaDanychZaszyfrowanych = MainVM.LoadData();
                }
                else
                {
                    Log($"Anulowano usuwanie wpisu: '{MainVM.SelectedItem.Id}'");
                }
            }
        }

        private void Click_UpdateRecord(object sender, RoutedEventArgs e)
        {
            MainVM.MasterKey = PasswordBoxMasterKey.Password;

            if (MainVM.Name == "") MainVM.Name = MainVM.SelectedItem.Name;
            if (MainVM.UserEntry == "") MainVM.UserEntry = Crypt.Decrypt(MainVM.SelectedItem.EncryptedData, MainVM.SelectedItem.Key);
            if (MainVM.MasterKey == "") Log("Hasło nie może być puste!", true);
            else if (MainVM.SelectedItem != null)
            {
                MessageBoxResult result = MessageBox.Show($"Czy na pewno chcesz edytwać wpis:'{MainVM.SelectedItem.Name}'", "Usuwanie", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                {
                    Log($"Edytwano wpis: '{MainVM.SelectedItem.Id}'");
                    MainVM.SendQuerry($"UPDATE WPF SET name='{MainVM.Name}', data='{Crypt.Encrypt(MainVM.UserEntry, MainVM.MasterKey)}', key='{MainVM.MasterKey}' WHERE id = '{MainVM.SelectedItem.Id}';");
                    MainVM.ListaDanychZaszyfrowanych = MainVM.LoadData();
                }
                else Log($"Anulowano edytwanie wpisu: '{MainVM.SelectedItem.Id}'");
            }
        }





        private void Log(string text, bool error = false)
        {
            MainVM.Log_tb = String.Format("{0} {1}\n", DateTime.Now, text) + MainVM.Log_tb;

            if (error) MessageBox.Show(text, "Błąd danych", MessageBoxButton.OK, MessageBoxImage.Error);
        }



        /*private int mostRepeatedLetters(string text)
        {
            int max = 1, score = 1;
            char lastLetter = text[0];
            for (int i = 1; i < text.Length; i++)
            {
                if (text[i] == lastLetter) score++;
                else score = 1;
                
                if (score > max) max = score;

                lastLetter = text[i];
            }
            return max;
        }
        private bool isIn(int liczba)
        {
            foreach (int i in MainVM.Lista) {
                if (i == liczba) return true;
            }
            return false;
        } */
    }
}