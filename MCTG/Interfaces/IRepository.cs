namespace MTCG.Interfaces;

public interface IRepository<T>
{
    // READ
    T Get(int id);

    IEnumerable<T> GetAll();

    // CREATE
    bool Add(T t);

    // UPDATE
    bool Update(T t, string[] parameters);

    // DELETE
    bool Delete(T t);

}