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
    /// Interaktionslogik für GebotAendern.xaml
    /// </summary>
    public partial class GebotAendern : Window
    {
        int zeileId;
        GebotsZeile gz;
        /// <summary>
        /// Konstruktor des Fensters Gebot ändern
        /// </summary>
        /// <param name="kundenNr">Kundennummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <param name="zeileId">ZeilenID</param>
        public GebotAendern( int kundenNr, int auktionsNr, int zeileId)
        {
            InitializeComponent();
            Initialisieren(kundenNr, auktionsNr, zeileId);
        }

        public void Initialisieren(int kundenNr, int auktionsNr, int zeileId)
        {
            this.zeileId = zeileId;
            lblKundenNr.Content = kundenNr;
            lblAuktion.Content = auktionsNr;

            int budget = DBClass.GetBudget(kundenNr, auktionsNr);
            lblBudget.Content = budget != 0 ? budget.ToString() : "Kein Budget";

            this.gz = GebotsZeile.GetGebot(DBClass.GetZeile(zeileId));

            txbGebot1.Text = gz.Gebot1 == 0 ? "" : gz.Gebot1.ToString();
            txbGebot2.Text = gz.Gebot2 == 0 ? "" : gz.Gebot2.ToString();
            txbGebot3.Text = gz.Gebot3 == 0 ? "" : gz.Gebot3.ToString();

            txbLosNr1.Text = gz.Nr1 == 0 ? "" : gz.Nr1.ToString();
            txbLosNr2.Text = gz.Nr2 == 0 ? "" : gz.Nr2.ToString();
            txbLosNr3.Text = gz.Nr3 == 0 ? "" : gz.Nr3.ToString();

            txbBemerkung.Text = gz.Bemerkung;
        }

        /// <summary>
        /// Event beim Drücken der Enter Taste
        /// </summary>
        /// <param name="sender">Button welcher das Event auslöst</param>
        /// <param name="e">KeyEventArgs</param>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                EingabeUebernehmen();
            }
        }

        /// <summary>
        /// Event beim Drücken auf den Butten Gebot ändern
        /// </summary>
        /// <param name="sender">Button welcher das Event auslöst</param>
        /// <param name="e">RoutedEventArgs</param>
        private void AendereGebot(object sender, RoutedEventArgs e)
        {

            EingabeUebernehmen();

        }

        /// <summary>
        /// Bricht den Vorgang ab und schliesst das Fenster
        /// </summary>
        /// <param name="sender">Button welcher das Event auslöst</param>
        /// <param name="e">RoutedEventArgs</param>
        private void Abbrechen(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Methode die die änderungen in die Datenbank übernimmt
        /// </summary>
        private void EingabeUebernehmen()
        {
            int kundenNr = Convert.ToInt32(lblKundenNr.Content.ToString());
            int auktionNr = Convert.ToInt32(lblAuktion.Content.ToString());
            int los1, gebot1, los2, gebot2, los3, gebot3;
            int gebotsID;

            try
            {

                if (!DBClass.AuktionBeendet(auktionNr))
                {
                    #region Gebot1
                    if (!string.IsNullOrEmpty(txbLosNr1.Text) && !string.IsNullOrEmpty(txbGebot1.Text))
                    {
                        #region erstes Gebot prüfen
                        Int32.TryParse(txbLosNr1.Text, out los1);
                        Int32.TryParse(txbGebot1.Text, out gebot1);
                        if (DBClass.LosVorhanden(los1, auktionNr))
                        {
                            if (gebot1 > DBClass.GetLosMindestPreis(los1, auktionNr))
                            {
                                //eingabe zulässig
                                //Los vorhanden
                                if (DBClass.KundenGebotInAndererZeileVorhanden(kundenNr, auktionNr, los1, zeileId) ||
                                    DBClass.KundenGebotVorhandenAusnahme(kundenNr,auktionNr,los1,gz.Gebot1ID,zeileId))
                                {
                                    string s = string.Format("Los mit der Nummer {0} wurde bereits bei einem anderen schriftlichen Gebot verwendet.", los1);
                                    throw new ArgumentException(s);
                                }
                                if (!DBClass.KundenGebotVorhanden(kundenNr, auktionNr, los1))
                                {
                                    //neues Kundengebot anlegen
                                    Gebot g = new Gebot { AuktionsNummer = auktionNr, LosId = los1, HoechstsGebot = gebot1, ZeileId = zeileId };
                                    DBClass.KundenGebotAnlegen(g);
                                }
                                else
                                {
                                    gebotsID = DBClass.GetKundenGebot(zeileId, auktionNr, gz.Nr1).Id;
                                    DBClass.KundenGebotAktualisieren(gebotsID, los1, gebot1);
                                }
                            }
                            else
                            {
                                //Fehlermeldung Gebot unter MindestPreis
                                throw new ArgumentException("Das Gebot muss Mindestens dem Mindestgebotes entsprechen");
                            }
                        }
                        else
                        {
                            //Los nicht vorhanden
                            throw new ArgumentException(string.Format("Los mit der Nummer {0} existiert nicht", los1.ToString()));
                        }
                        #endregion
                    }
                    else if ((string.IsNullOrEmpty(txbLosNr1.Text) && !string.IsNullOrEmpty(txbGebot1.Text)) ||
                                (!string.IsNullOrEmpty(txbLosNr1.Text) && string.IsNullOrEmpty(txbGebot1.Text)))
                    {
                        MessageBox.Show("Eingaben in Gebot 1 falsch");
                        return;
                    }
                    else
                    {
                        if (gz.Nr1 != 0)
                            DBClass.KundenGebotLoeschen(DBClass.GetKundenGebot(zeileId, auktionNr, gz.Nr1).Id);
                    }
                    #endregion
                    #region Gebot2
                    if (!string.IsNullOrEmpty(txbLosNr2.Text) && !string.IsNullOrEmpty(txbGebot2.Text))
                    {
                        #region zweites Gebot prüfen
                        Int32.TryParse(txbLosNr2.Text, out los2);
                        Int32.TryParse(txbGebot2.Text, out gebot2);
                        if (DBClass.LosVorhanden(los2, auktionNr))
                        {
                            if (los2 > 0 && gebot2 > DBClass.GetLosMindestPreis(los2, auktionNr))
                            {
                                //eingabe zulässig
                                //Los vorhanden
                                if (DBClass.KundenGebotInAndererZeileVorhanden(kundenNr, auktionNr, los2, zeileId) ||
                                    DBClass.KundenGebotVorhandenAusnahme(kundenNr, auktionNr, los2, gz.Gebot2ID, zeileId))
                                {
                                    string s = string.Format("Los mit der Nummer {0} wurde bereits bei einem anderen schriftlichen Gebot verwendet.", los2);
                                    throw new ArgumentException(s);
                                }
                                if (!DBClass.KundenGebotVorhanden(kundenNr, auktionNr, los2))
                                {
                                    //neues Kundengebot anlegen
                                    Gebot g = new Gebot { AuktionsNummer = auktionNr, LosId = los2, HoechstsGebot = gebot2, ZeileId = zeileId };
                                    DBClass.KundenGebotAnlegen(g);
                                }
                                else
                                {
                                    gebotsID = DBClass.GetKundenGebot(zeileId, auktionNr, gz.Nr2).Id;
                                    DBClass.KundenGebotAktualisieren(gebotsID, los2, gebot2);
                                }
                            }
                            else
                            {
                                //Fehlermeldung Gebot unter MindestPreis
                                throw new ArgumentException("Das Gebot muss Mindestens dem Mindestgebotes entsprechen");
                            }
                        }
                        else
                        {
                            //Los nicht vorhanden
                            throw new ArgumentException(string.Format("Los mit der Nummer {0} existiert nicht", los2.ToString()));
                        }
                        #endregion
                    }
                    else if ((string.IsNullOrEmpty(txbLosNr2.Text) && !string.IsNullOrEmpty(txbGebot2.Text)) ||
                                (!string.IsNullOrEmpty(txbLosNr2.Text) && string.IsNullOrEmpty(txbGebot2.Text)))
                    {
                        MessageBox.Show("Eingaben in Gebot 2 falsch");
                        return;
                    }
                    else
                    {
                        if (gz.Nr2 != 0)
                            DBClass.KundenGebotLoeschen(DBClass.GetKundenGebot(zeileId, auktionNr, gz.Nr2).Id);
                    }
                    #endregion
                    #region Gebot3
                    if (!string.IsNullOrEmpty(txbLosNr3.Text) && !string.IsNullOrEmpty(txbGebot3.Text))
                    {

                        #region drittes Gebot prüfen
                        Int32.TryParse(txbLosNr3.Text, out los3);
                        Int32.TryParse(txbGebot3.Text, out gebot3);
                        if (DBClass.LosVorhanden(los3, auktionNr))
                        {
                            if (los3 > 0 && gebot3 > DBClass.GetLosMindestPreis(los3, auktionNr))
                            {
                                //eingabe zulässig
                                //Los vorhanden
                                if (DBClass.KundenGebotInAndererZeileVorhanden(kundenNr, auktionNr, los3, zeileId) ||
                                    DBClass.KundenGebotVorhandenAusnahme(kundenNr, auktionNr, los3, gz.Gebot3ID, zeileId))
                                {
                                    string s = string.Format("Los mit der Nummer {0} wurde bereits bei einem anderen schriftlichen Gebot verwendet.", los3);
                                    throw new ArgumentException(s);
                                }
                                if (!DBClass.KundenGebotVorhanden(kundenNr, auktionNr, los3))
                                {
                                    //neues Kundengebot anlegen
                                    Gebot g = new Gebot { AuktionsNummer = auktionNr, LosId = los3, HoechstsGebot = gebot3, ZeileId = zeileId };
                                    DBClass.KundenGebotAnlegen(g);
                                }
                                else
                                {
                                    gebotsID = DBClass.GetKundenGebot(zeileId, auktionNr, gz.Nr3).Id;
                                    DBClass.KundenGebotAktualisieren(gebotsID, los3, gebot3);
                                }
                            }
                            else
                            {
                                //Fehlermeldung Gebot unter MindestPreis
                                throw new ArgumentException("Das Gebot muss Mindestens dem Mindestgebotes entsprechen");
                            }
                        }
                        else
                        {
                            //Los nicht vorhanden
                            throw new ArgumentException(string.Format("Los mit der Nummer {0} existiert nicht", los3.ToString()));
                        }
                        #endregion
                    }
                    else if ((string.IsNullOrEmpty(txbLosNr3.Text) && !string.IsNullOrEmpty(txbGebot3.Text)) ||
                                (!string.IsNullOrEmpty(txbLosNr3.Text) && string.IsNullOrEmpty(txbGebot3.Text)))
                    {
                        MessageBox.Show("Eingaben in Gebot 3 falsch");
                        return;
                    }
                    else
                    {
                        if (gz.Nr3 != 0)
                            DBClass.KundenGebotLoeschen(DBClass.GetKundenGebot(zeileId, auktionNr, gz.Nr3).Id);
                    }
                    #endregion

                    if (!string.IsNullOrEmpty(txbBemerkung.Text))
                    {
                        DBClass.AktualisiereZeile(zeileId, txbBemerkung.Text);
                    }

                    if (string.IsNullOrEmpty(txbLosNr1.Text) && string.IsNullOrEmpty(txbGebot1.Text) &&
                       string.IsNullOrEmpty(txbLosNr2.Text) && string.IsNullOrEmpty(txbGebot2.Text) &&
                       string.IsNullOrEmpty(txbLosNr3.Text) && string.IsNullOrEmpty(txbGebot3.Text))
                    {
                        DBClass.LoescheZeile(zeileId);
                        MessageBox.Show("Zeile gelöscht.");
                    }
                    else
                    {
                        MessageBox.Show("Gebote geändert.");
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Gebote können nachdem die Auktion beendet wurde nicht mehr geändert werden.");
                }
                
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                Initialisieren(kundenNr, auktionNr, zeileId);
            }
        }

    }
}
