using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MapCellEditor
{
    public static class CellType2DArrayExtension
    {
        // code from
        // https://rosettacode.org/wiki/Bitmap/Midpoint_circle_algorithm#C.23k
        //
        public static void DrawCircleInside(this CellType[,] array, int centerX, int centerY, int radius, CellType fulfillValue)
        {
            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            do
            {
                // ensure index is in range before setting (depends on your image implementation)
                // in this case we check if the pixel location is within the bounds of the image before setting the pixel
                if (centerX + x >= 0 && centerX + x <= array.GetLength(0) - 1 && centerY + y >= 0 && centerY + y <= array.GetLength(1) - 1 &&
                    centerX + x >= 0 && centerX + x <= array.GetLength(0) - 1 && centerY - y >= 0 && centerY - y <= array.GetLength(1) - 1)
                {
                    DrawLine(array, centerX + x, centerY + y, centerX + x, centerY - y, fulfillValue);
                }

                if (centerX - x >= 0 && centerX - x <= array.GetLength(0) - 1 && centerY + y >= 0 && centerY + y <= array.GetLength(1) - 1 &&
                    centerX - x >= 0 && centerX - x <= array.GetLength(0) - 1 && centerY - y >= 0 && centerY - y <= array.GetLength(1) - 1)
                {
                    DrawLine(array, centerX - x, centerY - y, centerX - x, centerY + y, fulfillValue);
                }

                if (centerX + y >= 0 && centerX + y <= array.GetLength(0) - 1 && centerY + x >= 0 && centerY + x <= array.GetLength(1) - 1 &&
                    centerX + y >= 0 && centerX + y <= array.GetLength(0) - 1 && centerY - x >= 0 && centerY - x <= array.GetLength(1) - 1)
                {
                    DrawLine(array, centerX + y, centerY + x, centerX + y, centerY - x, fulfillValue);
                }

                if (centerX - y >= 0 && centerX - y <= array.GetLength(0) - 1 && centerY + x >= 0 && centerY + x <= array.GetLength(1) - 1 &&
                    centerX - y >= 0 && centerX - y <= array.GetLength(0) - 1 && centerY - x >= 0 && centerY - x <= array.GetLength(1) - 1)
                {
                    DrawLine(array, centerX - y, centerY + x, centerX - y, centerY - x, fulfillValue);
                }

                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            } while (x <= y);
        }

        // code from
        // https://rosettacode.org/wiki/Bitmap/Bresenham%27s_line_algorithm#C.23
        public static void DrawLine(CellType[,] array, int x0, int y0, int x1, int y1, CellType fulfillValue)
        {
            int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                array[x0, y0] = fulfillValue;
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }

        public static CellType[] GetCircleInside(this CellType[,] array, int centerX, int centerY, int radius)
        {
            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            List<CellType> output = new List<CellType>();

            do
            {
                // ensure index is in range before setting (depends on your image implementation)
                // in this case we check if the pixel location is within the bounds of the image before setting the pixel
                if (centerX + x >= 0 && centerX + x <= array.GetLength(0) - 1 && centerY + y >= 0 && centerY + y <= array.GetLength(1) - 1 &&
                    centerX + x >= 0 && centerX + x <= array.GetLength(0) - 1 && centerY - y >= 0 && centerY - y <= array.GetLength(1) - 1)
                {
                    GetLine(array, centerX + x, centerY + y, centerX + x, centerY - y, ref output);
                }

                if (centerX - x >= 0 && centerX - x <= array.GetLength(0) - 1 && centerY + y >= 0 && centerY + y <= array.GetLength(1) - 1 &&
                    centerX - x >= 0 && centerX - x <= array.GetLength(0) - 1 && centerY - y >= 0 && centerY - y <= array.GetLength(1) - 1)
                {
                    GetLine(array, centerX - x, centerY - y, centerX - x, centerY + y, ref output);
                }

                if (centerX + y >= 0 && centerX + y <= array.GetLength(0) - 1 && centerY + x >= 0 && centerY + x <= array.GetLength(1) - 1 &&
                    centerX + y >= 0 && centerX + y <= array.GetLength(0) - 1 && centerY - x >= 0 && centerY - x <= array.GetLength(1) - 1)
                {
                    GetLine(array, centerX + y, centerY + x, centerX + y, centerY - x, ref output);
                }

                if (centerX - y >= 0 && centerX - y <= array.GetLength(0) - 1 && centerY + x >= 0 && centerY + x <= array.GetLength(1) - 1 &&
                    centerX - y >= 0 && centerX - y <= array.GetLength(0) - 1 && centerY - x >= 0 && centerY - x <= array.GetLength(1) - 1)
                {
                    GetLine(array, centerX - y, centerY + x, centerX - y, centerY - x, ref output);
                }

                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    d += 2 * (x - y) + 1;
                    y--;
                }
                x++;
            } while (x <= y);

            return output.ToArray();
        }

        // code from
        // https://rosettacode.org/wiki/Bitmap/Bresenham%27s_line_algorithm#C.23
        public static void GetLine(CellType[,] array, int x0, int y0, int x1, int y1, ref List<CellType> output)
        {
            int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                output.Add(array[x0, y0]);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }
    }
}
