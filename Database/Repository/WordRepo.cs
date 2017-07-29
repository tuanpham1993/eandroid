using Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Repository
{
  public class WordRepo : GenericRepo<Word>
  {
    public WordRepo(EAppContext db) : base(db)
    {
    }
  }
}
