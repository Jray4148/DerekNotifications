using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DerekNotifications;
using DerekNotifications.Delegates;
using DerekNotifications.Factories;
using DerekNotifications.Interfaces;
using DerekNotifications.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCors(options =>
{
    options.AddPolicy(Constants.CorsPolicyName,
        policyBuilder => policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IDynamoService, DynamoService>();
builder.Services.AddScoped<ICsvService, CsvService>();
builder.Services.AddTransient<ZohoTokenRefreshHandler>();
builder.Services.Configure<AppSettingsService>(builder.Configuration);
builder.Services.AddScoped<IIssueFactoryYouTrack, IssueFactoryYouTrack>();
builder.Services.AddScoped<IEmailFactory, EmailFactory>();

// HttpClient for Zoho Invoices
builder.Services.AddHttpClient<IInvoicesServices, InvoicesServiceZoho>(client =>
{
    client.BaseAddress = new Uri(Constants.Zoho.InvoicesUrl);
    client.DefaultRequestHeaders.Add("X-com-zoho-invoice-organizationid", Constants.Zoho.OrganizationId);
}).AddHttpMessageHandler<ZohoTokenRefreshHandler>();

builder.Services.AddHttpClient<ITasksServiceLessAnnoying, TasksServiceLessAnnoying>(client =>
{
    client.BaseAddress = new Uri(Constants.LessAnnoying.Url);
    client.DefaultRequestHeaders.Add("Authorization", $"{builder.Configuration.GetValue<string>("LESS_ANNOYING_API_TOKEN")}");
});

// HttpClient for Zoho Contacts
builder.Services.AddHttpClient<IContactsService, ContactsServiceZoho>(client =>
{
    client.BaseAddress = new Uri(Constants.Zoho.ContactsUrl);
    client.DefaultRequestHeaders.Add("X-com-zoho-invoice-organizationid", Constants.Zoho.OrganizationId);
}).AddHttpMessageHandler<ZohoTokenRefreshHandler>();

// HttpClient for YouTrack
Action<IServiceProvider, HttpClient> youtrackConfig = (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<AppSettingsService>>().Value;
    client.BaseAddress = new Uri(Constants.Yt.Url);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.Yt.ApiToken}");
};
builder.Services.AddHttpClient<ITicketsService, TicketsServiceYouTrack>(youtrackConfig);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITokenStorageService, InMemoryTokenStorageServiceZoho>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_GekDUPL2X";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateAudience = false, // Access tokens don't have 'aud' claim
            ValidIssuer = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_GekDUPL2X"
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DerekNotifications API v1"); 
    });    
}

app.UseHttpsRedirection();
app.UseCors(Constants.CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();