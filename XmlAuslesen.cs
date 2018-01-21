using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Muenzhaus2
{
    static class XmlAuslesen
    {
        public static List<Lose> Auslesen(string dateiPfad, out int unZulaessig)
        {
            List<Lose> losListe = new List<Lose>();
            XElement x = XElement.Load(dateiPfad);
            unZulaessig = 0;

            var zulaessigDS = from zeile in x.Elements("Los")
                              where zeile.Element("Losnummer") != null && zeile.Element("Auktionsnummer") != null && zeile.Element("Bezeichnung") != null && zeile.Element("Schaetzpreis") != null && zeile.Element("Mehrwertsteuer") != null
                              select new
                              {
                                  Losnummer = (int)zeile.Element("Losnummer"),
                                  Auktionsnummer = (int)zeile.Element("Auktionsnummer"),
                                  Bezeichnung = (string)zeile.Element("Bezeichnung"),
                                  Anmerkungen = (string)zeile.Element("Anmerkungen"),
                                  Mindestgebot = DBClass.BerechneMindestGebot((int)zeile.Element("Schaetzpreis")),
                                  MehrWertSteuer = Convert.ToBoolean((int)zeile.Element("Mehrwertsteuer"))
                              };

            unZulaessig = (from zeile in x.Elements("Los")
                           where (string)zeile.Element("Losnummer") == null ||
                                 ((string)zeile.Element("Losnummer")).Trim() == "" ||
                                 zeile.Element("Auktionsnummer") == null ||
                                 ((string)zeile.Element("Auktionsnummer")).Trim() == "" ||
                                 zeile.Element("Bezeichnung") == null ||
                                 ((string)zeile.Element("Bezeichnung")).Trim() == "" ||
                                 zeile.Element("Schaetzpreis") == null ||
                                 ((string)zeile.Element("Schaetzpreis")).Trim() == "" ||
                                 zeile.Element("Mehrwertsteuer") == null ||
                                 ((string)zeile.Element("Mehrwertsteuer")).Trim() == ""
                           select zeile.Element("Losnummer")).Count();

            // einzelne Datensätze des LinQ Ausdrucks werden in ein passendes Objekt umgewandelt
            // und zur Liste lose hinzugefügt
            Lose los;
            foreach (var l in zulaessigDS)
            {
                if (l.Losnummer > 0 && l.Auktionsnummer > 0 && l.Bezeichnung != null)
                {
                    los = new Lose { LosNummer = l.Losnummer, AuktionsNummer = l.Auktionsnummer, Bezeichnung = l.Bezeichnung, Anmerkung = l.Anmerkungen, MindestGebot = l.Mindestgebot, Mehrwertsteuer = l.MehrWertSteuer, Valid = true };
                    losListe.Add(los);
                }
            }

            return losListe;
        }
    }
}
