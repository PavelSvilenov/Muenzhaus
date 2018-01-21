using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Muenzhaus2
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Standart Konstruktor des MainWindow Fensters
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            cbxAuktion.ItemsSource = DBClass.GetAlleAuktionen();
            if(cbxAuktion.Items.Count > 0)
                cbxAuktion.SelectedIndex = cbxAuktion.Items.Count-1;
        }

        private void btnErfassen_Click(object sender, RoutedEventArgs e)
        {
            if(cbxAuktion.Text == "")
            {
                MessageBox.Show("Bitte Auktion auswählen");
            }
            else
            {
                SchriftlicheAngebote window = new SchriftlicheAngebote(Int32.Parse(cbxAuktion.Text));
                window.ShowDialog();
            }
        }

        private void btnAuktion_Click(object sender, RoutedEventArgs e)
        {
            if (cbxAuktion.Text == "")
            {
                MessageBox.Show("Bitte Auktion auswählen");
            }
            else
            {
                LiveAuktion window = new LiveAuktion(Int32.Parse(cbxAuktion.Text));
                window.ShowDialog();
            }
        }

        private void btnLosdateiEinlesen_Click(object sender, RoutedEventArgs e)
        {
            XML_Losdatei_einlesen window = new XML_Losdatei_einlesen();
            window.ShowDialog();
            cbxAuktion.ItemsSource = DBClass.GetAlleAuktionen();
        }


        private void btnLoseAnzeigen_Click(object sender, RoutedEventArgs e)
        {
            if (cbxAuktion.Text == "")
            {
                MessageBox.Show("Bitte Auktion auswählen");
            }
            else
            {
                LoseAnzeigen la = new LoseAnzeigen(Int32.Parse(cbxAuktion.Text));
                la.ShowDialog();
            }
            
        }

        private void RechnungErstellen(object sender, RoutedEventArgs e)
        {
            if (cbxAuktion.Text == "")
            {
                MessageBox.Show("Bitte Auktion auswählen");
            }
            else
            {
                RechnungErstellen re = new RechnungErstellen(Int32.Parse(cbxAuktion.Text));
                re.ShowDialog();
            }
            
        }

        private void btnLosZurueck_Click(object sender, RoutedEventArgs e)
        {
            if (cbxAuktion.Text == "")
            {
                MessageBox.Show("Bitte Auktion auswählen");
            }
            else
            {
                LosZurueckziehen re = new LosZurueckziehen(Int32.Parse(cbxAuktion.Text));
                re.ShowDialog();
            }
        }
        private void AuktionAnlegen(object sender, RoutedEventArgs e)
        {
            AuktionAnlegen aa = new AuktionAnlegen();
            aa.ShowDialog();
            cbxAuktion.ItemsSource = DBClass.GetAlleAuktionen();
            cbxAuktion.SelectedIndex = cbxAuktion.Items.Count - 1;
        }

        private void btnStatistik_Click(object sender, RoutedEventArgs e)
        {
            if (cbxAuktion.Text == "")
            {
                MessageBox.Show("Bitte Auktion auswählen");
            }
            else
            {
                Statistik re = new Statistik(Int32.Parse(cbxAuktion.Text));
                re.ShowDialog();
            }
        }






        //private void btnLogin_Click(object sender, RoutedEventArgs e)
        //{



        //    int login = DBClass.UserLogin(txbUsername.Text, txbPassword.Password.ToString());


        //    if (login == 1)
        //    {
        //        LiveAuktion window = new LiveAuktion();
        //        window.Show();
        //        this.Close();
        //    }
        //    else if (login == 2)
        //    {
        //        SchriftlicheAngebote window = new SchriftlicheAngebote();
        //        window.Show();
        //        this.Close();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Falsche user oder password");
        //    }





        //}


    }
}
