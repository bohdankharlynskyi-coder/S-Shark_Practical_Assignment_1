namespace ConsoleApp2;

public interface IPolygon
{
    int VertexCount { get; }
    double[,] Vertices { get; set; }
    double CalculateArea();
    string DeterminePolygonType();
    bool IsShape(string shapeName);
    bool IsConvex();
}
