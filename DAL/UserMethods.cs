﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace DALOrg
{
    public static class UserMethods
    {
        #region Data Pulling
        #region User
        public static Component.User GetUser(string username, string password)
        {
            DataTable Data = OledbHelper.GetTable("Select * From Users Where UName='" + username + "' AND PWord='" + password + "'");
            DataRow DataR = Data.Rows[0];
            return new Component.User(int.Parse(DataR["UserID"].ToString()), DataR["FName"].ToString(), DataR["LName"].ToString(), int.Parse(DataR["UserType"].ToString()));
        }
        #region Students
        public static DataRowCollection GetStudentsOfGroup(int GroupID)
        {
            string expr = string.Format("PARAMETERS ID Short = {0}; SELECT * FROM GetStudentsByGroupID", GroupID);
            DataTable Data = OledbHelper.GetTable(expr);
            return Data.Rows;
        }
        #endregion
        #region Teachers
        public static DataRowCollection GetTeachers()
        {
            DataTable Data = OledbHelper.GetTable("SELECT Teacher.UserID, Teacher.FName, Teacher.LName FROM Users AS Teacher WHERE (((Teacher.UserType)=0))");
            return Data.Rows;
        }
        #endregion
        #endregion
        #region Course
        public static DataRowCollection GetCourseToTeacher(int teacherID)
        {
            string expr = String.Format("SELECT * FROM GetTeacherToCourse Where TeacherID = {0}", teacherID);
            DataTable data = OledbHelper.GetTable(expr);
            return data.Rows;
        }
        public static DataRowCollection GetCourseToTeacher(int teacherID, int grade)
        {
            string expr = String.Format("SELECT * FROM GetTeacherToCourse Where TeacherID = {0} AND Grade = {1}", teacherID, grade);
            DataTable data = OledbHelper.GetTable(expr);
            return data.Rows;
        }
        public static DataRowCollection GetCourseToGroup()
        {
            string expr = "SELECT * FROM GetCourseToGroup";
            DataTable data = OledbHelper.GetTable(expr);
            return data.Rows;
        }
        public static DataRowCollection GetCourseToGroups(int courseID)
        {
            string expr = String.Format("SELECT * FROM GetCourseToGroup Where CourseID = {0}", courseID);
            DataTable data = OledbHelper.GetTable(expr);
            return data.Rows;
        }
        public static DataRowCollection GetCourseToAvailableGroups(int courseID, int day, int period)
        {
            string expr = String.Format("SELECT GetCourseToGroup.[GroupID],GetCourseToGroup.[GroupName] FROM GetCourseToGroup WHERE (((GetCourseToGroup.[GroupID]) Not In (SELECT MainGroupID FROM GetLessonsOfGroupMembers WHERE (Day = {1} AND Period = {2}))) AND (GetCourseToGroup.[CourseID])={0});", courseID,day,period);
            DataTable data = OledbHelper.GetTable(expr);
            return data.Rows;
        }
        #endregion
        #region Lesson
        public static DataRow GetLesson(int lessonID)
        {
            throw new NotImplementedException();
        }
        public static DataRow GetCurrentLesson(int TeacherID)
        {
            return OledbHelper.GetTable("Select * From Lessons L Where L.[StartTime] <  AND L.[EndTime] < GETTIME() AND L.[TeacherID] == " + TeacherID.ToString()).Rows[0];
        }
        /// <summary>
        /// Returns A Collection Of Lesson To Teacher Relations.
        /// Fields: TeacherID, Teacher.FName, Teacher.LName, Lesson.LessonID, Lesson.Day, Lesson.Period, Groups.GroupName, Groups.GroupID, Subjects.SubjectName 
        /// ORDERED By Lesson.Day, Lesson.Period - ASC,ASC
        /// </summary>
        /// <param name="TeacherID"></param>
        /// <returns></returns>
        public static DataRowCollection GetLessonsByTeacher(int TeacherID)
        {
            string expr = string.Format("SELECT * FROM GetLessonsByTeacher WHERE [TeacherID] = {0}", TeacherID);
            DataTable data = OledbHelper.GetTable(expr);
            return data.Rows;
        }
        /// <summary>
        /// Returns A Collection Of Lesson To Group Relations.
        /// Fields: TeacherID, Teacher.FName, Teacher.LName, Lesson.LessonID, Lesson.Day, Lesson.Period, Groups.GroupName, Groups.GroupName, Subjects.SubjectName 
        /// ORDERED By Lesson.Day, Lesson.Period - ASC,ASC
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public static DataRowCollection GetLessonsByGroup(int GroupID)
        {
            string expr = string.Format("SELECT * FROM GetLessonsOfGroupMembers WHERE [MainGroupID] = {0}", GroupID);
            DataTable data = OledbHelper.GetTable(expr);
            return data.Rows;
        }
        #endregion
        #endregion
        #region Data Validation
        public static bool UserExists(string Username, string Password)
        {
            try
            {
                GetUser(Username, Password);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region Data Modification
        #region Add
        public static bool AddUser(string Username, string Password)
        {
            try
            {
                if (!UserExists(Username, Password))
                {
                    string cmd = "INSERT INTO Users (UName,PWord) Values ('" + Username + "','" + Password + "');";
                    OledbHelper.Execute(cmd);
                    return true;
                }
                else
                {
                    throw new Exception("User Exists");
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region Update

        #endregion
        #region Remove
        /// <summary>
        /// Removes The Specified Lesson From The Database
        /// </summary>
        /// <param name="LessonID"></param>
        public static void RemoveLesson(int LessonID)
        {
            string Expr = String.Format("REMOVE * FROM Lessons Where [LessonID]={0}", LessonID);
            OledbHelper.Execute(Expr);
        }
        #endregion
        #endregion
    }
}
