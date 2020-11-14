#!/usr/bin/env dotnet-script

using System;
using System.Diagnostics;
using System.Text.RegularExpressions; 

#region Main
Distribution distribution;
Bash bash = new Bash();

GetDistributionInformation();

InstallNeededTools();
#endregion

#region Methodes

private void GetDistributionInformation() 

{
    var lsb_release_a = bash.Execute("lsb_release -da").Split(Environment.NewLine).ToList();
    string os_release = bash.Execute("cat /etc/os-release");

    string distributorID, description, release, version, codename, idLike;
    distributorID = description = release = version = codename = idLike = string.Empty;
//    Regex regex;
    var i = lsb_release_a.IndexOf("Distributor ID:");

    foreach (var lsb_release_a_field in lsb_release_a)
    {
        if (lsb_release_a_field.Contains("Distributor ID:"))
            distributorID = Regex.Match(lsb_release_a_field, "Distributor ID:(.*)$").Result("$1").ToString().Trim();

    }    
    Console.WriteLine(distributorID);
    
    distribution = new Distribution(distributorID, description, release, version, codename, idLike);
}

private void InstallNeededTools() 
{
    List<string> tools = new List<string>();
    tools.Add("lsb-core");
    tools.Add("lsb-release");


    bash.Execute("Installing needed tools", "sudo apt-get install " + string.Join(" ", tools.ToArray()) + " -y");
}
#endregion

#region "Shells "
public class Bash
{
    #region Fields
    private System.Diagnostics.Process process;

    #endregion

    #region Constructor 
    public Bash() 
    {
        process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
    }
    
    #endregion
    
    #region Public methods

    public string Execute(string cmd) 
    {
        process.StartInfo.Arguments = $"-c \"{EscapeArgs(cmd)}\"";
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;        
    }

    public string Execute(string description, string cmd) 
    {
        process.StartInfo.Arguments = $"-c \"{EscapeArgs(cmd)}\"";
        DateTime startTime = DateTime.Now;      

        process.Start();
        Console.WriteLine("----- STARTED [{0} - {1}] : {2} -----", startTime.ToShortDateString(), startTime.ToShortTimeString(), description);        

        string result = process.StandardOutput.ReadToEnd();
        do 
        {
            process.Refresh();
            //ToDo - Clear line before writing
            Console.Write("Duration: {0} seconds", DateTime.Now.Subtract(startTime).TotalSeconds);
        } while (!process.HasExited);

        Console.WriteLine();
        Console.WriteLine("----- FINISHED [{0} - {1}] : {2} -----", DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortTimeString(), description);
        return result;        
    }
    
    public string ls(params string[] args) => Execute("ls " + string.Join(" ", args));

    #endregion

    #region Private methods
    
    private string EscapeArgs(string cmd) => cmd.Replace("\"", "\\\"");
    
    #endregion
}


   public static string Fish(this string cmd) 
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "usr/bin/fish",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;
    }
    
#endregion

#region Models
public class Distribution 
{
    public string DistributorID { get; internal set; }
    public string Description { get; internal set; }
    public string Release { get; internal set; }
    public string Version { get; internal set; }
    public string Codename { get; internal set; }
    public string IdLike { get; internal set; }

    public Distribution(string distributorID, string description, string release, string version, string codename, string idLike)
    {

    }

    public bool IsDebian() => IdLike.Contains("debian");
    
}

#endregion