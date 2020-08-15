using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouletteGame.Models.Dtos
{
    public class DtoAddBet
    {
     
        public int InIdRoulette { get; set; }
      
        public int InBetPosition { get; set; }
        public string StrBetColor { get; set; }

        public decimal DcBetValue { get; set; }
        public int InCodPlayer { get; set; }
       
    }
}
