using System;
using System.Globalization;
using System.Text;

namespace ConsoleApp2;

public class Polygon : IPolygon
{
    private const double Epsilon = 1e-6;

    private int _vertexCount;
    private double[,] _vertices;

    public int VertexCount => _vertexCount;

    public double[,] Vertices
    {
        get
        {
            double[,] copy = new double[_vertexCount, 2];
            for (int i = 0; i < _vertexCount; i++)
            {
                copy[i, 0] = _vertices[i, 0];
                copy[i, 1] = _vertices[i, 1];
            }
            return copy;
        }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Масив координат вершин не може бути null.");
            }
            if (value.GetLength(1) != 2)
            {
                throw new ArgumentException("Другий вимір масиву координат повинен дорівнювати 2 (X та Y).", nameof(value));
            }

            _vertexCount = value.GetLength(0);
            _vertices = new double[_vertexCount, 2];
            for (int i = 0; i < _vertexCount; i++)
            {
                _vertices[i, 0] = value[i, 0];
                _vertices[i, 1] = value[i, 1];
            }
        }
    }

    public Polygon()
    {
        _vertexCount = 3;
        _vertices = new double[3, 2]
        {
            { 0.0, 0.0 },
            { 1.0, 0.0 },
            { 0.0, 1.0 }
        };
    }

    public Polygon(double[,] vertices)
    {
        _vertices = new double[0, 2];
        Vertices = vertices;
    }

    public Polygon(int vertexCount, double[,] vertices)
    {
        if (vertices == null)
        {
            throw new ArgumentNullException(nameof(vertices), "Масив координат не може бути null.");
        }
        if (vertexCount < 0 || vertexCount > vertices.GetLength(0))
        {
            throw new ArgumentException("Некоректна кількість вершин відносно розміру масиву.", nameof(vertexCount));
        }
        if (vertices.GetLength(1) != 2)
        {
            throw new ArgumentException("Другий вимір масиву повинен дорівнювати 2 (координати X та Y).", nameof(vertices));
        }

        _vertexCount = vertexCount;
        _vertices = new double[_vertexCount, 2];
        for (int i = 0; i < _vertexCount; i++)
        {
            _vertices[i, 0] = vertices[i, 0];
            _vertices[i, 1] = vertices[i, 1];
        }
    }

    public Polygon(double[] flatCoordinates)
    {
        if (flatCoordinates == null)
        {
            throw new ArgumentNullException(nameof(flatCoordinates), "Масив координат не може бути null.");
        }
        if (flatCoordinates.Length % 2 != 0)
        {
            throw new ArgumentException("Кількість елементів одновимірного масиву координат має бути парною.", nameof(flatCoordinates));
        }

        _vertexCount = flatCoordinates.Length / 2;
        _vertices = new double[_vertexCount, 2];
        for (int i = 0; i < _vertexCount; i++)
        {
            _vertices[i, 0] = flatCoordinates[2 * i];
            _vertices[i, 1] = flatCoordinates[2 * i + 1];
        }
    }

    public Polygon(Polygon other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other), "Об'єкт для копіювання не може бути null.");
        }

        _vertexCount = other._vertexCount;
        _vertices = new double[_vertexCount, 2];
        for (int i = 0; i < _vertexCount; i++)
        {
            _vertices[i, 0] = other._vertices[i, 0];
            _vertices[i, 1] = other._vertices[i, 1];
        }
    }

    public double CalculateArea()
    {
        ValidatePolygon();
        return ComputeRawShoelaceArea();
    }

    public string DeterminePolygonType()
    {
        ValidatePolygon();

        if (_vertexCount == 3)
        {
            if (AreAllSidesEqual())
            {
                return "Правильний трикутник";
            }
            if (HasRightAngleInTriangle())
            {
                return "Прямокутний трикутник";
            }
            if (HasAtLeastTwoEqualSidesInTriangle())
            {
                return "Рівнобедрений трикутник";
            }
            return "Довільний трикутник";
        }

        if (_vertexCount == 4)
        {
            bool allSidesEqual = AreAllSidesEqual();
            bool allAnglesRight = AreAllAnglesRight();

            if (allSidesEqual && allAnglesRight)
            {
                return "Квадрат";
            }
            if (allAnglesRight)
            {
                return "Прямокутник";
            }
            if (allSidesEqual)
            {
                return "Ромб";
            }
            if (IsParallelogram())
            {
                return "Паралелограм";
            }
            if (IsTrapezoid())
            {
                return "Трапеція";
            }
            return IsConvex() ? "Опуклий чотирикутник" : "Неопуклий чотирикутник";
        }

        if (IsRegularPolygon())
        {
            return $"Правильний {_vertexCount}-кутник";
        }

        return IsConvex()
            ? $"Опуклий {_vertexCount}-кутник"
            : $"Неопуклий {_vertexCount}-кутник";
    }

    public bool IsShape(string shapeName)
    {
        if (string.IsNullOrWhiteSpace(shapeName))
        {
            throw new ArgumentException("Назва фігури не може бути порожньою або складатися лише з пробілів.", nameof(shapeName));
        }

        ValidatePolygon();

        string normalized = shapeName.Trim().ToLowerInvariant();
        switch (normalized)
        {
            case "трикутник":
                return _vertexCount == 3;
            case "правильний трикутник":
            case "рівносторонній трикутник":
                return _vertexCount == 3 && AreAllSidesEqual();
            case "прямокутний трикутник":
                return _vertexCount == 3 && HasRightAngleInTriangle();
            case "рівнобедрений трикутник":
                return _vertexCount == 3 && HasAtLeastTwoEqualSidesInTriangle();
            case "чотирикутник":
                return _vertexCount == 4;
            case "квадрат":
                return _vertexCount == 4 && AreAllSidesEqual() && AreAllAnglesRight();
            case "прямокутник":
                return _vertexCount == 4 && AreAllAnglesRight();
            case "ромб":
                return _vertexCount == 4 && AreAllSidesEqual();
            case "паралелограм":
                return _vertexCount == 4 && IsParallelogram();
            case "трапеція":
                return _vertexCount == 4 && IsTrapezoid();
            case "опуклий":
            case "опуклий многокутник":
                return IsConvex();
            case "неопуклий":
            case "неопуклий многокутник":
                return !IsConvex();
            case "правильний":
            case "правильний многокутник":
                return IsRegularPolygon();
            default:
                throw new ArgumentException($"Невідома назва фігури на площині: '{shapeName}'.", nameof(shapeName));
        }
    }

    public bool IsConvex()
    {
        ValidatePolygon();

        bool hasPositive = false;
        bool hasNegative = false;

        for (int i = 0; i < _vertexCount; i++)
        {
            int next = (i + 1) % _vertexCount;
            int nextNext = (i + 2) % _vertexCount;

            double cross = CrossProduct(
                _vertices[i, 0], _vertices[i, 1],
                _vertices[next, 0], _vertices[next, 1],
                _vertices[nextNext, 0], _vertices[nextNext, 1]);

            if (cross > Epsilon)
            {
                hasPositive = true;
            }
            else if (cross < -Epsilon)
            {
                hasNegative = true;
            }

            if (hasPositive && hasNegative)
            {
                return false;
            }
        }

        return hasPositive || hasNegative;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj is not Polygon other)
        {
            return false;
        }
        if (_vertexCount != other._vertexCount)
        {
            return false;
        }

        for (int i = 0; i < _vertexCount; i++)
        {
            if (Math.Abs(_vertices[i, 0] - other._vertices[i, 0]) > Epsilon ||
                Math.Abs(_vertices[i, 1] - other._vertices[i, 1]) > Epsilon)
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hash = _vertexCount.GetHashCode();
        for (int i = 0; i < _vertexCount; i++)
        {
            hash = HashCode.Combine(
                hash,
                Math.Round(_vertices[i, 0], 4),
                Math.Round(_vertices[i, 1], 4));
        }
        return hash;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(CultureInfo.InvariantCulture, "Polygon (Vertices = {0}): [", _vertexCount);
        for (int i = 0; i < _vertexCount; i++)
        {
            if (i > 0)
            {
                sb.Append(", ");
            }
            sb.AppendFormat(
                CultureInfo.InvariantCulture,
                "({0:0.##}; {1:0.##})",
                _vertices[i, 0],
                _vertices[i, 1]);
        }
        sb.Append(']');
        return sb.ToString();
    }

    private void ValidatePolygon()
    {
        if (_vertexCount < 3)
        {
            throw new InvalidOperationException("Многокутник повинен мати щонайменше 3 вершини.");
        }

        for (int i = 0; i < _vertexCount; i++)
        {
            for (int j = i + 1; j < _vertexCount; j++)
            {
                if (GetDistance(i, j) <= Epsilon)
                {
                    throw new InvalidOperationException("Многокутник містить співпадаючі вершини.");
                }
            }
        }

        if (ComputeRawShoelaceArea() <= Epsilon)
        {
            throw new InvalidOperationException("Многокутник є виродженим (вершини лежать на одній прямій).");
        }

        for (int i = 0; i < _vertexCount; i++)
        {
            int next = (i + 1) % _vertexCount;
            int nextNext = (i + 2) % _vertexCount;
            double cross = CrossProduct(
                _vertices[i, 0], _vertices[i, 1],
                _vertices[next, 0], _vertices[next, 1],
                _vertices[nextNext, 0], _vertices[nextNext, 1]);

            if (Math.Abs(cross) <= Epsilon)
            {
                throw new InvalidOperationException("Три послідовні вершини многокутника лежать на одній прямій.");
            }
        }

        if (HasSelfIntersection())
        {
            throw new InvalidOperationException("Контур многокутника має самоперетин.");
        }
    }

    private double ComputeRawShoelaceArea()
    {
        double sum = 0.0;
        for (int i = 0; i < _vertexCount; i++)
        {
            int next = (i + 1) % _vertexCount;
            sum += (_vertices[i, 0] * _vertices[next, 1]) - (_vertices[next, 0] * _vertices[i, 1]);
        }
        return 0.5 * Math.Abs(sum);
    }

    private double GetDistance(int indexA, int indexB)
    {
        double dx = _vertices[indexB, 0] - _vertices[indexA, 0];
        double dy = _vertices[indexB, 1] - _vertices[indexA, 1];
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private double[] GetSideLengths()
    {
        double[] sides = new double[_vertexCount];
        for (int i = 0; i < _vertexCount; i++)
        {
            int next = (i + 1) % _vertexCount;
            sides[i] = GetDistance(i, next);
        }
        return sides;
    }

    private bool AreAllSidesEqual()
    {
        double[] sides = GetSideLengths();
        for (int i = 1; i < _vertexCount; i++)
        {
            if (Math.Abs(sides[i] - sides[0]) > Epsilon)
            {
                return false;
            }
        }
        return true;
    }

    private bool HasAtLeastTwoEqualSidesInTriangle()
    {
        double[] s = GetSideLengths();
        return Math.Abs(s[0] - s[1]) <= Epsilon ||
               Math.Abs(s[1] - s[2]) <= Epsilon ||
               Math.Abs(s[0] - s[2]) <= Epsilon;
    }

    private bool HasRightAngleInTriangle()
    {
        for (int i = 0; i < 3; i++)
        {
            if (IsRightAngleAtVertex(i))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsRightAngleAtVertex(int vertexIndex)
    {
        int prev = (vertexIndex - 1 + _vertexCount) % _vertexCount;
        int next = (vertexIndex + 1) % _vertexCount;

        double ux = _vertices[prev, 0] - _vertices[vertexIndex, 0];
        double uy = _vertices[prev, 1] - _vertices[vertexIndex, 1];
        double vx = _vertices[next, 0] - _vertices[vertexIndex, 0];
        double vy = _vertices[next, 1] - _vertices[vertexIndex, 1];

        double dot = ux * vx + uy * vy;
        return Math.Abs(dot) <= Epsilon;
    }

    private bool AreAllAnglesRight()
    {
        for (int i = 0; i < _vertexCount; i++)
        {
            if (!IsRightAngleAtVertex(i))
            {
                return false;
            }
        }
        return true;
    }

    private bool AreEdgesParallel(int edgeIndex1, int edgeIndex2)
    {
        int next1 = (edgeIndex1 + 1) % _vertexCount;
        int next2 = (edgeIndex2 + 1) % _vertexCount;

        double dx1 = _vertices[next1, 0] - _vertices[edgeIndex1, 0];
        double dy1 = _vertices[next1, 1] - _vertices[edgeIndex1, 1];
        double dx2 = _vertices[next2, 0] - _vertices[edgeIndex2, 0];
        double dy2 = _vertices[next2, 1] - _vertices[edgeIndex2, 1];

        double cross = dx1 * dy2 - dy1 * dx2;
        return Math.Abs(cross) <= Epsilon;
    }

    private bool IsParallelogram()
    {
        return _vertexCount == 4 && AreEdgesParallel(0, 2) && AreEdgesParallel(1, 3);
    }

    private bool IsTrapezoid()
    {
        if (_vertexCount != 4)
        {
            return false;
        }
        bool pair1 = AreEdgesParallel(0, 2);
        bool pair2 = AreEdgesParallel(1, 3);
        return (pair1 && !pair2) || (!pair1 && pair2);
    }

    private bool IsRegularPolygon()
    {
        if (!IsConvex() || !AreAllSidesEqual())
        {
            return false;
        }

        double firstCos = GetAngleCosineAtVertex(0);
        for (int i = 1; i < _vertexCount; i++)
        {
            if (Math.Abs(GetAngleCosineAtVertex(i) - firstCos) > Epsilon)
            {
                return false;
            }
        }
        return true;
    }

    private double GetAngleCosineAtVertex(int vertexIndex)
    {
        int prev = (vertexIndex - 1 + _vertexCount) % _vertexCount;
        int next = (vertexIndex + 1) % _vertexCount;

        double ux = _vertices[prev, 0] - _vertices[vertexIndex, 0];
        double uy = _vertices[prev, 1] - _vertices[vertexIndex, 1];
        double vx = _vertices[next, 0] - _vertices[vertexIndex, 0];
        double vy = _vertices[next, 1] - _vertices[vertexIndex, 1];

        double lenU = Math.Sqrt(ux * ux + uy * uy);
        double lenV = Math.Sqrt(vx * vx + vy * vy);
        return (ux * vx + uy * vy) / (lenU * lenV);
    }

    private bool HasSelfIntersection()
    {
        if (_vertexCount < 4)
        {
            return false;
        }

        for (int i = 0; i < _vertexCount; i++)
        {
            int iNext = (i + 1) % _vertexCount;
            for (int j = i + 2; j < _vertexCount; j++)
            {
                if (i == 0 && j == _vertexCount - 1)
                {
                    continue;
                }

                int jNext = (j + 1) % _vertexCount;
                if (SegmentsIntersect(
                    _vertices[i, 0], _vertices[i, 1],
                    _vertices[iNext, 0], _vertices[iNext, 1],
                    _vertices[j, 0], _vertices[j, 1],
                    _vertices[jNext, 0], _vertices[jNext, 1]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static double CrossProduct(
        double ax, double ay,
        double bx, double by,
        double cx, double cy)
    {
        return (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
    }

    private static bool SegmentsIntersect(
        double ax, double ay, double bx, double by,
        double cx, double cy, double dx, double dy)
    {
        double d1 = CrossProduct(cx, cy, dx, dy, ax, ay);
        double d2 = CrossProduct(cx, cy, dx, dy, bx, by);
        double d3 = CrossProduct(ax, ay, bx, by, cx, cy);
        double d4 = CrossProduct(ax, ay, bx, by, dx, dy);

        if (((d1 > Epsilon && d2 < -Epsilon) || (d1 < -Epsilon && d2 > Epsilon)) &&
            ((d3 > Epsilon && d4 < -Epsilon) || (d3 < -Epsilon && d4 > Epsilon)))
        {
            return true;
        }

        if (Math.Abs(d1) <= Epsilon && OnSegment(cx, cy, dx, dy, ax, ay)) return true;
        if (Math.Abs(d2) <= Epsilon && OnSegment(cx, cy, dx, dy, bx, by)) return true;
        if (Math.Abs(d3) <= Epsilon && OnSegment(ax, ay, bx, by, cx, cy)) return true;
        if (Math.Abs(d4) <= Epsilon && OnSegment(ax, ay, bx, by, dx, dy)) return true;

        return false;
    }

    private static bool OnSegment(
        double px, double py,
        double qx, double qy,
        double rx, double ry)
    {
        return rx <= Math.Max(px, qx) + Epsilon &&
               rx >= Math.Min(px, qx) - Epsilon &&
               ry <= Math.Max(py, qy) + Epsilon &&
               ry >= Math.Min(py, qy) - Epsilon;
    }
}
