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
    /// Interaction logic for Statistik.xaml
    /// </summary>
    public partial class Statistik : Window
    {
        /// <summary>
        /// Konstruktor zum initialisieren des Statistik Fensters
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        public Statistik(int auktionsNr)
        {
            InitializeComponent();
            cbxAuktion.ItemsSource = DBClass.GetAlleAuktionen();
            if (cbxAuktion.Items.Count > 0)
                cbxAuktion.SelectedValue = auktionsNr;

            UpdateAllgemein(false);
           
        }

        private void cbxAuktion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
              
                if (cbxAuktion.SelectedItem != null)
                {
                  UpdateTabelle( int.Parse(cbxAuktion.SelectedItem.ToString()));
       
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            

        }

        /// <summary>
        /// Aktuallisieren statistik Tabelle
        /// </summary>
        /// <param name="auktionsNr"></param>
        private void UpdateTabelle(int auktionsNr)
        {
            double keinZuschlag = DBStats.KeinZuschlagLose(auktionsNr);
            double behandelte = DBStats.BehandeltenLose(auktionsNr);
            double wert;
            if (behandelte != 0)
            {
                wert = (keinZuschlag / behandelte) * 100;
                wert = Math.Round(wert, 2);
            }
            else
            {
                wert = 0;
            }
            try
            {

                
                lblZuschlagspreiseSum.Content = string.Format("{0:#,##0.00} € ", DBStats.SummeZuschagspreise(auktionsNr));
                lblSchaetzpreiseSum.Content = string.Format("{0:#,##0.00} € ", DBStats.SummeSchaetzpreise(auktionsNr));
                lblZuschlagSchatzAbweichung.Content = DBStats.ZuschlagSchaetzsummeAbweichung(auktionsNr)+" %";
                lblPositiveAbweichung.Content = DBStats.PositiveAbweichung(auktionsNr)+ " %";
                lblNegativeAbweichung.Content = DBStats.NegativeAbweichung(auktionsNr) + " %";
                lblDurchschnittAbweichung.Content = DBStats.DurchschnittlicheAbweichung(auktionsNr) + " %";

        
                lblKeinZuschlagBehandelten.Content = DBStats.KeinZuschlagLose(auktionsNr)+" / "+ behandelte;
                lblKeinZuschlagBehandeltenProzent.Content = wert + " % ";
                lblAnzahlZuschlagKunden.Content = DBStats.AnzahlZuschlagKunden(auktionsNr);
                lblDurchschnittZuschlagsummeProKunde.Content = string.Format("{0:#,##0.00} € ", DBStats.DurchschnittZuschlagProKunde(auktionsNr));
                lblMaxZuschlagsummeVonKunde.Content = string.Format("{0:#,##0.00} € ", DBStats.MaxZuschlagSumme(auktionsNr));




            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
    


        }

        /// <summary>
        /// Aktuallisiere Allgemeine Tabelle
        /// </summary>
        /// <param name="beendete">Checkbox beendete</param>
        private void UpdateAllgemein(bool beendete)
        {
            List<int> alleAutionen = new List<int>();
            if (beendete == false)
            {
                alleAutionen = DBClass.GetAlleAuktionen();
            }
            else
            {
                alleAutionen = DBClass.GetAlleAuktionenBeended();
            }
            List<double> prozAbweichung = new List<double>();
            List<double> durchschnittAbweichung = new List<double>();

            List<int> keinZuschlag = new List<int>();
            List<int> behandelten = new List<int>();
            List<int> anzKunde = new List<int>();
            List<double> durchZuschlagKunde = new List<double>();
            List<int> maxZuschlag = new List<int>();

            foreach (var x in alleAutionen)
            {
                prozAbweichung.Add(DBStats.ZuschlagSchaetzsummeAbweichung(x));
                durchschnittAbweichung.Add(DBStats.DurchschnittlicheAbweichung(x));
                keinZuschlag.Add(DBStats.KeinZuschlagLose(x));
                behandelten.Add(DBStats.BehandeltenLose(x));
                anzKunde.Add(DBStats.AnzahlZuschlagKunden(x));
                durchZuschlagKunde.Add(DBStats.DurchschnittZuschlagProKunde(x));
                maxZuschlag.Add(DBStats.MaxZuschlagSumme(x));
            }

            double keinZuschlagWert = keinZuschlag.Average();
            double behandeltenWert = behandelten.Average();
            double wert;
            if (behandeltenWert != 0)
            {
                wert = (keinZuschlagWert / behandeltenWert) * 100;
                wert = Math.Round(wert, 2);
            }
            else
            {
                wert = 0;
            }

            lblZuschlagSchatzAbweichungAll.Content = string.Format("{0:#,##0.00} % ", prozAbweichung.Average());
            lblDurchschnittAbweichungAll.Content = string.Format("{0:#,##0.00} % ", durchschnittAbweichung.Average());

            lblKeinZuschlagBehandeltenAll.Content = Math.Round(keinZuschlag.Average(),2) + " / " +  Math.Round(behandelten.Average(),2);
            lblKeinZuschlagBehandeltenAllProzentual.Content = wert + " %";
            lblAnzahlZuschlagKundenAll.Content = string.Format("{0:#,##0} ", anzKunde.Average());
            lblDurchschnittZuschlagsummeProKundeAll.Content = string.Format("{0:#,##0.00} € ", durchZuschlagKunde.Average());
            lblMaxZuschlagsummeVonKundeAll.Content = string.Format("{0:#,##0.00} € ", maxZuschlag.Average());





        }
        /// <summary>
        /// Aktualisiere die Allgemeine Statistik
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkbBeendeten_Checked(object sender, RoutedEventArgs e)
        {


            cbxAuktion.ItemsSource = DBClass.GetAlleAuktionenBeended();

            if (cbxAuktion.Items.Count > 0)
            {

                UpdateAllgemein(true);
            }
            else
            {
                lblZuschlagSchatzAbweichungAll.Content = 0;
                lblDurchschnittAbweichungAll.Content = 0;
                lblAnzahlZuschlagKundenAll.Content = 0;
                lblDurchschnittZuschlagsummeProKundeAll.Content = 0;
                lblKeinZuschlagBehandeltenAll.Content = 0;
                lblKeinZuschlagBehandeltenAll.Content = "0/0";
                lblMaxZuschlagsummeVonKundeAll.Content = 0;
                lblKeinZuschlagBehandeltenAllProzentual.Content = 0;
            }


        }
        /// <summary>
        /// Aktualisiere die Allgemeine Statistik
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkbBeendeten_UnChecked(object sender, RoutedEventArgs e)
        {

     
            cbxAuktion.ItemsSource = DBClass.GetAlleAuktionen();
            UpdateAllgemein(false);
        }
    }
}
