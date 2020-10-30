using System;
using System.Collections.Generic;
using System.Text;

namespace Cirilla.MVVM.ViewModels
{
    public interface ISidebarItemViewModel : ITitledViewModel
    {
        bool CanClose { get; }

        /// <summary>
        /// Signal the view that you want to close it. Returns <see langword="true"/> when view has been closed.
        /// </summary>
        /// <returns><see langword="true"/> when successfully closed.</returns>
        bool Close();
    }
}
