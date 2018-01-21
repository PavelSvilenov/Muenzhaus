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
    /// Interaktionslogik für RechnungErstellen.xaml
    /// </summary>
    public partial class RechnungErstellen : Window
    {
        private GridViewColumnHeader listViewSortCol;
        private SortAdorner listViewSortAdorner = null;
        int aNr;
        /// <summary>
        /// Konstruktor zum erstellen des RechnungErstellen Fensters
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer der Auktion zu welcher eine Rechnung erstellt werden soll</param>
        public RechnungErstellen(int auktionsNr)
        {
            InitializeComponent();
            this.aNr = auktionsNr;
            lblAuktion.Content = aNr;
            cbxKundenNr.ItemsSource = DBClass.GetAlleKaeufer(aNr);

            if (cbxKundenNr.Items.Count > 0)
            {
                cbxKundenNr.SelectedIndex = 0;
            }
        }

        private void AuswahlVeraendert(object sender, SelectionChangedEventArgs e)
        {
            int kNr;
            if (cbxKundenNr.SelectedValue != null)
            {
                kNr = Convert.ToInt32(cbxKundenNr.SelectedValue.ToString());
                lvZuschlaege.ItemsSource = DBClass.KundeGekaufteLose(kNr, aNr);
            }
        }
        private void lvLosColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvZuschlaege.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lvZuschlaege.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void Drucken(object sender, RoutedEventArgs e)
        {
            int kNr;
            if (cbxKundenNr.SelectedValue != null)
            {
                kNr = Convert.ToInt32(cbxKundenNr.SelectedValue.ToString());
                List<Lose> list = DBClass.KundeGekaufteLose(kNr, aNr);
                if (list.Count > 0)
                {
                    RechnungDruckVorschau dv = new RechnungDruckVorschau(list);
                    dv.ShowDialog();
                }
            }
        }
    }
}
