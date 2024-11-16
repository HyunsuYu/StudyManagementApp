using System;
using System.Windows.Forms;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace StudyManagementApp
{
    sealed public class NewsManager
    {
        public struct NewsData
        {
            public string newsName, link, from;



            public NewsData(string newsName, string link, string from)
            {
                this.newsName = newsName;
                this.link = link;
                this.from = from;
            }
        }



        private MySqlConnection mySqlConnection;
        private MySqlCommand mySqlCommand;
        private MySqlDataReader mySqlDataReader;

        private bool state;

        private List<NewsData> newsDatas;



        public NewsManager()
        {
            mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", /*Personal Infos*/));
            try
            {
                mySqlConnection.Open();
                state = true;

                newsDatas = new List<NewsData>();

                mySqlCommand = new MySqlCommand("SELECT * FROM NewsTable", mySqlConnection);
                mySqlDataReader = mySqlCommand.ExecuteReader();

                while(mySqlDataReader.Read())
                {
                    if(mySqlDataReader["Use"].ToString() == "1")
                    {
                        newsDatas.Add(new NewsData(mySqlDataReader["ArticleName"].ToString(), mySqlDataReader["Link"].ToString(), mySqlDataReader["From"].ToString()));
                    }
                }

                mySqlConnection.Close();
            }
            catch
            {
                state = false;
            }
        }

        public bool State
        {
            get => state;
        }
        public List<NewsData> NewsDatas
        {
            get => newsDatas;
        }

        public void Refresh()
        {
            try
            {
                mySqlConnection.Open();
                state = true;

                newsDatas.Clear();

                while (mySqlDataReader.Read())
                {
                    if (mySqlDataReader["Use"].ToString() == "1")
                    {
                        newsDatas.Add(new NewsData(mySqlDataReader["ArticleName"].ToString(), mySqlDataReader["Link"].ToString(), mySqlDataReader["From"].ToString()));
                    }
                }

                mySqlConnection.Close();
            }
            catch
            {
                state = false;
            }
        }
    }
    sealed public class LectureManager
    {
        public struct LectureData
        {
            public string date, lectureName;
            public SelfScoreData selfScoreData;
            public List<List<AnalyzeData>> analyzeDatas;
            public List<Problem> problems;
            public List<string> codeTexts;



            public LectureData(string date, string lectureName, SelfScoreData selfScoreData, List<List<AnalyzeData>> analyzeDatas, List<Problem> problems)
            {
                this.date = date;
                this.lectureName = lectureName;
                this.selfScoreData = selfScoreData;
                this.analyzeDatas = analyzeDatas;
                this.problems = problems;
                codeTexts = new List<string>();
                for(int index = 0; index < problems.Count; index++)
                {
                    codeTexts.Add(default(string));
                }
            }
        }
        public struct SelfScoreData
        {
            public enum Degree : byte
            {
                Lowest = 1,
                LittleLow = 2,
                Middle = 3,
                LittleHigh = 4,
                Highest = 5
            };



            public Degree contentUnderstanding, codingUnderstanding;



            public SelfScoreData(Degree contentUnderstanding, Degree codingUnderstanding)
            {
                this.contentUnderstanding = contentUnderstanding;
                this.codingUnderstanding = codingUnderstanding;
            }
            public SelfScoreData(SelfScoreData selfScoreData)
            {
                this.contentUnderstanding = selfScoreData.contentUnderstanding;
                this.codingUnderstanding = selfScoreData.codingUnderstanding;
            }

            public static bool operator ==(SelfScoreData a, SelfScoreData b)
            {
                return a.codingUnderstanding == b.codingUnderstanding && a.contentUnderstanding == b.contentUnderstanding;
            }
            public static bool operator !=(SelfScoreData a, SelfScoreData b)
            {
                return !(a == b);
            }
        }
        public struct AnalyzeData
        {
            public float runTime, totalMemoryAllocation, GC_Gen1, GC_Gen2, GC_Gen3;
            public string param;



            public AnalyzeData(float runTime, float totalMemoryAllocation, float GC_Gen1, float GC_Gen2, float GC_Gen3, string param)
            {
                this.runTime = runTime;
                this.totalMemoryAllocation = totalMemoryAllocation;
                this.GC_Gen1 = GC_Gen1;
                this.GC_Gen2 = GC_Gen2;
                this.GC_Gen3 = GC_Gen3;
                this.param = param;
            }
            public AnalyzeData(AnalyzeData analyzeData)
            {
                this.runTime = analyzeData.runTime;
                this.totalMemoryAllocation = analyzeData.totalMemoryAllocation;
                this.GC_Gen1 = analyzeData.GC_Gen1;
                this.GC_Gen2 = analyzeData.GC_Gen2;
                this.GC_Gen3 = analyzeData.GC_Gen3;
                this.param = analyzeData.param;
            }

            public static bool operator ==(AnalyzeData a, AnalyzeData b)
            {
                return a.GC_Gen1 == b.GC_Gen1 && a.GC_Gen2 == b.GC_Gen2 && a.GC_Gen3 == b.GC_Gen3 && a.runTime == b.runTime && a.totalMemoryAllocation == b.totalMemoryAllocation;
            }
            public static bool operator !=(AnalyzeData a, AnalyzeData b)
            {
                return !(a == b);
            }
        }
        public struct Problem
        {
            public string name, context;
            public List<List<string>> cases;



            public Problem(string name, string context, List<List<string>> cases)
            {
                this.name = name;
                this.context = context;
                this.cases = cases;
            }
        }



        private MySqlConnection mySqlConnection;
        private MySqlCommand mySqlCommand;
        private MySqlDataReader mySqlDataReader;

        private bool state;

        private List<LectureData> lectureDatas;



        public LectureManager(string userIdentifyNumber)
        {
            mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", /*Personal Infos*/));

            lectureDatas = new List<LectureData>();
            mySqlCommand = new MySqlCommand("SELECT * FROM LectureDataTable WHERE UserIdentifyNumber = " + userIdentifyNumber, mySqlConnection);
        }

        public bool State
        {
            get => state;
        }
        public List<LectureData> LectureDatas
        {
            get => lectureDatas;
        }

        public void Refrech()
        {
            try
            {
                mySqlConnection.Open();
                state = true;

                lectureDatas.Clear();

                List<SelfScoreData> selfScoreDatas = new List<SelfScoreData>();
                List<List<List<AnalyzeData>>> analyzeDatas = new List<List<List<AnalyzeData>>>();
                List<List<int>> relavantProblems = new List<List<int>>();
                List<string> dates = new List<string>(), lectureNames = new List<string>();

                mySqlDataReader = mySqlCommand.ExecuteReader();
                while (mySqlDataReader.Read())
                {
                    selfScoreDatas.Add(mySqlDataReader["SelfScoreDataJson"].ToString() == "" ? default(SelfScoreData) : JsonConvert.DeserializeObject<SelfScoreData>(mySqlDataReader["SelfScoreDataJson"].ToString()));
                    analyzeDatas.Add(mySqlDataReader["AnalyzeDatajson"].ToString() == "" ? default(List<List<AnalyzeData>>) : JsonConvert.DeserializeObject<List<List<AnalyzeData>>>(mySqlDataReader["AnalyzeDatajson"].ToString()));
                    dates.Add(mySqlDataReader["Date"].ToString());
                    lectureNames.Add(mySqlDataReader["LectureName"].ToString());

                    relavantProblems.Add(mySqlDataReader["RelevantProblemIDJson"].ToString() == "" ? default(List<int>) : JsonConvert.DeserializeObject<List<int>>(mySqlDataReader["RelevantProblemIDJson"].ToString()));
                }
                mySqlConnection.Close();

                mySqlConnection.Open();
                MySqlCommand tempMySqlCommand = new MySqlCommand("SELECT * FROM new_schema.ProblemBackTable", mySqlConnection);
                MySqlDataReader tempMySqlDataReader = tempMySqlCommand.ExecuteReader();

                List<List<Problem>> problems = new List<List<Problem>>();

                while (tempMySqlDataReader.Read())
                {
                    for (int lectureIndex = 0; lectureIndex < relavantProblems.Count; lectureIndex++)
                    {
                        problems.Add(new List<Problem>());
                        for (int problemIndex = 0; problemIndex < relavantProblems[lectureIndex].Count; problemIndex++)
                        {
                            if (tempMySqlDataReader["ID"].ToString() == relavantProblems[lectureIndex][problemIndex].ToString())
                            {
                                string casesJsonText = tempMySqlDataReader["SampleCasesJson"].ToString();
                                List<List<string>> cases = casesJsonText == "" ? new List<List<string>>() : JsonConvert.DeserializeObject<List<List<string>>>(casesJsonText);

                                problems[lectureIndex].Add(new Problem(tempMySqlDataReader["ProblemName"].ToString(), tempMySqlDataReader["ProblemContext"].ToString(), cases));
                                break;
                            }
                        }
                    }
                }
                mySqlConnection.Close();

                for (int index = 0; index < relavantProblems.Count; index++)
                {
                    if(problems[index].Count == 0)
                    {
                        problems[index].Add(new Problem());
                    }
                    if(analyzeDatas[index].Count == 0)
                    {
                        analyzeDatas[index].Add(new List<AnalyzeData>());
                        analyzeDatas[index][0].Add(new AnalyzeData());
                    }

                    lectureDatas.Add(new LectureData(dates[index], lectureNames[index], selfScoreDatas[index], analyzeDatas[index], problems[index]));
                }
            }
            catch
            {
                state = false;
            }
        }
        public void UploadSelfScoreData(string date, SelfScoreData selfScoreData)
        {
            try
            {
                mySqlConnection.Open();

                string tempCommand = "UPDATE `new_schema`.`LectureDataTable` SET `SelfScoreDataJson` = '" + JsonConvert.SerializeObject(selfScoreData) + "' WHERE (`Date` = '" + date + "')";

                MySqlCommand tempMySqlCommand = new MySqlCommand(tempCommand, mySqlConnection);
                tempMySqlCommand.ExecuteNonQuery();

                tempMySqlCommand.Dispose();

                mySqlConnection.Close();
            }
            catch
            {
                ;
            }
        }
        public void UploadAnalyzeData(string date, AnalyzeData analyzeData)
        {
            try
            {
                mySqlConnection.Open();

                string tempCommand = "UPDATE `new_schema`.`LectureDataTable` SET `AnalyzeDataJson` = '" + JsonConvert.SerializeObject(analyzeData) + "' WHERE (`Date` = '" + date + "')";

                MySqlCommand tempMySqlCommand = new MySqlCommand(tempCommand, mySqlConnection);
                tempMySqlCommand.ExecuteNonQuery();

                tempMySqlCommand.Dispose();

                mySqlConnection.Close();
            }
            catch
            {
                ;
            }
            
        }
        public void UploadProblemCode(int lectureIndex)
        {
            try
            {
                mySqlConnection.Open();

                string tempCommand = "UPDATE `new_schema`.`LectureDataTable` SET `CodeTextJson` = '" + JsonConvert.SerializeObject(lectureDatas[lectureIndex].codeTexts) + "' WHERE (`Date` = '" + lectureDatas[lectureIndex].date.Substring(0, 10) + "')";

                MySqlCommand tempMySqlCommand = new MySqlCommand(tempCommand, mySqlConnection);
                tempMySqlCommand.ExecuteNonQuery();

                tempMySqlCommand.Dispose();


                mySqlConnection.Close();
            }
            catch
            {

            }
        }
    }
    sealed public class UserManager
    {
        private string identifyNumber, mainTitle, subTitle, startDay, endDay;



        public UserManager()
        {

        }

        public string IdentifyNumber
        {
            get => identifyNumber;
        }
        public string MainTitle
        {
            get => mainTitle;
        }
        public string SubTitle
        {
            get => subTitle;
        }
        public string StartDay
        {
            get => startDay;
        }
        public string EndDay
        {
            get => endDay;
        }

        public bool ProceedSignIn(string id, string password, TextBox errorTextBox)
        {
            bool state = false;
            MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", /*Personal Infos*/));

            try
            {
                mySqlConnection.Open();

                MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM UserManagementTable WHERE (ID LIKE '" + id + "') and (PW LIKE '" + password + "')", mySqlConnection);
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();

                while (mySqlDataReader.Read())
                {
                    state = true;

                    identifyNumber = mySqlDataReader["IdentifyNumber"].ToString();
                    mainTitle = mySqlDataReader["MainTitle"].ToString();
                    subTitle = mySqlDataReader["SubTitle"].ToString();

                    break;
                }

                if(!state)
                {
                    errorTextBox.Text = "ID or password is diffrent";
                }

                mySqlCommand.Dispose();
                mySqlDataReader.Dispose();

                mySqlConnection.Close();
            }
            catch
            {
                state = false;

                errorTextBox.Text = "Please check your internet connection";
            }

            mySqlConnection.Dispose();

            return state;
        }
        public bool ProceedSignUp(string id, string password, TextBox errorTextBox)
        {
            bool state = false;
            MySqlConnection mySqlConnection = new MySqlConnection(string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", /*Personal Infos*/));

            try
            {
                state = true;

                mySqlConnection.Open();

                string commandText = "INSERT INTO `new_schema`.`UserManagementTable` (`ID`, `Password`) VALUES ('" + id + "', '" + password + "')";
                MySqlCommand mySqlCommand = new MySqlCommand(commandText, mySqlConnection);
                mySqlCommand.ExecuteNonQuery();

                mySqlCommand.Dispose();

                mySqlConnection.Close();
            }
            catch
            {
                state = false;

                errorTextBox.Text = "Please check your internet connection";
            }

            mySqlConnection.Dispose();

            return state;
        }
    }
}
