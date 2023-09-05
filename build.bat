IF "%1"=="-clean" dotnet clean
dotnet build
cls
dotnet run --project EagleBot