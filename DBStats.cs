using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muenzhaus2
{
    class DBStats
    {
        // private const string conn_string = @"Data Source=PSVILENOV-PC\SQLEXPRESS;Initial Catalog=ProjectDB;Integrated Security=True";
        //private const string conn_string = @"Data Source=ita-server;Initial Catalog=Muenzhaus2;Persist Security Info=True;User ID=muenzhaus2;Password=muenzhaus2";
        private const string conn_string = @"Data Source=PSVILENOV-PC\SQLEXPRESS;Initial Catalog=Muenzhaus2;Integrated Security=True";




        /// <summary>
        /// Liefert die Summe der Zuschlagspreise aller versteigerten Lose
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummr</param>
        /// <returns>Int Wert mit summe</returns>
        public static int SummeZuschagspreise(int auktionsNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            int erg = 0;
            var summe = (from l in db.Lose
                         where l.AuktionsNummer == auktionsNr
                         && l.Kaeufer != -1
                         select l.ZuschlagPreis).Sum();

            if (summe.HasValue)
            {
                erg = Convert.ToInt32( summe.ToString());
            }

            return erg;
        }
        /// <summary>
        /// Summe der Schätzpreise der versteigerten Lose
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Double wert mit Summe zurück</returns>
        public static double SummeSchaetzpreise(int auktionsNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            double erg = 0;
      
            var summe = (from l in db.Lose
                          where l.AuktionsNummer == auktionsNr
                          && l.Kaeufer != -1
                          && l.Kaeufer != null
                          select l).Sum(x => (int?)x.MindestGebot) ?? 0;

            // Convert.ToDecimal(summe.)
            if (summe > 0)
            {
                erg = BerechneSchaetzpreis(summe);
            }

            return erg;
        }



        /// <summary>
        /// Prozentuale Abweichung der Zuschlagspreisumme von der Schätzpreissumme
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Ergebnis als Double wert</returns>
        public static double ZuschlagSchaetzsummeAbweichung(int auktionsNr)
        {
            double erg = 0;
            int zuchlagSumme = 0;
            double schaetzSumme = 0;
            LinqDBDataContext db = new LinqDBDataContext();


            zuchlagSumme = SummeZuschagspreise(auktionsNr);
            schaetzSumme = SummeSchaetzpreise(auktionsNr);

            if (zuchlagSumme > 0 && schaetzSumme > 0)
            {
                erg = (zuchlagSumme / schaetzSumme - 1) * 100;
            }

            erg = Math.Round(erg, 2);
            return erg;

        }


        /// <summary>
        /// Höchste Positive Abweichung des Zuschlagspreis von Schätzpreis in prozent
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Maximale wert als Double</returns>
        public static double PositiveAbweichung(int auktionsNr)
        {
            double erg = 0;
            List<double> ergList = new List<double>();
            ergList = ProzentualeAbweichungList(auktionsNr);
            if(ergList.Count > 0)
            {
                erg = ergList.Max();
            }

            erg = Math.Round(erg, 2);
            return erg;


        }


        /// <summary>
        /// Höchste Negative Abweichung des Zuschlagspreis von Schätzpreis in prozent
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Maximale wert als Double</returns>
        public static double NegativeAbweichung(int auktionsNr)
        {

            double erg = 0;

            List<double> ergList = new List<double>();
            ergList = ProzentualeAbweichungList(auktionsNr);
            if (ergList.Count > 0)
            {
                erg = ergList.Min();
            }

            erg = Math.Round(erg, 2);
            return erg;


        }



        /// <summary>
        /// Durchschnittliche Abweichung des Zuschlagspreis von Schätzpreis in prozent
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Maximale wert als Double</returns>
        public static double DurchschnittlicheAbweichung(int auktionsNr)
        {
            double erg = 0;

            List<double> ergList = new List<double>();
            ergList = ProzentualeAbweichungList(auktionsNr);
            if (ergList.Count > 0)
            {
                erg = ergList.Average();
            }

            erg = Math.Round(erg, 2);
            return erg;


        }


        /// <summary>
        /// Gibt anzahl alle Lose mit kein Zuschlag
        /// </summary>
        /// <param name="auktionsNr">AutktionsNummer</param>
        /// <returns>Anzahl Lose mit kein zuschlag</returns>
        public static int KeinZuschlagLose(int auktionsNr)
        {
            int erg = 0;
            LinqDBDataContext db = new LinqDBDataContext();


             erg = (from l in db.Lose
                             where l.AuktionsNummer == auktionsNr
                             && l.Kaeufer == -1
                             select l).Count();

            return erg;

        }
        /// <summary>
        /// Anzahl alle bahandelten Lose
        /// </summary>
        /// <param name="auktionsNr">AutktionsNummer</param>
        /// <returns>Anzahl Lose die behandelt sind</returns>
        public static int BehandeltenLose(int auktionsNr)
        {
            int erg = 0;
            LinqDBDataContext db = new LinqDBDataContext();
     
            erg = (from l in db.Lose
                           where l.AuktionsNummer == auktionsNr
                           && l.Kaeufer > 0
                           select l).Count();     
            return erg;

        }

        /// <summary>
        /// Anzahl Kunden mit ein Zuschlag für die Auktion
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Anzahl als integer</returns>
        public static int AnzahlZuschlagKunden(int auktionsNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            int erg = 0;

            int treffer = (from l in db.Lose
                           where l.AuktionsNummer == auktionsNr
                           && l.ZuschlagPreis > 0
                           select l.Kaeufer).Distinct().Count();

            erg = treffer;
            return erg;

        }
        /// <summary>
        /// Durchschnittliche Zuschlagspreissumme pro Kunde
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Double wert mit dem Durchschnitt</returns>
        public static double DurchschnittZuschlagProKunde(int auktionsNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            double erg = 0;

            var treffer = (from l in db.Lose
                           where l.AuktionsNummer == auktionsNr
                           select l.Kaeufer).Distinct();

            List<int> list = new List<int>();
            foreach (var item in treffer)
            {
                if (item.HasValue && item.Value != -1)
                {
                    int summe = (from ls in db.Lose
                                 where ls.AuktionsNummer == auktionsNr
                                 && ls.Kaeufer == item.Value
                                 select ls.ZuschlagPreis.GetValueOrDefault()).Sum();
                    list.Add(summe);

                }
            }
            if (list.Count > 0)
            {
                erg = (from max in list
                       select max).Average();
            }

            erg = Math.Round(erg, 2);
            return  erg;


        }




        /// <summary>
        /// Maximale Zuschlagsumme die von eine Kunde gemacht ist
        /// </summary>
        /// <param name="auktionsNr">auktionsNummer</param>
        /// <returns>Maximale summe als integer</returns>
        public static int MaxZuschlagSumme(int auktionsNr)
        {
            int erg = 0;
            LinqDBDataContext db = new LinqDBDataContext();

            var treffer = (from l in db.Lose
                           where l.AuktionsNummer == auktionsNr
                           select l.Kaeufer).Distinct();

            List<int> list = new List<int>();
            foreach (var item in treffer)
            {
                if (item.HasValue)
                {
                    int summe = (from ls in db.Lose
                                 where ls.AuktionsNummer == auktionsNr
                                 && ls.Kaeufer == item.Value
                                 select ls.ZuschlagPreis.GetValueOrDefault()).Sum();
                    list.Add(summe);

                }
            }
            if (list.Count > 0)
            {
                erg = (from max in list
                       select max).Max();
            }
            return erg;

        }













        /// <summary>
        /// Gibt ein List mit alle prozentualle Abweichungen von Zuschlagspreis von Schätzpreis
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>List mit double werte</returns>
        private static List<double> ProzentualeAbweichungList(int auktionsNr)
        {
          
            double tmp = 0;

            List<Lose> losList = new List<Lose>();
            LinqDBDataContext db = new LinqDBDataContext();

            var list = from l in db.Lose
                       where l.AuktionsNummer == auktionsNr
                       && l.ZuschlagPreis > 0
                       select l;

            losList = list.ToList();

            List<double> ergList = new List<double>();
            if (losList.Count > 0)
            {
                foreach (var item in losList)
                {
                    tmp = ((double)item.ZuschlagPreis / BerechneSchaetzpreis(item.MindestGebot) - 1) * 100;
                    ergList.Add(tmp);
                }

            }


            return ergList;
        }


        /// <summary>
        /// Gibt alle autionen als List zurück
        /// </summary>
        /// <returns>List mit alle Auktionen</returns>
        public static List<int> GibAlleAutionen()
        {
            List<int> ergList = new List<int>();
            LinqDBDataContext db = new LinqDBDataContext();

            ergList = (from a in db.Lose
                       where a.Kaeufer != null
                      
                       select a.AuktionsNummer).Distinct().ToList();

            return ergList;

        }

        /// <summary>
        /// Berechne den Los Schätzpreis
        /// </summary>
        /// <param name="mindestPreis">Mindestpreis eine Los</param>
        /// <returns>Schätzpreis</returns>
        public static double BerechneSchaetzpreis(double mindestPreis)
        {
            int satz = 80;
           // int mindestGebot = schaetzPreis * satz / 100;
            double schaetzPreis = (mindestPreis / satz) * 100;
            return schaetzPreis;
        }

    }
}
