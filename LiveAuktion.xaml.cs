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
    /// Interaktionslogik für LiveAuktion.xaml
    /// </summary>
    public partial class LiveAuktion : Window
    {
        /// <summary>
        /// Konstruktor der LiveAuktion Fensters
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        public LiveAuktion(int auktionsNr)
        {
            InitializeComponent();

            btnNextLos.Visibility = Visibility.Hidden;


            lblAuktion.Content = auktionsNr.ToString();

            List<Lose> loseList = DBClass.AlleNichtVersteigertenLose(auktionsNr);
            ListCollectionView listView = new ListCollectionView(loseList);
            grdGesamt.DataContext = listView;
            bool geboteDrin = DBClass.GeboteVorhanden(auktionsNr);


            try
            {


                if (listView.Count > 0)
                {

                    lblKeineLose.Visibility = Visibility.Hidden;

                    UpdateGebot(loseList[0].LosNummer);
               

                    btnNextLos.Click += (sender, e) =>
                    {
                        if (listView.CurrentPosition != listView.Count - 1)
                        {

                            listView.MoveCurrentToNext();

                            UpdateGebot(Int32.Parse(lblLosNummer.Content.ToString()));




                        }
                    };
                    btnNextLos2.Click += (sender, e) =>
                    {
                        if (listView.CurrentPosition != listView.Count - 1)
                        {

                            listView.MoveCurrentToNext();

                            UpdateGebot(Int32.Parse(lblLosNummer.Content.ToString()));



                        }
                    };


                    btnZuschlag.Click += (sender, e) =>
                    {
                        if (listView.CurrentPosition != listView.Count)
                        {
                            if (Zuschlagen())
                            {
                                if (listView.CurrentPosition != listView.Count - 1)
                                {
                                    listView.MoveCurrentToNext();

                                    UpdateGebot(Int32.Parse(lblLosNummer.Content.ToString()));
                                }
                                else
                                {
                                    MessageBox.Show("Auktion Beendet!");
                                    btnZuschlag.IsEnabled = false;
                                    btnKeinZuschlag.IsEnabled = false;
                                }

                            }
                        }

                    };

                    btnKeinZuschlag.Click += (sender, e) =>
                    {
                        if (KeinZuschlag())
                        {

                            if (listView.CurrentPosition != listView.Count - 1)
                            {

                                listView.MoveCurrentToNext();

                                UpdateGebot(Int32.Parse(lblLosNummer.Content.ToString()));



                            }
                        }
                    };

                    btnLosReinziehen.Click += (sender, e) =>
                     {


                         //MessageBox.Show(lblLosNummer.Content.ToString());

                         try
                         {
                             int losnummer = Convert.ToInt32(lblLosNummer.Content);

                             if (MessageBox.Show("Sind Sie sicher, dass Sie das Los mit dem Nummer: " + losnummer + " wieder in Auktion reinziehen wollen?", "Los Reinziehen", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                             {
                                 DBClass.LosZulassen(losnummer, auktionsNr);
                             }

                             UpdateGebot(Convert.ToInt32(lblLosNummer.Content));
                         }
                         catch (Exception ex)
                         {

                             MessageBox.Show(ex.Message);
                         }
          

                     };



                   // UpdateGebot(Int32.Parse( lblLosNummer.Content.ToString()));
                }
                else
                {
        
                    grdSchriftlich.Visibility = Visibility.Hidden;
                    grdMeldung.Visibility = Visibility.Hidden;
                    grdLose.Visibility = Visibility.Hidden;
                    lblKeineLose.Visibility = Visibility.Visible;
                    btnNextLos2.Visibility = Visibility.Hidden;
                    btnLosReinziehen.Visibility = Visibility.Hidden;
                    btnZuschlag.IsEnabled = false;
                    btnKeinZuschlag.IsEnabled = false;

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }


        /// <summary>
        /// Maximum, Aktuelle und Mindest Gebot nach Losnummer akttualisieren
        /// </summary>
        /// <param name="losNr">Lose Nummer</param>
        private void UpdateGebot(int losNr)
        {
            int auktion = Int32.Parse(lblAuktion.Content.ToString());
            Gebot maxGebot = DBClass.GetMaxGebot(losNr, auktion);
            int aktuellPreis = DBClass.GetAktuellPreis(losNr, auktion);
            int mindestPreis = DBClass.GetLosMindestPreis(losNr, auktion);
            bool aktiv = DBClass.LosValid(losNr, auktion);

            //MessageBox.Show(aktuellPreis.ToString());
            lblMindestPreis.Content = mindestPreis;



            if (aktiv)
            {

                grdMeldung.Visibility = Visibility.Hidden;
                grdSchriftlich.Visibility = Visibility.Visible;
                btnZuschlag.Visibility = Visibility.Visible;
                btnNextLos2.Visibility = Visibility.Hidden;
                btnKeinZuschlag.Visibility = Visibility.Visible;
                btnLosReinziehen.Visibility = Visibility.Hidden;


                if (maxGebot != null)
                {

                    if (maxGebot.HoechstsGebot >= mindestPreis)
                    {
                        lblHoechstesGebot.Content = maxGebot.HoechstsGebot;
                        lblKunde.Content = maxGebot.Zeile.Kundennummer;
                        btnKeinZuschlag.IsEnabled = false;
                    }
                    else
                    {
                        lblHoechstesGebot.Content = null;
                        lblKunde.Content = null;
                        btnKeinZuschlag.IsEnabled = true;
                    }
                }
                else
                {
                    lblHoechstesGebot.Content = null;
                    lblKunde.Content = null;
                    btnKeinZuschlag.IsEnabled = true;
                }
                if (aktuellPreis > 0)
                {
                    if (maxGebot.HoechstsGebot >= aktuellPreis)
                    {
                        if (aktuellPreis < mindestPreis)
                        {
                            lblAktuellesGebot.Content = mindestPreis;
                          
                        }
                        else
                        {
                            if (aktuellPreis > mindestPreis)
                            {
                                lblAktuellesGebot.Content = aktuellPreis;
                            
                            }
                        }

                    }
                    else
                    {
                        //if (maxGebot.HoechstsGebot > mindestPreis)
                        //{
                            MessageBox.Show("3");
                            lblAktuellesGebot.Content = maxGebot.HoechstsGebot;
                       // }
                   
                    }
                }
                else
                {
                   
                    lblAktuellesGebot.Content = null;

                }



            }
            else
            {
                grdMeldung.Visibility = Visibility.Visible;
                grdSchriftlich.Visibility = Visibility.Hidden;
                lblGrund.Content = DBClass.LosInvalidGrund(losNr, auktion);
                btnKeinZuschlag.Visibility = Visibility.Hidden;
                btnLosReinziehen.Visibility = Visibility.Visible;
                btnZuschlag.Visibility = Visibility.Hidden;
                btnNextLos2.Visibility = Visibility.Visible;

            }



        }

        private void btnAbbruch_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private bool KeinZuschlag()
        {
            int losNr = Convert.ToInt32(lblLosNummer.Content);
            int auktionsNr = Convert.ToInt32(lblAuktion.Content);
            bool erfolg = false;
            if (MessageBox.Show("Sind Sie sicher, dass kein Zuschlag gibt ?", "Kein Zuschlag", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DBClass.KeinZuschlag(losNr, auktionsNr);
                erfolg = true;
            }
            return erfolg;

        }
        /// <summary>
        /// Zuschlag für ein Aktuelles Los eingeben
        /// </summary>
        /// <returns></returns>
        private bool Zuschlagen()
        {
            int losNr = Convert.ToInt32(lblLosNummer.Content);
            int auktionsNr = Convert.ToInt32(lblAuktion.Content);
            bool erfolg = false;
            int kaufPreis, kundenNr;
            int kundenNrAktuell = Convert.ToInt32(lblKunde.Content);

            if (!string.IsNullOrEmpty(txbKaeufer.Text) && !string.IsNullOrEmpty(txbPreis.Text))
            {
                Int32.TryParse(txbKaeufer.Text, out kundenNr);
                Int32.TryParse(txbPreis.Text, out kaufPreis);
                if (kundenNr > 0 && kaufPreis > 0)
                {
                    if (DBClass.KundeVorhanden(kundenNr) && kundenNr != -1)
                    {
                        if (kundenNr != kundenNrAktuell && kaufPreis <= Convert.ToInt32(lblHoechstesGebot.Content))
                        {
                            MessageBox.Show("Kaufpreis muss größer als Höchstes Gebot sein");
                            txbPreis.Focus();
                        }
                        else if (kundenNr == kundenNrAktuell && kaufPreis < Convert.ToInt32(lblAktuellesGebot.Content))
                        {
                            MessageBox.Show("Kaufpreis muss größer als Aktuelles Gebot sein");
                            txbPreis.Focus();

                        }
                        else if (kaufPreis < Convert.ToInt32(lblMindestPreis.Content))
                        {
                            MessageBox.Show("Kaufpreis darf nicht kleiner als Mindestpreis sein");
                            txbPreis.Focus();
                        }
                        else
                        {
                            DBClass.ZuschlagSetzen(losNr, auktionsNr, kundenNr, kaufPreis);
                            MessageBox.Show("Zuschlag aufgenomen");
                            erfolg = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Kunde nicht vorhanden");
                        txbKaeufer.Focus();

                    }
                }
                else
                {
                    MessageBox.Show("Falsche Eingabe");
                    txbKaeufer.Focus();
                }
            }
            else
            {
                MessageBox.Show("Bitte Käufer und Kaufpreis eingeben");
                txbKaeufer.Focus();
            }

            return erfolg;
        }


    }   
}
