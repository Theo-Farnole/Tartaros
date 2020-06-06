using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FogOfWar
{
    public class FogMap
    {
        #region Fields
        private int _size;
        private FogState[] _array;
        #endregion

        #region Properties
        public int Size { get => _size; }
        #endregion

        #region ctor
        public FogMap(int size)
        {
            _size = size;

            _array = new FogState[_size * _size];

            for (int i =0; i < _array.Length; i++)
            {
                _array[i] = FogState.NotVisible;
            }
        }
        #endregion

        #region Methods
        #region Public Methods
        public int GetIndex(int x, int y)
        {
            return x + y * _size;
        }

        public FogState GetValue(int x, int y)
        {
            return _array[GetIndex(x, y)];
        }

        public void SetIndex(int x, int y, FogState value)
        {
            _array[GetIndex(x, y)] = value;
        }

        public void SetVisibleAsRevealed()
        {
            int length = _array.Length;

            for (int i = 0; i < length; i++)
            {
                if (_array[i] == FogState.Visible)
                {
                    _array[i] = FogState.Revealed;
                }
            }
        }

        public void DrawViewersVision(List<IFogVision> viewers, SnapGridDatabase snapGrid)
        {
            for (int i = 0; i < viewers.Count; i++)
            {
                Vector2Int viewersCoords = snapGrid.GetNearestCoords(viewers[i].Position);
                int relativeViewRadius = Mathf.RoundToInt(viewers[i].ViewRadius / snapGrid.CellSize);

                DrawCircleInside(viewersCoords.x, viewersCoords.y, relativeViewRadius, FogState.Visible);
            }
        }
        #endregion

        #region Private Methods
        private void DrawCircleInside(int centerX, int centerY, int radius, FogState fulfillValue)
        {
            int d = (5 - radius * 4) / 4;
            int x = 0;
            int y = radius;

            int lengthOne = _size;
            int lengthTwo = _size;

            do
            {
                // ensure index is in range before setting (depends on your image implementation)
                // in this case we check if the pixel location is within the bounds of the image before setting the pixel
                if (centerX + x >= 0 && centerX + x <= lengthOne - 1 &&
                    centerY + y >= 0 && centerY + y <= lengthTwo - 1 && centerY - y >= 0 && centerY - y <= lengthTwo - 1)
                {
                    DrawLine(centerX + x, centerY + y, centerX + x, centerY - y, fulfillValue);
                }

                if (centerX - x >= 0 && centerX - x <= lengthOne - 1 &&
                    centerY + y >= 0 && centerY + y <= lengthTwo - 1 && centerY - y >= 0 && centerY - y <= lengthTwo - 1)
                {
                    DrawLine(centerX - x, centerY - y, centerX - x, centerY + y, fulfillValue);
                }

                if (centerX + y >= 0 && centerX + y <= lengthOne - 1 &&
                    centerY + x >= 0 && centerY + x <= lengthTwo - 1 && centerY - x >= 0 && centerY - x <= lengthTwo - 1)
                {
                    DrawLine(centerX + y, centerY + x, centerX + y, centerY - x, fulfillValue);
                }

                if (centerX - y >= 0 && centerX - y <= lengthOne - 1 &&
                    centerY + x >= 0 && centerY + x <= lengthTwo - 1 && centerY - x >= 0 && centerY - x <= lengthTwo - 1)
                {
                    DrawLine(centerX - y, centerY + x, centerX - y, centerY - x, fulfillValue);
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

        private void DrawLine(int x0, int y0, int x1, int y1, FogState fulfillValue)
        {
            int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                SetIndex(x0, y0, fulfillValue);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }
        #endregion
        #endregion
    }
}
