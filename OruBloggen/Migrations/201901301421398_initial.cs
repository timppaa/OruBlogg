namespace OruBloggen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategoryModels",
                c => new
                {
                    CategoryID = c.Int(nullable: false, identity: true),
                    CategoryName = c.String(),
                })
                .PrimaryKey(t => t.CategoryID);

            CreateTable(
                "dbo.PostModels",
                c => new
                {
                    PostID = c.Int(nullable: false, identity: true),
                    PostTitle = c.String(),
                    PostText = c.String(),
                    PostDate = c.DateTime(nullable: false),
                    PostFilePath = c.String(),
                    PostFormal = c.Boolean(nullable: false),
                    PostUserID = c.String(maxLength: 128),
                    PostCategoryID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.PostID)
                .ForeignKey("dbo.UserModels", t => t.PostUserID)
                .ForeignKey("dbo.CategoryModels", t => t.PostCategoryID, cascadeDelete: true)
                .Index(t => t.PostUserID)
                .Index(t => t.PostCategoryID);

            CreateTable(
                "dbo.PostReportModels",
                c => new
                {
                    PostReportID = c.Int(nullable: false, identity: true),
                    PostID = c.Int(nullable: false),
                    ReportID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.PostReportID)
                .ForeignKey("dbo.ReportModels", t => t.ReportID, cascadeDelete: true)
                .ForeignKey("dbo.PostModels", t => t.PostID, cascadeDelete: true)
                .Index(t => t.PostID)
                .Index(t => t.ReportID);

            CreateTable(
                "dbo.ReportModels",
                c => new
                {
                    ReportID = c.Int(nullable: false, identity: true),
                    ReportReason = c.String(),
                })
                .PrimaryKey(t => t.ReportID);

            CreateTable(
                "dbo.UserModels",
                c => new
                {
                    UserID = c.String(nullable: false, maxLength: 128),
                    UserFirstname = c.String(),
                    UserLastname = c.String(),
                    UserBirthDate = c.DateTime(nullable: false),
                    UserPhoneNumber = c.Int(nullable: false),
                    UserImagePath = c.String(),
                    UserIsAdmin = c.Boolean(nullable: false),
                    UserTeamID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.UserID)
                .ForeignKey("dbo.TeamModels", t => t.UserTeamID, cascadeDelete: true)
                .Index(t => t.UserTeamID);

            CreateTable(
                "dbo.MeetingModels",
                c => new
                {
                    MeetingID = c.Int(nullable: false, identity: true),
                    MeetingTitle = c.String(),
                    MeetingDesc = c.String(),
                    MeetingDate = c.DateTime(nullable: false),
                    MeetingActive = c.Boolean(nullable: false),
                    MeetingUserID = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.MeetingID)
                .ForeignKey("dbo.UserModels", t => t.MeetingUserID)
                .Index(t => t.MeetingUserID);

            CreateTable(
                "dbo.UserMeetingModels",
                c => new
                {
                    UserMeetingID = c.Int(nullable: false, identity: true),
                    MeetingID = c.Int(nullable: false),
                    UserID = c.String(maxLength: 128),
                    AcceptedInvite = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.UserMeetingID)
                .ForeignKey("dbo.MeetingModels", t => t.MeetingID, cascadeDelete: true)
                .ForeignKey("dbo.UserModels", t => t.UserID)
                .Index(t => t.MeetingID)
                .Index(t => t.UserID);

            CreateTable(
                "dbo.MessageModels",
                c => new
                {
                    MessageID = c.Int(nullable: false, identity: true),
                    MessageSenderID = c.String(maxLength: 128),
                    MessageReceiverID = c.String(maxLength: 128),
                    MessageTitle = c.String(),
                    MessageText = c.String(),
                    MessageRead = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.MessageID)
                .ForeignKey("dbo.UserModels", t => t.MessageReceiverID)
                .ForeignKey("dbo.UserModels", t => t.MessageSenderID)
                .Index(t => t.MessageSenderID)
                .Index(t => t.MessageReceiverID);

            CreateTable(
                "dbo.ProjectModels",
                c => new
                {
                    ProjectID = c.Int(nullable: false, identity: true),
                    ProjectType = c.String(),
                    ProjectName = c.String(),
                    ProjectDesc = c.String(),
                    ProjectStatus = c.String(),
                    ProjectFilePath = c.String(),
                    ProjectTeamID = c.Int(nullable: false),
                    ProjectUserID = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.ProjectID)
                .ForeignKey("dbo.TeamModels", t => t.ProjectTeamID, cascadeDelete: true)
                .ForeignKey("dbo.UserModels", t => t.ProjectUserID)
                .Index(t => t.ProjectTeamID)
                .Index(t => t.ProjectUserID);

            CreateTable(
                "dbo.TeamModels",
                c => new
                {
                    TeamID = c.Int(nullable: false, identity: true),
                    TeamName = c.String(),
                })
                .PrimaryKey(t => t.TeamID);

        }

        public override void Down()
        {
            DropForeignKey("dbo.PostModels", "PostCategoryID", "dbo.CategoryModels");
            DropForeignKey("dbo.UserMeetingModels", "UserID", "dbo.UserModels");
            DropForeignKey("dbo.ProjectModels", "ProjectUserID", "dbo.UserModels");
            DropForeignKey("dbo.UserModels", "UserTeamID", "dbo.TeamModels");
            DropForeignKey("dbo.ProjectModels", "ProjectTeamID", "dbo.TeamModels");
            DropForeignKey("dbo.PostModels", "PostUserID", "dbo.UserModels");
            DropForeignKey("dbo.MessageModels", "MessageSenderID", "dbo.UserModels");
            DropForeignKey("dbo.MessageModels", "MessageReceiverID", "dbo.UserModels");
            DropForeignKey("dbo.MeetingModels", "MeetingUserID", "dbo.UserModels");
            DropForeignKey("dbo.UserMeetingModels", "MeetingID", "dbo.MeetingModels");
            DropForeignKey("dbo.PostReportModels", "PostID", "dbo.PostModels");
            DropForeignKey("dbo.PostReportModels", "ReportID", "dbo.ReportModels");
            DropIndex("dbo.ProjectModels", new[] { "ProjectUserID" });
            DropIndex("dbo.ProjectModels", new[] { "ProjectTeamID" });
            DropIndex("dbo.MessageModels", new[] { "MessageReceiverID" });
            DropIndex("dbo.MessageModels", new[] { "MessageSenderID" });
            DropIndex("dbo.UserMeetingModels", new[] { "UserID" });
            DropIndex("dbo.UserMeetingModels", new[] { "MeetingID" });
            DropIndex("dbo.MeetingModels", new[] { "MeetingUserID" });
            DropIndex("dbo.UserModels", new[] { "UserTeamID" });
            DropIndex("dbo.PostReportModels", new[] { "ReportID" });
            DropIndex("dbo.PostReportModels", new[] { "PostID" });
            DropIndex("dbo.PostModels", new[] { "PostCategoryID" });
            DropIndex("dbo.PostModels", new[] { "PostUserID" });
            DropTable("dbo.TeamModels");
            DropTable("dbo.ProjectModels");
            DropTable("dbo.MessageModels");
            DropTable("dbo.UserMeetingModels");
            DropTable("dbo.MeetingModels");
            DropTable("dbo.UserModels");
            DropTable("dbo.ReportModels");
            DropTable("dbo.PostReportModels");
            DropTable("dbo.PostModels");
            DropTable("dbo.CategoryModels");
        }
    }
    
}
