using Pipelines;
using System.Linq;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var pipeline = new Pipeline()
    .First(async () => new Success<string[]>(await File.ReadAllLinesAsync("data1.csv")))
    .Then<string[]>(prev => {
        Console.WriteLine("Splitting lines");
        return new Success<List<string[]>>(
            prev!
                .Select(l => l.Split(","))
                .ToList()
        );
    })
    .Then<List<string[]>>(prev => {
        Console.WriteLine("Counting lines");
        return new Success<int>(prev.Count());
    })
    .HandleError(error => {
        if(error.Exception is FileNotFoundException)
            return new Success<int>(15);
        
        return error;
    });


var csvCountResult = await pipeline.InvokeAsync();
Console.WriteLine(csvCountResult.GetType().Name);
Console.WriteLine($"CSV contains {csvCountResult.Unwrap<int>()} rows");