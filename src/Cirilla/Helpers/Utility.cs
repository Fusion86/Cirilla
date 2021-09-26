using Cirilla.Core.Models;
using Cirilla.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cirilla.Helpers
{
    public static class Utility
    {
        private static readonly MHFileType[] _fileTypes;

        static Utility()
        {
            _fileTypes = _fileTypes = Enumeration.GetAll<MHFileType>().ToArray();
        }

        /// <summary>
        /// Get filetype based on file's magic, and if that doesn't work get filetype based on file extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns>MHFileType or null if not found</returns>
        public static MHFileType GetFileType(string path) => GetFileTypeFromMagic(path) ?? GetFileTypeFromFileExtension(path);

        private static MHFileType GetFileTypeFromMagic(string path)
        {
            using FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using BinaryReader br = new(fs);
            byte[] bytes = br.ReadBytes(4);

            return _fileTypes.Where(x => x.Magic != null).FirstOrDefault(x => x.Magic.SequenceEqual(bytes.Take(x.Magic.Length)));
        }

        private static MHFileType GetFileTypeFromFileExtension(string path)
        {
            string ext = Path.GetExtension(path);
            return _fileTypes.Where(x => x.FileExtensions != null).FirstOrDefault(x => x.FileExtensions.Contains(ext));
        }

        // Here we map a Core.Models type to a ViewModel
        private static readonly Dictionary<Type, Type> _handlerTypeToViewModelTypeMap = new()
        {
            { typeof(GMD), typeof(GMDViewModel) },
        };

        // Maps a MHFileType to a FileTypeTabItemViewModelBase
        public static FileTypeTabItemViewModelBase GetViewModelForFile(string path)
        {
            MHFileType reg = Utility.GetFileType(path);

            if (reg != null && _handlerTypeToViewModelTypeMap.TryGetValue(reg.Handler, out Type vmType))
                return (FileTypeTabItemViewModelBase)Activator.CreateInstance(vmType, path);
            else
                throw new Exception("This type of files are currently not supported.");
        }
    }
}
