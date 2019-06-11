﻿using Api.DAL.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Api.DAL.Repos {
    public class PlayerRepos : IRepository<Player> {
        
        public Func<IDbConnection> Connection { get; set; }

        public Player Create(Player entity) {

            List<int> _rowCountList = new List<int>();

            Player p = new Player();

            using (var conn = Connection()) {
                    
                using (IDbTransaction tran = conn.BeginTransaction()) {
                    try {

                        //Set imagePath to default image
                        string imagePath = "https:\\localhost:44310\\Resources\\Files\\player-icon.png";
                        

                        //Insert userCredentials and return usercredentials ID
                        string userCredentialsSQL = @"INSERT INTO UserCredentials (Hashpassword, Salt, LoginAttempts) VALUES (@Hashpassword, @Salt, @LoginAttempts); 
                                     SELECT CAST(SCOPE_IDENTITY() as int)";
                        int userCredentials_ID = conn.Query<int>(userCredentialsSQL, new { Hashpassword = entity.UserCredentials.HashPassword, Salt = entity.UserCredentials.Salt, LoginAttempts = 0 }, transaction: tran).Single();

                        //Insert player and return player_ID
                        string playerSQL = @"INSERT INTO Player (Firstname, Lastname, Email, Day, Month, Year, Country, League, Height, Weight, Bodyfat, PreferredHand, CurrentClub, Accomplishments, Statistic, StrengthDescription, 
                                            WeaknessDescription, VideoPath, ImagePath, FormerClubs, ContractStatus, ContractExpired, InjuryStatus, InjuryExpired, InjuryDescription, IsAvailable, PrimaryPosition, SecondaryPosition, CurrentClubPrimaryPosition, CurrentClubSecondaryPosition, UserCredentials_ID) 
                                        VALUES (@Firstname, @Lastname, @Email, @Day, @Month, @Year, @Country, @League, @Height, @Weight, @Bodyfat, @PreferredHand, @CurrentClub, @Accomplishments, @Statistic,
                                            @StrengthDescription, @WeaknessDescription, @VideoPath, @ImagePath, @FormerClubs, @ContractStatus, @ContractExpired, @InjuryStatus, 
                                            @InjuryExpired, @InjuryDescription, @IsAvailable, @PrimaryPosition, @SecondaryPosition, @CurrentClubPrimaryPosition, @CurrentClubSecondaryPosition, @UserCredentials_ID);
                                        SELECT CAST(SCOPE_IDENTITY() as int)";

                        int player_ID = conn.Query<int>(playerSQL, new {
                            Firstname = entity.FirstName,
                            Lastname = entity.LastName,
                            entity.Email,
                            entity.Day,
                            entity.Month,
                            entity.Year,
                            entity.Country,
                            entity.League,
                            entity.Height,
                            entity.Weight,
                            entity.Bodyfat,
                            entity.PreferredHand,
                            entity.CurrentClub,
                            entity.Accomplishments,
                            entity.Statistic,
                            entity.StrengthDescription,
                            entity.WeaknessDescription,
                            entity.VideoPath,
                            ImagePath = imagePath,
                            entity.FormerClubs,
                            entity.ContractStatus,
                            entity.ContractExpired,
                            entity.InjuryStatus,
                            entity.InjuryExpired,
                            entity.InjuryDescription,
                            entity.IsAvailable,
                            entity.PrimaryPosition,
                            entity.SecondaryPosition,
                            entity.CurrentClubPrimaryPosition,
                            entity.CurrentClubSecondaryPosition,
                            UserCredentials_ID = userCredentials_ID
                        }, transaction: tran).Single();



                        //Player strengths
                        if (entity.StrengthList.Count > 0) {
                            foreach (string strength in entity.StrengthList) {

                                //Return strength ID
                                string strengthSQL = @"Select id from Strength where name = @Name";
                                int strength_ID = conn.Query<int>(strengthSQL, new { Name = strength }, transaction: tran).FirstOrDefault();

                                if (strength_ID != 0) {

                                    //Insert PlayerStrength
                                    string playerStrengthSQL = @"INSERT INTO PlayerStrength (Player_ID, Strength_ID) 
                                        VALUES (@Player_ID, @Strength_ID)";

                                    _rowCountList.Add(conn.Execute(playerStrengthSQL, new {
                                        Player_ID = player_ID,
                                        Strength_ID = strength_ID
                                    }, transaction: tran));
                                }
                            }
                        }

                        //Player weaknesses
                        if (entity.WeaknessList.Count > 0) {
                            foreach (string weakness in entity.WeaknessList) {

                                //Return weakness ID
                                string weaknessSQL = @"Select id from Weakness where name = @Name";
                                int weakness_ID = conn.Query<int>(weaknessSQL, new { Name = weakness }, transaction: tran).FirstOrDefault();

                                if (weakness_ID != 0) {

                                    //Insert PlayerWeakness
                                    string playerWeaknessSQL = @"INSERT INTO PlayerWeakness (Player_ID, Weakness_ID) 
                                        VALUES (@Player_ID, @Weakness_ID)";

                                    _rowCountList.Add(conn.Execute(playerWeaknessSQL, new {
                                        Player_ID = player_ID,
                                        Weakness_ID = weakness_ID
                                    }, transaction: tran));
                                }
                            }
                        }

                        if (entity.NationalTeamList.Count > 0) {
                            foreach (NationalTeam nt in entity.NationalTeamList) {

                                //Insert NationalTeam
                                string nationalTeamSQL = @"INSERT INTO NationalTeam (Name, Appearances, Statistic, Position, Player_ID) 
                                            VALUES (@Name, @Appearances, @Statistic, @Position, @Player_ID)";

                                _rowCountList.Add(conn.Execute(nationalTeamSQL, new {
                                    Name = nt.Name,
                                    Appearances = nt.Appearances,
                                    Statistic = nt.Statistic,
                                    Position = nt.Position,
                                    Player_ID = player_ID,
                                }, transaction: tran));

                            }
                        }

                        //Check for 0 in rowcount list
                        if (_rowCountList.Contains(0)) {
                            p.ErrorMessage = "The player was not registred";
                            tran.Rollback();
                        }
                        else {
                            p.ErrorMessage = "";
                            tran.Commit();
                        }

                    }
                    catch (SqlException e) {

                        tran.Rollback();
                        p.ErrorMessage = ErrorHandling.Exception(e);
                    }
                }
            }
            return p;
        }

        public int Delete(int id) {
            throw new NotImplementedException();
        }

        public IEnumerable<Player> GetAll() {
            List<Player> playerList = new List<Player>();
            using (var conn = Connection()) {
                try {
                    playerList = conn.Query<Player>("select * from player where isAvailable = 1").ToList();
                    return playerList;
                }
                catch (SqlException ) {
                    return null;
                }
                
            }
        }

        public Player GetByEmail(string email) {
            Player player = new Player();
            string sql =
                " select p.* from player p where p.email = @email;" +

                " select s.name from Player p " +
                " inner join playerstrength ps on ps.player_id = p.id " +
                " inner join strength s on s.id = ps.strength_ID where p.email = @email;" +

                " select w.name from Player p " +
                " inner join playerweakness pw on pw.player_id = p.id " +
                " inner join weakness w on w.id = pw.weakness_ID where p.email = @email;" +

                " select nt.name, nt.appearances, nt.statistic, nt.position, nt.id from Player p" +
                " inner join NationalTeam nt on nt.player_Id = p.id where p.email = @email;";

            using (var conn = Connection()) {
                try {

                    using (var multi = conn.QueryMultiple(sql, new { email })) {
                        player = multi.Read<Player>().First();
                        player.StrengthList = multi.Read<string>().ToList();
                        player.WeaknessList = multi.Read<string>().ToList();
                        player.NationalTeamList = multi.Read<NationalTeam>().ToList();
                    }
                    return player;
                }
                catch (SqlException) {
                    return null;
                }
            }
        }

        public Player GetById(int id) {
            Player player = new Player();
            string sql =
                " select p.* from player p where p.id = @id;" +

                " select s.name from playerstrength ps " +
                " inner join strength s on s.id = ps.strength_ID where ps.player_id = @id;" +

                " select w.name from playerweakness pw " +
                " inner join weakness w on w.id = pw.weakness_ID where pw.player_id = @id;" +

                " select nt.name, nt.appearances, nt.statistic, nt.position, nt.id from NationalTeam nt where nt.player_id = @id;";

            using (var conn = Connection()) {
                try {
                    using (var multi = conn.QueryMultiple(sql, new { id })) {
                        player = multi.Read<Player>().First();
                        player.StrengthList = multi.Read<string>().ToList();
                        player.WeaknessList = multi.Read<string>().ToList();
                        player.NationalTeamList = multi.Read<NationalTeam>().ToList();
                    }
                    return player;
                }
                catch (SqlException) {
                    return null;
                }
            }
        }

        public IEnumerable<Player> GetBySearchCriteria(string sqlStatement) {
            List<Player> playerList = new List<Player>();
       
              string sql = " select p.*, s.name from player p " +
                    "left join playerstrength ps on ps.player_id = p.id " +
                     "left join strength s on s.id = ps.strength_ID where " + sqlStatement;

            using (var conn = Connection()) {
                Player result = null;

                conn.Query<Player, string, Player>(sql,
                    (playerinside, strength) => {

                        Player p = null;
                        if (!playerList.Any(pl => pl.Id == playerinside.Id)) {
                            p = BuildPlayer(playerinside);
                            result = p;
                            playerList.Add(result);
                        }
                        else {
                            result = playerList.Single(pl => pl.Id == playerinside.Id);
                        }
                        if (strength != null) {
                            result.StrengthList.Add(strength);
                        }
                        return result;
                    }, splitOn: "name");    
            }
            return playerList;
        }

        public UserCredentials getCredentialsByEmail(string email) {

            int id = 0;
            bool club = false;
            using (var conn = Connection()) {
                try {
                    //Select a club, if we find one we set UC.club to true, if not, we try to find a player, if we find one, we set uc.club to false 
                    id = conn.Query<int>("select userCredentials_id from Club where email=@email", new { email }).FirstOrDefault();
                    if (id != 0) {
                        club = true;
                    }
                    else if (id == 0) {
                        id = conn.Query<int>("select userCredentials_id from Player where email=@email", new { email }).FirstOrDefault();
                    }

                    //Checks if we found a player or a club. If found, we select their credentials from DB
                    if (id == 0) {
                        return null;
                    }
                    else {
                        UserCredentials UC = conn.Query<UserCredentials>("select * from Usercredentials where id=@id", new { id }).FirstOrDefault();
                        UC.Club = club;
                        return UC;
                    }
                }
                catch (SqlException e) {
                    return null;
                }
            }

        }

        public bool UpdateInfo(Player entity) {

            bool res = false;

            List<int> _rowCountList = new List<int>();

            using (var conn = Connection()) {

                using (IDbTransaction tran = conn.BeginTransaction()) {
                    try {

                        if (entity.UserCredentials != null) {
                            //Update user credentials
                            string userCredentialsSQL = @"Update UserCredentials Set Hashpassword = @HashPassword, Salt = @Salt Where ID = @ID";

                            _rowCountList.Add(conn.Execute(userCredentialsSQL, new {
                                entity.UserCredentials.HashPassword,
                                entity.UserCredentials.Salt,
                                entity.Id
                            }, transaction: tran));
                        }

                        //Update player
                        string updatePlayerSQL = @"Update Player Set Firstname = @FirstName, Lastname = @LastName, Day = @Day, Month = @Month, Year = @Year, Country = @Country
                                                        Where ID = @ID";
                        _rowCountList.Add(conn.Execute(updatePlayerSQL, new {
                            entity.FirstName,
                            entity.LastName,
                            entity.Day,
                            entity.Month,
                            entity.Year,
                            entity.Country,
                            entity.Id
                        }, transaction: tran));

                        if (_rowCountList.Contains(0)) {
                            tran.Rollback();
                        }
                        else {
                            tran.Commit();
                            res = true;
                        }
                    }
                    catch (SqlException e) {
                        tran.Rollback();
                    }
                }
            }
            return res;
        }

        public bool UpdateAdditionalInfo(Player entity) {

            bool res = false;

            int rowCount = 0;
            using (var conn = Connection()) {

                using (IDbTransaction tran = conn.BeginTransaction()) {
                    try {

                        //Update player additional info
                        string updatePlayerSQL = @"Update Player Set League = @League, Height = @Height, Weight = @Weight, Bodyfat = @Bodyfat,
                                                                    PreferredHand = @PreferredHand, ContractStatus = @ContractStatus,
                                                                    ContractExpired = @ContractExpired, InjuryStatus = @InjuryStatus, InjuryExpired = @InjuryExpired, 
                                                                    InjuryDescription = @InjuryDescription, PrimaryPosition = @PrimaryPosition, 
                                                                    SecondaryPosition = @SecondaryPosition
                                                                 Where ID = @ID";
                        rowCount = conn.Execute(updatePlayerSQL, new {
                            entity.League,
                            entity.Height,
                            entity.Weight,
                            entity.Bodyfat,
                            entity.PreferredHand,
                            entity.ContractStatus,
                            entity.ContractExpired,
                            entity.InjuryStatus,
                            entity.InjuryExpired,
                            entity.InjuryDescription,
                            entity.PrimaryPosition,
                            entity.SecondaryPosition,
                            entity.Id
                        }, transaction: tran);
                        
                        if (rowCount == 0) {
                            tran.Rollback();
                        }
                        else {
                            tran.Commit();
                            res = true;
                        }
                    }
                    catch (SqlException e) {
                        tran.Rollback();
                    }
                }
            }
            return res;
        }

        public bool DeleteStrengthsAndWeaknesses(int player_ID) {

            bool res = false;

            List<int> _rowCountList = new List<int>();

            using (var conn = Connection()) {

                using (IDbTransaction tran = conn.BeginTransaction()) {
                    try {

                        //Check if player weakness exist in DB
                        string playerWeaknessSQL = @"Select * from PlayerWeakness where player_ID = @Player_ID";
                        int weaknesscheck = conn.Query<int>(playerWeaknessSQL, new { Player_ID = player_ID }, transaction: tran).FirstOrDefault();

                        if (weaknesscheck != 0) {

                            //Delete playerWeakness
                            string deletePlayerWeaknessSQL = @"Delete From PlayerWeakness Where Player_ID = @Player_ID";

                            _rowCountList.Add(conn.Execute(deletePlayerWeaknessSQL, new {
                                player_ID
                            }, transaction: tran));
                        }

                        //Check if player strength exist in DB
                        string playerStrengthSQL = @"Select * from playerStrength where Player_ID = @Player_ID";
                        int strengthcheck = conn.Query<int>(playerStrengthSQL, new { Player_ID = player_ID }, transaction: tran).FirstOrDefault();

                        if (strengthcheck != 0) {

                            //Delete playerStrength
                            string deletePlayerStrengthSQL = @"Delete From PlayerStrength Where player_ID = @Player_ID";

                            _rowCountList.Add(conn.Execute(deletePlayerStrengthSQL, new {
                                player_ID
                            }, transaction: tran));
                        }
                        
                        //Check for 0 in rowcount
                        if (_rowCountList.Contains(0)) {
                            tran.Rollback();
                        }
                        else {
                            tran.Commit();
                            res = true;
                        }
                    }
                    catch (SqlException e) {
                        tran.Rollback();
                    }
                }
            }
            return res;
        }

        public bool UpdateStrengthsAndWeaknesses(Club entity) {

            bool res = false;

            List<int> _rowCountList = new List<int>();

            using (var conn = Connection()) {

                using (IDbTransaction tran = conn.BeginTransaction()) {
                    try {

                        //Update player
                        string updatePlayerSQL = @"Update Club Set ValueDescription = @ValueDescription, PreferenceDescription = @PreferenceDescription
                                                                 Where ID = @ID";


                        _rowCountList.Add(conn.Execute(updateClubSQL, new {
                            entity.ValueDescription,
                            entity.PreferenceDescription,
                            entity.Id
                        }, transaction: tran));

                        //Values
                        if (entity.ValuesList.Count > 0) {

                            foreach (string value in entity.ValuesList) {

                                //Return value ID
                                string valueSQL = @"Select id from Value where name = @Name";
                                int value_ID = conn.Query<int>(valueSQL, new { Name = value }, transaction: tran).FirstOrDefault();

                                if (value_ID != 0) {

                                    //Insert ClubValue
                                    string clubValueSQL = @"INSERT INTO ClubValue (Club_ID, Value_ID) 
                                        VALUES (@Club_ID, @Value_ID)";

                                    _rowCountList.Add(conn.Execute(clubValueSQL, new {
                                        Club_ID = entity.Id,
                                        Value_ID = value_ID
                                    }, transaction: tran));
                                }
                            }
                        }

                        //Preference
                        if (entity.PreferenceList.Count > 0) {

                            foreach (string preference in entity.PreferenceList) {

                                //Return preference ID
                                string preferenceSQL = @"Select id from Preference where name = @Name";
                                int preference_ID = conn.Query<int>(preferenceSQL, new { Name = preference }, transaction: tran).FirstOrDefault();

                                if (preference_ID != 0) {

                                    //Update ClubPreference
                                    string clubPreferenceSQL = @"INSERT INTO ClubPreference (Club_ID, Preference_ID)
                                                                 VALUES (@Club_ID, @Preference_ID)";

                                    _rowCountList.Add(conn.Execute(clubPreferenceSQL, new {
                                        Club_ID = entity.Id,
                                        Preference_ID = preference_ID
                                    }, transaction: tran));
                                }
                            }
                        }

                        //Check for 0 in rowcount list
                        if (_rowCountList.Contains(0)) {
                            tran.Rollback();
                        }
                        else {
                            tran.Commit();
                            res = true;
                        }
                    }
                    catch (SqlException e) {

                        tran.Rollback();
                    }
                }
            }
            return res;
        }

        public Player Update(Player entity) {

            Player p = new Player();
            for (int i = 0; i < 5; i++) {

                List<int> _rowCountList = new List<int>();

                using (var conn = Connection()) {

                    using (IDbTransaction tran = conn.BeginTransaction()) {
                        try {

                            //Return row ID
                            string rowIDSQL = @"Select rowID from Player where email = @Email";
                            byte[] row_ID = conn.Query<byte[]>(rowIDSQL, new { Email = entity.Email }, transaction: tran).Single();


                            //Update player
                            string updatePlayerSQL = @"Update Player Set Firstname = @FirstName, Lastname = @LastName, Day = @Day, Month = @Month, Year = @Year, Country = @Country,
                                                                    League = @League, Height = @Height, Weight = @Weight, Bodyfat = @Bodyfat,
                                                                    PreferredHand = @PreferredHand, CurrentClub = @CurrentClub, Accomplishments = @Accomplishments,
                                                                    Statistic = @Statistic, StrengthDescription = @StrengthDescription, WeaknessDescription = @WeaknessDescription,
                                                                    VideoPath = @VideoPath, ImagePath = @ImagePath, FormerClubs = @FormerClubs, ContractStatus = @ContractStatus,
                                                                    ContractExpired = @ContractExpired, InjuryStatus = @InjuryStatus, InjuryExpired = @InjuryExpired, 
                                                                    InjuryDescription = @InjuryDescription, IsAvailable = @IsAvailable, PrimaryPosition = @PrimaryPosition, 
                                                                    SecondaryPosition = @SecondaryPosition, CurrentClubPrimaryPosition = @CurrentClubPrimaryPosition,
                                                                    CurrentClubSecondaryPosition = @CurrentClubSecondaryPosition
                                                                 Where Email = @Email AND RowID = @RowID";
                            _rowCountList.Add(conn.Execute(updatePlayerSQL, new {
                                entity.FirstName,
                                entity.LastName,
                                entity.Day,
                                entity.Month,
                                entity.Year,
                                entity.Country,
                                entity.League,
                                entity.Height,
                                entity.Weight,
                                entity.Bodyfat,
                                entity.PreferredHand,
                                entity.CurrentClub,
                                entity.Accomplishments,
                                entity.Statistic,
                                entity.StrengthDescription,
                                entity.WeaknessDescription,
                                entity.VideoPath,
                                entity.ImagePath,
                                entity.FormerClubs,
                                entity.ContractStatus,
                                entity.ContractExpired,
                                entity.InjuryStatus,
                                entity.InjuryExpired,
                                entity.InjuryDescription,
                                entity.IsAvailable,
                                entity.PrimaryPosition,
                                entity.SecondaryPosition,
                                entity.CurrentClubPrimaryPosition,
                                entity.CurrentClubSecondaryPosition,
                                entity.Email,
                                RowID = row_ID
                            }, transaction: tran));

                            //Return player ID
                            string playerIDSQL = @"Select id from Player where email = @Email";
                            int player_ID = conn.Query<int>(playerIDSQL, new { Email = entity.Email }, transaction: tran).FirstOrDefault();

                            // NationalTeam
                            if (entity.NationalTeamList.Count > 0) {

                                foreach (NationalTeam nt in entity.NationalTeamList) {

                                    if (player_ID != 0) {

                                        //Update nationalTeam
                                        string updateNationalTeamSQL = @"Update NationalTeam Set Appearances = @Appearances, Statistic = @Statistic, Position = @Position
                                                                     Where Player_ID = @Player_ID";

                                        _rowCountList.Add(conn.Execute(updateNationalTeamSQL, new {
                                            nt.Appearances,
                                            nt.Statistic,
                                            nt.Position,
                                            Player_ID = player_ID
                                        }, transaction: tran));
                                    }

                                }
                            }

                            //Weaknesses
                            if (entity.WeaknessList.Count > 0) {

                                foreach (string weakness in entity.WeaknessList) {

                                    //Return weakness ID
                                    string weaknessSQL = @"Select id from Weakness where name = @Name";
                                    int weakness_ID = conn.Query<int>(weaknessSQL, new { Name = weakness }, transaction: tran).FirstOrDefault();

                                    if (weakness_ID != 0 && player_ID != 0) {

                                        //Update PlayerWeakness
                                        string updatePlayerWeaknessSQL = @"Update PlayerWeakness Set Weakness_ID = @Weakness_ID
                                                                       Where Player_ID = @Player_ID";

                                        _rowCountList.Add(conn.Execute(updatePlayerWeaknessSQL, new {
                                            Player_ID = player_ID,
                                            Weakness_ID = weakness_ID
                                        }, transaction: tran));
                                    }
                                }
                            }

                            //Strengths
                            if (entity.StrengthList.Count > 0) {

                                foreach (string strength in entity.StrengthList) {

                                    //Return strength ID
                                    string strengthSQL = @"Select id from Strength where name = @Name";
                                    int strength_ID = conn.Query<int>(strengthSQL, new { Name = strength }, transaction: tran).FirstOrDefault();

                                    if (strength_ID != 0 && player_ID != 0) {

                                        //Update PlayerStrength
                                        string updatePlayerStrengthSQL = @"Update PlayerStrength Set Strength_ID = @Strength_ID
                                                                       Where Player_ID = @Player_ID";

                                        _rowCountList.Add(conn.Execute(updatePlayerStrengthSQL, new {
                                            Player_ID = player_ID,
                                            Strength_ID = strength_ID
                                        }, transaction: tran));
                                    }
                                }
                            }

                            //Check for 0 in rowcount list
                            if (_rowCountList.Contains(0)) {
                                p.ErrorMessage = "The player was not updated";
                                tran.Rollback();
                            }
                            else {
                                p.ErrorMessage = "";
                                tran.Commit();
                            }
                        }

                        catch (SqlException e) {

                            tran.Rollback();
                            p.ErrorMessage = ErrorHandling.Exception(e);
                        }

                    }

                }
            }
            return p;
        }

        public string DeleteNationalTeam(List<NationalTeam> ntl) {

            string errorMessage;

            List<int> _rowCountList = new List<int>();

            using (var conn = Connection()) {

                using (IDbTransaction tran = conn.BeginTransaction()) {
                    //try {

                    if (ntl.Count > 0) {

                        foreach (NationalTeam nt in ntl) {

                            //Delete nationalTeam
                            string deleteNationalTeamSQL = @"Delete From NationalTeam Where ID = @ID";

                            _rowCountList.Add(conn.Execute(deleteNationalTeamSQL, new {
                                ID = nt.Id
                            }, transaction: tran));
                        }
                    }

                    //Check for 0 in rowcount list
                    if (_rowCountList.Contains(0)) {
                        errorMessage = "NationalTeam was not deleted";
                        tran.Rollback();
                    }
                    else {
                        errorMessage = "";
                        tran.Commit();
                    }
                    //}
                    //catch (SqlException e) {

                    //    tran.Rollback();
                    //    c.ErrorMessage = ErrorHandling.Exception(e);
                    //}

                }
            }
            return errorMessage;
        }
        

        private Player BuildPlayer(Player playerinside) {
            return new Player {
                Id = playerinside.Id,
                FirstName = playerinside.FirstName,
                LastName = playerinside.LastName,
                Email = playerinside.Email,
                Day = playerinside.Day,
                Month = playerinside.Month,
                Year = playerinside.Year,
                Country = playerinside.Country,
                League = playerinside.League,
                Height = playerinside.Height,
                Weight = playerinside.Weight,
                Bodyfat = playerinside.Bodyfat,
                PreferredHand = playerinside.PreferredHand,
                CurrentClub = playerinside.CurrentClub,
                Accomplishments = playerinside.Accomplishments,
                Statistic = playerinside.Statistic,
                StrengthDescription = playerinside.StrengthDescription,
                WeaknessDescription = playerinside.WeaknessDescription,
                VideoPath = playerinside.VideoPath,
                ImagePath = playerinside.ImagePath,
                FormerClubs = playerinside.FormerClubs,
                ContractStatus = playerinside.ContractStatus,
                ContractExpired = playerinside.ContractExpired,
                InjuryStatus = playerinside.InjuryStatus,
                InjuryDescription = playerinside.InjuryDescription,
                InjuryExpired = playerinside.InjuryExpired,
                IsAvailable = playerinside.IsAvailable,
                PrimaryPosition = playerinside.PrimaryPosition,
                SecondaryPosition = playerinside.SecondaryPosition,
                CurrentClubPrimaryPosition = playerinside.CurrentClubPrimaryPosition,
                CurrentClubSecondaryPosition = playerinside.CurrentClubSecondaryPosition
            };
        }

        bool IRepository<Player>.Delete(int id) {
            throw new NotImplementedException();
        }
    }
}
