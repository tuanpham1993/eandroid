using Android;
using Android.App;
using Android.Views;
using Android.Widget;
using Database.Entities;
using System.Collections.Generic;

namespace App
{
  public class CustomAdapter : BaseAdapter<Word>
  {
    public List<Word> words;
    private readonly Activity context;
    public CustomAdapter(Activity context, List<Word> words)
        : base()
    {
      this.context = context;
      this.words = words;
    }
    public override long GetItemId(int position)
    {
      return position;
    }
    public override Word this[int position]
    {
      get { return words[position]; }
    }
    public override int Count
    {
      get { return words.Count; }
    }

    public override View GetView(int position, View convertView, ViewGroup parent)
    {
      var item = words[position];
      View view = convertView;
      if (view == null) // no view to re-use, create new
        view = context.LayoutInflater.Inflate(Resource.Layout.CustomListView, null);
      view.FindViewById<TextView>(Resource.Id.Order).Text = position.ToString();
      view.FindViewById<TextView>(Resource.Id.EN).Text = item.EN;
      return view;
    }
  }
}