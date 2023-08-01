using System.Collections.Generic;
using LiteDB;
using Unity.Collections;

namespace SkanksAIO.Models;

public class PlayerLocation
{

    public string? Name { get; set; }
    
    public float X { get; set; }
    
    public float Y { get; set; }
    
    public float Z { get; set; }

    public PlayerLocation() { }
    
    public PlayerLocation(string Name, float X, float Y, float Z)
    {
        this.Name = Name;
        this.X = X;
        this.Y = Y;
        this.Z = Z;
    }
}