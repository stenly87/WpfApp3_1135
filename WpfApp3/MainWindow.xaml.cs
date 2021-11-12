using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private QRCode selectedCode;

        public QRCode SelectedCode
        {
            get => selectedCode;
            set
            { 
                selectedCode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedCode"));
            }
        }
        public ObservableCollection<QRCode> Codes { get; set; } = new ObservableCollection<QRCode>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            var codes = QrCodeRestClient.GetAllQRCodes();
            foreach (var code in codes)
                Codes.Add(code);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void btnUpdate(object sender, RoutedEventArgs e)
        {
            if (SelectedCode == null)
            {
                var code = QrCodeRestClient.Post(new QRCode());
                Codes.Add(code);
            }
            else
            {
                if (!QrCodeRestClient.Put(SelectedCode))
                    MessageBox.Show("Не удалось обновить объект на сервере");
            }
        }

        private void btnDelete(object sender, RoutedEventArgs e)
        {
            if (SelectedCode == null)
                return;
            if (!QrCodeRestClient.Delete(SelectedCode))
                MessageBox.Show("Не удалось удалить объект на сервере");
            else
                Codes.Remove(SelectedCode);
        }
    }
}
