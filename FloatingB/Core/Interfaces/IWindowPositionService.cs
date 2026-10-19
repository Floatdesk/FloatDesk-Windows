using System.Windows;
using FloatingB.Core.Models;

namespace FloatingB.Core.Interfaces
{
    public interface IWindowPositionService
    {
        void PositionAtEdge(Window window,ScreenEdge edge);

        ScreenEdge GetNearestEdge(Window window);
    }
}