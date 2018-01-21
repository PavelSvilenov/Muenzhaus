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
    class RechnungDrucken : System.Windows.Documents.DocumentPaginator
    {
        private int seitenAnzahl;
        private Size seitenGroesse;
        private List<Lose> list;
        public RechnungDrucken(Size seitenGroesse, List<Lose> list)
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
            if (anzahlZeilen > 13)
                seitenZahl = Convert.ToInt32(Math.Ceiling((double)anzahlZeilen / (double)13));
            else
                seitenZahl = 1;
            return seitenZahl;
        }
        public override DocumentPage GetPage(int seitenNummer)
        {
            int maxLose = 10;
            if (list.Count > 10)
                maxLose = 13;

            int start = (seitenNummer * 1) * maxLose;

            #region Abschlussrechnungen
            //Abschlussrechnungen (Brutto,Netto, Aufgeld, MWST)
            decimal nettoPreis = 0;
            decimal nettoPreisMWST = 0;
            foreach (Lose l in list)
            {
                nettoPreis = nettoPreis + l.ZuschlagPreis.Value;
                if(l.Mehrwertsteuer == true)
                {
                    nettoPreisMWST = nettoPreisMWST + l.ZuschlagPreis.Value;
                }
            }
            decimal aufgeld = nettoPreis * 0.15m;
            decimal mehrWertSteuer = nettoPreisMWST * 0.19m;
            decimal bruttoPreis = nettoPreis + aufgeld + mehrWertSteuer;
            #endregion

            DrawingVisual dV = new DrawingVisual();
            using (DrawingContext dC = dV.RenderOpen())
            {
                //Fix
                FormattedText absender = new FormattedText("Numis AG | Pfennigweg 9 | 60321 Frankfurt",
                                                            CultureInfo.CurrentUICulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Arial"), 11, Brushes.Black);
                absender.SetFontWeight(FontWeights.ExtraBold, 0, 8);


                FormattedText addressat = new FormattedText(string.Format("{0} {1}\n", list.First().Kunden.Vorname, list.First().Kunden.Name) +
                                                            string.Format("{0} {1}\n", list.First().Kunden.Strasse, list.First().Kunden.Hausnummer) +
                                                            string.Format("{0} {1}", list.First().Kunden.Plz, list.First().Kunden.Ort),
                                                            CultureInfo.CurrentUICulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Arial"), 12, Brushes.Black);

                FormattedText betreff = new FormattedText(string.Format("Rechnung {0}X{1}", list.First().AuktionsNummer, list.First().Kaeufer),
                                                            CultureInfo.CurrentUICulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Arial"), 14, Brushes.Black);

                betreff.SetFontWeight(FontWeights.ExtraBold);
                FormattedText auktion = new FormattedText(string.Format("Auktion {0}", list.First().AuktionsNummer),
                                                            CultureInfo.CurrentUICulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Arial"), 12, Brushes.Black);
                FormattedText kunde = new FormattedText(string.Format("Kundennummer {0}", list.First().Kaeufer),
                                                            CultureInfo.CurrentUICulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Arial"), 12, Brushes.Black);

                FormattedText rechnungsDatum = new FormattedText(string.Format("Rechnungsdatum {0}", DateTime.Today.ToShortDateString()),
                                                                    CultureInfo.CurrentUICulture,
                                                                    FlowDirection.RightToLeft,
                                                                    new Typeface("Arial"), 12, Brushes.Black);

                FormattedText seitenZahl = new FormattedText(string.Format("Seite {0}/{1}", seitenNummer + 1, seitenAnzahl),
                                                                CultureInfo.CurrentUICulture,
                                                                FlowDirection.RightToLeft,
                                                                new Typeface("Arial"), 12, Brushes.Black);

                FormattedText tabellenKopf1 = new FormattedText("Los",
                                                                  CultureInfo.CurrentUICulture,
                                                                  FlowDirection.LeftToRight,
                                                                  new Typeface("Arial"), 14, Brushes.Black);
                tabellenKopf1.SetFontStyle(FontStyles.Italic);

                FormattedText tabellenKopf2 = new FormattedText("Bezeichnung",
                                                                 CultureInfo.CurrentUICulture,
                                                                 FlowDirection.LeftToRight,
                                                                 new Typeface("Arial"), 14, Brushes.Black);
                tabellenKopf2.SetFontStyle(FontStyles.Italic);

                FormattedText tabellenKopf3 = new FormattedText("Preis in Euro",
                                                                 CultureInfo.CurrentUICulture,
                                                                 FlowDirection.RightToLeft,
                                                                 new Typeface("Arial"), 14, Brushes.Black);
                tabellenKopf3.SetFontStyle(FontStyles.Italic);

                FormattedText nettoBetLbl = new FormattedText(  "Nettobetrag",
                                                                 CultureInfo.CurrentUICulture,
                                                                 FlowDirection.LeftToRight,
                                                                 new Typeface("Arial"), 14, Brushes.Black);

                FormattedText bruttoBetLbl = new FormattedText("Gesamtbetrag",
                                                                 CultureInfo.CurrentUICulture,
                                                                 FlowDirection.LeftToRight,
                                                                 new Typeface("Arial"), 14, Brushes.Black);

                FormattedText aufGeldLbl = new FormattedText("Aufgeld",
                                                                 CultureInfo.CurrentUICulture,
                                                                 FlowDirection.LeftToRight,
                                                                 new Typeface("Arial"), 14, Brushes.Black);
                FormattedText steuerBetLbl = new FormattedText("19% UST",
                                                                 CultureInfo.CurrentUICulture,
                                                                 FlowDirection.LeftToRight,
                                                                 new Typeface("Arial"), 14, Brushes.Black);
                FormattedText hinweis = new FormattedText(      "Mit * gekennzeichnete Lose sind Mehrwertsteuerpflichtig.",
                                                                 CultureInfo.CurrentUICulture,
                                                                 FlowDirection.LeftToRight,
                                                                 new Typeface("Arial"), 12, Brushes.Black);

                FormattedText fussZeile = new FormattedText(    "Zahlbar ohne Abzug nach Erhalt der Rechnung\n" +
                                                                "Eigentum bis zur vollständigen Bezahlung vorbehalten\n" +
                                                                "Umsatzsteuer-Identifikationsnummer DE 249646087\n\n" +
                                                                "Frankfurter Sparkasse IBAN DE83 5005 0201 0004 7550 04\n" +
                                                                "Deutsche Bank AG IBAN DE47 5007 0010 0096 4313 00\n" +
                                                                "Commerzbank AG IBAN DE24 5004 0000 0589 6238 00",
                                                                 CultureInfo.CurrentUICulture,
                                                                 FlowDirection.LeftToRight,
                                                                 new Typeface("Arial"), 11, Brushes.Black);

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
                
                //dC.DrawImage(new BitmapImage(new Uri(@"\\ita-server\alle\NumisLogo1.jpg")), new Rect(450, 120, 260, 130));
                //dC.DrawImage(new BitmapImage(new Uri(@"\..\..\Bilder\NumisLogo1.jpg")), new Rect(450, 120, 260, 130));
                //Absender
                dC.DrawText(absender, new Point(70, 200));
                ////Addressat
                dC.DrawText(addressat, new Point(70, 220));
                ////Betreff
                dC.DrawText(betreff, new Point(70, 400));
                dC.DrawText(auktion, new Point(70, 415));
                dC.DrawText(kunde, new Point(70, 430));
                ////Rechnungsdatum
                dC.DrawText(rechnungsDatum, new Point(570, 415));

                //hinweis
                dC.DrawText(hinweis, new Point(70, 445));
                ////seite
                dC.DrawText(seitenZahl, new Point(570, 430));
                ////Tabellenkopf
                dC.DrawLine(new Pen(Brushes.Black, 1), new Point(50, 465), new Point(540, 465));
                dC.DrawText(tabellenKopf1, new Point(70, 475));
                dC.DrawText(tabellenKopf2, new Point(170, 475));
                dC.DrawText(tabellenKopf3, new Point(520, 475));
                dC.DrawLine(new Pen(Brushes.Black, 1), new Point(50, 500), new Point(540, 500));
                //TabellenInhalt
                FormattedText inhaltLos, inhaltBez, inhaltPreis, bruttoBetrag, nettoBetrag, steuerBetrag, aufGeldBetrag, mwStPflichtig;
                int zeile = 515;
                int z = 0;
                int zZeiger = 0;
                for (int i = start; i < list.Count && i < (start + maxLose); i++)
                {
                    zZeiger = zeile + (z * 25);
                    inhaltLos = new FormattedText(list[i].LosNummer.ToString(),
                                                    CultureInfo.CurrentUICulture,
                                                    FlowDirection.LeftToRight,
                                                    new Typeface("Arial"), 14, Brushes.Black);
                    inhaltBez = new FormattedText(list[i].Bezeichnung.ToString(),
                                                    CultureInfo.CurrentUICulture,
                                                    FlowDirection.LeftToRight,
                                                    new Typeface("Arial"), 14, Brushes.Black);
                    inhaltPreis = new FormattedText(string.Format("€ {0:#,##0.00}", list[i].ZuschlagPreis),
                                                    CultureInfo.CurrentUICulture,
                                                    FlowDirection.RightToLeft,
                                                    new Typeface("Arial"), 14, Brushes.Black);
                    if (list[i].Mehrwertsteuer == true)
                    {
                        mwStPflichtig = new FormattedText(  "*", 
                                                            CultureInfo.CurrentUICulture,
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Arial"), 14, Brushes.Black);
                        dC.DrawText(mwStPflichtig, new Point(100, zZeiger));
                            
                    }
                    dC.DrawText(inhaltLos, new Point(70, zZeiger));
                    dC.DrawText(inhaltBez, new Point(170, zZeiger));
                    dC.DrawText(inhaltPreis, new Point(520, zZeiger));
                    z = z + 1;


                    // Nach dem Letzten Los soll Gesamtbetrag ausgegeben werden!
                    if (i == list.Count - 1)
                    {
                        nettoBetrag = new FormattedText(string.Format("€ {0:#,##0.00}", nettoPreis),
                                                        CultureInfo.CurrentUICulture,
                                                        FlowDirection.RightToLeft,
                                                        new Typeface("Arial"), 14, Brushes.Black);

                        aufGeldBetrag = new FormattedText(string.Format("€ {0:#,##0.00}", aufgeld),
                                                        CultureInfo.CurrentUICulture,
                                                        FlowDirection.RightToLeft,
                                                        new Typeface("Arial"), 14, Brushes.Black);

                        steuerBetrag = new FormattedText(string.Format("€ {0:#,##0.00}", mehrWertSteuer),
                                                        CultureInfo.CurrentUICulture,
                                                        FlowDirection.RightToLeft,
                                                        new Typeface("Arial"), 14, Brushes.Black);

                        bruttoBetrag = new FormattedText(string.Format("€ {0:#,##0.00}", bruttoPreis),
                                                        CultureInfo.CurrentUICulture,
                                                        FlowDirection.RightToLeft,
                                                        new Typeface("Arial"), 14, Brushes.Black);

                        dC.DrawLine(new Pen(Brushes.Black, 1), new Point(50, zZeiger + 30), new Point(540, zZeiger + 30));
                        dC.DrawText(nettoBetLbl, new Point(170, zZeiger + 50));
                        dC.DrawText(nettoBetrag, new Point(520, zZeiger + 50));

                        dC.DrawText(aufGeldLbl, new Point(170, zZeiger + 70));
                        dC.DrawText(aufGeldBetrag, new Point(520, zZeiger + 70));

                        dC.DrawText(steuerBetLbl, new Point(170, zZeiger + 90));
                        dC.DrawText(steuerBetrag, new Point(520, zZeiger + 90));
                        dC.DrawLine(new Pen(Brushes.Black, 1), new Point(420, zZeiger + 115), new Point(540, zZeiger + 115));

                        dC.DrawText(bruttoBetLbl, new Point(170, zZeiger + 120));
                        dC.DrawText(bruttoBetrag, new Point(520, zZeiger + 120));
                        dC.DrawLine(new Pen(Brushes.Black, 1), new Point(420, zZeiger + 140), new Point(540, zZeiger + 140));
                    }
                }

                //RechteFußzeile
                dC.DrawText(fussZeileRechtsSchriftzug, new Point(600, 725));
                dC.DrawText(fussZeileRechts, new Point(600, 740));
                //Fußzeile
                dC.DrawText(fussZeile, new Point(70, 960));


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
