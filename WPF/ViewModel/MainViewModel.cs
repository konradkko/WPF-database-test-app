using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Windows;

namespace WPF.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region PROPERTY CHANGE
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
        private string name = string.Empty, userEntry = string.Empty, result = string.Empty, log_tb = string.Empty;

        public string Name { get { return name; } set { name = value; OnPropertyChanged("Name"); } }
        public string UserEntry { get { return userEntry; } set { userEntry = value; OnPropertyChanged("UserEntry"); } }
        public string MasterKey = string.Empty;
        public string Result { get { return result; } set { result = value; OnPropertyChanged("Result"); } }
        public string Log_tb { get { return log_tb; } set { log_tb = value; OnPropertyChanged("Log_tb"); } }


        private EncryptedInformation selectedItem;
        public EncryptedInformation SelectedItem { get { return selectedItem; } set { selectedItem = value; OnPropertyChanged("SelectedItem"); } }

        private string selectedNameLabel = "Nie wybrano", selectedDataLabel = "Nie wybrano", selectedKeyLabel = "Nie wybrano";
        public string SelectedNameLabel { get { return selectedNameLabel; } set { selectedNameLabel = value; OnPropertyChanged("SelectedNameLabel"); } }
        public string SelectedDataLabel { get { return selectedDataLabel; } set { selectedDataLabel = value; OnPropertyChanged("SelectedDataLabel"); } }
        public string SelectedKeyLabel { get { return selectedKeyLabel; } set { selectedKeyLabel = value; OnPropertyChanged("SelectedKeyLabel"); } }


        private ObservableCollection<EncryptedInformation> listaDanychZaszyfrowanych = new ObservableCollection<EncryptedInformation>();
        public ObservableCollection<EncryptedInformation> ListaDanychZaszyfrowanych { get { return listaDanychZaszyfrowanych; } set { listaDanychZaszyfrowanych = value; OnPropertyChanged("ListaDanychZaszyfrowanych"); } }



        //<ListView Margin = "100,0,0,0" Width="50px" ItemsSource="{Binding Lista, UpdateSourceTrigger=PropertyChanged}" Height="200"/>
        //private List<int> lista = new List<int>();
        //public List<int> Lista { get { return lista; } set { lista = value; OnPropertyChanged("Lista"); } }
        //Random rnd = new Random();
        //for (int i = 0; i < 20; i++) Lista.Add(rnd.Next(0, 100 + 1));
        //Lista.Sort();
        //Log_tb = String.Format("MIN:{0}, MAX:{1}\n {2}" , Lista.Min(), Lista.Max(), Log_tb);

        public string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "database.db");
        public MainViewModel()
        {
            if (File.Exists(dbPath))
            {
                ListaDanychZaszyfrowanych = LoadData();
            }
            else
            {
                SendQuerry("CREATE TABLE WPF(id INT NULL, name VARCHAR(25) NULL, data VARCHAR(255) NULL, key VARCHAR(50) NULL); " +
                    "CREATE UNIQUE INDEX 'Indeks 1' ON WPF (id);");
            }
        }

        public ObservableCollection<EncryptedInformation> LoadData()
        {
            ObservableCollection<EncryptedInformation> retVal = new ObservableCollection<EncryptedInformation>();

            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT id, name, data, key FROM WPF";
            using var readerdb = command.ExecuteReader();

            while (readerdb.Read())
            {
                var id = readerdb.GetString(0);
                var name = readerdb.GetString(1);
                var data = readerdb.GetString(2);
                var key = readerdb.GetString(3);

                var obiekt = new EncryptedInformation();
                obiekt.Id = id;
                obiekt.Name = name;
                obiekt.Key = key;
                obiekt.EncryptedData = data;
                retVal.Add(obiekt);
            }
            connection.Close();

            return retVal;
        }

        public string SendQuerryAndReturn(string querry)
        {
            string ret = String.Empty;
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = querry;
            using var readerdb = command.ExecuteReader();

            while (readerdb.Read())
            {
                ret = readerdb.GetString(0);
            }
            connection.Close();
            return ret;
        }

        public void SendQuerry(string querry)
        {
            try
            {
                using var connection = new SqliteConnection($"Data Source={dbPath}");
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = querry;
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd bazy danych, obiekt nie dodany. \n" + ex.Message);
            }
        }
    }
}
