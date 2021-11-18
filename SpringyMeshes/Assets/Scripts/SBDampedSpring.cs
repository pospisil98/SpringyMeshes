using System.Net;

public class SBSDampedSpring
{
    private float length;
    
    /// <summary> Strength constant </summary>
    private float k;
    
    /// <summary> Damping constant </summary>
    private float d;

    private SBNode start;
    private SBNode end;
}
