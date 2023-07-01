using Pipelines;
using System.Linq;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var pipeline = new Pipeline()
    .First(async () => new Success<string[]>(await File.ReadAllLinesAsync("data1.csv")))
    .Then<string[]>(prev => {
        return new Success<List<string[]>>(
            prev!
                .Select(l => l.Split(","))
                .ToList()
        );
    })
    .Then<List<string[]>>(prev => {
        return new Success<int>(prev.Count());
    });


var csvCountResult = await pipeline.InvokeAsync();
Console.WriteLine(csvCountResult.GetType().Name);
if(csvCountResult is Success<int> success)
    Console.WriteLine($"CSV contains {success.Value} rows");