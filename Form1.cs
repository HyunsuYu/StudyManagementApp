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
            Analyze = 3
        };
        public enum OptionKind : byte
        {
            Home = 1,
            Setting = 2,
            Help = 3
        };



        private DashboardKind dashboardKind;

        private NewsManager newsManager;
        private LectureManager lectureManager;
        private UserManager userManager;

        private List<Panel> selfScorePanels_Content, selfScorePanels_Coding, simpleAnalyzePanels, analyze_runtimePanels, analyze_allocationPanels, analyze_GC1Panels, analyze_GC2Panels, analyze_GC3Panels;
        private List<Button> lectureBtns, problemBtns;

        private bool login;

        private int curEditLecture, curEditProblem, curAnalyzeLecture, curAnalyzeProblem;



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
            userManager = new UserManager();

            login = false;
        }
        private void SetDefaultLayout()
        {
            //  Set Visable&Enbale Property
            textBox_Lecture_CodeText.Enabled = false;

            button_NewsLink_1.Enabled = false;
            button_NewsLink_2.Enabled = false;
            button_NewsLink_3.Enabled = false;
            button_NewsLink_4.Enabled = false;

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

            panel_Analyze_RuntimeAndAllocationGraphBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Analyze_RuntimeAndAllocationGraphBack.Width, panel_Analyze_RuntimeAndAllocationGraphBack.Height, 20, 20));
            panel_Analyze_Edgy_1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Analyze_Edgy_1.Width, panel_Analyze_Edgy_1.Height, 20, 20));
            panel_Analyze_Edgy_2.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Analyze_Edgy_2.Width, panel_Analyze_Edgy_2.Height, 20, 20));
            panel_Analyze_GCGraphBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Analyze_GCGraphBack.Width, panel_Analyze_GCGraphBack.Height, 20, 20));
            panel_Analyze_SelectLectureBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Analyze_SelectLectureBack.Width, panel_Analyze_SelectLectureBack.Height, 20, 20));
            panel_Analyze_SelectProblemBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Analyze_SelectProblemBack.Width, panel_Analyze_SelectProblemBack.Height, 20, 20));

            flowLayoutPanel_Analyze_SelectLecture_FlowBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, flowLayoutPanel_Analyze_SelectLecture_FlowBack.Width, flowLayoutPanel_Analyze_SelectLecture_FlowBack.Height, 20, 20));
            panel_Analyze_SelectProblem_MainBack.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel_Analyze_SelectProblem_MainBack.Width, panel_Analyze_SelectProblem_MainBack.Height, 20, 20));

            //  Buttons

            //  Images
            pictureBox1.Image = Properties.Resources.microsoft_1_png;
            pictureBox2.Image = Properties.Resources.stack_overflow_1_png;
            pictureBox4.Image = Properties.Resources.github_1_png;
            pictureBox3.Image = Properties.Resources.google_1_png;

            pictureBox5.Image = Properties.Resources.user;

            button_NewsLink_1.Image = Properties.Resources.external_link_symbol_1_png;
            button_NewsLink_2.Image = Properties.Resources.external_link_symbol_1_png;
            button_NewsLink_3.Image = Properties.Resources.external_link_symbol_1_png;
            button_NewsLink_4.Image = Properties.Resources.external_link_symbol_1_png;

            button_Overview_MSDN.Image = Properties.Resources.external_link_symbol_1_png;
            button_Overview_StackOverflow.Image = Properties.Resources.external_link_symbol_1_png;
            button_Overview_Github.Image = Properties.Resources.external_link_symbol_1_png;
            button_Overview_Google.Image = Properties.Resources.external_link_symbol_1_png;

            button_Sign_CloseBtn.Image = Properties.Resources.close_cross_1_png;

            //  Analyze
            button_Analyze_SelectLectureBtn_1.Visible = false;
            button_Analyze_SelectLectureBtn_1.Enabled = false;

            button_Analyze_SelectLectureBtn_2.Visible = false;
            button_Analyze_SelectLectureBtn_2.Enabled = false;

            button_Analyze_SelectLectureBtn_3.Visible = false;
            button_Analyze_SelectLectureBtn_3.Enabled = false;

            button_Analyze_SelectLectureBtn_4.Visible = false;
            button_Analyze_SelectLectureBtn_4.Enabled = false;

            button_Analyze_SelectLectureBtn_5.Visible = false;
            button_Analyze_SelectLectureBtn_5.Enabled = false;

            button_Analyze_SelectLectureBtn_6.Visible = false;
            button_Analyze_SelectLectureBtn_6.Enabled = false;

            button_Analyze_SelectLectureBtn_7.Visible = false;
            button_Analyze_SelectLectureBtn_7.Enabled = false;

            button_Analyze_SelectLectureBtn_8.Visible = false;
            button_Analyze_SelectLectureBtn_8.Enabled = false;

            button_Analyze_SelectLectureBtn_9.Visible = false;
            button_Analyze_SelectLectureBtn_9.Enabled = false;

            button_Analyze_SelectLectureBtn_10.Visible = false;
            button_Analyze_SelectLectureBtn_10.Enabled = false;

            button_Analyze_SelectLectureBtn_11.Visible = false;
            button_Analyze_SelectLectureBtn_11.Enabled = false;

            button_Analyze_SelectLectureBtn_12.Visible = false;
            button_Analyze_SelectLectureBtn_12.Enabled = false;

            button_Analyze_SelectLectureBtn_13.Visible = false;
            button_Analyze_SelectLectureBtn_13.Enabled = false;

            button_Analyze_SelectLectureBtn_14.Visible = false;
            button_Analyze_SelectLectureBtn_14.Enabled = false;

            button_Analyze_SelectLectureBtn_15.Visible = false;
            button_Analyze_SelectLectureBtn_15.Enabled = false;

            button_Analyze_SelectProblemBtn_1.Visible = false;
            button_Analyze_SelectProblemBtn_1.Enabled = false;

            button_Analyze_SelectProblemBtn_2.Visible = false;
            button_Analyze_SelectProblemBtn_2.Enabled = false;

            button_Analyze_SelectProblemBtn_3.Visible = false;
            button_Analyze_SelectProblemBtn_3.Enabled = false;

            button_Analyze_SelectProblemBtn_4.Visible = false;
            button_Analyze_SelectProblemBtn_4.Enabled = false;

            button_Analyze_SelectProblemBtn_5.Visible = false;
            button_Analyze_SelectProblemBtn_5.Enabled = false;
        }
        private void PrepareAll()
        {
            //  Alloc
            newsManager = new NewsManager();
            lectureManager = new LectureManager(userManager.IdentifyNumber);

            selfScorePanels_Content = new List<Panel>();
            selfScorePanels_Coding = new List<Panel>();
            simpleAnalyzePanels = new List<Panel>();
            analyze_runtimePanels = new List<Panel>();
            analyze_allocationPanels = new List<Panel>();
            analyze_GC1Panels = new List<Panel>();
            analyze_GC2Panels = new List<Panel>();
            analyze_GC3Panels = new List<Panel>();
            lectureBtns = new List<Button>();
            problemBtns = new List<Button>();

            for(int index = 0; index < 6; index++)
            {
                Panel panel_runtime = new Panel();
                Panel panel_allocation = new Panel();
                Panel panel_GC1 = new Panel();
                Panel panel_GC2 = new Panel();
                Panel panel_GC3 = new Panel();

                Controls.Add(panel_runtime);
                Controls.Add(panel_allocation);
                Controls.Add(panel_GC1);
                Controls.Add(panel_GC2);
                Controls.Add(panel_GC3);

                panel_runtime.BackColor = Color.FromArgb(254, 188, 44);
                panel_allocation.BackColor = Color.FromArgb(248, 76, 72);
                panel_GC1.BackColor = Color.FromArgb(254, 188, 44);
                panel_GC2.BackColor = Color.FromArgb(248, 76, 72);
                panel_GC3.BackColor = Color.Lime;

                panel_runtime.Visible = false;
                panel_allocation.Visible = false;
                panel_GC1.Visible = false;
                panel_GC2.Visible = false;
                panel_GC3.Visible = false;

                panel_runtime.Enabled = false;
                panel_allocation.Enabled = false;
                panel_GC1.Enabled = false;
                panel_GC2.Enabled = false;
                panel_GC3.Enabled = false;

                panel_runtime.BringToFront();
                panel_allocation.BringToFront();
                panel_GC1.BringToFront();
                panel_GC2.BringToFront();
                panel_GC3.BringToFront();

                analyze_runtimePanels.Add(panel_runtime);
                analyze_allocationPanels.Add(panel_allocation);
                analyze_GC1Panels.Add(panel_GC1);
                analyze_GC2Panels.Add(panel_GC2);
                analyze_GC3Panels.Add(panel_GC3);
            }

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

            //  Analyze
            if(lectureManager.LectureDatas.Count >= 1)
            {
                button_Analyze_SelectLectureBtn_1.Visible = true;
                button_Analyze_SelectLectureBtn_1.Enabled = true;
                button_Analyze_SelectLectureBtn_1.Text = "Ep.1 " + lectureManager.LectureDatas[0].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 2)
            {
                button_Analyze_SelectLectureBtn_2.Visible = true;
                button_Analyze_SelectLectureBtn_2.Enabled = true;
                button_Analyze_SelectLectureBtn_2.Text = "Ep.2 " + lectureManager.LectureDatas[1].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 3)
            {
                button_Analyze_SelectLectureBtn_3.Visible = true;
                button_Analyze_SelectLectureBtn_3.Enabled = true;
                button_Analyze_SelectLectureBtn_3.Text = "Ep.3 " + lectureManager.LectureDatas[2].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 4)
            {
                button_Analyze_SelectLectureBtn_4.Visible = true;
                button_Analyze_SelectLectureBtn_4.Enabled = true;
                button_Analyze_SelectLectureBtn_4.Text = "Ep.4 " + lectureManager.LectureDatas[3].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 5)
            {
                button_Analyze_SelectLectureBtn_5.Visible = true;
                button_Analyze_SelectLectureBtn_5.Enabled = true;
                button_Analyze_SelectLectureBtn_5.Text = "Ep.5 " + lectureManager.LectureDatas[4].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 6)
            {
                button_Analyze_SelectLectureBtn_6.Visible = true;
                button_Analyze_SelectLectureBtn_6.Enabled = true;
                button_Analyze_SelectLectureBtn_6.Text = "Ep.6 " + lectureManager.LectureDatas[5].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 7)
            {
                button_Analyze_SelectLectureBtn_7.Visible = true;
                button_Analyze_SelectLectureBtn_7.Enabled = true;
                button_Analyze_SelectLectureBtn_7.Text = "Ep.7 " + lectureManager.LectureDatas[6].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 8)
            {
                button_Analyze_SelectLectureBtn_8.Visible = true;
                button_Analyze_SelectLectureBtn_8.Enabled = true;
                button_Analyze_SelectLectureBtn_8.Text = "Ep.8 " + lectureManager.LectureDatas[7].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 9)
            {
                button_Analyze_SelectLectureBtn_9.Visible = true;
                button_Analyze_SelectLectureBtn_9.Enabled = true;
                button_Analyze_SelectLectureBtn_9.Text = "Ep.9 " + lectureManager.LectureDatas[8].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 10)
            {
                button_Analyze_SelectLectureBtn_10.Visible = true;
                button_Analyze_SelectLectureBtn_10.Enabled = true;
                button_Analyze_SelectLectureBtn_10.Text = "Ep.10 " + lectureManager.LectureDatas[9].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 11)
            {
                button_Analyze_SelectLectureBtn_11.Visible = true;
                button_Analyze_SelectLectureBtn_11.Enabled = true;
                button_Analyze_SelectLectureBtn_11.Text = "Ep.11 " + lectureManager.LectureDatas[10].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 12)
            {
                button_Analyze_SelectLectureBtn_12.Visible = true;
                button_Analyze_SelectLectureBtn_12.Enabled = true;
                button_Analyze_SelectLectureBtn_12.Text = "Ep.12 " + lectureManager.LectureDatas[11].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 13)
            {
                button_Analyze_SelectLectureBtn_13.Visible = true;
                button_Analyze_SelectLectureBtn_13.Enabled = true;
                button_Analyze_SelectLectureBtn_13.Text = "Ep.13 " + lectureManager.LectureDatas[12].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 14)
            {
                button_Analyze_SelectLectureBtn_14.Visible = true;
                button_Analyze_SelectLectureBtn_14.Enabled = true;
                button_Analyze_SelectLectureBtn_14.Text = "Ep.14 " + lectureManager.LectureDatas[13].lectureName;
            }
            if (lectureManager.LectureDatas.Count >= 15)
            {
                button_Analyze_SelectLectureBtn_15.Visible = true;
                button_Analyze_SelectLectureBtn_15.Enabled = true;
                button_Analyze_SelectLectureBtn_15.Text = "Ep.15 " + lectureManager.LectureDatas[14].lectureName;
            }

            SetCurAnalyzeLecture(0);
            SetCurDashboard(dashboardKind);
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

                    if(login)
                    {
                        for(int index = 0; index < lectureManager.LectureDatas[curAnalyzeLecture].analyzeDatas[curAnalyzeProblem].Count; index++)
                        {
                            analyze_runtimePanels[index].Visible = true;
                            analyze_runtimePanels[index].Enabled = true;
                        }
                        for (int index = 0; index < lectureManager.LectureDatas[curAnalyzeLecture].analyzeDatas[curAnalyzeProblem].Count; index++)
                        {
                            analyze_allocationPanels[index].Visible = true;
                            analyze_allocationPanels[index].Enabled = true;
                        }
                        for (int index = 0; index < lectureManager.LectureDatas[curAnalyzeLecture].analyzeDatas[curAnalyzeProblem].Count; index++)
                        {
                            analyze_GC1Panels[index].Visible = true;
                            analyze_GC1Panels[index].Enabled = true;
                        }
                        for (int index = 0; index < lectureManager.LectureDatas[curAnalyzeLecture].analyzeDatas[curAnalyzeProblem].Count; index++)
                        {
                            analyze_GC2Panels[index].Visible = true;
                            analyze_GC2Panels[index].Enabled = true;
                        }
                        for (int index = 0; index < lectureManager.LectureDatas[curAnalyzeLecture].analyzeDatas[curAnalyzeProblem].Count; index++)
                        {
                            analyze_GC3Panels[index].Visible = true;
                            analyze_GC3Panels[index].Enabled = true;
                        }
                    }
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

            if (login)
            {
                for (int index = 0; index < lectureManager.LectureDatas[curAnalyzeLecture].analyzeDatas[curAnalyzeProblem].Count; index++)
                {
                    analyze_runtimePanels[index].Visible = false;
                    analyze_runtimePanels[index].Enabled = false;
                }
                for (int index = 0; index < lectureManager.LectureDatas[curAnalyzeLecture].analyzeDatas[curAnalyzeProblem].Count; index++)
                {
                    analyze_allocationPanels[index].Visible = false;
                    analyze_allocationPanels[index].Enabled = false;
                }
                for (int index = 0; index < lectureManager.LectureDatas[curAnalyzeLecture].analyzeDatas[curAnalyzeProblem].Count; index++)
                {
                    analyze_GC1Panels[index].Visible = false;
                    analyze_GC1Panels[index].Enabled = false;
                }
                for (int index = 0; index < lectureManager.LectureDatas[curAnalyzeLecture].analyzeDatas[curAnalyzeProblem].Count; index++)
                {
                    analyze_GC2Panels[index].Visible = false;
                    analyze_GC2Panels[index].Enabled = false;
                }
                for (int index = 0; index < lectureManager.LectureDatas[curAnalyzeLecture].analyzeDatas[curAnalyzeProblem].Count; index++)
                {
                    analyze_GC3Panels[index].Visible = false;
                    analyze_GC3Panels[index].Enabled = false;
                }
            }
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
        private void SetCurAnalyzeLecture(int index)
        {
            curAnalyzeLecture = index;

            DisableAllAnalyzeLecture();

            switch (index)
            {
                case 0:
                    button_Analyze_SelectLectureBtn_1.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 1:
                    button_Analyze_SelectLectureBtn_2.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 2:
                    button_Analyze_SelectLectureBtn_3.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 3:
                    button_Analyze_SelectLectureBtn_4.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 4:
                    button_Analyze_SelectLectureBtn_5.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 5:
                    button_Analyze_SelectLectureBtn_6.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 6:
                    button_Analyze_SelectLectureBtn_7.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 7:
                    button_Analyze_SelectLectureBtn_8.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 8:
                    button_Analyze_SelectLectureBtn_9.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 9:
                    button_Analyze_SelectLectureBtn_10.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 10:
                    button_Analyze_SelectLectureBtn_11.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 11:
                    button_Analyze_SelectLectureBtn_12.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 12:
                    button_Analyze_SelectLectureBtn_13.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 13:
                    button_Analyze_SelectLectureBtn_14.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 14:
                    button_Analyze_SelectLectureBtn_15.ForeColor = Color.FromArgb(253, 65, 60);
                    break;
            }

            if(lectureManager.LectureDatas[index].analyzeDatas.Count >= 1)
            {
                button_Analyze_SelectProblemBtn_1.Visible = true;
                button_Analyze_SelectProblemBtn_1.Enabled = true;

                if (lectureManager.LectureDatas[index].analyzeDatas.Count >= 2)
                {
                    button_Analyze_SelectProblemBtn_2.Visible = true;
                    button_Analyze_SelectProblemBtn_2.Enabled = true;

                    if (lectureManager.LectureDatas[index].analyzeDatas.Count >= 3)
                    {
                        button_Analyze_SelectProblemBtn_3.Visible = true;
                        button_Analyze_SelectProblemBtn_3.Enabled = true;

                        if (lectureManager.LectureDatas[index].analyzeDatas.Count >= 4)
                        {
                            button_Analyze_SelectProblemBtn_4.Visible = true;
                            button_Analyze_SelectProblemBtn_4.Enabled = true;

                            if (lectureManager.LectureDatas[index].analyzeDatas.Count >= 5)
                            {
                                button_Analyze_SelectProblemBtn_5.Visible = true;
                                button_Analyze_SelectProblemBtn_5.Enabled = true;
                            }
                        }
                    }
                }
            }

            SetCurAnalyzeProblem(0);
        }
        private void DisableAllAnalyzeLecture()
        {
            button_Analyze_SelectProblemBtn_1.Visible = false;
            button_Analyze_SelectProblemBtn_1.Enabled = false;

            button_Analyze_SelectProblemBtn_2.Visible = false;
            button_Analyze_SelectProblemBtn_2.Enabled = false;

            button_Analyze_SelectProblemBtn_3.Visible = false;
            button_Analyze_SelectProblemBtn_3.Enabled = false;

            button_Analyze_SelectProblemBtn_4.Visible = false;
            button_Analyze_SelectProblemBtn_4.Enabled = false;

            button_Analyze_SelectProblemBtn_5.Visible = false;
            button_Analyze_SelectProblemBtn_5.Enabled = false;

            button_Analyze_SelectLectureBtn_1.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_2.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_3.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_4.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_5.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_6.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_7.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_8.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_9.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_10.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_11.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_12.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_13.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_14.ForeColor = Color.White;
            button_Analyze_SelectLectureBtn_15.ForeColor = Color.White;
        }
        private void SetCurAnalyzeProblem(int index)
        {
            curAnalyzeProblem = index;

            DIsableAllAnalyzeProblem();

            switch(index)
            {
                case 0:
                    button_Analyze_SelectProblemBtn_1.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 1:
                    button_Analyze_SelectProblemBtn_2.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 2:
                    button_Analyze_SelectProblemBtn_3.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 3:
                    button_Analyze_SelectProblemBtn_4.ForeColor = Color.FromArgb(253, 65, 60);
                    break;

                case 4:
                    button_Analyze_SelectProblemBtn_5.ForeColor = Color.FromArgb(253, 65, 60);
                    break;
            }

            RefreshAnalyzeGraphs(curAnalyzeLecture, curAnalyzeProblem);
        }
        private void DIsableAllAnalyzeProblem()
        {
            button_Analyze_SelectProblemBtn_1.ForeColor = Color.White;
            button_Analyze_SelectProblemBtn_2.ForeColor = Color.White;
            button_Analyze_SelectProblemBtn_3.ForeColor = Color.White;
            button_Analyze_SelectProblemBtn_4.ForeColor = Color.White;
            button_Analyze_SelectProblemBtn_5.ForeColor = Color.White;
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
            List<float> runTimeAve = new List<float>();
            for (int index = 0; index < lectureManager.LectureDatas.Count; index++)
            {
                int temp = 0;
                runTimeAve.Add(0.0f);
                for(int problemIndex = 0; problemIndex < lectureManager.LectureDatas[index].analyzeDatas.Count; problemIndex++)
                {
                    for(int count = 0; count < lectureManager.LectureDatas[index].analyzeDatas[problemIndex].Count; count++)
                    {
                        runTimeAve[index] += lectureManager.LectureDatas[index].analyzeDatas[problemIndex][count].runTime;
                        temp++;
                    }
                }

                runTimeAve[index] /= temp;
                if (runTimeAve[index] > worstRunTime)
                {
                    worstRunTime = runTimeAve[index];
                }

                //runTimeAve.Add(0.0f);
                //for (int count = 0; count < lectureManager.LectureDatas[index].analyzeDatas.Count; count++)
                //{
                //    runTimeAve[index] += lectureManager.LectureDatas[index].analyzeDatas[problemIndex][count].runTime;
                //}

                //runTimeAve[index] /= lectureManager.LectureDatas[index].analyzeDatas.Count;
                //if (runTimeAve[index] > worstRunTime)
                //{
                //    worstRunTime = runTimeAve[index];
                //}
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
                panel.Location = new Point(107 + 20 * index - randumNum + 1, 436 - randumNum + 1 + 140 - (int)(140 * runTimeAve[index] / worstRunTime));
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
        private void RefreshAnalyzeGraphs(int lectureIndex, int problemIndex)
        {
            DisableAllAnalyzeGraphs();

            Random random = new Random();

            //  Runtime And Allocation Graph
            float worstYAxisValue_RuntimeAndAllocation = float.MinValue;
            for(int index = 0; index < lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count; index++)
            {
                if(lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].runTime > worstYAxisValue_RuntimeAndAllocation)
                {
                    worstYAxisValue_RuntimeAndAllocation = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].runTime;
                }
                if(lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].totalMemoryAllocation > worstYAxisValue_RuntimeAndAllocation)
                {
                    worstYAxisValue_RuntimeAndAllocation = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].totalMemoryAllocation;
                }

                //  Runtime Panel
                int randNum = random.Next(1, 6);

                analyze_runtimePanels[index].Visible = true;
                analyze_runtimePanels[index].Enabled = true;
                analyze_runtimePanels[index].Size = new Size(randNum * 2, randNum * 2);
                analyze_runtimePanels[index].Location = new Point(100 - randNum + 60 * index, 370 - randNum - (int)(120 * lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].runTime / worstYAxisValue_RuntimeAndAllocation));
                analyze_runtimePanels[index].Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, analyze_runtimePanels[index].Width, analyze_runtimePanels[index].Height, randNum, randNum));

                //  Allocation Panel
                randNum = random.Next(1, 6);
                analyze_allocationPanels[index].Visible = true;
                analyze_allocationPanels[index].Enabled = true;
                analyze_allocationPanels[index].Size = new Size(randNum * 2, randNum * 2);
                analyze_allocationPanels[index].Location = new Point(100 - randNum + 60 * index, 370 - randNum - (int)(120 * lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].totalMemoryAllocation / worstYAxisValue_RuntimeAndAllocation));
                analyze_allocationPanels[index].Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, analyze_allocationPanels[index].Width, analyze_allocationPanels[index].Height, randNum, randNum));
            }

            float runtimeAndAllocationUnit = worstYAxisValue_RuntimeAndAllocation / 6.0f;
            textBox_Analyze_RuntimeAndAllocation_Y_2.Text = (runtimeAndAllocationUnit).ToString("000.00") + " ms/mb";
            textBox_Analyze_RuntimeAndAllocation_Y_3.Text = (runtimeAndAllocationUnit * 2.0f).ToString("000.00") + " ms/mb";
            textBox_Analyze_RuntimeAndAllocation_Y_4.Text = (runtimeAndAllocationUnit * 3.0f).ToString("000.00") + " ms/mb";
            textBox_Analyze_RuntimeAndAllocation_Y_5.Text = (runtimeAndAllocationUnit * 4.0f).ToString("000.00") + " ms/mb";
            textBox_Analyze_RuntimeAndAllocation_Y_6.Text = (runtimeAndAllocationUnit * 5.0f).ToString("000.00") + " ms/mb";
            textBox_Analyze_RuntimeAndAllocation_Y_7.Text = (runtimeAndAllocationUnit * 6.0f).ToString("000.00") + " ms/mb";

            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 1)
            {
                textBox_Analyze_RuntimeAndAllocation_X_1.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][0].param;
            }
            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 2)
            {
                textBox_Analyze_RuntimeAndAllocation_X_2.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][1].param;
            }
            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 3)
            {
                textBox_Analyze_RuntimeAndAllocation_X_3.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][2].param;
            }
            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 4)
            {
                textBox_Analyze_RuntimeAndAllocation_X_4.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][3].param;
            }
            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 5)
            {
                textBox_Analyze_RuntimeAndAllocation_X_5.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][4].param;
            }
            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 6)
            {
                textBox_Analyze_RuntimeAndAllocation_X_6.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][5].param;
            }

            //  GC Graph
            float worstYAxisValue_GC = float.MinValue;
            for (int index = 0; index < lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count; index++)
            {
                if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].GC_Gen1 > worstYAxisValue_GC)
                {
                    worstYAxisValue_GC = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].GC_Gen1;
                }
                if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].GC_Gen2 > worstYAxisValue_GC)
                {
                    worstYAxisValue_GC = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].GC_Gen2;
                }
                if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].GC_Gen3 > worstYAxisValue_GC)
                {
                    worstYAxisValue_GC = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].GC_Gen3;
                }

                //  GC1 Panel
                int randNum = random.Next(1, 6);

                analyze_GC1Panels[index].Visible = true;
                analyze_GC1Panels[index].Enabled = true;
                analyze_GC1Panels[index].Size = new Size(randNum * 2, randNum * 2);
                analyze_GC1Panels[index].Location = new Point(315 - randNum + 60 * index, 583 - randNum - (int)(120 * lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].GC_Gen1 / worstYAxisValue_GC));
                analyze_GC1Panels[index].Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, analyze_GC1Panels[index].Width, analyze_GC1Panels[index].Height, randNum, randNum));

                //  GC2 Panel
                randNum = random.Next(1, 6);
                analyze_GC2Panels[index].Visible = true;
                analyze_GC2Panels[index].Enabled = true;
                analyze_GC2Panels[index].Size = new Size(randNum * 2, randNum * 2);
                analyze_GC2Panels[index].Location = new Point(315 - randNum + 60 * index, 583 - randNum - (int)(120 * lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].GC_Gen2 / worstYAxisValue_GC));
                analyze_GC2Panels[index].Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, analyze_GC2Panels[index].Width, analyze_GC2Panels[index].Height, randNum, randNum));

                //  GC3 Panel
                randNum = random.Next(1, 6);
                analyze_GC3Panels[index].Visible = true;
                analyze_GC3Panels[index].Enabled = true;
                analyze_GC3Panels[index].Size = new Size(randNum * 2, randNum * 2);
                analyze_GC3Panels[index].Location = new Point(315 - randNum + 60 * index, 583 - randNum - (int)(120 * lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][index].GC_Gen3 / worstYAxisValue_GC));
                analyze_GC3Panels[index].Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, analyze_GC3Panels[index].Width, analyze_GC3Panels[index].Height, randNum, randNum));
            }

            float GCUnit = worstYAxisValue_GC / 6.0f;
            textBox_Analyze_GC_Y_2.Text = (GCUnit).ToString("000.00") + " mb";
            textBox_Analyze_GC_Y_3.Text = (GCUnit * 2.0f).ToString("000.00") + " mb";
            textBox_Analyze_GC_Y_4.Text = (GCUnit * 3.0f).ToString("000.00") + " mb";
            textBox_Analyze_GC_Y_5.Text = (GCUnit * 4.0f).ToString("000.00") + " mb";
            textBox_Analyze_GC_Y_6.Text = (GCUnit * 5.0f).ToString("000.00") + " mb";
            textBox_Analyze_GC_Y_7.Text = (GCUnit * 6.0f).ToString("000.00") + " mb";

            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 1)
            {
                textBox_Analyze_GC_X_1.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][0].param;
            }
            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 2)
            {
                textBox_Analyze_GC_X_2.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][1].param;
            }
            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 3)
            {
                textBox_Analyze_GC_X_3.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][2].param;
            }
            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 4)
            {
                textBox_Analyze_GC_X_4.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][3].param;
            }
            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 5)
            {
                textBox_Analyze_GC_X_5.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][4].param;
            }
            if (lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex].Count >= 6)
            {
                textBox_Analyze_GC_X_6.Text = lectureManager.LectureDatas[lectureIndex].analyzeDatas[problemIndex][5].param;
            }

            random = null;
        }
        private void DisableAllAnalyzeGraphs()
        {
            for(int index = 0; index < 6; index++)
            {
                analyze_runtimePanels[index].Visible = false;
                analyze_allocationPanels[index].Visible = false;
                analyze_GC1Panels[index].Visible = false;
                analyze_GC2Panels[index].Visible = false;
                analyze_GC3Panels[index].Visible = false;

                analyze_runtimePanels[index].Enabled = false;
                analyze_allocationPanels[index].Enabled = false;
                analyze_GC1Panels[index].Enabled = false;
                analyze_GC2Panels[index].Enabled = false;
                analyze_GC3Panels[index].Enabled = false;
            }
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
            System.Diagnostics.Process.Start("https://calendar.google.com/calendar/u/1?cid=dTBsczYwOGhvbjJhdTJicHF2aXAzdGNlaWNAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ");
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

                textBox_Lecture_CodeText.Enabled = true;

                button_NewsLink_1.Enabled = true;
                button_NewsLink_2.Enabled = true;
                button_NewsLink_3.Enabled = true;
                button_NewsLink_4.Enabled = true;

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

        private void button_Analyze_SelectLectureBtn_1_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(0);
        }
        private void button_Analyze_SelectLectureBtn_2_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(1);
        }
        private void button_Analyze_SelectLectureBtn_3_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(2);
        }
        private void button_Analyze_SelectLectureBtn_4_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(3);
        }
        private void button_Analyze_SelectLectureBtn_5_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(4);
        }
        private void button_Analyze_SelectLectureBtn_6_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(5);
        }
        private void button_Analyze_SelectLectureBtn_7_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(6);
        }
        private void button_Analyze_SelectLectureBtn_8_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(7);
        }
        private void button_Analyze_SelectLectureBtn_9_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(8);
        }
        private void button_Analyze_SelectLectureBtn_10_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(9);
        }
        private void button_Analyze_SelectLectureBtn_11_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(10);
        }
        private void button_Analyze_SelectLectureBtn_12_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(11);
        }
        private void button_Analyze_SelectLectureBtn_13_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(12);
        }
        private void button_Analyze_SelectLectureBtn_14_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(13);
        }
        private void button_Analyze_SelectLectureBtn_15_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeLecture(14);
        }

        private void button_Analyze_SelectProblemBtn_1_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeProblem(0);
        }
        private void button_Analyze_SelectProblemBtn_2_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeProblem(1);
        }
        private void button_Analyze_SelectProblemBtn_3_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeProblem(2);
        }
        private void button_Analyze_SelectProblemBtn_4_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeProblem(3);
        }
        private void button_Analyze_SelectProblemBtn_5_Click(object sender, EventArgs e)
        {
            SetCurAnalyzeProblem(4);
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