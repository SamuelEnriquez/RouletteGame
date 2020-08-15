using RouletteGame.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouletteGame.Data.Interfaces
{
    public interface IRouletteRepository
    {
        Task<ClsRoulette> CreateRoulette(ClsRoulette clsRoulette);
        Task<List<ClsBet>> CloseRoulette(int InIdRoulette);
        Task<List<ClsRoulette>> GetRoulettes();

        Task<List<ClsRoulettePosition>> GetRoulettePositions();
        Task<ClsReponseGeneric> ValidateBet(ClsBet clsBet);
        Task<ClsReponseGeneric> AddBet(ClsBet clsBet);
        Task<ClsReponseGeneric> OpenRoulette(int InIdRoulette);

    }
}
