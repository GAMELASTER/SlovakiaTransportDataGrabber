using HtmlAgilityPack;
/**
 * Slovakian Transport Data Grabber
 * Author: GAMELASTER
 * Version: 1.0
 * About: The Data grabber from unnamed website. This is not made & published couse make someone angry or anything bad, just for research and make it some more opened..
 * License: GNU General Public License v2.0
 * ChangeLog:
 * 1.0: first stable version.
 **/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;

namespace SlovakiaTransportDataGrabber
{
    class Program
    {
        static StreamWriter logWriter = new StreamWriter("log" + Environment.TickCount + ".txt");

        static void writeLog(string value)
        {
            value = "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + value;
            Console.Write(value);
            logWriter.Write(value.Replace("\n", Environment.NewLine));
        }

        static void Main(string[] args)
        {
            logWriter.AutoFlush = true;
            writeLog("Slovakian Transport Data Grabber\nVersion: 1.0\nAuthor: GAMELASTER\n\n");
            writeLog("Please write a name of grabbing website (sample: http://thedatasite.sk/):\n");
            string webUrl = Console.ReadLine();
            writeLog("Okay, " + webUrl + " is maybe valid!\nPlease write a name of data source (without .xml) (file from DATA directory):\n");
            string fileName = Console.ReadLine();
            writeLog("Okay, wait, parsing a XML!\n");
            region data = null;
            try
            {
                using(Stream dataStream = File.OpenRead("./DATA/" + fileName + ".xml"))
                {
                    data = new XmlSerializer(typeof(region)).Deserialize(dataStream) as region;
                }
            }
            catch(Exception ex)
            {
                writeLog("Failed to parse a " + fileName + ".xml! Error: " + ex.Message);
                Console.ReadKey();
                return;
            }
            Console.Clear();
            writeLog("Okay! " + fileName + ".xml is fine! Starting parsing in 10 seconds!\n");
            System.Threading.Thread.Sleep(10000);
            Console.Clear();
            if (Directory.Exists(data.shortcut)) Directory.Delete(data.shortcut);
            Directory.CreateDirectory(data.shortcut);
            foreach(regionRoute route in data.route)
            {
                writeLog("Parsing a Route:" + route.number + "\n");
                parseAlink(webUrl, data.shortcut, route.number, route.type);
            }

            logWriter.Close();
        }

        static WebClient wc = new WebClient();
        
        #region a ugly reworked function

        /*
         * im sorry for this ugly and non commented function, but i do this ALMOST year ago, and i do that really bad!
         * Just now im do that for get a valid XML c:
         * */

        public static void parseAlink(string website, string region, string route, string type)
        {
            //string oblast = args[0];
            //string linka = args[1];
            File.Delete("work.xml");

            //string ikonka = "";

            //if (args.Length > 2) ikonka = "ikonka='" + args[2] + "'";
            //string data = "<linka " + ikonka + " cislo='" + linka + "'>\n";

            linka linkaObj = new linka();
            linkaObj.cislo = route;
            linkaObj.type = type;

            
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.OptionFixNestedTags = true;
            writeLog("Downloading a file: " + website + region.ToLower() + "/cestovny-poriadok/linka/" + route + ".html");
            wc.DownloadFile(website + region.ToLower() + "/cestovny-poriadok/linka/" + route + ".html", "temp.xml");
            htmlDoc.DetectEncodingAndLoad("temp.xml", true);

            IEnumerable<HtmlNode> tabulky = htmlDoc.DocumentNode.Descendants("table").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "tabulka");

            List<linkaSmer> smery = new List<linkaSmer>();

            foreach (HtmlNode zastavkyContainer in tabulky)
            {
                HtmlNode smer = zastavkyContainer.Descendants("td").First();
                if (!smer.InnerHtml.Contains("►")) continue;
                //data += "\t<smer zastavka='" + smer.InnerHtml.Replace("► ", "") + "'>\n";

                linkaSmer smerObj = new linkaSmer();
                smerObj.zastavka = smer.InnerHtml.Replace("► ", "");

                List<linkaSmerPoriadok> poriadky = new List<linkaSmerPoriadok>();

                IEnumerable<HtmlNode> zastavky = zastavkyContainer.Descendants("a");
                foreach (HtmlNode zastavka in zastavky)
                {
                    if (!zastavka.Attributes["href"].Value.Contains("cestovny-poriadok")) continue;
                    if (zastavka.Attributes.Contains("class") && zastavka.Attributes["class"].Value == "button button_mini") continue;

//                    data += "\t\t<poriadok zozastavky='" + zastavka.InnerText + "'>\n";
                    linkaSmerPoriadok poriadokObj = new linkaSmerPoriadok();
                    poriadokObj.zozastavky = zastavka.InnerText;
                    
                    int lel = new Random().Next(50, 100);
                    wc.DownloadFile(website + zastavka.Attributes["href"].Value, "temp" + lel + ".xml");

                    HtmlAgilityPack.HtmlDocument htmlDoc2 = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc2.OptionFixNestedTags = true;
                    htmlDoc2.DetectEncodingAndLoad("temp" + lel + ".xml", true);

                    IEnumerable<HtmlNode> hodiny = htmlDoc2.DocumentNode.Descendants("table").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "cp_odchody_tabulka_max");
                    IEnumerable<HtmlNode> dni = htmlDoc2.DocumentNode.Descendants("td").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "nazov_dna");

                    int denIndex = 0;

                    List<linkaSmerPoriadokDen> dniList = new List<linkaSmerPoriadokDen>();

                    foreach (HtmlNode den in dni)
                    {
//                        data += "\t\t\t<den nazov='" + den.InnerText.Replace("&nbsp;", " ") + "'>\n";
                        linkaSmerPoriadokDen denObj = new linkaSmerPoriadokDen();
                        denObj.nazov = den.InnerText.Replace("&nbsp;", " ");

                        List<linkaSmerPoriadokDenHodina> hodinyList = new List<linkaSmerPoriadokDenHodina>();
                        HtmlNode hodinyEx = hodiny.ToList()[denIndex];

                        foreach (HtmlNode hodina in hodinyEx.Descendants("tr").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "cp_odchody"))
                        {
                            linkaSmerPoriadokDenHodina hodinaObj = new linkaSmerPoriadokDenHodina();
                            List<string> minutyList = new List<string>();

                            int columnIndex = 0;
                            foreach (HtmlNode minuta in hodina.Descendants("td"))
                            {
                                if (columnIndex == 0)
                                {
//                                    data += "\t\t\t\t<hodina ktora='" + minuta.InnerText + "'>";
                                    hodinaObj.ktora = minuta.InnerText;
                                }
                                else
                                {
                                    string add = "";
                                    if (minuta.Attributes.Contains("class") && minuta.Attributes["class"].Value == "cp_odchody_doplnenie") continue;
                                    if (minuta.Attributes.Contains("class") && minuta.Attributes["class"].Value == "nizkopodlazne") add = "nv";
 //                                   data += minuta.InnerText + add + " ";
                                    minutyList.Add(minuta.InnerText + add);
                                }
                                columnIndex++;
                            }
                            hodinaObj.minuta = minutyList.ToArray();
                            hodinyList.Add(hodinaObj);
//                          data += "</hodina>\n";
                        }

                        denObj.hodina = hodinyList.ToArray();
                        dniList.Add(denObj);
//                        data += "\t\t\t</den>\n";
                        denIndex++;
                    }

                    poriadokObj.den = dniList.ToArray();

  //                  data += "\t\t\t<poznamky>\n";

                    HtmlNode poznamky = htmlDoc2.DocumentNode.Descendants("table").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "poznamky" && x.HasChildNodes == true).First().Descendants("td").First();
                    string[] poznamkyEx = poznamky.InnerHtml.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                    string poznamkyObj = "";

                    foreach (string poznamka in poznamkyEx)
                    {
                        if (poznamka.Contains("\"symbol\"")) continue;
                        if (!poznamka.Contains(" - ")) continue;
                        string poznamkaEx = Regex.Replace(poznamka, "<span (.*)>([0-9]|[0-9][0-9])<\\/span>", "nv");
                        poznamkaEx = Regex.Replace(poznamkaEx, "<.*?>", string.Empty);
                        string[] datas = poznamkaEx.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);

                        poznamkyObj += poznamkaEx + Environment.NewLine;
//                        if (datas[0].Length < 5) data += poznamkaEx + "\n";
                    }
                    poriadokObj.poznamky = poznamkyObj;
//                    data += "\t\t\t</poznamky>\n";

                    File.Delete("temp" + lel + ".xml");
//                    data += "\t\t</poriadok>\n";

                    poriadky.Add(poriadokObj);

                }
                smerObj.poriadok = poriadky.ToArray();
//                data += "\t</smer>\n";
                smery.Add(smerObj);
            }

            linkaObj.smer = smery.ToArray();

  //          data += "</linka>";

            //StreamWriter sw = File.CreateText("./" + region + "/" + region + "-" + route + ".xml");
            // XmlSerializer xs = new XmlSerializer(typeof(linka));
            //xs.Serialize(sw, linkaObj);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(linka));
            FileStream fs = File.Create("./" + region + "/" + region + "-" + route + ".json");

            js.WriteObject(fs, linkaObj);
            fs.Close();
            //sw.Write(data);
            //sw.Close();
            

            File.Delete("temp.xml");
        }
        
        #endregion
         
    }


    #region xmldefinition
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class linka
    {

        private linkaSmer[] smerField;

        private string cisloField;
        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("smer")]
        public linkaSmer[] smer
        {
            get
            {
                return this.smerField;
            }
            set
            {
                this.smerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cislo
        {
            get
            {
                return this.cisloField;
            }
            set
            {
                this.cisloField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class linkaSmer
    {

        private linkaSmerPoriadok[] poriadokField;

        private string zastavkaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("poriadok")]
        public linkaSmerPoriadok[] poriadok
        {
            get
            {
                return this.poriadokField;
            }
            set
            {
                this.poriadokField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zastavka
        {
            get
            {
                return this.zastavkaField;
            }
            set
            {
                this.zastavkaField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class linkaSmerPoriadok
    {

        private linkaSmerPoriadokDen[] denField;

        private string poznamkyField;

        private string zozastavkyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("den")]
        public linkaSmerPoriadokDen[] den
        {
            get
            {
                return this.denField;
            }
            set
            {
                this.denField = value;
            }
        }

        /// <remarks/>
        public string poznamky
        {
            get
            {
                return this.poznamkyField;
            }
            set
            {
                this.poznamkyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zozastavky
        {
            get
            {
                return this.zozastavkyField;
            }
            set
            {
                this.zozastavkyField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class linkaSmerPoriadokDen
    {

        private linkaSmerPoriadokDenHodina[] hodinaField;

        private string nazovField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("hodina")]
        public linkaSmerPoriadokDenHodina[] hodina
        {
            get
            {
                return this.hodinaField;
            }
            set
            {
                this.hodinaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nazov
        {
            get
            {
                return this.nazovField;
            }
            set
            {
                this.nazovField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class linkaSmerPoriadokDenHodina
    {

        private string[] minutaField;

        private string ktoraField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("minuta")]
        public string[] minuta
        {
            get
            {
                return this.minutaField;
            }
            set
            {
                this.minutaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ktora
        {
            get
            {
                return this.ktoraField;
            }
            set
            {
                this.ktoraField = value;
            }
        }
    }


    #endregion

    #region a XML definitions



    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class region
    {

        private regionRoute[] routeField;

        private string shortcutField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("route")]
        public regionRoute[] route
        {
            get
            {
                return this.routeField;
            }
            set
            {
                this.routeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string shortcut
        {
            get
            {
                return this.shortcutField;
            }
            set
            {
                this.shortcutField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class regionRoute
    {

        private string numberField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

#endregion
}
