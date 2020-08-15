using Microsoft.Extensions.Configuration;
using RouletteGame.Data.Interfaces;
using RouletteGame.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RouletteGame.Data.Repositories
{
    public class RouletteRepository : IRouletteRepository
    {
        private readonly string _connectionString;
        private readonly PlayerRepository _playerRepository;

        public RouletteRepository(IConfiguration configuration, PlayerRepository playerRepository)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
            _playerRepository = playerRepository;
        }
        public async Task<ClsReponseGeneric> ValidateBet(ClsBet clsBet)
        {
            ClsReponseGeneric clsReponseGeneric = new ClsReponseGeneric();
            List<ClsRoulettePosition> positionsList = await GetRoulettePositions();
            ClsPlayer clsPlayer = await _playerRepository.FindPlayerByCod(clsBet.InCodPlayer);

            if (clsPlayer.InCodUser != 0)
            {
                if (clsBet.DcBetValue <= 10000)
                {
                    if (clsPlayer.DcCredit >= clsBet.DcBetValue)
                    {
                        if (String.IsNullOrEmpty(clsBet.StrBetColor))
                        {
                            if (Enumerable.Range(0, 36).Contains(clsBet.InBetPosition))
                            {                               
                                clsReponseGeneric = await AddBet(clsBet);
                            }
                            else{clsReponseGeneric.message = "invalid position";}
                        }else{
                            clsBet.InBetPosition = -1;
                            if (clsBet.StrBetColor.Contains("N") || clsBet.StrBetColor.Contains("R"))
                            {
                                clsReponseGeneric = await AddBet(clsBet);
                            }
                            else{clsReponseGeneric.message = "invalid color position (N/R)";}
                        }                      
                    }else{clsReponseGeneric.message = "insufficient credit";}
                }else{clsReponseGeneric.message = "The maximum value to bet is 10000,00";}
            }else{clsReponseGeneric.message = "invalid player";}

            return clsReponseGeneric;

        }

        public async Task<List<ClsBet>> CloseRoulette(int InIdRoulette)
        {
            SqlConnection sql = new SqlConnection(_connectionString);
            List<ClsBet> betsByRoulette = new List<ClsBet>();
            var datos = new DataSet();
            var sqlDataAdapter = new SqlDataAdapter("dbo.USP_CloseRoulette", _connectionString);
            SqlParameter[] parametro =
            {
                new SqlParameter { ParameterName = "@InIdRoulette", Value = InIdRoulette }
            };

            try
            {
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDataAdapter.SelectCommand.CommandTimeout = 0;
                sqlDataAdapter.SelectCommand.Parameters.AddRange(parametro);
                sqlDataAdapter.SelectCommand.Connection.Open();
                await Task.Run(() => sqlDataAdapter.Fill(datos));
                foreach (DataRow item in datos.Tables[0].Rows)
                {
                    betsByRoulette.Add(MapBet(item));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
            finally
            {
                sqlDataAdapter.SelectCommand.Connection.Close();
                sqlDataAdapter.SelectCommand.Dispose();
            }

            return betsByRoulette;
        }

        private static ClsBet MapBet(DataRow DrFilaDatos)
        {
            ClsBet clsBet = new ClsBet();
            clsBet.InIdRoulette = int.Parse(DrFilaDatos["InIdRoulette"].ToString());
            clsBet.StrRouletteName = DrFilaDatos["VcRouletteName"].ToString();
            clsBet.StrBetColor = DrFilaDatos["ChBetColor"].ToString();
            clsBet.InBetPosition = int.Parse(DrFilaDatos["InBetPosition"].ToString());
            clsBet.DcBetValue = decimal.Parse(DrFilaDatos["DcBetValue"].ToString());
            clsBet.InCodPlayer = int.Parse(DrFilaDatos["InCodPlayer"].ToString());
            clsBet.StrPlayerName = DrFilaDatos["VcPlayerName"].ToString();
            return clsBet;
        }

        public async Task<ClsRoulette> CreateRoulette(ClsRoulette clsRoulette)
        {
            SqlConnection sql = new SqlConnection(_connectionString);
            ClsRoulette ObjRoulette = new ClsRoulette();
            var datos = new DataSet();
            var sqlDataAdapter = new SqlDataAdapter("dbo.USP_CreateRoulette", _connectionString);
            SqlParameter[] parametro =
            {
                new SqlParameter { ParameterName = "@VcRouletteName", Value = clsRoulette.StrRouletteName }
            };

            try
            {
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDataAdapter.SelectCommand.CommandTimeout = 0;
                sqlDataAdapter.SelectCommand.Parameters.AddRange(parametro);
                sqlDataAdapter.SelectCommand.Connection.Open();
                await Task.Run(() => sqlDataAdapter.Fill(datos));
                foreach (DataRow item in datos.Tables[0].Rows)
                {
                    ObjRoulette.InIdRoulette = int.Parse(item[0].ToString());
                    ObjRoulette.StrRouletteName = clsRoulette.StrRouletteName;                 
                }
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
            finally
            {
                sqlDataAdapter.SelectCommand.Connection.Close();
                sqlDataAdapter.SelectCommand.Dispose();
            }
            
            return ObjRoulette;
        }

        private static ClsRoulette MapRoulette(DataRow DrFilaDatos)
        {
            ClsRoulette clsRoulette = new ClsRoulette();
            clsRoulette.InIdRoulette = int.Parse(DrFilaDatos["InIdRoulette"].ToString());
            clsRoulette.StrRouletteName = DrFilaDatos["VcRouletteName"].ToString();
            clsRoulette.BlStatus = Convert.ToBoolean(DrFilaDatos["BtStatus"].ToString());

            return clsRoulette;
        }
        public async Task<List<ClsRoulette>> GetRoulettes()
        {
            SqlConnection sql = new SqlConnection(_connectionString);
            List<ClsRoulette> roulettesList = new List<ClsRoulette>();
            var datos = new DataSet();
            var sqlDataAdapter = new SqlDataAdapter("dbo.USP_GetAllRoulettes", _connectionString);
         
            try
            {
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDataAdapter.SelectCommand.CommandTimeout = 0;
                sqlDataAdapter.SelectCommand.Connection.Open();
                await Task.Run(() => sqlDataAdapter.Fill(datos));
                foreach (DataRow item in datos.Tables[0].Rows)
                {
                    roulettesList.Add(MapRoulette(item));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
            finally
            {
                sqlDataAdapter.SelectCommand.Connection.Close();
                sqlDataAdapter.SelectCommand.Dispose();
            }

            return roulettesList;
        }

        public async Task<ClsReponseGeneric> OpenRoulette(int InIdRoulette)
        {
            SqlConnection sql = new SqlConnection(_connectionString);
            ClsReponseGeneric clsResponseGeneric = new ClsReponseGeneric();
            DataTable datos = new DataTable();
            var sqlDataAdapter = new SqlDataAdapter("dbo.USP_OpeningRoulette", _connectionString);
            SqlParameter[] parametro =
            {
                new SqlParameter { ParameterName = "@InIdRoulette", Value = InIdRoulette }
            };

            try
            {
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDataAdapter.SelectCommand.CommandTimeout = 0;
                sqlDataAdapter.SelectCommand.Parameters.AddRange(parametro);
                sqlDataAdapter.SelectCommand.Connection.Open();
                await Task.Run(() => sqlDataAdapter.Fill(datos));

                if (datos.Rows.Count > 0)
                {
                    clsResponseGeneric.message = datos.Rows[0].ItemArray[0].ToString();
                }
                else
                {
                    clsResponseGeneric.message = "Open Roulette";
                }
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
            finally
            {
                sqlDataAdapter.SelectCommand.Connection.Close();
                sqlDataAdapter.SelectCommand.Dispose();
            }

            return clsResponseGeneric;
        }

        public async Task<List<ClsRoulettePosition>> GetRoulettePositions()
        {
            SqlConnection sql = new SqlConnection(_connectionString);
            List<ClsRoulettePosition> roulettesPositionsList = new List<ClsRoulettePosition>();
            var datos = new DataSet();
            var sqlDataAdapter = new SqlDataAdapter("dbo.USP_GetAllRoulettesPositions", _connectionString);

            try
            {
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDataAdapter.SelectCommand.CommandTimeout = 0;
                sqlDataAdapter.SelectCommand.Connection.Open();
                await Task.Run(() => sqlDataAdapter.Fill(datos));
                foreach (DataRow item in datos.Tables[0].Rows)
                {
                    roulettesPositionsList.Add(MapRoulettePosition(item));
                }
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
            finally
            {
                sqlDataAdapter.SelectCommand.Connection.Close();
                sqlDataAdapter.SelectCommand.Dispose();
            }

            return roulettesPositionsList;
        }

        private static ClsRoulettePosition MapRoulettePosition(DataRow DrFilaDatos)
        {
            ClsRoulettePosition clsRoulettePosition = new ClsRoulettePosition();
            clsRoulettePosition.InIdRoulettePosition = int.Parse(DrFilaDatos["InIdRoulettePosition"].ToString());
            clsRoulettePosition.InNumberPosition = int.Parse(DrFilaDatos["InNumberPosition"].ToString());
            clsRoulettePosition.StrColorPosition = DrFilaDatos["VcColorPosition"].ToString();

            return clsRoulettePosition;
        }

        public async Task<ClsReponseGeneric> AddBet(ClsBet clsBet)
        {
            SqlConnection sql = new SqlConnection(_connectionString);
            ClsReponseGeneric clsResponseGeneric = new ClsReponseGeneric();
            DataTable datos = new DataTable();
            var sqlDataAdapter = new SqlDataAdapter("dbo.USP_NewBet", _connectionString);
            SqlParameter[] parametro =
            {
                new SqlParameter { ParameterName = "@InIdRoulette", Value = clsBet.InIdRoulette },
                new SqlParameter { ParameterName = "@InBetPosition", Value = clsBet.InBetPosition },
                new SqlParameter { ParameterName = "@ChBetColor", Value = clsBet.StrBetColor },
                new SqlParameter { ParameterName = "@DcBetValue", Value = clsBet.DcBetValue },
                new SqlParameter { ParameterName = "@InCodPlayer", Value = clsBet.InCodPlayer }
                
            };

            try
            {
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDataAdapter.SelectCommand.CommandTimeout = 0;
                sqlDataAdapter.SelectCommand.Parameters.AddRange(parametro);
                sqlDataAdapter.SelectCommand.Connection.Open();
                await Task.Run(() => sqlDataAdapter.Fill(datos));

                if (datos.Rows.Count > 0)
                {
                    clsResponseGeneric.message = datos.Rows[0].ItemArray[0].ToString();
                }
                else
                {
                    clsResponseGeneric.message = "registered bet";
                }
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
            finally
            {
                sqlDataAdapter.SelectCommand.Connection.Close();
                sqlDataAdapter.SelectCommand.Dispose();
            }

            return clsResponseGeneric;
        }
    }
}
