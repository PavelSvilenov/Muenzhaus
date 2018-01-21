using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muenzhaus2
{
    /// <summary>
    /// Dummy Klasse zum Anzeigen der Lose in einem ListView Objekt mit Schätzpreis
    /// </summary>
    public class LosDummy
    {
        //l.LosNummer, l.Bezeichnung, l.Anmerkung, l.MindestGebot, l.Kaeufer, l.ZuschlagPreis, HoechstGebot = 0
        int losNummer, schaetzpreis, kaeufer, zuschlagPreis, hoechstGebot;
        string bezeichnung, anmerkung;

        /// <summary>
        /// Losnummer
        /// </summary>
        public int LosNummer
        {
            get
            {
                return losNummer;
            }

            set
            {
                losNummer = value;
            }
        }
        /// <summary>
        /// Schätzpreis
        /// </summary>
        public int Schaetzpreis
        {
            get
            {
                return schaetzpreis;
            }

            set
            {
                schaetzpreis = (value / 80) * 100;
            }
        }
        /// <summary>
        /// Nummer des Käufers
        /// </summary>
        public int? Kaeufer
        {
            get
            {
                return kaeufer;
            }

            set
            {
                if (value != null)
                    kaeufer = (int)value;
            }
        }
        /// <summary>
        /// Preis zu dem Das Los Verkauft wurde (Hammerpreis)
        /// </summary>
        public int? ZuschlagPreis
        {
            get
            {
                return zuschlagPreis;
            }

            set
            {
                if (value != null)
                    zuschlagPreis = (int)value;
            }
        }
        /// <summary>
        /// Höchstes Schriftliches Gebot
        /// </summary>
        public int HoechstGebot
        {
            get
            {
                return hoechstGebot;
            }

            set
            {
                hoechstGebot = value;
            }
        }
        /// <summary>
        /// Bezeichnung des Loses
        /// </summary>
        public string Bezeichnung
        {
            get
            {
                return bezeichnung;
            }

            set
            {
                bezeichnung = value;
            }
        }
        /// <summary>
        /// Anmerkungen zu dem Los
        /// </summary>
        public string Anmerkung
        {
            get
            {
                return anmerkung;
            }

            set
            {
                anmerkung = value;
            }
        }
    }

}
