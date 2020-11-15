#!/usr/bin/env dotnet-script

using System;
using System.Diagnostics;
using System.Text.RegularExpressions; 

#region ToDo 

/// Non-ehaustive list of things to do
/// 1. Add documentation


#endregion ToDo

#region Main

Distribution distribution;
Bash bash = new Bash();

GetDistributionInformation();
InstallNeededTools();

#endregion Main

#region Methodes

private void GetDistributionInformation()
{
    List<string> lsb_release_a = bash.Execute("lsb_release -da").Split(Environment.NewLine).ToList();
    List<string> os_release = bash.Execute("cat /etc/os-release").Split(Environment.NewLine).ToList();

    string distributorID, description, release, codename, version, idLike;
    distributorID = description = release = version = codename = idLike = string.Empty;

    foreach (var lsb_release_a_field in lsb_release_a)
    {
        if (lsb_release_a_field.Contains("Distributor ID:"))
            distributorID = Regex.Match(lsb_release_a_field, "Distributor ID:(.*)$").Result("$1").ToString().Trim();
        else if (lsb_release_a_field.Contains("Description:"))
            description = Regex.Match(lsb_release_a_field, "Description:(.*)$").Result("$1").ToString().Trim();
        else if (lsb_release_a_field.Contains("Release:"))
            release = Regex.Match(lsb_release_a_field, "Release:(.*)$").Result("$1").ToString().Trim();
        else if (lsb_release_a_field.Contains("Codename:"))
            codename = Regex.Match(lsb_release_a_field, "Codename:(.*)$").Result("$1").ToString().Trim();            
    }    


    foreach (var os_release_field in os_release)
    {
        if (os_release_field.Contains("VERSION="))
            version = Regex.Match(os_release_field, "VERSION=(.*)$").Result("$1").ToString().Trim();
        else if (os_release_field.Contains("ID_LIKE="))
            idLike = Regex.Match(os_release_field, "ID_LIKE=(.*)$").Result("$1").ToString().Trim();
    }     
    
    distribution = new Distribution(distributorID, description, release, version, codename, idLike);    
    Console.WriteLine("You're running {0} ({1})", distribution.Description, distribution.IdLike);
}

private void InstallNeededTools() 
{
    List<string> tools = new List<string>();
    tools.Add("lsb-core");
    tools.Add("lsb-release");
    
    if (distribution.IsDebian()) 
    {
        tools.Add("ppa-purge");
    }
    

    bash.Execute("Updating system", "sudo apt-get update -y");
    bash.Execute("Upgrading system", "sudo apt-get upgrade -y");
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
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("----- STARTED [{0} - {1}] : {2} -----", startTime.ToShortDateString(), startTime.ToShortTimeString(), description);        
        Console.ForegroundColor = ConsoleColor.White;

        string result = process.StandardOutput.ReadToEnd();
        do 
        {
            process.Refresh();
            //ToDo - Clear line before writing
            Console.Write("Duration: {0} seconds", DateTime.Now.Subtract(startTime).TotalSeconds);
        } while (!process.HasExited);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("----- FINISHED [{0} - {1}] : {2} -----", DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortTimeString(), description);
        Console.ForegroundColor = ConsoleColor.White;        
        
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
        this.DistributorID = distributorID;
        this.Description = description;
        this.Release = release;
        this.Version = version;
        this.IdLike = idLike;
    }

    public bool IsDebian() => IdLike.Contains("debian");
    
}

#endregion