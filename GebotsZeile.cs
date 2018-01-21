using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muenzhaus2
{
    class GebotsZeile
    {

        public int Zeile { get; set; }
        public int ZeileID { get; set; }
        public int Nr1 { get; set; }
        public int Gebot1 { get; set; }
        public int Gebot1ID { get; set; }
        public int Nr2 { get; set; }
        public int Gebot2 { get; set; }
        public int Gebot2ID { get; set; }
        public int Nr3 { get; set; }
        public int Gebot3 { get; set; }
        public int Gebot3ID { get; set; }
        public string Bemerkung { get; set; }

        public static List<GebotsZeile> GetGeboteZeile(int kundenNr, int auktionsNr)
        {
            int zeileNummer = 1;
            List<GebotsZeile> gebotList = new List<GebotsZeile>();
            List<Zeile> alleGeboteZeile = DBClass.GetAlleKundeZeile(kundenNr, auktionsNr);

            GebotsZeile gebot = new GebotsZeile();
            foreach (var item in alleGeboteZeile)
            {
                gebot = GetGebot(item);
                gebot.Zeile = zeileNummer;
                gebot.ZeileID = item.Id;
                gebotList.Add(gebot);
                zeileNummer++;
            }
            return gebotList;
        }

        /// <summary>
        /// Get eine liste mit alle gebote an bestimmte Zeile
        /// </summary>
        /// <param name="z">Zeile Objekt</param>
        /// <returns></returns>
        public static GebotsZeile GetGebot(Zeile z)
        {
            GebotsZeile g = new GebotsZeile();

            if (z.Bemerkung != null)
                g.Bemerkung = z.Bemerkung;

            for (int i = 0; i < z.Gebot.Count; i++)
            {
                if (i == 0)
                {
                    g.Nr1 = z.Gebot[i].LosId;
                    g.Gebot1 = z.Gebot[i].HoechstsGebot;
                    g.Gebot1ID = z.Gebot[i].Id;
                }
                else if (i == 1)
                {
                    g.Nr2 = z.Gebot[i].LosId;
                    g.Gebot2 = z.Gebot[i].HoechstsGebot;
                    g.Gebot2ID = z.Gebot[i].Id;
                }
                else
                {
                    g.Nr3 = z.Gebot[i].LosId;
                    g.Gebot3 = z.Gebot[i].HoechstsGebot;
                    g.Gebot3ID = z.Gebot[i].Id;
                }
            }
            return g;

        }

    }
}
