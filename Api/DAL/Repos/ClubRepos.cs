﻿using Dapper;
using Api.DAL.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Api.DAL.Repos {
    public class ClubRepos : IRepository<Club> {

        public Func<IDbConnection> Connection { get; set; }

        public Club Create(Club entity) {

            List<int> _rowCountList = new List<int>();

            Club c = new Club();

            using (var conn = Connection()) {

                using (IDbTransaction tran = conn.BeginTransaction()) {
                    //try {

                    //Return usercredentials ID
                    string userCredentialsSQL = @"INSERT INTO UserCredentials (Hashpassword, Salt, LoginAttempts) VALUES (@Hashpassword, @Salt, @LoginAttempts); 
                                     SELECT CAST(SCOPE_IDENTITY() as int)";
                    int userCredentials_ID = conn.Query<int>(userCredentialsSQL, new { Hashpassword = entity.UserCredentials.HashPassword, Salt = entity.UserCredentials.Salt, LoginAttempts = 0 }, transaction: tran).Single();

                    //Return zipcodeCity ID
                    string zipcodeCitySQL = @"INSERT INTO ZipcodeCity (Zipcode, City) VALUES (@Zipcode, @City);
                                        SELECT CAST(SCOPE_IDENTITY() as int)";
                    int zipcodeCity_ID = conn.Query<int>(zipcodeCitySQL, new { Zipcode = entity.Zipcode, City = entity.City }, transaction: tran).Single();

                    //Insert Club
                    string clubSQL = @"INSERT INTO Club (Name, Email, League, Country, StreetAddress, StreetNumber, Trainer, AssistantTrainer, Physiotherapist, AssistantPhysiotherapist, Manager, ValueDescription, PreferenceDescription, 
                                        IsAvailable, ZipcodeCity_ID, UserCredentials_ID) 
                                        VALUES (@Name, @Email, @League, @Country, @StreetAddress, @StreetNumber, @Trainer, @AssistantTrainer, @Physiotherapist, @AssistantPhysiotherapist, @Manager, @ValueDescription, @PreferenceDescription, 
                                        @IsAvailable, @ZipcodeCity_ID, @UserCredentials_ID);
                                            SELECT CAST(SCOPE_IDENTITY() as int)";

                    var club_ID = conn.Query<int>(clubSQL, new {
                        Name = entity.Name,
                        Email = entity.Email,
                        entity.League,
                        entity.Country,
                        entity.StreetAddress,
                        entity.StreetNumber,
                        entity.Trainer,
                        entity.AssistantTrainer,
                        entity.Physiotherapist,
                        entity.AssistantPhysiotherapist,
                        entity.Manager,
                        entity.ValueDescription,
                        entity.PreferenceDescription,
                        entity.IsAvailable,
                        ZipcodeCity_ID = zipcodeCity_ID,
                        UserCredentials_ID = userCredentials_ID
                    }, transaction: tran).Single();

                    //Club values
                    if (entity.ValuesList.Count > 0) {
                        foreach (string value in entity.ValuesList) {

                            //Return value ID
                            string valuesSQL = @"Select id from Value where name = @Name";
                            int value_ID = conn.Query<int>(valuesSQL, new { Name = value }, transaction: tran).FirstOrDefault();

                            if (value_ID != 0) {

                                //Insert ClubValue
                                string clubValueSQL = @"INSERT INTO ClubValue (Club_ID, Value_ID) 
                                        VALUES (@Club_ID, @Value_ID)";

                                _rowCountList.Add(conn.Execute(clubValueSQL, new {
                                    Club_ID = club_ID,
                                    Value_ID = value_ID
                                }, transaction: tran));
                            }
                        }
                    }

                    //Club preferences
                    if (entity.PreferenceList.Count > 0) {
                        foreach (string preference in entity.PreferenceList) {

                            //Return preference ID
                            string preferenceSQL = @"Select id from Preference where name = @Name";
                            int preference_ID = conn.Query<int>(preferenceSQL, new { Name = preference }, transaction: tran).FirstOrDefault();

                            if (preference_ID != 0) {

                                //Insert ClubPreference
                                string clubPreferenceSQL = @"INSERT INTO ClubPreference (Club_ID, Preference_ID) 
                                        VALUES (@Club_ID, @Preference_ID)";

                                _rowCountList.Add(conn.Execute(clubPreferenceSQL, new {
                                    Club_ID = club_ID,
                                    Preference_ID = preference_ID
                                }, transaction: tran));
                            }
                        }
                    }


                    //Current Squad Players
                    if (entity.CurrentSquadPlayersList.Count > 0) {
                        foreach (SquadPlayer csp in entity.CurrentSquadPlayersList) {

                            //Return squad player position ID

                            string squadPlayerPositionSQL = @"Select id from position where name = @Position";

                            int squadPlayerPosition_ID = conn.Query<int>(squadPlayerPositionSQL, new { Position = csp.Position }, transaction: tran).FirstOrDefault();

                            if (squadPlayerPosition_ID != 0) {

                                //Insert Squad Player
                                string squadPlayerSQL = @"INSERT INTO SquadPlayers (ShirtNumber, Season, Name, Club_ID, Position_ID) 
                                        VALUES (@ShirtNumber, @Season, @Name, @Club_ID, @Position_ID)";

                                _rowCountList.Add(conn.Execute(squadPlayerSQL, new {
                                    ShirtNumber = csp.ShirtNumber,
                                    Season = csp.Season,
                                    Name = csp.Name,
                                    Club_ID = club_ID,
                                    Position_ID = squadPlayerPosition_ID
                                }, transaction: tran));
                            }
                        }
                    }

                    //Next Year Squad Players
                    if (entity.NextYearSquadPlayersList.Count > 0) {
                        foreach (SquadPlayer nysp in entity.NextYearSquadPlayersList) {

                            //Return squad player position ID

                            string squadPlayerPositionSQL = @"Select id from position where name = @Position";

                            int squadPlayerPosition_ID = conn.Query<int>(squadPlayerPositionSQL, new { Position = nysp.Position }, transaction: tran).FirstOrDefault();

                            if (squadPlayerPosition_ID != 0) {

                                //Insert Squad Player
                                string squadPlayerSQL = @"INSERT INTO SquadPlayers (ShirtNumber, Season, Name, Club_ID, Position_ID) 
                                        VALUES (@ShirtNumber, @Season, @Name, @Club_ID, @Position_ID)";

                                _rowCountList.Add(conn.Execute(squadPlayerSQL, new {
                                    ShirtNumber = nysp.ShirtNumber,
                                    Season = nysp.Season,
                                    Name = nysp.Name,
                                    Club_ID = club_ID,
                                    Position_ID = squadPlayerPosition_ID
                                }, transaction: tran));
                            }
                        }
                    }

                    //Job position
                    if (entity.JobPositionsList.Count > 0) {
                        foreach (JobPosition jp in entity.JobPositionsList) {

                            var jobPosition_ID = 0;
                            //Return jobPosition position ID
                            string positionSQL = @"Select id from position where name = @Position";
                            int position_ID = conn.Query<int>(positionSQL, new { Position = jp.Position }, transaction: tran).FirstOrDefault();

                            if (position_ID != 0) {

                                //Insert JobPosition
                                string jobPositionSQL = @"INSERT INTO JobPosition (League, PreferredHand, Height, MinAge, MaxAge, Season, ContractStatus, Club_ID, Position_ID) 
                                        VALUES (@League, @PreferredHand, @Height, @MinAge, @MaxAge, @Season, @ContractStatus, @Club_ID, @Position_ID);
                                            SELECT CAST(SCOPE_IDENTITY() as int)";

                                jobPosition_ID = conn.Query<int>(jobPositionSQL, new {
                                    League = jp.League,
                                    PreferredHand = jp.PreferredHand,
                                    Height = jp.Height,
                                    MinAge = jp.MinAge,
                                    MaxAge = jp.MaxAge,
                                    Season = jp.Season,
                                    ContractStatus = jp.ContractStatus,
                                    Club_ID = club_ID,
                                    Position_ID = position_ID
                                }, transaction: tran).Single();
                            }

                            if (jp.StrengthsList.Count > 0) {
                                foreach (string strength in jp.StrengthsList) {

                                    //Return strength ID
                                    string strengthSQL = @"Select id from Strength where name = @Name";
                                    int strength_ID = conn.Query<int>(strengthSQL, new { Name = strength }, transaction: tran).FirstOrDefault();

                                    if (strength_ID != 0) {

                                        //Insert JobPositionStrength
                                        string jobPositionStrengthSQL = @"INSERT INTO JobPositionStrength (JobPosition_ID, Strength_ID) 
                                        VALUES (@JobPosition_ID, @Strength_ID)";

                                        _rowCountList.Add(conn.Execute(jobPositionStrengthSQL, new {
                                            JobPosition_ID = jobPosition_ID,
                                            Strength_ID = strength_ID
                                        }, transaction: tran));
                                    }
                                }

                            }

                        }
                    }
                    
                    //Training hours
                    foreach (TrainingHours traininghours in entity.TrainingHoursList) {

                        //Insert Training hours
                        string trainingHoursSQL = @"INSERT INTO TrainingHours (Name, Mon, Tue, Wed, Thu, Fri, Sat, Sun, Club_ID) 
                                        VALUES (@Name, @Mon, @Tue, @Wed, @Thu, @Fri, @Sat, @Sun, @Club_ID)";

                        _rowCountList.Add(conn.Execute(trainingHoursSQL, new {
                            Name = traininghours.Name,
                            Mon = traininghours.Mon,
                            Tue = traininghours.Tue,
                            Wed = traininghours.Wed,
                            Thu = traininghours.Thu,
                            Fri = traininghours.Fri,
                            Sat = traininghours.Sat,
                            Sun = traininghours.Sun,
                            Club_ID = club_ID
                        }, transaction: tran));
                    }


                    //Check for 0 in rowcount list
                    if (_rowCountList.Contains(0)) {
                        c.ErrorMessage = "The club was not registred";
                        tran.Rollback();
                    }
                    else {
                        c.ErrorMessage = "";
                        tran.Commit();
                    }
                    //}
                    //catch (SqlException e) {

                    //    tran.Rollback();
                    //    c.ErrorMessage = ErrorHandling.Exception(e);
                    //}
                }
            }
            return c;
        }
        

        public int Delete(int id) {
            throw new NotImplementedException();
        }

        public IEnumerable<Club> GetAll() {
            throw new NotImplementedException();
        }

        public Club GetByEmail(string email) {
            Club club = new Club();
            using (var conn = Connection()) {
                //try {
                club = conn.Query<Club, int, string, Club>("select c.*, ci.zipcode, ci.city from club c" +
                    " inner join ZipcodeCity ci on c.zipcodecity_id = ci.id where c.email = @email",
                (clubinside, code, city) => { clubinside.Zipcode = code; clubinside.City = city; return clubinside; }, new { email }, splitOn: "Zipcode,city").Single();

                club = GetClubTraningHourList(club, conn);
                club = GetClubCurrentSquadList(club, conn);
                club = GetClubNextYearSquadList(club, conn);
                club = GetJobPosition(club, conn);
                club = GetClubValueList(club, conn);
                club = GetClubPreferenceList(club, conn);

                //}
                //catch (SqlException e) {
                //    club.ErrorMessage = ErrorHandling.Exception(e);
                //}
            }
            return club;
        }

        public Club GetById(int id) {
            Club club = new Club();
            using (var conn = Connection()) {
                //try {
                club = conn.Query<Club, int, string, Club>("select c.*, ci.zipcode, ci.city from club c" +
                    " inner join ZipcodeCity ci on c.zipcodecity_id = ci.id where c.id = @id",
                (clubinside, code, city) => { clubinside.Zipcode = code; clubinside.City = city; return clubinside; }, new { id }, splitOn: "Zipcode,city").Single();

                club = GetClubTraningHourList(club, conn);
                club = GetClubCurrentSquadList(club, conn);
                club = GetClubNextYearSquadList(club, conn);
                club = GetJobPosition(club, conn);
                club = GetClubValueList(club, conn);
                club = GetClubPreferenceList(club, conn);
                club = GetClubFacilityImagesList(club, conn);

                //}
                //catch (SqlException e) {
                //    club.ErrorMessage = ErrorHandling.Exception(e);
                //}
            }
            return club;
        }



        public UserCredentials getCredentialsByEmail(string email) {
            throw new NotImplementedException();
        }

        public void Insert(Club entity) {
            throw new NotImplementedException();
        }

        public void Save() {
            throw new NotImplementedException();
        }

        public Club Update(Club entity) {

            Club c = new Club();
            for (int i = 0; i < 5; i++) {

                List<int> _rowCountList = new List<int>();

                using (var conn = Connection()) {
                    
                    using (IDbTransaction tran = conn.BeginTransaction()) {
                        //try {
                        
                            //Return row ID
                            string rowIDSQL = @"Select rowID from Club where email = @Email";
                            byte[] row_ID = conn.Query<byte[]>(rowIDSQL, new { Email = entity.Email }, transaction: tran).Single();

                            //Return zipcodeCity ID
                            string zipcodeCitySQL = @"INSERT INTO ZipcodeCity (Zipcode, City) VALUES (@Zipcode, @City);
                                        SELECT CAST(SCOPE_IDENTITY() as int)";
                            int zipcodeCity_ID = conn.Query<int>(zipcodeCitySQL, new { Zipcode = entity.Zipcode, City = entity.City }, transaction: tran).Single();

                            //Update club
                            string updateClubSQL = @"Update Club Set Name = @Name, League = @League, Country = @Country, StreetAddress = @StreetAddress, StreetNumber = @StreetNumber, Trainer = @Trainer,
                                                                    AssistantTrainer = @AssistantTrainer, Physiotherapist = @Physiotherapist, AssistantPhysiotherapist = @AssistantPhysiotherapist, Manager = @Manager,
                                                                    ValueDescription = @ValueDescription, PreferenceDescription = @PreferenceDescription, ImagePath = @ImagePath, IsAvailable = @IsAvailable, ZipcodeCity_ID = @ZipcodeCity_ID
                                                                 Where Email = @Email AND RowID = @RowID";


                            _rowCountList.Add(conn.Execute(updateClubSQL, new {
                                Name = entity.Name,
                                entity.League,
                                entity.Country,
                                entity.StreetAddress,
                                entity.StreetNumber,
                                entity.Trainer,
                                entity.AssistantTrainer,
                                entity.Physiotherapist,
                                entity.AssistantPhysiotherapist,
                                entity.Manager,
                                entity.ValueDescription,
                                entity.PreferenceDescription,
                                entity.ImagePath,
                                Email = entity.Email,
                                IsAvailable = entity.IsAvailable,
                                ZipcodeCity_ID = zipcodeCity_ID,
                                RowID = row_ID
                            }, transaction: tran));


                            //Return club ID
                            string clubIDSQL = @"Select id from Club where email = @Email";
                            int club_ID = conn.Query<int>(clubIDSQL, new { Email = entity.Email }, transaction: tran).FirstOrDefault();


                        //Facility image
                        if (entity.FacilityImagesList.Count > 0) {
                            
                                foreach (string imagePath in entity.FacilityImagesList) {
                                
                                    if(club_ID != 0) {

                                        //Check if imagePath already exist in DB
                                        string facilityImageIDSQL = @"Select id from FacilityImage where imagePath = @ImagePath";
                                        int id = conn.Query<int>(facilityImageIDSQL, new { ImagePath = imagePath }, transaction: tran).FirstOrDefault();
                                        
                                        if(id == 0) {
                                            //Insert facility image
                                            string facilityImageSQL = @"INSERT INTO FacilityImage (ImagePath, Club_ID) 
                                            VALUES (@ImagePath, @Club_ID)";

                                            _rowCountList.Add(conn.Execute(facilityImageSQL, new {
                                                ImagePath = imagePath,
                                                Club_ID = club_ID
                                            }, transaction: tran));
                                        }
                                     
                                    }     
                                }
                            }

                        // CurrentSquadPlayers
                        if (entity.CurrentSquadPlayersList.Count > 0) {

                            foreach (SquadPlayer csp in entity.CurrentSquadPlayersList) {
                                
                                //Update SquadPlayer
                                string updateCurrentSquadPlayersSQL = @"Update SquadPlayers Set ShirtNumber = @ShirtNumber, Season = @Season, Name = @Name, Position = @Position
                                                                 Where ID = @ID";

                                _rowCountList.Add(conn.Execute(updateCurrentSquadPlayersSQL, new {
                                    csp.ShirtNumber,
                                    csp.Season,
                                    csp.Name,
                                    csp.Position,
                                    ID = csp.Id,
                                }, transaction: tran));
                            }
                        }

                        // NextYearSquadPlayers
                        if (entity.NextYearSquadPlayersList.Count > 0) {

                            foreach (SquadPlayer csp in entity.NextYearSquadPlayersList) {
                                
                                //Update SquadPlayer
                                string updateNextYearSquadPlayersSQL = @"Update SquadPlayers Set ShirtNumber = @ShirtNumber, Season = @Season, Name = @Name, Position = @Position
                                                                 Where ID = @ID";

                                _rowCountList.Add(conn.Execute(updateNextYearSquadPlayersSQL, new {
                                    csp.ShirtNumber,
                                    csp.Season,
                                    csp.Name,
                                    csp.Position,
                                    ID = csp.Id
                                }, transaction: tran));

                            }
                        }


                        //Job position
                        if (entity.JobPositionsList.Count > 0) {
                            foreach (JobPosition jp in entity.JobPositionsList) {

                                var jobPosition_ID = 0;
                               
                                //Update JobPosition
                                string jobPositionSQL = @"Update JobPosition Set League = @League, PreferredHand = @PreferredHand, Height = @Height, MinAge = @MinAge, MaxAge = @MaxAge, Season = @Season, 
                                                                    ContractStatus = @ContractStatus, Position = @Position
                                                          Where ID = @ID";

                                jobPosition_ID = conn.Query<int>(jobPositionSQL, new {
                                    League = jp.League,
                                    PreferredHand = jp.PreferredHand,
                                    Height = jp.Height,
                                    MinAge = jp.MinAge,
                                    MaxAge = jp.MaxAge,
                                    Season = jp.Season,
                                    ContractStatus = jp.ContractStatus,
                                    Position = jp.Position,
                                    ID = jp.Id
                                }, transaction: tran).Single();


                                if (jp.StrengthsList.Count > 0) {
                                    foreach (string strength in jp.StrengthsList) {

                                        //Return strength ID
                                        string strengthSQL = @"Select id from Strength where name = @Name";
                                        int strength_ID = conn.Query<int>(strengthSQL, new { Name = strength }, transaction: tran).FirstOrDefault();

                                        if (strength_ID != 0) {

                                            //Update JobPositionStrength
                                            string updateJobPositionStrengthSQL = @"Update JobPositionStrength Set Strength_ID = @Strength_ID
                                                                 Where JobPosition_ID = @ID";

                                            _rowCountList.Add(conn.Execute(updateJobPositionStrengthSQL, new {
                                                Strength_ID = strength_ID,
                                                ID = jp.Id
                                            }, transaction: tran));
                                        }

                                    }

                                }

                            }
                        }

                        
                        //Values
                        if (entity.ValuesList.Count > 0) {

                            foreach (string value in entity.ValuesList) {

                                //Return value ID
                                string valueSQL = @"Select id from Value where name = @Name";
                                int value_ID = conn.Query<int>(valueSQL, new { Name = value }, transaction: tran).FirstOrDefault();

                                if (value_ID != 0 && club_ID != 0) {

                                    //Update ClubValue
                                    string updateClubValueSQL = @"Update ClubValue Set Value_ID = @Value_ID
                                                                 Where Club_ID = @Club_ID";

                                    _rowCountList.Add(conn.Execute(updateClubValueSQL, new {
                                        Value_ID = value_ID,
                                        Club_ID = club_ID
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

                                if (preference_ID != 0 && club_ID != 0) {

                                    //Update ClubPreference
                                    string updateClubPreferenceSQL = @"Update ClubPreference Set Preference_ID = @Preference_ID
                                                                 Where Club_ID = @Club_ID";

                                    _rowCountList.Add(conn.Execute(updateClubPreferenceSQL, new {
                                        Preference_ID = preference_ID,
                                        Club_ID = club_ID
                                    }, transaction: tran));
                                }
                            }
                        }

                        // TrainingHours
                        if (entity.TrainingHoursList.Count > 0) {

                            foreach (TrainingHours th in entity.TrainingHoursList) {


                                //Update TrainingHours
                                string trainingHoursSQL = @"Update TrainingHours Set Name = @Name, Mon = @Mon, Tue = @Tue, Wed = @Wed, Thu = @Thu, Fri = @Fri, Sat = @Sat, Sun = @Sun
                                                                 Where ID = @ID";

                                _rowCountList.Add(conn.Execute(trainingHoursSQL, new {
                                    th.Name,
                                    th.Mon,
                                    th.Tue,
                                    th.Wed,
                                    th.Thu,
                                    th.Fri,
                                    th.Sat,
                                    th.Sun,
                                    ID = th.Id
                                }, transaction: tran));
                            }
                        }

                        //Check for 0 in rowcount list
                        if (_rowCountList.Contains(0)) {
                                c.ErrorMessage = "The club was not updated";
                                tran.Rollback();
                            }
                            else {
                                c.ErrorMessage = "";
                                tran.Commit();
                                break;
                            }
                        //}
                        //catch (SqlException e) {

                        //    tran.Rollback();
                        //    c.ErrorMessage = ErrorHandling.Exception(e);
                        //}
                    }
                }
            }
            return c;
        }

        public string DeleteJobPosition(List<JobPosition> jpl) {

            string errorMessage = "";
            
            for (int i = 0; i < 5; i++) {

                List<int> _rowCountList = new List<int>();

                using (var conn = Connection()) {

                    using (IDbTransaction tran = conn.BeginTransaction()) {
                        //try {
                        

                        if (jpl.Count > 0) {

                            foreach (JobPosition jp in jpl) {

                                //Return row ID
                                string rowIDSQL = @"Select rowID from JobPosition where ID = @ID";
                                byte[] row_ID = conn.Query<byte[]>(rowIDSQL, new { ID = jp.Id }, transaction: tran).Single();

                                //Delete jobPosition
                                string deleteJobPositionSQL = @"Delete From JobPosition Where ID = @ID AND RowID = @RowID";

                                _rowCountList.Add(conn.Execute(deleteJobPositionSQL, new {
                                    ID = jp.Id,
                                    RowID = row_ID
                                }, transaction: tran));
                            }
                        }

                        //Check for 0 in rowcount list
                        if (_rowCountList.Contains(0)) {
                            errorMessage = "JobPosition was not deleted";
                            tran.Rollback();
                        }
                        else {
                            errorMessage = "";
                            tran.Commit();
                            break;
                        }
                        //}
                        //catch (SqlException e) {

                        //    tran.Rollback();
                        //    c.ErrorMessage = ErrorHandling.Exception(e);
                        //}

                    }
                }
            }
            return errorMessage;
        }

        public string DeleteJobPositionStrength(List<string> jpsl, int jobPosition_ID) {

            string errorMessage = "";

            for (int i = 0; i < 5; i++) {

                List<int> _rowCountList = new List<int>();

                using (var conn = Connection()) {

                    using (IDbTransaction tran = conn.BeginTransaction()) {
                        //try {

                        if (jpsl.Count > 0) {

                            foreach (string jps in jpsl) {

                                //Return row ID
                                string rowIDSQL = @"Select rowID from JobPositionStrength where JobPosition_ID = @ID";
                                byte[] row_ID = conn.Query<byte[]>(rowIDSQL, new { ID = jobPosition_ID }, transaction: tran).Single();

                                //Return Strength ID
                                string strengthSQL = @"Select id from Strength where name = @Name";
                                int strength_ID = conn.Query<int>(strengthSQL, new { Name = jps }, transaction: tran).FirstOrDefault();

                                if (strength_ID != 0) {

                                    //Delete jobPositionStrength
                                    string jobPositionStrengthSQL = @"Delete From JobPositionStrength Where Strength_ID = @Strength_ID, JobPosition_ID = @JobPosition_ID AND RowID = @RowID";

                                    _rowCountList.Add(conn.Execute(jobPositionStrengthSQL, new {
                                        Strength_ID = strength_ID,
                                        JobPosition_ID = jobPosition_ID,
                                        RowID = row_ID
                                    }, transaction: tran));
                                }
                            }
                        }

                        //Check for 0 in rowcount list
                        if (_rowCountList.Contains(0)) {
                            errorMessage = "JobPosition Strength was not deleted";
                            tran.Rollback();
                        }
                        else {
                            errorMessage = "";
                            tran.Commit();
                            break;
                        }
                        //}
                        //catch (SqlException e) {

                        //    tran.Rollback();
                        //    c.ErrorMessage = ErrorHandling.Exception(e);
                        //}

                    }
                }
            }
            return errorMessage;
        }

        public string DeleteSquadPlayer(List<SquadPlayer> spl) {

            string errorMessage = "";

            for (int i = 0; i < 5; i++) {

                List<int> _rowCountList = new List<int>();

                using (var conn = Connection()) {

                    using (IDbTransaction tran = conn.BeginTransaction()) {
                        //try {

                        if (spl.Count > 0) {

                            foreach (SquadPlayer sp in spl) {

                                //Return row ID
                                string rowIDSQL = @"Select rowID from SquadPlayer where ID = @ID";
                                byte[] row_ID = conn.Query<byte[]>(rowIDSQL, new { ID = sp.Id }, transaction: tran).Single();

                                //Delete squadplayers
                                string squadPlayerSQL = @"Delete From SquadPlayer Where ID = @ID AND rowID = @RowID";

                                _rowCountList.Add(conn.Execute(squadPlayerSQL, new {
                                    ID = sp.Id,
                                    RowID = row_ID
                                }, transaction: tran));
                            }
                        }

                        //Check for 0 in rowcount list
                        if (_rowCountList.Contains(0)) {
                            errorMessage = "Squadplayer was not deleted";
                            tran.Rollback();
                        }
                        else {
                            errorMessage = "";
                            tran.Commit();
                            break;
                        }
                        //}
                        //catch (SqlException e) {

                        //    tran.Rollback();
                        //    c.ErrorMessage = ErrorHandling.Exception(e);
                        //}

                    }
                }
            }
            return errorMessage;
        }

        public string DeleteClubValue(List<string> cvl, int club_ID) {

            string errorMessage = "";

            for (int i = 0; i < 5; i++) {

                List<int> _rowCountList = new List<int>();

                using (var conn = Connection()) {

                    using (IDbTransaction tran = conn.BeginTransaction()) {
                        //try {

                        if (cvl.Count > 0) {

                            foreach (string v in cvl) {

                                //Return row ID
                                string rowIDSQL = @"Select rowID from ClubValue where Club_ID = @ID";
                                byte[] row_ID = conn.Query<byte[]>(rowIDSQL, new { ID = club_ID }, transaction: tran).Single();

                                //Return value ID
                                string valueSQL = @"Select id from Value where name = @Name";
                                int value_ID = conn.Query<int>(valueSQL, new { Name = v }, transaction: tran).FirstOrDefault();

                                if (value_ID != 0) {

                                    //Delete value
                                    string deleteClubValueSQL = @"Delete From ClubValue Where Value_ID = @Value_ID, Club_ID = @Club_ID AND rowID = @RowID";

                                    _rowCountList.Add(conn.Execute(deleteClubValueSQL, new {
                                        Value_ID = value_ID,
                                        Club_ID = club_ID,
                                        RowID = row_ID
                                    }, transaction: tran));
                                }
                            }
                        }

                        //Check for 0 in rowcount list
                        if (_rowCountList.Contains(0)) {
                            errorMessage = "Club value was not deleted";
                            tran.Rollback();
                        }
                        else {
                            errorMessage = "";
                            tran.Commit();
                            break;
                        }
                        //}
                        //catch (SqlException e) {

                        //    tran.Rollback();
                        //    c.ErrorMessage = ErrorHandling.Exception(e);
                        //}

                    }
                }
            }
            return errorMessage;
        }

        public string DeleteClubPreference(List<string> cpl, int club_ID) {

            string errorMessage = "";

            for (int i = 0; i < 5; i++) {

                List<int> _rowCountList = new List<int>();

                using (var conn = Connection()) {

                    using (IDbTransaction tran = conn.BeginTransaction()) {
                        //try {

                        if (cpl.Count > 0) {

                            foreach (string p in cpl) {

                                //Return row ID
                                string rowIDSQL = @"Select rowID from ClubPreference where club_ID = @ID";
                                byte[] row_ID = conn.Query<byte[]>(rowIDSQL, new { ID = club_ID }, transaction: tran).Single();

                                //Return preference ID
                                string preferenceSQL = @"Select id from Preference where name = @Name";
                                int preference_ID = conn.Query<int>(preferenceSQL, new { Name = p }, transaction: tran).FirstOrDefault();

                                if (preference_ID != 0) {

                                    //Delete value
                                    string deleteClubPreferenceSQL = @"Delete From ClubPreference Where Preference_ID = @Preference_ID, Club_ID = @Club_ID AND rowID = @RowID";

                                    _rowCountList.Add(conn.Execute(deleteClubPreferenceSQL, new {
                                        Preference_ID = preference_ID,
                                        Club_ID = club_ID,
                                        RowID = row_ID
                                    }, transaction: tran));
                                }
                            }
                        }

                        //Check for 0 in rowcount list
                        if (_rowCountList.Contains(0)) {
                            errorMessage = "Club preference was not deleted";
                            tran.Rollback();
                        }
                        else {
                            errorMessage = "";
                            tran.Commit();
                            break;
                        }
                        //}
                        //catch (SqlException e) {

                        //    tran.Rollback();
                        //    c.ErrorMessage = ErrorHandling.Exception(e);
                        //}

                    }
                }
            }
            return errorMessage;
        }

        public string DeleteFacilityImage(List<string> fil, int club_ID) {

            string errorMessage = "";

            for (int i = 0; i < 5; i++) {

                List<int> _rowCountList = new List<int>();

                using (var conn = Connection()) {

                    using (IDbTransaction tran = conn.BeginTransaction()) {
                        //try {

                        if (fil.Count > 0) {

                            foreach (string fi in fil) {

                                //Return row ID
                                string rowIDSQL = @"Select rowID from FacilityImage where ImagePath = @ImagePath";
                                byte[] row_ID = conn.Query<byte[]>(rowIDSQL, new { ImagePath = fi }, transaction: tran).Single();

                                //Delete facility image
                                string facilityImageSQL = @"Delete From FacilityImage Where ImagePath = @ImagePath";

                                _rowCountList.Add(conn.Execute(facilityImageSQL, new {
                                    ImagePath = fi,
                                    RowID = row_ID
                                }, transaction: tran));
                            }
                        }

                        //Check for 0 in rowcount list
                        if (_rowCountList.Contains(0)) {
                            errorMessage = "FacilityImage was not deleted";
                            tran.Rollback();
                        }
                        else {
                            errorMessage = "";
                            tran.Commit();
                            break;
                        }
                        //}
                        //catch (SqlException e) {

                        //    tran.Rollback();
                        //    c.ErrorMessage = ErrorHandling.Exception(e);
                        //}

                    }
                }
            }
            return errorMessage;
        }

        public string DeleteTrainingHours(List<TrainingHours> thl) {

            string errorMessage = "";

            for (int i = 0; i < 5; i++) {

                List<int> _rowCountList = new List<int>();

                using (var conn = Connection()) {

                    using (IDbTransaction tran = conn.BeginTransaction()) {
                        //try {

                        if (thl.Count > 0) {

                            foreach (TrainingHours th in thl) {

                                //Return row ID
                                string rowIDSQL = @"Select rowID from TrainingHours where ID = @ID";
                                byte[] row_ID = conn.Query<byte[]>(rowIDSQL, new { ID = th.Id }, transaction: tran).Single();

                                //Delete trainingHours
                                string trainingHoursSQL = @"Delete From TrainingHours Where ID = @ID AND rowID = @rowID";

                                _rowCountList.Add(conn.Execute(trainingHoursSQL, new {
                                    ID = th.Id,
                                    RowID = row_ID
                                }, transaction: tran));
                            }
                        }

                        //Check for 0 in rowcount list
                        if (_rowCountList.Contains(0)) {
                            errorMessage = "Squadplayer was not deleted";
                            tran.Rollback();
                        }
                        else {
                            errorMessage = "";
                            tran.Commit();
                            break;
                        }
                        //}
                        //catch (SqlException e) {

                        //    tran.Rollback();
                        //    c.ErrorMessage = ErrorHandling.Exception(e);
                        //}

                    }
                }
            }
            return errorMessage;
        }


        //Helping method to build club traininghours
        private Club GetClubTraningHourList(Club club, IDbConnection conn) {
            club.TrainingHoursList = conn.Query<TrainingHours>("select * from TrainingHours where club_ID = @id", new { id = club.Id }).ToList();
            return club;
        }

        //Helping method to build club facilityImages
        private Club GetClubFacilityImagesList(Club club, IDbConnection conn) {
            club.FacilityImagesList = conn.Query<string>("select imagePath from FacilityImage where club_ID = @id", new { id = club.Id }).ToList();
            return club;
        }

        //Helping method to build club current squad list
        private Club GetClubCurrentSquadList(Club club, IDbConnection conn) {
            club.CurrentSquadPlayersList = conn.Query<SquadPlayer, string, SquadPlayer>("select s.*, p.* from SquadPlayers s" +
                    " inner join Position p on p.id = s.position_ID where s.club_id = @id and s.season = 'Current year'",
            (squadPlayers, position) => { squadPlayers.Position = position; return squadPlayers; }, new { id = club.Id }, splitOn: "name").ToList();
            return club;
        }

        //Helping method to build club next year squad list
        private Club GetClubNextYearSquadList(Club club, IDbConnection conn) {
            club.NextYearSquadPlayersList = conn.Query<SquadPlayer, string, SquadPlayer>("select s.*, p.* from SquadPlayers s" +
                     " inner join Position p on p.id = s.position_ID where s.club_id = @id and s.season = 'Next year'",
                (squadPlayers, position) => { squadPlayers.Position = position; return squadPlayers; }, new { id = club.Id }, splitOn: "name").ToList();
            return club;
        }

        //Helping method to build club open position list
        private Club GetJobPosition(Club club, IDbConnection conn) {
            club.JobPositionsList = conn.Query<JobPosition, string, JobPosition>("select jp.*, p.name from JobPosition jp " +
                "inner join Position p on p.id = jp.position_ID where jp.club_ID = @id",
                (jobPosition, position) => { jobPosition.Position = position; return jobPosition; }, new { id = club.Id }, splitOn: "name").ToList();

            foreach (JobPosition item in club.JobPositionsList) {
                item.StrengthsList = conn.Query<string>("select s.name from Strength s " +
                    "inner join JobPositionStrength jps on jps.strength_id = s.id where jps.jobposition_ID = @newid", new { newid = item.Id }).ToList();
            }
            return club;
        }

        //Helping method to build club value list
        private Club GetClubValueList(Club club, IDbConnection conn) {
            club.ValuesList = conn.Query<string>("select v.name from Value v " +
                 "inner join ClubValue cv on cv.value_id = v.id where cv.club_ID = @id", new { id = club.Id }).ToList();
            return club;
        }

        //Helping method to build club strength list
        private Club GetClubPreferenceList(Club club, IDbConnection conn) {
            club.PreferenceList = conn.Query<string>("select p.name from Preference p" +
                " inner join ClubPreference cp on cp.preference_id = p.id where cp.club_ID = @id", new { id = club.Id }).ToList();
            return club;
        }

        public IEnumerable<Club> GetBySearchCriteria(string sqlStatement) {
            throw new NotImplementedException();
        }
    }
}
