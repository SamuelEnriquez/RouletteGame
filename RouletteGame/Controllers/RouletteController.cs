using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouletteGame.Data.Repositories;
using RouletteGame.Models.Dtos;
using RouletteGame.Models.Entities;

namespace RouletteGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouletteController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly RouletteRepository _rouletteRepository;

        public RouletteController(RouletteRepository rouletteRepository, IMapper mapper)
        {
            _mapper = mapper;
            _rouletteRepository = rouletteRepository;
        }

        [HttpGet]
        [Route("OpenRoulette")]
        public async Task<ClsReponseGeneric> OpenRoulette(int InIdRoulette)
        {
            return await _rouletteRepository.OpenRoulette(InIdRoulette);
        }

        [HttpGet]
        [Route("GetAllRoulettes")]
        public async Task<List<ClsRoulette>> GetAllRoulettes()
        {
            return await _rouletteRepository.GetRoulettes();
        }

        [HttpGet]
        [Route("CloseRoulette")]
        public async Task<List<ClsBet>> CloseRoulette(int InIdRoulette)
        {
            return await _rouletteRepository.CloseRoulette(InIdRoulette);
        }

        [HttpPost]
        [Route("CreateRoulette")]
        public async Task<ClsRoulette> UpdateMunicipioCobertura([FromBody] DtoAddRoulette dtoAddRoulette)
        {
            ClsRoulette clsRoulette = _mapper.Map<ClsRoulette>(dtoAddRoulette);
            return await _rouletteRepository.CreateRoulette(clsRoulette);
        }

        [HttpPost]
        [Route("AddBet")]
        public async Task<ClsReponseGeneric> AddBet([FromBody] DtoAddBet dtoAddBet)
        {
            ClsBet  clsBet = _mapper.Map<ClsBet>(dtoAddBet);
            return await _rouletteRepository.ValidateBet(clsBet);
        }
    }
}
