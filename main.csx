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

UserInformation userInformation;
Distribution distribution;
IShell _bash = new Bash();
Git git = new Git(_bash);

GetUserInformation();

CreateNecessaryFolders();

GetDistributionInformation();

InstallNeededTools();

CloneGitRepositories(userInformation.HomePath);


#endregion Main

#region Methodes

private string bash(string command) => _bash.Execute(command);

private string bash(string description, string cmd) => _bash.Execute(description, cmd);
private void GetUserInformation() 
{
    userInformation = new UserInformation();
    userInformation.HomePath = "~";
}
private void CreateNecessaryFolders(string homePath = "~") 
{
    Directory.CreateDirectory($"{homePath}/dev");
    Directory.CreateDirectory($"{homePath}/dev/shell-scripts");
    Directory.CreateDirectory($"{homePath}/packages");
    Directory.CreateDirectory($"{homePath}/applications");
}
private void GetDistributionInformation()
{
    List<string> lsb_release_a = bash("lsb_release -da").Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
    List<string> os_release = bash("cat /etc/os-release").Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

    string distributorID, description, release, codename, version, idLike, kernel, architecture;
    distributorID = description = release = version = codename = idLike = kernel = architecture = string.Empty;

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

    if (distribution.IsDebian()) 
    {
        List<string> hostnamectl = bash("hostnamectl").Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

        foreach (var hostnamectl_field in hostnamectl)
        {
            if (hostnamectl_field.Contains("Kernel:"))
                kernel = Regex.Match(hostnamectl_field, "Kernel:(.*)$").Result("$1").ToString().Trim();
            else if (hostnamectl_field.Contains("Architecture:"))
                architecture = Regex.Match(hostnamectl_field, "Architecture:(.*)$").Result("$1").ToString().Trim();
        }    
    }
    
    distribution.AddKernelAndArchicteture(kernel, architecture);
    
    Console.WriteLine("You're running {0} ({1})", distribution.Description, distribution.IdLike);
}

private void InstallNeededTools() 
{
    List<string> tools = new List<string>();
    tools.Add("lsb-core");
    tools.Add("lsb-release");
    tools.Add("git");
    tools.Add("wmctrl");
    tools.Add("autoconf");
    tools.Add("automake");
    tools.Add("intltool");
    tools.Add("yad");

    if (distribution.IsDebian()) 
    {
        tools.Add("ppa-purge");

    }    

    bash("Updating system", "sudo apt-get update -y");
    bash("Upgrading system", "sudo apt-get upgrade -y");
    bash("Installing needed tools", "sudo apt-get install " + string.Join(" ", tools.ToArray()) + " -y");
}

private void CloneGitRepositories(string homePath) 
{    
    git.Clone("https://github.com/frohlichcortezh/bash-scripts.git", $"{homePath}/dev/shell-scripts/");
    git.Clone("https://github.com/frohlichcortezh/fish-functions.git", $"{homePath}/dev/shell-scripts/");
    
    /*git.Clone("https://gitlab.gnome.org/GNOME/jhbuild.git", $"{homePath}/dev/shell-scripts/");
    Console.WriteLine(bash.Execute(string.Concat("cd ", $"{homePath}/dev/shell-scripts/jhbuild", " && ./autogen.sh")));
    Console.WriteLine(bash.Execute(string.Concat("cd ", $"{homePath}/dev/shell-scripts/jhbuild", " && make")));
    Console.WriteLine(bash.Execute(string.Concat("cd ", $"{homePath}/dev/shell-scripts/jhbuild", " && make install")));
    bash.Execute("echo 'PATH=$PATH:~/.local/bin' >> ~/.bashrc");

    git.Clone("https://github.com/v1cont/yad.git", $"{homePath}/dev/shell-scripts/yad-dialog-code");
    Console.WriteLine(bash.Execute(string.Concat("cd ", $"{homePath}/dev/shell-scripts/yad-dialog-code", " && autoreconf -ivf && intltoolize")));
    Console.WriteLine(bash.Execute(string.Concat("cd ", $"{homePath}/dev/shell-scripts/yad-dialog-code", " && ./configure && make && make install")));
    bash.Execute("gtk-update-icon-cache");*/
}
#endregion