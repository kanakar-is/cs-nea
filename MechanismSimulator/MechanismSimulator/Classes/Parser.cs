using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MechanismSimulator.Classes
{
    internal class Parser : Body
    {
        protected MainWindow MainWindow;
        protected bool StarLoaded;
        public Parser(Canvas spaceCanvas, MainWindow mainWindow, bool starLoaded) : base(spaceCanvas)
        {
            SpaceCanvas = spaceCanvas;
            MainWindow = mainWindow;
            StarLoaded = starLoaded;
        }

        public void ParseSavedJson(string selectedJson) 
        {

            JToken jsonToken;
            using (StreamReader file = File.OpenText($"Presets/{selectedJson}"))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jsonToken = JToken.ReadFrom(reader);
                }
            }

            foreach (var obj in jsonToken)
            {
                string type = obj["type"].ToString();

                if (type == "Star")
                {
                    StarLoaded = true;
                    Star star = new Star(SpaceCanvas, (int)obj["radius"]);
                    star.GenerateStar(obj["name"].ToString(), (int)obj["radius"], (int)obj["temperature"], (int)obj["systype"]);
                    MainWindow.AddStarToQueue(obj["name"].ToString(), (int)obj["fieldstrength"], (int)obj["radius"], (int)obj["temperature"], (int)obj["systype"]);
                    
                }
                else if (type == "TerrestrialPlanet")
                {
                    TerrestrialPlanet terrestrialPlanet = new TerrestrialPlanet(SpaceCanvas, (int)obj["radius"]);
                    terrestrialPlanet.GenerateTerrestrialPlanet(obj["name"].ToString(),
                        (int)obj["lifelevel"],
                        (int)obj["radius"],
                        (int)obj["waterlevel"],
                        (int)obj["temperature"],
                        (bool)obj["lm"],
                        (int)obj["moonnumber"],
                        (double)obj["distancefromstar"]);
                    MainWindow.AddPlanetToQueue(obj["name"].ToString(), (int)obj["lifelevel"], (int)obj["waterlevel"], (int)obj["radius"], (int)obj["temperature"], (int)obj["moonnumber"]);

                }
                else if (type == "GaseousPlanet")
                {
                    GaseousPlanet gaseousPlanet = new GaseousPlanet(SpaceCanvas, (int)obj["radius"]);
                    gaseousPlanet.GenerateGaseousPlanet(obj["name"].ToString(), (int)obj["temperature"], (int)obj["radius"], (int)obj["moonnumber"], (double)obj["distancefromstar"]);
                    MainWindow.AddPlanetToQueue(obj["name"].ToString(), 0, 0, (int)obj["radius"], (int)obj["temperature"], (int)obj["moonnumber"]);

                }
            }

        }

        public void SaveToJson(JArray fullJson)
        {
            bool successfulSave = false;
            int increment = 0;
            while (successfulSave == false) 
            {
                if(File.Exists($".Presets/Untitled-{increment}.json") == false)
                {
                    File.WriteAllText($"Presets/Untitled-{increment}.json", fullJson.ToString());
                    successfulSave = true;
                }
                    
                increment = increment + 1;
            }
        }
    }

}
