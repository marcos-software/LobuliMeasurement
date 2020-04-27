using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace LobuliMeasurement
{
    public class MySQL
    {
        public MySqlConnection Connection;
        public int _waitTime = 5;
        public MySQL(
            bool autoConnect = true)
        {
            if (autoConnect)
            {
                ConnectToMySQL();
            }
        }

        public void ConnectToMySQL()
        {
            if (Connection == null || Connection.State == ConnectionState.Broken || Connection.State == ConnectionState.Closed)
            {
                Connection = new MySqlConnection($"SERVER={Config.MySQLServer};DATABASE={Config.MySQLDatabase};UID={Config.MySQLUser};PASSWORD={Config.MySQLPass};");
                Connection.Open();
            }
        }

        ~MySQL()
        {
            DisconnectFromMySQL();
        }

        public void DisconnectFromMySQL()
        {
            if (Connection != null && Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
            }
        }

        public string GetUserPasswordHash(string mail)
        {

            string query = $"SELECT cPasswordHash FROM vUsers WHERE cMail = @mail";
            
       
            MySqlCommand cmd = new MySqlCommand(query, Connection);
            cmd.Parameters.AddWithValue("@mail", mail);

            MySqlDataReader reader = cmd.ExecuteReader();
            string passwordHash = string.Empty;
                
            while (reader.Read())
            {
                passwordHash = reader.GetString(0);
            }

            return passwordHash;
        }

        public List<Cut> GetCut(int minId, int maxId, string orderBy = "kCut")
        {
            List<Cut> result = new List<Cut>();
            try
            {
                string query = $"SELECT * FROM vCut WHERE kCut >= {minId} AND kCut <= {maxId} ORDER BY {orderBy}";

                MySqlCommand cmd = new MySqlCommand(query, Connection)
                {
                    CommandType = CommandType.Text
                };

                using (MySqlDataReader mySqlDataReader = cmd.ExecuteReader())
                {
                    while (mySqlDataReader.Read())
                    {
                        //Cut cut = new Cut();

                        //cut.Id = Convert.ToInt16(mySqlDataReader.GetFieldValue<UInt16>(0));
                        //cut.Age = Convert.ToInt16(mySqlDataReader.GetFieldValue<byte>(1));
                        //cut.Genotype = mySqlDataReader.GetFieldValue<string>(2);
                        //cut.Animal = mySqlDataReader.GetFieldValue<string>(3);
                        //cut.CutIdentifier = mySqlDataReader.GetFieldValue<string>(4);
                        //cut.Method = mySqlDataReader.GetFieldValue<string>(5);

                        //if (DateTime.TryParse(mySqlDataReader.GetValue(6)?.ToString(), out var dateMeasurement))
                        //{
                        //    cut.DateMeasurement = dateMeasurement;
                        //}

                        //if (DateTime.TryParse(mySqlDataReader.GetValue(7)?.ToString(), out var dateStaining))
                        //{
                        //    cut.DateStaining = dateStaining;
                        //}

                        //cut.ZoomFactor = (float) Convert.ToDouble(mySqlDataReader.GetFieldValue<Single>(8));
                        //cut.Layer = mySqlDataReader.GetFieldValue<string>(9);
                        //cut.Note = mySqlDataReader.GetFieldValue<string>(10);

                        //result.Add(cut);

                        try
                        {
                            Cut cut = new Cut();

                            try
                            {
                                cut.Id = Convert.ToInt16(mySqlDataReader.GetFieldValue<UInt16>(0));
                            }
                            catch (Exception ex)
                            {
                                cut.Id = -1;
                            }

                            try
                            {
                                cut.Age = Convert.ToInt16(mySqlDataReader.GetFieldValue<byte>(1));
                            }
                            catch (Exception ex)
                            {
                                cut.Age = -1;
                            }

                            try
                            {
                                cut.Genotype = mySqlDataReader.GetFieldValue<string>(2);
                            }
                            catch (Exception ex)
                            {
                                cut.Genotype = "KO";
                            }

                            try
                            {
                                cut.Animal = mySqlDataReader.GetFieldValue<string>(3);
                            }
                            catch (Exception ex)
                            {
                                cut.Animal = "unknown";
                            }

                            try
                            {
                                cut.CutIdentifier = mySqlDataReader.GetFieldValue<string>(4);
                            }
                            catch (Exception ex)
                            {
                                cut.CutIdentifier = "unknown";
                            }

                            try
                            {
                                cut.Method = mySqlDataReader.GetFieldValue<string>(5);
                            }
                            catch (Exception ex)
                            {
                                cut.Method = "unknown";
                            }

                            try
                            {
                                if (DateTime.TryParse(mySqlDataReader.GetValue(6)?.ToString(), out var dateMeasurement))
                                {
                                    cut.DateMeasurement = dateMeasurement;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                                    ex.Message);
                            }

                            try
                            {
                                if (DateTime.TryParse(mySqlDataReader.GetValue(7)?.ToString(), out var dateStaining))
                                {
                                    cut.DateStaining = dateStaining;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                                    ex.Message);
                            }

                            try
                            {
                                cut.ZoomFactor = (float)Convert.ToDouble(mySqlDataReader.GetFieldValue<Single>(8));
                            }
                            catch (Exception ex)
                            {
                                cut.ZoomFactor = -1;
                            }

                            try
                            {
                                cut.Layer = mySqlDataReader.GetFieldValue<string>(9);
                            }
                            catch (Exception ex)
                            {
                                cut.Layer = "unknown";
                            }

                            try
                            {
                                cut.Note = mySqlDataReader.GetFieldValue<string>(10);
                            }
                            catch (Exception ex)
                            {
                                cut.Note = "unknown";
                            }

                            result.Add(cut);
                        }
                        catch (Exception ex)
                        {
                            MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                                ex.Message);
                        }
                    }

                    mySqlDataReader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    ex.Message);
            }

            return result;
        }

        public Cut GetCut(string kAge, string cGenoType, string cAnimal, string cCutIdentifier)
        {
            string query = $"SELECT * FROM vCut WHERE kAge = '{kAge}' AND cGenoType = '{cGenoType}' AND cAnimal = '{cAnimal}' AND cCutIdentifier = '{cCutIdentifier}' LIMIT 1";

            MySqlCommand cmd = new MySqlCommand(query, Connection)
            {
                CommandType = CommandType.Text
            };

            using (MySqlDataReader mySqlDataReader = cmd.ExecuteReader())
            {
                while (mySqlDataReader.Read())
                {
                    try
                    {
                        Cut cut = new Cut();

                        try
                        {
                            cut.Id = Convert.ToInt16(mySqlDataReader.GetFieldValue<UInt16>(0));
                        }
                        catch (Exception ex)
                        {
                            cut.Id = -1;
                        }

                        try
                        {
                            cut.Age = Convert.ToInt16(mySqlDataReader.GetFieldValue<byte>(1));
                        }
                        catch (Exception ex)
                        {
                            cut.Age = -1;
                        }

                        try
                        {
                            cut.Genotype = mySqlDataReader.GetFieldValue<string>(2);
                        }
                        catch (Exception ex)
                        {
                            cut.Genotype = "KO";
                        }

                        try
                        {
                            cut.Animal = mySqlDataReader.GetFieldValue<string>(3);
                        }
                        catch (Exception ex)
                        {
                            cut.Animal = "unknown";
                        }

                        try
                        {
                            cut.CutIdentifier = mySqlDataReader.GetFieldValue<string>(4);
                        }
                        catch (Exception ex)
                        {
                            cut.CutIdentifier = "unknown";
                        }

                        try
                        {
                            cut.Method = mySqlDataReader.GetFieldValue<string>(5);
                        }
                        catch (Exception ex)
                        {
                            cut.Method = "unknown";
                        }

                        try
                        {
                            if (DateTime.TryParse(mySqlDataReader.GetValue(6)?.ToString(), out var dateMeasurement))
                            {
                                cut.DateMeasurement = dateMeasurement;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                                ex.Message);
                        }

                        try
                        {
                            if (DateTime.TryParse(mySqlDataReader.GetValue(7)?.ToString(), out var dateStaining))
                            {
                                cut.DateStaining = dateStaining;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                                ex.Message);
                        }

                        try
                        {
                            cut.ZoomFactor = (float)Convert.ToDouble(mySqlDataReader.GetFieldValue<Single>(8));
                        }
                        catch (Exception ex)
                        {
                            cut.ZoomFactor = -1;
                        }

                        try
                        {
                            cut.Layer = mySqlDataReader.GetFieldValue<string>(9);
                        }
                        catch (Exception ex)
                        {
                            cut.Layer = "unknown";
                        }

                        try
                        {
                            cut.Note = mySqlDataReader.GetFieldValue<string>(10);
                        }
                        catch (Exception ex)
                        {
                            cut.Note = "unknown";
                        }

                        return cut;
                    }
                    catch (Exception ex)
                    {
                        MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                            ex.Message);
                    }
                }

                mySqlDataReader.Close();
            }

            return null;
        }

        public List<Cut> GetCut(bool includeExcludetCuts = true, string orderBy = "kAge, cGenotype, cAnimal, cCutIdentifier")
        {
            List<Cut> result = new List<Cut>();

            string query = $"SELECT * FROM vCut ORDER BY {orderBy}";

            MySqlCommand cmd = new MySqlCommand(query, Connection)
            {
                CommandType = CommandType.Text
            };

            using (MySqlDataReader mySqlDataReader = cmd.ExecuteReader())
            {
                while (mySqlDataReader.Read())
                {
                    try
                    {
                        Cut cut = new Cut();

                        try
                        {
                            cut.Id = Convert.ToInt16(mySqlDataReader.GetFieldValue<UInt16>(0));
                        }
                        catch (Exception ex)
                        {
                            cut.Id = -1;
                        }

                        try
                        {
                            cut.Age = Convert.ToInt16(mySqlDataReader.GetFieldValue<byte>(1));
                        }
                        catch (Exception ex)
                        {
                            cut.Age = -1;
                        }

                        try
                        {
                            cut.Genotype = mySqlDataReader.GetFieldValue<string>(2);
                        }
                        catch (Exception ex)
                        {
                            cut.Genotype = "KO";
                        }

                        try
                        {
                            cut.Animal = mySqlDataReader.GetFieldValue<string>(3);
                        }
                        catch (Exception ex)
                        {
                            cut.Animal = "unknown";
                        }

                        try
                        {
                            cut.CutIdentifier = mySqlDataReader.GetFieldValue<string>(4);
                        }
                        catch (Exception ex)
                        {
                            cut.CutIdentifier = "unknown";
                        }

                        try
                        {
                            cut.Method = mySqlDataReader.GetFieldValue<string>(5);
                        }
                        catch (Exception ex)
                        {
                            cut.Method = "unknown";
                        }

                        try
                        {
                            if (DateTime.TryParse(mySqlDataReader.GetValue(6)?.ToString(), out var dateMeasurement))
                            {
                                cut.DateMeasurement = dateMeasurement;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                                ex.Message);
                        }

                        try
                        {
                            if (DateTime.TryParse(mySqlDataReader.GetValue(7)?.ToString(), out var dateStaining))
                            {
                                cut.DateStaining = dateStaining;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                                ex.Message);
                        }

                        try
                        {
                            cut.ZoomFactor = (float) Convert.ToDouble(mySqlDataReader.GetFieldValue<Single>(8));
                        }
                        catch (Exception ex)
                        {
                            cut.ZoomFactor = -1;
                        }

                        try
                        {
                            cut.Layer = mySqlDataReader.GetFieldValue<string>(9);
                        }
                        catch (Exception ex)
                        {
                            cut.Layer = "unknown";
                        }

                        try
                        {
                            cut.Note = mySqlDataReader.GetFieldValue<string>(10);
                        }
                        catch (Exception ex)
                        {
                            cut.Note = "unknown";
                        }

                        try
                        {
                            if (includeExcludetCuts == false)
                            {
                                if (mySqlDataReader.GetBoolean(11))
                                {
                                    continue;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                                ex.Message);
                        }

                        result.Add(cut);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                            ex.Message);
                    }
                }

                mySqlDataReader.Close();
            }

            return result;
        }

        public List<Point> GetCoordinates(string id)
        {
            List<Point> result = new List<Point>();

            string query = $"SELECT * FROM vCoordinate WHERE kCut = {id}";

            MySqlCommand cmd = new MySqlCommand(query, Connection)
            {
                CommandType = CommandType.Text
            };

            using (MySqlDataReader mySqlDataReader = cmd.ExecuteReader())
            {
                while (mySqlDataReader.Read())
                {
                    try
                    {
                        Point point = new Point();

                        try
                        {
                            point.X = Convert.ToInt32(mySqlDataReader.GetFieldValue<double>(1));
                        }
                        catch (Exception ex)
                        {
                            point.X = 0;
                        }

                        try
                        {
                            point.Y = Convert.ToInt32(mySqlDataReader.GetFieldValue<double>(2));
                        }
                        catch (Exception ex)
                        {
                            point.Y = 0;
                        }

                        result.Add(point);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                            ex.Message);
                    }
                }

                mySqlDataReader.Close();
            }

            return result;
        }

        public void GetImage(string id, string type, string localFilePath)
        {
            MySQL innerMySql = null;
            try
            {
                if (File.Exists(localFilePath))
                {
                    File.Delete(localFilePath);
                }

                string query = $"SELECT cImage FROM vImage WHERE cType = '{type}' AND kCut = {id}";

                innerMySql = new MySQL();
                innerMySql._waitTime = _waitTime;
                innerMySql.ConnectToMySQL();

                MySqlCommand cmd = new MySqlCommand(query, innerMySql.Connection);
                cmd.CommandType = CommandType.Text;

                using (MySqlDataReader mySqlDataReader = cmd.ExecuteReader())
                {
                    while (mySqlDataReader.Read())
                    {
                        try
                        {
                            int size = (int) mySqlDataReader.GetBytes(0, 0, null, 0, 0);
                            var result = new byte[size];
                            int bytesRead = 0;
                            int curPos = 0;
                            while (curPos < size)
                            {
                                bytesRead += (int) mySqlDataReader.GetBytes(0, curPos, result, curPos, size - curPos);
                                curPos += bytesRead;
                            }

                            File.WriteAllBytes(localFilePath, result);
                        }
                        catch (Exception ex)
                        {
                            MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                                ex.Message);
                        }
                    }

                    mySqlDataReader.Close();
                }

                try
                {
                    innerMySql.DisconnectFromMySQL();
                }
                catch (Exception ex)
                {
                    MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                        ex.Message);
                }

            }
            catch (Exception ex)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    ex.Message);
                try
                {
                    innerMySql.DisconnectFromMySQL();
                }
                catch (Exception exInner)
                {
                    MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                        exInner.Message);
                }
            }
            finally
            {
                if (innerMySql != null)
                {
                    try
                    {
                        innerMySql.DisconnectFromMySQL();
                    }
                    catch (Exception ex)
                    {
                        MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                            ex.Message);
                    }
                }
            }
        }

        public List<Result> GetResults(string id)
        {
            List<Result> resultList = new List<Result>();

            string query = $"SELECT * FROM vResult WHERE kCut = {id} ORDER BY cResultType, cIdentifier, cKey";

            MySqlCommand cmd = new MySqlCommand(query, Connection)
            {
                CommandType = CommandType.Text
            };

            using (MySqlDataReader mySqlDataReader = cmd.ExecuteReader())
            {
                while (mySqlDataReader.Read())
                {
                    Result result = new Result();

                    object rawResultType = mySqlDataReader.GetFieldValue<object>(1);
                    object rawIdentifier = mySqlDataReader.GetFieldValue<object>(2);
                    object rawKey = mySqlDataReader.GetFieldValue<object>(3);
                    object rawValue = mySqlDataReader.GetFieldValue<object>(4);
                    object rawUnit = mySqlDataReader.GetFieldValue<object>(5);


                    result.CutId = Convert.ToInt16(id);
                    result.ResultType = rawResultType is DBNull ? null : (string)rawResultType;
                    result.Identifier = rawIdentifier is DBNull ? null : (string) rawIdentifier;
                    result.Key = rawKey is DBNull ? null : (string)rawKey;
                    result.Value = rawValue is DBNull ? null : (string)rawValue;
                    result.Unit = rawUnit is DBNull ? null : (string)rawUnit;

                    resultList.Add(result);
                }

                mySqlDataReader.Close();
            }

            return resultList;
        }

        public void DeleteCut(Cut cut)
        {
            try
            {
                ConnectToMySQL();
                MySqlCommand cmd = new MySqlCommand("spDeleteCut", Connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("in_kAge", cut.Age);
                cmd.Parameters["in_kAge"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cGenotype", cut.Genotype);
                cmd.Parameters["in_cGenotype"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cAnimal", cut.Animal);
                cmd.Parameters["in_cAnimal"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cCutIdentifier", cut.CutIdentifier);
                cmd.Parameters["in_cCutIdentifier"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    ex.Message);
            }
            finally
            {
                DisconnectFromMySQL();
            }
        }

        public void CreateCut(Cut cut)
        {
            DeleteCut(cut);
            try
            {
                ConnectToMySQL();
                MySqlCommand cmd = new MySqlCommand("spCreateCut", Connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("in_kAge", cut.Age);
                cmd.Parameters["in_kAge"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cGenotype", cut.Genotype);
                cmd.Parameters["in_cGenotype"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cAnimal", cut.Animal);
                cmd.Parameters["in_cAnimal"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cCutIdentifier", cut.CutIdentifier);
                cmd.Parameters["in_cCutIdentifier"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cMethod", cut.Method);
                cmd.Parameters["in_cMethod"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_dDateMeasurement", cut.DateMeasurement);
                cmd.Parameters["in_dDateMeasurement"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_dDateStaining", cut.DateStaining);
                cmd.Parameters["in_dDateStaining"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_fZoomFactor", cut.ZoomFactor);
                cmd.Parameters["in_fZoomFactor"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cLayer", cut.Layer);
                cmd.Parameters["in_cLayer"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cNote", cut.Note);
                cmd.Parameters["in_cNote"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("out_kCut", cut.Id);
                cmd.Parameters["out_kCut"].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                cut.Id = Convert.ToInt16(cmd.Parameters["out_kCut"].Value.ToString());
            }
            catch (Exception ex)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    ex.Message);
            }
            finally
            {
                DisconnectFromMySQL();
            }
        }

        public void CreateNote(short cutId, string user, string note)
        {
            try
            {
                ConnectToMySQL();
                MySqlCommand cmd = new MySqlCommand("spAddNote", Connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("in_kCut", cutId);
                cmd.Parameters["in_kCut"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cNote", note);
                cmd.Parameters["in_cNote"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cMail", user);
                cmd.Parameters["in_cMail"].Direction = ParameterDirection.Input;
                
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    ex.Message);
            }
            finally
            {
                DisconnectFromMySQL();
            }
        }

        public void ExcludeFromAnalytics(short cutId)
        {
            try
            {
                ConnectToMySQL();
                MySqlCommand cmd = new MySqlCommand("spChangeExclude", Connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("in_kCut", cutId);
                cmd.Parameters["in_kCut"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    ex.Message);
            }
            finally
            {
                DisconnectFromMySQL();
            }
        }

        public void CreateOption(Option option)
        {
            try
            {
                ConnectToMySQL();
                MySqlCommand cmd = new MySqlCommand("spCreateOption", Connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("in_kCut", option.CutId);
                cmd.Parameters["in_kCut"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cKey", option.Key);
                cmd.Parameters["in_cKey"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cUnit", option.Unit);
                cmd.Parameters["in_cUnit"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cValue", option.Value);
                cmd.Parameters["in_cValue"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    ex.Message);
            }
            finally
            {
                DisconnectFromMySQL();
            }
        }

        public void CreateResultOnlyValid(Result result)
        {
            double value;
            if (Double.TryParse(result.Value, out value) == false)
            {
                return;
            }

            if (value <= 0)
            {
                return;
            }
            CreateResult(result);
        }

        public void CreateResult(Result result)
        {
            try
            {
                ConnectToMySQL();
                MySqlCommand cmd = new MySqlCommand("spCreateResult", Connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("in_kCut", result.CutId);
                cmd.Parameters["in_kCut"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cResultType", result.ResultType);
                cmd.Parameters["in_cResultType"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cIdentifier", result.Identifier);
                cmd.Parameters["in_cIdentifier"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cKey", result.Key);
                cmd.Parameters["in_cKey"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cUnit", result.Unit);
                cmd.Parameters["in_cUnit"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cValue", result.Value);
                cmd.Parameters["in_cValue"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    ex.Message);
            }
            finally
            {
                DisconnectFromMySQL();
            }
        }

        public void CreateCoordinate(short cutId, List<Point> points)
        {
            try
            {
                List<Coordinate> coordinates = new List<Coordinate>();
                foreach (Point point in points)
                {
                    coordinates.Add(new Coordinate(point));   
                }

                string jsonCoordinates = JsonConvert.SerializeObject(coordinates);

                ConnectToMySQL();
                MySqlCommand cmd = new MySqlCommand("spCreateCoordinates", Connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("in_kCut", cutId);
                cmd.Parameters["in_kCut"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_jsonCoordinates", jsonCoordinates);
                cmd.Parameters["in_jsonCoordinates"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    ex.Message);
            }
            finally
            {
                DisconnectFromMySQL();
            }
        }

        public void CreateImage(short cutId, string imageType, byte[] imageData)
        {
            try
            {
                ConnectToMySQL();
                MySqlCommand cmd = new MySqlCommand("spCreateImage", Connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("in_kCut", cutId);
                cmd.Parameters["in_kCut"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_cImageType", imageType);
                cmd.Parameters["in_cImageType"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("in_xImage", imageData);
                cmd.Parameters["in_xImage"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    ex.Message);
            }
            finally
            {
                DisconnectFromMySQL();
            }
        }
    }



    public class Cut
    {
        public short Age { get; set; }
        public string Genotype { get; set; }
        public string Animal { get; set; }
        public string CutIdentifier { get; set; }
        public string Method { get; set; }
        public DateTime DateMeasurement { get; set; }
        public DateTime DateStaining { get; set; }
        public float ZoomFactor { get; set; }
        public string Layer { get; set; }
        public string Note { get; set; }
        public short Id { get; set; }
    }

    public class Option
    {
        public short CutId { get; set; }
        public string Key { get; set; }
        public string Unit { get; set; }
        public string Value { get; set; }

    }

    public class Result
    {
        public short CutId { get; set; }
        public string ResultType { get; set; }
        public string Identifier { get; set; }
        public string Key { get; set; }
        public string Unit { get; set; }
        public string Value { get; set; }

    }

    public class Coordinate
    {
        public Coordinate(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public int X { get; set; }
        public int Y { get; set; }

    }
}
