﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using MySql.Data.MySqlClient;

namespace Dogmeat.Database
{
    public class InsigniaHandler
    {
        private MySqlConnection Connection;

        public InsigniaHandler(MySqlConnection connection) => Connection = connection;
        
        public async Task AddInsignia(String ID, String Name, String URL = "None")
        {
            MySqlCommand Command = Connection.CreateCommand();
            Command.Parameters.AddWithValue("ID", ID);
            Command.Parameters.AddWithValue("Name", Name);
            Command.Parameters.AddWithValue("URL", URL);
            Command.CommandText = "INSERT INTO Insignias VALUES(@ID, @Name, @URL)";

            await Utilities.MySql.ExecuteCommand(Command, Utilities.MySql.CommandExecuteType.NONQUERY);
        }
        
        public async Task<IEnumerable<Insignia>> GetInsignia(String IDs)
        {
            List<Insignia> Insignias = new List<Insignia>();
            String[] ids = IDs.Split(';');
            
            lock (Connection)
            {
                try
                {
                    Connection.OpenAsync().GetAwaiter().GetResult();
                    foreach (String ID in ids)
                    {
                        Insignia Insignia = new Insignia(ID, "", "");
                        
                        MySqlCommand Command = Connection.CreateCommand();
                        Command.Parameters.AddWithValue("ID", ID);
                        Command.CommandText = "SELECT * FROM Insignias WHERE ID = @ID";

                        using (MySqlDataReader Reader = Command.ExecuteReader())
                        {
                            while (Reader.ReadAsync().GetAwaiter().GetResult())
                            {
                                Insignia.Name = Reader.GetString(1);
                                Insignia.URL = Reader.GetString(2);
                            }
                        }
                        Insignias.Add(Insignia);
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }
                finally { Connection.Close(); }
            }
            
            return Insignias;
        }
        
        public async Task<bool> CheckInsignia(String ID)
        {
            bool Exists = false;

            MySqlCommand Command = Connection.CreateCommand();
            Command.Parameters.AddWithValue("ID", ID);
            Command.CommandText = "SELECT EXISTS(SELECT 1 FROM Insignias WHERE ID = @ID LIMIT 1);";

            object Result =
                await Utilities.MySql.ExecuteCommand(Command, Utilities.MySql.CommandExecuteType.SCALAR);

            if (Result == null) return Exists;
            
            Int32.TryParse(Result.ToString(), out int exists);

            Exists = exists != 0;

            return Exists;
        }
    }

    public class Insignia
    {
        public String ID, Name, URL;
        
        public Insignia(String Id, String name, String url)
        {
            ID = Id;
            Name = name;
            URL = url;
        }
    }
}