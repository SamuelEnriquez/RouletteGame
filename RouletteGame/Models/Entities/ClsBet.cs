using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouletteGame.Models.Entities
{
    public class ClsBet
    {
        public int InIdBet { get; set; }
        public int InIdRoulette { get; set; }

        public string StrRouletteName { get; set; }
        public int InBetPosition { get; set; }
        public string StrBetColor { get; set; }

        public decimal DcBetValue { get; set; }
        public int InCodPlayer { get; set; }
        public string StrPlayerName { get; set; }
    }
}
