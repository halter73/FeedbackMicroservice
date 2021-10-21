using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connection = new SqliteConnection("Filename=:memory:");
connection.Open();

var dbOptions = new DbContextOptionsBuilder<FeedbackDb>().UseSqlite(connection).Options;

using (var dbContext = new FeedbackDb(dbOptions))
{
    dbContext.Database.EnsureCreated();
}

builder.Services.AddSingleton(dbOptions);
builder.Services.AddScoped<FeedbackDb>();

var app = builder.Build();

app.MapPost("/feedback/submit", async (FeedbackDb db, NewFeedback feedback) =>
{
    if (feedback.Rating < 1 || feedback.Rating > 5)
    {
        return Results.BadRequest(new
        {
            Details = "Rating must be between 1 and 5!"
        });
    }

    var savedFeedback = new Feedback
    {
        Rating = feedback.Rating,
        Comment = feedback.Comment,
    };

    db.Add(savedFeedback);
    await db.SaveChangesAsync();
    return Results.Created($"/feedback/{savedFeedback.Id}", savedFeedback);
});

app.MapGet("/feedback/{id}", async (FeedbackDb db, int id) =>
    await db.Feedback.FirstOrDefaultAsync(feedback => feedback.Id == id)
        is Feedback feedback
            ? Results.Ok(feedback)
            : Results.NotFound());

app.MapGet("/feedback/pending", async (FeedbackDb db) =>
    await db.Feedback.Where(feedback => !feedback.WasReviewed).ToListAsync());

app.MapPost("/feedback/review/{id}", async (FeedbackDb db, int id, Review? review) =>
{
    var feedback = await db.Feedback.FirstOrDefaultAsync(feedback => feedback.Id == id);

    if (feedback is null)
    {
        return Results.NotFound();
    }

    feedback.WasReviewed = true;
    feedback.ReviewNotes = review?.Notes;

    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();

record NewFeedback(int Rating, string? Comment);
record Review(string? Notes);

class Feedback
{
    public int Id { get; set; }

    public int Rating { get; set; }
    public string? Comment { get; set; }

    public bool WasReviewed { get; set; }
    public string? ReviewNotes { get; set; }
}

class FeedbackDb : DbContext
{
    public FeedbackDb(
        DbContextOptions<FeedbackDb> options)
        : base(options) { }

    public DbSet<Feedback> Feedback => Set<Feedback>();
}
