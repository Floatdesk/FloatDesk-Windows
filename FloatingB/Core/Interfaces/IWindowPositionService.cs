using System.Windows;
using FloatingB.Core.Models;

namespace FloatingB.Core.Interfaces
{
    public interface IWindowPositionService
    {
        void PositionAtEdge(
            Window window,
            ScreenEdge edge);

        void SnapToNearestEdge(
            Window window);
    }
}