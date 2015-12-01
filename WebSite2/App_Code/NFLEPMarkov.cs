using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.OleDb;

/// <summary>
/// Summary description for NFLEPMarkov
/// </summary>
public class NFLEPMarkov
{
    public string State { get; set; }
    public double Downs { get; set; }
    public double FGMade { get; set; }
    public double FGMiss { get; set; }
    public double FUM { get; set; }
    public double INT { get; set; }
    public double PUNT { get; set; }
    public double SAF { get; set; }
    public double TD { get; set; }
    public double Frequency { get; set; }
    public double PlaysRemaining { get; set; }
    //acn Markov EP, Recursive EP, Regressive EP
    public double Markov { get; set; }
    public double Recursive { get; set; }
    public double Regressive { get; set; }
    public double Down { get; set; }
    //acn DTG, Ydline
    public double YardsToGo { get; set; }
    public double YardLine { get; set; }

    public NFLEPMarkov() { }
    //if I felt like it could probably do this at runtime using BuildObject and other voodoo/black magic
    public static NFLEPMarkov Create(IDataRecord record)
    {
        return new NFLEPMarkov { 
            State = (string)record["State"],
            Downs = (double)record["Downs"],
            FGMade = (double)record["FGMade"],
            FGMiss = (double)record["FGMiss"],
            FUM = (double)record["FUM"],
            INT = (double)record["INT"],
            PUNT = (double)record["PUNT"],
            SAF = (double)record["SAF"],
            TD = (double)record["TD"],
            Frequency = (double)record["Frequency"],
            PlaysRemaining = (double)record["PlaysRemaining"],
            Markov = (double)record["Markov EP"],
            Recursive = (double)record["Recursive EP"],
            Regressive = (double)record["Regressive EP"],
            Down = (double)record["Down"],
            YardsToGo = (double)record["DTG"],
            YardLine = (double)record["Ydline"]
        };
    }

    public IEnumerable<NFLEPMarkov> GetMarkovData(string FileName)
    {
        string sSheetName = null;
        string sConnection = null;
        DataTable dtTablesList = default(DataTable);
        OleDbCommand oleExcelCommand = default(OleDbCommand);
        OleDbDataReader oleExcelReader = default(OleDbDataReader);
        OleDbConnection oleExcelConnection = default(OleDbConnection);

        sConnection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FileName + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";
        oleExcelConnection = new OleDbConnection(sConnection);
        oleExcelConnection.Open();

        dtTablesList = oleExcelConnection.GetSchema("Tables");

        if (dtTablesList.Rows.Count > 0)
        {
            sSheetName = dtTablesList.Rows[0]["TABLE_NAME"].ToString();
        }

        dtTablesList.Clear();
        dtTablesList.Dispose();


        if (!string.IsNullOrEmpty(sSheetName))
        {
            oleExcelCommand = oleExcelConnection.CreateCommand();
            oleExcelCommand.CommandText = "Select * From [" + sSheetName + "]";
            oleExcelCommand.CommandType = CommandType.Text;
            oleExcelReader = oleExcelCommand.ExecuteReader();

            //nOutputRow = 0;
            //var sheets = oleExcelConnection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            //oleExcelCommand.CommandText = "SELECT * FROM [" + sheets.Rows[0]["TABLE_NAME"].ToString() + "] ";

            //var adapter = new OleDbDataAdapter(oleExcelCommand);
            //var ds = new DataSet();
            //adapter.Fill(ds);

            //object[] dataFill = new object[17];

            using (var reader = oleExcelReader) { //will this work?
            //using (var reader = oleExcelCommand.ExecuteReader())
            //I think using gets rid of the reader, hopefully.
                while (reader.Read()) {
                     yield return NFLEPMarkov.Create(reader);
                }
            }
        }
        oleExcelConnection.Close();
    }
}