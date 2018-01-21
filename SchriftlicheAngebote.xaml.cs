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
    /// Interaktionslogik für SchriftlicheAngebote.xaml
    /// </summary>
    public partial class SchriftlicheAngebote : Window
    {
        /// <summary>
        /// Konstruktor zum initialisieren des SchriftlicheAngebote Fensters
        /// </summary>
        /// <param name="auktionsNr">Nummer der Auktion</param>
        public SchriftlicheAngebote(int auktionsNr)
        {
            InitializeComponent();
            lblAuktion.Content = auktionsNr;

        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            int kundenNr;
            Int32.TryParse(txbKundenNummer.Text, out kundenNr);
            int auktionsNr = Convert.ToInt32(lblAuktion.Content);

            if (KundeVorhanden())
            {
                AddGebot window = new AddGebot(kundenNr, Int32.Parse(lblAuktion.Content.ToString()));
                window.Show();
                window.Closed += (x,z) => { lvGebote.ItemsSource = GebotsZeile.GetGeboteZeile(kundenNr,auktionsNr); };

            }

        }




        private void btnAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

 

        ///// <summary>
        ///// Gibt eine liste mit alle gebote an alle Zeilen
        ///// </summary>
        ///// <returns></returns>
        //private List<GebotsZeile> GetGeboteZeile()
        //{
        //    int kundenNr = Convert.ToInt32(txbKundenNummer.Text);
        //    int auktionsNr = Convert.ToInt32(lblAuktion.Content);
        //    int zeileNummer = 1;
        //    List<GebotsZeile> gebotList = new List<GebotsZeile>();
        //    List<Zeile> alleGeboteZeile = DBClass.GetAlleKundeZeile(kundenNr, auktionsNr);

        //    GebotsZeile gebot = new GebotsZeile();
        //    foreach (var item in alleGeboteZeile)
        //    {
        //        gebot = GetGebot(item);
        //        gebot.Zeile = zeileNummer ;
        //        gebotList.Add(gebot);
        //        zeileNummer++;
        //    }
        //    return gebotList;
        //}

        ///// <summary>
        ///// Get eine liste mit alle gebote an bestimmte Zeile
        ///// </summary>
        ///// <param name="z">Zeile Objekt</param>
        ///// <returns></returns>
        //private GebotsZeile GetGebot(Zeile z)
        //{
        //    GebotsZeile g = new GebotsZeile();
         
        //    g.Bemerkung = z.Bemerkung;

        //    for (int i = 0; i < z.Gebot.Count; i++)
        //    {
        //        if(i == 0)
        //        {
        //            g.Nr1 = z.Gebot[i].LosId;
        //            g.Gebot1 = z.Gebot[i].HoechstsGebot;
        //        }else if(i == 1)
        //        {
        //            g.Nr2 = z.Gebot[i].LosId;
        //            g.Gebot2 = z.Gebot[i].HoechstsGebot;
        //        }
        //        else
        //        {
        //            g.Nr3 = z.Gebot[i].LosId;
        //            g.Gebot3 = z.Gebot[i].HoechstsGebot;
        //        }
        //    }
        //    return g;
            
        //}

        /// <summary>
        /// Bei textchange aktualisiert die gebotListe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbKundenNummer_TextChanged(object sender, TextChangedEventArgs e)
        {

            ListViewFuellen();

        }

        private void ListViewFuellen()
        {
            int auktionsNr = Convert.ToInt32(lblAuktion.Content);


            if (!string.IsNullOrEmpty(txbKundenNummer.Text))
            {
                int kundenNr;
                Int32.TryParse(txbKundenNummer.Text, out kundenNr);
                if (Int32.TryParse(txbKundenNummer.Text, out kundenNr))
                {

                    if (DBClass.KundeVorhanden(kundenNr))
                    {


                        lvGebote.ItemsSource = GebotsZeile.GetGeboteZeile(kundenNr, auktionsNr); //GetGeboteZeile();

               
                        int budget = DBClass.GetBudget(kundenNr, auktionsNr);
                        lblBudget.Content = budget != 0 ? budget.ToString() : "Kein Budget";
                    }
                    else
                    {
                        lvGebote.ItemsSource = null;
                    }

                }

            }
        }



        private void btnBudget_Click(object sender, RoutedEventArgs e)
        {
            int kunde, auktion,budget;
            Int32.TryParse(txbKundenNummer.Text, out kunde);
            Int32.TryParse(lblAuktion.Content.ToString(), out auktion);
            Int32.TryParse(txbBudget.Text, out budget);

            if (KundeVorhanden())
            {
                if (budget > 0)
                {
                    if (!DBClass.SchriftlichesGebotVorhanden(kunde, auktion))
                    {
                        DBClass.AddSchriftlichesGebot(kunde, auktion, budget);
                    }
                    else
                    {
                        DBClass.SetBudget(kunde, auktion, budget);

                    }

                    MessageBox.Show("Max Budget erfasst!");
                    //MessageBox.Show(DBClass.GetKundeRestBetrag(kunde, auktion).ToString());
                    //int budgetlbl = DBClass.GetBudget(kundenNr, auktionsNr);
                    lblBudget.Content = budget != 0 ? budget.ToString() : "Kein Budget";
                    txbBudget.Clear();
                }
                else
                {
                    MessageBox.Show("Bitte Preis eingeben!");
                }
            }
            
        }

        private bool KundeVorhanden()
        {
            bool erg = false;
            if (!string.IsNullOrEmpty(txbKundenNummer.Text))
            {
                int kunde;
                Int32.TryParse(txbKundenNummer.Text, out kunde);
                if (Int32.TryParse(txbKundenNummer.Text, out kunde))
                {

                    if (DBClass.KundeVorhanden(kunde))
                    {
                        erg = true;
                    }
                    else
                    {
                        MessageBox.Show("Kunde nicht vorhanden");
                    }

                }
                else
                {
                    MessageBox.Show("Falsche Eingabe");
                }
            }
            else
            {
                MessageBox.Show("Bitte Kunden Nummer eingeben");
            }

            return erg;
        }

        private void GebotVeraendern(object sender, RoutedEventArgs e)
        {
            int kundenNr, auktionNr;
            Int32.TryParse(txbKundenNummer.Text, out kundenNr);
            Int32.TryParse(lblAuktion.Content.ToString(), out auktionNr);
            //int budget = DBClass.GetBudget(kundenNr, auktionNr);

            int zeileId = -1;
            if (lvGebote.SelectedItem != null)
            {
                zeileId = ((GebotsZeile)lvGebote.SelectedItem).ZeileID;

                GebotAendern ga = new GebotAendern(kundenNr, auktionNr, zeileId);
                ga.ShowDialog();
                ListViewFuellen();
            }


        }
    }
}

