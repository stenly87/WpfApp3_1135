using System;
using System.Collections.Generic;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string url = @"http://localhost:42394/WeatherForecast";

        private void btnClick(object sender, RoutedEventArgs e)
        {
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            DataContractJsonSerializer jsonSerializer =
                new DataContractJsonSerializer(typeof(List<WeatherData>));

            using (var stream = response.GetResponseStream())
            {
                var answer =
                    (List<WeatherData>)jsonSerializer.ReadObject(stream);
                listBox1.ItemsSource = null;
                listBox1.Items.Clear();
                listBox1.ItemsSource = answer;
            }

        }

        [Serializable]
        struct WeatherData
        {
            public int id;
            public long date;
            public int temperatureC;
            public int temperatureF;
            public string summary;

            public override string ToString()
            {
                return $"{DateTime.FromBinary(date).ToLongDateString()} {temperatureC} {summary}";
            }
        }

        private void btnClickPost(object sender, RoutedEventArgs e)
        {
            var request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            var data = new WeatherData
            {
                date = DateTime.UtcNow.ToBinary(),
                summary = "жарко",
                temperatureC = 26
            };
            DataContractJsonSerializer dataContractJson =
                new DataContractJsonSerializer(typeof(WeatherData));
            using (var stream = request.GetRequestStream())
                dataContractJson.WriteObject(stream, data);

            try
            {
                var responce = (HttpWebResponse)request.GetResponse();
                Title = responce.StatusCode.ToString();
                if (responce.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = responce.GetResponseStream())
                    {
                        var answer = dataContractJson.ReadObject(stream);
                        listBox1.ItemsSource = null;
                        listBox1.Items.Add(answer);
                    }
                }
            }
            catch (WebException error)
            {
                MessageBox.Show(error.Message);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        WeatherData data;
        int selectedIndex = 0;
        private void listSelect(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;
            selectedIndex = listBox1.SelectedIndex;
            data = (WeatherData)listBox1.SelectedItem;
            datePicker.SelectedDate = DateTime.FromBinary(data.date);
            textC.Text = data.temperatureC.ToString();
            textSummary.Text = data.summary;
        }

        private void btnClickPut(object sender, RoutedEventArgs e)
        {
            if (data.id == 0) return;
            data.date = ((DateTime)datePicker.SelectedDate).ToBinary();
            data.temperatureC = int.Parse(textC.Text);
            data.summary = textSummary.Text;
            DataContractJsonSerializer jsonSerializer =
                new DataContractJsonSerializer(typeof(WeatherData));
            var request = WebRequest.Create(url);
            request.Method = "PUT";
            request.ContentType = "application/json";
            using (var stream = request.GetRequestStream())
                jsonSerializer.WriteObject(stream, data);
            var response = request.GetResponse();
            string result = null;
            using (var stream = response.GetResponseStream())
            {
                byte[] array = new byte[5];
                stream.Read(array, 0, array.Length);
                result = Encoding.UTF8.GetString(array);
            }
            if (result == "false")
                MessageBox.Show("Не удалось обновить объект на сервере");
            else
            {
                listBox1.Items[selectedIndex] = data;
            }
        }
    }
}
