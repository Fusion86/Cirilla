using Cirilla.Core.Models;
using Cirilla.Models;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace Cirilla.ViewModels
{
    public class AppearanceViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public AppearancePossibleValues PossibleValues { get; }

        private SaveSlot _saveSlot;

        // Can't bind to Appearance field directly because it's a struct, thats why we have the ISaveSlotAppearanceMethods
        public AppearanceViewModel(SaveSlot saveSlot)
        {
            _saveSlot = saveSlot;

            PossibleValues = _saveSlot.Appearance.Gender == Gender.Female ? AppearancePossibleValues.Female : AppearancePossibleValues.Male;
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

        public Color HairColor
        {
            get => _saveSlot.Appearance.HairColor;
            set => _saveSlot.Appearance.HairColor = value;
        }

        public CharacterObjectTypeWithRect EyebrowType
        {
            get => PossibleValues.EyebrowTypes.FirstOrDefault(x => x.Value == _saveSlot.Appearance.EyebrowType);
            set => _saveSlot.Appearance.EyebrowType = (byte)value.Value;
        }

        public CharacterObjectTypeWithRect BrowType
        {
            get => PossibleValues.BrowTypes.FirstOrDefault(x => x.Value == _saveSlot.Appearance.BrowType);
            set => _saveSlot.Appearance.BrowType = (byte)value.Value;
        }

        public Color EyebrowColor
        {
            get => _saveSlot.Appearance.EyebrowColor;
            set => _saveSlot.Appearance.EyebrowColor = value;
        }
    }
}
