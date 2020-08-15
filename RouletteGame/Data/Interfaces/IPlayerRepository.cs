using RouletteGame.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouletteGame.Data.Interfaces
{
   public interface IPlayerRepository
    {
        Task<ClsPlayer> FindPlayerByCod(int InCodPlayer);
        Task<ClsReponseGeneric> UpdatePlayerCredit(int InCodPlayer, decimal DcCredit);


    }
}
