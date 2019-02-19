using Microsoft.AspNet.Identity;
using OruBloggen.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OruBloggen.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {

        // GET: Project
        public ActionResult Project()
        {
            ListTeams();
            ListTypes();
            ListStatus();

            var projectView = new ProjectViewModel();
            projectView.ProjectList = ListProjects();

            return View(projectView);
        }

        [HttpPost]
        public ActionResult CreateProject(ProjectViewModel project, HttpPostedFileBase[] files)
        {
            var ctx = new OruBloggenDbContext();
            var team = ctx.Teams.FirstOrDefault(t => t.TeamID == project.NewProject.ProjectTeamID).TeamID;
            var creatorID = User.Identity.GetUserId();

            ctx.Projects.Add(new ProjectModel
            {
                ProjectType = project.NewProject.ProjectType,
                ProjectName = project.NewProject.ProjectName,
                ProjectDesc = project.NewProject.ProjectDesc,
                ProjectStatus = project.NewProject.ProjectStatus,
                ProjectTeamID = team,
                ProjectUserID = creatorID
            });

            ctx.SaveChanges();

            if (files != null)
            {
                foreach (var file in files)
                {
                    SaveFile(file);
                }
            }

            return RedirectToAction("Project");
        }

        private void SaveFile(HttpPostedFileBase file)
        {
            var ctx = new OruBloggenDbContext();
            var project = ctx.Projects.OrderByDescending(p => p.ProjectID).FirstOrDefault();

            if (file != null)
            {
                string fileType = Path.GetExtension(file.FileName).ToLower();
                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + " (" + project.ProjectID + ")";
                var filePath = fileName.ToString() + fileType;
                string path = Path.Combine(Server.MapPath("~/ProjectFiles/" + filePath));
                file.SaveAs(path);

                ctx.ProjectFiles.Add(new ProjectFilesModel
                {
                    ProjectID = project.ProjectID,
                    FilePath = filePath
                });

                ctx.SaveChanges();
            }


        }

        public void ListTeams()
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in ctx.Teams)
            {
                list.Add(new SelectListItem() { Text = item.TeamName, Value = item.TeamID.ToString() });
            }
            ViewData["Teams"] = list;

        }

        public void ListTypes()
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Utbildning", Value = "Utbildning" });
            list.Add(new SelectListItem() { Text = "Forskning", Value = "Forskning" });

            ViewData["Types"] = list;

        }

        public void ListStatus()
        {
            var ctx = new OruBloggenDbContext();
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Text = "Ej påbörjat", Value = "1" });
            list.Add(new SelectListItem() { Text = "Pågående", Value = "2" });
            list.Add(new SelectListItem() { Text = "Avslutat", Value = "3" });


            ViewData["Status"] = list;

        }

        public List<ProjectItemViewModel> ListProjects()
        {
            var ctx = new OruBloggenDbContext();
            var list = new List<ProjectItemViewModel>();
            var projects = ctx.Projects;
            var users = ctx.Users;
            var files = ctx.ProjectFiles;

            foreach (var project in projects.OrderBy(p => p.ProjectStatus))
            {
                AddToViewList(project, list);
            }
            

            return list;
        }

        private void AddToViewList (ProjectModel project, List<ProjectItemViewModel> list)
        {
            var ctx = new OruBloggenDbContext();
            var projects = ctx.Projects;
            var fileList = ctx.ProjectFiles.Where(f => f.ProjectID == project.ProjectID).ToList();
            var creatorName = ctx.Users.FirstOrDefault(u => u.UserID == project.ProjectUserID).UserFirstname + " " + ctx.Users.FirstOrDefault(u => u.UserID == project.ProjectUserID).UserLastname;
            var team = ctx.Teams.FirstOrDefault(t => t.TeamID == project.ProjectTeamID).TeamName;
            string status = null;

            if (project.ProjectStatus == "1")
            {
                status = "Ej påbörjat";
            }
            if (project.ProjectStatus == "2")
            {
                status = "Pågående";
            }
            if (project.ProjectStatus == "3")
            {
                status = "Avslutat";
            }

            list.Add(new ProjectItemViewModel
            {
                ProjectID = project.ProjectID,
                ProjectName = project.ProjectName,
                ProjectDesc = project.ProjectDesc,
                ProjectCreatorName = creatorName,
                ProjectStatus = status,
                ProjectType = project.ProjectType,
                ProjectFiles = fileList,
                ProjectCreatorID = project.ProjectUserID,
                TeamName = team
            });
        }

        public ActionResult UpdateStatus(int projectID, string status)
        {
            var ctx = new OruBloggenDbContext();
            var project = ctx.Projects.FirstOrDefault(p => p.ProjectID == projectID);

            project.ProjectStatus = status;
            ctx.SaveChanges();

            return RedirectToAction("Project");
        }

        public ActionResult FilterOnTeams(int filterID)
        {
            var ctx = new OruBloggenDbContext();
            var list = new List<ProjectItemViewModel>();
            var projects = ctx.Projects.Where(p => p.ProjectTeamID == filterID);

            foreach(var project in projects)
            {
                AddToViewList(project, list);
            }

            var projectView = new ProjectViewModel();
            projectView.ProjectList = list;
            ListTeams();
            ListTypes();
            ListStatus();

            return View("Project", projectView);
        }

        public ActionResult FilterOnStatus(string filterID)
        {
            var ctx = new OruBloggenDbContext();
            var list = new List<ProjectItemViewModel>();
            var projects = ctx.Projects.Where(p => p.ProjectStatus == filterID);

            foreach (var project in projects)
            {
                AddToViewList(project, list);
            }

            var projectView = new ProjectViewModel();
            projectView.ProjectList = list;
            ListTeams();
            ListTypes();
            ListStatus();

            return View("Project", projectView);
        }

        public ActionResult RemoveProject(int projectID)
        {
            var ctx = new OruBloggenDbContext();
            var project = ctx.Projects.FirstOrDefault(p => p.ProjectID == projectID);
            var fileList = ctx.ProjectFiles.Where(f => f.ProjectID == projectID);

            foreach(var file in fileList)
            {
                ctx.ProjectFiles.Remove(file);
            }

            ctx.SaveChanges();
            ctx.Projects.Remove(project);
            ctx.SaveChanges();

            return RedirectToAction("Project");
        }

    }
}