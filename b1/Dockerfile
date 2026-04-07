# STAGE 1: Môi trường chạy (Runtime) - Dùng .NET 10
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# STAGE 2: Môi trường biên dịch (Build) - Dùng SDK .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy file .csproj và restore
COPY ["b1.csproj", "./"]
RUN dotnet restore "b1.csproj"

# Copy toàn bộ và build
COPY . .
RUN dotnet build "b1.csproj" -c Release -o /app/build

# STAGE 3: Đóng gói
FROM build AS publish
RUN dotnet publish "b1.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 4: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "b1.dll"]