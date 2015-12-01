using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Data;

/// <summary>
/// Summary description for ReadJSONDatafromNFL
/// </summary>
[WebService]
public class ReadJSONDatafromNFL
{
    public void DeserializePlayerStats(JObject homeStats, string homeTeam, JObject awayStats, string awayTeam)
    {
        //This is the names of the two different properties in the JSON 
        JObject homePassing = (JObject)homeStats["passing"];
        JObject awayPassing = (JObject)awayStats["passing"];
        //Need something of all stats subcategories strings 
        //List<string> StatsChildren = new List<string>(new string[] { "passing", "rushing", "receiving", "fumbles", "kicking" });
        Dictionary<string, string> StatsChildren = new Dictionary<string,string> {
            {"passing", "PassingStats"},
            {"rushing", "RushingStats"},
            {"receiving", "ReceivingStats"},
            {"fumbles", "FumbleStats"},
            {"kicking", "KickingStats"}
         };
        //playerIDKeys.AddRange(awayPassing.Properties().Select(p => p.Name).ToList());
        Dictionary<string, JObject> JObjectHomeAway = new Dictionary<string, JObject>();
        JObjectHomeAway.Add("homeStats", homeStats);
        JObjectHomeAway.Add("awayStats", awayStats);
        List<String> playerIDKeys = new List<string>();
        List<NFLPlayer> PlayerList = new List<NFLPlayer>();     //List of made NFL Players

        foreach (KeyValuePair<string, string> child in StatsChildren) {
            foreach (KeyValuePair<string, JObject> objName in JObjectHomeAway) {

                JObject statsJObj = objName.Value;   //passed in from dict, is either the homeStats or awayStats jObject
                string objPropertyName = objName.Key;
                JObject getIDs = (JObject)statsJObj[child.Key];

                playerIDKeys.AddRange(getIDs.Properties().Select(p => p.Name).ToList());

                foreach (string playerID in playerIDKeys) {
                    /*PsC - Create list of players
                      Get PlayerID of player about to be added
                     Compare to List of players
                     If Found*/
                    
                    NFLPlayer NFLFoundPlayer = null;

                    //going through the list of already made players and pulling the player if the id's match
                    //if found add stats according to child (pass,rec, rush, etc)
                    //if not found, create player and copy material
                    if (PlayerList.Count() != 0) {
                       NFLFoundPlayer = PlayerList.Find(x => x.id.Contains(playerID));
                    }


                    //Player not found if null, so create and fill in player info
                    if (NFLFoundPlayer == null) {
                        NFLFoundPlayer = new NFLPlayer();
                        NFLFoundPlayer.id = playerID;
                        //Using homeStats and awayStats as property names., jObj is home or awayStats jObject                  
                        NFLFoundPlayer.name = statsJObj[child.Key][playerID]["name"].ToString();

                        if (objPropertyName == "homeStats") {
                            NFLFoundPlayer.team = homeTeam;
                        }
                        else if (objPropertyName == "awayStats") {
                            NFLFoundPlayer.team = awayTeam;
                        }
                        else {
                            NFLFoundPlayer.team = "XXX";
                        }
                    }
                    else { }

                    //Finds the correct type of stats with the playerId and puts it into a JOnject
                    var statsPullJSON = statsJObj[child.Key][playerID];

                    //takes pulled stats and adds them to the FoundPlayer
                    if (child.Key == "passing") {                    
                        NFLFoundPlayer.PassingStats = (PassingGameStats)statsPullJSON.ToObject(typeof(PassingGameStats));
                    }
                    else if (child.Key == "rushing") {
                        NFLFoundPlayer.RushingStats = (RushingGameStats)statsPullJSON.ToObject(typeof(RushingGameStats));
                    }
                    else if (child.Key == "receiving") {
                        NFLFoundPlayer.ReceivingStats = (ReceivingGameStats)statsPullJSON.ToObject(typeof(ReceivingGameStats));
                    }
                    else if (child.Key == "fumbles") {
                        NFLFoundPlayer.FumbleStats = (FumbleGameStats)statsPullJSON.ToObject(typeof(FumbleGameStats));
                    }
                    else if (child.Key == "kicking") {
                        NFLFoundPlayer.KickingStats = (KickingGameStats)statsPullJSON.ToObject(typeof(KickingGameStats));
                    }
                    else { //throw exception
                    }

                    //add in NFLPlayer to Playerlist
                    PlayerList.Add(NFLFoundPlayer);
                    //NFLPlayer NFLPlayer = (Plays)serializer.Deserialize(new JTokenReader(playsInCurrentDrive[key]), typeof(Plays));
                    
                }
                playerIDKeys.Clear();   //empty keys for next iteration
            }
        }
    }

    [WebMethod]
	public void DeserializeData()
	{
        //string json = get_web_content("http://localhost:54551/2015101200_gtd.json"); //NFL.com address
        //dynamic game = NFLData;
        //dynamic newGame = new JObject();
        string FileName = "2015101200_gtd.json";

        int ID = 2015101200;                //Parse from somewhere, prob web addr call or my schedule database
        string gameID = ID.ToString();      //Needs to be in string for JSON calls.

        string Root = HttpContext.Current.Server.MapPath("~/");
        string FullPath = Root + FileName;
        var results = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(FullPath));
        JObject NFLData = JObject.Parse(File.ReadAllText(FullPath));

        var score = (string)NFLData.SelectToken(gameID + ".home.score.1");
        var homeScore1 = NFLData["2015101200"]["home"]["score"];
        var homeScoreres = results[gameID]["home"]["score"];

        var firstDrive = results[gameID]["drives"]["1"];
        var secDrive = results[gameID]["drives"]["2"];
        

        string homeTeam = results[gameID]["home"]["abbr"];    
        string awayTeam = results[gameID]["away"]["abbr"];     
        JObject homeStats = results[gameID]["home"]["stats"];
        JObject awayStats = results[gameID]["away"]["stats"];

        DeserializePlayerStats(homeStats, homeTeam, awayStats, awayTeam);
        DeserializeTeamStats(homeStats, homeTeam, awayStats, awayTeam);

        string excelFileName = "MARKOVDATA.xlsx";
        string excelFullPath = Root + excelFileName;

        //NFLEPMarkov MarkovTable = new NFLEPMarkov();
        //MarkovTable.GetMarkovData(excelFullPath);
        //foreach(var a in MarkovTable.GetMarkovData(excelFullPath))
        //{//replace with add to MarkovList!
        //    Console.WriteLine(a); }


        int totalDrives = results[gameID]["drives"]["crntdrv"];     //if game is over crntdrive will be the total # of drives
        JObject drives = results[gameID]["drives"];
        //function pass in totalDrives, find out what var is 99.9% sure its a JObject
        serializeDrives(totalDrives, drives);
        
	}

    //This function takes JObjects and finds the correct path to pull the TeamStatsData and stores in a TeamStates Object
    public void DeserializeTeamStats(JObject homeStats, string homeTeam, JObject awayStats, string awayTeam) {
        homeStats = (JObject)homeStats["team"];
        awayStats = (JObject)awayStats["team"];
        TeamStats homeStatsobj = (TeamStats)homeStats.ToObject(typeof(TeamStats));
        TeamStats awayStatsobj = (TeamStats)awayStats.ToObject(typeof(TeamStats));
    }

    public int convertToIntYardLine(string yardLine, string posTeam) {
        string[] seperatedYardLine = yardLine.Split();
        string ballSideOfField = seperatedYardLine[0];
        int yardNum = Convert.ToInt32(seperatedYardLine[1]);

        if (ballSideOfField == null || yardNum < 0 || yardNum > 50) {
            throw new FormatException("ballSideOfField is null or yardNum is out of Range");
        }

        /*Trying to figure out if ball is on your side of the field or opponent's convert from teamname yrdline to yrdsfromtd
        INPUT yardLine-SD 35, posTeam-SD
        your side of the field is 51-99, opponents is 1-50
        yourSideOfTheField = true
         * output is 65, 65 yards from the endzone */
        if (ballSideOfField == posTeam) {
            yardNum = 100 - yardNum;

            if (yardNum < 50 ) {
                throw new FormatException("Ball is on your side of the field, yardsfromTD should be greater than 51");
            }
        }

        return yardNum;
    }

    public void getExpectedPointsForPlay(int down, int ydsToFirst, int convertedYardsToTD) {
        NFLEPMarkov EP = new NFLEPMarkov();
        //need format of MarkovChains List?
        //Source Markov Drive Analysis-Google spreadsheet
        /*Find down
         * Find Yards To Go - (Format - 1,2,3,4,5,6,7,8,9,10,11,16,21)
         * Find convertedYardsToTD 
         * 
         * Code - Find down and yards to go
         * Take all remaning convertedYardsToTD and put into list and search and find closest
         * */
        
        //var findDownYardToFirst = list.FindAll(x => (x.Down == down) && (x.YardsToGo == ydsToFirst));
        //IList<double> AllYardsToTDFound = new IList<double();
        //AllYardsToTDFound = findDownYardToFirst.Properties().Select(p => p.YardLine).ToList();

        //AllYardsToTDFound.Find(x => (x.YardLine >= convertedYardsToTD));
        //

    }

    public void serializeDrives(int totalDrives, JObject drives) {
        //Goes through each drive in gameID
        for (int i = 1; i <= totalDrives; ++i) {
            //Is the current drive aka 1,2,3,etc.
            var currentDrive = (JObject)drives[i.ToString()];

            if (currentDrive != null) {

                //Not sure the best way to do this, but the Plays and Players aren't Deserializing correctly.
                //I could write a custom deserializer or jtokenreader? but not sure how or how long that would take.
                //Work around is I can parse it "manually", using the results and keys that are different for every game.

                //throw into using, IDisposable?
                JsonSerializer serializer = new JsonSerializer();
                //Store currentDrive into Drives object
                //Drives test = currentDrive.ToObject(typeof(Drives));
                Drives storeCurrentDrive = (Drives)serializer.Deserialize(new JTokenReader(currentDrive), typeof(Drives));

                //taking the plays out of the currentDrive and storing into JObject
                JObject playsInCurrentDrive = (JObject)currentDrive["plays"];
                //taking the key of each individual play and storing into a list
                IList<string> playsKeys = playsInCurrentDrive.Properties().Select(p => p.Name).ToList();

                //going through each key of the ind. play and taking out the play, players and storing into objects
                foreach (string key in playsKeys) {
                    //storing the play into play object, need the unique key for each play to read
                    Plays play = (Plays)serializer.Deserialize(new JTokenReader(playsInCurrentDrive[key]), typeof(Plays));
                    int convertedYardsToTD = convertToIntYardLine(play.Yrdln, play.Posteam);
                    getExpectedPointsForPlay(play.Down, play.Ydstogo, convertedYardsToTD); //Down,YardsToGo,YardLine(in 1-99 format)
                    //getting players from current play
                    JObject playersInCurrentPlay = (JObject)currentDrive["plays"][key]["players"];

                    //Getting the key (semi-unique, it's the playerID) and storing in a list
                    IList<string> playersKeys = playersInCurrentPlay.Properties().Select(p => p.Name).ToList();
                    //Going through each key and storing the players into players list
                    foreach (string playerKey in playersKeys) {
                        //storing the playerID
                        string playerID = playerKey;
                        //Putting the current play players into a list
                        IList<Players> PlayersList = serializer.Deserialize<IList<Players>>(new JTokenReader(playersInCurrentPlay[playerKey]));
                    }
                }

                //another way to do it, didnt work as well
                //JEnumerable<JToken> playsContainer = results[gameID]["drives"][i.ToString()]["plays"].Children();
            }
            else { } //ThrowException NOT SUPPOSED TO BE NULL
        }
    }

    public string get_web_content(string url)
    {
        Uri uri = new Uri(url);
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
        request.Method = WebRequestMethods.Http.Get;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string output = reader.ReadToEnd();
        response.Close();

        return output;
    }
}