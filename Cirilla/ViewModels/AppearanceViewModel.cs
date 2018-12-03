using Cirilla.Core.Enums;
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

        // Can't bind to Appearance field directly because it's a struct, thats why we have the IAppearanceMethods
        public AppearanceViewModel(SaveSlot saveSlot)
        {
            _saveSlot = saveSlot;

            PossibleValues = _saveSlot.CharacterAppearance.Gender == Gender.Male ? AppearancePossibleValues.Male : AppearancePossibleValues.Female;
        }

        public CharacterObjectTypeWithRect FaceType
        {
            get => PossibleValues.FaceTypes.FirstOrDefault(x => x.Value == _saveSlot.CharacterAppearance.FaceType);
            set => _saveSlot.CharacterAppearance.FaceType = (byte)value.Value;
        }

        public CharacterObjectTypeWithRect HairType
        {
            get => PossibleValues.HairTypes.FirstOrDefault(x => x.Value == _saveSlot.CharacterAppearance.HairType);
            set => _saveSlot.CharacterAppearance.HairType = (short)value.Value;
        }

        public Color HairColor
        {
            get => _saveSlot.CharacterAppearance.HairColor;
            set => _saveSlot.CharacterAppearance.HairColor = value;
        }

        public CharacterObjectTypeWithRect EyebrowType
        {
            get => PossibleValues.EyebrowTypes.FirstOrDefault(x => x.Value == _saveSlot.CharacterAppearance.EyebrowType);
            set => _saveSlot.CharacterAppearance.EyebrowType = (byte)value.Value;
        }

        public CharacterObjectTypeWithRect BrowType
        {
            get => PossibleValues.BrowTypes.FirstOrDefault(x => x.Value == _saveSlot.CharacterAppearance.BrowType);
            set => _saveSlot.CharacterAppearance.BrowType = (byte)value.Value;
        }

        public Color EyebrowColor
        {
            get => _saveSlot.CharacterAppearance.EyebrowColor;
            set => _saveSlot.CharacterAppearance.EyebrowColor = value;
        }
    }
}
