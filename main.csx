#!/usr/bin/env dotnet-script
#load "models.csx"
#load "shell.csx"

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
    List<string> lsb_release_a = bash.Execute("lsb_release -da").Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
    List<string> os_release = bash.Execute("cat /etc/os-release").Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

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

    bash.Execute("Updating system", "sudo apt-get update -y");
    bash.Execute("Upgrading system", "sudo apt-get upgrade -y");
    bash.Execute("Installing needed tools", "sudo apt-get install " + string.Join(" ", tools.ToArray()) + " -y");
}

#endregion