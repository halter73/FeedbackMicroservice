using Azure.Data.Tables;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var feedbackTable = new TableClient(app.Configuration.GetConnectionString("CosmosTableApi"), "Feedback");
var appNames = app.Configuration.GetSection("AppNames").Get<string[]>();

app.Use((httpContext, next) =>
{
    if (!appNames.Contains(httpContext.GetRouteValue("appName")))
    {
        httpContext.Response.StatusCode = 400;
        return Task.CompletedTask;
    }

    return next(httpContext);
});

app.MapPost("/{appName}/feedback", async (string appName, Feedback feedback) =>
{
    if (feedback.Rating < 1 || feedback.Rating > 5)
    {
        return Results.BadRequest(new
        {
            Details = "Rating must be between 1 and 5!"
        });
    }

    var entity = new TableEntity(appName, Guid.NewGuid().ToString())
    {
        ["Rating"] = feedback.Rating,
        ["Comment"] = feedback.Comment,
    };

    await feedbackTable.AddEntityAsync(entity);

    return Results.Created($"/{appName}/feedback/{entity.RowKey}", entity);
});

app.MapGet("/{appName}/feedback", (string appName) =>
    feedbackTable.QueryAsync<TableEntity>($"PartitionKey eq '{appName}'"));

app.MapGet("/{appName}/feedback/{id}", (string appName, Guid id) =>
    feedbackTable.GetEntityAsync<TableEntity>(appName, id.ToString()));

app.Run();

record Feedback(int Rating, string? Comment);
