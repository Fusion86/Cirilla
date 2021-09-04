using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Text;

namespace ShoutoutReset.WPF
{
    public class ShoutoutViewModel : ViewModelBase
    {
        [Reactive] public string Value { get; set; } = "";

        private readonly int index;
        private readonly SaveSlot saveSlot;

        public ShoutoutViewModel(SaveSlot saveSlot, int shoutoutIndex)
        {
            this.saveSlot = saveSlot;
            index = shoutoutIndex;
            UpdateValue();
        }

        public void Reset()
        {
            var defaultValue = MhwDefaultValues.GetDefaultShoutoutValue(saveSlot.SaveSlotIndex);
            saveSlot.Native.Shoutouts[index].Length = 0;
            saveSlot.Native.Shoutouts[index].Value = defaultValue;
            UpdateValue();
        }

        private void UpdateValue()
        {
            var defaultValue = MhwDefaultValues.GetDefaultShoutoutValue(saveSlot.SaveSlotIndex);
            var value = saveSlot.Native.Shoutouts[index].Value;
            if (value.SequenceEqual(defaultValue))
            {
                Value = "(default value)";
            }
            else
            {
                var len = saveSlot.Native.Shoutouts[index].Length;
                Value = Encoding.UTF8.GetString(value.Take(len).ToArray());
            }
        }
    }
}
