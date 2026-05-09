using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Character.Core.Models;

namespace Character.Core.Interfaces
{
    public interface ICharacterInterface
    {
        Task<(bool success, string message)> SaveAsync(List<CharacterItem> items, string fullPath);
        Task<(List<CharacterItem>? data, string message)> LoadAsync(string fullPath);
    }
}
