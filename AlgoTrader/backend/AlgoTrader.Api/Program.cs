using AlgoTrader.Api.Hubs;
using AlgoTrader.Core.Risk.Implementations;
using AlgoTrader.Core.Risk.Interfaces;
using AlgoTrader.Trading.Brokers.Implementations;
using AlgoTrader.Trading.Brokers.Interfaces;
using AlgoTrader.Trading.MarketData.Implementations;
using AlgoTrader.Trading.MarketData.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddSingleton<KillSwitch>();
builder.Services.AddSingleton<IRiskManager, RiskManager>();
builder.Services.AddSingleton<ITradingBroker, PaperTradingBroker>();
builder.Services.AddSingleton<IMarketDataFeed, ZerodhaWebSocketFeed>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapHub<MarketHub>("/marketHub");

app.Run();
