using MTCG.Classes;
using MTCG.Classes.CardStructure;
using MTCG.DAL;
using Npgsql;

public class UnitOfWork : IDisposable
{
    private NpgsqlConnection _connection;
    private NpgsqlTransaction _transaction;

    public UserRepository UserRepository { get; }
    public DeckRepository DeckRepository { get; }

    public TradingShopRepository ShopRepository { get; }
    public CardRepository CardRepository { get; }

    private static UnitOfWork _instance;

    public static UnitOfWork Instance
    {
        get
        {
            if (_instance is null)
            {
                throw new InvalidOperationException("Database not initialized. Call UnitOfWork first.");
            }

            return _instance;

        }
    }



    public UnitOfWork(string connectionstring)
    {
        _connection = new NpgsqlConnection(connectionstring);
        _connection.Open();
        CardRepository = CardRepository.Instance;
        UserRepository = UserRepository.Instance;
        DeckRepository = DeckRepository.Instance;
        ShopRepository = TradingShopRepository.Instance;
    }

    public static void InitUnitOfWork(string connectionstring)
    {
        _instance = new UnitOfWork(connectionstring);
    }



    public void BeginTransaction()
    {
        _transaction = _connection.BeginTransaction();
        UserRepository.SetTransaction(_transaction);
        DeckRepository.SetTransaction(_transaction);
        ShopRepository.SetTransaction(_transaction);
        CardRepository.SetTransaction(_transaction);
    }

    public void Commit()
    {
        _transaction?.Commit();
        DisposeTransaction();
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        DisposeTransaction();
    }

    private void DisposeTransaction()
    {
        _transaction?.Dispose();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection.Dispose();
    }
}