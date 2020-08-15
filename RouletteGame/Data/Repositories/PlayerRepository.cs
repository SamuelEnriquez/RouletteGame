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
    public class PlayerRepository : IPlayerRepository
    {
        private readonly string _connectionString;

        public PlayerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }
        public async Task<ClsPlayer> FindPlayerByCod(int InCodPlayer)
        {
            SqlConnection sql = new SqlConnection(_connectionString);
            ClsPlayer clsPlayer = new ClsPlayer();
            var datos = new DataSet();
            var sqlDataAdapter = new SqlDataAdapter("dbo.USP_ValidateUser", _connectionString);
            SqlParameter[] parametro =
            {
                new SqlParameter { ParameterName = "@InCodPlayer", Value = InCodPlayer }
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
                    clsPlayer = MapPlayer(item);
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


            return clsPlayer;
        }

        private static ClsPlayer MapPlayer(DataRow DrFilaDatos)
        {
            ClsPlayer clsPlayer = new ClsPlayer();
            clsPlayer.InCodUser = int.Parse(DrFilaDatos["InCodPlayer"].ToString());
            clsPlayer.StrVcPlayerName = DrFilaDatos["VcPlayerName"].ToString();
            clsPlayer.DcCredit = decimal.Parse(DrFilaDatos["DcCredit"].ToString());

            return clsPlayer;
        }

        public async Task<ClsReponseGeneric> UpdatePlayerCredit(int InCodPlayer, decimal DcCredit)
        {
            SqlConnection sql = new SqlConnection(_connectionString);
            ClsReponseGeneric clsResponseGeneric = new ClsReponseGeneric();
            DataTable datos = new DataTable();
            
            var sqlDataAdapter = new SqlDataAdapter("ARR.USP_UpdatePlayerCredit", _connectionString);
            SqlParameter[] parametro =
            {
                new SqlParameter { ParameterName = "@InCodPlayer", Value = InCodPlayer },
                new SqlParameter { ParameterName = "@DcCredit", Value = DcCredit },                
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
                    clsResponseGeneric.message = "Updated Data";
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
