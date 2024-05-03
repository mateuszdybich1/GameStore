using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;

namespace GameStore.Users;
public class UserUnbanService(string conn) : IHostedService, IDisposable
{
    private readonly string _conn = conn;
    private Timer _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();

        GC.SuppressFinalize(this);
    }

    private void DoWork(object? state)
    {
        var query = "UPDATE AspNetUsers " +
            "SET IsBanned = 0, BanDuration = '', BanTime = NULL " +
            "WHERE IsBanned = 1 " +
            "AND BanDuration != 'Permanent' " +
            "AND DATEADD(" +
                "MINUTE, " +
                "CASE BanDuration " +
                    "WHEN '1 hour' THEN 60 " +
                    "WHEN '1 week' THEN 60 * 24 * 7 " +
                    "WHEN '1 month' THEN 60 * 24 * 30 ELSE 0 END, " +
                "BanTime) " +
            "<= GETUTCDATE();";

        using var connection = new SqlConnection(_conn);
        connection.Open();

        using var command = new SqlCommand(query, connection);
        command.ExecuteNonQuery();
    }
}
