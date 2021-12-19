using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strut
{
    public float k;
    public float d;
    private float restLength;
    private float restAngle;
    public float kTheta;
    public float dTheta;

    public Vertex from;
    public Vertex to;

    public Face face1;
    public Face face2;

    public Vertex opposite1;
    public Vertex opposite2;


    public Strut(float k, float d, Vertex from, Vertex to)
    {
        this.k = k;
        this.d = d;

        this.from = from;
        this.to = to;

        this.restLength = Vector3.Distance(from.Position, to.Position);
        
        //TODO: fix
        this.kTheta = 0.0f;
        this.dTheta = 0.0f;
    }

    public void CalculateRestAngle()
    {
        this.restAngle = Vector3.Angle(face1.normal, face2.normal);
    }

    public bool IsSame(int id1, int id2)
    {
        return (from.id == id1 && to.id == id2) || (@from.id == id2 && to.id == id1);
    }

    public void ApplyForces()
    {
        float length = Vector3.Distance(from.Position, to.Position);

        Vector3 dir = (to.Position - from.Position).normalized;

        // apply spring forces
        Vector3 f_s = k * (length - restLength) * dir;
        from.AddForce(f_s);
        to.AddForce(-f_s);

        // apply damping force
        Vector3 f_d = d * Vector3.Dot((to.Velocity - from.Velocity), dir) * dir;
        from.AddForce(f_d);
        to.AddForce(-f_d);
        
        // TODO: computation of hinge forces
        
                                                                                                                                                                                                                
        //                                                                                       /-          
        //          -.                                                                           /o.         
        //         -s-                                                                         `. `/`        
        //         /` `                                                                        o.   :.       
        //        /. +.                                                                        :..   //.     
        //       --  +.`                                                                         .`-/ss:     
        //      .:    `.                                                   `                  `..:/+o.`/-    
        //     `/                                                         `::             `..----::.  `--..  
        //    /h/..`                          -o ``                     +/./+`        `..--`....:.       ``  
        // `/..:oy+-:---.````                 +/ //`                    -/-  `    `..--.```.``--`            
        // `:-- ./```........---..````       :- `+/-                      :.  ``.--.`  `..` --`              
        //    `  `:`    ``..``   `....---..`-:     ``           ``        `/:--.`   `..`  .:.                
        //        `/`       ``...``      ``:/..---.````        `+:`    `.--.:-    `..`  `:.                  
        //         `/`           ``..``    -      `....----.`````-:`.--.`    ` `..`   `:-                    
        //           /.              ``...``               `....:ho.`       `...    `--`                     
        //            :.                  `...`                 ::`       `..`     .:`                       
        //             :-                     `....`           --`      ..`      .:.                         
        //              --                   +.    `...`      .: `   `..       `:-                           
        //               -:                  :`.       `....``/  ``..``:`     :-                             
        //                .:                   `            .+  .-.   .:``  -:`                              
        //                 `/                               /...`-.     ` .:`                                
        //                  `/                             +-.:` `      `:.                                  
        //                    /`                          :. .s`      `:-                                    
        //                     /`                        :-   `.`    :-                                      
        //                      :.                      --         -:`                                       
        //                       :-                    .:        .:`                                         
        //                        --                  `/       `:.                                           
        //                         -:           `.   `/      `:-                                             
        //                          .:          +-` -y`     --`                                              
        //                           .:         o:/ +-    -:`                                                
        //                            `/`       ```:.   .:.                                                  
        //                             `:`        --  `:.                                                    
        //                              `:.      -- `--`                                                     
        //                                :.    .:`--`                                                       
        //                                 --  ./-:`                                                         
        //                                  --`o:.                                                           
        //                                   oh/``                                                           
        //                                   `o`.o.                                                          
        //                                    + ``.-`                                                        
        //                                `-` :.                                                             
        //                                -+  .:                                                             
        //                                `.-.`+`                                                            
        //                                  ` .h.                                                            
        //                                     `     
        
        Vector3 h = (to.Position - from.Position).normalized;
        Vector3 x02 = opposite1.Position - from.Position;
        Vector3 x03 = opposite2.Position - from.Position;
        
        // vectors formed by lofting a perpendicular from the hinge edge
        Vector3 r_l = x02 - Vector3.Dot(x02, h) * h;
        Vector3 r_r = x02 - Vector3.Dot(x02, h) * h;

        Vector3 n_l = face1.normal;
        Vector3 n_r = face2.normal;

        float theta = Mathf.Atan2(Vector3.Dot(n_l, n_r), Vector3.Dot(Vector3.Cross(n_l, n_r), h));

        Vector3 tau_k = kTheta * (theta - restAngle) * h;

        float theta_l = Vector3.Dot(opposite1.Velocity, n_l) / Vector3.Magnitude(r_l);
        float theta_r = Vector3.Dot(opposite2.Velocity, n_r) / Vector3.Magnitude(r_r);

        Vector3 tau_d = -dTheta * (theta_l + theta_r) * h;

        Vector3 tau = tau_k + tau_d;

        Vector3 f2 = n_l * Vector3.Dot(tau, h) / Vector3.Magnitude(r_l);
        Vector3 f3 = n_r * Vector3.Dot(tau, h) / Vector3.Magnitude(r_r);

        float d02 = Vector3.Dot(x02, h);
        float d03 = Vector3.Dot(x03, h);
        
        Vector3 f1 = -(d02 * f2 + d03 * f3)/ Vector3.Magnitude(to.Position - from.Position);
        Vector3 f0 = -(f1 + f2 + f3);
        
        from.AddForce(f0);
        to.AddForce(f1);
        opposite1.AddForce(f2);
        opposite2.AddForce(f3);

    }
}


