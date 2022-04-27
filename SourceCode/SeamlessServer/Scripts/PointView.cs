using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointView : MonoBehaviour
{
    [SerializeField] Color firstColor = Color.black;
    [SerializeField] Color otherColor = Color.black;
    [SerializeField] float r = 0.8f;
    [SerializeField] DrawType type;
    [SerializeField] private Transform[] points;
    private void Awake()
    {
        points = GetComponentsInChildren<Transform>();
    }
    private void OnValidate()
    {
        points = GetComponentsInChildren<Transform>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = firstColor;
        Gizmos.DrawSphere(points[0].position, 1);
        Gizmos.color = otherColor;

        for (int i = 1; i < points.Length; i++)
        {

            var pos = points[i].position;
            switch (type)
            {
                case DrawType.Shpere:
                    Gizmos.DrawSphere(pos, r);
                    continue;
                case DrawType.WriteShpere:
                    Gizmos.DrawWireSphere(pos, r);
                    continue;
                case DrawType.Cube:
                    Gizmos.DrawCube(pos, Vector3.right * r);
                    continue;
                case DrawType.WriteCube:
                    Gizmos.DrawWireCube(pos, Vector3.right * r);
                    continue;
            }
        }
    }
}
public enum DrawType
{
    Shpere,
    WriteShpere,
    Cube,
    WriteCube
}
