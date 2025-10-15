# Giai đoạn 1: Build (Sử dụng SDK để biên dịch)
# Sử dụng phiên bản .NET SDK mới nhất
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Sao chép file Solution và Project file của WebApi để Restore Dependencies
# Đảm bảo tên file .sln và .csproj là chính xác
COPY *.sln .
COPY Serein.Candle.WebApi/Serein.Candle.WebApi.csproj Serein.Candle.WebApi/
COPY Serein.Candle.Application/Serein.Candle.Application.csproj Serein.Candle.Application/
COPY Serein.Candle.Domain/Serein.Candle.Domain.csproj Serein.Candle.Domain/
COPY Serein.Candle.Infrastructure/Serein.Candle.Infrastructure.csproj Serein.Candle.Infrastructure/

# Chạy restore cho Solution
RUN dotnet restore

# Sao chép toàn bộ mã nguồn
COPY . .

# Build và Publish dự án WebApi
# Chỉ Publish project WebApi (điểm vào của ứng dụng)
WORKDIR /src/Serein.Candle.WebApi
RUN dotnet publish "Serein.Candle.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# ----------------------------------------------------------------------
# Giai đoạn 2: Final (Chỉ chứa Runtime - Bảo mật và nhẹ hơn)
# Sử dụng phiên bản ASP.NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Sao chép output từ giai đoạn publish
COPY --from=build /app/publish .

# Thiết lập cổng mặc định (8080 cho Render/Docker)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Điểm khởi động: Chạy file DLL của project WebApi
# Tên file DLL thường là tên project
ENTRYPOINT ["dotnet", "Serein.Candle.WebApi.dll"]