using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Interaktionslogik für LoseAnzeigen.xaml
    /// </summary>
    public partial class LoseAnzeigen : Window
    {
        int aNr;
        private GridViewColumnHeader listViewSortCol;
        private SortAdorner listViewSortAdorner = null;
        List<LosDummy> liste;

        /// <summary>
        /// Konstruktor zum initialisieren des LoseAnzeigen Fensters
        /// </summary>
        /// <param name="auktionsNummer">Nummer der Auktion zu welcher die Lose angezeigt werden</param>
        public LoseAnzeigen(int auktionsNummer)
        {
            InitializeComponent();
            this.aNr = auktionsNummer;
            this.liste = DBClass.AlleLoseMitGebot(aNr);
            lvLose.ItemsSource = liste;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvLose.ItemsSource);
            view.Filter = LoseFilter;
        }

        /// <summary>
        /// Filtert die Lose nach der in der Textbox eingegebenen Bezeichnung
        /// </summary>
        /// <param name="item">item</param>
        /// <returns>bool wert</returns>
        private bool LoseFilter (object item)
        {
            if (String.IsNullOrEmpty(txtBezeichnung.Text))
                return true;
            else
                return((item as LosDummy).Bezeichnung.IndexOf(txtBezeichnung.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        /// <summary>
        /// Event das die Sortierung des ListView Elements anstösst
        /// </summary>
        /// <param name="sender">Control objekt welches das Event auslösst</param>
        /// <param name="e">RoutedEventArgs</param>
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
        /// <summary>
        /// Schliesst das Fenster
        /// </summary>
        /// <param name="sender">Control objekt welches das Event auslösst</param>
        /// <param name="e">RoutedEventArgs</param>
        private void Schliessen(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// Die Lose werden in eine Ergebnisdatei geschrieben in einem Format das es ermöglich die Datei in Excel zu importieren
        /// </summary>
        /// <param name="sender">Control objekt welches das Event auslösst</param>
        /// <param name="e">RoutedEventArgs</param>
        private void InDateiSpeichern(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "*.txt";
            sfd.Filter = "Text|*.txt";
            if (sfd.ShowDialog() == true)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(sfd.FileName,false);
                    string zeile;
                    foreach(var l in liste)
                    {
                        zeile = string.Format("{0}/{1}/{2}/{3}\n", l.LosNummer, l.Bezeichnung, l.Schaetzpreis, l.ZuschlagPreis);
                        sw.WriteLine(zeile);
                    }
                    sw.Close();

                }
                catch (IOException ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// Event zum anstossen des Filters
        /// </summary>
        /// <param name="sender">Control objekt welches das Event auslösst</param>
        /// <param name="e">TextChangedEventArgs</param>
        private void Filtern(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvLose.ItemsSource).Refresh();
        }

        /// <summary>
        /// Event zum Drucken der Ergebnisliste
        /// </summary>
        /// <param name="sender">Control objekt welches das Event auslösst</param>
        /// <param name="e">RoutedEventArgs</param>
        private void ErgebnisListeDrucken(object sender, RoutedEventArgs e)
        {
            if (DBClass.AuktionBeendet(aNr))
            {
                ErgebnisListeDruckVorschau dv = new ErgebnisListeDruckVorschau(DBClass.AlleLose(aNr));
                dv.ShowDialog();
            }
            else
                MessageBox.Show("Auktion noch nicht Beendet!", "Fehler!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
