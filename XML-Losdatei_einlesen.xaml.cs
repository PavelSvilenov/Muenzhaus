using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaktionslogik für XML_Losdatei_einlesen.xaml
    /// </summary>
    public partial class XML_Losdatei_einlesen : Window
    {
        bool zulaessig = true;
        List<Lose> losListe;

        private GridViewColumnHeader listViewSortCol;
        private SortAdorner listViewSortAdorner = null;

        /// <summary>
        /// Standart-Konstruktor zum initialisieren der WPF-Fensters
        /// </summary>
        public XML_Losdatei_einlesen()
        {
            InitializeComponent();
            FensterSaeubern();
        }

        /// <summary>
        /// Versetzt das Fenster in seinen Ursprungszustand zurück
        /// </summary>
        public void FensterSaeubern()
        {
            lblAnzeigeFDS.Visibility = Visibility.Hidden;
            lblAnzFDS.Visibility = Visibility.Hidden;
            lblPfad.Content = "keine Datei ausgewählt";
            losListe = new List<Lose>();
            lblAnzahlLose.Content = losListe.Count.ToString();
            lvLose.ItemsSource = null;
        }

        /// <summary>
        /// Auswählen einer XML-Datei zum Auslesen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateiAuswaehlen(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "XML-Datei auswählen";
            ofd.Filter = "XML-Dateien|*.xml|Alle Dateien|*.*";
            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileName.Contains(".xml"))
                    LoseAnzeigenDialog(ofd.FileName);
                else
                {
                    if (MessageBox.Show("Es wurde ein falscher Dateitp ausgewählt\nBitte wählen Sie eine Datei vom Typ XML.", "Falsche Datei!", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                        DateiAuswaehlen(sender, e);
                }  
            }
        }

        /// <summary>
        /// Mithilfe des Datei-Pfades wird eine XML Datei bestehend aus Losen, für eine bereits 
        /// in der Datenbank existierende Auktion, ausgelesen und anschließend dem Anwender 
        /// am Monitor angezeigt.
        /// </summary>
        /// <param name="dateiPfad">Voller Datei-Pfad zur XML-Datei</param>
        private void LoseAnzeigenDialog(string dateiPfad)
        {
            int unZulaessig;
            losListe = XmlAuslesen.Auslesen(dateiPfad, out unZulaessig);

            // Wenn 1 oder mehr unzulässige Datensätze in der XMl-Datei gefunden wurden, 
            // wird eine Meldung in form eines Labels in roter schriftfarbe für den benutzer sichtbar
            if (unZulaessig > 0)
            {
                lblAnzeigeFDS.Visibility = Visibility.Visible;
                zulaessig = false;
                lblAnzFDS.Visibility = Visibility.Visible;
                lblAnzFDS.Content = unZulaessig.ToString();
            }
            else
            {
                zulaessig = true;
                lblAnzeigeFDS.Visibility = Visibility.Hidden;
                lblAnzFDS.Visibility = Visibility.Hidden;
            }

            lblPfad.Content = dateiPfad.Substring(dateiPfad.LastIndexOf('\\') + 1);
            lblAnzahlLose.Content = losListe.Count.ToString();

            //anbinden der Liste lose an das ListView Objekt der 
            lvLose.ItemsSource = losListe;

        }

        /// <summary>
        /// Jedes eingelesene Los in der losListe wird mithilfe von LinQ to SQL in die Datenbank übertragen
        /// </summary>
        private void UebernehmenInDB()
        {
            if (zulaessig)
            {

                try
                {
                    if (losListe.Count == 0)
                    {
                        MessageBox.Show("Bitte zuerst eine Losdatei zum auslesen auswählen", "Fehler!", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                    else if (DBClass.LosVorhanden(losListe.First()))
                    {
                        MessageBox.Show("Losdatei wurde bereits eingelesen", "Fehler!", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                    else
                    {
                        if (!DBClass.AuktionVorhanden(losListe.First().AuktionsNummer))
                        {
                            if (MessageBox.Show("Die in der XML-Datei angegebene Auktion wurde nicht in der Datenbank angelegt.\nBitte legen Sie die Auktion nun manuell an.", "Auktion nicht gefunden", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                            {
                                // Fenster für Auktion anlegen öffnen
                                AuktionAnlegen aa = new AuktionAnlegen(losListe.First().AuktionsNummer);
                                aa.ShowDialog();
                                if (!DBClass.AuktionVorhanden(losListe.First().AuktionsNummer))
                                    return;
                            }
                            else
                            {
                                // Abbruch
                                return;
                            }
                        }
                        foreach (Lose l in losListe)
                        {
                            DBClass.LosAnlegen(l);
                        }
                        MessageBox.Show("Alle Lose wurden angelegt.", "Anlegen Erfolgreich", MessageBoxButton.OK, MessageBoxImage.Information);
                        FensterSaeubern();
                    }


                    //!DBClass.LosVorhanden(losListe.First())
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Speichern in Datenbank verhindert","Fehlerhafter Datensatz", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Click event zur Sortierung des ListView-Elements über die angeklickte spalte
        /// </summary>
        private void lvLosColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvLose.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lvLose.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        // Event des Buttens btnAbbrechen
        private void Abbrechen(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Sollte vor dem schliessen des Fensters bereits eine Losdatei eingelesen aber noch nicht in die Datenbank gespeichert worden sein
        /// öffnet sich ein Warnfenster indem der Anwender gefragt wird ob er die eingelesene Datei in die Datenbank speichern möchte.
        /// </summary>
        private void Schliessen()
        {
            if (losListe.Count > 0)
            {
                if (!DBClass.LosVorhanden(losListe.First()) && zulaessig)
                {
                    if (MessageBox.Show("Lose wurden noch nicht gespeichert!\nLose jetzt speichern?", "Warnung!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        UebernehmenInDB();
                    }
                }
            }
            //this.Close();
        }

        // Event des Schliessen Kreuzes
        private void Beenden(object sender, CancelEventArgs e)
        {
            Schliessen();
        }

        // Event des Buttens btnUebernehmen
        private void Uebernehmen(object sender, RoutedEventArgs e)
        {
            UebernehmenInDB();
        }
    }


}


