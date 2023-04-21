namespace DntConsole.DataLayer.Context;

public interface IAppDbContextFactory
{
    AppDbContext CreateScopedDbContext();
    string GetConnectionString();
    Task InitDatabaseAsync();
    void EnsureDatabaseCreated();
}