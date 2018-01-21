using System;
using System.Collections.Generic;
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
using System.Windows.Xps;               // System.Printing.dll
using System.Windows.Xps.Packaging;     // ReachFrame.dll

namespace Muenzhaus2
{
    /// <summary>
    /// Interaktionslogik für ErgebnisListe.xaml
    /// </summary>
    public partial class ErgebnisListeDruckVorschau : Window
    {
        /// <summary>
        /// Druckvorschau der ErgebnisListe
        /// </summary>
        /// <param name="liste">Liste mit Losen und deren Ergebnisse</param>
        public ErgebnisListeDruckVorschau(List<Lose> liste)
        {
            InitializeComponent();
            PrintDialog druckDialog = new PrintDialog();
            ErgebnisListeDrucken sD = new ErgebnisListeDrucken(new Size(druckDialog.PrintableAreaWidth, druckDialog.PrintableAreaHeight), liste);
            string tempDateiName = System.IO.Path.GetRandomFileName();
            using (XpsDocument xpsDocument = new XpsDocument(tempDateiName, FileAccess.ReadWrite))
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                writer.Write(sD);
                dvErgebnisListe.Document = xpsDocument.GetFixedDocumentSequence();
            }
        }
    }
}
