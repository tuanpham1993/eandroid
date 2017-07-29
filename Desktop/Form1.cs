using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
//using WMPLib;
using EApp.Entities;
using System.Diagnostics;

namespace EApp
{
  public partial class Form1 : Form
  {
    public EntityModel db;
    static Random _random = new Random();
    public string filePath;
    public Form1()
    {
      db = new EntityModel();
      InitializeComponent();
    }

    public void renderTableData()
    {
      var datasource = db.Words.Where(x => x.Order != 0).OrderBy(x => x.Order).Select(x => new TableViewModel() { EN = x.EN, Order = x.Order + 1 }).ToList();
      dataGridView1.DataSource = datasource;
    }
    //reload all data
    public void firstLoadData()
    {
      var datasource = db.Words.ToList();
      datasource = Shuffle(datasource);
      for (var i = 0; i < datasource.Count; i++)
      {
        datasource.ElementAt(i).Order = i + 1;
      }
      db.SaveChanges();
      var source = datasource.Select((x, i) => new TableViewModel() { EN = x.EN, Order = i + 1 }).ToList();

      dataGridView1.DataSource = source;
    }

    static List<Word> Shuffle(List<Word> input)
    {
      int n = input.Count;
      for (int i = 0; i < n; i++)
      {
        int r = i + (int)(_random.NextDouble() * (n - i));
        Word w = input.ElementAt(r);
        input[r] = input.ElementAt(i);
        input[i] = w;
      }
      return input;
    }

    public List<Word> LoadDataFromWeb(string url, string xPath)
    {
      List<Word> oldw = db.Words.ToList();
      List<CommonWord> oldc = db.CommonWords.ToList();

      List<Word> model = new List<Word>();
      HtmlWeb web = new HtmlWeb();
      HtmlAgilityPack.HtmlDocument document = web.Load(url);
      var words = document.DocumentNode.SelectNodes(xPath).ToList();
      foreach (var item in words)
      {
        if (!oldw.Any(x => x.EN == item.InnerText) && !oldc.Any(x => x.EN == item.InnerText))
        {
          model.Add(new Word
          {
            EN = item.InnerText
          });
        }
      }
      model = model.GroupBy(x => x.EN).Select(x => x.First()).ToList();
      return model;

    }

    public List<Word> LoadDataFromWeb(string url, string page, string xPath)
    {
      List<Word> oldw = db.Words.ToList();
      List<CommonWord> oldc = db.CommonWords.ToList();

      List<Word> model = new List<Word>();
      for (var i = 1; true; i++)
      {
        try
        {
          string link = url + page + i;
          HtmlWeb web = new HtmlWeb();
          HtmlAgilityPack.HtmlDocument document = web.Load(link);
          var words = document.DocumentNode.SelectNodes(xPath).ToList();
          foreach (var item in words)
          {
            if (!oldw.Any(x => x.EN == item.InnerText) && !oldc.Any(x => x.EN == item.InnerText))
            {
              model.Add(new Word
              {
                EN = item.InnerText
              });
            }
          }
        }
        catch (Exception e)
        {
          break;
        }
      }
      model = model.GroupBy(x => x.EN).Select(x => x.First()).ToList();
      return model;

    }

    public List<Word> LoadDataFromFile(string filePath, string xPath)
    {
      List<Word> oldw = db.Words.ToList();
      List<CommonWord> oldc = db.CommonWords.ToList();

      var htmlDoc = new HtmlAgilityPack.HtmlDocument();
      htmlDoc.Load(filePath);
      var words = htmlDoc.DocumentNode.SelectNodes(xPath).ToList();
      List<Word> model = new List<Word>();
      foreach (var item in words)
      {
        if (!oldw.Any(x => x.EN == item.InnerText) && !oldc.Any(x => x.EN == item.InnerText))
        {
          model.Add(new Word
          {
            EN = item.InnerText
          });
        }
      }
      model = model.GroupBy(x => x.EN).Select(x => x.First()).ToList();
      return model;
    }

    private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      var en = dataGridView1.Rows[e.RowIndex].Cells["EN"].Value.ToString();
      await Task.Run(() =>
      {
        translate(en);
      });
    }

    private void translate(string en)
    {
      try
      {
        HtmlWeb web = new HtmlWeb();
        string url = "http://tratu.soha.vn/dict/en_vn/" + en;
        HtmlAgilityPack.HtmlDocument document = web.Load(url);
        //string url = "http://tratu.coviet.vn/hoc-tieng-anh/tu-dien/lac-viet/A-V/" + en + ".html";
        //var html = document.DocumentNode.SelectNodes("//div[@id='mtd_0']/div/div[@class='p10']").ElementAt(0).InnerHtml;
        var html = document.DocumentNode.SelectNodes("//div[@id='show-alter']").ElementAt(0).InnerHtml;
        webBrowser1.DocumentText = html;
      }
      catch (Exception e)
      {
        webBrowser1.DocumentText = "Error!!!!!!!!!!!!!!!!!!!!!!!!!!!!!";
      }
    }

    private string getTranslate(string en)
    {
      try
      {
        Debug.WriteLine(en);
        HtmlWeb web = new HtmlWeb();
        string url = "http://tratu.soha.vn/dict/en_vn/" + en;
        HtmlAgilityPack.HtmlDocument document = web.Load(url);
        //string url = "http://tratu.coviet.vn/hoc-tieng-anh/tu-dien/lac-viet/A-V/" + en + ".html";
        //var html = document.DocumentNode.SelectNodes("//div[@id='mtd_0']/div/div[@class='p10']").ElementAt(0).InnerHtml;
        var html = document.DocumentNode.SelectNodes("//div[@id='show-alter']").ElementAt(0).InnerHtml;
        return html;
      }
      catch (Exception e)
      {
        Debug.WriteLine("day la " + en);
        return "404 not found";
      }
    }
    //continue with current data
    private void button1_Click(object sender, EventArgs e)
    {
      renderTableData();
    }

    //reload all data
    private void button2_Click(object sender, EventArgs e)
    {
      firstLoadData();
    }

    //Add forget
    private void button3_Click(object sender, EventArgs e)
    {
      try
      {
        DataGridViewCellStyle CellStyle = new DataGridViewCellStyle();
        CellStyle.BackColor = Color.Blue;
        dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["EN"].Style = CellStyle;
        var en = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["EN"].Value.ToString();
        db.Words.Where(x => x.EN == en).First().IsForgeted = true;
        db.SaveChanges();
      }
      catch (Exception ex)
      {

      }
    }

    //Add common
    private void button4_Click(object sender, EventArgs e)
    {
      try
      {
        var en = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["EN"].Value.ToString();
        var word = db.Words.Where(x => x.EN == en).First();
        var common = new CommonWord { EN = word.EN };
        db.CommonWords.Add(common);
        db.Words.Remove(word);
        db.SaveChanges();
      }
      catch (Exception ex)
      {

      }

    }

    //show forget only
    private async void button5_Click(object sender, EventArgs e)
    {
      var list = db.Words.Select(x=>x.EN);
      var common = list.Concat(db.CommonWords.Select(x=>x.EN));
      List<Task<string>> tasks = new List<Task<string>>();
      string[] vi = new string[common.Count()];
      using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Public\word.txt"))
      {
        foreach(var c in common)
        {
          tasks.Add(Task.Run(() =>
          {
            return getTranslate(c);
          }));
        }
        vi = await Task.WhenAll(tasks);
        foreach(var c in common)
        {
          file.WriteLine(c);
        }
      }

      using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Public\vi.txt"))
      {
        foreach(var item in vi)
        {
          file.WriteLine(item);
        }
      }
        //var datasource = db.Words.Where(x => x.IsForgeted).ToList().Select((x, i) => new TableViewModel() { EN = x.EN, Order = i + 1 }).ToList();
        //dataGridView1.DataSource = datasource;
      }

      //show common only
      private void button6_Click(object sender, EventArgs e)
    {
      var datasource = db.CommonWords.Select((x, i) => new TableViewModel() { EN = x.EN, Order = i + 1 }).ToList();
      dataGridView1.DataSource = datasource;
    }

    //open choose file form
    private void button7_Click(object sender, EventArgs e)
    {
      DialogResult result = openFileDialog1.ShowDialog();
      if (result == DialogResult.OK)
      {
        filePath = openFileDialog1.FileName;
      }
    }

    //render form file or url
    private void button8_Click(object sender, EventArgs e)
    {
      var url = textBox1.Text;
      var page = textBox2.Text == null ? "" : textBox2.Text;
      var xPath = textBox3.Text;
      List<Word> words = new List<Word>();
      if (filePath != null)
      {
        var model = LoadDataFromFile(filePath, xPath);
        words.AddRange(model);
      }
      else
      {
        if (page == "")
        {
          var model = LoadDataFromWeb(url, xPath);
          words.AddRange(model);
        }
        else
        {
          var model = LoadDataFromWeb(url, page, xPath);
          words.AddRange(model);
        }
      }
      label3.Text = words.Count + " item";
      db.Words.AddRange(words);
      db.SaveChanges();
    }

    private void dataGridView1_SelectionChanged(object sender, EventArgs e)
    {
      //var en = dataGridView1.CurrentRow.Cells["EN"].Value.ToString();
      //var category = en.First();
      //PlayMp3FromUrl("http://tratu.coviet.vn/sounds/en/"+category+"/" + en + ".mp3");
    }

    //public static void PlayMp3FromUrl(string url)
    //{
    //  using (Stream ms = new MemoryStream())
    //  {
    //    using (Stream stream = WebRequest.Create(url)
    //        .GetResponse().GetResponseStream())
    //    {
    //      byte[] buffer = new byte[32768];
    //      int read;
    //      while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
    //      {
    //        ms.Write(buffer, 0, read);
    //      }
    //    }

    //    ms.Position = 0;
    //    using (WaveStream blockAlignedStream =
    //        new BlockAlignReductionStream(
    //            WaveFormatConversionStream.CreatePcmStream(
    //                new Mp3FileReader(ms))))
    //    {
    //      using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
    //      {
    //        waveOut.Init(blockAlignedStream);
    //        waveOut.Play();
    //        while (waveOut.PlaybackState == PlaybackState.Playing)
    //        {
    //          System.Threading.Thread.Sleep(100);
    //        }
    //      }
    //    }
    //  }
    //}


  }
}
