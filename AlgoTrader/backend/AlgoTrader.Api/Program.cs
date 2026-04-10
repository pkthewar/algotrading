using AlgoTrader.Api.Hubs.Implementations;
using AlgoTrader.Core.Risk.Implementations;
using AlgoTrader.Core.Risk.Interfaces;
using AlgoTrader.Engine.Implementations;
using AlgoTrader.Engine.Interfaces;
using AlgoTrader.MarketData.Implementations;
using AlgoTrader.MarketData.Interfaces;
using AlgoTrader.Trading.Brokers.Implementations;
using AlgoTrader.Trading.Brokers.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddSingleton<KillSwitch>();
builder.Services.AddSingleton<IRiskManager, RiskManager>();
// register market data before paper broker so DI can inject
builder.Services.AddSingleton<IMarketDataFeed, ZerodhaWebSocketFeed>();
builder.Services.AddSingleton<ITradingBroker, PaperTradingBroker>();
builder.Services.AddSingleton<IStrategyExecutionEngine, StrategyExecutionEngine>();
builder.Services.AddSingleton<MarketHub>();
// Register event publisher for broadcasting trades/portfolios from execution engine
builder.Services.AddSingleton<AlgoTrader.Api.Services.BrokerEventPublisher>();
builder.Services.AddSingleton<AlgoTrader.Core.Models.IEventPublisher>(sp => sp.GetRequiredService<AlgoTrader.Api.Services.BrokerEventPublisher>());

string[] allowedOrigins = ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("TradingCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
        //Allowed Headers
        .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
        //Allowed Methods
        .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
        //Required for SignalR
        .AllowCredentials()
        //Time till which browser will remember the request. Often called Preflight request. 
        .SetPreflightMaxAge(TimeSpan.FromSeconds(10));
    });
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("TradingCors");

app.UseAuthorization();

app.MapControllers();

app.MapHub<MarketHub>("/marketHub");

app.Run();
