using FloatingB.Core.Models;

namespace FloatingB.Core.Interfaces
{
    public interface ISettingsService
    {
        AppSettings Load();

        void Save(AppSettings settings);
    }
}
