using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using Database.Entities;
using System.Linq;
using Android.Content.Res;
using System.Collections.Generic;
using Android.Webkit;
using HtmlAgilityPack;
using System.Threading.Tasks;
using Android.Content.PM;

namespace App
{
  [Activity(Label = "EAndroid", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
  public class MainActivity : Activity
  {
    public EAppContext db;
    public List<Word> words;
    private int selectedItem;
    private WebView webView;
    private ListView listView;
    private CustomAdapter adapter;

    public MainActivity()
    {
      selectedItem = -1;
      var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "android.db");
      db = new EAppContext(dbPath);

    }
    protected override async void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);
      SetContentView(Resource.Layout.Main);
      webView = FindViewById<WebView>(Resource.Id.webView1);
      listView = FindViewById<ListView>(Resource.Id.listView1);
      db.Database.EnsureCreated();
      //var old = db.Words;
      //db.Words.RemoveRange(old);
      //db.SaveChanges();

      //List<Word> list = new List<Word>();
      //AssetManager assets = this.Assets;
      //StreamReader sr = new StreamReader(assets.Open("word.txt"));
      //var i = 1;
      //while (sr.Peek() >= 0)
      //{
      //  list.Add(new Word
      //  {
      //    Id = i,
      //    Order = i,
      //    IsForgeted = false,
      //    IsCommon = false,
      //    VI = null,
      //    EN = sr.ReadLine().Trim()
      //  });
      //  i++;
      //}
      //db.Words.AddRange(list);
      //db.SaveChanges();

      words = db.Words.Where(x => !x.IsCommon).ToList();
      adapter = new CustomAdapter(this, words);
      listView.Adapter = adapter;

      listView.ItemClick += OnListItemClick;
    }

    public override bool OnPrepareOptionsMenu(IMenu menu)
    {
      menu.Clear();
      MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
      return base.OnPrepareOptionsMenu(menu);
    }

    public override bool OnOptionsItemSelected(IMenuItem item)
    {
      switch (item.ItemId)
      {
        case Resource.Id.addForget:
          if (this.selectedItem >= 0)
          {
            words.ElementAt(selectedItem).IsForgeted = true;
            db.SaveChanges();
            Toast.MakeText(this, "Add to forget", ToastLength.Short).Show();
          }
          return true;
        case Resource.Id.addCommon:
          if (this.selectedItem >= 0)
          {
            words.ElementAt(selectedItem).IsCommon = true;
            db.SaveChanges();
            Toast.MakeText(this, "Add to common", ToastLength.Short).Show();
          }
          return true;
        case Resource.Id.viewForget:
          words = db.Words.Where(x => x.IsForgeted).ToList();
          adapter.words = words;
          adapter.NotifyDataSetChanged();
          return true;
        case Resource.Id.viewAll:
          words = db.Words.Where(x => !x.IsForgeted && !x.IsCommon).OrderBy(x => x.Order).ToList();
          adapter.words = words;
          adapter.NotifyDataSetChanged();
          return true;
        case Resource.Id.viewCommon:
          words = db.Words.Where(x => x.IsCommon).ToList();
          adapter.words = words;
          adapter.NotifyDataSetChanged();
          return true;
        case Resource.Id.removeForget:
          var temp = db.Words.Where(x => x.IsForgeted);
          foreach (var i in temp)
          {
            i.IsForgeted = false;
          }
          db.SaveChanges();
          words = db.Words.Where(x => !x.IsForgeted && !x.IsCommon).OrderBy(x => x.Order).ToList();
          adapter.words = words;
          adapter.NotifyDataSetChanged();
          return true;
        case Resource.Id.reload:
          words = db.Words.Where(x => !x.IsCommon).ToList();
          words = Shuffle(words);
          adapter.words = words;
          adapter.NotifyDataSetChanged();
          return true;
      }
      return base.OnOptionsItemSelected(item);
    }

    private string getTranslate(string en)
    {
      try
      {
        HtmlWeb web = new HtmlWeb();
        //string url = "http://tratu.soha.vn/dict/en_vn/" + en;
        string url = "http://tratu.coviet.vn/hoc-tieng-anh/tu-dien/lac-viet/A-V/" + en + ".html";

        HtmlAgilityPack.HtmlDocument document = web.Load(url);
        var html = document.DocumentNode.SelectNodes("//div[@id='mtd_0']/div/div[@class='p10']").ElementAt(0).InnerHtml;
        //var html = document.DocumentNode.SelectNodes("//div[@id='show-alter']").ElementAt(0).InnerHtml;
        return html;
      }
      catch (Exception e)
      {
        return null;
      }
    }

    protected async void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
    {
      selectedItem = e.Position;
      var word = words.ElementAt(e.Position);
      var html = word.VI;
      if (html != null)
      {
        webView.LoadData(html, "text/html; charset=utf-8", "UTF-8");
      }
      else
      {
        var VI = await Task.Run(() =>
        {
          return getTranslate(word.EN);
        });
        if (VI != null)
        {
          word.VI = VI;
          db.SaveChanges();
          webView.LoadData(VI, "text/html; charset=utf-8", "UTF-8");
        }
        else
        {
          webView.LoadData("404", "text/html; charset=utf-8", "UTF-8");
        }
      }
    }

    public List<Word> Shuffle(List<Word> input)
    {
      Random _random = new Random();
      int n = input.Count;
      for (int i = 0; i < n; i++)
      {
        int r = i + (int)(_random.NextDouble() * (n - i));
        Word w = input.ElementAt(r);
        input[r] = input.ElementAt(i);
        input[i] = w;
      }
      var count = 0;
      foreach(var item in input)
      {
        item.Order = count++;
      }
      db.SaveChanges();
      return input;
    }
  }
}

