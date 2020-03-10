using HtmlAgilityPack;
using Nancy.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Wba.Crawler.Domain
{
    public class CustomCrawler
    {

        public long TimeOut { get; set; }

        public string Url { get; set; }

        public string Dest { get; set; }

        public CustomCrawler()
        {
            Url = "https://dota2.gamepedia.com/";
            Dest = @"D:/programmeren/webbackend/Wba.Crawler/Wba.Crawler/wwwroot/files/";
        }

        public void MakeHeroFiles(IEnumerable<string> heronames)
        {
           // using (WebClient client = new WebClient())
            //{
                foreach (string hero in heronames)
                {
                    File.Create(Dest + hero + ".html");
                }
            //}
        }

        public void DownloadHero(IEnumerable<string> heronames)
        {
            using (WebClient client = new WebClient())
            {
                foreach (string hero in heronames)
                {
                    string theUrl = Url + hero.Replace("_", "").Replace("'", "%27");
                    string thePath = Dest + hero + ".html";
                    client.DownloadFile(theUrl, thePath);
                }
            }
        }

       

        public void DownloadHeroes() 
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(Url+"Heroes", Dest+"_heroes.html");
            }
        }

        public Dictionary<string,string> GetAllLinks()
        {
            var text = File.ReadAllText(Dest + "_heroes.html");
            var links = KeepLinks(text);
            var heroLinks = KeepHeroLinks(links);
            var heroImages =  DownloadImages(heroLinks); //todo rename function
            var heroNames = GetHeroNames(heroLinks);
            var dictionaryHeroNames = MatchHeroesWithTheirImages(heroImages, heroNames);
            //CreateHeroImages(dictionaryHeroNames);
            //DownloadHeroImages(dictionaryHeroNames);
            return dictionaryHeroNames;

        }

        public void CreateHeroImages(Dictionary<string, string> dictionaryHeroNames)
        {
            //using (WebClient client = new WebClient())
            //{
                foreach (var key in dictionaryHeroNames.Keys)
                {
                    File.Create(Dest+"images/"+ key +".png");
                }

            //}
        }

        public void DownloadHeroImages(Dictionary<string, string> dictionaryHeroNames)
        {
            using (WebClient client = new WebClient())
            {
                foreach (var key in dictionaryHeroNames.Keys)
                {
                    client.DownloadFile(new Uri(dictionaryHeroNames[key]), Dest+"images/"+ key +".png");
                }

            }
        }

        private Dictionary<string, string> MatchHeroesWithTheirImages(List<string> heroImages, List<string> heroNames)
        {
            Dictionary<string, string> heroNamesandImages = new Dictionary<string, string>();
            string[] heroImagesArray = heroImages.ToArray();
            heroNames.Sort();
            for (int i = 0; i < heroNames.Count(); i++) //119
            {
                var results = Array.FindAll(heroImagesArray, s => s.Contains(heroNames.ElementAt(i).Replace(" ","_").Replace("'", "%27")));
                heroNamesandImages.Add(heroNames.ElementAt(i), results[0]);
            }
            return heroNamesandImages;

        }

        private List<string> DownloadImages(List<HtmlNode> heroLinks)
        {
            List<string> heroimages = new List<string>();
            foreach (var image in heroLinks) 
            {
                //<a href="/Alchemist" title="Alchemist"><img alt="Alchemist icon.png"
                //src="https://gamepedia.cursecdn.com/dota2_gamepedia/thumb/f/fe/Alchemist_icon.png/150px-Alchemist_icon.png?version=362ad1d92c189517ec2b15833387bf86"
                //decoding="async" width="150" height="84"
                //srcset="https://gamepedia.cursecdn.com/dota2_gamepedia/thumb/f/fe/Alchemist_icon.png/225px-Alchemist_icon.png?version=362ad1d92c189517ec2b15833387bf86
                //1.5x, https://gamepedia.cursecdn.com/dota2_gamepedia/f/fe/Alchemist_icon.png?version=362ad1d92c189517ec2b15833387bf86 2x"></a>
                var heroImg = image.OuterHtml.Substring(image.OuterHtml.IndexOf("<a href="));
                var myHeroImgArray = heroImg.Split('"');
                if (myHeroImgArray.Count() >= 7)
                {
                    heroimages.Add(myHeroImgArray[7]);
                }
            }

            heroimages.RemoveAll(x => x.Contains("Strength"));
            heroimages.RemoveAll(x => x.Contains("Agility"));
            heroimages.RemoveAll(x => x.Contains("Intelligence"));
            return heroimages;
        }

        private List<string> GetHeroNames(List<HtmlNode> heroLinks)
        {
            List<string> heroNames = new List<string>();
            foreach(var hero in heroLinks )
            {
                var heroName = hero.OuterHtml.Substring(hero.OuterHtml.IndexOf("<a href="));
                var myHeroNameArray = heroName.Split('"');
                heroNames.Add(myHeroNameArray[1].Replace("/\"","").Replace("/","").Replace("_"," ").Replace("%27","'"));
            }
            heroNames.RemoveAll(x => x == "Strength");
            heroNames.RemoveAll(x => x == "Agility");
            heroNames.RemoveAll(x => x == "Intelligence"); 
            return heroNames;
        }

        private List<HtmlNode> KeepHeroLinks(List<HtmlNode> links)
        {
            int index = links.FindIndex(x => x.OuterHtml.Contains("Abaddon"));
            for (int i = 0; i < index; i++) 
            {
                links.RemoveAt(0);
            }
            //int index2 = links.FindIndex(x => x.OuterHtml.Contains("Abaddon")); // is now 0
            links.Reverse();
            index = links.FindIndex(x => x.OuterHtml.Contains("Zeus"));
            for (int i = 0; i < index; i++)
            {
                links.RemoveAt(0);
            }
            links.Reverse();
            return links;
        }

        private List<HtmlNode> KeepLinks( string text)
        {
            //remove everything before table tag
            //var newText = text.Substring(text.IndexOf("<tbody>"));
            //newText = newText.Substring(newText.IndexOf("<tr>"));
            //newText = newText.Substring(newText.IndexOf("<tr>"));
            //int index = newText.IndexOf("</table>");

            //if (index > 0) 
            //{
            //    newText = newText.Substring(0, index);
            //}
            List<HtmlNode> links = new List<HtmlNode>();
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                links.Add(link);
            }
            return links;
        }
        public void CreateJsonFile()
        {
            using (WebClient client = new WebClient())
            {
                File.Create(Dest + "_DotaKwarted.json");

            }
        }
        public void PopulateJsonFile(Dictionary<string,string> heronames)
        {
            string html= "";
            List<Hero> heroes = new List<Hero>();
            
            foreach (var key in heronames.Keys)  //heronames.Count()
            {
                html = ReadHeroFile(key);
                Hero theHero = GetStats(html);
                heroes.Add(theHero);
            }
            string json = new JavaScriptSerializer().Serialize(heroes);

            System.IO.File.WriteAllText(Dest+"_DotaKwartet.json", json);
        }
        private string ReadHeroFile(string heroName)
        {
            return File.ReadAllText(Dest + heroName + ".html");
        }
        private Hero GetStats(string html)
        {
            //create html object
            List<HtmlNode> tables = new List<HtmlNode>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            //get table
            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table")) //table
            {
                tables.Add(table);
                //foreach (HtmlNode row in table.SelectNodes("tr")) //row
                //{
                //    foreach (HtmlNode cell in row.SelectNodes("th|td")) //cell
                //    {
                //    }
                //}
            }
            //get data from table - the data we want is at pos 1 & 2

            var theFooter = doc.GetElementbyId("catlinks");

            List<string> basicStats = new List<string>();
            foreach (HtmlNode tbody in tables.ElementAt(0).SelectNodes("tbody"))
             {
                foreach (HtmlNode row in tbody.SelectNodes("tr")) //cell
                {
                    foreach (HtmlNode cell in row.SelectNodes("th|td")) //cell
                    {
                        basicStats.Add(cell.InnerText);
                    }
                }
            }
            //table 1: level 1 stats

            List<string> stats = new List<string>();
            foreach (HtmlNode tbody in tables.ElementAt(1).SelectNodes("tbody"))
            {
                foreach (HtmlNode row in tbody.SelectNodes("tr")) //cell
                {
                    foreach (HtmlNode cell in row.SelectNodes("th|td")) //cell
                    {
                        stats.Add(cell.InnerText);
                    }
                }
            }
            stats.Count();
            //table 2: hero stats
            List<string> info = new List<string>();
            foreach (HtmlNode tbody in tables.ElementAt(2).SelectNodes("tbody"))
            {
                foreach (HtmlNode row in tbody.SelectNodes("tr")) //cell
                {
                    foreach (HtmlNode cell in row.SelectNodes("th|td")) //cell
                    {
                        info.Add(cell.InnerText);
                    }
                }
            }
            info.Count();
            //table 4: roles
            List<string> roles = new List<string>();
            foreach (HtmlNode tbody in tables.ElementAt(4).SelectNodes("tbody"))
            {
                foreach (HtmlNode row in tbody.SelectNodes("tr")) //cell
                {
                    foreach (HtmlNode cell in row.SelectNodes("th|td")) //cell
                    {
                        roles.Add(cell.InnerText);
                    }
                }
            }
            roles.Count();
            //lore
            var lore = doc.GetElementbyId("heroBio");

            //get correct data from the lists
            DeleteNewLines(info);
            DeleteNewLines(stats);
            DeleteNewLines(roles);
            Hero hero = new Hero();
            GetHeroBasicInfo(basicStats.ElementAt(0), hero);
            GetHeroInfo(info,hero);
            GetHeroStats(stats, hero);
            GetHeroRoles(roles, hero);
            GetHeroTypeAndRange(theFooter, hero);
            GetHeroLore(lore,hero);

            return hero;
        }

        private void GetHeroLore(HtmlNode lore, Hero hero)
        {
            var loreDiv = lore.SelectNodes("div");
            var heroLoreAndVoiceActor = loreDiv[2].InnerText.Remove(0, 8);
            var arrayLoreVoiceActor = heroLoreAndVoiceActor.Split(new string[] { "\n\n\nVoice:" }, StringSplitOptions.None);
            hero.Lore = arrayLoreVoiceActor[0];
            hero.VoiceActor = arrayLoreVoiceActor[1].Replace("(Responses)", "").Replace("\n","").Trim();
        }

        private void GetHeroTypeAndRange(HtmlNode theFooter, Hero hero)
        {
            string footerString = theFooter.InnerHtml;
            if (footerString.Contains("Strength"))
            {
                hero.PrimaryAttribute = "Strength";
            }
            else if (footerString.Contains("Agility"))
            {
                hero.PrimaryAttribute = "Agility";
            }
            else
            {
                hero.PrimaryAttribute = "Intelligence";
            }

            if (footerString.Contains("Melee"))
            {
                hero.AttackType = "Melee";
            }
            else 
            {
                hero.AttackType = "Range";
            }
        }

        private void GetHeroBasicInfo(string basicStats, Hero hero)
        {
            do
            {
                basicStats = basicStats.Replace("\n", "+").Replace("++", "+").Replace(" ", "");
            } while (basicStats.Contains("++"));
            var theBasicStats = basicStats.Split('+');
            hero.Name = theBasicStats[0];
            hero.Image = "images/"+theBasicStats[0]+".png";
            hero.startStrength = Int32.Parse(theBasicStats[1]);
            hero.StrengthPerLevel = Convert.ToDouble(theBasicStats[2].Replace(".",","));
            hero.startAgility = Int32.Parse(theBasicStats[3]);
            hero.AgilityPerLevel = Convert.ToDouble(theBasicStats[4].Replace(".", ","));
            hero.startIntelligence = Int32.Parse(theBasicStats[5]);
            hero.IntelligencePerLevel = Convert.ToDouble(theBasicStats[6].Replace(".", ","));
        }

        private void GetHeroRoles(List<string> roles, Hero hero)
        {
            var heroRoles = roles.ElementAt(3).Replace("&#32;", ",").Replace("\n","").Replace(" ", "").Split(',');
            //heroRoles.Reverse();
            List<string> values = heroRoles.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            hero.Roles = values;
        }

        private void GetHeroStats(List<string> stats, Hero hero)
        { //8 - 14 - 20 - ...
            hero.Health = Int32.Parse(stats.ElementAt(8));
            hero.HealthRegen = Convert.ToDouble(stats.ElementAt(14).Replace(".", ","));
            hero.MagicalResistance = Convert.ToDouble(stats.ElementAt(20).Replace("%","").Replace(".", ",")) /100;
            hero.Mana = Int32.Parse(stats.ElementAt(26));
            hero.ManaRegen = Convert.ToDouble(stats.ElementAt(32).Replace(".", ","));
            hero.SpellDamageAmplification = Convert.ToDouble(stats.ElementAt(38).Replace(".", ",").Replace("%", ""))/100;
            hero.Armor = Convert.ToDouble(stats.ElementAt(44).Replace(".", ","));
            hero.AttacksPerSecond = Convert.ToDouble(stats.ElementAt(50).Replace(".", ","));
            hero.MovementSpeedAmplification = Convert.ToDouble(stats.ElementAt(56).Replace(".", ",").Replace("%", ""))/100;
            hero.Damage = Convert.ToDouble(stats.ElementAt(64).Replace(".", ",").Split('‒').ElementAt(1));
        }

        private void GetHeroInfo(List<string> info, Hero hero)
        {
            hero.MovementSpeed = Int32.Parse(info.ElementAt(1));
            hero.AttackSpeed = Int32.Parse(info.ElementAt(3));
            hero.TurnRate = Convert.ToDouble(info.ElementAt(5).Replace(".",","));
            hero.DayVisionRange = Int32.Parse(info.ElementAt(7).Split('/').ElementAt(0));
            hero.NightVisionRange = Int32.Parse(info.ElementAt(7).Split('/').ElementAt(1));
            hero.AttackRange = Int32.Parse(info.ElementAt(9));
            if (info.ElementAt(11).Equals("Instant"))
            {
                hero.ProjectileSpeed = 0;
            }
            else
            {
                hero.ProjectileSpeed = Int32.Parse(info.ElementAt(9));
            }
            hero.AttackAnimation = info.ElementAt(13).Replace("\n", "");
            hero.BaseAttackTIme = Convert.ToDouble(info.ElementAt(15).Replace(".", ","));
            hero.DamageBlock = Int32.Parse(info.ElementAt(17));
            hero.CollisionSize = Int32.Parse(info.ElementAt(19));
            hero.Legs = Int32.Parse(info.ElementAt(21).Replace("&#160;",""));
            hero.GibType = info.ElementAt(23).Replace("\n", "");
        }

        private void DeleteNewLines(List<string> heroInformation)
        {
            heroInformation.ForEach(x => x.Replace(System.Environment.NewLine, ""));
        }
    }
}
