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
    /// Interaktionslogik für LosZurueckziehen.xaml
    /// </summary>
    public partial class LosZurueckziehen : Window
    {
        /// <summary>
        /// Konstruktor der LosZurueckziehen Fensters
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        public LosZurueckziehen(int auktionsNr)
        {
            InitializeComponent();
            lblAuktionsnummer.Content = auktionsNr;


            btnZurueckziehen.Click += (sender,e) => { Zurueckziehen(auktionsNr); };

            btnSchliessen.Click += (sender, e) => this.Close();
           


        }
        /// <summary>
        /// Zurückziehen ein Ein los in Auktion
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>true falls geklappt hat</returns>
        private bool Zurueckziehen(int auktionsNr)
        {
            bool erg = false;
            int losNr;

            if(Int32.TryParse( txbLosnummer.Text , out losNr))
            {
                if (DBClass.LosVorhanden(losNr, auktionsNr))
                {
                    DBClass.LosZurueckziehen(losNr, auktionsNr, txbGrund.Text);
                    MessageBox.Show("Losnummer " + losNr + " ist von der Auktion zurückgezogen!");
                    txbLosnummer.Clear();
                    txbGrund.Clear();
                    erg = true;
                }
                else
                {
                    MessageBox.Show("Losnummer nicht vorhanden!");
                }
            }
            else
            {
                MessageBox.Show("Falsche Eingabe");
            }


            return erg;
        }


    }
}
