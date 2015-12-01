using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

///<summary>
/// Summary description for Drives
/// </summary>
public class Drives
{// Start\Key is the number of the drive

    [JsonProperty("posteam")]
    public string Posteam { get; set; }

    [JsonProperty("qtr")]
    public int Qtr { get; set; }

    [JsonProperty("redzone")]
    public bool Redzone { get; set; }

    //each play has a unique number as the key
    //public Plays Plays { get; set; }

    [JsonProperty("fds")]
    public int Fds { get; set; }

    [JsonProperty("result")]
    public string Result { get; set; }

    [JsonProperty("penyds")]
    public int Penyds { get; set; }

    [JsonProperty("ydsgained")]
    public int Ydsgained { get; set; }

    [JsonProperty("numplays")]
    public int Numplays { get; set; }

    [JsonProperty("postime")]
    public string Postime { get; set; }

    [JsonProperty("start")]
    public Start Start { get; set; }

    [JsonProperty("end")]
    public End End { get; set; }
}

public class Plays
{//Start\Key is a number that I have not figured out what it means.

    [JsonProperty("sp")]
    public int Sp { get; set; }

    [JsonProperty("qtr")]
    public int Qtr { get; set; }

    [JsonProperty("down")]
    public int Down { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }

    [JsonProperty("yrdln")]
    public string Yrdln { get; set; }

    [JsonProperty("ydstogo")]
    public int Ydstogo { get; set; }

    [JsonProperty("ydsnet")]
    public int Ydsnet { get; set; }

    [JsonProperty("posteam")]
    public string Posteam { get; set; }

    [JsonProperty("desc")]
    public string Desc { get; set; }

    [JsonProperty("note")]
    public string Note { get; set; }

    public string EPState { get; set; }
    public double MarkovExpPts { get; set; }

    //The key to players will change on playerID, I don't have the NFL playerID's, so will need a work around
    //[JsonProperty("players")]
    //public IList<Players> Players { get; set; }    
}

public class Players
{//The "Start"/Key of this JSONArray is the playerID, each one is different this could get complicated
    [JsonProperty("sequence")]
    public int Sequence { get; set; }

    [JsonProperty("clubcode")]
    public string Clubcode { get; set; }

    [JsonProperty("playerName")]
    public string PlayerName { get; set; }

    [JsonProperty("statId")]
    public int StatId { get; set; }

    [JsonProperty("yards")]
    public int Yards { get; set; }
}

public class Start
{
    [JsonProperty("qtr")]
    public int Qtr { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }

    [JsonProperty("yrdln")]
    public string Yrdln { get; set; }

    [JsonProperty("team")]
    public string Team { get; set; }

    public double expectedPts { get; set; }
}

public class End
{
    [JsonProperty("qtr")]
    public int Qtr { get; set; }

    [JsonProperty("time")]
    public string Time { get; set; }

    [JsonProperty("yrdln")]
    public string Yrdln { get; set; }

    [JsonProperty("team")]
    public string Team { get; set; }

    public double expectedPts { get; set; }
}