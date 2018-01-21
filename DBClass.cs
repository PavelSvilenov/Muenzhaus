using Muenzhaus2;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muenzhaus2
{
    class DBClass
    {
        // private const string conn_string = @"Data Source=PSVILENOV-PC\SQLEXPRESS;Initial Catalog=ProjectDB;Integrated Security=True";
        //private const string conn_string = @"Data Source=ita-server;Initial Catalog=Muenzhaus2;Persist Security Info=True;User ID=muenzhaus2;Password=muenzhaus2";
        private const string conn_string = @"Data Source=PSVILENOV-PC\SQLEXPRESS;Initial Catalog=Muenzhaus2;Integrated Security=True";



        /// <summary>
        /// Funtkion die alle Lose zurückliefert   
        /// </summary>
        /// <param name="auktionsNr"> Akutonsnummer</param>
        /// <returns>Alle Lose als List</returns>
        public static List<Lose> AlleLose(int auktionsNr)
        {

            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);

            var alleLose = from list in auktionDBDataContext.Lose
                           where list.AuktionsNummer == auktionsNr
                           select list;

            List<Lose> loseList = new List<Lose>();

            foreach (var item in alleLose)
            {
                loseList.Add(item);
            }

            return loseList;

        }
        /// <summary>
        /// Alle Lose die noch nicht versteigert sind
        /// </summary>
        /// <param name="auktionsNr">AuktionsNummer</param>
        /// <returns>Alle Lose als List</returns>
        public static List<Lose> AlleNichtVersteigertenLose(int auktionsNr)
        {
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);

            var alleLose = from list in auktionDBDataContext.Lose
                           where list.AuktionsNummer == auktionsNr
                           && list.Kaeufer == null
                           select list;

            List<Lose> loseList = new List<Lose>();

            foreach (var item in alleLose)
            {
                loseList.Add(item);
            }

            return loseList;

        }

        /// <summary>
        /// Fügt neue zeile mit den übergebenen Gebote
        /// </summary>
        /// <param name="l">List mit Gebote</param>
        /// <param name="kundenNr">Kundennummer</param>
        /// <param name="bemerkung">Bemerkung zur Zeile</param>
        public static void AddGebotZeile(List<Gebot> l, int kundenNr, string bemerkung)
        {
            if (l != null)
            {
                LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);
                int auktionsNr = l.First().AuktionsNummer;
                //int kundenNr = l.First().Zeile.Kundennummer;

                if (!SchriftlichesGebotVorhanden(kundenNr, auktionsNr))
                {
                    AddSchriftlichesGebot(kundenNr, auktionsNr);
                }

                EntitySet<Gebot> list = new EntitySet<Gebot>();
                list.AddRange(l);

                auktionDBDataContext.Zeile.InsertOnSubmit(new Zeile { Gebot = list, Kundennummer = kundenNr, AuktionsNummer = auktionsNr, Bemerkung = bemerkung });
                auktionDBDataContext.SubmitChanges();
            }
        }
        /// <summary>
        /// Setzt ein Zuschlag für bestimmte Los
        /// </summary>
        /// <param name="losNr">Losnummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <param name="kundenNr">Kundennummer</param>
        /// <param name="kaufpreis">Kaufpreis</param>
        public static void ZuschlagSetzen(int losNr, int auktionsNr, int kundenNr, int kaufpreis)
        {
            LinqDBDataContext dbContext = new LinqDBDataContext(conn_string);

            Lose los = (from lose in dbContext.Lose
                        where lose.LosNummer == losNr
                        && lose.AuktionsNummer == auktionsNr
                        select lose).FirstOrDefault();
            los.Kaeufer = kundenNr;
            los.ZuschlagPreis = kaufpreis;


            var gebZeileId = from zeile in dbContext.Zeile
                             where zeile.Kundennummer == kundenNr
                             && zeile.AuktionsNummer == auktionsNr
                             select zeile;

            int zeileId = (from geb in dbContext.Gebot
                           where geb.LosId == losNr
                           && geb.Zeile.Kundennummer == kundenNr
                           && geb.AuktionsNummer == auktionsNr
                           select geb.ZeileId).FirstOrDefault();

            var gebotList = from gebList in dbContext.Gebot
                            where gebList.ZeileId == zeileId
                            && gebList.AuktionsNummer == auktionsNr
                            select gebList;

            foreach (var item in gebotList)
            {
                if (item.LosId == losNr && item.AuktionsNummer == los.AuktionsNummer)
                {
                    item.Zuschlag = true;
                }
                else
                {
                    item.Zuschlag = false;
                }

            }
            dbContext.SubmitChanges();
        }
        /// <summary>
        /// Setzt für Lose ein Kundennummer -1 d.h Keine Zuschlag 
        /// </summary>
        /// <param name="losNr">Losnummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        public static void KeinZuschlag(int losNr,int auktionsNr)
        {
            LinqDBDataContext dbContext = new LinqDBDataContext(conn_string);

            Lose los = (from lose in dbContext.Lose
                        where lose.LosNummer == losNr
                        && lose.AuktionsNummer == auktionsNr
                        select lose).FirstOrDefault();
            los.Kaeufer = -1;
            los.ZuschlagPreis = 0;
            dbContext.SubmitChanges();
        }
        /// <summary>
        /// Los von Auktion zurückziehen
        /// </summary>
        /// <param name="losNr">Losnummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <param name="grund">Grund</param>
        public static void LosZurueckziehen(int losNr, int auktionsNr, string grund)
        {
            LinqDBDataContext dbContext = new LinqDBDataContext(conn_string);
            Lose los = (from lose in dbContext.Lose
                        where lose.LosNummer == losNr
                        && lose.AuktionsNummer == auktionsNr
                        select lose).FirstOrDefault();

            los.Valid = false;
            los.Grund = grund;

            dbContext.SubmitChanges();

        }
        /// <summary>
        /// Berechnet das Mindestgebot für ein Los
        /// </summary>
        /// <param name="schaetzPreis">Übergebener Schätzpreis</param>
        /// <returns>Liefert Mindestgebot als Int-Wert</returns>
        public static int BerechneMindestGebot(int schaetzPreis)
        {
            int satz = 80;
            int mindestGebot = schaetzPreis * satz / 100;
            return mindestGebot;
        }

        /// <summary>
        /// legt ein neues schriftliches Gebot in der Datenbank an
        /// </summary>
        /// <param name="kundenNr"> Kunden nummer</param>
        /// <param name="auktionsNr">Nummer der Auktion</param>
        /// <returns>Zeile Id</returns>
        public static Schriftliches_Gebot AddSchriftlichesGebot(int kundenNr, int auktionsNr)
        {
            Schriftliches_Gebot z = new Schriftliches_Gebot { Kundennummer = kundenNr, AuktionsNummer = auktionsNr };
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);
            auktionDBDataContext.Schriftliches_Gebot.InsertOnSubmit(z);
            auktionDBDataContext.SubmitChanges();
            return z;
        }
        /// <summary>
        /// Fügt neues schriftliches Gebot mit Gesamtbetrag in die Datenbank ein.
        /// </summary>
        /// <param name="kundenNr">Kunden nummer</param>
        /// <param name="auktionsNr">Nummer der Auktion</param>
        /// <param name="gesamtBetrag">Gesamtbetrag aller gekauften Auktionen</param>
        /// <returns>Zeile Id</returns>
        public static Schriftliches_Gebot AddSchriftlichesGebot(int kundenNr, int auktionsNr, int gesamtBetrag)
        {
            Schriftliches_Gebot z = new Schriftliches_Gebot { Kundennummer = kundenNr, AuktionsNummer = auktionsNr, Gesamtbetrag = gesamtBetrag };
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);
            auktionDBDataContext.Schriftliches_Gebot.InsertOnSubmit(z);
            auktionDBDataContext.SubmitChanges();
            return z;
        }
        /// <summary>
        /// Prüft ob eine Los mit der übergebenen ID bereits in der Datenbank angelegt wurde
        /// </summary>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <param name="kundenNr">Kundennummer</param>
        /// <returns>true falls vorhanden, false falls nicht vorhanden</returns>
        public static bool SchriftlichesGebotVorhanden(int kundenNr, int auktionsNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            bool vorhanden = (from b in db.Schriftliches_Gebot
                              where b.Kundennummer == kundenNr && b.AuktionsNummer == auktionsNr
                              select b).Any();
            return vorhanden;
        }
        /// <summary>
        /// Prüft ob ein Gebot in der Datenbank vorhanden ist
        /// </summary>
        /// <param name="auktionsNr">Nummer der Auktion</param>
        /// <returns>liefer wahr wenn das Gebot vorhanden ist und false wenn es das nicht ist.</returns>
        public static bool GeboteVorhanden(int auktionsNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            bool vorhanden = (from g in db.Gebot
                              where g.AuktionsNummer == auktionsNr
                              select g).Any();
            return vorhanden;
        }
        /// <summary>
        /// Gibt eine Liste mit allen Zeilen eines Kunden zurück
        /// </summary>
        /// <param name="kundenNr">Kundennummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns> Liste vom Typ Zeile mit allen Zeilen des Kunden</returns>
        public static List<Zeile> GetAlleKundeZeile(int kundenNr, int auktionsNr)
        {
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);
            List<Zeile> zeileList = new List<Zeile>();
            var list = (from z in auktionDBDataContext.Zeile
                        where z.Kundennummer == kundenNr && z.AuktionsNummer == auktionsNr
                        select z).Distinct();
            zeileList = list.ToList();

            return zeileList;
        }
        /// <summary>
        /// Löscht ein Gebot eines Kunden
        /// </summary>
        /// <param name="iD">ID des Gebots</param>
        /// <returns>liefert wahr wenn löschen erfolgreich und false wenn das löschen fehlgeschlagen ist</returns>
        public static bool KundenGebotLoeschen(int iD)
        {
            bool ok = false;
            try
            {

                LinqDBDataContext dbc = new LinqDBDataContext(conn_string);
                Gebot g = (from geb in dbc.Gebot
                           where geb.Id == iD
                           select geb).First();
                dbc.Gebot.DeleteOnSubmit(g);
                dbc.SubmitChanges();
                ok = true;
            }
            catch (SqlException ex)
            {
                if(ex != null)
                    ok = false;
            }


            return ok;

        }
        /// <summary>
        /// Löscht eine Zeile der schriftlichen Gebote
        /// </summary>
        /// <param name="iD">ID der Zeile welche zu löschen ist</param>
        /// <returns>liefert false wenn das löschen fehlschlägt und true wenn der Datensatz gelöscht werden konnte</returns>
        public static bool LoescheZeile(int iD)
        {
            bool ok = false;
            try
            {
                LinqDBDataContext dbc = new LinqDBDataContext(conn_string);
                Zeile z = (from ze in dbc.Zeile
                           where ze.Id == iD
                           select ze).First();
                dbc.Zeile.DeleteOnSubmit(z);
                dbc.SubmitChanges();
                ok = true;
            }
            catch (SqlException ex)
            {
                if (ex != null)
                    ok = false;
            }

            return ok;
        }
        /// <summary>
        /// Liefert eine Zeile der schriftlichen Gebote
        /// </summary>
        /// <param name="zeileId">Zeilen ID</param>
        /// <returns>Liefert die Zeile als Zeile-Objekt</returns>
        public static Zeile GetZeile(int zeileId)
        {
            LinqDBDataContext dbc = new LinqDBDataContext(conn_string);
            Zeile z = (from ze in dbc.Zeile
                      where ze.Id == zeileId
                      select ze).FirstOrDefault();
            return z;

        }
        /// <summary>
        /// Aktualisiert eine Zeile der schriftlichen Gebote
        /// </summary>
        /// <param name="zeileId">Eindeutige Id der Zeile welche aktualisiert werden soll</param>
        /// <param name="bemerkung">Bemerkung des Kunden zu der Zeile</param>
        public static void AktualisiereZeile(int zeileId, string bemerkung)
        {
            LinqDBDataContext dbc = new LinqDBDataContext(conn_string);
            Zeile z = (from ze in dbc.Zeile
                       where ze.Id == zeileId
                       select ze).FirstOrDefault();
            z.Bemerkung = bemerkung;
            dbc.SubmitChanges();
        }
        /// <summary>
        /// Setzt ein maximales Budget für einen Kunden dessen KundenNr übergeben wurde.
        /// </summary>
        /// <param name="kundenNr">Kundennummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <param name="budget">Maximales budget</param>
        public static void SetBudget(int kundenNr, int auktionsNr, int budget)
        {
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);

            Schriftliches_Gebot schriftlichesGebot = (from geb in auktionDBDataContext.Schriftliches_Gebot
                                                      where geb.Kundennummer == kundenNr && geb.AuktionsNummer == auktionsNr
                                                      select geb).FirstOrDefault();

            schriftlichesGebot.Gesamtbetrag = budget;
            auktionDBDataContext.SubmitChanges();
        }
        /// <summary>
        /// Liefert das maximale Budget des Kunden dessen KundenNr übergeben wurde.
        /// </summary>
        /// <param name="kundenNr">Nummer des Kunden</param>
        /// <param name="auktionsNr">Nummer der Auktion</param>
        /// <returns>Liefert das Budget als Int-Wert und 0 falls keines gesetzt wurde</returns>
        public static int GetBudget(int kundenNr, int auktionsNr)
        {
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);
            int erg = 0;
            Schriftliches_Gebot schriftlichesGebot = (from geb in auktionDBDataContext.Schriftliches_Gebot
                                                      where geb.Kundennummer == kundenNr && geb.AuktionsNummer == auktionsNr
                                                      select geb).FirstOrDefault();
            if (schriftlichesGebot != null)
            {
                Int32.TryParse(schriftlichesGebot.Gesamtbetrag.ToString(), out erg);
            }
            return erg;

        }
        /// <summary>
        /// Gibt den Aktuelle Preis eines Loses zurück
        /// </summary>
        /// <param name="losNr">Losnummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Den aktuellen Preis oder -1 falls Losnummer nicht gefunden bzw. 1 falls nur ein Gebot für Lose Gibt</returns>
        public static int GetAktuellPreis(int losNr, int auktionsNr)
        {
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);
            int preis = -1;

            var gebotList = auktionDBDataContext.Gebot
                            .Where(x => x.LosId == losNr)
                            .Where(x => x.Lose.AuktionsNummer == auktionsNr)
                            .Where(x => x.Zuschlag == null)
                            .OrderByDescending((x) =>
                                                    x.HoechstsGebot
                                               )
                            .ThenBy(x => x.Id).ToList();


            if (gebotList.Count > 1)
            {


                int rest;
                foreach (var item in gebotList)
                {
                    rest = GetKundeRestBetrag(item.Zeile.Kundennummer, auktionsNr);
                    if (rest != 0 && item.HoechstsGebot > rest)
                    {
                        item.HoechstsGebot = rest;
                    }
                }

                gebotList = gebotList
                              .OrderByDescending((x) => x.HoechstsGebot)
                              .ThenBy((x) => x.Id).ToList();


                Gebot maxGebot = gebotList.FirstOrDefault();

                Gebot aktuellGebot = gebotList.Skip(1).Take(1).FirstOrDefault();


                aktuellGebot.HoechstsGebot++;
                preis = aktuellGebot.HoechstsGebot;


            }
            else if (gebotList.Count() == 1)
            {
                preis = 1;
            }

            return preis;


        }
        /// <summary>
        /// Gibt die in einer bestimmten Auktion ersteigerten Lose des übergebenen Kunden aus
        /// </summary>
        /// <param name="kundenNr">Eindeutige Nummer des Kunden</param>
        /// <param name="auktionNr">Eindeutige Nummer der Auktion</param>
        /// <returns>Liste mit allen Losen die der Kunde in der Auktion ersteigert hat</returns>
        public static List<Lose> KundeGekaufteLose(int kundenNr, int auktionNr)
        {
            LinqDBDataContext md = new LinqDBDataContext();
            var treffer = from z in md.Lose
                           where z.Kaeufer == kundenNr && z.AuktionsNummer == auktionNr
                           select z;
            return treffer.ToList();
        }
        /// <summary>
        /// Gibt das höchste Schriftliche Gebot für eine bestimmte Losnummer zurück, mit einberechnung des Max Budgets
        /// </summary>
        /// <param name="losNr">Losnummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Höchstes Schriftliches Gebot vom Objekttyp Gebot</returns>
        public static Gebot GetMaxGebot(int losNr, int auktionsNr)
        {
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);

            var gebotList = auktionDBDataContext.Gebot
                            .Where(x => x.LosId == losNr)
                            .Where(x => x.Lose.AuktionsNummer == auktionsNr)
                            .Where(x => x.Zuschlag == null)
                            .OrderByDescending((x) =>
                                                    x.HoechstsGebot
                                                    )
                            .ThenBy(x => x.Id).ToList();


            int rest = 0;
            if (gebotList.Count > 1)
            {
                foreach (var item in gebotList)
                {
                    rest = GetKundeRestBetrag(item.Zeile.Kundennummer, auktionsNr);
                    if (rest != 0 && item.HoechstsGebot > rest)
                    {
                        item.HoechstsGebot = rest;
                    }
                }

                gebotList = gebotList
                              .OrderByDescending((x) => x.HoechstsGebot)
                              .ThenBy((x) => x.Id).ToList();


            }
            else
            {
                if(gebotList.FirstOrDefault() != null)
                {
                    rest = GetKundeRestBetrag(gebotList.FirstOrDefault().Zeile.Kundennummer, auktionsNr);
                    if (rest != 0 && gebotList.FirstOrDefault().HoechstsGebot > rest)
                    {
                        gebotList.FirstOrDefault().HoechstsGebot = rest;
                    }
                }
        
            }

            Gebot maxGebot = gebotList.FirstOrDefault();


            return maxGebot;

        }
        /// <summary>
        /// Gibt das höchste Schriftliche Gebot für eine bestimmte Losnummer zurück
        /// </summary>
        /// <param name="losNr">Losnummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Höchstes Schriftliches Gebot vom Objekttyp Gebot</returns>
        public static Gebot GetSchriftMaxGebot(int losNr, int auktionsNr)
        {
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);

            var gebotList = auktionDBDataContext.Gebot
                            .Where(x => x.LosId == losNr)
                            .Where(x => x.Lose.AuktionsNummer == auktionsNr)
                            .Where(x => x.Zuschlag == null)
                            .AsEnumerable()
                            .OrderByDescending((x) =>
                                                    x.HoechstsGebot
                                                    )
                            .ThenBy(x => x.Id);

            Gebot gebot = gebotList.FirstOrDefault();

            return gebot;
        }



        /// <summary>
        /// Berechnet den Restbetrag eines Kunde auf Basis seines maximalen Budgets
        /// </summary>
        /// <param name="kundenNr">Kundennummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Liefert den Restbetrag als Int Wert</returns>
        public static int GetKundeRestBetrag(int kundenNr, int auktionsNr)
        {
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);
            int zuschlagSumme = 0;
            int rest = 0;
            int gesamt = 0;
            var treffer = (from g in auktionDBDataContext.Lose
                           where g.Kaeufer == kundenNr
                           && g.AuktionsNummer == auktionsNr
                           select g.ZuschlagPreis).Sum();
            if(treffer != null)
            {
                zuschlagSumme = Convert.ToInt32(treffer);
            }
       

            var treffer2 = (from s in auktionDBDataContext.Schriftliches_Gebot
                            where s.Kundennummer == kundenNr && s.AuktionsNummer == auktionsNr
                            select s).FirstOrDefault().Gesamtbetrag;
            if (treffer2 != null)
            {
                gesamt = Convert.ToInt32(treffer2);
                rest = gesamt - zuschlagSumme;
            }
            return rest;
        }
        /// <summary>
        /// Gibt den mindestpreis eines Loses zurück
        /// </summary>
        /// <param name="losId">Losnummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Liefert den Mindestpreis für das Los</returns>
        public static int GetLosMindestPreis(int losId, int auktionsNr)
        {
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);
            int preis = (from los in auktionDBDataContext.Lose
                         where los.LosNummer == losId && los.AuktionsNummer == auktionsNr
                         select los.MindestGebot).FirstOrDefault();

            return preis;

        }
        /// <summary>
        /// Überprüft ob eine Kunde schon Gebot mit entsprechender Losnummer gemacht hat
        /// </summary>
        /// <param name="kundenNr">Kundennummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <param name="losNummer">zu suchende Losnummer</param>
        /// <returns>true falls Gebot mit diese Losnummer existiert, und false sonst</returns>
        public static bool KundenGebotVorhanden(int kundenNr, int auktionsNr, int losNummer)
        {
            LinqDBDataContext db = new LinqDBDataContext(conn_string);
            bool vorhanden = (from gebote in db.Gebot
                              where gebote.Zeile.Kundennummer == kundenNr
                              && gebote.LosId == losNummer
                              && gebote.AuktionsNummer == auktionsNr
                              select gebote).Any();
            return vorhanden;

        }

        public static bool KundenGebotVorhandenAusnahme(int kundenNr, int auktionsNr, int losNummer, int gebotID, int zeileID)
        {
            LinqDBDataContext db = new LinqDBDataContext(conn_string);
            bool vorhanden = (from gebote in db.Gebot
                              where gebote.Zeile.Kundennummer == kundenNr
                              && gebote.LosId == losNummer
                              && gebote.AuktionsNummer == auktionsNr
                              && gebote.ZeileId == zeileID
                              && gebote.Id != gebotID
                              select gebote).Any();

            return vorhanden;

        }

        /// <summary>
        /// Prüft ob eine Gebot für ein Los in einer anderen Zeile vorhanden ist
        /// </summary>
        /// <param name="kundenNr">Kundennummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <param name="losNummer">Losnummer</param>
        /// <param name="zeileId">Zeilennummer</param>
        /// <returns>liefert wahr wenn das Gebot in einer Anderen Zeile vorhanden ist.</returns>
        public static bool KundenGebotInAndererZeileVorhanden(int kundenNr, int auktionsNr, int losNummer, int zeileId)
        {
            LinqDBDataContext db = new LinqDBDataContext(conn_string);
            bool vorhanden = (from gebote in db.Gebot
                              where gebote.Zeile.Kundennummer == kundenNr
                              && gebote.LosId == losNummer
                              && gebote.AuktionsNummer == auktionsNr
                              && gebote.ZeileId != zeileId
                              select gebote).Any();
            return vorhanden;
        }
        /// <summary>
        /// Funktion zum Anlegen eines neuen Gebotes
        /// </summary>
        /// <param name="g">Das neue Gebot-Objekt das in die Datenbank übernommen werden soll</param>
        /// <returns>Liefert die Id des neu angelegt Gebotes</returns>
        public static int KundenGebotAnlegen(Gebot g)
        {
            LinqDBDataContext db = new LinqDBDataContext(conn_string);
            db.Gebot.InsertOnSubmit(g);
            db.SubmitChanges();

            return g.Id;

        }
        /// <summary>
        /// Aktualisiert einen Datensatz in der Gebote Tabelle der Datenbank
        /// </summary>
        /// <param name="iD">ID des Gebotes</param>
        /// <param name="losID">Nummer des Loses auf welches Geboten wird.</param>
        /// <param name="hoechstGebot">Das angegebene Höchstgebot für das betreffende Los</param>
        public static void KundenGebotAktualisieren(int iD, int losID, int hoechstGebot)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            Gebot g = (from geb in db.Gebot
                        where geb.Id == iD
                        select geb).First();
            g.LosId = losID;
            g.HoechstsGebot = hoechstGebot;
            db.SubmitChanges();
        }

        /// <summary>
        /// Gibt ein gebot auf bestimte zeile for ein Losnummer
        /// </summary>
        /// <param name="zeileID">Id der Zeile</param>
        /// <param name="auktionsNr">AuktionsNr</param>
        /// <param name="losId">Losnummer</param>
        /// <returns></returns>
        public static Gebot GetKundenGebot(int zeileID, int auktionsNr, int losId)
        {
            Gebot g;
            LinqDBDataContext db = new LinqDBDataContext();
            g = (from geb in db.Gebot
                where geb.AuktionsNummer == auktionsNr &&
                        geb.ZeileId == zeileID &&
                        geb.LosId == losId
                select geb).FirstOrDefault();
            return g;
        }
        /// <summary>
        /// Gibt List mit alle Auktionen zurück
        /// </summary>
        /// <returns>Alle Auktionen als List</returns>
        public static List<int> GetAlleAuktionenBeended()
        {

            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);

            List<int> auktionenList = new List<int>();


            var auktionen = GetAlleAuktionen();
            foreach (var item in auktionen)
            {
                if (AuktionBeendet(item))
                {
                    auktionenList.Add(item);
                    //throw new ArgumentException("dfsf");
                }

            }

            return auktionenList;
        }
        /// <summary>
        /// Gibt List mit alle Auktionen zurück
        /// </summary>
        /// <returns>Alle Auktionen als List</returns>
        public static List<int> GetAlleAuktionen()
        {

            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);

            List<int> auktionenList = new List<int>();


            var auktionen = (from l in auktionDBDataContext.Lose
                            select l.AuktionsNummer).Distinct();

            foreach (var item in auktionen)
            {
                auktionenList.Add(item);

            }

            return auktionenList;
        }
        /// <summary>
        /// Prüft ob eine Auktion mit der übergebenen ID bereits in der Datenbank angelegt wurde
        /// </summary>
        /// <param name="AuktionsID">Auktionsnummer</param>
        /// <returns>true falls vorhanden, false falls nicht vorhanden</returns>
        public static bool AuktionVorhanden(int AuktionsID)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            bool vorhanden = (from b in db.Auktion
                              where b.AuktionsNummer == AuktionsID
                              select b).Any();
            return vorhanden;
        }
        /// <summary>
        /// Legt eine Neue Auktion in der Datenbank an
        /// </summary>
        /// <param name="a">Neu anzulegende Auktion vom Objekttyp Auktion</param>
        /// <returns>Liefert die Auktionsnummer der gerade angelegten Auktion zurück</returns>
        public static int AuktionAnlegen(Auktion a)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            db.Auktion.InsertOnSubmit(a);
            db.SubmitChanges();
            return a.AuktionsNummer;
        }
        /// <summary>
        /// Prüft ob eine Los mit der übergebenen ID bereits in der Datenbank angelegt wurde
        /// </summary>
        /// <param name="l">Lose</param>
        /// <returns>true falls vorhanden, false falls nicht vorhanden</returns>
        public static bool LosVorhanden(Lose l)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            bool vorhanden = (from b in db.Lose
                              where b.LosNummer == l.LosNummer && b.AuktionsNummer == l.AuktionsNummer
                              select b).Any();
            return vorhanden;
        }
        /// <summary>
        /// Überprüft ob ein Los zurückgezogen ist oder nicht
        /// </summary>
        /// <param name="losNr">Losnummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>true falls nicht zürückgezogen</returns>
        public static bool LosValid(int losNr, int auktionsNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            bool aktiv = (from l in db.Lose
                          where l.LosNummer == losNr
                          && l.AuktionsNummer == auktionsNr
                          && l.Valid == true                   
                          select l).Any();
            return aktiv;
        }
        /// <summary>
        /// Gibt den Gründ warum ein Los zurückgezogen ist
        /// </summary>
        /// <param name="losNr">Losnummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>Grund als string</returns>
        public static string LosInvalidGrund(int losNr, int auktionsNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            string grund = (from l in db.Lose
                            where l.LosNummer == losNr
                            && l.AuktionsNummer == auktionsNr
                            && l.Valid == false
                            select l.Grund).FirstOrDefault();
            return grund;
        }
        /// <summary>
        /// Gibt die Möglichkeit ein Los wieder zur Auktion zuzulassen
        /// </summary>
        /// <param name="losNr">Losnummer</param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        public static void LosZulassen(int losNr,int auktionsNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            Lose lose = (from l in db.Lose
                      where l.LosNummer == losNr
                      && l.AuktionsNummer == auktionsNr
                      && l.Valid == false
                      select l).FirstOrDefault();
            lose.Valid = true;
            lose.Grund = null;
            db.SubmitChanges();
        }
        /// <summary>
        /// Prüft ob eine Los mit der übergebenen Losnummer und Auktionsnummer bereits in der Datenbank angelegt wurde
        /// </summary>
        /// <param name="losNr">LosNummer </param>
        /// <param name="auktionsNr">Auktionsnummer</param>
        /// <returns>true falls vorhanden, false falls nicht vorhanden</returns>
        public static bool LosVorhanden(int losNr, int auktionsNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            bool vorhanden = (from b in db.Lose
                              where b.LosNummer == losNr && b.AuktionsNummer == auktionsNr
                              select b).Any();
            return vorhanden;
        }
        /// <summary>
        /// Prüft ob eine Kunde mit der übergebenen Kundennummer bereits in der Datenbank angelegt wurde
        /// </summary>
        /// <param name="kundenNr">Kundennummer</param>
        /// <returns>true falls vorhanden, false falls nicht vorhanden</returns>
        public static bool KundeVorhanden(int kundenNr)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            bool vorhanden = (from b in db.Kunden
                              where b.Kundennummer == kundenNr
                              select b).Any();
            return vorhanden;
        }
        /// <summary>
        /// Legt ein neues Los in der Datenbank an, sollte die dazugehörige Auktionsnummer 
        /// nicht in der Datenbank zu finden sein wird diese ebenfalls neu angelegt
        /// </summary>
        /// <param name="l">Neu anzulegendes Los</param>
        /// <returns>Id auf das angelegtes Los</returns>
        public static int LosAnlegen(Lose l)
        {
            LinqDBDataContext db = new LinqDBDataContext();
            int iD = -1;

            if (!AuktionVorhanden(l.AuktionsNummer))
            {
                //db.Auktion.InsertOnSubmit(new Auktion { AuktionsNummer = l.AuktionsNummer });
                //db.SubmitChanges();
                throw new ArgumentException("Auktion nicht vorhanden!");
            }

            db.Lose.InsertOnSubmit(l);
            db.SubmitChanges();
            iD = l.LosNummer;

            return iD;
        }
        /// <summary>
        /// Gibt Liste mit allen Kunden der ausgewählten Auktion zurück
        /// </summary>
        /// <param name="auktionsNr">Nr der Auktion</param>
        /// <returns>Alle Auktionen als List</returns>
        public static List<int> GetAlleKaeufer(int auktionsNr)
        {
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);

            List<int> kundenList = new List<int>();

            var kunden = (from k in auktionDBDataContext.Kunden
                          join l in auktionDBDataContext.Lose
                          on k.Kundennummer equals l.Kaeufer
                          where l.AuktionsNummer == auktionsNr
                          select k.Kundennummer).Distinct();

            foreach (var item in kunden)
            {
                if(item > 0)
                    kundenList.Add(item);

            }
            return kundenList;
        }
        /// <summary>
        /// Funtkion die alle Lose zurückliefert   
        /// </summary>
        /// <param name="auktionsNr"> Akutonsnummer</param>
        /// <returns>Alle Lose als List</returns>
        public static List<LosDummy> AlleLoseMitGebot(int auktionsNr)
        {

            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);

            var alleLose = from l in auktionDBDataContext.Lose
                           where l.AuktionsNummer == auktionsNr
                           select new LosDummy
                           {
                               LosNummer = l.LosNummer,
                               Bezeichnung = l.Bezeichnung,
                               Anmerkung = l.Anmerkung,
                               Schaetzpreis = l.MindestGebot,
                               Kaeufer = l.Kaeufer,
                               ZuschlagPreis = l.ZuschlagPreis
                           };

            List<LosDummy> loseList = new List<LosDummy>();
            foreach (LosDummy ld in alleLose)
            {
                ld.HoechstGebot = DBClass.GetSchriftMaxGebot(ld.LosNummer, auktionsNr) != null ? DBClass.GetSchriftMaxGebot(ld.LosNummer, auktionsNr).HoechstsGebot : 0;
                loseList.Add(ld);
            }

            return loseList;

        }
        /// <summary>
        /// Überprüft ob eine Auktion beended ist
        /// </summary>
        /// <param name="auktionsNummer">auktionsNummer</param>
        /// <returns>True falls beended, false sonst</returns>
        public static bool AuktionBeendet(int auktionsNummer)
        {
            bool beendet = false;
            LinqDBDataContext auktionDBDataContext = new LinqDBDataContext(conn_string);

            var treffer = from l in auktionDBDataContext.Lose
                          where l.AuktionsNummer == auktionsNummer && l.Kaeufer == null && l.Valid == true
                          select l.LosNummer;
            if (treffer.Count() == 0)
                beendet = true;

            return beendet;


        }
    }
}
