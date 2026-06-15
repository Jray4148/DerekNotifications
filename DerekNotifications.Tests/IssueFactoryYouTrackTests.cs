using DerekNotifications.Factories;
using DerekNotifications.Interfaces;
using DerekNotifications.Models.Issues;
using DerekNotifications.Models.Requests;
using DerekNotifications.Services;
using Microsoft.Extensions.Options;

namespace DerekNotifications.Tests;

public class IssueFactoryYouTrackTests
{
    private readonly IIssueFactoryYouTrack _factoryYouTrack;

    public IssueFactoryYouTrackTests()
    {
        // Setup the settings for the test environment
        var settings = new AppSettingsService
        {
            Yt = new Yt
            {
                EFile25ProjectId = "TEST-PROJECT-ID", 
                EFile25ProjectName = "TEST-PROJECT-NAME", 
                ApiToken = "test-token"
            },
            Rsi = new Rsi 
            { 
                ApiToken = "test-token",
                Emails = new Emails
                {
                    RegisterFrom = "from@example.com",
                    RegisterTo = "to@example.com",
                    EFileFrom = "efile-from@example.com",
                    EFileUnderstandTo = "understand@example.com",
                    EFileBillMeTo = "billme@example.com"
                }
            },
            Zoho = new Zoho { ClientId = "id", ClientSecret = "secret", RefreshToken = "token" }
        };

        // Wrap settings in IOptions and instantiate the factory
        var options = Options.Create(settings);
        _factoryYouTrack = new IssueFactoryYouTrack(options);
    }    
    
    [Fact]
    public void Create_DerekRegisterRequest_ReturnsDerekRegisterIssue()
    {
        // Arrange
        var request = new DerekRegisterRequest();

        // Act
        var issue = _factoryYouTrack.Create(request);

        // Assert
        Assert.NotNull(issue);
        Assert.IsType<DerekRegisterIssueYouTrack>(issue);
    }
 
    [Fact]
    public void Create_EbsRegisterRequest_ReturnsEbsRegisterIssue()
    {
        // Arrange
        var request = new EbsRegisterRequest();

        // Act
        var issue = _factoryYouTrack.Create(request);

        // Assert
        Assert.NotNull(issue);
        Assert.IsType<EbsRegisterIssueYouTrack>(issue);
    }


    [Trait("Category", "IssueCreation")]
    [Fact]
    public void Create_EbsRegisterRequest_SetsCorrectProjectId()
    {
        // Arrange
        const string expectedProjectId = "0-21";
        var request = new EbsRegisterRequest();

        // Act
        var issue = _factoryYouTrack.Create(request);

        // Assert
        Assert.NotNull(issue);
        Assert.Equal(expectedProjectId, issue.Project?.Id);
    }
    
    [Trait("Category", "IssueCreation")]
    [Fact]
    public void Create_DerekRegisterRequest_SetsCorrectProjectId()
    {
        // Arrange
        const string expectedProjectId = "0-21";
        var request = new DerekRegisterRequest();

        // Act
        var issue = _factoryYouTrack.Create(request);

        // Assert
        Assert.NotNull(issue);
        Assert.Equal(expectedProjectId, issue.Project?.Id);
    }    
    
}