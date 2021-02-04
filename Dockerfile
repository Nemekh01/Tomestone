FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY ["./bin/Release/net5.0/publish", "/publish"]
WORKDIR /publish
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "LuminaAPI.dll"]