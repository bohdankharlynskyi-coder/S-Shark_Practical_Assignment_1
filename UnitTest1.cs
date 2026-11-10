using System;
using ConsoleApp2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolygonTests;

[TestClass]
public class UnitTest
{
    private const double Delta = 1e-5;

    [TestMethod]
    public void CalculateArea_RightTriangleWithLegsFourAndThree_ReturnsSix()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 4.0, 0.0 },
            { 0.0, 3.0 }
        };
        IPolygon polygon = new Polygon(vertices);
        double expectedArea = 6.0;

        double actualArea = polygon.CalculateArea();

        Assert.AreEqual(expectedArea, actualArea, Delta);
    }

    [TestMethod]
    public void CalculateArea_SquareCreatedFromOneDimensionalArray_ReturnsTwentyFive()
    {
        double[] flatCoordinates = new double[] { 0.0, 0.0, 5.0, 0.0, 5.0, 5.0, 0.0, 5.0 };
        Polygon polygon = new Polygon(flatCoordinates);
        double expectedArea = 25.0;

        double actualArea = polygon.CalculateArea();

        Assert.AreEqual(4, polygon.VertexCount);
        Assert.AreEqual(expectedArea, actualArea, Delta);
    }

    [TestMethod]
    public void CalculateArea_ConcavePentagon_ReturnsTwelve()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 4.0, 0.0 },
            { 4.0, 4.0 },
            { 2.0, 2.0 },
            { 0.0, 4.0 }
        };
        Polygon polygon = new Polygon(5, vertices);
        double expectedArea = 12.0;

        double actualArea = polygon.CalculateArea();

        Assert.AreEqual(expectedArea, actualArea, Delta);
    }

    [TestMethod]
    public void CalculateArea_ClockwisePolygonWithNegativeCoordinates_ReturnsTwelve()
    {
        double[,] vertices = new double[,]
        {
            { -5.0, -2.0 },
            { -5.0, 1.0 },
            { -1.0, 1.0 },
            { -1.0, -2.0 }
        };
        Polygon original = new Polygon(vertices);
        Polygon copy = new Polygon(original);
        double expectedArea = 12.0;

        double actualArea = copy.CalculateArea();

        Assert.IsTrue(original.Equals(copy));
        Assert.AreEqual(expectedArea, actualArea, Delta);
    }

    [TestMethod]
    public void CalculateArea_LessThanThreeVertices_ThrowsInvalidOperationException()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 3.0, 4.0 }
        };
        Polygon polygon = new Polygon(2, vertices);

        Assert.ThrowsExactly<InvalidOperationException>(() => polygon.CalculateArea());
    }

    [TestMethod]
    public void CalculateArea_CollinearVertices_ThrowsInvalidOperationException()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 2.0, 2.0 },
            { 5.0, 5.0 }
        };
        Polygon polygon = new Polygon(vertices);

        Assert.ThrowsExactly<InvalidOperationException>(() => polygon.CalculateArea());
    }

    [TestMethod]
    public void CalculateArea_SelfIntersectingPolygon_ThrowsInvalidOperationException()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 4.0, 4.0 },
            { 4.0, 0.0 },
            { 0.0, 4.0 }
        };
        Polygon polygon = new Polygon(vertices);

        Assert.ThrowsExactly<InvalidOperationException>(() => polygon.CalculateArea());
    }

    [TestMethod]
    public void DeterminePolygonType_EquilateralTriangle_ReturnsRegularTriangle()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 2.0, 0.0 },
            { 1.0, Math.Sqrt(3.0) }
        };
        Polygon polygon = new Polygon(vertices);
        string expectedType = "Правильний трикутник";

        string actualType = polygon.DeterminePolygonType();

        Assert.AreEqual(expectedType, actualType);
    }

    [TestMethod]
    public void DeterminePolygonType_RightTriangle_ReturnsRightTriangle()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 3.0, 0.0 },
            { 0.0, 4.0 }
        };
        Polygon polygon = new Polygon(vertices);
        string expectedType = "Прямокутний трикутник";

        string actualType = polygon.DeterminePolygonType();

        Assert.AreEqual(expectedType, actualType);
    }

    [TestMethod]
    public void DeterminePolygonType_IsoscelesTriangle_ReturnsIsoscelesTriangle()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 4.0, 0.0 },
            { 2.0, 3.0 }
        };
        Polygon polygon = new Polygon(vertices);
        string expectedType = "Рівнобедрений трикутник";

        string actualType = polygon.DeterminePolygonType();

        Assert.AreEqual(expectedType, actualType);
    }

    [TestMethod]
    public void DeterminePolygonType_Square_ReturnsSquare()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 3.0, 0.0 },
            { 3.0, 3.0 },
            { 0.0, 3.0 }
        };
        Polygon polygon = new Polygon(vertices);
        string expectedType = "Квадрат";

        string actualType = polygon.DeterminePolygonType();

        Assert.AreEqual(expectedType, actualType);
    }

    [TestMethod]
    public void DeterminePolygonType_RectangleWithDifferentAdjacentSides_ReturnsRectangle()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 6.0, 0.0 },
            { 6.0, 2.0 },
            { 0.0, 2.0 }
        };
        Polygon polygon = new Polygon(vertices);
        string expectedType = "Прямокутник";

        string actualType = polygon.DeterminePolygonType();

        Assert.AreEqual(expectedType, actualType);
    }

    [TestMethod]
    public void DeterminePolygonType_RhombusWithDiagonalsSixAndEight_ReturnsRhombus()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 3.0 },
            { 4.0, 0.0 },
            { 0.0, -3.0 },
            { -4.0, 0.0 }
        };
        Polygon polygon = new Polygon(vertices);
        string expectedType = "Ромб";

        string actualType = polygon.DeterminePolygonType();

        Assert.AreEqual(expectedType, actualType);
    }

    [TestMethod]
    public void DeterminePolygonType_RegularHexagon_ReturnsRegularSixGon()
    {
        double[,] vertices = new double[6, 2];
        for (int i = 0; i < 6; i++)
        {
            double angle = i * Math.PI / 3.0;
            vertices[i, 0] = 2.0 * Math.Cos(angle);
            vertices[i, 1] = 2.0 * Math.Sin(angle);
        }
        Polygon polygon = new Polygon(vertices);
        string expectedType = "Правильний 6-кутник";

        string actualType = polygon.DeterminePolygonType();

        Assert.AreEqual(expectedType, actualType);
    }

    [TestMethod]
    public void DeterminePolygonType_CoincidingVertices_ThrowsInvalidOperationException()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 4.0, 0.0 },
            { 4.0, 0.0 },
            { 0.0, 3.0 }
        };
        Polygon polygon = new Polygon(vertices);

        Assert.ThrowsExactly<InvalidOperationException>(() => polygon.DeterminePolygonType());
    }

    [TestMethod]
    public void IsShape_SquareCheckedAsRectangle_ReturnsTrue()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 2.0, 0.0 },
            { 2.0, 2.0 },
            { 0.0, 2.0 }
        };
        Polygon polygon = new Polygon(vertices);

        bool result = polygon.IsShape("прямокутник");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsShape_NonSquareRectangleCheckedAsSquare_ReturnsFalse()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 5.0, 0.0 },
            { 5.0, 2.0 },
            { 0.0, 2.0 }
        };
        Polygon polygon = new Polygon(vertices);

        bool result = polygon.IsShape("квадрат");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsShape_TrapezoidCheckedAsTrapezoid_ReturnsTrue()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 6.0, 0.0 },
            { 4.0, 3.0 },
            { 2.0, 3.0 }
        };
        Polygon polygon = new Polygon(vertices);

        bool result = polygon.IsShape("трапеція");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsShape_EmptyShapeName_ThrowsArgumentException()
    {
        Polygon polygon = new Polygon();

        Assert.ThrowsExactly<ArgumentException>(() => polygon.IsShape(""));
    }

    [TestMethod]
    public void IsShape_UnknownShapeName_ThrowsArgumentException()
    {
        Polygon polygon = new Polygon();

        Assert.ThrowsExactly<ArgumentException>(() => polygon.IsShape("гіперкуб"));
    }

    [TestMethod]
    public void IsConvex_ConvexPentagon_ReturnsTrue()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 4.0, 0.0 },
            { 5.0, 3.0 },
            { 2.0, 5.0 },
            { -1.0, 3.0 }
        };
        Polygon polygon = new Polygon(vertices);

        bool isConvex = polygon.IsConvex();

        Assert.IsTrue(isConvex);
    }

    [TestMethod]
    public void IsConvex_ConcavePentagon_ReturnsFalse()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 4.0, 0.0 },
            { 4.0, 4.0 },
            { 2.0, 1.0 },
            { 0.0, 4.0 }
        };
        Polygon polygon = new Polygon();
        polygon.Vertices = vertices;

        bool isConvex = polygon.IsConvex();

        Assert.IsFalse(isConvex);
    }

    [TestMethod]
    public void IsConvex_DefaultConstructedTriangle_ReturnsTrue()
    {
        Polygon polygon = new Polygon();
        string expectedString = "Polygon (Vertices = 3): [(0; 0), (1; 0), (0; 1)]";

        bool isConvex = polygon.IsConvex();
        string actualString = polygon.ToString();

        Assert.AreEqual(expectedString, actualString);
        Assert.IsTrue(isConvex);
    }

    [TestMethod]
    public void IsConvex_SelfIntersectingQuadrilateral_ThrowsInvalidOperationException()
    {
        double[,] vertices = new double[,]
        {
            { 0.0, 0.0 },
            { 3.0, 3.0 },
            { 3.0, 0.0 },
            { 0.0, 3.0 }
        };
        Polygon polygon = new Polygon(vertices);

        Assert.ThrowsExactly<InvalidOperationException>(() => polygon.IsConvex());
    }
}
