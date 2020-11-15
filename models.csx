#!/usr/bin/env dotnet-script

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