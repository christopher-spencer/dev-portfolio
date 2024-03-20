using System;
using System.Collections.Generic;
using System.Data.Common;
using Capstone.DAO.Interfaces;
using Capstone.Exceptions;
using Capstone.Models;
using Npgsql;

namespace Capstone.DAO
{
    public class SideProjectPostgresDao : ISideProjectDao
    {
        private readonly string connectionString;
        private readonly IGoalDao _goalDao;
        private readonly IImageDao _imageDao;
        private readonly ISkillDao _skillDao;
        private readonly IContributorDao _contributorDao;
        private readonly IApiServiceDao _apiServiceDao;
        private readonly IWebsiteDao _websiteDao;
        private readonly IDependencyLibraryDao _dependencyLibraryDao;

        public SideProjectPostgresDao(string dbConnectionString, IGoalDao goalDao, IImageDao imageDao,
            ISkillDao skillDao, IContributorDao contributorDao, IApiServiceDao apiServiceDao,
            IDependencyLibraryDao dependencyLibraryDao, IWebsiteDao websiteDao)
        {
            connectionString = dbConnectionString;
            this._goalDao = goalDao;
            this._imageDao = imageDao;
            this._skillDao = skillDao;
            this._contributorDao = contributorDao;
            this._apiServiceDao = apiServiceDao;
            this._dependencyLibraryDao = dependencyLibraryDao;
            this._websiteDao = websiteDao;
        }

        /*  
            **********************************************************************************************
                                            SIDE PROJECT CRUD
            **********************************************************************************************
        */   

        public SideProject CreateSideProject(SideProject sideProject)
        {
            if (string.IsNullOrEmpty(sideProject.Name))
            {
                throw new ArgumentException("SideProject name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(sideProject.Description))
            {
                throw new ArgumentException("SideProject description cannot be null or empty.");
            }

            string sql = "INSERT INTO sideprojects (name, description, video_walkthrough_url, project_status, " +
                        "start_date, finish_date) " +
                         "VALUES (@name, @description, @video_walkthrough_url, @project_status, @start_date, " +
                         "@finish_date) " +
                         "RETURNING id";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", sideProject.Name);
                        cmd.Parameters.AddWithValue("@description", sideProject.Description);
                        cmd.Parameters.AddWithValue("@video_walkthrough_url", sideProject.VideoWalkthroughUrl);
                        cmd.Parameters.AddWithValue("@project_status", sideProject.ProjectStatus);
                        cmd.Parameters.AddWithValue("@start_date", sideProject.StartDate);
                        cmd.Parameters.AddWithValue("@finish_date", sideProject.FinishDate);

                        int id = Convert.ToInt32(cmd.ExecuteScalar());
                        sideProject.Id = id;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while creating the side project.", ex);
            }

            return sideProject;
        }     

        public List<SideProject> GetSideProjects()
        {
            List<SideProject> sideProjects = new List<SideProject>();

            string sql = "SELECT id, name, main_image_id, description, video_walkthrough_url, " +
                    "website_id, github_repo_link_id, project_status, start_date, finish_date " +
                    "FROM sideprojects";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {                    
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SideProject sideProject = MapRowToSideProject(reader);
                                sideProjects.Add(sideProject);
                            }
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the side projects.", ex);
            }

            return sideProjects;
        }

        public SideProject GetSideProject(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            SideProject sideProject = null;

            string sql = "SELECT id, name, main_image_id, description, video_walkthrough_url, " +
                    "website_id, github_repo_link_id, project_status, start_date, finish_date " +
                    "FROM sideprojects WHERE id = @sideProjectId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        sideProject = MapRowToSideProject(reader);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while retrieving the side project.", ex);
            }

            return sideProject;
        }

        public SideProject UpdateSideProject(SideProject sideProject, int sideProjectId)
        {
            if (string.IsNullOrEmpty(sideProject.Name))
            {
                throw new ArgumentException("SideProject name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(sideProject.Description))
            {
                throw new ArgumentException("SideProject description cannot be null or empty.");
            }

            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            string sql = "UPDATE sideprojects SET name = @name, description = @description, " +
                         "video_walkthrough_url = @video_walkthrough_url, project_status = @project_status, " +
                         "start_date = @start_date, finish_date = @finish_date " +
                         "WHERE id = @sideProjectId";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);
                        cmd.Parameters.AddWithValue("@name", sideProject.Name);
                        cmd.Parameters.AddWithValue("@description", sideProject.Description);
                        cmd.Parameters.AddWithValue("@video_walkthrough_url", sideProject.VideoWalkthroughUrl);
                        cmd.Parameters.AddWithValue("@project_status", sideProject.ProjectStatus);
                        cmd.Parameters.AddWithValue("@start_date", sideProject.StartDate);
                        cmd.Parameters.AddWithValue("@finish_date", sideProject.FinishDate);

                        int count = cmd.ExecuteNonQuery();

                        if (count == 1)
                        {
                            return sideProject;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while updating the side project.", ex);
            }
        }

        public int DeleteSideProject(int sideProjectId)
        {
            if (sideProjectId <= 0)
            {
                throw new ArgumentException("SideProjectId must be greater than zero.");
            }

            string sql = "DELETE FROM sideprojects WHERE id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected;
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DaoException("An error occurred while deleting the side project.", ex);
            }

        }

        /*  
            **********************************************************************************************
                                                HELPER METHODS
            **********************************************************************************************
        */

        private int? GetMainImageIdBySideProjectId(int sideProjectId)
        {
            string sql = "SELECT main_image_id FROM sideprojects WHERE id = @sideProjectId;";

            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving MainImage ID by SideProject ID: " + ex.Message);
                return null;
            }
        }

        private int? GetWebsiteIdBySideProjectId(int sideProjectId)
        {
            string sql = "SELECT website_id FROM sideprojects WHERE id = @sideProjectId;";
           
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving Website ID by SideProject ID: " + ex.Message);
                return null;
            }
        }

        private int? GetGithubIdBySideProjectId(int sideProjectId)
        {
            string sql = "SELECT github_repo_link_id FROM sideprojects WHERE id = @sideProjectId;";
           
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@sideProjectId", sideProjectId);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving GitHub ID by SideProject ID: " + ex.Message);
                return null;
            }
        }

        private int DeleteGoalsAndObjectivesBySideProjectId(int sideProjectId)
        {
            List<Goal> goals = _goalDao.GetGoalsBySideProjectId(sideProjectId);

            int goalsDeletedCount = 0;

            foreach (Goal goal in goals)
            {
                int goalId = goal.Id;

                try
                {
                    _goalDao.DeleteGoalBySideProjectId(sideProjectId, goalId);
                    goalsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting goal/objective from SideProject with side project ID:{sideProjectId} and goal ID:{goalId}: {ex.Message}");
                }
            }

            return goalsDeletedCount;
        }

        private int DeleteAdditionalImagesBySideProjectId(int sideProjectId)
        {
            List<Image> additionalImages = _imageDao.GetImagesBySideProjectId(sideProjectId);

            int imagesDeletedCount = 0;

            foreach (Image image in additionalImages)
            {
                int imageId = image.Id;

                try
                {
                    _imageDao.DeleteImageBySideProjectId(sideProjectId, imageId);
                    imagesDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting additional image from SideProject with side project ID:{sideProjectId} and image ID:{imageId}: {ex.Message}");
                }
            }

            return imagesDeletedCount;
        }

        private int DeleteToolsUsedBySideProjectId(int sideProjectId)
        {
            List<Skill> toolsUsed = _skillDao.GetSkillsBySideProjectId(sideProjectId);

            int toolsDeletedCount = 0;

            foreach (Skill tool in toolsUsed)
            {
                int toolId = tool.Id;

                try
                {
                    _skillDao.DeleteSkillBySideProjectId(sideProjectId, toolId);
                    toolsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting tool used from SideProject with side project ID:{sideProjectId} and tool ID:{toolId}: {ex.Message}");
                }
            }
            return toolsDeletedCount;
        }

        private int DeleteContributorsBySideProjectId(int sideProjectId)
        {
            List<Contributor> contributors = _contributorDao.GetContributorsBySideProjectId(sideProjectId);

            int contributorsDeletedCount = 0;

            foreach (Contributor contributor in contributors)
            {
                int contributorId = contributor.Id;

                try
                {
                    _contributorDao.DeleteContributorBySideProjectId(sideProjectId, contributorId);
                    contributorsDeletedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting contributor from SideProject with side project ID:{sideProjectId} and contributor ID:{contributorId}: {ex.Message}");
                }
            }

            return contributorsDeletedCount;
        }

        private int DeleteExternalApisAndServicesUsedBySideProjectId(int sideProjectId)
        {
            List<ApiService> externalApisAndServices = _apiServiceDao.GetAPIsAndServicesBySideProjectId(sideProjectId);

            int apisAndServicesDeleteCount = 0;

            foreach (ApiService externalApiOrService in externalApisAndServices)
            {
                int apiServiceId = externalApiOrService.Id;

                try
                {
                    _apiServiceDao.DeleteAPIOrServiceBySideProjectId(sideProjectId, apiServiceId);
                    apisAndServicesDeleteCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting external API/Service from SideProject with side project ID:{sideProjectId} and API/Service ID:{apiServiceId}: {ex.Message}");
                }
            }

            return apisAndServicesDeleteCount;
        }

        private int DeleteDependenciesAndLibrariesUsedBySideProjectId(int sideProjectId)
        {
            List<DependencyLibrary> dependenciesAndLibraries = _dependencyLibraryDao.GetDependenciesAndLibrariesBySideProjectId(sideProjectId);

            int dependenciesAndLibrariesDeleteCount = 0;

            foreach (DependencyLibrary dependencyLibrary in dependenciesAndLibraries)
            {
                int dependencyLibraryId = dependencyLibrary.Id;

                try
                {
                    _dependencyLibraryDao.DeleteDependencyOrLibraryBySideProjectId(sideProjectId, dependencyLibraryId);
                    dependenciesAndLibrariesDeleteCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting Dependency/Library from SideProject with side project ID:{sideProjectId} and Dependency/Library ID:{dependencyLibraryId}: {ex.Message}");
                }
            }

            return dependenciesAndLibrariesDeleteCount;
        }

        /*  
            **********************************************************************************************
                                            SIDE PROJECT MAP ROW
            **********************************************************************************************
        */
        private SideProject MapRowToSideProject(NpgsqlDataReader reader)
        {
            SideProject sideProject = new SideProject
            {
                Id = Convert.ToInt32(reader["id"]),
                Name = Convert.ToString(reader["name"]),
                Description = Convert.ToString(reader["description"]),
                VideoWalkthroughUrl = Convert.ToString(reader["video_walkthrough_url"]),
                ProjectStatus = Convert.ToString(reader["project_status"]),
                StartDate = Convert.ToDateTime(reader["start_date"]),
                FinishDate = Convert.ToDateTime(reader["finish_date"])
            };

            int projectId = sideProject.Id;

            SetSideProjectMainImageIdProperties(reader, sideProject, projectId);
            SetSideProjectWebsiteIdProperties(reader, sideProject, projectId);
            SetSideProjectGitHubRepoLinkIdProperties(reader, sideProject, projectId);

            sideProject.GoalsAndObjectives = _goalDao.GetGoalsBySideProjectId(projectId);
            sideProject.AdditionalImages = _imageDao.GetImagesBySideProjectId(projectId);
            sideProject.ToolsUsed = _skillDao.GetSkillsBySideProjectId(projectId);
            sideProject.Contributors = _contributorDao.GetContributorsBySideProjectId(projectId);
            sideProject.ExternalAPIsAndServicesUsed = _apiServiceDao.GetAPIsAndServicesBySideProjectId(projectId);
            sideProject.DependenciesOrLibrariesUsed = _dependencyLibraryDao.GetDependenciesAndLibrariesBySideProjectId(projectId);

            return sideProject;
        }

        private void SetSideProjectMainImageIdProperties(NpgsqlDataReader reader, SideProject sideProject, int projectId)
        {
            if (reader["main_image_id"] != DBNull.Value)
            {   
                sideProject.MainImageId = Convert.ToInt32(reader["main_image_id"]);

                int mainImageId = Convert.ToInt32(reader["main_image_id"]);
                sideProject.MainImage = _imageDao.GetImageBySideProjectId(projectId, mainImageId);
            }
            else
            {
                sideProject.MainImageId = 0;
            }
        }

        private void SetSideProjectWebsiteIdProperties(NpgsqlDataReader reader, SideProject sideProject, int projectId)
        {
            if (reader["website_id"] != DBNull.Value)
            {
                sideProject.WebsiteId = Convert.ToInt32(reader["website_id"]);

                int websiteId = Convert.ToInt32(reader["website_id"]);
                sideProject.Website = _websiteDao.GetWebsiteBySideProjectId(projectId, websiteId);
            }
            else
            {
                sideProject.WebsiteId = 0;
            }
        }

        private void SetSideProjectGitHubRepoLinkIdProperties(NpgsqlDataReader reader, SideProject sideProject, int projectId)
        {
            if (reader["github_repo_link_id"] != DBNull.Value)
            {
                sideProject.GitHubRepoLinkId = Convert.ToInt32(reader["github_repo_link_id"]);

                int githubRepoLinkId = Convert.ToInt32(reader["github_repo_link_id"]);
                sideProject.GitHubRepoLink = _websiteDao.GetWebsiteBySideProjectId(projectId, githubRepoLinkId);
            }
            else
            {
                sideProject.GitHubRepoLinkId = 0;
            }
        }

    }
}
