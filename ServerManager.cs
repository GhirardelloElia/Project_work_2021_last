using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace Progetto_Main
{
    class ServerManager
    {
        private SqlConnection _sqlConnection = new SqlConnection("Server=tcp:servergestionale.database.windows.net,1433;Initial Catalog=DbProgetto2021;Persist Security Info=False;User ID=amministratore;Password=Vmware1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        public void UpdateCommessaInCorso(int pezziBuoni, int pezziScarti, int pezziTotali, int pezziDaFare, string codiceCommessa, string codiceCliente)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = $"UPDATE tbCommessa " +
                    $"SET Pezzi_buoni = '{pezziBuoni}', Pezzi_scarti = '{pezziScarti}' " +
                    $"Pezzi_totali = '{pezziTotali}', Da_produrre = '0', In_Produzione = '1', Prodotta = '0' " +
                    $"WHERE Codice_commessa = '{codiceCommessa}'; ";

                command.ExecuteNonQuery();

                Console.WriteLine("Aggiornato commessa in corso...");

                if (pezziDaFare == pezziBuoni)
                    UpdateCommessaFinita(codiceCommessa);
            }
            catch (Exception ex)
            {
                // TODO: enum errori
                Console.WriteLine("Errore sull'aggiornamento della commessa in corso...");
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        public void UpdateCommessaFinita(string codiceCommessa)
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

                Console.WriteLine("Aggiornato commessa finita...");
            }
            catch (Exception ex)
            {
                // TODO: enum errori
                Console.WriteLine("Errore sull'aggiornamento della commessa finita");
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        public int RegistraMessaggio(string messageContent, bool isFromPLC)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = $"INSERT INTO tbStoricoMessaggi (Messaggio, DaOperatore) " +
                    $"VALUES ('{messageContent}', '{(isFromPLC ? 1 : 0)}')";

                command.ExecuteNonQuery();

                Console.WriteLine("Registrato messaggio su DB...");
            }
            catch (Exception ex)
            {
                // TODO: enum errori
                Console.WriteLine("Errore sulla registrazione del messaggio su DB...");
                return 3;
            }
            finally
            {
                _sqlConnection.Close();
            }

            return 0;
        }

        public Messaggio[] GetMessaggiDB(UInt32 index = 0)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT TOP 8 * from tbStoricoMessaggi ORDER BY DataMessaggio ASC";

                if (index != 0)
                    command.CommandText = $" SELECT * from tbStoricoMessaggi ORDER BY DataMessaggio ASC OFFSET ({index}) ROWS FETCH NEXT (8) ROWS ONLY";


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

                Console.WriteLine("Ricevuti messaggi da DB...");

                return messaggi;
            }
            catch 
            {
                // TODO: Catch
                Console.WriteLine("Errore nella recezione dei messaggi da DB...");
                return new Messaggio[0];
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        public void AggiornaStatoPLC(string codiceCommessa, float velocita, int inAllarme, int StatoPLC)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = $"UPDATE tbStatoPLC " +
                    $"SET Codice_commessa_in_produzione = {codiceCommessa}, Velocita_macchina = {velocita}," +
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

        public void AggiornaStatoPLC(StatoPLC plc)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = $"UPDATE tbStatoPLC SET " +
                    $"codice_commessa_in_produzione = '{plc.commessaInCorso}', " +
                    $"isInWarning = '{(plc.IsInWarning ? 1 : 0)}', " +
                    $"isInErrore = '{(plc.IsInErrore ? 1 : 0)}', " +
                    $"isOnline = '{(plc.IsOnline ? 1 : 0)}', " +
                    $"isWorking = '{(plc.IsInWarning ? 1 : 0)}', " +
                    $"abilitazioneDaUfficio = '{(plc.AbilitazioneDaUfficio ? 1 : 0)}', " +
                    $"clienteInCorso = '{plc.clienteInCorso}', " +
                    $"pezziTotali = '{plc.pezziTotali}', " +
                    $"pezziBuoni = '{plc.pezziBuoni}', " +
                    $"pezziScarti = '{plc.pezziScarti}'," +
                    $"ultimoAggiornamento = GETDATE() ";

                command.ExecuteNonQuery();

                Console.WriteLine("Aggiornato stato PLC su DB...");
            }
            catch
            {
                // TODO: Catch
                Console.WriteLine("Errore sull'aggiornamento dello stato PLC su DB...");
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        public void ScriviAllarme(string MessaggioAllarme, string codiceCommessaInProduzione, bool isEmergenza = false)
        {
            try
            {
                _sqlConnection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;

                command.CommandText = $"INSERT INTO tbStoricoAllarmi (MessaggioAllarme, Codice_commessa_in_produzione, IsWarning) " +
                    $"VALUES ('{MessaggioAllarme}', '{codiceCommessaInProduzione}', '{(isEmergenza ? 1 : 0)}')";

                command.ExecuteNonQuery();

                Console.WriteLine("Scritto allarme su DB...");
            }
            catch
            {
                // TODO: Catch
                Console.WriteLine("Errore sulla scrittura dell'allarme su DB...");
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

        public bool LeggiAbilitazioneDaUfficio()
        {
            try
            {
                _sqlConnection.Open();

                bool res = false;

                SqlCommand command = new SqlCommand();
                command.Connection = _sqlConnection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "SELECT abilitazioneDaUfficio from tbStatoPLC";

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    res = Convert.ToBoolean(reader[0]);
                }

                Console.WriteLine("Ricevuto abilitazioneDaUfficio da DB...");

                return res;
            }
            catch
            {
                // TODO: Catch
                Console.WriteLine("Errore nella recezione dell'abilitazioneDaUfficio...");
                return false;
            }
            finally
            {
                _sqlConnection.Close();
            }
        }

    }
}
