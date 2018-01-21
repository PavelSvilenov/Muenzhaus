using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Muenzhaus2
{
    class ErgebnisListeDrucken : System.Windows.Documents.DocumentPaginator
    {
        private int seitenAnzahl;
        private Size seitenGroesse;
        private List<Lose> list;
        public ErgebnisListeDrucken(Size seitenGroesse, List<Lose> list)
        {
            if (list.Count > 0)
            {
                this.PageSize = seitenGroesse;
                this.list = list;
                this.seitenAnzahl = Seiten(list.Count);
            }
            else throw new ArgumentNullException("Keine Elemente zum Drucken");
        }
        /// <summary>
        /// Berechnet wie viele Seiten benötigt werden um alle Zeilen zu drucken 
        /// </summary>
        /// <param name="anzahlZeilen"></param>
        /// <returns></returns>
        public int Seiten(int anzahlZeilen)
        {
            int seitenZahl = 0;
            if (anzahlZeilen > 51)
                seitenZahl = Convert.ToInt32(Math.Ceiling((double)anzahlZeilen / (double)51));
            else
                seitenZahl = 1;
            return seitenZahl;
        }
        public override DocumentPage GetPage(int seitenNummer)
        {


            DrawingVisual dV = new DrawingVisual();
            using (DrawingContext dC = dV.RenderOpen())
            {
                //Fix
                FormattedText absender = new FormattedText("Numis AG | Pfennigweg 9 | 60321 Frankfurt",
                                                            CultureInfo.CurrentUICulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Arial"), 11, Brushes.Black);
                absender.SetFontWeight(FontWeights.ExtraBold, 0, 8);

                string auktionText = string.Format("Auktion {0}", list.First().AuktionsNummer);
                FormattedText auktion = new FormattedText(  auktionText,
                                                            CultureInfo.CurrentUICulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Arial"), 16, Brushes.Black);
                auktion.SetFontWeight(FontWeights.ExtraBold);



                FormattedText rechnungsDatum = new FormattedText(string.Format("Datum {0}", DateTime.Today.ToShortDateString()),
                                                                    CultureInfo.CurrentUICulture,
                                                                    FlowDirection.RightToLeft,
                                                                    new Typeface("Arial"), 12, Brushes.Black);

                FormattedText seitenZahl = new FormattedText(string.Format("Seite {0}/{1}", seitenNummer + 1, seitenAnzahl),
                                                                CultureInfo.CurrentUICulture,
                                                                FlowDirection.RightToLeft,
                                                                new Typeface("Arial"), 12, Brushes.Black);

                //TabellenÜberschrift
                FormattedText tabellenKopf1 = new FormattedText("Los",
                                                                  CultureInfo.CurrentUICulture,
                                                                  FlowDirection.RightToLeft,
                                                                  new Typeface("Arial"), 14, Brushes.Black);

                FormattedText tabellenKopf2 = new FormattedText("Preis in Euro",
                                                                 CultureInfo.CurrentUICulture,
                                                                 FlowDirection.RightToLeft,
                                                                 new Typeface("Arial"), 12, Brushes.Black);

                //HinweisText
                string hinweisText =   @"Hinweis: Bei Preisangabe /// wurde das zugehörige Los nicht verkauft." + "\n" + 
                                        "Die unverkauften Stücke dieser Auktion können bis fünf Wochen nach der " + "\n" + 
                                        "Auktion zwanzig Prozent unter Schätzpreis zu den üblichen Auktionsbe-" + "\n" +
                                        "dingungen erworben werden.";
                FormattedText hinweis = new FormattedText(  hinweisText,
                                                            CultureInfo.CurrentUICulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Arial"), 14, Brushes.Black);





                FormattedText fussZeileRechtsSchriftzug = new FormattedText("Numis",
                                                                             CultureInfo.CurrentUICulture,
                                                                             FlowDirection.LeftToRight,
                                                                             new Typeface("Arial"), 11, Brushes.Black);
                fussZeileRechtsSchriftzug.SetFontWeight(FontWeights.ExtraBold);

                FormattedText fussZeileRechts = new FormattedText("Aktiengesellschaft für\n" +
                                                                    "Münzen und Medallien\n" +
                                                                    "________________________\n" +
                                                                    "\n" +
                                                                    "Pfennigweg 9\n" +
                                                                    "D 60321 Frankfurt\n" +
                                                                    "________________________\n" +
                                                                    "\n" +
                                                                    "Telefon 0 69/34 43 11\n" +
                                                                    "Telefax 0 69/34 43 22\n" +
                                                                    "www.numis.de\n" +
                                                                    "numis@numis.de\n" +
                                                                    "________________________\n" +
                                                                    "\n" +
                                                                    "Aufsichtsratvorsitzender:\n" +
                                                                    "Prof. Dr. Otto Dukatenesel\n" +
                                                                    "\n" +
                                                                    "Vorstand:\n" +
                                                                    "Horst Goldzahn\n" +
                                                                    "Dr. Melusine Silberfisch\n" +
                                                                    "________________________\n" +
                                                                    "\n" +
                                                                    "Amtsgericht Frankfurt\n" +
                                                                    "HRB 23554",
                                                                     CultureInfo.CurrentUICulture,
                                                                     FlowDirection.LeftToRight,
                                                                     new Typeface("Arial"), 11, Brushes.Black);



                //Zeichnen

                dC.DrawText(auktion, new Point(70, 415));
                //Logo
                BitmapImage img = new BitmapImage();
                MemoryStream mem = new MemoryStream();
                Properties.Resources.NumisLogo1.Save(mem, System.Drawing.Imaging.ImageFormat.Bmp);
                mem.Position = 0;
                img.BeginInit();
                img.StreamSource = mem;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();

                dC.DrawImage(img, new Rect(450, 120, 260, 130));


                ////Rechnungsdatum
                dC.DrawText(rechnungsDatum, new Point(570, 415));
                ////seite
                dC.DrawText(seitenZahl, new Point(570, 430));

                ////Tabellenkopf
                dC.DrawLine(new Pen(Brushes.Black, 1), new Point(50, 465), new Point(540, 465));
                //erste Spalte
                dC.DrawText(tabellenKopf1, new Point(100, 475));
                dC.DrawText(tabellenKopf2, new Point(190, 475));
                //zweite Spalte
                dC.DrawText(tabellenKopf1, new Point(240, 475));
                dC.DrawText(tabellenKopf2, new Point(330, 475));
                //dritte Spalte
                dC.DrawText(tabellenKopf1, new Point(380, 475));
                dC.DrawText(tabellenKopf2, new Point(470, 475));
                dC.DrawLine(new Pen(Brushes.Black, 1), new Point(50, 500), new Point(540, 500));

                //TabellenInhalt
                int ersteZeile = 520;
                int aZeile = ersteZeile;
                int zeilenAbstand = 25;
                int maxLoseProSeite = 51;
                int start = (seitenNummer * 1) * maxLoseProSeite;
                FormattedText losNummer, preis;
                int temp = 0;
                for (int i = start; i < list.Count && i < (start + maxLoseProSeite); i++)
                {
                    losNummer = new FormattedText(  string.Format("{0}",list[i].LosNummer),
                                                    CultureInfo.CurrentUICulture,
                                                    FlowDirection.RightToLeft,
                                                    new Typeface("Arial"), 14, Brushes.Black);
                    if (list[i].Kaeufer == -1)
                    {
                        //kein Zuschlag
                        preis = new FormattedText(@"///",
                                                    CultureInfo.CurrentUICulture,
                                                    FlowDirection.RightToLeft,
                                                    new Typeface("Arial"), 14, Brushes.Black);
                    }
                    else if (list[i].Valid == false)
                    {
                        //Los Zurückgezogen
                        preis = new FormattedText("zurückgez.",
                                                CultureInfo.CurrentUICulture,
                                                FlowDirection.RightToLeft,
                                                new Typeface("Arial"), 14, Brushes.Black);
                    }
                    else
                    {
                        preis = new FormattedText(string.Format("{0}", list[i].ZuschlagPreis),
                                                    CultureInfo.CurrentUICulture,
                                                    FlowDirection.RightToLeft,
                                                    new Typeface("Arial"), 14, Brushes.Black);
                    }

                    temp = i % 3 + 1;
                    //if(i==0)
                    //{
                    //    //erste Spalte
                    //    dC.DrawText(losNummer, new Point(100, aZeile));
                    //    dC.DrawText(preis, new Point(190, aZeile));
                    //}
                    //else if (i%3==0)
                    if (temp % 3 == 0 && temp % 2 != 0)
                    {
                        //dritte Spalte
                        dC.DrawText(losNummer, new Point(380, aZeile));
                        dC.DrawText(preis, new Point(470, aZeile));
                        aZeile = aZeile + zeilenAbstand;
                    }
                    else if(temp % 2==0)
                    {
                        //zweite Spalte
                        dC.DrawText(losNummer, new Point(240, aZeile));
                        dC.DrawText(preis, new Point(330, aZeile));
                    }
                    else
                    {
                        //erste Spalte
                        dC.DrawText(losNummer, new Point(100, aZeile));
                        dC.DrawText(preis, new Point(190, aZeile));
                    }
                    
                }

                if(seitenNummer == 0)
                {
                    dC.DrawText(hinweis, new Point(70, 1000));
                }

                //RechteFußzeile
                dC.DrawText(fussZeileRechtsSchriftzug, new Point(600, 725));
                dC.DrawText(fussZeileRechts, new Point(600, 740));


                //Blattgröße
                dC.DrawLine(new Pen(Brushes.White, 1), new Point(1, 1), new Point(1, 1104));
                dC.DrawLine(new Pen(Brushes.White, 1), new Point(1, 1), new Point(768, 1));
            }
            return new DocumentPage(dV);
        }
        public override Size PageSize
        {
            get { return seitenGroesse; }
            set { seitenGroesse = value; }
        }
        public override bool IsPageCountValid
        {
            get { return true; }
        }
        public override int PageCount
        {
            get { return seitenAnzahl; }
        }
        public override IDocumentPaginatorSource Source
        {
            get { return null; }
        }

    }
}
