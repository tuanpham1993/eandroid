using Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Repository
{
  public class CommonWordRepo : GenericRepo<CommonWord>
  {
    public CommonWordRepo(EAppContext db) : base(db)
    {
    }
  }
}
