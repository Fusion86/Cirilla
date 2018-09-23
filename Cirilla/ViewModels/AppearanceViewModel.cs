using Cirilla.Core.Models;
using Cirilla.Models;
using System.ComponentModel;
using System.Linq;

namespace Cirilla.ViewModels
{
    public class AppearanceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AppearancePossibleValues PossibleValues { get; } = AppearancePossibleValues.Default;

        private SaveSlot _saveSlot;

        // Can't bind to Appearance field directly because it's a struct, thats why we have the ISaveSlotAppearanceMethods
        public AppearanceViewModel(SaveSlot saveSlot)
        {
            _saveSlot = saveSlot;
        }

        public CharacterObjectTypeWithRect FaceType
        {
            get => PossibleValues.FaceTypes.FirstOrDefault(x => x.Value == _saveSlot.Appearance.FaceType);
            set => _saveSlot.Appearance.FaceType = (byte)value.Value;
        }

        public CharacterObjectTypeWithRect HairType
        {
            get => PossibleValues.HairTypes.FirstOrDefault(x => x.Value == _saveSlot.Appearance.HairType);
            set => _saveSlot.Appearance.HairType = (short)value.Value;
        }
    }
}
