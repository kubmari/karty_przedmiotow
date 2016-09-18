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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Data.Entity;

namespace karty_przedmiotow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
          //  Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
            String path = "../../xml/";
            String[] directories = Directory.GetDirectories(path);
            foreach(string dirList in directories)
            {
      //          Console.WriteLine(dirList);
            }
            if(Directory.Exists(path))
            {
                ProcessDirectory(path);
            }else
            {
    //            Console.WriteLine("Something wrong with path!");
              
            }

        }

        private void ProcessDirectory(string path)
        {
            string[] fileEntries = Directory.GetFiles(path);
            foreach(string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }

        }

        
        private void ProcessFile(string fileName)
        {
            string xmlFile = loadXML(fileName);
            TextBox tB1 = textBox;
            tB1.Text += xmlFile;
            tB1.Text += "\n";
        }



        private string loadXML(string fileName)
        {
            XElement lectureSheet = XElement.Load(fileName);
            string name = lectureSheet.Attribute("name").Value;
            //Polish names to fit tags in XML files..
            XElement szczegoly = lectureSheet.Element("szczegoly");
            XElement tresci_przedmiotu = szczegoly.Element("tresci_przedmiotu");
            IEnumerable<XElement> opis_szczegolowy_item = lectureSheet.Descendants("opis_szczegolowy_item");
          
            using (var db = new KartContext())
            {

                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Typs]");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [KodGlobalnies]");
                //KOD Lokalny:TYP
              //  Console.WriteLine("\n**");
                foreach (XElement current_osi in opis_szczegolowy_item)
                {
                    IEnumerable<XElement> opis_szczegolowy_item_forma_godziny = current_osi.Elements("opis_szczegolowy_item_forma_godziny");
                    IEnumerable<XAttribute> osi_forma = opis_szczegolowy_item_forma_godziny.Attributes("forma");
                    IEnumerable<XElement> opis_szczegolowy_item_efekty_efekt = current_osi.Descendants("opis_szczegolowy_item_efekty_efekt");
                    foreach (XElement kod in opis_szczegolowy_item_efekty_efekt)
                    {

                        foreach (XAttribute forma in osi_forma)
                        {
                            var data = new Typ { kodLokalny = kod.Value.Trim(), typ = forma.Value.Trim() };
                            db.typ.Add(data);
                            db.SaveChanges();

                        }
                    }
                }

                //KOD Lokalny:KOD
                IEnumerable<XElement> efekty = szczegoly.Descendants("efekt");
                foreach (XElement efekt in efekty)
                {
                    string kodL = efekt.Element("efekt_kod").Value.Trim();
                    IEnumerable<XElement> efekty_kierunkowe = efekt.Descendants("efekt_kierunkowy");
                    foreach (XElement efekt_kierunkowy in efekty_kierunkowe)
                    {
                        string kodG = efekt_kierunkowy.Value.Trim();
                        var data = new KodGlobalny { kodGlobalny = kodG , kodLokalny=kodL};
                        db.kodGlobalny.Add(data);
                        db.SaveChanges();
                    }
                }
            }
            string statString = getStatistics(name);
            return statString;
        }

        int cardnumbs = 0;
        string getStatistics(string name)
        {
            cardnumbs++;

            string statMsg = cardnumbs + ": "+ name + "\n";

            var db = new KartContext();
            var query = from b in db.kodGlobalny
                        select b.kodLokalny;
            var uniqueKodGlobalny = query.Distinct();
            foreach(var ukg in uniqueKodGlobalny)
            {

                var countquery = from b in db.kodGlobalny
                                 where b.kodLokalny == ukg
                                 select new { b.kodLokalny, b.kodGlobalny };
                statMsg += ukg + " ilosc efektow: "+ countquery.Count();

                var countamount = from b in db.typ
                                  where b.kodLokalny == ukg
                                  select new { b.kodLokalny, b.typ};

                statMsg += "\t \t \tPowtorzen razy: " + countamount.Count();
                foreach(var efekt in countquery)
                {
                    statMsg += "\t \t" +efekt.kodGlobalny;

                }
                statMsg += "\t \t \t";
                var uniqueCountAmount = countamount.Distinct();
                foreach (var selectedTyp in uniqueCountAmount)
                {
                    statMsg += selectedTyp.typ+", ";

                }
                statMsg += "\n";
                
            }

            return statMsg;
        }
    }
}
