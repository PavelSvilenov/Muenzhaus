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


namespace Muenzhaus2
{
    /// <summary>
    /// Interaktionslogik für AddGebot.xaml
    /// </summary>
    public partial class AddGebot : Window
    {
        /// <summary>
        /// Konstruktor zum initialisieren des AddGebote Fensters
        /// </summary>
        /// <param name="kundenNr">Kundenummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        public AddGebot(int kundenNr, int auktionsNr)
        {

            InitializeComponent();
            lblKundenNr.Content = kundenNr ;
            lblAuktion.Content = auktionsNr;
            
            int budget = DBClass.GetBudget(kundenNr, auktionsNr);
            lblBudget.Content = budget != 0 ? budget.ToString() : "Kein Budget";
        }

        
        private void btnAddGebot_Click(object sender, RoutedEventArgs e)
        {
            ValidateForm();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ValidateForm();
            }
        }


        /// <summary>
        /// Validiert die Benutzer eingabe und speichert die Gebote in Datenbank
        /// </summary>
        private void ValidateForm()
        {
            int kundenNr = Convert.ToInt32(lblKundenNr.Content.ToString());
            int auktionNr = Convert.ToInt32(lblAuktion.Content.ToString());
            int los1, gebot1, los2, gebot2, los3, gebot3,mindestPreis;

            try
            {
                List<Gebot> list = new List<Gebot>();
                // Gebot neuesGebot;

                if (!string.IsNullOrEmpty(txbLosNr1.Text) && !string.IsNullOrEmpty(txbGebot1.Text))
                {

                    //list.Add(new Gebot { AuktionsNummer = auktionNr, LosId = Convert.ToInt32(txbLosNr1.Text), Preis = Convert.ToInt32(txbGebot1.Text), Kundennummer = kundenNr });
                    Int32.TryParse(txbLosNr1.Text, out los1);
                    Int32.TryParse(txbGebot1.Text, out gebot1);

                    if (los1 > 0 && gebot1 > 0)
                    {
                        if (DBClass.LosVorhanden(los1, auktionNr))
                        {
                            if (DBClass.KundenGebotVorhanden(kundenNr,auktionNr, los1))
                            {
                                txbLosNr1.Focus();
                                throw new ArgumentException(" Kunde " + kundenNr + " hat schon für Losnummer " + los1 + " geboten");
                            }else if (gebot1 < (mindestPreis=DBClass.GetLosMindestPreis(los1, auktionNr)))
                            {
                                txbGebot1.Focus();
                                throw new ArgumentException("Losnummer: " + los1 + " hat ein Mindestpreis von " + mindestPreis);
                            }
                            // int zeileID = DBClass.AddZeile(kundenNr, txbBemerkung.Text);
                            list.Add(new Gebot { LosId = los1, AuktionsNummer = auktionNr, HoechstsGebot = gebot1});
                            txbLosNr1.Focus();

                            if (!string.IsNullOrEmpty(txbLosNr2.Text))
                            {

                                Int32.TryParse(txbLosNr2.Text, out los2);
                                Int32.TryParse(txbGebot2.Text, out gebot2);
                                if (los2 > 0 && gebot2 > 0 && los2 != los1 && !string.IsNullOrEmpty(txbGebot2.Text))
                                {
                                    if (DBClass.LosVorhanden(los2, auktionNr))
                                    {
                                        if (DBClass.KundenGebotVorhanden(kundenNr, auktionNr, los2))
                                        {
                                            txbLosNr2.Focus();
                                            throw new ArgumentException(" Kunde " + kundenNr + " hat schon für Losnummer " + los2 + " geboten");
                                        }
                                        else if (gebot2 < (mindestPreis = DBClass.GetLosMindestPreis(los2, auktionNr)))
                                        {
                                            txbGebot2.Focus();
                                            throw new ArgumentException("Losnummer: " + los2 + " hat ein Mindestpreis von " + mindestPreis);
                                        }
                                        list.Add(new Gebot { LosId = los2, AuktionsNummer = auktionNr, HoechstsGebot = gebot2 });
                                        txbLosNr1.Focus();

                                        if (!string.IsNullOrEmpty(txbLosNr3.Text))
                                        {

                                            Int32.TryParse(txbLosNr3.Text, out los3);
                                            Int32.TryParse(txbGebot3.Text, out gebot3);
                                            if (los3 > 0 && gebot3 > 0 && los3 != los2 && los3 != los1 && !string.IsNullOrEmpty(txbGebot3.Text))
                                            {
                                                if (DBClass.LosVorhanden(los3, auktionNr))
                                                {
                                                    //neuesGebot = new Gebot { ZeileId = zeileID, LosId = los3, AuktionsNummer = auktionNr, Preis = gebot3, Kundennummer = kundenNr, };
                                                    //DBClass.AddNeuesGebot(neuesGebot);
                                                    if (DBClass.KundenGebotVorhanden(kundenNr,auktionNr, los3))
                                                    {
                                                        txbLosNr3.Focus();
                                                        throw new ArgumentException(" Kunde " + kundenNr + " hat schon für Losnummer " + los3 + " geboten");
                                                    }
                                                    else if (gebot3 < (mindestPreis = DBClass.GetLosMindestPreis(los3, auktionNr)))
                                                    {
                                                        txbGebot3.Focus();
                                                        throw new ArgumentException("Losnummer: " + los3 + " hat ein Mindestpreis von " + mindestPreis);
                                                    }
                                                    list.Add(new Gebot { LosId = los3, AuktionsNummer = auktionNr, HoechstsGebot = gebot3});
                                                    txbLosNr1.Focus();
                                                }
                                                else {
                                                    txbLosNr3.Focus();
                                                    throw new ArgumentException("Los "+los3+" nicht vorhanden");
                                                }
                                            }
                                            else {
                                                txbLosNr3.Focus();
                                                throw new ArgumentException("Falsche Eingabe beim 3. Gebot");
                                            }

                                        }
                                        //neuesGebot = new Gebot { ZeileId = zeileID, LosId = los2, AuktionsNummer = auktionNr, Preis = gebot2, Kundennummer = kundenNr, };
                                        //DBClass.AddNeuesGebot(neuesGebot);
                                    }


                                    else
                                    {
                                        txbLosNr2.Focus();
                                        throw new ArgumentException("Los "+los2+" nicht vorhanden");
                                    }
                                }
                                else
                                {
                                    txbLosNr2.Focus();
                                    throw new ArgumentException("Falsche Eingabe beim 2. Gebot");
                                }

                            }

                            //neuesGebot = new Gebot { ZeileId = zeileID, LosId = los1, AuktionsNummer = auktionNr, Preis = gebot1, Kundennummer = kundenNr, };
                            //DBClass.AddNeuesGebot(neuesGebot);

                            DBClass.AddGebotZeile(list, kundenNr, txbBemerkung.Text);
                            EintraegeLoeschen();

                        }
                        else {
                            txbLosNr1.Focus();
                            throw new ArgumentException("Los "+los1+" nicht vorhanden");
                        }
                    }
                    else
                    {
                        txbLosNr1.Focus();
                        throw new ArgumentException("Falsche Eingabe beim 1. Gebot");
                    }

                    //DBClass.AddGebotZeile(list, kundenNr);

                }
                else
                {
                    txbLosNr1.Focus();
                    throw new ArgumentException("Wennigstens ein Gebot eingeben");
                }
            }
            catch (Exception ex)
            {
                lblErfolg.Content = "Fehler!";
                lblErfolg.Foreground = System.Windows.Media.Brushes.Red;
                MessageBox.Show(ex.Message);
            }


        }


        /// <summary>
        /// Alle Textfelder Entleeren
        /// </summary>
        private void EintraegeLoeschen()
        {
            txbGebot1.Clear();
            txbGebot2.Clear();
            txbGebot3.Clear();
            txbLosNr1.Clear();
            txbLosNr2.Clear();
            txbLosNr3.Clear();
            txbBemerkung.Clear();
            lblErfolg.Content = "Erfolg!";
            lblErfolg.Foreground = System.Windows.Media.Brushes.Green;
        }
        private void btnAbbruch_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
