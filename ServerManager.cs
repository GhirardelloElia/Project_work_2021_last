using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Main
{
    class ServerManager
    {
        private SqlConnection _sqlConnection = new SqlConnection("Server=tcp:servergestionale.database.windows.net,1433;Initial Catalog=DbProgetto2021;Persist Security Info=False;User ID=amministratore;Password=Vmware1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        public int UpdateCommessaInCorso(int pezziBuoni, int pezziScarti, int pezziTotali, int pezziDaFare, string codiceCommessa, string codiceCliente)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = $"UPDATE tbCommessa " +
                    $"SET Pezzi_buoni = '{pezziBuoni}', Pezzi_scarti = '{pezziScarti}' " +
                    $"Pezzi_totali = '{pezziTotali}'" +
                    $"WHERE Codice_commessa = '{codiceCommessa}'; ";

                command.ExecuteNonQuery();

                if (pezziDaFare == pezziBuoni)
                    UpdateCommessaFinita(codiceCommessa);
            }
            catch (Exception ex)
            {
                // TODO: enum errori
                return 1;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return 0;
        }

        public int UpdateCommessaFinita(string codiceCommessa)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = $"UPDATE tbCommessa " +
                    $"SET Da_produrre = '0', In_Produzione = '0', Prodotta = '1' " +
                    $"WHERE Codice_commessa = '{codiceCommessa}'; ";

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // TODO: enum errori
                return 1;
            }
            finally
            {
                _sqlConnection.Close();
            }

            return 0;
        }

        public int RegistraMessaggio(string messageContent, bool isFromPLC)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = $"INSERT INTO tbStoricoMessaggi " +
                    $"VALUES ({messageContent}, '{(isFromPLC ? 1 : 0)}')";

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // TODO: enum errori
                return 3;
            }
            finally
            {
                _sqlConnection.Close();
            }

            return 0;
        }

        public Messaggio[] GetMessaggiDB()
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT TOP 8 * from tbStoricoMessaggi";
                
                SqlDataReader reader = command.ExecuteReader();
                int i = 0;
                Messaggio[] messaggi = new Messaggio[8]; 
                while (reader.Read())
                {
                    Messaggio messaggio = new Messaggio()
                    {
                        Text = Convert.ToString(reader[0]),
                        IsSent = Convert.ToBoolean(reader[1]),
                        TimeStamp = Convert.ToString(reader[2])
                    };
                    messaggi[i] = messaggio;
                    i++;
                }

                return messaggi;
            }
            catch 
            {
                // TODO: Catch
                return new Messaggio[0];
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        public void AggiornaStatoPLC(string codiceCommessa, string velocita, string ore, int inAllarme, int StatoPLC)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = $"UPDATE tbStatoPLC " +
                    $"SET Codice_commessa_in_produzione = {codiceCommessa}, Velocita_macchina = {velocita}," +
                    $" Ore_lavorazione = {ore}," +
                    $" In_allarme = {inAllarme}, StatoPLC = {StatoPLC} " +
                    $"WHERE 1 = 1; ";

                command.ExecuteNonQuery();
            }
            catch
            {
                // TODO: Catch
            }
            finally
            {
                _sqlConnection.Close();
            }
        }
    }
}
