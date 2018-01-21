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
using System.Windows.Shapes;

namespace Muenzhaus2
{
    /// <summary>
    /// Interaktionslogik für AuktionAnlegen.xaml
    /// </summary>
    public partial class AuktionAnlegen : Window
    {
        bool aufrufMain = true;
        /// <summary>
        /// Standard-Konstrukter ohne Parameter
        /// </summary>
        public AuktionAnlegen()
        {
            InitializeComponent();
            dpDatum.SelectedDate = DateTime.Today;
        }

        /// <summary>
        /// Konstruktor der eine Übergebene Auktionsnummer erfordert.
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        public AuktionAnlegen(int auktionsNr)
        {
            InitializeComponent();
            dpDatum.SelectedDate = DateTime.Today;
            txtAuktionsNr.Text = auktionsNr.ToString();
            txtAuktionsNr.IsReadOnly = true;
            this.aufrufMain = false;
        }

        /// <summary>
        /// Legt eine Neue Auktion an
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NeueAuktionAnlegen(object sender, RoutedEventArgs e)
        {
            bool ok = true;
            Auktion a = new Auktion();
            int tmp;

            if (!string.IsNullOrEmpty(txtAuktionsNr.Text))
            {
                if (Int32.TryParse(txtAuktionsNr.Text, out tmp))
                    a.AuktionsNummer = tmp;
                else
                {
                    MessageBox.Show("Nur Zahlen erlaubt", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtAuktionsNr.BorderBrush = System.Windows.Media.Brushes.Red;
                    ok = false;
                }
            }
            else
            {
                ok = false;
                txtAuktionsNr.BorderBrush = System.Windows.Media.Brushes.Red;
            }

            if (!string.IsNullOrEmpty(txtStrasse.Text))
            {
                a.Strasse = txtStrasse.Text;
            }
            else
            {
                ok = false;
                txtStrasse.BorderBrush = System.Windows.Media.Brushes.Red;
            }

            if (!string.IsNullOrEmpty(txtHausNr.Text))
            {
                if (Int32.TryParse(txtHausNr.Text, out tmp))
                    a.Hausnummer = Convert.ToByte(tmp);
                else
                {
                    MessageBox.Show("Nur Zahlen erlaubt", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtAuktionsNr.BorderBrush = System.Windows.Media.Brushes.Red;
                    ok = false;
                }
            }
            else
            {
                ok = false;
                txtHausNr.BorderBrush = System.Windows.Media.Brushes.Red;
            }

            if (!string.IsNullOrEmpty(txtPlz.Text))
            {
                if (Int32.TryParse(txtPlz.Text, out tmp))
                    a.Plz = tmp;
                else
                {
                    MessageBox.Show("Nur Zahlen erlaubt", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtAuktionsNr.BorderBrush = System.Windows.Media.Brushes.Red;
                    ok = false;
                }
            }
            else
            {
                ok = false;
                txtPlz.BorderBrush = System.Windows.Media.Brushes.Red;
            }

            if (!string.IsNullOrEmpty(txtOrt.Text))
            {
                a.Ort = txtOrt.Text;
            }
            else
            {
                ok = false;
                txtOrt.BorderBrush = System.Windows.Media.Brushes.Red;
            }

            if (!string.IsNullOrEmpty(txtGebaeudeBez.Text))
            {
                a.GebaeudeBezeichnung = txtGebaeudeBez.Text;
            }
            if (txtBemerkung.Text != null && txtBemerkung.Text.Trim() != "")
            {
                a.Bemerkung = txtBemerkung.Text;
            }
            if (dpDatum.SelectedDate.Value >= DateTime.Today)
                a.Datum = dpDatum.SelectedDate.Value;
            else
            {
                MessageBox.Show("Datum muss älter oder gleich dem heutigen Datum sein", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                ok = false;
            }

            if (ok)
            {
                int aNr;
                if (!DBClass.AuktionVorhanden(a.AuktionsNummer))
                {
                    aNr = DBClass.AuktionAnlegen(a);
                    if (aNr > -1)
                    {
                        string s = string.Format("Anlegen der Auktion mit der Nummer: {0} erfolgreich.", aNr.ToString());
                        MessageBox.Show(s);
                        if (!this.aufrufMain)
                        {
                            this.Close();
                        }
                        else
                        {
                            FensterSaeubern();
                        }
                    }
                }
                else
                {
                    string s = string.Format("Auktion mit der Nummer: {0} bereits vorhanden.", a.AuktionsNummer);
                    MessageBox.Show(s, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("Bitte füllen Sie alle ohne * gekennzeichneten Felder aus", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        /// <summary>
        /// Säubert den Inhalt sämtlicher TextBoxen als vorbereitung zum erneuten eintragen von Werten
        /// </summary>
        private void FensterSaeubern()
        {
            txtAuktionsNr.Text = "";
            txtBemerkung.Text = "";
            txtGebaeudeBez.Text = "";
            txtHausNr.Text = "";
            txtOrt.Text = "";
            txtPlz.Text = "";
            txtStrasse.Text = "";

        }

        /// <summary>
        /// Bricht den Vorgang ab und schliesst das Fenster.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Abbrechen(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
