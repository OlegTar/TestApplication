var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const int AllocationSize = 25 * 1024 * 1024;
var allocations = new List<byte[]>();
var allocationsLock = new object();

app.MapGet("/increase", () =>
{
    try
    {
        lock (allocationsLock)
        {
            allocations.Add(new byte[AllocationSize]);
            return Results.Ok(new
            {
                allocatedBytes = allocations.Count * AllocationSize,
                allocatedMegabytes = allocations.Count * 25
            });
        }
    }
    catch (OutOfMemoryException exception)
    {
        Environment.FailFast("The process ran out of memory.", exception);
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
});

app.Run();

