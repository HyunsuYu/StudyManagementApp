using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;

namespace StudyManagementApp
{
    public partial class MainForm : Form
    {
        public enum DashboardKind : byte
        {
            None = 0,
            Overview = 1,
            Lecture = 2,
            Analyze = 3,
            Schedule = 4
        };
        public enum OptionKind : byte
        {
            Home = 1,
            Setting = 2,
            Help = 3
        };



        private ImageList imageList;

        private DashboardKind dashboardKind;

        private NewsManager newsManager;
        private LectureManager lectureManager;
        private UserManager userManager;

        private List<Panel> selfScorePanels_Content, selfScorePanels_Coding, simpleAnalyzePanels;
        private List<Button> lectureBtns, problemBtns;

        private bool login;

        private int curEditLecture, curEditProblem;



        public MainForm()
        {
            InitializeComponent();

            Init();
            SetDefaultLayout();
        }
        public void FocusCancel(object sender, EventArgs e)
        {
            panel_RightBack.Focus();
        }

        private void Init()
        {
            imageList = new ImageList();

            imageList.ImageSize = new Size(15, 15);
            imageList.Images.Add(Image.FromFile("C:\\Users\\lioie\\Desktop\\plus-symbol-button.png"));
            imageList.Images.Add(Image.FromFile("C:\\Users\\lioie\\Desktop\\New Piskel-1.png (23).png"));

            userManager = new UserManager();

            login = false;
        }
        private void SetDefaultLayout()
        {
            //  Set Visable Property

            //  Call Default Methods
            dashboardKind = DashboardKind.Overview;
            SetCurDashboard(DashboardKind.Overview);
            SetCurOption(OptionKind.Home);

            //  PictureBoxs
            //pictureBox_MsdnLogo.Image = imageList.Images[0];
            //pictureBox_StackoverflowLogo.Image = imageList.Images[1];

            //  Panels
            panel_NewsBackPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_NewsBackPanel.Width, panel_NewsBackPanel.Height, 20, 20));

            panel_Overview_LittleSchedule_Back.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Overview_LittleSchedule_Back.Width, panel_Overview_LittleSchedule_Back.Height, 20, 20));
            panel_Overview_SelfScoreGraph_Back.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Overview_SelfScoreGraph_Back.Width, panel_Overview_SelfScoreGraph_Back.Height, 20, 20));
            panel_Overview_AnalyzeGraph_Back.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Overview_AnalyzeGraph_Back.Width, panel_Overview_AnalyzeGraph_Back.Height, 20, 20));
            panel_Overview_LinkBtns1_Back.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Overview_LinkBtns1_Back.Width, panel_Overview_LinkBtns1_Back.Height, 20, 20));
            panel_Overview_LinkBtns2_Back.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Overview_LinkBtns2_Back.Width, panel_Overview_LinkBtns2_Back.Height, 20, 20));

            panel_SignUpBack.Visible = false;
            panel_SignUpBack.Enabled = false;

            panel_SignFailBack.Visible = false;
            panel_SignFailBack.Enabled = false;

            panel_Overview_AnalyzeGraph_KindBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Overview_AnalyzeGraph_KindBack.Width, panel_Overview_AnalyzeGraph_KindBack.Height, 20, 20));

            panel_Lecture_MainBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Lecture_MainBack.Width, panel_Lecture_MainBack.Height, 20, 20));
            panel_Lecture_LecturesBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Lecture_LecturesBack.Width, panel_Lecture_LecturesBack.Height, 20, 20));
            panel_Lecture_ProblemsBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Lecture_ProblemsBack.Width, panel_Lecture_ProblemsBack.Height, 20, 20));

            // Buttons

        }
        private void PrepareAll()
        {
            //  Alloc
            newsManager = new NewsManager();
            lectureManager = new LectureManager(userManager.IdentifyNumber);

            selfScorePanels_Content = new List<Panel>();
            selfScorePanels_Coding = new List<Panel>();
            simpleAnalyzePanels = new List<Panel>();
            lectureBtns = new List<Button>();
            problemBtns = new List<Button>();

            // News
            RefreshNews();

            //  Self Score Graph
            lectureManager.Refrech();
            RefreshSelfScoreGraph();

            //  Simple Analyze Graph
            RefreshSimpleAnalyzeGraph();

            //  Dashboard Check
            SetCurDashboard(dashboardKind);

            //  Little Schedule
            RefreshLittleSchedule();

            //  TextBox
            textBox_CourseName.Text = userManager.MainTitle;
            textBox_CourseSubName.Text = userManager.SubTitle;

            textBox_UserName.Text = textBox_signInIDText.Text;

            DateTime lowDay = DateTime.Parse(lectureManager.LectureDatas[0].date.Substring(0, 10)), highDay = lowDay;
            for(int index = 0; index < lectureManager.LectureDatas.Count; index++)
            {
                string tempDate = lectureManager.LectureDatas[index].date.Substring(0, 10);

                DateTime dateTime = DateTime.Parse(tempDate);

                if(DateTime.Compare(dateTime, lowDay) < 0)
                {
                    lowDay = dateTime;
                }
                if(DateTime.Compare(dateTime, highDay) > 0)
                {
                    highDay = dateTime;
                }
            }

            textBox_StartDay.Text = (lowDay.Month < 10 ? "0" + lowDay.Month.ToString() : lowDay.Month.ToString()) + " / " + (lowDay.Day < 10 ? "0" + lowDay.Day.ToString() : lowDay.Day.ToString());
            textBox_EndDay.Text = (highDay.Month < 10 ? "0" + highDay.Month.ToString() : highDay.Month.ToString()) + " / " + (highDay.Day < 10 ? "0" + highDay.Day.ToString() : highDay.Day.ToString());

            //  Lecture
            lectureBtns.Add(button_Lecture_LecturesBtn_1);
            lectureBtns.Add(button_Lecture_LecturesBtn_2);
            lectureBtns.Add(button_Lecture_LecturesBtn_3);
            lectureBtns.Add(button_Lecture_LecturesBtn_4);
            lectureBtns.Add(button_Lecture_LecturesBtn_5);
            lectureBtns.Add(button_Lecture_LecturesBtn_6);
            lectureBtns.Add(button_Lecture_LecturesBtn_7);
            lectureBtns.Add(button_Lecture_LecturesBtn_8);
            lectureBtns.Add(button_Lecture_LecturesBtn_9);
            lectureBtns.Add(button_Lecture_LecturesBtn_10);
            lectureBtns.Add(button_Lecture_LecturesBtn_11);
            lectureBtns.Add(button_Lecture_LecturesBtn_12);
            lectureBtns.Add(button_Lecture_LecturesBtn_13);
            lectureBtns.Add(button_Lecture_LecturesBtn_14);
            lectureBtns.Add(button_Lecture_LecturesBtn_15);

            problemBtns.Add(button_Lecture_ProblemBtn_1);
            problemBtns.Add(button_Lecture_ProblemBtn_2);
            problemBtns.Add(button_Lecture_ProblemBtn_3);
            problemBtns.Add(button_Lecture_ProblemBtn_4);
            problemBtns.Add(button_Lecture_ProblemBtn_5);

            RefreshLectures();

        }

        private void SetCurDashboard(DashboardKind dashboardKind)
        {
            DisableAllDashboard();

            switch(dashboardKind)
            {
                case DashboardKind.Overview:
                    this.dashboardKind = DashboardKind.Overview;

                    button_Overview.ForeColor = Color.White;
                    panel_OverviewBtnLine.Visible = true;

                    panel_Overview_BackPanel.Visible = true;
                    panel_Overview_BackPanel.Enabled = true;

                    //panel_Overview_LittleSchedule_Back.Visible = true;
                    //panel_Overview_LittleSchedule_Back.Enabled = true;

                    //panel_Overview_UploadNumGraph.Visible = true;
                    //panel_Overview_UploadNumGraph.Enabled = true;

                    //panel_Overview_SelfScoreGraph_Back.Visible = true;
                    //panel_Overview_SelfScoreGraph_Back.Enabled = true;

                    //panel_Overview_MsdnBtnBack.Visible = true;
                    //panel_Overview_MsdnBtnBack.Enabled = true;

                    //panel_Overview_StackBtnBack.Visible = true;
                    //panel_Overview_StackBtnBack.Enabled = true;

                    if (login)
                    {
                        for (int index = 0; index < selfScorePanels_Content.Count; index++)
                        {
                            selfScorePanels_Content[index].Visible = true;
                            selfScorePanels_Content[index].Enabled = true;
                        }
                        for (int index = 0; index < selfScorePanels_Coding.Count; index++)
                        {
                            selfScorePanels_Coding[index].Visible = true;
                            selfScorePanels_Coding[index].Enabled = true;
                        }

                        for(int index = 0; index < simpleAnalyzePanels.Count; index++)
                        {
                            simpleAnalyzePanels[index].Visible = true;
                            simpleAnalyzePanels[index].Enabled = true;
                        }
                    }
                    break;

                case DashboardKind.Lecture:
                    this.dashboardKind = DashboardKind.Lecture;

                    button_Lecture.ForeColor = Color.White;
                    panel_LectureBtnLine.Visible = true;

                    panel_Lecture_LecturesBack.Visible = true;
                    panel_Lecture_LecturesBack.Enabled = true;
                    break;

                case DashboardKind.Analyze:
                    this.dashboardKind = DashboardKind.Analyze;

                    button_Analyze.ForeColor = Color.White;
                    panel_AnalyzeBtnLine.Visible = true;

                    panel_Analyze_AnalyzeBack.Visible = true;
                    panel_Analyze_AnalyzeBack.Enabled = true;
                    break;

                case DashboardKind.Schedule:
                    this.dashboardKind = DashboardKind.Schedule;

                    button_Schedule.ForeColor = Color.White;
                    panel_ScheduleBtnLine.Visible = true;
                    break;
            }
        }
        private void DisableAllDashboard()
        {
            //  Overview
            button_Overview.ForeColor = Color.FromArgb(166, 166, 166);
            panel_OverviewBtnLine.Visible = false;

            panel_Overview_BackPanel.Visible = false;
            panel_Overview_BackPanel.Enabled = false;

            //panel_Overview_LittleSchedule_Back.Visible = false;
            //panel_Overview_LittleSchedule_Back.Enabled = false;

            //panel_Overview_UploadNumGraph.Visible = false;
            //panel_Overview_UploadNumGraph.Enabled = false;

            //panel_Overview_SelfScoreGraph_Back.Visible = false;
            //panel_Overview_SelfScoreGraph_Back.Enabled = false;
            //panel_Overview_MsdnBtnBack.Visible = false;
            //panel_Overview_MsdnBtnBack.Enabled = false;

            //panel_Overview_StackBtnBack.Visible = false;
            //panel_Overview_StackBtnBack.Enabled = false;

            if (login)
            {
                for (int index = 0; index < selfScorePanels_Content.Count; index++)
                {
                    selfScorePanels_Content[index].Visible = false;
                    selfScorePanels_Content[index].Enabled = false;
                }
                for (int index = 0; index < selfScorePanels_Coding.Count; index++)
                {
                    selfScorePanels_Coding[index].Visible = false;
                    selfScorePanels_Coding[index].Enabled = false;
                }

                for(int index = 0; index < simpleAnalyzePanels.Count; index++)
                {
                    simpleAnalyzePanels[index].Visible = false;
                    simpleAnalyzePanels[index].Enabled = false;
                }
            }

            //  Lecture
            button_Lecture.ForeColor = Color.FromArgb(166, 166, 166);
            panel_LectureBtnLine.Visible = false;

            panel_Lecture_LecturesBack.Visible = false;
            panel_Lecture_LecturesBack.Enabled = false;

            //  Analyze
            button_Analyze.ForeColor = Color.FromArgb(166, 166, 166);
            panel_AnalyzeBtnLine.Visible = false;

            panel_Analyze_AnalyzeBack.Visible = false;
            panel_Analyze_AnalyzeBack.Enabled = false;

            //  Schedule
            button_Schedule.ForeColor = Color.FromArgb(166, 166, 166);
            panel_ScheduleBtnLine.Visible = false;
        }
        private void SetCurOption(OptionKind optionKind)
        {
            DisableAllOption();
            SetCurDashboard(DashboardKind.None);

            switch(optionKind)
            {
                case OptionKind.Home:
                    button_Home.ForeColor = Color.FromArgb(253, 65, 60);

                    textBox_DashboardText.Visible = true;

                    button_Overview.Visible = true;
                    button_Overview.Enabled = true;
                    panel_OverviewBtnLine.Visible = true;

                    button_Lecture.Visible = true;
                    button_Lecture.Enabled = true;
                    panel_LectureBtnLine.Visible = true;

                    button_Analyze.Visible = true;
                    button_Analyze.Enabled = true;
                    panel_AnalyzeBtnLine.Visible = true;

                    button_Schedule.Visible = true;
                    button_Schedule.Enabled = true;
                    panel_ScheduleBtnLine.Visible = true;

                    SetCurDashboard(dashboardKind);
                    break;

                case OptionKind.Setting:
                    button_Setting.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case OptionKind.Help:
                    button_Help.ForeColor = Color.FromArgb(253, 65, 60);
                    break;
            }
        }
        private void DisableAllOption()
        {
            //  Home
            button_Home.ForeColor = Color.FromArgb(166, 166, 166);

            textBox_DashboardText.Visible = false;

            button_Overview.Visible = false;
            button_Overview.Enabled = false;
            panel_OverviewBtnLine.Visible = false;

            button_Lecture.Visible = false;
            button_Lecture.Enabled = false;
            panel_LectureBtnLine.Visible = false;

            button_Analyze.Visible = false;
            button_Analyze.Enabled = false;
            panel_AnalyzeBtnLine.Visible = false;

            button_Schedule.Visible = false;
            button_Schedule.Enabled = false;
            panel_ScheduleBtnLine.Visible = false;

            //  Setting
            button_Setting.ForeColor = Color.FromArgb(166, 166, 166);

            //  Help
            button_Help.ForeColor = Color.FromArgb(166, 166, 166);
        }
        private void SetCurLecture(int index)
        {
            curEditLecture = index;

            DisableAllLecture();

            lectureBtns[index].ForeColor = Color.FromArgb(253, 65, 60);

            for(int count = 0; count < 5; count++)
            {
                problemBtns[count].Visible = false;
                problemBtns[count].Enabled = false;
            }
            for(int count = 0; count < lectureManager.LectureDatas[index].problems.Count; count++)
            {
                problemBtns[count].Visible = true;
                problemBtns[count].Enabled = true;
            }
            SetCurProblem(0);
        }
        private void DisableAllLecture()
        {
            for(int index = 0; index < lectureBtns.Count; index++)
            {
                lectureBtns[index].ForeColor = Color.White;
            }
        }
        private void SetCurProblem(int index)
        {
            curEditProblem = index;

            DisableAllProblem();

            problemBtns[index].ForeColor = Color.FromArgb(253, 65, 60);

            textBox_Lecture_ProblemName.Text = "";
            textBox_Lecture_ProblemName.Text = " " + lectureManager.LectureDatas[curEditLecture].problems[curEditProblem].name;
            textBox_Lecture_ProblemExplaneText.Text = lectureManager.LectureDatas[curEditLecture].problems[curEditProblem].context;
            textBox_Lecture_ProblemCases.Text = "";
            for(int count = 0; count < lectureManager.LectureDatas[curEditLecture].problems[curEditProblem].cases.Count; count++)
            {
                textBox_Lecture_ProblemCases.Text += lectureManager.LectureDatas[curEditLecture].problems[curEditProblem].cases[count][0] + "  :  " + lectureManager.LectureDatas[curEditLecture].problems[curEditProblem].cases[count][1] + Environment.NewLine;
            }
            textBox_Lecture_CodeText.Text = lectureManager.LectureDatas[curEditLecture].codeTexts[curEditProblem];
        }
        private void DisableAllProblem()
        {
            for(int index = 0; index < problemBtns.Count; index++)
            {
                problemBtns[index].ForeColor = Color.White;
            }
        }
        private void RefreshNews()
        {
            if (newsManager.State)
            {
                if (newsManager.NewsDatas.Count >= 1)
                {
                    textBox_NewsName_1.Text = newsManager.NewsDatas[0].newsName;
                    textBox_NewsFrom_1.Text = newsManager.NewsDatas[0].from;
                    button_NewsLink_1.Enabled = true;
                    button_NewsLink_1.Visible = true;
                }
                else
                {
                    button_NewsLink_1.Enabled = false;
                    button_NewsLink_1.Visible = false;
                }

                if (newsManager.NewsDatas.Count >= 2)
                {
                    textBox_NewsName_2.Text = newsManager.NewsDatas[1].newsName;
                    textBox_NewsFrom_2.Text = newsManager.NewsDatas[1].from;
                    button_NewsLink_2.Enabled = true;
                    button_NewsLink_2.Visible = true;
                }
                else
                {
                    button_NewsLink_2.Enabled = false;
                    button_NewsLink_2.Visible = false;
                }

                if (newsManager.NewsDatas.Count >= 3)
                {
                    textBox_NewsName_3.Text = newsManager.NewsDatas[2].newsName;
                    textBox_NewsFrom_3.Text = newsManager.NewsDatas[2].from;
                    button_NewsLink_3.Enabled = true;
                    button_NewsLink_3.Visible = true;
                }
                else
                {
                    button_NewsLink_3.Enabled = false;
                    button_NewsLink_3.Visible = false;
                }

                if (newsManager.NewsDatas.Count >= 4)
                {
                    textBox_NewsName_4.Text = newsManager.NewsDatas[3].newsName;
                    textBox_NewsFrom_4.Text = newsManager.NewsDatas[3].from;
                    button_NewsLink_4.Enabled = true;
                    button_NewsLink_4.Visible = true;
                }
                else
                {
                    button_NewsLink_4.Enabled = false;
                    button_NewsLink_4.Visible = false;
                }
            }
        }
        private void RefreshSelfScoreGraph()
        {
            for (int index = 0; index < lectureManager.LectureDatas.Count; index++)
            {
                if (lectureManager.LectureDatas[index].selfScoreData == default(StudyManagementApp.LectureManager.SelfScoreData))
                {
                    continue;
                }

                //  Content
                if (selfScorePanels_Content.Count == 0)
                {
                    Panel panel_Content = new Panel();

                    Controls.Add(panel_Content);

                    panel_Content.Size = new Size(13, 13);
                    panel_Content.Location = new Point(269 + 44 + 20 * index, 222 + 51 + (5 - (int)lectureManager.LectureDatas[index].selfScoreData.contentUnderstanding - 1) * 19);
                    panel_Content.BackColor = Color.FromArgb(254, 188, 44);
                    panel_Content.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Content.Width, panel_Content.Height, 7, 7));
                    panel_Content.BorderStyle = BorderStyle.None;

                    panel_Content.BringToFront();

                    selfScorePanels_Content.Add(panel_Content);
                }
                else
                {
                    //  Content
                    if (lectureManager.LectureDatas[index - 1].selfScoreData.contentUnderstanding == lectureManager.LectureDatas[index].selfScoreData.contentUnderstanding)
                    {
                        selfScorePanels_Content[selfScorePanels_Content.Count - 1].Size = new Size(selfScorePanels_Content[selfScorePanels_Content.Count - 1].Size.Width + 20, 13);
                        selfScorePanels_Content[selfScorePanels_Content.Count - 1].Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, selfScorePanels_Content[selfScorePanels_Content.Count - 1].Width, selfScorePanels_Content[selfScorePanels_Content.Count - 1].Height, 7, 7));
                    }
                    else
                    {
                        Panel panel_Content = new Panel();

                        Controls.Add(panel_Content);

                        panel_Content.Size = new Size(13, 13);
                        panel_Content.Location = new Point(selfScorePanels_Content[selfScorePanels_Content.Count - 1].Location.X + selfScorePanels_Content[selfScorePanels_Content.Count - 1].Width + 7, 222 + 51 + (5 - (int)lectureManager.LectureDatas[index].selfScoreData.contentUnderstanding - 1) * 19);
                        panel_Content.BackColor = Color.FromArgb(254, 188, 44);
                        panel_Content.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Content.Width, panel_Content.Height, 7, 7));
                        panel_Content.BorderStyle = BorderStyle.None;

                        panel_Content.BringToFront();

                        selfScorePanels_Content.Add(panel_Content);
                    }
                }

                //  Coding
                if (selfScorePanels_Coding.Count == 0)
                {
                    Panel panel_Coding = new Panel();

                    Controls.Add(panel_Coding);

                    panel_Coding.Size = new Size(9, 9);
                    panel_Coding.Location = new Point(269 + 46 + 20 * index, 222 + 53 + (5 - (int)lectureManager.LectureDatas[index].selfScoreData.codingUnderstanding - 1) * 19);
                    panel_Coding.BackColor = Color.FromArgb(248, 76, 72);
                    panel_Coding.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Coding.Width, panel_Coding.Height, 5, 5));
                    panel_Coding.BorderStyle = BorderStyle.None;

                    panel_Coding.BringToFront();

                    selfScorePanels_Coding.Add(panel_Coding);
                }
                else
                {
                    //  Coding
                    if (selfScorePanels_Coding.Count > 0)
                    {
                        if (lectureManager.LectureDatas[index - 1].selfScoreData.codingUnderstanding == lectureManager.LectureDatas[index].selfScoreData.codingUnderstanding)
                        {
                            selfScorePanels_Coding[selfScorePanels_Coding.Count - 1].Size = new Size(selfScorePanels_Coding[selfScorePanels_Coding.Count - 1].Size.Width + 20, 9);
                            selfScorePanels_Coding[selfScorePanels_Coding.Count - 1].Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, selfScorePanels_Coding[selfScorePanels_Coding.Count - 1].Width, selfScorePanels_Coding[selfScorePanels_Coding.Count - 1].Height, 7, 7));

                            selfScorePanels_Coding[selfScorePanels_Coding.Count - 1].BringToFront();
                        }
                        else
                        {
                            Panel panel_Coding = new Panel();

                            Controls.Add(panel_Coding);

                            panel_Coding.Size = new Size(9, 9);
                            panel_Coding.Location = new Point(selfScorePanels_Coding[selfScorePanels_Coding.Count - 1].Location.X + selfScorePanels_Coding[selfScorePanels_Coding.Count - 1].Width + 11, 222 + 53 + (5 - (int)lectureManager.LectureDatas[index].selfScoreData.codingUnderstanding - 1) * 19);
                            panel_Coding.BackColor = Color.FromArgb(248, 76, 72);
                            panel_Coding.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Coding.Width, panel_Coding.Height, 5, 5));
                            panel_Coding.BorderStyle = BorderStyle.None;

                            panel_Coding.BringToFront();

                            selfScorePanels_Coding.Add(panel_Coding);
                        }
                    }
                }
            }
        }
        private void RefreshSimpleAnalyzeGraph()
        {
            float worstRunTime = float.MinValue;
            for(int index = 0; index < lectureManager.LectureDatas.Count; index++)
            {
                if(lectureManager.LectureDatas[index].analyzeData.runTime > worstRunTime)
                {
                    worstRunTime = lectureManager.LectureDatas[index].analyzeData.runTime;
                }
            }

            //  Y Axis Text Box
            float yAxisUnit = worstRunTime / 7.0f;
            textBox_SimpleAnalyze_Y_1.Text = "00.00 ms";
            textBox_SimpleAnalyze_Y_2.Text = yAxisUnit.ToString("00.00") + " ms";
            textBox_SimpleAnalyze_Y_3.Text = (yAxisUnit * 2.0f).ToString("00.00") + " ms";
            textBox_SimpleAnalyze_Y_4.Text = (yAxisUnit * 3.0f).ToString("00.00") + " ms";
            textBox_SimpleAnalyze_Y_5.Text = (yAxisUnit * 4.0f).ToString("00.00") + " ms";
            textBox_SimpleAnalyze_Y_6.Text = (yAxisUnit * 5.0f).ToString("00.00") + " ms";
            textBox_SimpleAnalyze_Y_7.Text = (yAxisUnit * 6.0f).ToString("00.00") + " ms";
            textBox_SimpleAnalyze_Y_8.Text = (yAxisUnit * 7.0f).ToString("00.00") + " ms";

            Random random = new Random();
            for (int index = 0; index < lectureManager.LectureDatas.Count; index++)
            {
                Panel panel = new Panel();

                Controls.Add(panel);

                int randumNum = random.Next(1, 6);
                panel.Size = new Size(randumNum * 2 + 1, randumNum * 2 + 1);
                panel.Location = new Point(107 + 20 * index - randumNum + 1, 436 - randumNum + 1 + 140 - (int)(140 * lectureManager.LectureDatas[index].analyzeData.runTime / worstRunTime));
                panel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel.Width, panel.Height, randumNum, randumNum));
                panel.BackColor = Color.FromArgb(248, 76, 72);

                panel.BringToFront();

                simpleAnalyzePanels.Add(panel);
            }
            random = null;
        }
        private void RefreshLittleSchedule()
        {
            textBox_Overvire_LittleSchedule_DateText.Text = DateTime.Now.ToString("yyyy-MM-dd");
            textBox_Overview_LittleSchedule_LectureNameText.Text = "None";

            for(int index = 0; index < lectureManager.LectureDatas.Count; index++)
            {
                if(DateTime.Now.ToString("yyyy-MM-dd") == lectureManager.LectureDatas[index].date.Substring(0, 10))
                {
                    textBox_Overview_LittleSchedule_LectureNameText.Text = lectureManager.LectureDatas[index].lectureName;
                    break;
                }
            }
        }
        private void RefreshLectures()
        {
            for(int index = lectureManager.LectureDatas.Count; index < 15; index++)
            {
                lectureBtns[index].Enabled = false;
            }

            for(int index = 0; index < lectureManager.LectureDatas.Count; index++)
            {
                lectureBtns[index].Enabled = true;
                lectureBtns[index].Text = "Ep." + (index + 1) + " " + lectureManager.LectureDatas[index].lectureName;
            }

            curEditLecture = 0;
            curEditProblem = 0;

            SetCurLecture(0);
            SetCurProblem(0);
        }



        #region DLLImports
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );
        #endregion



        #region Controls
        //  Button Controls
        private void button_Overview_Click(object sender, EventArgs e)
        {
            SetCurDashboard(DashboardKind.Overview);
        }
        private void button_Lecture_Click(object sender, EventArgs e)
        {
            SetCurDashboard(DashboardKind.Lecture);
        }
        private void button_Analyze_Click(object sender, EventArgs e)
        {
            SetCurDashboard(DashboardKind.Analyze);
        }
        private void button_Schedule_Click(object sender, EventArgs e)
        {
            SetCurDashboard(DashboardKind.Schedule);
        }

        private void button_Home_Click(object sender, EventArgs e)
        {
            SetCurOption(OptionKind.Home);
        }
        private void button_Setting_Click(object sender, EventArgs e)
        {
            SetCurOption(OptionKind.Setting);
        }
        private void button_Help_Click(object sender, EventArgs e)
        {
            SetCurOption(OptionKind.Help);
        }

        private void button_NewsLink_1_Click(object sender, EventArgs e)
        {
            if(newsManager.NewsDatas.Count >= 1)
            {
                System.Diagnostics.Process.Start(newsManager.NewsDatas[0].link);
            }
        }
        private void button_NewsLink_2_Click(object sender, EventArgs e)
        {
            if (newsManager.NewsDatas.Count >= 2)
            {
                System.Diagnostics.Process.Start(newsManager.NewsDatas[1].link);
            }
        }
        private void button_NewsLink_3_Click(object sender, EventArgs e)
        {
            if (newsManager.NewsDatas.Count >= 3)
            {
                System.Diagnostics.Process.Start(newsManager.NewsDatas[2].link);
            }
        }

        private void button_Overview_MSDN_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://docs.microsoft.com/en-us/dotnet/csharp/");
        }
        private void button_Overview_StackOverflow_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://stackoverflow.com/");
        }
        private void button_Overview_Github_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/");
        }
        private void button_Overview_Google_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.google.co.kr/?client=safari&channel=ipad_bm");
        }

        private void button_Overview_LittleScheduleDetailBtn_Click(object sender, EventArgs e)
        {
            SetCurDashboard(DashboardKind.Schedule);
        }
        private void button_Overview_AnalyzeGraph_DetailBtn_Click(object sender, EventArgs e)
        {
            SetCurDashboard(DashboardKind.Analyze);
        }

        private void button_SignIn_ProceedBtn_Click(object sender, EventArgs e)
        {
            if(userManager.ProceedSignIn(textBox_signInIDText.Text, textBox_SignIn_PasswordText.Text, textBox_SignFail_ErrorText))
            {
                panel_SignInBackPanel.Visible = false;
                panel_SignInBackPanel.Enabled = false;

                login = true;

                PrepareAll();
            }
            else
            {
                panel_SignFailBack.Visible = true;
                panel_SignFailBack.Enabled = true;
            }
        }

        private void button_Lecture_LecturesBtn_1_Click(object sender, EventArgs e)
        {
            SetCurLecture(0);
        }
        private void button_Lecture_LecturesBtn_2_Click(object sender, EventArgs e)
        {
            SetCurLecture(1);
        }
        private void button_Lecture_LecturesBtn_3_Click(object sender, EventArgs e)
        {
            SetCurLecture(2);
        }
        private void button_Lecture_LecturesBtn_4_Click(object sender, EventArgs e)
        {
            SetCurLecture(3);
        }
        private void button_Lecture_LecturesBtn_5_Click(object sender, EventArgs e)
        {
            SetCurLecture(4);
        }
        private void button_Lecture_LecturesBtn_6_Click(object sender, EventArgs e)
        {
            SetCurLecture(5);
        }
        private void button_Lecture_LecturesBtn_7_Click(object sender, EventArgs e)
        {
            SetCurLecture(6);
        }
        private void button_Lecture_LecturesBtn_8_Click(object sender, EventArgs e)
        {
            SetCurLecture(7);
        }
        private void button_Lecture_LecturesBtn_9_Click(object sender, EventArgs e)
        {
            SetCurLecture(8);
        }
        private void button_Lecture_LecturesBtn_10_Click(object sender, EventArgs e)
        {
            SetCurLecture(9);
        }
        private void button_Lecture_LecturesBtn_11_Click(object sender, EventArgs e)
        {
            SetCurLecture(10);
        }
        private void button_Lecture_LecturesBtn_12_Click(object sender, EventArgs e)
        {
            SetCurLecture(11);
        }
        private void button_Lecture_LecturesBtn_13_Click(object sender, EventArgs e)
        {
            SetCurLecture(12);
        }
        private void button_Lecture_LecturesBtn_14_Click(object sender, EventArgs e)
        {
            SetCurLecture(13);
        }
        private void button_Lecture_LecturesBtn_15_Click(object sender, EventArgs e)
        {
            SetCurLecture(14);
        }

        private void button_Lecture_ProblemBtn_1_Click(object sender, EventArgs e)
        {
            SetCurProblem(0);
        }
        private void button_Lecture_ProblemBtn_2_Click(object sender, EventArgs e)
        {
            SetCurProblem(1);
        }
        private void button_Lecture_ProblemBtn_3_Click(object sender, EventArgs e)
        {
            SetCurProblem(2);
        }
        private void button_Lecture_ProblemBtn_4_Click(object sender, EventArgs e)
        {
            SetCurProblem(3);
        }
        private void button_Lecture_ProblemBtn_5_Click(object sender, EventArgs e)
        {
            SetCurProblem(4);
        }

        private void button_Lecture_TempSaveBtn_Click(object sender, EventArgs e)
        {
            lectureManager.LectureDatas[curEditLecture].codeTexts[curEditProblem] = textBox_Lecture_CodeText.Text;
        }
        private void button_Lecture_UploadBtn_Click(object sender, EventArgs e)
        {
            lectureManager.UploadProblemCode(curEditLecture);
        }

        private void button_SignIn_SignUpBtn_Click(object sender, EventArgs e)
        {
            panel_SignInBackPanel.Visible = false;
            panel_SignInBackPanel.Enabled = false;

            panel_SignUpBack.Visible = true;
            panel_SignUpBack.Enabled = true;
        }

        private void button_SignUp_ProceedBtn_Click(object sender, EventArgs e)
        {
            if(userManager.ProceedSignUp(textBox_SignUp_IDText.Text, textBox_SignUp_PasswordText.Text, textBox_SignFail_ErrorText))
            {
                panel_SignInBackPanel.Visible = true;
                panel_SignInBackPanel.Enabled = true;

                panel_SignUpBack.Visible = false;
                panel_SignUpBack.Enabled = false;
            }
            else
            {
                panel_SignFailBack.Visible = true;
                panel_SignFailBack.Enabled = true;
            }
        }

        private void button_Sign_CloseBtn_Click(object sender, EventArgs e)
        {
            panel_SignFailBack.Visible = false;
            panel_SignFailBack.Enabled = false;
        }

        private void button_NewsLink_4_Click(object sender, EventArgs e)
        {
            if (newsManager.NewsDatas.Count >= 4)
            {
                System.Diagnostics.Process.Start(newsManager.NewsDatas[3].link);
            }
        }
        #endregion
    }
}