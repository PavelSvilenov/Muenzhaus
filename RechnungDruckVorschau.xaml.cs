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
    /// Interaktionslogik für Druckvorschau.xaml
    /// </summary>
    public partial class RechnungDruckVorschau : Window
    {
        /// <summary>
        /// Konstruktor zum erstellen der DruckVorschau der Rechnung
        /// </summary>
        /// <param name="list">Liste mit Losen die von einem Kunden ersteigert wurden</param>
        public RechnungDruckVorschau(List<Lose> list)
        {
            InitializeComponent();
            PrintDialog druckDialog = new PrintDialog();
            RechnungDrucken sD = new RechnungDrucken(new Size(druckDialog.PrintableAreaWidth, druckDialog.PrintableAreaHeight), list);
            string tempDateiName = System.IO.Path.GetRandomFileName();
            using (XpsDocument xpsDocument = new XpsDocument(tempDateiName, FileAccess.ReadWrite))
            {
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                writer.Write(sD);
                dvDrucken.Document = xpsDocument.GetFixedDocumentSequence(); 
            }

        }
    }
}
