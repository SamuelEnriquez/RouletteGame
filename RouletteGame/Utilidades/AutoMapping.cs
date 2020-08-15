using AutoMapper;
using RouletteGame.Models.Dtos;
using RouletteGame.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouletteGame.Utilidades
{
    public class AutoMapping:Profile
    {
        public AutoMapping()
        {
            CreateMap<ClsBet, DtoAddBet>().ReverseMap();
            CreateMap<ClsRoulette, DtoAddRoulette>().ReverseMap();

        }
    }
}
