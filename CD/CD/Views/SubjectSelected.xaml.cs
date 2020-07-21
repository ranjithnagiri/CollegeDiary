﻿using System;
using CD.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CD.Helper;
using CD.ViewModel;
using CD.Views;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using Syncfusion.XForms.ProgressBar;
using System.Threading.Tasks;

namespace CD.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class SubjectSelected : ContentPage
    {
        private Subject _subject;
        private SubjectMark subjectMark;
        readonly FireBaseHelperSubject fireBaseHelperSubject = new FireBaseHelperSubject();
        readonly FireBaseHelperMark fireBaseHelperMark = new FireBaseHelperMark();

        //public static SubjectSelected Instance;

        public SubjectSelected(Subject subject)
        {
            _subject = subject;
            InitializeComponent();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            status_bars(_subject);
            details(_subject);
            load_list(_subject);
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            status_bars(_subject);
            details(_subject);
            load_list(_subject);
        }
        public async void load_list(Subject _subject)
        {
            List<Mark> listMarks = await fireBaseHelperMark.GetMarksForSubject(_subject.SubjectID);
            subjectMark = new SubjectMark(_subject, listMarks);
            this.BindingContext = subjectMark; //!!!
        }
        public async void details(Subject _subject)
        {
            var subject = await fireBaseHelperSubject.GetSubject(_subject.SubjectID);
            Title = subject.SubjectName;
            subjectName.Text = subject.SubjectName;
            lecturerName.Text = subject.LecturerName;
            lecturerEmail.Text = subject.LecturerEmail;
            double remCA = await fireBaseHelperSubject.remainigCA(_subject.SubjectID);
            remainingCA.Text = remCA.ToString("F2") + "%";
            double remFE = await fireBaseHelperSubject.remainigFE(_subject.SubjectID);
            remainingFE.Text = remFE.ToString("F2") + "%";
            SubjectNameGpa.Text = subject.SubjectName;


            // refrashing the selected subject
            _subject = subject;
        }
        public async void status_bars(Subject _subject)
        {
            double CAProgress = await fireBaseHelperSubject.getTotalCA(_subject.SubjectID);
            double FinalExamProgress = await fireBaseHelperSubject.Final_Exam_Progress(_subject.SubjectID);
            double GPA = CAProgress + FinalExamProgress;

            statusCA.Progress = CAProgress;
            colorTheStatusBars(CAProgress, statusCA, "CA");
            statusFinalExam.Progress = FinalExamProgress;
            colorTheStatusBars(FinalExamProgress, statusFinalExam, "FE");
            statusSubjectGPA.Progress = GPA;
            colorTheStatusBars(GPA, statusSubjectGPA, "GPA");

            Ca_StatusBar.Text = CAProgress.ToString("F2");
            Fe_StatusBar.Text = FinalExamProgress.ToString("F2");
            gpa_StatusBar.Text = GPA.ToString("F2");
        }

        public void colorTheStatusBars(double process, SfLinearProgressBar bar, string type)
        {
            double segments = 0;
            double grade = 0;
            if (type == "CA") 
            { 
                segments = _subject.CA / 3;
                grade = _subject.CA;
            }
            else if (type == "FE") 
            { 
                segments = _subject.FinalExam / 3;
                grade = _subject.FinalExam;
            }
            else if (type == "GPA") 
            { 
                segments = 100 / 3;
                grade = 100;
            }
            
            RangeColorCollection rangeColors = new RangeColorCollection();
            if (type == "CA" || type == "FE")
            {
                if (process <= grade / 3)
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#ffcccb"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Red, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
                else if ((process > grade / 3) && (process < grade / 3 * 2))
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#FDE8D3"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Orange, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
                else if (process >= grade / 3 * 2)
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#d2f8d2"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Green, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
            }
            else if (type == "GPA")
            {
                if (process < 40)
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#ffcccb"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Red, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
                else if ((process >= 40) && (process < 70))
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#FDE8D3"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Orange, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
                else if (process > 70)
                {
                    rangeColors.Add(new RangeColor() { Color = Color.FromHex("#d2f8d2"), IsGradient = true, Start = 0, End = segments * 2 });
                    rangeColors.Add(new RangeColor() { Color = Color.Green, IsGradient = true, Start = segments * 2, End = segments * 3 });
                    bar.RangeColors = rangeColors;
                }
            }
        }

        [Obsolete]
        private async void add_new_mark(object sender, EventArgs e)
        {
            add_ca_button.IsEnabled = false;
            await PopupNavigation.PushAsync(new AddMarkToSubject(_subject));
            add_ca_button.IsEnabled = true;
        }

        [Obsolete]
        private async void add_final_exam(object sender, EventArgs e)
        {
            add_fe_button.IsEnabled = false;
            await PopupNavigation.PushAsync(new AddFinalExamToSubject(_subject));
            add_fe_button.IsEnabled = true;
        }

        [Obsolete]
        private async void edit_subject(object sender, EventArgs e)
        {
            await PopupNavigation.PushAsync(new EditDeleteSubject(_subject));
        }

        private async void delete_subject(object sender, EventArgs e)
        {
            var result = await DisplayAlert("Are you sure you want to delete this subject?", "Subject name: " +  _subject.SubjectName, "Yes", "No");
            if (result) // YES
            {
                try
                {
                    await fireBaseHelperSubject.DeleteSubject(_subject.SubjectID);
                    await fireBaseHelperMark.DeleteMarks(_subject.SubjectID);
                    await DisplayAlert("Success", "Subject Deleted", "OK"); //TODO: add a toast message
                    await Navigation.PopAsync();
                }
                catch (Exception)
                {
                    await DisplayAlert("Error", "Please try again", "Ok");
                }
            }
        }

        private async void delete_mark(object sender, Syncfusion.ListView.XForms.ItemHoldingEventArgs e)
        {
            var thisMark = e.ItemData as Mark;
            var result = await DisplayAlert("Are you sure you want to delete this result?", "Name: "  + thisMark.MarkName + "\nResult: " + thisMark.Result + "%", "Yes", "No");
            if (result)
            {
                try
                {
                    fireBaseHelperMark.DeleteMark(thisMark.MarkID);
                    details(_subject);
                    status_bars(_subject);
                    load_list(_subject);
                }
                catch (Exception)
                {
                    await DisplayAlert("Error", "Please try again", "Ok");
                }
            }
        }

        private void tips(object sender, EventArgs e)
        {
            if (hidenSubjectDetailsHelp.IsVisible)
            {
                hidenSubjectDetailsHelp.IsVisible = false;
            }
            else
            {
                hidenSubjectDetailsHelp.IsVisible = true;
                if (hidenYourResultsHelp.IsVisible)
                {
                    hidenYourResultsHelp.IsVisible = false;
                }
            }
        }

        private void tipsYourResults(object sender, EventArgs e)
        {
            if (hidenYourResultsHelp.IsVisible)
            {
                hidenYourResultsHelp.IsVisible = false;
            }
            else
            {
                hidenYourResultsHelp.IsVisible = true;
                if (hidenSubjectDetailsHelp.IsVisible)
                {
                    hidenSubjectDetailsHelp.IsVisible = false;
                }
            }
        }

        private void moreDetailsExpans(object sender, EventArgs e)
        {
            if (moreSubjectDetails.IsVisible == false)
            {
                moreSubjectDetails.IsVisible = true;
                moreDetails.Text = "↑";
            }
            else if (moreSubjectDetails.IsVisible == true)
            {
                moreSubjectDetails.IsVisible = false;
                moreDetails.Text = "↓";
            }
        }
    }
}