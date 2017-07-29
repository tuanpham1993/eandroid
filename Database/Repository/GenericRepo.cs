using Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Database.Repository
{
  public class GenericRepo<T> where T : class, new()
  {
    protected EAppContext db;
    protected DbSet<T> table;
    public GenericRepo(EAppContext db)
    {
      this.db = db;
      table = this.db.Set<T>();
    }

    public T Get(object id)
    {
      try
      {
        return table.Find(id);
      }
      catch (Exception e)
      {
        return null;
      }
    }

    public IEnumerable<T> Get()
    {
      return table;
    }

    public IEnumerable<T> GetAsNoTracking()
    {
      return table.AsNoTracking();
    }

    public void Add(T obj)
    {
      table.Add(obj);
    }

    public void Update(T obj)
    {
      table.Attach(obj);
      db.Entry(obj).State = EntityState.Modified;
    }

    public void Remove(T obj)
    {
      table.Remove(obj);
    }

    public void Remove(int id)
    {
      var row = table.Find(id);
      if (row != null)
      {
        table.Remove(row);
      }
    }

    public void Remove(string id)
    {
      var row = table.Find(id);
      if (row != null)
      {
        table.Remove(row);
      }
    }

    public void Remove()
    {
      var model = table;
      table.RemoveRange(model);
    }

    public void Save()
    {
      db.SaveChanges();
    }

    public bool IsExist(object id)
    {
      return table.Find(id) != null;
    }

    public string GenerateId()
    {
      return Guid.NewGuid().ToString();
    }

    public void RemoveRange(IEnumerable<T> list)
    {
      table.RemoveRange(list);
    }

    public void AddRange(IEnumerable<T> list)
    {
      table.AddRange(list);
    }

    private bool disposed = false;
    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        if (disposing)
        {
          db.Dispose();
        }
      }
      disposed = true;
    }
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}
