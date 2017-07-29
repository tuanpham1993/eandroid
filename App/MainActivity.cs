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
    private readonly EAppContext db;
    private IEnumerable<Word> words;
    private WebView webView;
    public MainActivity()
    {
      var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "exrin.db");
      db = new EAppContext(dbPath);
      
    }
    protected override async void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);
      SetContentView(Resource.Layout.Main);
      webView = FindViewById<WebView>(Resource.Id.webView1);
      
      db.Database.EnsureCreated();

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
      //    VI = null,
      //    EN = sr.ReadLine().Trim()
      //  });
      //  i++;
      //}
      //db.Words.AddRange(list);
      //db.SaveChanges();

      words = db.Words;
      ListView listView1 = FindViewById<ListView>(Resource.Id.listView1);
      listView1.Adapter = new CustomAdapter(this, words.ToList());

      listView1.ItemClick += OnListItemClick;
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
        word.VI = VI;
        db.SaveChanges();
        webView.LoadData(VI, "text/html; charset=utf-8", "UTF-8");
      }
    }
  }
}

