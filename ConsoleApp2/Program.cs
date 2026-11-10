using System;
using System.Text;

namespace ConsoleApp2;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Лабораторна робота: Варіант 30 ===");
        Console.WriteLine("«Довільний многокутник у двовимірному просторі»\n");

        Polygon defaultTriangle = new Polygon();
        PrintPolygonInfo("1. Многокутник за замовчуванням", defaultTriangle);

        double[,] rightTriangleVertices = new double[,]
        {
            { 0.0, 0.0 },
            { 3.0, 0.0 },
            { 0.0, 4.0 }
        };
        Polygon rightTriangle = new Polygon(rightTriangleVertices);
        PrintPolygonInfo("2. Прямокутний трикутник (через 2D масив)", rightTriangle);

        double[] squareFlatCoords = new double[] { 0.0, 0.0, 5.0, 0.0, 5.0, 5.0, 0.0, 5.0 };
        Polygon square = new Polygon(squareFlatCoords);
        PrintPolygonInfo("3. Квадрат зі стороною 5 (через 1D масив)", square);
        Console.WriteLine($"   Чи є прямокутником (IsShape(\"прямокутник\")): {square.IsShape("прямокутник")}");
        Console.WriteLine($"   Чи є квадратом (IsShape(\"квадрат\")):         {square.IsShape("квадрат")}\n");

        double[,] concavePentagonVertices = new double[,]
        {
            { 0.0, 0.0 },
            { 4.0, 0.0 },
            { 4.0, 4.0 },
            { 2.0, 2.0 },
            { 0.0, 4.0 }
        };
        Polygon concavePentagon = new Polygon(5, concavePentagonVertices);
        PrintPolygonInfo("4. Неопуклий п'ятикутник", concavePentagon);

        Polygon copyOfSquare = new Polygon(square);
        Console.WriteLine("5. Перевірка конструктора копіювання та методу Equals:");
        Console.WriteLine($"   Оригінал: {square}");
        Console.WriteLine($"   Копія:    {copyOfSquare}");
        Console.WriteLine($"   square.Equals(copyOfSquare): {square.Equals(copyOfSquare)}");
        Console.WriteLine($"   square.Equals(rightTriangle): {square.Equals(rightTriangle)}\n");

        Console.WriteLine("6. Перевірка генерації винятку для колінеарних вершин:");
        try
        {
            Polygon degenerate = new Polygon(new double[,]
            {
                { 0.0, 0.0 },
                { 2.0, 2.0 },
                { 5.0, 5.0 }
            });
            Console.WriteLine($"   Площа: {degenerate.CalculateArea()}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"   Перехоплено очікуваний виняток InvalidOperationException: {ex.Message}");
        }
    }

    private static void PrintPolygonInfo(string title, IPolygon polygon)
    {
        Console.WriteLine(title);
        Console.WriteLine($"   ToString():             {polygon}");
        Console.WriteLine($"   Кількість вершин:       {polygon.VertexCount}");
        Console.WriteLine($"   Тип фігури:             {polygon.DeterminePolygonType()}");
        Console.WriteLine($"   Опуклий (IsConvex):     {polygon.IsConvex()}");
        Console.WriteLine($"   Площа (CalculateArea):  {polygon.CalculateArea():F2}\n");
    }
}
