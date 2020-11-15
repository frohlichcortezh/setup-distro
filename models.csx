#!/usr/bin/env dotnet-script
#load "shell.csx"

#region Models
public class Distribution 
{
    public string DistributorID { get; internal set; }
    public string Description { get; internal set; }
    public string Release { get; internal set; }
    public string Version { get; internal set; }
    public string Codename { get; internal set; }
    public string IdLike { get; internal set; }

    public string Kernel { get; internal set; }
    
    public string Architecture { get; internal set; }

    public Distribution(string distributorID, string description, string release, string version, string codename, string idLike, 
        string kernel = "", string architecture = "")
    {
        this.DistributorID = distributorID;
        this.Description = description;
        this.Release = release;
        this.Version = version;
        this.IdLike = idLike;
        this.Kernel = kernel;
        this.Architecture = architecture;
    }

    public void AddKernelAndArchicteture(string kernel, string architecture) 
    {
        if (string.IsNullOrWhiteSpace(this.Kernel))
            this.Kernel = kernel;

        if (string.IsNullOrWhiteSpace(this.Architecture))
            this.Architecture = architecture;
    }

    public bool IsDebian() => IdLike.Contains("debian");
    
}

public class UserInformation 
{
    public string Name {get;set;}

    public string Username {get;set;}

    public string Email {get;set;}

    public string HomePath {get;set;}
}


public class Git 
{
    private IShell _shell1;

    public Git(IShell shell) 
    {
        _shell1 = shell;
    }
    public string Username {get;set;}

    public string Email {get;set;}

    public string Clone(string repositoryUrl, string path = ".") 
    {        
        return _shell1.Execute(string.Concat("git clone ", repositoryUrl, " ", path));
    }

    public string AddCommitPush(string message) 
    {
        string r = string.Empty;
        r += _shell1.Execute(string.Concat("git add ."));
        r += System.Environment.NewLine + _shell1.Execute(string.Concat("git commit -m ", message));
        r += System.Environment.NewLine + _shell1.Execute(string.Concat("git push"));
        return r;
    }
}

#endregion